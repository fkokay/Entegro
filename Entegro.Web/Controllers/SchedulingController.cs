using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Services;
using Entegro.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Entegro.Web.Controllers
{
    public class SchedulingController : Controller
    {
        private readonly ITaskDescriptorService _taskDescriptorService;
        private readonly ISettingService _settingService;
        private readonly HttpClient _client;
        public SchedulingController(ITaskDescriptorService taskDescriptorService, ISettingService settingService, HttpClient client)
        {
            _taskDescriptorService = taskDescriptorService;
            _settingService = settingService;
            _client = client;
        }

        public IActionResult Index()
        {
            return List();
        }

        public IActionResult List()
        {
            return View();
        }

        public async Task<IActionResult> RunAsync(string type,int taskId)
        {

            if (string.IsNullOrWhiteSpace(type))
                return Json(new { success = false, error = "Type parametresi boş olamaz." });

            var setting = await _settingService.GetByKeyAsync("SystemApiUrl");
            if (setting == null || string.IsNullOrEmpty(setting.Value))
            {
                throw new Exception("SystemApiUrl ayarı bulunamadı");
            }

            _client.BaseAddress = new Uri(setting.Value);

            // Type parametresini query string ile gönderiyoruz
            var response = await _client.PostAsync($"api/job/run?type={type}&taskId={taskId}", null);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                return Json(new { success = false, error });
            }

            var result = await response.Content.ReadAsStringAsync();
            return Json(new { success = true, data = result });
        }

        [HttpPost]
        public async Task<IActionResult> SchedulingListAsync([FromBody] GridCommand gridCommand)
        {
            var result = await _taskDescriptorService.GetPagedAsync(gridCommand);

            return Json(new
            {
                draw = gridCommand.Draw,
                recordsTotal = result.TotalCount,
                recordsFiltered = result.TotalCount,
                data = result.Items
            });
        }
    }
}
