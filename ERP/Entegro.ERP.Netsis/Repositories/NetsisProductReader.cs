using Dapper;
using Entegro.ERP.Abstractions.DTOs;
using Entegro.ERP.Abstractions.Interfaces;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.ERP.Netsis.Repositories
{
    public class NetsisProductReader : IErpProductReader
    {
        private readonly string _connectionString;

        public NetsisProductReader(string connectionString)
        {
            _connectionString = connectionString;
        }
        public async Task<ErpResponse<ProductDto>> GetProductsAsync(int page, int pageSize)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var countSql = @"SELECT COUNT(*) FROM ENTEGRO_PRODUCTS";
            var totalCount = await connection.ExecuteScalarAsync<int>(countSql);

            var sql = @"
            SELECT * FROM ENTEGRO_PRODUCTS
            WHERE CODE='MRC2035'
            ORDER BY Code
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            var products = await connection.QueryAsync<ProductDto>(sql, new { Offset = (page - 1) * pageSize, PageSize = pageSize });

            foreach (var product in products)
            {
                var productVariantAttributes = await connection.QueryAsync<ProductVariantAttributeDto>("SELECT * FROM ENTEGRO_PRODUCT_VARIANTS WHERE ProductCode=@ProductCode", new { ProductCode = product.Code });
                product.ProductVariantAttributes = productVariantAttributes.ToList();
            }


            return new ErpResponse<ProductDto>
            {
                Content = products.ToList(),
                Page = page,
                Size = pageSize,
                TotalElements = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };
        }
    }
}
