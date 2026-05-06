using AutoMapper;
using BLL.DTOs.Appointment;
using BLL.Interfaces;
using Common.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Web.ViewModel;
using Web.ViewModels.Appointment;

namespace Web.Controllers
{
    [Authorize]
    public class AppointmentController : Controller
    {
        private const int MaxPatientAdvanceBookingDays = 30;

        private readonly IAppointmentService _appointmentService;
        private readonly IDoctorService _doctorService;
        private readonly IDoctorScheduleService _doctorScheduleService;
        private readonly IPatientService _patientService;
        private readonly IMapper _mapper;

        public AppointmentController(
            IAppointmentService appointmentService,
            IDoctorService doctorService,
            IDoctorScheduleService doctorScheduleService,
            IPatientService patientService,
            IMapper mapper)
        {
            _appointmentService = appointmentService;
            _doctorService = doctorService;
            _doctorScheduleService = doctorScheduleService;
            _patientService = patientService;
            _mapper = mapper;
        }

        // ─────────────────────────────────────────────────────────
        // INDEX
        // Admin / Receptionist: see all appointments for selected day.
        // Doctor: sees own appointments only.
        // Patient: sees own appointments only.
        // ─────────────────────────────────────────────────────────
        [Authorize(Roles = "Admin,Receptionist,Doctor,Patient")]
        public async Task<IActionResult> Index(
            DateTime? date,
            int? doctorId,
            AppointmentStatus? status,
            AppointmentType? type,
            int month = 0,
            int year = 0)
        {
            var selectedDate = date?.Date ?? DateTime.Today;
            var calendarMonth = month > 0 ? month : selectedDate.Month;
            var calendarYear = year > 0 ? year : selectedDate.Year;

            var dayRows = new List<AppointmentRowViewModel>();
            var busyDays = new List<DateTime>();

            // Doctor view: only current doctor's appointments.
            if (User.IsInRole(nameof(UserRole.Doctor)))
            {
                var currentDoctorId = await GetCurrentDoctorIdAsync();
                if (!currentDoctorId.HasValue)
                    return RedirectToAction("AccessDenied", "Account");

                var result = await _appointmentService.GetDoctorAppointmentsByDateAsync(
                    currentDoctorId.Value,
                    selectedDate);

                var appointments = result.IsSuccess
                    ? result.Data!.ToList()
                    : new List<AppointmentDto>();

                dayRows = _mapper.Map<List<AppointmentRowViewModel>>(appointments);

                var allDoctorAppointments = await _appointmentService.GetDoctorAppointmentsAsync(
                    currentDoctorId.Value);

                if (allDoctorAppointments.IsSuccess)
                {
                    busyDays = allDoctorAppointments.Data!
                        .Where(a => a.AppointmentDate.Month == calendarMonth
                                 && a.AppointmentDate.Year == calendarYear)
                        .Select(a => a.AppointmentDate.Date)
                        .Distinct()
                        .ToList();
                }
            }
            // Patient view: only current patient's appointments.
            else if (User.IsInRole(nameof(UserRole.Patient)))
            {
                var currentPatientId = await GetCurrentPatientIdAsync();
                if (!currentPatientId.HasValue)
                    return RedirectToAction("AccessDenied", "Account");

                var result = await _appointmentService.GetPatientHistoryAsync(
                    currentPatientId.Value);

                var history = result.IsSuccess
                    ? result.Data!.ToList()
                    : new List<AppointmentHistoryDto>();

                dayRows = history
                    .Where(a => a.AppointmentDate.Date == selectedDate.Date)
                    .Select(a => new AppointmentRowViewModel
                    {
                        Id = a.Id,
                        PatientId = currentPatientId.Value,
                        PatientName = User.Identity?.Name ?? "Current patient",
                        DoctorName = a.DoctorName ?? "-",
                        AppointmentDate = a.AppointmentDate,
                        DurationMinutes = a.DurationMinutes,
                        AppointmentType = a.AppointmentType,
                        Status = a.Status,
                        Notes = a.Notes,
                        HasMedicalRecord = a.HasMedicalRecord,
                        HasInvoice = a.HasInvoice
                    })
                    .ToList();

                busyDays = history
                    .Where(a => a.AppointmentDate.Month == calendarMonth
                             && a.AppointmentDate.Year == calendarYear)
                    .Select(a => a.AppointmentDate.Date)
                    .Distinct()
                    .ToList();
            }
            // Admin / Receptionist view: all appointments for the selected date.
            else
            {
                var result = await _appointmentService.GetByDateAsync(selectedDate);

                var appointments = result.IsSuccess
                    ? result.Data!.ToList()
                    : new List<AppointmentDto>();

                if (doctorId.HasValue)
                {
                    appointments = appointments
                        .Where(a => a.DoctorId == doctorId.Value)
                        .ToList();
                }

                dayRows = _mapper.Map<List<AppointmentRowViewModel>>(appointments);

                var allResult = await _appointmentService.GetAllAsync();

                if (allResult.IsSuccess)
                {
                    busyDays = allResult.Data!
                        .Where(a => a.AppointmentDate.Month == calendarMonth
                                 && a.AppointmentDate.Year == calendarYear)
                        .Select(a => a.AppointmentDate.Date)
                        .Distinct()
                        .ToList();
                }
            }

            // Common filters for all roles.
            if (status.HasValue)
                dayRows = dayRows.Where(a => a.Status == status.Value).ToList();

            if (type.HasValue)
                dayRows = dayRows.Where(a => a.AppointmentType == type.Value).ToList();

            dayRows = dayRows
                .OrderBy(a => a.AppointmentDate)
                .ToList();

            // Doctors dropdown only for Admin / Receptionist.
            var doctors = new List<BLL.DTOs.Patient.DoctorDto>();

            if (User.IsInRole(nameof(UserRole.Admin)) ||
                User.IsInRole(nameof(UserRole.Receptionist)))
            {
                var doctorsResult = await _doctorService.GetAllAsync(1, 1000);
                if (doctorsResult.IsSuccess)
                    doctors = doctorsResult.Data!.ToList();
            }

            var vm = new AppointmentIndexViewModel
            {
                DayAppointments = dayRows,
                SelectedDate = selectedDate,
                CalendarMonth = calendarMonth,
                CalendarYear = calendarYear,
                BusyDays = busyDays,
                FilterDoctorId = doctorId,
                FilterStatus = status,
                FilterType = type,
                Doctors = doctors,

                TotalToday = dayRows.Count,
                WaitingCount = dayRows.Count(a => a.Status == AppointmentStatus.Waiting),
                InProgressCount = dayRows.Count(a => a.Status == AppointmentStatus.InProgress),
                CompletedCount = dayRows.Count(a => a.Status == AppointmentStatus.Completed)
            };

            return View(vm);
        }

