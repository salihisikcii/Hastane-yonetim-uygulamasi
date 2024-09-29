using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sağlik_Ocagii
{
    public class VeriTabaniBaglanti
    {
        SqlConnection con = new SqlConnection("data source=.\\SQLExpress; database=Saglik_Ocagi; Trusted_Connection=true; TrustServerCertificate=true; User ID=saglikocagi; pwd=12345; ");

    }
}
