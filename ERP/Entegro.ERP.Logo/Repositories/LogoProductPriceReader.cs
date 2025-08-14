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
    public class LogoProductPriceReader : IErpProductPriceReader
    {
        private readonly string _connectionString;

        public LogoProductPriceReader(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<ErpResponse<ProductPriceDto>> GetProductPricesAsync(int page, int pageSize)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var countSql = @"SELECT COUNT(*) FROM ENTEGRO_PRODUCT_PRICES";
            var totalCount = await connection.ExecuteScalarAsync<int>(countSql);

            var sql = @"
            SELECT * FROM ENTEGRO_PRODUCT_PRICES
            ORDER BY ProductCode
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            var productPrices = await connection.QueryAsync<ProductPriceDto>(sql, new { Offset = (page - 1) * pageSize, PageSize = pageSize });


            return new ErpResponse<ProductPriceDto>
            {
                Content = productPrices.ToList(),
                Page = page,
                Size = pageSize,
                TotalElements = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };
        }
    }
}
