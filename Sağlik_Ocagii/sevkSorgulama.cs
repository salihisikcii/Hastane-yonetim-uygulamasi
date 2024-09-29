using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Sağlik_Ocagii
{
    public partial class sevkSorgulama : Form
    {
        private SqlConnection con;
        private SqlDataAdapter sqlDataAdapter;
        private DataTable dataTable;

        public sevkSorgulama()
        {
            InitializeComponent();
            con = new SqlConnection("data source=.\\SQLExpress; database=Saglik_Ocagi; Trusted_Connection=true; TrustServerCertificate=true; User ID=saglikocagi; pwd=12345; ");
            sqlDataAdapter = new SqlDataAdapter("SELECT DosyaNo,Durum, Poliklinik, Yapilacakislem, SıraNo, Tarih, DoktorNo,Fiyat FROM Islemler", con);
            dataTable = new DataTable();  // DataTable örneğini başlat
        }

        
             void button1_Click(object sender, EventArgs e)
            {
                DateTime baslangicTarihi = dateTimePicker1.Value;
                DateTime bitisTarihi = dateTimePicker2.Value;

                string durumFiltresi = "Hepsi";

                if (radioButton1.Checked)
                    durumFiltresi = "Taburcu Olmamış";
                else if (radioButton2.Checked)
                    durumFiltresi = "Taburcu Olmuş";

                VerileriGetir(baslangicTarihi, bitisTarihi, durumFiltresi);
            }


        private void VerileriGetir(DateTime baslangicTarihi, DateTime bitisTarihi, string durumFiltresi)
        {
            try
            {
                con.Open();

                // Islemler ve Hastalar tablolarını birleştirip verileri çek
                string query = "SELECT Islemler.DosyaNo, Islemler.Durum, Hastalar.Hasta_ad, Hastalar.Hasta_soyad, Hastalar.Hasta_tc, Islemler.Polikinlik, Islemler.YapılacakIslem, Islemler.SıraNo, Islemler.Tarih, Islemler.DoktorNo, Islemler.Fiyat "
                             + "FROM Islemler "
                             + "INNER JOIN Hastalar ON Islemler.DosyaNo = Hastalar.Hasta_DosyaNo "
                             + "WHERE Islemler.Tarih BETWEEN @BaslangicTarihi AND @BitisTarihi AND (@DurumFiltresi = 'Hepsi' OR Islemler.Durum = @DurumFiltresi)";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@BaslangicTarihi", baslangicTarihi);
                cmd.Parameters.AddWithValue("@BitisTarihi", bitisTarihi);
                cmd.Parameters.AddWithValue("@DurumFiltresi", durumFiltresi);

                DataTable dataTable = new DataTable();
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(cmd);
                sqlDataAdapter.Fill(dataTable);

                dataGridView1.DataSource = dataTable;
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

        private void button3_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form2 form2 = new Form2();
            form2.Show();
        }
    }
}
