using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Domain.Entities;
using Entegro.Domain.Entities.Catalog;
using Entegro.Domain.Entities.Checkout;
using Entegro.Domain.Entities.Common;
using Entegro.Domain.Entities.Content;
using Entegro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Order = Entegro.Domain.Entities.Checkout.Order;

namespace Entegro.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly EntegroContext _context;
        private readonly IProductRepository _productRepository;
        public OrderRepository(EntegroContext context, IProductRepository productRepository)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        }
        public async Task AddAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Order order)
        {
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsByOrderNoAsync(string orderNo) => await _context.Orders.AnyAsync(p => p.OrderNumber == orderNo);

        public async Task<List<Order>> GetAllAsync()
        {
            return await _context.Orders.AsNoTracking().ToListAsync();
        }

        public async Task<PagedResult<Order>> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = _context.Orders.AsNoTracking().AsQueryable();

            var totalCount = await query.CountAsync();

            var orders = await query.Select(o => new Order
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                OrderDate = o.OrderDate,
                CustomerId = o.CustomerId,
                OrderTotal = o.OrderTotal,
                Deleted = o.Deleted,
                IsTransient = o.IsTransient,
                OrderSource = o.OrderSource,
                OrderSourceId = o.OrderSourceId,
                Customer = o.Customer,
                OrderItems = o.OrderItems.Select(i => new OrderItem
                {
                    Id = i.Id,
                    DiscountAmount = i.DiscountAmount,
                    OrderId = i.OrderId,
                    Price = i.Price,
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    TaxRate = i.TaxRate,
                    UnitPrice = i.UnitPrice
                }).ToList()
            }).OrderBy(m => m.Id)
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .ToListAsync();


            return new PagedResult<Order>
            {
                Items = orders,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<Order?> GetByIdAsync(int id)
        {
            var order = await _context.Orders.Select(o => new Order
            {
                Id = o.Id,
                CustomerId = o.CustomerId,
                Deleted = o.Deleted,
                VatNumber = o.VatNumber,
                CurrencyRate = o.CurrencyRate,
                CustomerIp = o.CustomerIp,
                OrderDiscount = o.OrderDiscount,
                OrderGuid = o.OrderGuid,
                OrderShippingExclTax = o.OrderShippingExclTax,
                OrderShippingInclTax = o.OrderShippingInclTax,
                OrderShippingTaxRate = o.OrderShippingTaxRate,
                OrderStatus = o.OrderStatus,
                OrderStatusId = o.OrderStatusId,
                OrderSubTotalDiscountExclTax = o.OrderSubTotalDiscountExclTax,
                OrderSubTotalDiscountInclTax = o.OrderSubTotalDiscountInclTax,
                OrderSubtotalExclTax = o.OrderSubtotalExclTax,
                OrderSubtotalInclTax = o.OrderSubtotalInclTax,
                OrderTax = o.OrderTax,
                PaidDateUtc = o.PaidDateUtc == null ? null : o.PaidDateUtc,
                PaymentMethodAdditionalFeeExclTax = o.PaymentMethodAdditionalFeeExclTax,
                PaymentMethodAdditionalFeeInclTax = o.PaymentMethodAdditionalFeeInclTax,
                PaymentMethodAdditionalFeeTaxRate = o.PaymentMethodAdditionalFeeTaxRate,
                PaymentMethodSystemName = o.PaymentMethodSystemName,
                PaymentStatus = o.PaymentStatus,
                PaymentStatusId = o.PaymentStatusId,
                RefundedAmount = o.RefundedAmount,
                Shipments = o.Shipments.Select(x => new Shipment
                {
                    Id = x.Id,
                    CreatedOnUtc = x.CreatedOnUtc,
                    DeliveryDateUtc = x.DeliveryDateUtc,
                    OrderId = x.OrderId,
                    ShippedDateUtc = x.ShippedDateUtc,
                    TrackingNumber = x.TrackingNumber,
                    TotalWeight = x.TotalWeight,
                    TrackingUrl = x.TrackingUrl,
                    ShipmentItems = x.ShipmentItems.Select(s => new ShipmentItem
                    {
                        Id = s.Id,
                        OrderItemId = s.OrderItemId,
                        Quantity = s.Quantity,
                        ShipmentId = s.ShipmentId
                    }).ToList()
                }).ToList(),
                ShippingMethod = o.ShippingMethod,
                TaxRates = o.TaxRates,
                ShippingStatus = o.ShippingStatus,
                ShippingStatusId = o.ShippingStatusId,
                IsTransient = o.Deleted,
                OrderNumber = o.OrderNumber,
                Customer = new Customer
                {
                    Id = o.Customer.Id,
                    Name = o.Customer.Name,
                    Email = o.Customer.Email,
                    PhoneNumber = o.Customer.PhoneNumber
                },
                OrderDate = o.OrderDate,
                OrderSource = o.OrderSource,
                OrderSourceId = o.OrderSourceId,
                OrderTotal = o.OrderTotal,
                BillingAddress = o.BillingAddress == null ? null : new Address
                {
                    CityId = o.BillingAddress.CityId,
                    LastName = o.BillingAddress.LastName,
                    Address1 = o.BillingAddress.Address1,
                    Address2 = o.BillingAddress.Address2,
                    AddressType = o.BillingAddress.AddressType,
                    Company = o.BillingAddress.Company,
                    CountryId = o.BillingAddress.CountryId,
                    CreatedOnUtc = o.BillingAddress.CreatedOnUtc,
                    DistrictId = o.BillingAddress.DistrictId,
                    Email = o.BillingAddress.Email,
                    FaxNumber = o.BillingAddress.FaxNumber,
                    FirstName = o.BillingAddress.FirstName,
                    Id = o.BillingAddress.Id,
                    PhoneNumber = o.BillingAddress.PhoneNumber,
                    Salutation = o.BillingAddress.Salutation,
                    TaxOffice = o.BillingAddress.TaxOffice,
                    TaxOfficeNumber = o.BillingAddress.TaxOfficeNumber,
                    Title = o.BillingAddress.Title,
                    TownId = o.BillingAddress.Id,
                    UpdatedOnUtc = o.BillingAddress.UpdatedOnUtc,
                    ZipPostalCode = o.BillingAddress.ZipPostalCode,
                },
                BillingAddressId = o.BillingAddressId == null ? null : o.BillingAddressId,
                ShippingAddress = o.ShippingAddress == null ? null : new Address
                {
                    CityId = o.ShippingAddress.CityId,
                    LastName = o.ShippingAddress.LastName,
                    Address1 = o.ShippingAddress.Address1,
                    Address2 = o.ShippingAddress.Address2,
                    AddressType = o.ShippingAddress.AddressType,
                    Company = o.ShippingAddress.Company,
                    CountryId = o.ShippingAddress.CountryId,
                    CreatedOnUtc = o.ShippingAddress.CreatedOnUtc,
                    DistrictId = o.ShippingAddress.DistrictId,
                    Email = o.ShippingAddress.Email,
                    FaxNumber = o.ShippingAddress.FaxNumber,
                    FirstName = o.ShippingAddress.FirstName,
                    Id = o.ShippingAddress.Id,
                    PhoneNumber = o.ShippingAddress.PhoneNumber,
                    Salutation = o.ShippingAddress.Salutation,
                    TaxOffice = o.ShippingAddress.TaxOffice,
                    TaxOfficeNumber = o.ShippingAddress.TaxOfficeNumber,
                    Title = o.ShippingAddress.Title,
                    TownId = o.ShippingAddress.Id,
                    UpdatedOnUtc = o.ShippingAddress.UpdatedOnUtc,
                    ZipPostalCode = o.ShippingAddress.ZipPostalCode,
                },
                ShippingAddressId = o.ShippingAddressId,
                OrderNotes = o.OrderNotes.Select(x => new OrderNote
                {
                    Id = x.Id,
                    CreatedOnUtc = DateTime.UtcNow,
                    Note = x.Note,
                    OrderId = x.OrderId,
                }).ToList(),
                OrderItems = o.OrderItems.Select(x => new OrderItem
                {
                    Id = x.Id,
                    DiscountAmount = x.DiscountAmount,
                    OrderId = x.OrderId,
                    Price = x.Price,
                    ProductId = x.ProductId,
                    Quantity = x.Quantity,
                    TaxRate = x.TaxRate,
                    UnitPrice = x.UnitPrice,
                    Product = new Product
                    {
                        Id = x.Product.Id,
                        Name = x.Product.Name,
                        Code = x.Product.Code,
                        MainPictureId = x.Product.MainPictureId,
                        MainPicture = x.Product.MainPicture == null ? null : new MediaFile()
                        {
                            Alt = x.Product.MainPicture.Alt,
                            CreatedOn = x.Product.MainPicture.CreatedOn,
                            Deleted = x.Product.MainPicture.Deleted,
                            Extension = x.Product.MainPicture.Extension,
                            MediaFolder = x.Product.MainPicture.MediaFolder,
                            MediaFolderId = x.Product.MainPicture.MediaFolderId,
                            Height = x.Product.MainPicture.Height,
                            Hidden = x.Product.MainPicture.Hidden,
                            Id = x.Product.MainPicture.Id,
                            IsTransient = x.Product.MainPicture.IsTransient,
                            MediaType = x.Product.MainPicture.MediaType,
                            Metadata = x.Product.MainPicture.Metadata,
                            MimeType = x.Product.MainPicture.MimeType,
                            Name = x.Product.MainPicture.Name,
                            PixelSize = x.Product.MainPicture.PixelSize,
                            Size = x.Product.MainPicture.Size,
                            Title = x.Product.MainPicture.Title,
                            UpdatedOn = x.Product.MainPicture.UpdatedOn,
                            Version = x.Product.MainPicture.Version,
                            Width = x.Product.MainPicture.Width
                        },
                    }
                }).ToList()
            }).FirstOrDefaultAsync(o => o.Id == id);

            if (order is null)
                return new Order();


            return order;
        }

        public async Task UpdateAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }
    }
}
