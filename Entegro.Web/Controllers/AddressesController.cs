using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Services;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;

namespace Entegro.Web.Controllers
{
    public class AddressesController : Controller
    {
        private readonly IAddressService _addressService;
        private readonly IMapper _mapper;
        public AddressesController(IAddressService addressService, IMapper mapper)
        {
            _addressService = addressService ?? throw new ArgumentNullException(nameof(addressService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        public IActionResult Index()
        {
            return View();
        }



        [HttpPost]
        public async Task<IActionResult> AddressList([FromBody] GridCommand gridCommand, int customerId)
        {
            var result = await _addressService.GetPagedAsync(gridCommand, customerId);
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
