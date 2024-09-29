using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Sağlik_Ocagii
{
    public partial class Doktor : Form
    {
        private SqlConnection con;
        private SqlDataAdapter sqlDataAdapter;
        private DataTable dataTable;

        public Doktor()
        {
            con = new SqlConnection("data source=.\\SQLExpress; database=Saglik_Ocagi; Trusted_Connection=true; TrustServerCertificate=true; User ID=saglikocagi; pwd=12345; ");
            sqlDataAdapter = new SqlDataAdapter("SELECT DosyaNo,Durum, Poliklinik, Yapilacakislem, SıraNo, Tarih, DoktorNo,Fiyat FROM Islemler", con);
            dataTable = new DataTable();  // DataTable örneğini başlat
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                con.Open();

                string Doktor_adı = textBox2.Text;
                string Doktor_Soyadı = textBox3.Text;
                string Doktor_dogumyeri = textBox10.Text;
                string Doktor_dogumtarihi = dateTimePicker1.Value.ToString("yyyy-MM-dd"); // Tarih formatını belirt
                string Doktor_unvan = comboBox3.SelectedItem.ToString();
                string Doktor_ceptel = textBox11.Text;

                // Yeni doktor eklemek için sorguyu oluştur
                string query = "INSERT INTO Doktorlar (Doktor_adı, Doktor_Soyadı, Doktor_dogumyeri, Doktor_dogumtarihi, Doktor_ceptel, Doktor_unvan) " +
                               "VALUES (@Doktor_adı, @Doktor_Soyadı, @Doktor_dogumyeri, @Doktor_dogumtarihi, @Doktor_ceptel, @Doktor_unvan)";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Doktor_adı", Doktor_adı);
                cmd.Parameters.AddWithValue("@Doktor_Soyadı", Doktor_Soyadı);
                cmd.Parameters.AddWithValue("@Doktor_dogumyeri", Doktor_dogumyeri);
                cmd.Parameters.AddWithValue("@Doktor_dogumtarihi", Doktor_dogumtarihi);
                cmd.Parameters.AddWithValue("@Doktor_ceptel", Doktor_ceptel);
                cmd.Parameters.AddWithValue("@Doktor_unvan", Doktor_unvan);
                cmd.ExecuteNonQuery();

                MessageBox.Show("Yeni doktor başarıyla eklendi.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata oluştu: " + ex.Message);
            }
            finally
            {
                YenileDataGridView();
                Temizle(this);
                con.Close();
            }
        }

        private void YenileDataGridView()
        {
            try
            {
                dataTable.Clear();
                sqlDataAdapter.Fill(dataTable);
                dataGridView1.DataSource = null;
                dataGridView1.DataSource = dataTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veriler yenilenirken hata oluştu: " + ex.Message);
            }
        }

        private void LoadDataToDataGridView()
        {
            try
            {
                con.Open();

                sqlDataAdapter = new SqlDataAdapter("SELECT * FROM Doktorlar", con);
                sqlDataAdapter.Fill(dataTable);

                dataGridView1.DataSource = dataTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veriler yüklenirken hata oluştu: " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

         void Temizle(Control control)
        {
            foreach (Control ctrl in control.Controls)
            {
                if (ctrl is TextBox)
                {
                    TextBox textBox = (TextBox)ctrl;
                    textBox.Text = "";
                }
                else if (ctrl is ComboBox)
                {
                    ComboBox comboBox = (ComboBox)ctrl;
                    comboBox.Items.Clear();
                    comboBox.SelectedIndex = -1;
                }

                if (ctrl.Controls.Count > 0)
                {
                    Temizle(ctrl);
                }
            }
        }


        private void Doktor_Load(object sender, EventArgs e)
        {
            LoadDataToDataGridView();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form2 form2 = new Form2();
            form2.Show();
        }
    }
}
