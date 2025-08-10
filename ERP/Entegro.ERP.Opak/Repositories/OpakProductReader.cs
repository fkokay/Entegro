using Dapper;
using Entegro.ERP.Abstractions.DTOs;
using Entegro.ERP.Abstractions.Interfaces;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.ERP.Opak.Repositories
{
    public class OpakProductReader : IErpProductReader
    {
        private readonly string _connectionString;

        public OpakProductReader(string connectionString)
        {
            _connectionString = connectionString;
        }
        public async Task<ErpResponse<ProductDto>> GetProductsAsync(int page, int pageSize)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var countSql = @"SELECT COUNT(*) FROM VOW_STOKLAR_ozgurtek";
            var totalCount = await connection.ExecuteScalarAsync<int>(countSql);

            var sql = @"
            SELECT STOK_ADI AS Name,STOK_KOD AS Code FROM VOW_STOKLAR_ozgurtek
            ORDER BY STOK_KOD
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
