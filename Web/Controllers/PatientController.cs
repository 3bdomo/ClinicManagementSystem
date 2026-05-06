using AutoMapper;
using BLL.DTOs.Patient;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Web.ViewModel;

namespace Web.Controllers
{
    [Authorize]
    public class PatientController : Controller
    {
        private readonly IPatientService _patientService;
        private readonly IPatientAccountService _patientAccountService;
        private readonly IMapper _mapper;

        public PatientController(IPatientService patientService, IPatientAccountService patientAccountService, IMapper mapper)
        {
            _patientService = patientService;
            _patientAccountService = patientAccountService;
            _mapper = mapper;
        }

        [Authorize(Roles = "Admin,Receptionist,Doctor")]
        public async Task<IActionResult> Index(string? searchQuery = null, int page = 1, int pageSize = 10)
        {
            var result = string.IsNullOrEmpty(searchQuery) 
                ? await _patientService.GetAllAsync(page, pageSize)
                : await _patientService.SearchAsync(searchQuery);

            var vm = new PatientListViewModel
            {
                Patients = result.IsSuccess ? _mapper.Map<IEnumerable<PatientRowViewModel>>(result.Data) : new List<PatientRowViewModel>(),
                SearchQuery = searchQuery
            };
            return View(vm);
        }

        [Authorize(Roles = "Admin,Receptionist,Doctor")]
        public IActionResult Create()
        {
            ViewBag.BloodTypes = GetBloodTypes();
            return View(new PatientFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Receptionist,Doctor")]
        public async Task<IActionResult> Create(PatientFormViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.BloodTypes = GetBloodTypes();
                return View(vm);
            }

            var dto = _mapper.Map<PatientDto>(vm);
            var result = await _patientService.CreateAsync(dto);
            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Patient created successfully.";
                return RedirectToAction(nameof(Index));
            }
            
            ModelState.AddModelError(string.Empty, result.Message);
            ViewBag.BloodTypes = GetBloodTypes();
            return View(vm);
        }

        [Authorize(Roles = "Admin,Receptionist,Doctor")]
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _patientService.GetByIdAsync(id);
            if (!result.IsSuccess)
                return NotFound();

            var vm = _mapper.Map<PatientFormViewModel>(result.Data);
            ViewBag.BloodTypes = GetBloodTypes();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Receptionist,Doctor")]
        public async Task<IActionResult> Edit(PatientFormViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.BloodTypes = GetBloodTypes();
                return View(vm);
            }

            var dto = _mapper.Map<PatientDto>(vm);
            var result = await _patientService.UpdateAsync(dto);
            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Patient updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, result.Message);
            ViewBag.BloodTypes = GetBloodTypes();
            return View(vm);
        }

        [Authorize(Roles = "Admin,Receptionist,Doctor")]
        public async Task<IActionResult> Details(int id)
        {
            var result = await _patientService.GetFullHistoryAsync(id);
            if (!result.IsSuccess)
                return NotFound();

            var vm = _mapper.Map<PatientDetailsViewModel>(result.Data);
            return View(vm);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Receptionist,Doctor")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _patientService.DeleteAsync(id);
            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Patient deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Deleted()
        {
            var result = await _patientService.GetDeletedAsync();
            var vm = new DeletedPatientsViewModel
            {
                DeletedPatients = result.IsSuccess ? _mapper.Map<IEnumerable<PatientRowViewModel>>(result.Data) : new List<PatientRowViewModel>()
            };
            return View(vm);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(int id)
        {
            var result = await _patientService.RestoreAsync(id);
            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Patient restored successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }
            return RedirectToAction(nameof(Deleted));
        }

        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> MyProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var result = await _patientAccountService.GetMyProfileAsync(userId);
            if (result.IsSuccess)
            {
                var vm = _mapper.Map<PatientDetailsViewModel>(result.Data);
                return View(vm);
            }
            return NotFound(result.Message);
        }

        private List<SelectListItem> GetBloodTypes()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Text = "A+", Value = "A+" },
                new SelectListItem { Text = "A-", Value = "A-" },
                new SelectListItem { Text = "B+", Value = "B+" },
                new SelectListItem { Text = "B-", Value = "B-" },
                new SelectListItem { Text = "AB+", Value = "AB+" },
                new SelectListItem { Text = "AB-", Value = "AB-" },
                new SelectListItem { Text = "O+", Value = "O+" },
                new SelectListItem { Text = "O-", Value = "O-" }
            };
        }
    }
}