        // ─────────────────────────────────────────────────────────
        // DETAILS
        // Shows appointment details and calculates available actions.
        // ─────────────────────────────────────────────────────────
        [Authorize(Roles = "Admin,Receptionist,Doctor,Patient")]
        public async Task<IActionResult> Details(int id)
        {
            var result = await _appointmentService.GetByIdAsync(id);

            if (!result.IsSuccess)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            var appointment = result.Data!;

            var accessAllowed = await CanCurrentUserAccessAppointmentAsync(appointment);
            if (!accessAllowed)
                return RedirectToAction("AccessDenied", "Account");

            var vm = _mapper.Map<AppointmentDetailsViewModel>(appointment);

            vm.CanEdit =
                (User.IsInRole(nameof(UserRole.Admin)) ||
                 User.IsInRole(nameof(UserRole.Receptionist)))
                && appointment.Status == AppointmentStatus.Waiting;

            vm.CanCancel =
                (User.IsInRole(nameof(UserRole.Admin)) ||
                 User.IsInRole(nameof(UserRole.Receptionist)) ||
                 User.IsInRole(nameof(UserRole.Patient)))
                && (appointment.Status == AppointmentStatus.Waiting ||
                    appointment.Status == AppointmentStatus.InProgress);

            vm.CanStart =
                User.IsInRole(nameof(UserRole.Doctor))
                && appointment.Status == AppointmentStatus.Waiting;

            vm.CanComplete =
                User.IsInRole(nameof(UserRole.Doctor))
                && appointment.Status == AppointmentStatus.InProgress;

            vm.CanDelete =
                User.IsInRole(nameof(UserRole.Admin))
                && appointment.Status != AppointmentStatus.InProgress
                && appointment.Status != AppointmentStatus.Completed;

            return View(vm);
        }

