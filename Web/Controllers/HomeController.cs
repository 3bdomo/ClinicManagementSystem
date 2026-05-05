using AutoMapper;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Web.ViewModel;

namespace Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IDashboardService _dashboardService;
        private readonly IMapper _mapper;

        public HomeController(IDashboardService dashboardService, IMapper mapper)
        {
            _dashboardService = dashboardService;
            _mapper = mapper;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Dashboard()
        {
            var statsResult = await _dashboardService.GetStatsAsync();
            var summaryResult = await _dashboardService.GetTodaySummaryAsync();

            var vm = new DashboardViewModel();
            if (statsResult.IsSuccess && statsResult.Data != null)
            {
                vm.TotalPatients = statsResult.Data.TotalPatients;
                vm.TodayAppointments = statsResult.Data.TodayAppointments;
                vm.TodayRevenue = statsResult.Data.TodayRevenue;
                vm.PendingInvoicesCount = statsResult.Data.PendingInvoicesCount;
                vm.UpcomingFollowUps = statsResult.Data.UpcomingFollowUps;
            }
            if (summaryResult.IsSuccess && summaryResult.Data != null)
            {
                vm.RecentAppointments = _mapper.Map<System.Collections.Generic.IEnumerable<AppointmentRowViewModel>>(summaryResult.Data.Appointments);
            }

            return View(vm);
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new UI.Models.ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
