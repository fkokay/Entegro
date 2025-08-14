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
            await InitializeProducts();
            await InitializeProductPrices();
            await InitializeProductStocks();
            await InitializeCustomers();
            await InitializeCustomerBalances();
        }

        private async Task InitializeProductStocks()
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var checkViewCmd = new SqlCommand("SELECT OBJECT_ID('ENTEGRO_PRODUCT_STOCKS', 'V')", connection);

            var result = await checkViewCmd.ExecuteScalarAsync();

            if (result == DBNull.Value || result == null)
            {
                var createViewSql = @"
                CREATE VIEW ENTEGRO_PRODUCT_STOCKS AS
                SELECT 
                ITEM.CODE AS ProductCode,
                ITEM.NAME AS ProductName,
                (SELECT ISNULL((SUM (GNSTITOT.ONHAND)-SUM (GNSTITOT.RESERVED)),0) AS STOCKAMOUNT FROM LV_200_01_GNTOTST GNSTITOT WITH(NOLOCK) WHERE (GNSTITOT.STOCKREF = ITEM.LOGICALREF) AND (GNSTITOT.INVENNO = -1)) AS Stock
                FROM 
                LG_200_ITEMS AS ITEM
                WHERE (ITEM.ACTIVE=0) AND (ITEM.CARDTYPE NOT IN(4,20,21));
                ";

                using var createCmd = new SqlCommand(createViewSql, connection);
                await createCmd.ExecuteNonQueryAsync();
            }
        }

        private async Task InitializeProductPrices()
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var checkViewCmd = new SqlCommand("SELECT OBJECT_ID('ENTEGRO_PRODUCT_PRICES', 'V')", connection);

            var result = await checkViewCmd.ExecuteScalarAsync();

            if (result == DBNull.Value || result == null)
            {
                var createViewSql = @"
                CREATE VIEW ENTEGRO_PRODUCT_PRICES AS
                SELECT 
                ITEM.CODE AS ProductCode,
                ITEM.NAME AS ProductName,	
                0 AS Price
                FROM 
                LG_200_ITEMS AS ITEM
                WHERE (ITEM.ACTIVE=0) AND (ITEM.CARDTYPE NOT IN(4,20,21));
                ";

                using var createCmd = new SqlCommand(createViewSql, connection);
                await createCmd.ExecuteNonQueryAsync();
            }
        }

        private async Task InitializeCustomerBalances()
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var checkViewCmd = new SqlCommand("SELECT OBJECT_ID('ENTEGRO_CUSTOMER_BALANCES', 'V')", connection);

            var result = await checkViewCmd.ExecuteScalarAsync();

            if (result == DBNull.Value || result == null)
            {
                var createViewSql = @"
                CREATE VIEW ENTEGRO_CUSTOMER_BALANCES AS
                SELECT
	            CODE AS CustomerCode,
	            DEFINITION_ AS CustomerName,
	            ISNULL((SELECT SUM(GNTOTCL.DEBIT) FROM LV_200_01_GNTOTCL AS GNTOTCL WITH(NOLOCK) WHERE CLCARD.LOGICALREF=GNTOTCL.CARDREF AND GNTOTCL.TOTTYP=1),0.00) AS Debit,
                ISNULL((SELECT SUM(GNTOTCL.CREDIT) FROM LV_200_01_GNTOTCL AS GNTOTCL WITH(NOLOCK) WHERE CLCARD.LOGICALREF=GNTOTCL.CARDREF AND GNTOTCL.TOTTYP=1),0.00) AS Credit,
                ISNULL((SELECT SUM(GNTOTCL.DEBIT)-SUM(GNTOTCL.CREDIT) FROM LV_200_01_GNTOTCL AS GNTOTCL WITH(NOLOCK) WHERE CLCARD.LOGICALREF=GNTOTCL.CARDREF AND GNTOTCL.TOTTYP=1),0.00) AS Balance
                FROM LG_200_CLCARD AS CLCARD WHERE ACTIVE=0 AND CARDTYPE <>  22;
                ";

                using var createCmd = new SqlCommand(createViewSql, connection);
                await createCmd.ExecuteNonQueryAsync();
            }
        }

        private async Task InitializeCustomers()
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var checkViewCmd = new SqlCommand("SELECT OBJECT_ID('ENTEGRO_CUSTOMERS', 'V')", connection);

            var result = await checkViewCmd.ExecuteScalarAsync();

            if (result == DBNull.Value || result == null)
            {
                var createViewSql = @"
                CREATE VIEW ENTEGRO_CUSTOMERS AS
                SELECT
	            CODE AS Code,
	            DEFINITION_ AS Name,
	            ISNULL((SELECT SUM(GNTOTCL.DEBIT) FROM LV_200_01_GNTOTCL AS GNTOTCL WITH(NOLOCK) WHERE CLCARD.LOGICALREF=GNTOTCL.CARDREF AND GNTOTCL.TOTTYP=1),0.00) AS Debit,
                ISNULL((SELECT SUM(GNTOTCL.CREDIT) FROM LV_200_01_GNTOTCL AS GNTOTCL WITH(NOLOCK) WHERE CLCARD.LOGICALREF=GNTOTCL.CARDREF AND GNTOTCL.TOTTYP=1),0.00) AS Credit,
                ISNULL((SELECT SUM(GNTOTCL.DEBIT)-SUM(GNTOTCL.CREDIT) FROM LV_200_01_GNTOTCL AS GNTOTCL WITH(NOLOCK) WHERE CLCARD.LOGICALREF=GNTOTCL.CARDREF AND GNTOTCL.TOTTYP=1),0.00) AS Balance
                FROM LG_200_CLCARD AS CLCARD WHERE ACTIVE=0 AND CARDTYPE <>  22;
                ";

                using var createCmd = new SqlCommand(createViewSql, connection);
                await createCmd.ExecuteNonQueryAsync();
            }
        }

        private async Task InitializeProducts()
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var checkViewCmd = new SqlCommand("SELECT OBJECT_ID('ENTEGRO_PRODUCTS', 'V')", connection);

            var result = await checkViewCmd.ExecuteScalarAsync();

            if (result == DBNull.Value || result == null)
            {
                var createViewSql = @"
                CREATE VIEW ENTEGRO_PRODUCTS AS
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
