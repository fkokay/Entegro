using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.ReturnRequest;
using Entegro.Application.Interfaces.Services.Base;
using Entegro.Domain.Enums;
using Entegro.Web.Helpers;
using Entegro.Web.Models.Checkout.Orders;
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

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var returnRequest = await _returnRequestService.GetByIdAsync(id);
            if (returnRequest == null)
            {
                return NotFound();
            }

            ViewBag.ReturnRequestStatus = EnumHelper.GetEnumSelectList<ReturnRequestStatus>();
            ViewBag.ReasonForReturnType = EnumHelper.GetEnumSelectList<ReasonForReturnType>();
            ViewBag.RequestedActionType = EnumHelper.GetEnumSelectList<RequestedActionType>();

            var mapped = _mapper.Map<ReturnRequestModel>(returnRequest);
            return View(mapped);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(ReturnRequestModel model)
        {
            if (ModelState.IsValid)
            {
                var modelDto = _mapper.Map<UpdateReturnRequestDto>(model);
                await _returnRequestService.UpdateAsync(modelDto);
                return RedirectToAction("List");
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _returnRequestService.DeleteAsync(id);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
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
