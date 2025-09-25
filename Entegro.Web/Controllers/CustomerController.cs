using Entegro.Application.DTOs.Address;
using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.Customer;
using Entegro.Application.DTOs.CustomerAddressMapping;
using Entegro.Application.Interfaces.Services;
using Entegro.Web.Models;
using Entegro.Web.Models.Common;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;

namespace Entegro.Web.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly ICustomerAddressMappingService _customerAddressMappingService;
        private readonly IAddressService _addressService;
        private readonly IMapper _mapper;
        public CustomerController(ICustomerService customerService, IMapper mapper, ICustomerAddressMappingService customerAddressMappingService, IAddressService addressService)
        {
            _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _customerAddressMappingService = customerAddressMappingService;
            _addressService = addressService;
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


        #region CustomerAddressMapping
        [HttpGet]
        public async Task<IActionResult> CreateOrUpdateCustomerAddressMapping(int customerId, int addressId)
        {

            if (customerId == 0 && addressId == 0)
            {
                return NotFound();
            }

            var customer = await _customerService.GetCustomerByIdAsync(customerId);
            if (customer == null)
            {
                return NotFound();
            }

            var isExistCustomerAddress = await _customerAddressMappingService.ExistsAsync(customerId, addressId);
            AddressModel addressModel = new AddressModel();
            if (isExistCustomerAddress)
            {
                var customerAddress = await _addressService.GetByIdAsync(addressId);
                var customerAddressMap = _mapper.Map<AddressModel>(customerAddress);
                ViewBag.CustomerId = customerId;
                return PartialView("_CreateOrUpdateCustomerAddressPartial", customerAddressMap);
            }

            ViewBag.CustomerId = customerId;
            return PartialView("_CreateOrUpdateCustomerAddressPartial", addressModel);
        }
        [HttpPost]
        public async Task<IActionResult> CreateOrUpdateCustomerAddressMapping(AddressModel model)
        {
            if (!ModelState.IsValid)
                return PartialView("_CreateOrUpdateCustomerAddressPartial", model);

            if (model.Id > 0)
            {
                var createdAddress = await _addressService.GetByIdAsync(model.Id);
                var mappedAddress = _mapper.Map<UpdateAddressDto>(model);
                await _addressService.UpdateAsync(mappedAddress);
                return RedirectToAction("Edit", "Customer", new { id = model.CustomerId });
            }


            var addressModel = _mapper.Map<CreateAddressDto>(model);
            var createdModel = await _addressService.AddAsync(addressModel);
            int customerId = model.CustomerId.Value;
            var costumerAddressModel = new CustomerAddressMappingModel
            {
                CustomerId = model.CustomerId.Value,
                AddressId = createdModel.Id
            };
            var costumerAddressMappingModel = _mapper.Map<CreateCustomerAddressMappingDto>(costumerAddressModel);
            await _customerAddressMappingService.AddAsync(costumerAddressMappingModel);
            return RedirectToAction("Edit", "Customer", new { id = model.CustomerId });
        }
        [HttpGet]
        public async Task<IActionResult> GetCustomerAddress(int customerId)
        {
            if (customerId == 0)
            {
                return NotFound();
            }
            var customerAddressMapping = await _customerAddressMappingService.GetByCustomerIdAsync(customerId);
            var map = _mapper.Map<List<CustomerAddressMappingModel>>(customerAddressMapping);
            ViewBag.CustomerId = customerId;
            return PartialView("_CustomerAddressTablePartial", map);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteCustomerAddress(int customerId, int addressId)
        {
            try
            {
                await _customerAddressMappingService.DeleteAsync(customerId, addressId);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion


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
