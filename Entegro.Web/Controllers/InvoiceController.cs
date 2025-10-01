using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Services.Base;
using Microsoft.AspNetCore.Mvc;

namespace Entegro.Web.Controllers
{
    public class InvoiceController : Controller
    {
        private readonly IInvoiceService _invoiceService;

        public InvoiceController(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService ?? throw new ArgumentNullException(nameof(invoiceService));
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
        public async Task<IActionResult> InvoiceList([FromBody] GridCommand gridCommand)
        {
            var result = await _invoiceService.GetPagedAsync(gridCommand);

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
