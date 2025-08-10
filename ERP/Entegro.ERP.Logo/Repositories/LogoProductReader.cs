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
    public class LogoProductReader : IErpProductReader
    {
        private readonly string _connectionString;

        public LogoProductReader(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<ErpResponse<ProductDto>> GetProductsAsync(int page, int pageSize)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var countSql = @"SELECT COUNT(*) FROM LG_200_ITEMS WHERE CARDTYPE = 1 AND ACTIVE = 0";
            var totalCount = await connection.ExecuteScalarAsync<int>(countSql);

            var sql = @"
            SELECT CODE as Code,NAME as Name FROM LG_200_ITEMS WHERE CARDTYPE = 1 AND ACTIVE = 0
            ORDER BY LOGICALREF
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            var products = await connection.QueryAsync<ProductDto>(sql, new { Offset = (page - 1) * pageSize, PageSize = pageSize });


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
