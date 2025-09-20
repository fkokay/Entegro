using Entegro.Application.DTOs.Brand;
using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.Customer;
using Entegro.Application.Interfaces.Services;
using Entegro.Domain.Entities.Catalog;
using Entegro.Web.Models;
using Entegro.Web.Models.Catalog.Brands;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;

namespace Entegro.Web.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly IMapper _mapper;
        public CustomerController(ICustomerService customerService,IMapper mapper)
        {
            _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
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
        public IActionResult Create()
        {
            CustomerModel model = new CustomerModel();
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Create(CustomerModel model)
        {
            if (ModelState.IsValid)
            {
                var createDto = _mapper.Map<CreateCustomerDto>(model);
                await _customerService.CreateCustomerAsync(createDto);
                return Json(new { success = true });
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            var model = _mapper.Map<CustomerModel>(customer);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CustomerModel model)
        {
            if (ModelState.IsValid)
            {
                var updateDto = _mapper.Map<UpdateCustomerDto>(model);
                await _customerService.UpdateCustomerAsync(updateDto);

                return Json(new { success = true });
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CustomerList([FromBody] GridCommand gridCommand)
        {
            var result = await _customerService.GetPagedAsync(gridCommand);

            return Json(new
            {
                draw = gridCommand.Draw,
                recordsTotal = result.TotalCount,
                recordsFiltered = result.TotalCount,
                data = result.Items
            });

        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _customerService.DeleteCustomerAsync(id);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
