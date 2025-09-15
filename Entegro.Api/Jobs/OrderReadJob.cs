using Entegro.Application.DTOs.Address;
using Entegro.Application.DTOs.Commerce.Smartstore;
using Entegro.Application.DTOs.Customer;
using Entegro.Application.DTOs.IntegrationSystem;
using Entegro.Application.DTOs.Marketplace.CicekSepeti;
using Entegro.Application.DTOs.Marketplace.Hepsiburada;
using Entegro.Application.DTOs.Marketplace.N11;
using Entegro.Application.DTOs.Marketplace.Pazarama;
using Entegro.Application.DTOs.Marketplace.Trendyol;
using Entegro.Application.DTOs.Order;
using Entegro.Application.DTOs.Product;
using Entegro.Application.Interfaces.Services;
using Entegro.Application.Interfaces.Services.Commerce;
using Entegro.Application.Interfaces.Services.Marketplace;
using Entegro.Application.Mappings.Commerce.Smartstore;
using Entegro.Application.Mappings.Marketplace.CicekSepeti;
using Entegro.Application.Mappings.Marketplace.Hepsiburada;
using Entegro.Application.Mappings.Marketplace.N11;
using Entegro.Application.Mappings.Marketplace.Pazarama;
using Entegro.Application.Mappings.Marketplace.Trendyol;
using Entegro.Application.Services;
using Entegro.Application.Services.Commerce;
using Entegro.Domain.Entities.Checkout;
using MapsterMapper;
using Quartz;

namespace Entegro.Api.Jobs
{
    public class OrderReadJob : IJob
    {
        private readonly IN11Service _n11;
        private readonly IPazaramaService _pazarama;
        private readonly IHepsiburadaService _hepsiburada;
        private readonly ITrendyolService _trendyol;
        private readonly ICicekSepetiService _cicekSepeti;
     
        private readonly ISmartstoreService _smartstore;
        private readonly IOrderService _orderService;
        private readonly ICustomerService _customerService;
        private readonly IAddressService _addressService;
        private readonly IProductService _productService;
        private readonly IIntegrationSystemService _integrationSystemService;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderReadJob> _logger;
        public OrderReadJob(
            IN11Service n11,
            IPazaramaService pazarama,
            IHepsiburadaService hepsiburada,
            ITrendyolService trendyol,
            ICicekSepetiService cicekSepeti,
            ISmartstoreService smartstore,
            IOrderService orderService,
            ICustomerService customerService,
            IAddressService addressService,
            IProductService productService,
            IIntegrationSystemService integrationSystemService,
            IMapper mapper,
            ILogger<OrderReadJob> logger)
        {
            _n11 = n11;
            _pazarama = pazarama;
            _hepsiburada = hepsiburada;
            _trendyol = trendyol;
            _cicekSepeti = cicekSepeti;
            _smartstore = smartstore;
            _orderService = orderService;
            _customerService = customerService;
            _addressService = addressService;
            _productService = productService;
            _integrationSystemService = integrationSystemService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("Sipariş aktarım servisi başladı.");

            var integrationSystems = await _integrationSystemService.GetAllAsync(null);
            foreach (var item in integrationSystems)
            {
                switch (item.IntegrationSystemType)
                {
                    case Domain.Enums.IntegrationSystemType.Commerce:
                        await CommerceOrderSync(item);
                        break;
                    case Domain.Enums.IntegrationSystemType.Marketplace:
                        await MarketplaceOrderSync(item);
                        break;
                    default:
                        break;
                }
            }

        }

        private async Task MarketplaceOrderSync(IntegrationSystemDto item)
        {
            string marketPlaceType = item.IntegrationSystemParameters.Where(m => m.Key == "MarketplaceType").Select(m => m.Value).FirstOrDefault() ?? "";
            switch (marketPlaceType)
            {
                case "N11":
                    await N11OrderSync(item);
                    break;
                case "Hepsiburada":
                    await HepsiburadaOrderSync(item);
                    break;
                case "Trendyol":
                    await TrendyolOrderSync(item);
                    break;
                case "CicekSepeti":
                    await CicekSepetiOrderSync(item);
                    break;
                case "Pazarama":
                    await PazaramaOrderSync(item);
                    break;
                case "Idefix":
                    await IdefixOrderSync(item);
                    break;
                default:
                    _logger.LogError("{0} pazaryerine ait sipariş çekme işlemi bulunamadı", marketPlaceType);
                    break;
            }
        }

