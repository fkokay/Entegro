using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Services;
using Entegro.Web.Models.Platform.Logging;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;

namespace Entegro.Web.Controllers
{
    public class LogController : Controller
    {
        private readonly ILogService _logService;
        private readonly IMapper _mapper;

        public LogController(ILogService logService, IMapper mapper)
        {
            _logService = logService;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult List()
        {
            return View();
        }
        public async Task<IActionResult> View(int logId)
        {
            var log = await _logService.GetByIdAsync(logId);
            if (log == null)
            {
                return NotFound();
            }
            var mapLog = _mapper.Map<LogViewModel>(log);
            return View(mapLog);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (id == 0)
                {
                    await _logService.DeleteAllAsync();
                    return Json(new { success = true });
                }
                await _logService.DeleteAsync(id);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> LogList([FromBody] GridCommand gridCommand)
        {
            var result = await _logService.GetPagedAsync(gridCommand);

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
