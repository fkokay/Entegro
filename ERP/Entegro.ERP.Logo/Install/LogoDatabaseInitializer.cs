using Entegro.ERP.Abstractions.Interfaces;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.ERP.Logo.Install
{
    public class LogoDatabaseInitializer : IErpDatabaseInitializer
    {
        private readonly string _connectionString;

        public LogoDatabaseInitializer(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task EnsureViewsAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var checkViewCmd = new SqlCommand("SELECT OBJECT_ID('ENTEGRO_ITEMS', 'V')", connection);

            var result = await checkViewCmd.ExecuteScalarAsync();

            if (result == DBNull.Value || result == null)
            {
                var createViewSql = @"
                CREATE VIEW ENTEGRO_ITEMS AS
                SELECT
                    CODE AS Code, 
                    NAME AS Name, 
                    '' AS Description, 
                    0 AS Price, 
                    '' AS Currency, 
                    '' AS Unit, 
                    0 AS VatRate, 
                    0 AS VatInc, 
                    '' AS BrandName, 
                    '' AS GroupName, 
                    '' AS Barcode, 
                    '' AS Category1, 
                    '' AS Category2, 
                    '' AS Category3, 
                    '' AS Category4
                FROM dbo.LG_200_ITEMS
                WHERE (CARDTYPE = 1) AND (ACTIVE = 0) 
                AND (NAME IS NOT NULL) AND (CODE IS NOT NULL) 
                AND (LEN(NAME) > 0) AND (LEN(CODE) > 0);
                ";

                using var createCmd = new SqlCommand(createViewSql, connection);
                await createCmd.ExecuteNonQueryAsync();
            }
        }
    }
}
