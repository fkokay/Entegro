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

            var countSql = @"SELECT COUNT(*) FROM TBLSTSABIT";
            var totalCount = await connection.ExecuteScalarAsync<int>(countSql);

            var sql = @"
            SELECT dbo.TRK(STOK_ADI) AS Name,dbo.TRK(STOK_KODU) AS Code FROM TBLSTSABIT
            WHERE STOK_KODU IS NOT NULL AND STOK_ADI IS NOT NULL
            ORDER BY STOK_KODU
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
