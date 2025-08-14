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
    public class LogoCustomerBalanceReader : IErpCustomerBalanceReader
    {
        private readonly string _connectionString;

        public LogoCustomerBalanceReader(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<ErpResponse<CustomerBalanceDto>> GetCustomerBalancesAsync(int page, int pageSize)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var countSql = @"SELECT COUNT(*) FROM ENTEGRO_CUSTOMER_BALANCES";
            var totalCount = await connection.ExecuteScalarAsync<int>(countSql);

            var sql = @"
            SELECT * FROM ENTEGRO_CUSTOMER_BALANCES
            ORDER BY CustomerCode
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            var customerBalances = await connection.QueryAsync<CustomerBalanceDto>(sql, new { Offset = (page - 1) * pageSize, PageSize = pageSize });


            return new ErpResponse<CustomerBalanceDto>
            {
                Content = customerBalances.ToList(),
                Page = page,
                Size = pageSize,
                TotalElements = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };
        }
    }
}
