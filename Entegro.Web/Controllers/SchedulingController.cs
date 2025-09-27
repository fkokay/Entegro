using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Services;
using Entegro.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Entegro.Web.Controllers
{
    public class SchedulingController : Controller
    {
        private readonly ITaskDescriptorService _taskDescriptorService;
        public SchedulingController(ITaskDescriptorService taskDescriptorService) 
        {
            _taskDescriptorService = taskDescriptorService;
        }

        public IActionResult Index()
        {
            return List();
        }

        public IActionResult List()
        {
            return View();
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
