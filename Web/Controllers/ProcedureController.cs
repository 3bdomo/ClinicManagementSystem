using AutoMapper;
using BLL.DTOs.Procedure;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Web.ViewModel;

namespace Web.Controllers
{
    [Authorize(Roles = "Admin,Doctor,Receptionist")]
    public class ProcedureController : Controller
    {
        private readonly IProcedureService _procedureService;
        private readonly IMapper _mapper;

        public ProcedureController(IProcedureService procedureService, IMapper mapper)
        {
            _procedureService = procedureService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> ManageTypes()
        {
            var typesResult = await _procedureService.GetAllTypesAsync();
            var viewModel = new ProcedureTypeListViewModel();

            if (typesResult.IsSuccess)
            {
                viewModel.Types = _mapper.Map<IEnumerable<ProcedureTypeFormViewModel>>(typesResult.Data);
                viewModel.ActiveTypesCount = typesResult.Data.Count(t => t.IsActive);
                viewModel.TotalProceduresToday = 0; 
                viewModel.TotalRevenueToday = 0;
            }

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult CreateType() => View(new ProcedureTypeFormViewModel());

        [HttpPost]
        public async Task<IActionResult> CreateType(ProcedureTypeFormViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var dto = _mapper.Map<CreateProcedureTypeDto>(model);
            var result = await _procedureService.CreateTypeAsync(dto);

            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", result.Message ?? "Failed to create type.");
                return View(model);
            }

            return RedirectToAction(nameof(ManageTypes));
        }

        [HttpGet]
        public async Task<IActionResult> EditType(int id)
        {
            var result = await _procedureService.GetTypeByIdAsync(id);
            if (!result.IsSuccess) return NotFound();

            var vm = _mapper.Map<ProcedureTypeFormViewModel>(result.Data);
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> EditType(int id, ProcedureTypeFormViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var dto = _mapper.Map<UpdateProcedureTypeDto>(model);
            var result = await _procedureService.UpdateTypeAsync(dto);

            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", result.Message ?? "Failed to update type.");
                return View(model);
            }

            return RedirectToAction(nameof(ManageTypes));
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var vm = new ProcedureFormViewModel();
            await PopulateDropdowns(vm);
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProcedureFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdowns(model);
                return View(model);
            }

            var dto = _mapper.Map<CreateProcedureDto>(model);
            var result = await _procedureService.CreateAsync(dto);

            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", result.Message ?? "Failed to create procedure.");
                await PopulateDropdowns(model);
                return View(model);
            }

            return RedirectToAction("Details", "MedicalRecord", new { id = model.MedicalRecordId });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _procedureService.GetByIdAsync(id);
            if (!result.IsSuccess) return NotFound();

            var vm = _mapper.Map<ProcedureFormViewModel>(result.Data);
            await PopulateDropdowns(vm);
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, ProcedureFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdowns(model);
                return View(model);
            }

            var dto = _mapper.Map<UpdateProcedureDto>(model);
            var result = await _procedureService.UpdateAsync(dto);

            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", result.Message ?? "Failed to update procedure.");
                await PopulateDropdowns(model);
                return View(model);
            }

            return RedirectToAction(nameof(Details), new { id = model.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var result = await _procedureService.GetByIdAsync(id);
            if (!result.IsSuccess) return NotFound();

            var vm = _mapper.Map<ProcedureDetailsViewModel>(result.Data);
            return View(vm);
        }

        private async Task PopulateDropdowns(ProcedureFormViewModel model)
        {
            var typesResult = await _procedureService.GetActiveTypesAsync();
            if (typesResult.IsSuccess)
            {
                model.AvailableTypes = typesResult.Data.Select(t => new SelectListItem
                {
                    Value = t.Id.ToString(),
                    Text = t.Name
                });
            }
        }
    }
}