        private async Task IdefixOrderSync(IntegrationSystemDto item)
        {
            throw new NotImplementedException();
        }

        private async Task PazaramaOrderSync(IntegrationSystemDto item)
        {
            try
            {
                _logger.LogInformation("Pazarama sipariş senkronizasyonu başlatıldı. Zaman: {Time}", DateTime.UtcNow);

                PazaramaApiContext context = GetPazaramaApiContext(item);

                var pazaramaOrders = await _pazarama.GetOrdersAsync(context);

                if (pazaramaOrders == null || !pazaramaOrders.Any())
                {
                    _logger.Warn("Pazarama'dan hiç sipariş alınamadı.");
                    return;
                }

                PazaramaOrderMapper.ConfigureLogger(_logger);
                var orders = PazaramaOrderMapper.ToDtoList(pazaramaOrders);

                foreach (var order in orders)
                {
                    try
                    {
                        #region Exists Order
                        if (await _orderService.ExistsByOrderNoAsync(order.OrderNumber))
                        {
                            _logger.LogInformation("'{OrderNumber}' nolu sipariş zaten kayıtlı", order.OrderNumber);
                            continue;
                        }
                        #endregion

                        #region Customer
                        var customer = await _customerService.GetCustomerByEmailAsync(order.Customer.Email);
                        if (customer == null)
                        {
                            var createCustomer = _mapper.Map<CreateCustomerDto>(order.Customer);

                            customer = await CreateCustomer(createCustomer);

                            order.CustomerId = customer.Id;
                            order.Customer = null;
                        }
                        else
                        {
                            order.CustomerId = customer.Id;
                            order.Customer = null;
                        }
                        #endregion

                        #region Address
                        if (order.ShippingAddress != null)
                        {
                            var createShippingAddress = _mapper.Map<CreateAddressDto>(order.ShippingAddress);
                            var address = await CreateAddress(createShippingAddress);
                            order.ShippingAddressId = address.Id;
                            order.ShippingAddress = null;
                        }

                        if (order.BillingAddress != null)
                        {
                            var createShippingAddress = _mapper.Map<CreateAddressDto>(order.BillingAddress);
                            var address = await CreateAddress(createShippingAddress);
                            order.BillingAddressId = address.Id;
                            order.BillingAddress = null;
                        }
                        #endregion

                        #region OrderItems
                        foreach (var orderItem in order.OrderItems)
                        {
                            if (orderItem.Product != null)
                            {
                                var product = await _productService.GetProductByCodeAsync(orderItem.Product.Code);

                                if (product == null)
                                {
                                    _logger.Error($"{orderItem.Product.Code} kodlu ürün {order.OrderNumber} ' +nolu siparişte bulunamadı");
                                    continue;
                                }

                                orderItem.Product = null;
                                orderItem.ProductId = product.Id;
                            }
                        }
                        #endregion

                        #region Order
                        var createOrder = _mapper.Map<CreateOrderDto>(order);
                        await _orderService.CreateOrderAsync(createOrder);

                        _logger.LogInformation("'{OrderNo}' nolu sipariş başarıyla kaydedildi.", order.OrderNumber);
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex);
                    }
                }

                _logger.LogInformation("Pazarama sipariş senkronizasyonu tamamlandı. Zaman: {Time}", DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private PazaramaApiContext GetPazaramaApiContext(IntegrationSystemDto item)
        {
            PazaramaApiContext context = new PazaramaApiContext();

            if (!item.IntegrationSystemParameters.Where(m => m.Key == "ClientId").Any() || string.IsNullOrEmpty(item.IntegrationSystemParameters.Where(m => m.Key == "ClientId").Select(m => m.Value).FirstOrDefault()))
            {
                _logger.Error("Pazarama ClientId Ayarlanmamış");
            }

            if (!item.IntegrationSystemParameters.Where(m => m.Key == "ClientSecret").Any() || string.IsNullOrEmpty(item.IntegrationSystemParameters.Where(m => m.Key == "ClientSecret").Select(m => m.Value).FirstOrDefault()))
            {
                _logger.Error("Pazarama ClientSecret Ayarlanmamış");
            }

            context.ClientId = item.IntegrationSystemParameters.Where(m => m.Key == "SupplierId").Select(m => m.Value).FirstOrDefault() ?? "";
            context.ClientSecret = item.IntegrationSystemParameters.Where(m => m.Key == "ClientSecret").Select(m => m.Value).FirstOrDefault() ?? "";

            return context;
        }

        private async Task CicekSepetiOrderSync(IntegrationSystemDto item)
        {
            try
            {
                _logger.LogInformation("ÇicekSepeti sipariş senkronizasyonu başlatıldı. Zaman: {Time}", DateTime.UtcNow);

                CicekSepetiApiContext context = GetCicekSepetiApiContext(item);

                var cicekSepetiOrders = await _cicekSepeti.GetOrdersAsync(context);

                if (cicekSepetiOrders == null || !cicekSepetiOrders.Any())
                {
                    _logger.Warn("ÇicekSepetin'den hiç sipariş alınamadı.");
                    return;
                }

                CicekSepetOrderMapper.ConfigureLogger(_logger);
                var orders = CicekSepetOrderMapper.ToDtoList(cicekSepetiOrders);

                foreach (var order in orders)
                {
                    try
                    {
                        #region Exists Order
                        if (await _orderService.ExistsByOrderNoAsync(order.OrderNumber))
                        {
                            _logger.LogInformation("'{OrderNumber}' nolu sipariş zaten kayıtlı", order.OrderNumber);
                            continue;
                        }
                        #endregion

                        #region Customer
                        var customer = await _customerService.GetCustomerByEmailAsync(order.Customer.Email);
                        if (customer == null)
                        {
                            var createCustomer = _mapper.Map<CreateCustomerDto>(order.Customer);

                            customer = await CreateCustomer(createCustomer);

                            order.CustomerId = customer.Id;
                            order.Customer = null;
                        }
                        else
                        {
                            order.CustomerId = customer.Id;
                            order.Customer = null;
                        }
                        #endregion

                        #region Address
                        if (order.ShippingAddress != null)
                        {
                            var createShippingAddress = _mapper.Map<CreateAddressDto>(order.ShippingAddress);
                            var address = await CreateAddress(createShippingAddress);
                            order.ShippingAddressId = address.Id;
                            order.ShippingAddress = null;
                        }

                        if (order.BillingAddress != null)
                        {
                            var createShippingAddress = _mapper.Map<CreateAddressDto>(order.BillingAddress);
                            var address = await CreateAddress(createShippingAddress);
                            order.BillingAddressId = address.Id;
                            order.BillingAddress = null;
                        }
                        #endregion

                        #region OrderItems
                        foreach (var orderItem in order.OrderItems)
                        {
                            if (orderItem.Product != null)
                            {
                                var product = await _productService.GetProductByCodeAsync(orderItem.Product.Code);

                                if (product == null)
                                {
                                    _logger.Error($"{orderItem.Product.Code} kodlu ürün {order.OrderNumber} ' +nolu siparişte bulunamadı");
                                    continue;
                                }

                                orderItem.Product = null;
                                orderItem.ProductId = product.Id;
                            }
                        }
                        #endregion

                        #region Order
                        var createOrder = _mapper.Map<CreateOrderDto>(order);
                        await _orderService.CreateOrderAsync(createOrder);

                        _logger.LogInformation("'{OrderNo}' nolu sipariş başarıyla kaydedildi.", order.OrderNumber);
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex);
                    }
                }

                _logger.LogInformation("ÇicekSepeti sipariş senkronizasyonu tamamlandı. Zaman: {Time}", DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private CicekSepetiApiContext GetCicekSepetiApiContext(IntegrationSystemDto item)
        {
            CicekSepetiApiContext context = new CicekSepetiApiContext();

            if (!item.IntegrationSystemParameters.Where(m => m.Key == "SupplierId").Any() || string.IsNullOrEmpty(item.IntegrationSystemParameters.Where(m => m.Key == "SupplierId").Select(m => m.Value).FirstOrDefault()))
            {
                _logger.Error("ÇicekSepeti SupplierId Ayarlanmamış");
            }

            if (!item.IntegrationSystemParameters.Where(m => m.Key == "ApiUser").Any() || string.IsNullOrEmpty(item.IntegrationSystemParameters.Where(m => m.Key == "ApiUser").Select(m => m.Value).FirstOrDefault()))
            {
                _logger.Error("ÇicekSepeti ApiUser Ayarlanmamış");
            }

            context.SupplierId = item.IntegrationSystemParameters.Where(m => m.Key == "SupplierId").Select(m => m.Value).FirstOrDefault() ?? "";
            context.ApiUser = item.IntegrationSystemParameters.Where(m => m.Key == "ApiUser").Select(m => m.Value).FirstOrDefault() ?? "";

            return context;

        }

        private async Task TrendyolOrderSync(IntegrationSystemDto item)
        {
            try
            {
                _logger.LogInformation("Trendyol sipariş senkronizasyonu başlatıldı. Zaman: {Time}", DateTime.UtcNow);

                TrendyolApiContext context = GetTrendyolApiContext(item);

                var trendyolShipmentPackages = await _trendyol.GetShipmentPackagesAsync(context);

                if (trendyolShipmentPackages == null || !trendyolShipmentPackages.Any())
                {
                    _logger.Warn("Trendyol'dan hiç sipariş alınamadı.");
                    return;
                }

                TrendyolShipmentPackageMapper.ConfigureLogger(_logger);
                var orders = TrendyolShipmentPackageMapper.ToDtoList(trendyolShipmentPackages);

                foreach (var order in orders)
                {
                    try
                    {
                        #region Exists Order
                        if (await _orderService.ExistsByOrderNoAsync(order.OrderNumber))
                        {
                            _logger.LogInformation("'{OrderNumber}' nolu sipariş zaten kayıtlı", order.OrderNumber);
                            continue;
                        }
                        #endregion

                        #region Customer
                        var customer = await _customerService.GetCustomerByEmailAsync(order.Customer.Email);
                        if (customer == null)
                        {
                            var createCustomer = _mapper.Map<CreateCustomerDto>(order.Customer);

                            customer = await CreateCustomer(createCustomer);

                            order.CustomerId = customer.Id;
                            order.Customer = null;
                        }
                        else
                        {
                            order.CustomerId = customer.Id;
                            order.Customer = null;
                        }
                        #endregion

                        #region Address
                        if (order.ShippingAddress != null)
                        {
                            var createShippingAddress = _mapper.Map<CreateAddressDto>(order.ShippingAddress);
                            var address = await CreateAddress(createShippingAddress);
                            order.ShippingAddressId = address.Id;
                            order.ShippingAddress = null;
                        }

                        if (order.BillingAddress != null)
                        {
                            var createShippingAddress = _mapper.Map<CreateAddressDto>(order.BillingAddress);
                            var address = await CreateAddress(createShippingAddress);
                            order.BillingAddressId = address.Id;
                            order.BillingAddress = null;
                        }
                        #endregion

                        #region OrderItems
                        foreach (var orderItem in order.OrderItems)
                        {
                            if (orderItem.Product != null)
                            {
                                var product = await _productService.GetProductByCodeAsync(orderItem.Product.Code);

                                if (product == null)
                                {
                                    _logger.Error($"{orderItem.Product.Code} kodlu ürün {order.OrderNumber} ' +nolu siparişte bulunamadı");
                                    continue;
                                }

                                orderItem.Product = null;
                                orderItem.ProductId = product.Id;
                            }
                        }
                        #endregion

                        #region Order
                        var createOrder = _mapper.Map<CreateOrderDto>(order);
                        await _orderService.CreateOrderAsync(createOrder);

                        _logger.LogInformation("'{OrderNo}' nolu sipariş başarıyla kaydedildi.", order.OrderNumber);
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex);
                    }
                }

                _logger.LogInformation("Trendyol sipariş senkronizasyonu tamamlandı. Zaman: {Time}", DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private TrendyolApiContext GetTrendyolApiContext(IntegrationSystemDto item)
        {
            TrendyolApiContext context = new TrendyolApiContext();

            if (!item.IntegrationSystemParameters.Where(m => m.Key == "SupplierId").Any() || string.IsNullOrEmpty(item.IntegrationSystemParameters.Where(m => m.Key == "SupplierId").Select(m => m.Value).FirstOrDefault()))
            {
                _logger.Error("Trendyol SupplierId Ayarlanmamış");
            }

            if (!item.IntegrationSystemParameters.Where(m => m.Key == "ApiUser").Any() || string.IsNullOrEmpty(item.IntegrationSystemParameters.Where(m => m.Key == "ApiUser").Select(m => m.Value).FirstOrDefault()))
            {
                _logger.Error("Trendyol ApiUser Ayarlanmamış");
            }

            if (!item.IntegrationSystemParameters.Where(m => m.Key == "ApiPassword").Any() || string.IsNullOrEmpty(item.IntegrationSystemParameters.Where(m => m.Key == "ApiPassword").Select(m => m.Value).FirstOrDefault()))
            {
                _logger.Error("Trendyol ApiPassword Ayarlanmamış");
            }

            context.SupplierId = item.IntegrationSystemParameters.Where(m => m.Key == "SupplierId").Select(m => m.Value).FirstOrDefault() ?? "";
            context.ApiUser = item.IntegrationSystemParameters.Where(m => m.Key == "ApiUser").Select(m => m.Value).FirstOrDefault() ?? "";
            context.ApiPassword = item.IntegrationSystemParameters.Where(m => m.Key == "ApiPassword").Select(m => m.Value).FirstOrDefault() ?? "";

            return context;
        }

        private async Task HepsiburadaOrderSync(IntegrationSystemDto item)
        {
            try
            {
                _logger.LogInformation("Hepsiburada sipariş senkronizasyonu başlatıldı. Zaman: {Time}", DateTime.UtcNow);

                HepsiburadaApiContext context = GetHepsiburadaApiContext(item);

                var hepsiburadaShipmentPackages = await _hepsiburada.GetShipmentPackagesAsync(context);

                if (hepsiburadaShipmentPackages == null || !hepsiburadaShipmentPackages.Any())
                {
                    _logger.Warn("Hepsiburada'dan hiç sipariş alınamadı.");
                    return;
                }

                HepsiburadaShipmentPackageMapper.ConfigureLogger(_logger);
                var orders = HepsiburadaShipmentPackageMapper.ToDtoList(hepsiburadaShipmentPackages);

                foreach (var order in orders)
                {
                    try
                    {
                        #region Exists Order
                        if (await _orderService.ExistsByOrderNoAsync(order.OrderNumber))
                        {
                            _logger.LogInformation("'{OrderNumber}' nolu sipariş zaten kayıtlı", order.OrderNumber);
                            continue;
                        }
                        #endregion

                        #region Customer
                        var customer = await _customerService.GetCustomerByEmailAsync(order.Customer.Email);
                        if (customer == null)
                        {
                            var createCustomer = _mapper.Map<CreateCustomerDto>(order.Customer);

                            customer = await CreateCustomer(createCustomer);

                            order.CustomerId = customer.Id;
                            order.Customer = null;
                        }
                        else
                        {
                            order.CustomerId = customer.Id;
                            order.Customer = null;
                        }
                        #endregion

                        #region Address
                        if (order.ShippingAddress != null)
                        {
                            var createShippingAddress = _mapper.Map<CreateAddressDto>(order.ShippingAddress);
                            var address = await CreateAddress(createShippingAddress);
                            order.ShippingAddressId = address.Id;
                            order.ShippingAddress = null;
                        }

                        if (order.BillingAddress != null)
                        {
                            var createShippingAddress = _mapper.Map<CreateAddressDto>(order.BillingAddress);
                            var address = await CreateAddress(createShippingAddress);
                            order.BillingAddressId = address.Id;
                            order.BillingAddress = null;
                        }
                        #endregion

                        #region OrderItems
                        foreach (var orderItem in order.OrderItems)
                        {
                            if (orderItem.Product != null)
                            {
                                var product = await _productService.GetProductByCodeAsync(orderItem.Product.Code);

                                if (product == null)
                                {
                                    _logger.Error($"{orderItem.Product.Code} kodlu ürün {order.OrderNumber} ' +nolu siparişte bulunamadı");
                                    continue;
                                }

                                orderItem.Product = null;
                                orderItem.ProductId = product.Id;
                            }
                        }
                        #endregion

                        #region Order
                        var createOrder = _mapper.Map<CreateOrderDto>(order);
                        await _orderService.CreateOrderAsync(createOrder);

                        _logger.LogInformation("'{OrderNo}' nolu sipariş başarıyla kaydedildi.", order.OrderNumber);
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex);
                    }
                }

                _logger.LogInformation("Hepsiburada sipariş senkronizasyonu tamamlandı. Zaman: {Time}", DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private HepsiburadaApiContext GetHepsiburadaApiContext(IntegrationSystemDto item)
        {
            HepsiburadaApiContext context = new HepsiburadaApiContext();

            if (!item.IntegrationSystemParameters.Where(m => m.Key == "MerchantId").Any() || string.IsNullOrEmpty(item.IntegrationSystemParameters.Where(m => m.Key == "MerchantId").Select(m => m.Value).FirstOrDefault()))
            {
                _logger.Error("Hepsiburada MerchantId Ayarlanmamış");
            }

            if (!item.IntegrationSystemParameters.Where(m => m.Key == "ApiUser").Any() || string.IsNullOrEmpty(item.IntegrationSystemParameters.Where(m => m.Key == "ApiUser").Select(m => m.Value).FirstOrDefault()))
            {
                _logger.Error("Hepsiburada ApiUser Ayarlanmamış");
            }

            if (!item.IntegrationSystemParameters.Where(m => m.Key == "ApiPassword").Any() || string.IsNullOrEmpty(item.IntegrationSystemParameters.Where(m => m.Key == "ApiPassword").Select(m => m.Value).FirstOrDefault()))
            {
                _logger.Error("Hepsiburada ApiPassword Ayarlanmamış");
            }

            if (!item.IntegrationSystemParameters.Where(m => m.Key == "UserAgent").Any() || string.IsNullOrEmpty(item.IntegrationSystemParameters.Where(m => m.Key == "UserAgent").Select(m => m.Value).FirstOrDefault()))
            {
                _logger.Error("Hepsiburada UserAgent Ayarlanmamış");
            }

            context.MerchantId = item.IntegrationSystemParameters.Where(m => m.Key == "MerchantId").Select(m => m.Value).FirstOrDefault() ?? "";
            context.ApiUser = item.IntegrationSystemParameters.Where(m => m.Key == "ApiUser").Select(m => m.Value).FirstOrDefault() ?? "";
            context.ApiPassword = item.IntegrationSystemParameters.Where(m => m.Key == "ApiPassword").Select(m => m.Value).FirstOrDefault() ?? "";
            context.UserAgent = item.IntegrationSystemParameters.Where(m => m.Key == "UserAgent").Select(m => m.Value).FirstOrDefault() ?? "";
            return context;
        }

        private async Task N11OrderSync(IntegrationSystemDto item)
        {
            try
            {
                _logger.LogInformation("N11 sipariş senkronizasyonu başlatıldı. Zaman: {Time}", DateTime.UtcNow);

                N11ApiContext context = GetN11ApiContext(item);

                var n11ShipmentPackages = await _n11.GetShipmentPackagesAsync(context);

                if (n11ShipmentPackages == null || !n11ShipmentPackages.Any())
                {
                    _logger.Warn("N11'dan hiç sipariş alınamadı.");
                    return;
                }

                N11ShipmentPackageMapper.ConfigureLogger(_logger);
                var orders = N11ShipmentPackageMapper.ToDtoList(n11ShipmentPackages);

                foreach (var order in orders)
                {
                    try
                    {
                        #region Exists Order
                        if (await _orderService.ExistsByOrderNoAsync(order.OrderNumber))
                        {
                            _logger.LogInformation("'{OrderNumber}' nolu sipariş zaten kayıtlı", order.OrderNumber);
                            continue;
                        }
                        #endregion

                        #region Customer
                        var customer = await _customerService.GetCustomerByEmailAsync(order.Customer.Email);
                        if (customer == null)
                        {
                            var createCustomer = _mapper.Map<CreateCustomerDto>(order.Customer);

                            customer = await CreateCustomer(createCustomer);

                            order.CustomerId = customer.Id;
                            order.Customer = null;
                        }
                        else
                        {
                            order.CustomerId = customer.Id;
                            order.Customer = null;
                        }
                        #endregion

                        #region Address
                        if (order.ShippingAddress != null)
                        {
                            var createShippingAddress = _mapper.Map<CreateAddressDto>(order.ShippingAddress);
                            var address = await CreateAddress(createShippingAddress);
                            order.ShippingAddressId = address.Id;
                            order.ShippingAddress = null;
                        }

                        if (order.BillingAddress != null)
                        {
                            var createShippingAddress = _mapper.Map<CreateAddressDto>(order.BillingAddress);
                            var address = await CreateAddress(createShippingAddress);
                            order.BillingAddressId = address.Id;
                            order.BillingAddress = null;
                        }
                        #endregion

                        #region OrderItems
                        foreach (var orderItem in order.OrderItems)
                        {
                            if (orderItem.Product != null)
                            {
                                var product = await _productService.GetProductByCodeAsync(orderItem.Product.Code);

                                if (product == null)
                                {
                                    _logger.Error($"{orderItem.Product.Code} kodlu ürün {order.OrderNumber} ' +nolu siparişte bulunamadı");
                                    continue;
                                }

                                orderItem.Product = null;
                                orderItem.ProductId = product.Id;
                            }
                        }
                        #endregion

                        #region Order
                        var createOrder = _mapper.Map<CreateOrderDto>(order);
                        await _orderService.CreateOrderAsync(createOrder);

                        _logger.LogInformation("'{OrderNo}' nolu sipariş başarıyla kaydedildi.", order.OrderNumber);
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex);
                    }
                }

                _logger.LogInformation("N11 sipariş senkronizasyonu tamamlandı. Zaman: {Time}", DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private N11ApiContext GetN11ApiContext(IntegrationSystemDto item)
        {
            N11ApiContext context = new N11ApiContext();

            if (!item.IntegrationSystemParameters.Where(m => m.Key == "AppKey").Any() || string.IsNullOrEmpty(item.IntegrationSystemParameters.Where(m => m.Key == "AppKey").Select(m => m.Value).FirstOrDefault()))
            {
                _logger.Error("N11 AppKey Ayarlanmamış");
            }

            if (!item.IntegrationSystemParameters.Where(m => m.Key == "AppSecret").Any() || string.IsNullOrEmpty(item.IntegrationSystemParameters.Where(m => m.Key == "AppSecret").Select(m => m.Value).FirstOrDefault()))
            {
                _logger.Error("N11 AppSecret Ayarlanmamış");
            }

            context.AppKey = item.IntegrationSystemParameters.Where(m => m.Key == "AppKey").Select(m => m.Value).FirstOrDefault() ?? "";
            context.AppSecret = item.IntegrationSystemParameters.Where(m => m.Key == "AppSecret").Select(m => m.Value).FirstOrDefault() ?? "";

            return context;
        }

        private async Task CommerceOrderSync(IntegrationSystemDto item)
        {
            string commerceType = item.IntegrationSystemParameters.Where(m => m.Key == "CommerceType").Select(m => m.Value).FirstOrDefault() ?? "";
            switch (commerceType)
            {
                case "Smartstore":
                    await SmartstoreOrderSync(item);
                    break;
                default:
                    _logger.Error("{0} eticaret sistemine ait sipariş çekme işlemi bulunamadı", commerceType);
                    break;
            }


        }

        private async Task SmartstoreOrderSync(IntegrationSystemDto item)
        {
            try
            {
                _logger.LogInformation("Smartstore sipariş senkronizasyonu başlatıldı. Zaman: {Time}", DateTime.UtcNow);

                SmartstoreApiContext context = GetSmartstoreApiContext(item);

                var smartstoreOrders = await _smartstore.GetOrdersAsync(context);

                if (smartstoreOrders == null || !smartstoreOrders.Any())
                {
                    _logger.Warn("Smartstore'dan hiç sipariş alınamadı.");
                    return;
                }

                SmartstoreOrderMapper.ConfigureLogger(_logger);
                var orders = SmartstoreOrderMapper.ToDtoList(smartstoreOrders);

                foreach (var order in orders)
                {
                    try
                    {
                        #region Exists Order
                        if (await _orderService.ExistsByOrderNoAsync(order.OrderNumber))
                        {
                            _logger.LogInformation("'{OrderNumber}' nolu sipariş zaten kayıtlı", order.OrderNumber);
                            continue;
                        }
                        #endregion

                        #region Customer
                        var customer = await _customerService.GetCustomerByEmailAsync(order.Customer.Email);
                        if (customer == null)
                        {
                            var createCustomer = _mapper.Map<CreateCustomerDto>(order.Customer);

                            customer = await CreateCustomer(createCustomer);

                            order.CustomerId = customer.Id;
                            order.Customer = null;
                        }
                        else
                        {
                            order.CustomerId = customer.Id;
                            order.Customer = null;
                        }
                        #endregion

                        #region Address
                        if (order.ShippingAddress != null)
                        {
                            var createShippingAddress = _mapper.Map<CreateAddressDto>(order.ShippingAddress);
                            var address = await CreateAddress(createShippingAddress);
                            order.ShippingAddressId = address.Id;
                            order.ShippingAddress = null;
                        }

                        if (order.BillingAddress != null)
                        {
                            var createShippingAddress = _mapper.Map<CreateAddressDto>(order.BillingAddress);
                            var address = await CreateAddress(createShippingAddress);
                            order.BillingAddressId = address.Id;
                            order.BillingAddress = null;
                        }
                        #endregion

                        #region OrderItems
                        foreach (var orderItem in order.OrderItems)
                        {
                            if (orderItem.Product != null)
                            {
                                var product = await _productService.GetProductByCodeAsync(orderItem.Product.Code);

                                if (product == null)
                                {
                                    _logger.Error($"{orderItem.Product.Code} kodlu ürün {order.OrderNumber} ' +nolu siparişte bulunamadı");
                                    continue;
                                }

                                orderItem.Product = null;
                                orderItem.ProductId = product.Id;
                            }
                        }
                        #endregion

                        #region Order
                        var createOrder = _mapper.Map<CreateOrderDto>(order);
                        await _orderService.CreateOrderAsync(createOrder);

                        _logger.LogInformation("'{OrderNo}' nolu sipariş başarıyla kaydedildi.", order.OrderNumber);
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex);
                    }
                }

                _logger.LogInformation("Smartstore sipariş senkronizasyonu tamamlandı. Zaman: {Time}", DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private SmartstoreApiContext GetSmartstoreApiContext(IntegrationSystemDto item)
        {
            SmartstoreApiContext context = new SmartstoreApiContext();

            if (!item.IntegrationSystemParameters.Where(m => m.Key == "ApiUrl").Any() || string.IsNullOrEmpty(item.IntegrationSystemParameters.Where(m => m.Key == "ApiUrl").Select(m => m.Value).FirstOrDefault()))
            {
                _logger.Error("Smartstore ApiUrl Ayarlanmamış");
            }

            if (!item.IntegrationSystemParameters.Where(m => m.Key == "ApiUser").Any() || string.IsNullOrEmpty(item.IntegrationSystemParameters.Where(m => m.Key == "ApiUser").Select(m => m.Value).FirstOrDefault()))
            {
                _logger.Error("Smartstore ApiUser Ayarlanmamış");
            }

            if (!item.IntegrationSystemParameters.Where(m => m.Key == "ApiPassword").Any() || string.IsNullOrEmpty(item.IntegrationSystemParameters.Where(m => m.Key == "ApiPassword").Select(m => m.Value).FirstOrDefault()))
            {
                _logger.Error("Smartstore ApiPassword Ayarlanmamış");
            }

            context.BaseUrl = item.IntegrationSystemParameters.Where(m => m.Key == "ApiUrl").Select(m => m.Value).FirstOrDefault() ?? "";
            context.ApiUser = item.IntegrationSystemParameters.Where(m => m.Key == "ApiUser").Select(m => m.Value).FirstOrDefault() ?? "";
            context.ApiPassword = item.IntegrationSystemParameters.Where(m => m.Key == "ApiPassword").Select(m => m.Value).FirstOrDefault() ?? "";

            return context;
        }

        private async Task<CustomerDto> CreateCustomer(CreateCustomerDto createCustomer)
        {
            createCustomer.CustomerType = 1;
            var customer = await _customerService.CreateCustomerAsync(createCustomer);

            return customer;
        }

        private async Task<AddressDto> CreateAddress(CreateAddressDto createAddress)
        {
            var address = await _addressService.AddAsync(createAddress);
            return address;
        }
    }
}