        // ─────────────────────────────────────────────────────────
        // BOOK STEP 1 - GET
        // Select doctor, and patient if Admin/Receptionist.
        // ─────────────────────────────────────────────────────────
        [Authorize(Roles = "Admin,Receptionist,Patient")]
        public async Task<IActionResult> BookStep1(Specialization? specialization)
        {
            var vm = await BuildBookStep1ViewModelAsync(specialization);
            return View(vm);
        }

        // ─────────────────────────────────────────────────────────
        // BOOK STEP 1 - POST
        // Validates doctor/patient selection, then redirects to Step 2.
        // ─────────────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Receptionist,Patient")]
        public async Task<IActionResult> BookStep1(BookStep1ViewModel vm)
        {
            if (User.IsInRole(nameof(UserRole.Patient)))
            {
                var patientId = await GetCurrentPatientIdAsync();
                if (!patientId.HasValue)
                    return RedirectToAction("AccessDenied", "Account");

                vm.SelectedPatientId = patientId.Value;
            }
            else
            {
                if (!vm.SelectedPatientId.HasValue)
                    ModelState.AddModelError(nameof(vm.SelectedPatientId), "Please select a patient.");
            }

            if (!ModelState.IsValid)
            {
                var reloaded = await BuildBookStep1ViewModelAsync(vm.FilterSpecialization);
                reloaded.SelectedDoctorId = vm.SelectedDoctorId;
                reloaded.SelectedPatientId = vm.SelectedPatientId;
                return View(reloaded);
            }

            return RedirectToAction(nameof(BookStep2), new
            {
                doctorId = vm.SelectedDoctorId!.Value,
                patientId = vm.SelectedPatientId
            });
        }

        // ─────────────────────────────────────────────────────────
        // BOOK STEP 2 - GET
        // Select date and schedule type.
        // ─────────────────────────────────────────────────────────
        [Authorize(Roles = "Admin,Receptionist,Patient")]
        public async Task<IActionResult> BookStep2(int doctorId, int? patientId)
        {
            var resolvedPatientId = await ResolvePatientIdForBookingAsync(patientId);
            if (!resolvedPatientId.HasValue)
                return RedirectToAction("AccessDenied", "Account");

            var vm = await BuildBookStep2ViewModelAsync(
                doctorId,
                resolvedPatientId.Value);

            if (vm == null)
            {
                TempData["Error"] = "Doctor or patient was not found.";
                return RedirectToAction(nameof(BookStep1));
            }

            return View(vm);
        }

        // ─────────────────────────────────────────────────────────
        // BOOK STEP 2 - POST
        // Validates selected date/type, then redirects to Step 3.
        // ─────────────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Receptionist,Patient")]
        public async Task<IActionResult> BookStep2(BookStep2ViewModel vm)
        {
            var resolvedPatientId = await ResolvePatientIdForBookingAsync(vm.PatientId);
            if (!resolvedPatientId.HasValue)
                return RedirectToAction("AccessDenied", "Account");

            vm.PatientId = resolvedPatientId.Value;

            if (User.IsInRole(nameof(UserRole.Patient)))
            {
                // Patient self-booking is consultation only.
                vm.SelectedScheduleType = ScheduleType.Consultation;
            }

            ValidateBookingDate(vm.SelectedDate);

            if (!ModelState.IsValid)
            {
                var reloaded = await BuildBookStep2ViewModelAsync(
                    vm.DoctorId,
                    vm.PatientId.Value);

                if (reloaded == null)
                    return RedirectToAction(nameof(BookStep1));

                reloaded.SelectedDate = vm.SelectedDate;
                reloaded.SelectedScheduleType = vm.SelectedScheduleType;

                return View(reloaded);
            }

            return RedirectToAction(nameof(BookStep3), new
            {
                doctorId = vm.DoctorId,
                patientId = vm.PatientId,
                date = vm.SelectedDate!.Value.Date,
                scheduleType = vm.SelectedScheduleType
            });
        }

