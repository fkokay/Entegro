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
    public class LogoProductStockReader : IErpProductStockReader
    {
        private readonly string _connectionString;

        public LogoProductStockReader(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<ErpResponse<ProductStockDto>> GetProductStocksAsync(int page, int pageSize)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var countSql = @"SELECT COUNT(*) FROM ENTEGRO_PRODUCT_STOCKS";
            var totalCount = await connection.ExecuteScalarAsync<int>(countSql);

            var sql = @"
            SELECT * FROM ENTEGRO_PRODUCT_STOCKS
            ORDER BY ProductCode
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            var productStocks = await connection.QueryAsync<ProductStockDto>(sql, new { Offset = (page - 1) * pageSize, PageSize = pageSize });


            return new ErpResponse<ProductStockDto>
            {
                Content = productStocks.ToList(),
                Page = page,
                Size = pageSize,
                TotalElements = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };
        }
    }
}
