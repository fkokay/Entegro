using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.TaskDescriptor;
using Entegro.Application.Interfaces.Services.Base;
using Entegro.Web.Models.Platform.Scheduling;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Entegro.Web.Controllers
{
    public class SchedulingController : Controller
    {
        private readonly ITaskDescriptorService _taskDescriptorService;
        private readonly ISettingService _settingService;
        private readonly HttpClient _client;
        private readonly IMapper _mapper;
        private readonly ITaskExecutionInfoService _taskExecutionInfoService;
        public SchedulingController(ITaskDescriptorService taskDescriptorService, ISettingService settingService, HttpClient client, IMapper mapper, ITaskExecutionInfoService taskExecutionInfoService)
        {
            _taskDescriptorService = taskDescriptorService;
            _settingService = settingService;
            _client = client;
            _mapper = mapper;
            _taskExecutionInfoService = taskExecutionInfoService;
        }

        public IActionResult Index()
        {
            return List();
        }

        public IActionResult List()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var schedule = await _taskDescriptorService.GetByIdAsync(id);
            if (schedule == null)
            {
                return NotFound();
            }
            var mapSchedule = _mapper.Map<TaskDescriptorViewModel>(schedule);
            ViewBag.Priorities = new List<SelectListItem>
            {
                new SelectListItem { Text = "Düşük", Value = "-1" },
                new SelectListItem { Text = "Normal", Value = "0" },
                new SelectListItem { Text = "Yüksek", Value = "1" },
            };

            return View(mapSchedule);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(TaskDescriptorViewModel model)
        {
            if (ModelState.IsValid)
            {
                var updateDto = _mapper.Map<UpdateTaskDescriptorDto>(model);
                await _taskDescriptorService.UpdateAsync(updateDto);

                return RedirectToAction("Edit", new { id = updateDto.Id });
            }

            ViewBag.Priorities = new List<SelectListItem>
            {
                new SelectListItem { Text = "Düşük", Value = "-1" },
                new SelectListItem { Text = "Normal", Value = "0" },
                new SelectListItem { Text = "Yüksek", Value = "1" },
            };
            return View(model);
        }

        public async Task<IActionResult> RunAsync(string type, int taskId,int? parameter)
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
            var response = await _client.PostAsync($"api/job/run?type={type}&taskId={taskId}&parameter={parameter}", null);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                return Json(new { success = false, error });
            }

            var result = await response.Content.ReadAsStringAsync();
            return Json(new { success = true, data = result });
        }


        [HttpPost]
        public async Task<IActionResult> TaskExecutionInfoList([FromBody] GridCommand gridCommand, int taskDescriptorId)
        {
            var result = await _taskExecutionInfoService.GetPagedAsync(gridCommand, taskDescriptorId);
            return Json(new
            {
                draw = gridCommand.Draw,
                recordsTotal = result.TotalCount,
                recordsFiltered = result.TotalCount,
                data = result.Items
            });
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