        // ─────────────────────────────────────────────────────────
        // BOOK STEP 3 - GET
        // Shows available slots for the selected date/type.
        // ─────────────────────────────────────────────────────────
        [Authorize(Roles = "Admin,Receptionist,Patient")]
        public async Task<IActionResult> BookStep3(
            int doctorId,
            int? patientId,
            DateTime date,
            ScheduleType scheduleType)
        {
            var resolvedPatientId = await ResolvePatientIdForBookingAsync(patientId);
            if (!resolvedPatientId.HasValue)
                return RedirectToAction("AccessDenied", "Account");

            if (User.IsInRole(nameof(UserRole.Patient)))
                scheduleType = ScheduleType.Consultation;

            var vm = await BuildBookStep3ViewModelAsync(
                doctorId,
                resolvedPatientId.Value,
                date,
                scheduleType);

            if (vm == null)
            {
                TempData["Error"] = "No available slots for the selected date.";
                return RedirectToAction(nameof(BookStep2), new
                {
                    doctorId,
                    patientId = resolvedPatientId.Value
                });
            }

            return View(vm);
        }

        // ─────────────────────────────────────────────────────────
        // BOOK STEP 3 - POST
        // Validates selected slot, then redirects to confirmation page.
        // ─────────────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Receptionist,Patient")]
        public async Task<IActionResult> BookStep3(BookStep3ViewModel vm)
        {
            var resolvedPatientId = await ResolvePatientIdForBookingAsync(vm.PatientId);
            if (!resolvedPatientId.HasValue)
                return RedirectToAction("AccessDenied", "Account");

            vm.PatientId = resolvedPatientId.Value;

            if (User.IsInRole(nameof(UserRole.Patient)))
                vm.ScheduleType = ScheduleType.Consultation;

            if (!vm.SelectedSlotStart.HasValue)
            {
                ModelState.AddModelError(
                    nameof(vm.SelectedSlotStart),
                    "Please select a time slot.");
            }

            if (vm.SelectedSlotStart.HasValue &&
                vm.SelectedSlotStart.Value <= DateTime.Now)
            {
                ModelState.AddModelError(
                    nameof(vm.SelectedSlotStart),
                    "Selected slot must be in the future.");
            }

            var availableSlotsResult = await _doctorScheduleService.GetAvailableSlotsAsync(
                vm.DoctorId,
                vm.SelectedDate,
                vm.ScheduleType);

            var availableSlots = availableSlotsResult.IsSuccess
                ? availableSlotsResult.Data!
                    .Where(s => s.SlotStart > DateTime.Now)
                    .ToList()
                : new List<BLL.DTOs.Doctor.TimeSlotDto>();

            var selectedSlot = availableSlots.FirstOrDefault(s =>
                vm.SelectedSlotStart.HasValue &&
                Math.Abs((s.SlotStart - vm.SelectedSlotStart.Value).TotalSeconds) < 1);

            if (selectedSlot == null)
            {
                ModelState.AddModelError(
                    nameof(vm.SelectedSlotStart),
                    "Selected slot is no longer available.");
            }

            if (!ModelState.IsValid)
            {
                var reloaded = await BuildBookStep3ViewModelAsync(
                    vm.DoctorId,
                    vm.PatientId.Value,
                    vm.SelectedDate,
                    vm.ScheduleType);

                if (reloaded == null)
                {
                    return RedirectToAction(nameof(BookStep2), new
                    {
                        doctorId = vm.DoctorId,
                        patientId = vm.PatientId
                    });
                }

                reloaded.SelectedSlotStart = vm.SelectedSlotStart;

                return View(reloaded);
            }

            return RedirectToAction(nameof(BookConfirm), new
            {
                doctorId = vm.DoctorId,
                patientId = vm.PatientId,
                appointmentDate = selectedSlot!.SlotStart,
                durationMinutes = selectedSlot.SlotMinutes,
                scheduleType = vm.ScheduleType
            });
        }

        // ─────────────────────────────────────────────────────────
        // BOOK CONFIRM - GET
        // Shows final booking confirmation before saving.
        // ─────────────────────────────────────────────────────────
        [Authorize(Roles = "Admin,Receptionist,Patient")]
        public async Task<IActionResult> BookConfirm(
            int doctorId,
            int? patientId,
            DateTime appointmentDate,
            int durationMinutes,
            ScheduleType scheduleType)
        {
            var resolvedPatientId = await ResolvePatientIdForBookingAsync(patientId);
            if (!resolvedPatientId.HasValue)
                return RedirectToAction("AccessDenied", "Account");

            if (User.IsInRole(nameof(UserRole.Patient)))
                scheduleType = ScheduleType.Consultation;

            var doctorResult = await _doctorService.GetByIdAsync(doctorId);
            var patientResult = await _patientService.GetByIdAsync(resolvedPatientId.Value);

            if (!doctorResult.IsSuccess || !patientResult.IsSuccess)
            {
                TempData["Error"] = "Doctor or patient was not found.";
                return RedirectToAction(nameof(BookStep1));
            }

            var appointmentType = MapScheduleTypeToAppointmentType(scheduleType);

            var vm = new BookConfirmViewModel
            {
                DoctorId = doctorId,
                PatientId = resolvedPatientId.Value,
                AppointmentDate = appointmentDate,
                DurationMinutes = durationMinutes,
                ScheduleType = scheduleType,
                AppointmentType = appointmentType,
                DoctorName = doctorResult.Data!.FullName,
                PatientName = patientResult.Data!.FullName,
                DoctorSpecialization = doctorResult.Data.Specialization,
                ConsultationFee = doctorResult.Data.ConsultationFee ?? 0
            };

            return View(vm);
        }

