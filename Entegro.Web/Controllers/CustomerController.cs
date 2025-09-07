using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.Customer;
using Entegro.Application.Interfaces.Services;
using Entegro.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Entegro.Web.Controllers
{
    public class CustomerController : Controller
    {
        public readonly ICustomerService _customerService;
        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            CustomerViewModel model = new CustomerViewModel();
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Create(CustomerViewModel model)
        {
            if (ModelState.IsValid)
            {
                var createDto = new CreateCustomerDto
                {
                    Address = model.Address,
                    City = model.City,
                    Email = model.Email,
                    CustomerType = model.CustomerType,
                    Town = model.Town,
                    District = model.District,
                    Name = model.Name,
                    PhoneNumber = model.PhoneNumber,
                    Street = model.Street,
                    TaxNumber = model.TaxNumber,
                    TaxOffice = model.TaxOffice,
                };

                await _customerService.CreateCustomerAsync(createDto);
                return Json(new { success = true });
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            CustomerViewModel model = new CustomerViewModel();

            var customer = await _customerService.GetCustomerByIdAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            model.Id = customer.Id;
            model.Address = customer.Address;
            model.City = customer.City;
            model.Email = customer.Email;
            model.CustomerType = customer.CustomerType;
            model.Town = customer.Town;
            model.District = customer.District;
            model.Name = customer.Name;
            model.PhoneNumber = customer.PhoneNumber;
            model.Street = customer.Street;
            model.TaxNumber = customer.TaxNumber;
            model.TaxOffice = customer.TaxOffice;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CustomerViewModel model)
        {
            if (ModelState.IsValid)
            {
                var updateDto = new UpdateCustomerDto
                {
                    CustomerId = model.Id,
                    Address = model.Address,
                    City = model.City,
                    Email = model.Email,
                    CustomerType = model.CustomerType,
                    Town = model.Town,
                    District = model.District,
                    Name = model.Name,
                    PhoneNumber = model.PhoneNumber,
                    Street = model.Street,
                    TaxNumber = model.TaxNumber,
                    TaxOffice = model.TaxOffice,
                };

                await _customerService.UpdateCustomerAsync(updateDto);

                return Json(new { success = true });
            }
            return View(model);
        }


        public IActionResult List()
        {
            return View();
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
