using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.OrderItem;
using Entegro.Application.DTOs.ProductIntegration;
using Entegro.Application.DTOs.ReturnRequest;
using Entegro.Application.DTOs.ReturnRequestItem;
using Entegro.Application.Interfaces.Services.Base;
using Entegro.Application.Services.Base;
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
        private readonly IIntegrationSystemService _integrationSystemService;
        private readonly IProductIntegrationService _productIntegrationService;
        private readonly IReturnRequestItemService _returnRequestItemService;
        private readonly IProductService _productService;
        public ReturnRequestController(IReturnRequestService returnRequestService, IMapper mapper, IIntegrationSystemService integrationSystemService, IProductIntegrationService productIntegrationService, IProductService productService, IReturnRequestItemService returnRequestItemService)
        {
            _returnRequestService = returnRequestService ?? throw new ArgumentNullException(nameof(returnRequestService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _integrationSystemService = integrationSystemService;
            _productIntegrationService = productIntegrationService;
            _productService = productService;
            _returnRequestItemService = returnRequestItemService;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> List(int returnRequestStatusId = 1)
        {
            ViewBag.ReturnRequestStatusId = returnRequestStatusId;
            var returnRequestPage = await _returnRequestService.GetReturnRequestPageAsync();
            var model = _mapper.Map<ReturnListModel>(returnRequestPage);
            return View(model);
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
        public async Task<IActionResult> CreateProductIntegration(int integrationSystemId, int productId, int? productVariantAttributeCombinationId, string integrationCode)
        {
            var integrationSystem = await _integrationSystemService.GetByIdAsync(integrationSystemId);
            var product = await _productService.GetProductByIdAsync(productId);

            ProductIntegrationDto? ifExistingProductIntegration = new();

            CreateProductIntegrationDto createProductIntegration = new CreateProductIntegrationDto();
            createProductIntegration.ProductId = productId;
            createProductIntegration.ProductVariantAttributeCombinationId = productVariantAttributeCombinationId;
            createProductIntegration.IntegrationCode = integrationCode;
            createProductIntegration.Price = 0;
            createProductIntegration.IntegrationSystemId = integrationSystemId;
            createProductIntegration.IsSync = true;
            createProductIntegration.Active = true;
            createProductIntegration.LastSyncDate = null;

            if (productVariantAttributeCombinationId == null)
            {
                ifExistingProductIntegration = await _productIntegrationService.GetByProductAndIntegrationSystemAsync(productId, integrationSystemId);
                if (ifExistingProductIntegration != null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Ürün zaten eşleştirilmiş.",
                    });
                }
                await _productIntegrationService.AddAsync(createProductIntegration);
            }
            else
            {
                ifExistingProductIntegration = await _productIntegrationService.GetByProductAndIntegrationSystemAsync(productId, integrationSystemId, productVariantAttributeCombinationId.Value);
                if (ifExistingProductIntegration != null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Ürün varyantı zaten eşleştirilmiş.",
                    });
                }
                await _productIntegrationService.AddAsync(createProductIntegration);
            }


            var returnRequestItems = await _returnRequestItemService.GetAllWithIntegrationSkuAsync(integrationCode);
            foreach (var returnRequest in returnRequestItems)
            {
                var returnRequestItem = await _returnRequestItemService.GetByIdAsync(returnRequest.Id);
                UpdateReturnRequestItemDto updateReturnRequestItem = new UpdateReturnRequestItemDto();
                updateReturnRequestItem.Id = returnRequestItem.Id;
                updateReturnRequestItem.ReturnRequestId = returnRequestItem.ReturnRequestId;
                updateReturnRequestItem.ProductId = productId;
                updateReturnRequestItem.ProductName = product.Name;
                updateReturnRequestItem.Barcode = product.Barcode;
                updateReturnRequestItem.MerchantSku = returnRequestItem.MerchantSku;
                updateReturnRequestItem.ProductColor = returnRequestItem.ProductColor;
                updateReturnRequestItem.ProductSize = returnRequestItem.ProductSize;
                updateReturnRequestItem.Price = returnRequestItem.Price;
                updateReturnRequestItem.VatBaseAmount = returnRequestItem.VatBaseAmount;
                updateReturnRequestItem.VatRate = returnRequestItem.VatRate;
                updateReturnRequestItem.SalesCampaignId = returnRequestItem.SalesCampaignId;
                updateReturnRequestItem.ProductCategory = returnRequestItem.ProductCategory;
                updateReturnRequestItem.ProductImageUrl = returnRequestItem.ProductImageUrl;
                updateReturnRequestItem.CustomerClaimReasonName = returnRequestItem.CustomerClaimReasonName;
                updateReturnRequestItem.CustomerClaimReasonCode = returnRequestItem.CustomerClaimReasonCode;
                updateReturnRequestItem.PlatformClaimReasonName = returnRequestItem.PlatformClaimReasonName;
                updateReturnRequestItem.PlatformClaimReasonCode = returnRequestItem.PlatformClaimReasonCode;
                updateReturnRequestItem.PlatformName = returnRequestItem.PlatformName;
                updateReturnRequestItem.AutoApproveDate = returnRequestItem.AutoApproveDate;
                updateReturnRequestItem.Note = returnRequestItem.Note;
                updateReturnRequestItem.CustomerNote = returnRequestItem.CustomerNote;
                updateReturnRequestItem.Resolved = returnRequestItem.Resolved;
                updateReturnRequestItem.AcceptedBySeller = returnRequestItem.AcceptedBySeller;
                updateReturnRequestItem.ReturnRequestStatusId = returnRequestItem.ReturnRequestStatusId;

                await _returnRequestItemService.UpdateAsync(updateReturnRequestItem);
            }


            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> ReturnRequestList([FromBody] ReturnRequestListRequest request, int returnRequestStatusId)
        {
            var result = await _returnRequestService.GetPagedAsync(request.Grid, request.Filters, returnRequestStatusId);

            return Json(new
            {
                draw = request.Grid.Draw,
                recordsTotal = result.TotalCount,
                recordsFiltered = result.TotalCount,
                data = result.Items
            });
        }
    }
}