        // ─────────────────────────────────────────────────────────
        // BOOK CONFIRM - POST
        // Saves appointment using CreateAppointmentDto.
        // ─────────────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Receptionist,Patient")]
        public async Task<IActionResult> BookConfirm(BookConfirmViewModel vm)
        {
            var resolvedPatientId = await ResolvePatientIdForBookingAsync(vm.PatientId);
            if (!resolvedPatientId.HasValue)
                return RedirectToAction("AccessDenied", "Account");

            vm.PatientId = resolvedPatientId.Value;

            if (User.IsInRole(nameof(UserRole.Patient)))
                vm.ScheduleType = ScheduleType.Consultation;

            vm.AppointmentType = MapScheduleTypeToAppointmentType(vm.ScheduleType);

            if (!ModelState.IsValid)
            {
                await PopulateBookConfirmDisplayDataAsync(vm);
                return View(vm);
            }

            var dto = _mapper.Map<CreateAppointmentDto>(vm);

            var result = await _appointmentService.CreateAsync(dto);

            if (!result.IsSuccess)
            {
                TempData["Error"] = result.Message;
                await PopulateBookConfirmDisplayDataAsync(vm);
                return View(vm);
            }

            TempData["Success"] = "Appointment booked successfully.";
            return RedirectToAction(nameof(Details), new { id = result.Data });
        }

        // ─────────────────────────────────────────────────────────
        // EDIT - GET
        // Admin/Receptionist can edit waiting appointments only.
        // ─────────────────────────────────────────────────────────
        [Authorize(Roles = "Admin,Receptionist")]
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _appointmentService.GetByIdAsync(id);

            if (!result.IsSuccess)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            var appointment = result.Data!;

            if (appointment.Status != AppointmentStatus.Waiting)
            {
                TempData["Error"] = "Only waiting appointments can be edited.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var vm = _mapper.Map<EditAppointmentViewModel>(appointment);

            return View(vm);
        }

        // ─────────────────────────────────────────────────────────
        // EDIT - POST
        // Updates appointment date/time and notes.
        // ─────────────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Receptionist")]
        public async Task<IActionResult> Edit(EditAppointmentViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                await ReloadEditDisplayDataAsync(vm);
                return View(vm);
            }

            var dto = _mapper.Map<UpdateAppointmentDto>(vm);

            var result = await _appointmentService.UpdateAsync(dto);

            if (!result.IsSuccess)
            {
                TempData["Error"] = result.Message;
                await ReloadEditDisplayDataAsync(vm);
                return View(vm);
            }

            TempData["Success"] = "Appointment updated successfully.";
            return RedirectToAction(nameof(Details), new { id = vm.Id });
        }

