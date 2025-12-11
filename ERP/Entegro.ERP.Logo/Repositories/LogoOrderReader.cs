using Dapper;
using Entegro.ERP.Abstractions.DTOs;
using Entegro.ERP.Abstractions.Interfaces;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.ERP.Logo.Repositories
{
    public class LogoOrderReader : IErpOrderReader
    {
        private readonly string _connectionString;
        public LogoOrderReader(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public async Task<ErpResponse<OrderDto>> GetOrdersAsync(int page, int pageSize)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var countSql = @"SELECT COUNT(*) FROM ENTEGRO_ORDERS";
            var totalCount = await connection.ExecuteScalarAsync<int>(countSql);

            var sql = @"
            SELECT * FROM ENTEGRO_ORDERS
            ORDER BY OrderNumber
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            var orders = await connection.QueryAsync<OrderDto>(sql, new { Offset = (page - 1) * pageSize, PageSize = pageSize });
            foreach (var item in orders)
            {
                var sqlItems = @"
                    SELECT * FROM ENTEGRO_ORDER_ITEMS
                    WHERE OrderNumber = @OrderNumber
                    ORDER BY OrderNumber";
                        item.OrderItems = (await connection.QueryAsync<OrderItemDto>(sqlItems, new { item.OrderNumber })).ToList();

                var sqlInvoiceAddress = @"
                    SELECT * FROM ENTEGRO_CUSTOMER_INVOCE_ADDRESS
                    WHERE CustomerCode = @CustomerCode
                    ORDER BY CustomerCode";
                item.InvoiceAddress = await connection.QueryFirstAsync<AddressDto>(sqlInvoiceAddress, new { item.CustomerCode });

                if (item.ShippingAddressId != 0 && item.ShippingAddressId != null)
                {
                    var sqlShippingAddress = @"
                    SELECT * FROM ENTEGRO_CUSTOMER_SHIPPING_ADDRESS
                    WHERE LOGICALREF = @ShippingAddressId
                    ORDER BY LOGICALREF";
                    item.ShippingAddress = await connection.QueryFirstAsync<AddressDto>(sqlShippingAddress, new { item.ShippingAddressId });
                }
            }


            return new ErpResponse<OrderDto>
            {
                Content = orders.ToList(),
                Page = page,
                Size = pageSize,
                TotalElements = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };
        }
    }
}
