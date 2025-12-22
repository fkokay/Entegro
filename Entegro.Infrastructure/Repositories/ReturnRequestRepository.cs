using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.IntegrationSystem;
using Entegro.Application.DTOs.IntegrationSystemParameter;
using Entegro.Application.DTOs.Order;
using Entegro.Application.DTOs.ReturnRequest;
using Entegro.Application.DTOs.ReturnRequestItem;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Domain.Entities.Checkout;
using Entegro.Domain.Enums;
using Entegro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace Entegro.Infrastructure.Repositories
{
    public class ReturnRequestRepository : IReturnRequestRepository
    {
        private readonly EntegroDbContext _context;

        public ReturnRequestRepository(EntegroDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ReturnRequest returnRequest)
        {
            await _context.ReturnRequests.AddAsync(returnRequest);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(ReturnRequest returnRequest)
        {
            var tracked = _context.ReturnRequests.Local.FirstOrDefault(b => b.Id == returnRequest.Id);
            if (tracked != null)
            {
                _context.ReturnRequests.Remove(tracked);
            }
            else
            {
                _context.ReturnRequests.Attach(returnRequest);
                _context.ReturnRequests.Remove(returnRequest);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsByCustomerNameAsync(string customerName)
        {
            return await _context.ReturnRequests.Include(x => x.Items).AsNoTracking().AnyAsync(rr => rr.CustomerFirstName == customerName);
        }

        public async Task<bool> ExistsByIdAsync(int id)
        {
            return await _context.ReturnRequests.AsNoTracking().AnyAsync(o => o.Id == id);
        }

        public async Task<bool> ExistsByOrderNumberAsync(string orderNumber)
        {
            return await _context.ReturnRequests
                .AsNoTracking()
                .AnyAsync(o => o.OrderNumber == orderNumber);
        }

        public async Task<bool> ExistsByReturnRequestStatusAsync(int requestStatus)
        {
            return await _context.ReturnRequests.AsNoTracking().AnyAsync(rr => rr.Items.Any(i => i.ReturnRequestStatusId == requestStatus));
        }

        public async Task<ReturnRequest?> GetByCustomerNameAsync(string name)
        {
            return await _context.ReturnRequests.Include(rr => rr.Items).FirstOrDefaultAsync(rr => rr.CustomerFirstName == name);
        }

        public async Task<ReturnRequest?> GetByIdAsync(int id)
        {
            return await _context.ReturnRequests.Include(rr => rr.Items).AsNoTracking().FirstOrDefaultAsync(rr => rr.Id == id);
        }

        public async Task<ReturnRequest?> GetByOrderNumberAsync(string orderNumber)
        {
            return await _context.ReturnRequests.Include(rr => rr.Items
            ).AsNoTracking().FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);
        }

        public async Task<ReturnRequest?> GetByReturnRequestStatusAsync(int requestStatus)
        {
            return await _context.ReturnRequests.Where(rr => rr.Items.Any(i => i.ReturnRequestStatusId == requestStatus))
            .Include(rr => rr.Items
                .Where(i => i.ReturnRequestStatusId == requestStatus))
            .AsNoTracking()
            .FirstOrDefaultAsync();
        }
        public async Task<ReturnListPageDto> GetReturnRequestPageAsync()
        {
            ReturnListPageDto returnListPage = new ReturnListPageDto();
            returnListPage.CreatedQuantity = await _context.ReturnRequestItems.Where(rr => rr.ReturnRequestStatusId == (int)ReturnRequestStatus.Created).CountAsync();
            returnListPage.WaitingInActionQuantity = await _context.ReturnRequestItems.Where(rr => rr.ReturnRequestStatusId == (int)ReturnRequestStatus.WaitingInAction).CountAsync();
            returnListPage.WaitingFraudCheckQuantity = await _context.ReturnRequestItems.Where(rr => rr.ReturnRequestStatusId == (int)ReturnRequestStatus.WaitingFraudCheck).CountAsync();
            returnListPage.UnresolvedQuantity = await _context.ReturnRequestItems.Where(rr => rr.ReturnRequestStatusId == (int)ReturnRequestStatus.Unresolved).CountAsync();
            returnListPage.RejectedQuantity = await _context.ReturnRequestItems.Where(rr => rr.ReturnRequestStatusId == (int)ReturnRequestStatus.Rejected).CountAsync();
            returnListPage.AcceptedQuantity = await _context.ReturnRequestItems.Where(rr => rr.ReturnRequestStatusId == (int)ReturnRequestStatus.Accepted).CountAsync();
            returnListPage.CancelledQuantity = await _context.ReturnRequestItems.Where(rr => rr.ReturnRequestStatusId == (int)ReturnRequestStatus.Cancelled).CountAsync();
            returnListPage.InAnalysisQuantity = await _context.ReturnRequestItems.Where(rr => rr.ReturnRequestStatusId == (int)ReturnRequestStatus.InAnalysis).CountAsync();
            return returnListPage;
        }
        public async Task<Application.DTOs.Common.PagedResult<ReturnRequest>> GetPagedAsync(GridCommand gridCommand)
        {
            var query = _context.ReturnRequests.Include(rr => rr.Items).OrderBy(b => b.Id).AsNoTracking();


            if (gridCommand.Columns != null)
            {
                foreach (var col in gridCommand.Columns)
                {
                    if (!string.IsNullOrEmpty(col.Search?.Value))
                    {
                        var searchVal = col.Search.Value.Trim('^', '$');

                        // ilgili property’nin tipini bul
                        var prop = typeof(ReturnRequest).GetProperty(col.Data);
                        if (prop == null) continue;

                        if (prop.PropertyType == typeof(string))
                        {
                            query = query.Where($"{col.Data}.Contains(@0)", searchVal);
                        }
                        else if (prop.PropertyType == typeof(int) || prop.PropertyType == typeof(int?))
                        {
                            if (int.TryParse(searchVal, out var intVal))
                                query = query.Where($"{col.Data} == @0", intVal);
                        }
                        else if (prop.PropertyType == typeof(bool) || prop.PropertyType == typeof(bool?))
                        {
                            if (bool.TryParse(searchVal, out var boolVal))
                                query = query.Where($"{col.Data} == @0", boolVal);
                        }
                        else if (prop.PropertyType == typeof(DateTime) || prop.PropertyType == typeof(DateTime?))
                        {
                            if (DateTime.TryParse(searchVal, out var dt))
                                query = query.Where($"{col.Data}.Date == @0", dt.Date);
                        }
                    }
                }
            }

            if (gridCommand.Search != null)
            {
                if (!string.IsNullOrEmpty(gridCommand.Search.Value))
                {
                    query = query.Where(b =>
                    b.CustomerFirstName.Contains(gridCommand.Search.Value)).AsQueryable();
                }
            }

            if (gridCommand.Order.Any())
            {
                foreach (var item in gridCommand.Order)
                {
                    query = query.OrderBy($"{gridCommand.Columns[item.Column].Data} {(item.Dir ?? "asc")}");
                }
            }
            else
            {
                query = query.OrderBy(b => b.Id);
            }

            var totalCount = await query.CountAsync();

            var request = await query
                .Skip(gridCommand.Start)
                .Take(gridCommand.Length)
                .AsNoTracking()
                .AsSplitQuery()
                .ToListAsync();

            return new Application.DTOs.Common.PagedResult<ReturnRequest>
            {
                Items = request,
                TotalCount = totalCount,
                PageNumber = gridCommand.Start / gridCommand.Length,
                PageSize = gridCommand.Length
            };
        }
        public async Task<Application.DTOs.Common.PagedResult<ReturnRequestListDto>> GetPagedAsync(GridCommand gridCommand, ReturnRequestListFilterDto filters, int returnrequestStatusId)
        {
            var query = _context.ReturnRequests
                .Include(rr => rr.Items)
                .AsNoTracking();

            if (filters != null)
            {
                if (!string.IsNullOrEmpty(filters.CustomerName))
                    query = query.Where(rr => (rr.CustomerFirstName + " " + rr.CustomerLastName).Contains(filters.CustomerName)
                    || (rr.CustomerLastName + " " + rr.CustomerFirstName).Contains(filters.CustomerName));

                if (!string.IsNullOrEmpty(filters.OrderNo))
                    query = query.Where(rr => rr.OrderNumber.Contains(filters.OrderNo));

                if (!string.IsNullOrEmpty(filters.ReturnReason))
                    query = query.Where(rr => rr.Items.Any(s => s.CustomerClaimReasonCode.Contains(filters.ReturnReason) || s.PlatformClaimReasonCode.Contains(filters.ReturnReason)));

                if (!string.IsNullOrEmpty(filters.Barcode))
                    query = query.Where(rr => rr.Items.Any(s => s.Barcode.Contains(filters.Barcode)));

                if (!string.IsNullOrEmpty(filters.ReturnCode))
                    query = query.Where(rr => rr.Items.Any(s => s.CustomerClaimReasonCode.Contains(filters.ReturnCode) || s.PlatformClaimReasonCode.Contains(filters.ReturnCode)));


                if (filters.StartDate.HasValue)
                    query = query.Where(rr => rr.ClaimDate >= filters.StartDate.Value);

                if (filters.EndDate.HasValue)
                    query = query.Where(rr => rr.ClaimDate <= filters.EndDate.Value);
            }


            if (gridCommand.Columns != null)
            {
                foreach (var col in gridCommand.Columns)
                {
                    if (!string.IsNullOrEmpty(col.Search?.Value))
                    {
                        var searchVal = col.Search.Value.Trim('^', '$');

                        // ilgili property’nin tipini bul
                        var propName = col.Data.Contains(".")
                        ? col.Data.Replace(".", "")
                        : col.Data;

                        var prop = typeof(ReturnRequestListDto).GetProperty(propName);
                        if (prop == null) continue;

                        if (prop.PropertyType == typeof(string))
                        {
                            query = query.Where($"{col.Data}.Contains(@0)", searchVal);
                        }
                        else if (prop.PropertyType == typeof(int) || prop.PropertyType == typeof(int?))
                        {
                            if (int.TryParse(searchVal, out var intVal))
                                query = query.Where($"{col.Data} == @0", intVal);
                        }
                        else if (prop.PropertyType == typeof(bool) || prop.PropertyType == typeof(bool?))
                        {
                            if (bool.TryParse(searchVal, out var boolVal))
                                query = query.Where($"{col.Data} == @0", boolVal);
                        }
                        else if (prop.PropertyType == typeof(DateTime) || prop.PropertyType == typeof(DateTime?))
                        {
                            if (DateTime.TryParse(searchVal, out var dt))
                                query = query.Where($"{col.Data}.Date == @0", dt.Date);
                        }
                    }
                }
            }

            if (gridCommand.Search != null)
            {
                if (!string.IsNullOrEmpty(gridCommand.Search.Value))
                {
                    query = query.Where(b =>
                    ((b.CustomerFirstName + " " + b.CustomerLastName).Contains(filters.CustomerName)
                    || (b.CustomerLastName + " " + b.CustomerFirstName).Contains(filters.CustomerName))).AsQueryable();
                }
            }

            if (gridCommand.Order.Any())
            {
                foreach (var item in gridCommand.Order)
                {
                    query = query.OrderBy($"{gridCommand.Columns[item.Column].Data} {(item.Dir ?? "asc")}");
                }
            }
            else
            {
                query = query.OrderBy(b => b.Id);
            }

            IOrderedQueryable<ReturnRequest> orderedQuery = null;
            if (gridCommand.Order.Any())
            {
                foreach (var item in gridCommand.Order)
                {
                    var field = string.IsNullOrEmpty(gridCommand.Columns[item.Column].Name)
                        ? gridCommand.Columns[item.Column].Data
                        : gridCommand.Columns[item.Column].Name;

                    if (orderedQuery == null)
                        orderedQuery = query.OrderBy($"{field} {(item.Dir ?? "asc")}");
                    else
                        orderedQuery = orderedQuery.ThenBy($"{field} {(item.Dir ?? "asc")}");
                }
                query = orderedQuery;
            }
            else
            {
                query = query.OrderBy(o => o.Id);
            }

            switch (returnrequestStatusId)
            {
                case 0: // Beklemede
                    query = query.Where(r => r.Items.Any(i => i.ReturnRequestStatusId == (int)ReturnRequestStatus.Created));
                    break;
                case 1: // hepsini getir
                    query = query;
                    break;
                case 5: // Aksiyon Bekleniyor
                    query = query.Where(r => r.Items.Any(i => i.ReturnRequestStatusId == (int)ReturnRequestStatus.WaitingInAction));
                    break;
                case 8: // İade onaylandı, fraud kontrolünde
                    query = query.Where(r => r.Items.Any(i => i.ReturnRequestStatusId == (int)ReturnRequestStatus.WaitingFraudCheck));
                    break;
                case 9: // Analizde
                    query = query.Where(r => r.Items.Any(i => i.ReturnRequestStatusId == (int)ReturnRequestStatus.InAnalysis));
                    break;
                case 12: // İhtilaflı
                    query = query.Where(r => r.Items.Any(i => i.ReturnRequestStatusId == (int)ReturnRequestStatus.Unresolved));
                    break;
                case 20: // Satıcı tarafından kabul edildi
                    query = query.Where(r => r.Items.Any(i => i.ReturnRequestStatusId == (int)ReturnRequestStatus.Accepted));
                    break;
                case 30: // Satıcı tarafından reddedildi
                    query = query.Where(r => r.Items.Any(i => i.ReturnRequestStatusId == (int)ReturnRequestStatus.Rejected));
                    break;
                case 40: // İade iptal edildi
                    query = query.Where(r => r.Items.Any(i => i.ReturnRequestStatusId == (int)ReturnRequestStatus.Cancelled));
                    break;
            }

            var totalCount = await query.CountAsync();
            var returnRequest = await query
    .Select(r => new ReturnRequestListDto
    {
        Id = r.Id,
        IntegrationSystemId = r.IntegrationSystemId,
        IntegrationSystem = r.IntegrationSystem != null
            ? new IntegrationSystemDto
            {
                Id = r.IntegrationSystem.Id,
                Name = r.IntegrationSystem.Name,
                Description = r.IntegrationSystem.Description,
                IntegrationSystemType = r.IntegrationSystem.IntegrationSystemType,
                IntegrationSystemTypeId = r.IntegrationSystem.IntegrationSystemTypeId,
                IntegrationSystemParameters = r.IntegrationSystem.IntegrationSystemParameters
                    .Select(p => new IntegrationSystemParameterDto
                    {
                        Id = p.Id,
                        IntegrationSystemId = p.IntegrationSystemId,
                        Key = p.Key,
                        Value = p.Value
                    }).ToList()
            }
            : null,

        OrderNumber = r.OrderNumber,
        OrderDate = r.OrderDate,
        ClaimDate = r.ClaimDate,
        CustomerFirstName = r.CustomerFirstName,
        CustomerLastName = r.CustomerLastName,

        CargoTrackingNumber = r.CargoTrackingNumber,
        CargoProviderName = r.CargoProviderName,
        CargoTrackingLink = r.CargoTrackingLink,

        SubTotal = r.Items.Sum(i => i.Price),

        Items = r.Items.Select(i => new ReturnRequestItemListDto
        {
            Id = i.Id,
            ReturnRequestId = i.ReturnRequestId,
            Barcode = i.Barcode,
            CustomerClaimReasonCode = i.CustomerClaimReasonCode,
            CustomerClaimReasonName = i.CustomerClaimReasonName,
            CustomerNote = i.CustomerNote,
            MerchantSku = i.MerchantSku,
            Price = i.Price,
            ProductId = i.ProductId,
            ProductImageUrl = i.ProductImageUrl,
            ProductName = i.ProductName,
            ProductSize = i.ProductSize,
            ReturnRequestStatusId = i.ReturnRequestStatusId,
            ReturnRequestStatus = i.ReturnRequestStatus
        }).ToList()
    })
    .Skip(gridCommand.Start)
    .Take(gridCommand.Length)
    .ToListAsync();


            return new Application.DTOs.Common.PagedResult<ReturnRequestListDto>
            {
                Items = returnRequest,
                TotalCount = totalCount,
                PageNumber = (gridCommand.Start / gridCommand.Length) + 1,
                PageSize = gridCommand.Length
            };
        }
        public async Task UpdateAsync(ReturnRequest returnRequest)
        {
            returnRequest.UpdatedOnUtc = DateTime.UtcNow;
            _context.ReturnRequests.Update(returnRequest);
            await _context.SaveChangesAsync();
        }
    }
}