        // ─────────────────────────────────────────────────────────
        // CANCEL - GET
        // Shows cancellation form.
        // ─────────────────────────────────────────────────────────
        [Authorize(Roles = "Admin,Receptionist,Patient")]
        public async Task<IActionResult> Cancel(int id)
        {
            var result = await _appointmentService.GetByIdAsync(id);

            if (!result.IsSuccess)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            var appointment = result.Data!;

            var accessAllowed = await CanCurrentUserAccessAppointmentAsync(appointment);
            if (!accessAllowed)
                return RedirectToAction("AccessDenied", "Account");

            if (appointment.Status == AppointmentStatus.Completed ||
                appointment.Status == AppointmentStatus.Cancelled)
            {
                TempData["Error"] = "This appointment cannot be cancelled.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var vm = _mapper.Map<CancelAppointmentViewModel>(appointment);

            return View(vm);
        }

        // ─────────────────────────────────────────────────────────
        // CANCEL - POST
        // Cancels appointment after ownership/role validation.
        // ─────────────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Receptionist,Patient")]
        public async Task<IActionResult> Cancel(CancelAppointmentViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var appointmentResult = await _appointmentService.GetByIdAsync(vm.AppointmentId);

            if (!appointmentResult.IsSuccess)
            {
                TempData["Error"] = appointmentResult.Message;
                return RedirectToAction(nameof(Index));
            }

            var accessAllowed = await CanCurrentUserAccessAppointmentAsync(appointmentResult.Data!);
            if (!accessAllowed)
                return RedirectToAction("AccessDenied", "Account");

            var result = await _appointmentService.CancelAppointmentAsync(
                vm.AppointmentId,
                vm.CancellationReason);

            if (!result.IsSuccess)
            {
                TempData["Error"] = result.Message;

                vm.DoctorName = appointmentResult.Data!.DoctorName ?? "-";
                vm.PatientName = appointmentResult.Data.PatientName ?? "-";
                vm.AppointmentDate = appointmentResult.Data.AppointmentDate;

                return View(vm);
            }

            TempData["Success"] = "Appointment cancelled successfully.";
            return RedirectToAction(nameof(Index));
        }

        // ─────────────────────────────────────────────────────────
        // START
        // Doctor starts a waiting appointment.
        // ─────────────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> Start(int id)
        {
            var result = await _appointmentService.GetByIdAsync(id);

            if (!result.IsSuccess)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            var accessAllowed = await CanCurrentUserAccessAppointmentAsync(result.Data!);
            if (!accessAllowed)
                return RedirectToAction("AccessDenied", "Account");

            var startResult = await _appointmentService.StartAppointmentAsync(id);

            TempData[startResult.IsSuccess ? "Success" : "Error"] =
                startResult.IsSuccess
                    ? "Appointment started successfully."
                    : startResult.Message;

            return RedirectToAction(nameof(Details), new { id });
        }

        // ─────────────────────────────────────────────────────────
        // COMPLETE
        // Doctor completes an in-progress appointment.
        // ─────────────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> Complete(int id)
        {
            var result = await _appointmentService.GetByIdAsync(id);

            if (!result.IsSuccess)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            var accessAllowed = await CanCurrentUserAccessAppointmentAsync(result.Data!);
            if (!accessAllowed)
                return RedirectToAction("AccessDenied", "Account");

            var completeResult = await _appointmentService.CompleteAppointmentAsync(id);

            TempData[completeResult.IsSuccess ? "Success" : "Error"] =
                completeResult.IsSuccess
                    ? "Appointment completed successfully."
                    : completeResult.Message;

            return RedirectToAction(nameof(Details), new { id });
        }

        // ─────────────────────────────────────────────────────────
        // DELETE
        // Admin hard-deletes appointment for data entry mistakes.
        // Service prevents deleting InProgress or Completed appointments.
        // ─────────────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _appointmentService.DeleteAsync(id);

            TempData[result.IsSuccess ? "Success" : "Error"] =
                result.IsSuccess
                    ? "Appointment deleted successfully."
                    : result.Message;

            return RedirectToAction(nameof(Index));
        }

        // ─────────────────────────────────────────────────────────
        // AJAX: GET AVAILABLE SLOTS
        // Used by Edit page or dynamic slot loading.
        // ─────────────────────────────────────────────────────────
        [HttpGet]
        [Authorize(Roles = "Admin,Receptionist,Patient")]
        public async Task<IActionResult> GetSlots(
            int doctorId,
            DateTime date,
            ScheduleType scheduleType)
        {
            if (User.IsInRole(nameof(UserRole.Patient)))
                scheduleType = ScheduleType.Consultation;

            var result = await _doctorScheduleService.GetAvailableSlotsAsync(
                doctorId,
                date,
                scheduleType);

            if (!result.IsSuccess)
            {
                return Json(new
                {
                    success = false,
                    message = result.Message
                });
            }

            var slots = result.Data!
                .Where(s => s.SlotStart > DateTime.Now)
                .Select(s => new
                {
                    start = s.SlotStart.ToString("HH:mm"),
                    end = s.SlotEnd.ToString("HH:mm"),
                    isoStart = s.SlotStart.ToString("o"),
                    minutes = s.SlotMinutes
                });

            return Json(new
            {
                success = true,
                slots
            });
        }

