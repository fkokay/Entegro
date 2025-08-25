using Entegro.ERP.Abstractions.Interfaces;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.ERP.Netsis.Install
{
    public class NetsisDatabaseInitializer : IErpDatabaseInitializer
    {
        private readonly string _connectionString;
        public NetsisDatabaseInitializer(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task EnsureViewsAsync()
        {
            await InitializeProducts();
            await InitializeProductVariants();
        }

        private async Task InitializeProductVariants()
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var checkViewCmd = new SqlCommand("SELECT OBJECT_ID('ENTEGRO_PRODUCT_VARIANTS', 'V')", connection);

            var result = await checkViewCmd.ExecuteScalarAsync();

            if (result == DBNull.Value || result == null)
            {
                var createViewSql = @"
                CREATE VIEW ENTEGRO_PRODUCT_VARIANTS AS
                SELECT 
                ProductCode = ISNULL(KULL1S,S.STOK_KODU),
                VariantCode = S.STOK_KODU,
                Variant1Name= DBO.TRK(KULL2S),
                Variant1Value = DBO.TRK(KULL3S ),
                Variant2Name = DBO.TRK(KULL4S),
                Variant2Value = DBO.TRK(KULL5S),
                Variant3Name = '',
                Variant3Value = '',
                Variant4Name = '',
                Variant4Value = '',
                Variant5Name= '',
                Variant5Value = '',
                Price = SATIS_FIAT1,
                StockQuantity=(SELECT TOP_GIRIS_MIK-TOP_CIKIS_MIK FROM TBLSTOKPH B WHERE DEPO_KODU=0 AND B.STOK_KODU=S.STOK_KODU)
                FROM TBLSTSABIT S 
                LEFT OUTER JOIN TBLSTSABITEK EK ON S.STOK_KODU=EK.STOK_KODU
                LEFT OUTER JOIN TBLSTGRUP G ON G.GRUP_KOD=S.GRUP_KODU
                LEFT OUTER JOIN TBLSTOKKOD1 K1 ON K1.GRUP_KOD=S.KOD_1
                LEFT OUTER JOIN TBLSTOKKOD2 K2 ON K2.GRUP_KOD=S.KOD_2
                LEFT OUTER JOIN TBLSTOKKOD3 K3 ON K3.GRUP_KOD=S.KOD_3
                LEFT OUTER JOIN TBLSTOKKOD4 K4 ON K4.GRUP_KOD=S.KOD_4
                LEFT OUTER JOIN TBLSTOKKOD5 K5 ON K5.GRUP_KOD=S.KOD_5

                WHERE KULL6S ='A'";

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
                Code= dbo.TRK(KULL1S),
                Name=dbo.TRK(MAX((STOK_ADI))),
                Price=MIN(SATIS_FIAT1),
                Currency = 'TL',
                VatInc=1,
                StockQunatity=0,
                Category1=dbo.TRK(MAX( G.GRUP_ISIM)),
                Category2=dbo.TRK(MAX( K1.GRUP_ISIM)),
                Category3=dbo.TRK(MAX( K2.GRUP_ISIM)),
                Category4=dbo.TRK(MAX( K3.GRUP_ISIM)), 
                BrandName=dbo.TRK(MAX( K4.GRUP_ISIM)),
                Description=dbo.TRK((SELECT TOP 1 A.BILGI FROM TBLSTOKBIL A WHERE A.STOK_KODU = MAX((S.STOK_KODU)))),
                VatRate = MAX(KDV_ORANI),
                Barcode = dbo.TRK(MAX(BARKOD1))
                FROM TBLSTSABITEK S 
                LEFT OUTER JOIN TBLSTSABIT ST ON S.STOK_KODU=ST.STOK_KODU 
                LEFT OUTER JOIN TBLSTGRUP G ON G.GRUP_KOD=GRUP_KODU
                LEFT OUTER JOIN TBLSTOKKOD1 K1 ON K1.GRUP_KOD=KOD_1
                LEFT OUTER JOIN TBLSTOKKOD2 K2 ON K2.GRUP_KOD=KOD_2
                LEFT OUTER JOIN TBLSTOKKOD3 K3 ON K3.GRUP_KOD=KOD_3
                LEFT OUTER JOIN TBLSTOKKOD3 K4 ON K4.GRUP_KOD=KOD_4
                LEFT OUTER JOIN TBLSTOKKOD4 K5 ON K5.GRUP_KOD=KOD_5 


                WHERE S.KULL1S IS NOT NULL

                GROUP BY KULL1S

                UNION ALL

                SELECT 
                Code=dbo.TRK(S.STOK_KODU),
                Name= dbo.TRK((STOK_ADI)),
                Price= (SATIS_FIAT1),
                Currency = 'TL',
                VatInc=1,
                StockQunatity=ISNULL((SELECT ISNULL(TOP_GIRIS_MIK-TOP_CIKIS_MIK,0) FROM TBLSTOKPH B WHERE DEPO_KODU=0 AND B.STOK_KODU=S.STOK_KODU),0),
                Category1= dbo.TRK((G.GRUP_ISIM)),
                Category2= dbo.TRK((K1.GRUP_ISIM)),
                Category3= dbo.TRK((K2.GRUP_ISIM)),
                Category4= dbo.TRK((K3.GRUP_ISIM)),
                BrandName= dbo.TRK((K4.GRUP_ISIM)),
                Description= dbo.TRK((SELECT TOP 1 A.BILGI FROM TBLSTOKBIL A WHERE A.STOK_KODU = S.STOK_KODU)),
                VatRate =  (KDV_ORANI),
                Barcode =  dbo.TRK((BARKOD1))
                FROM TBLSTSABITEK S 
                LEFT OUTER JOIN TBLSTSABIT ST ON S.STOK_KODU=ST.STOK_KODU 
                LEFT OUTER JOIN TBLSTGRUP G ON G.GRUP_KOD=GRUP_KODU
                LEFT OUTER JOIN TBLSTOKKOD1 K1 ON K1.GRUP_KOD=KOD_1
                LEFT OUTER JOIN TBLSTOKKOD2 K2 ON K2.GRUP_KOD=KOD_2
                LEFT OUTER JOIN TBLSTOKKOD3 K3 ON K3.GRUP_KOD=KOD_3
                LEFT OUTER JOIN TBLSTOKKOD3 K4 ON K4.GRUP_KOD=KOD_4
                LEFT OUTER JOIN TBLSTOKKOD4 K5 ON K5.GRUP_KOD=KOD_5 

                WHERE  ISNULL(S.KULL1S,'')  =''";

                using var createCmd = new SqlCommand(createViewSql, connection);
                await createCmd.ExecuteNonQueryAsync();
            }
        }
    }
}
