using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sağlik_Ocagii
{
    static public class dosysacek
    {
        public static string dosyaNo = "";

    }
    public partial class Dosyabulmaca : Form
    {
        private SqlConnection con;
        private SqlDataAdapter sqlDataAdapter;
        private DataTable dataTable;

        string tc = "";


        public Dosyabulmaca()
        {

            con = new SqlConnection("data source=.\\SQLExpress; database=Saglik_Ocagi; Trusted_Connection=true; TrustServerCertificate=true; User ID=saglikocagi; pwd=12345; ");
            sqlDataAdapter = new SqlDataAdapter("SELECT DosyaNo,Durum, Poliklinik, Yapilacakislem, SıraNo, Tarih, DoktorNo,Fiyat FROM Islemler", con);
            dataTable = new DataTable();  // DataTable örneğini başlat
            InitializeComponent();
        }


        public void GetDosyaNoByTC(string tc)
        {
            try
            {
                con.Open();

                string query = "SELECT Hasta_DosyaNo FROM Hastalar WHERE Hasta_tc = @tc";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@tc", tc);

                // ExecuteScalar ile tek bir değer (DosyaNo) alınır
                object result = cmd.ExecuteScalar();

                if (result != null)
                {
                    dosysacek.dosyaNo = result.ToString();
                }
                else
                {
                    MessageBox.Show("TC'ye uygun bir kayıt bulunamadı.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata oluştu: " + ex.Message);
            }
            finally
            {
                con.Close();
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            tc = textBox2.Text;
            if (tc != "")
            {
                GetDosyaNoByTC(tc);
                if (dosysacek.dosyaNo != "")
                {
                    this.Close();

                }
            }
            else
            {
                MessageBox.Show("Lütfen Bir Tc giriniz.");
            }
        }
    }
}
