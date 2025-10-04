using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Services.Base;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;

namespace Entegro.Web.Controllers
{
    public class ReturnRequestController : Controller
    {
        private readonly IReturnRequestService _returnRequestService;
        private readonly IMapper _mapper;
        public ReturnRequestController(IReturnRequestService returnRequestService, IMapper mapper)
        {
            _returnRequestService = returnRequestService ?? throw new ArgumentNullException(nameof(returnRequestService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult List()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ReturnRequestList([FromBody] GridCommand gridCommand)
        {
            var result = await _returnRequestService.GetPagedAsync(gridCommand);

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