        // ─────────────────────────────────────────────────────────
        // PRIVATE HELPERS
        // ─────────────────────────────────────────────────────────

        // Builds Step 1 view model with doctors and optional patients.
        private async Task<BookStep1ViewModel> BuildBookStep1ViewModelAsync(
            Specialization? specialization)
        {
            var doctorsResult = await _doctorService.GetAllAsync(1, 1000);
            var doctors = doctorsResult.IsSuccess
                ? doctorsResult.Data!.ToList()
                : new List<BLL.DTOs.Patient.DoctorDto>();

            if (specialization.HasValue)
            {
                doctors = doctors
                    .Where(d => d.Specialization == specialization.Value)
                    .ToList();
            }

            var patients = new List<BLL.DTOs.Patient.PatientDto>();
            var canSelectPatient =
                User.IsInRole(nameof(UserRole.Admin)) ||
                User.IsInRole(nameof(UserRole.Receptionist));

            if (canSelectPatient)
            {
                var patientsResult = await _patientService.GetAllAsync();
                if (patientsResult.IsSuccess)
                    patients = patientsResult.Data!.ToList();
            }

            return new BookStep1ViewModel
            {
                Doctors = doctors,
                Patients = patients,
                FilterSpecialization = specialization,
                CanSelectPatient = canSelectPatient
            };
        }

        // Builds Step 2 view model with doctor, patient and schedules.
        private async Task<BookStep2ViewModel?> BuildBookStep2ViewModelAsync(
            int doctorId,
            int patientId)
        {
            var doctorResult = await _doctorService.GetByIdAsync(doctorId);
            var patientResult = await _patientService.GetByIdAsync(patientId);

            if (!doctorResult.IsSuccess || !patientResult.IsSuccess)
                return null;

            var schedulesResult = await _doctorScheduleService.GetByDoctorAsync(doctorId);

            var schedules = schedulesResult.IsSuccess
                ? schedulesResult.Data!.ToList()
                : new List<BLL.DTOs.Doctor.DoctorScheduleDto>();

            return new BookStep2ViewModel
            {
                DoctorId = doctorId,
                PatientId = patientId,
                PatientName = patientResult.Data!.FullName,
                DoctorName = doctorResult.Data!.FullName,
                DoctorSpecialization = doctorResult.Data.Specialization,
                DoctorBio = doctorResult.Data.Bio,
                ConsultationFee = doctorResult.Data.ConsultationFee ?? 0,
                Schedules = schedules,
                SelectedScheduleType = ScheduleType.Consultation
            };
        }

        // Builds Step 3 view model with available slots.
        private async Task<BookStep3ViewModel?> BuildBookStep3ViewModelAsync(
            int doctorId,
            int patientId,
            DateTime date,
            ScheduleType scheduleType)
        {
            var doctorResult = await _doctorService.GetByIdAsync(doctorId);
            var patientResult = await _patientService.GetByIdAsync(patientId);

            if (!doctorResult.IsSuccess || !patientResult.IsSuccess)
                return null;

            var slotsResult = await _doctorScheduleService.GetAvailableSlotsAsync(
                doctorId,
                date,
                scheduleType);

            if (!slotsResult.IsSuccess || !slotsResult.Data!.Any())
                return null;

            var slots = slotsResult.Data!
                .Where(s => s.SlotStart > DateTime.Now)
                .ToList();

            if (!slots.Any())
                return null;

            return new BookStep3ViewModel
            {
                DoctorId = doctorId,
                PatientId = patientId,
                PatientName = patientResult.Data!.FullName,
                DoctorName = doctorResult.Data!.FullName,
                DoctorSpecialization = doctorResult.Data.Specialization,
                ConsultationFee = doctorResult.Data.ConsultationFee ?? 0,
                SelectedDate = date.Date,
                ScheduleType = scheduleType,
                MorningSlots = slots.Where(s => s.SlotStart.Hour < 12).ToList(),
                AfternoonSlots = slots.Where(s => s.SlotStart.Hour >= 12).ToList(),
                SlotDurationMinutes = slots.First().SlotMinutes
            };
        }

