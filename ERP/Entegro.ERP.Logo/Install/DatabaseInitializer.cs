using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.ERP.Logo.Install
{
    public class DatabaseInitializer
    {
        private readonly string _connectionString;

        public DatabaseInitializer(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task EnsureViewsAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var checkViewCmd = new SqlCommand(
                "SELECT OBJECT_ID('vw_LogoProducts', 'V')", connection);

            var result = await checkViewCmd.ExecuteScalarAsync();

            if (result == DBNull.Value || result == null)
            {
                var createViewSql = @"
                CREATE VIEW vw_LogoProducts AS
                SELECT 
                    ProductId AS Id,
                    ProductName AS Name,
                    ProductSku AS Sku,
                    UnitPrice AS Price,
                    AvailableStock AS StockQuantity
                FROM LogoProductsTable
                WHERE IsActive = 1;
            ";

                using var createCmd = new SqlCommand(createViewSql, connection);
                await createCmd.ExecuteNonQueryAsync();
            }
        }
    }
}
