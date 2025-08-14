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
    public class LogoCustomerReader : IErpCustomerReader
    {
        private readonly string _connectionString;

        public LogoCustomerReader(string connectionString)
        {
            _connectionString = connectionString;
        }
        public async Task<ErpResponse<CustomerDto>> GetCustomersAsync(int page, int pageSize)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var countSql = @"SELECT COUNT(*) FROM ENTEGRO_CUSTOMERS";
            var totalCount = await connection.ExecuteScalarAsync<int>(countSql);

            var sql = @"
            SELECT * FROM ENTEGRO_CUSTOMERS
            ORDER BY Code
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            var customers = await connection.QueryAsync<CustomerDto>(sql, new { Offset = (page - 1) * pageSize, PageSize = pageSize });


            return new ErpResponse<CustomerDto>
            {
                Content = customers.ToList(),
                Page = page,
                Size = pageSize,
                TotalElements = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };
        }
    }
}