        // Reloads display-only data for BookConfirm POST failures.
        private async Task PopulateBookConfirmDisplayDataAsync(BookConfirmViewModel vm)
        {
            var doctorResult = await _doctorService.GetByIdAsync(vm.DoctorId);
            var patientResult = await _patientService.GetByIdAsync(vm.PatientId);

            if (doctorResult.IsSuccess && doctorResult.Data != null)
            {
                vm.DoctorName = doctorResult.Data.FullName;
                vm.DoctorSpecialization = doctorResult.Data.Specialization;
                vm.ConsultationFee = doctorResult.Data.ConsultationFee ?? 0;
            }

            if (patientResult.IsSuccess && patientResult.Data != null)
            {
                vm.PatientName = patientResult.Data.FullName;
            }
        }

        // Reloads display-only data for Edit POST failures.
        private async Task ReloadEditDisplayDataAsync(EditAppointmentViewModel vm)
        {
            var result = await _appointmentService.GetByIdAsync(vm.Id);

            if (!result.IsSuccess || result.Data == null)
                return;

            var appointment = result.Data;

            vm.DoctorId = appointment.DoctorId;
            vm.DoctorName = appointment.DoctorName ?? "-";
            vm.PatientName = appointment.PatientName ?? "-";
            vm.CurrentStatus = appointment.Status;
            vm.AppointmentType = appointment.AppointmentType;
            vm.ScheduleType = appointment.AppointmentType == AppointmentType.Surgery
                ? ScheduleType.Surgery
                : ScheduleType.Consultation;
        }

        // Gets current logged-in Doctor.Id.
        private async Task<int?> GetCurrentDoctorIdAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return null;

            var doctorsResult = await _doctorService.GetAllAsync(1, 1000);
            if (!doctorsResult.IsSuccess)
                return null;

            return doctorsResult.Data!
                .FirstOrDefault(d => d.ApplicationUserId == userId)
                ?.Id;
        }

        // Gets current logged-in Patient.Id.
        private async Task<int?> GetCurrentPatientIdAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
                return null;

            var result = await _patientService.GetPatientIdByApplicationUserIdAsync(userId);

            if (!result.IsSuccess)
                return null;

            return result.Data;
        }

        // Resolves patient id for booking based on role.
        private async Task<int?> ResolvePatientIdForBookingAsync(int? postedPatientId)
        {
            if (User.IsInRole(nameof(UserRole.Patient)))
                return await GetCurrentPatientIdAsync();

            if (User.IsInRole(nameof(UserRole.Admin)) ||
                User.IsInRole(nameof(UserRole.Receptionist)))
                return postedPatientId;

            return null;
        }

        // Ensures doctor/patient can only access their own appointments.
        private async Task<bool> CanCurrentUserAccessAppointmentAsync(AppointmentDto appointment)
        {
            if (User.IsInRole(nameof(UserRole.Admin)) ||
                User.IsInRole(nameof(UserRole.Receptionist)))
                return true;

            if (User.IsInRole(nameof(UserRole.Doctor)))
            {
                var doctorId = await GetCurrentDoctorIdAsync();
                return doctorId.HasValue && appointment.DoctorId == doctorId.Value;
            }

            if (User.IsInRole(nameof(UserRole.Patient)))
            {
                var patientId = await GetCurrentPatientIdAsync();
                return patientId.HasValue && appointment.PatientId == patientId.Value;
            }

            return false;
        }

        // Validates booking date rules.
        private void ValidateBookingDate(DateTime? selectedDate)
        {
            if (!selectedDate.HasValue)
            {
                ModelState.AddModelError(
                    nameof(BookStep2ViewModel.SelectedDate),
                    "Please select a date.");
                return;
            }

            if (selectedDate.Value.Date < DateTime.Today)
            {
                ModelState.AddModelError(
                    nameof(BookStep2ViewModel.SelectedDate),
                    "Selected date cannot be in the past.");
                return;
            }

            if (User.IsInRole(nameof(UserRole.Patient)) &&
                selectedDate.Value.Date > DateTime.Today.AddDays(MaxPatientAdvanceBookingDays))
            {
                ModelState.AddModelError(
                    nameof(BookStep2ViewModel.SelectedDate),
                    $"Patients cannot book more than {MaxPatientAdvanceBookingDays} days in advance.");
            }
        }

        // Converts ScheduleType to AppointmentType.
        private static AppointmentType MapScheduleTypeToAppointmentType(ScheduleType scheduleType)
        {
            return scheduleType == ScheduleType.Surgery
                ? AppointmentType.Surgery
                : AppointmentType.Consultation;
        }
    }
}