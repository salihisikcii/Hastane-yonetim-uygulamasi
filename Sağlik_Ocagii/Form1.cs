using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Sağlik_Ocagii
{
    public partial class Form1 : Form
    {
        private SqlConnection con;
        private SqlDataAdapter sqlDataAdapter;
        private DataTable dataTable;

        public Form1()
        {
            InitializeComponent();

            con = new SqlConnection("data source=.\\SQLExpress; database=Saglik_Ocagi; Trusted_Connection=true; TrustServerCertificate=true; User ID=saglikocagi; pwd=12345; ");
            sqlDataAdapter = new SqlDataAdapter("SELECT DosyaNo,Durum, Poliklinik, Yapilacakislem, SıraNo, Tarih, DoktorNo,Fiyat FROM Islemler", con);
            dataTable = new DataTable();  

            textBox1.KeyDown += TextBox1_KeyDown;
        }

        private void TextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string dosyaNumarasi = textBox1.Text.Trim();
                DosyaNumarasinaGoreVeriCek(dosyaNumarasi);

                FiyatHesaplama();
            }
        }

        private void FiyatHesaplama()
        {
            string islemlerQuery = "SELECT Durum, Fiyat FROM Islemler WHERE  Durum = 'Taburcu Olmamış'";
            SqlCommand islemlerCmd = new SqlCommand(islemlerQuery, con);

            con.Open();

            int toplamFiyat = 0;

            using (SqlDataReader reader = islemlerCmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    int fiyat;
                    if (int.TryParse(reader["Fiyat"].ToString(), out fiyat))
                    {
                        toplamFiyat += fiyat;
                    }
                }
            }

            label12.Text = toplamFiyat.ToString();

            con.Close();
        }

        private void DosyaNumarasinaGoreVeriCek(string dosyaNumarasi)
        {
            // Islemler tablosundan veri çek
            string islemlerQuery = "SELECT DosyaNo,Durum, Polikinlik, YapılacakIslem, SıraNo, Tarih, DoktorNo,Fiyat FROM Islemler WHERE DosyaNo = @DosyaNumarasi";
            SqlCommand islemlerCmd = new SqlCommand(islemlerQuery, con);
            islemlerCmd.Parameters.AddWithValue("@DosyaNumarasi", dosyaNumarasi);

            con.Open();
            sqlDataAdapter.SelectCommand = islemlerCmd;
            dataTable.Clear();
            sqlDataAdapter.Fill(dataTable);
            dataGridView1.DataSource = dataTable;

            // Hastalar tablosundan veri çek
            string hastalarQuery = "SELECT Hasta_ad, Hasta_soyad, Hasta_tc,Hasta_cinsiyeti,Hasta_kangrubu,Hasta_dogumyeri,Hasta_dogumtarihi,Hasta_telno,Hasta_adres FROM Hastalar WHERE Hasta_DosyaNo = @DosyaNumarasi";
            SqlCommand hastalarCmd = new SqlCommand(hastalarQuery, con);
            hastalarCmd.Parameters.AddWithValue("@DosyaNumarasi", dosyaNumarasi);

            using (SqlDataReader reader = hastalarCmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    textBox2.Text = reader["Hasta_ad"].ToString();
                    textBox3.Text = reader["Hasta_soyad"].ToString();
                    textBox4.Text = reader["Hasta_tc"].ToString();
                    textBox7.Text = reader["Hasta_cinsiyeti"].ToString();
                    textBox8.Text = reader["Hasta_kangrubu"].ToString();
                    DateTime dogumTarihi = Convert.ToDateTime(reader["Hasta_dogumtarihi"]);
                    textBox10.Text = dogumTarihi.ToString("dd.MM.yyyy");
                    textBox11.Text = reader["Hasta_telno"].ToString();
                    textBox9.Text = reader["Hasta_dogumyeri"].ToString();
                    richTextBox1.Text = reader["Hasta_adres"].ToString();

                }
                else
                {
                    textBox2.Text = "";
                    textBox3.Text = "";
                    textBox4.Text = "";
                    textBox7.Text = "";
                    textBox8.Text = "";
                    textBox10.Text = "";
                    textBox11.Text = "";
                    richTextBox1.Text = "";
                }
            }

            con.Close();
        }

        // Yeni numara al
      

        private void button4_Click_1(object sender, EventArgs e)
        {
            try
            {
                con.Open();


                string tarih = DateTime.Now.ToString("yyyy-MM-dd HH:mm");

                string Polikinlik = comboBox2.SelectedItem.ToString();
                string YapılacakIslem = comboBox3.SelectedItem.ToString();

                int doktorNo = Convert.ToInt32(textBox6.Text);
                int dosyaNo = Convert.ToInt32(textBox1.Text);
                int fiyat = Convert.ToInt32(textBox5.Text);

                string durum = "Taburcu Olmamış"; 

                int siraNo = 0;
                if (YapılacakIslem == "Muayene")
                {
                    string que = "SELECT MAX(SıraNo) FROM Islemler WHERE YapılacakIslem =@YapılacakIslem AND Polikinlik = @Polikinlik";
                    SqlCommand cmdn = new SqlCommand(que, con);
                    cmdn.Parameters.AddWithValue("@YapılacakIslem", YapılacakIslem);
                    cmdn.Parameters.AddWithValue("@Polikinlik", Polikinlik);
                    object result = cmdn.ExecuteScalar();
                    if (result != DBNull.Value)
                    {
                        siraNo = Convert.ToInt32(result) + 1;
                    }
                    else
                    {
                        siraNo = 1;
                    }
                }
                else
                {
                    siraNo = YeniSiraNoAl(YapılacakIslem);
                }

                string query = "INSERT INTO Islemler (Polikinlik, YapılacakIslem, DoktorNo, DosyaNo, Durum, SıraNo, Tarih, Fiyat) " +
                               "VALUES (@Polikinlik, @YapılacakIslem, @DoktorNo, @DosyaNo, @Durum, @SıraNo, @Tarih, @Fiyat)";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Polikinlik", Polikinlik);
                cmd.Parameters.AddWithValue("@YapılacakIslem", YapılacakIslem);
                cmd.Parameters.AddWithValue("@DoktorNo", doktorNo);
                cmd.Parameters.AddWithValue("@DosyaNo", dosyaNo);
                cmd.Parameters.AddWithValue("@Durum", durum);
                cmd.Parameters.AddWithValue("@SıraNo", siraNo);
                cmd.Parameters.AddWithValue("@Tarih", tarih);
                cmd.Parameters.AddWithValue("@Fiyat", fiyat);

                cmd.ExecuteNonQuery();
                MessageBox.Show("Yeni işlem başarıyla eklendi.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata oluştu: " + ex.Message);
            }
            finally
            {
                YenileDataGridView();

                con.Close();
                FiyatHesaplama();

            }

        }
        private int YeniSiraNoAl(string YapılacakIslem)
        {
            int siraNo = 1;

            string query = "SELECT MAX(SıraNo) FROM Islemler WHERE YapılacakIslem = @YapılacakIslem";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@YapılacakIslem", YapılacakIslem);

            object result = cmd.ExecuteScalar();

            if (result != DBNull.Value)
            {
                siraNo = Convert.ToInt32(result) + 1;
            }

            return siraNo;
        }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
                if (DateTime.TryParse(selectedRow.Cells["Tarih"].Value.ToString(), out DateTime selectedDate))
                {
                    string yapilacakIslem = selectedRow.Cells["YapılacakIslem"].Value.ToString();

                    SilmeIslemiGerceklestir(selectedDate, yapilacakIslem);
                }
                else
                {
                    MessageBox.Show("Seçili satırın tarih bilgisi alınamadı.");
                }
            }
            else
            {
                MessageBox.Show("Lütfen silmek istediğiniz satırı seçin.");
            }
        }

        private void SilmeIslemiGerceklestir(DateTime selectedDate, string yapilacakIslem)
        {
            try
            {
                con.Open();

                string query = "DELETE FROM Islemler WHERE Tarih = @SelectedDate AND YapılacakIslem = @YapilacakIslem";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@SelectedDate", selectedDate);
                cmd.Parameters.AddWithValue("@YapilacakIslem", yapilacakIslem);

                cmd.ExecuteNonQuery();

                MessageBox.Show("İşlem başarıyla silindi.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata oluştu: " + ex.Message);
            }
            finally
            {
                YenileDataGridView();

                con.Close();
                FiyatHesaplama();

            }
        }

        private void YenileDataGridView()
        {
            dataTable.Clear();
            sqlDataAdapter.Fill(dataTable);
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = dataTable;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }



        private void button2_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = null;
            Temizle(this);
            label12.Text = "0";
        }
        private void Temizle(Control control)
        {
            foreach (Control ctrl in control.Controls)
            {
                if (ctrl is TextBox)
                {
                    TextBox textBox = (TextBox)ctrl;
                    textBox.Text = "";
                }
                else if (ctrl is RichTextBox)
                {
                    RichTextBox richTextBox = (RichTextBox)ctrl;
                    richTextBox.Text = "";
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

        private void button5_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Seçili satır varsa
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];

                selectedRow.Cells["Durum"].Value = "Taburcu Olmuş";

                // Tarih ve YapılacakIslem bilgilerini al
                if (DateTime.TryParse(selectedRow.Cells["Tarih"].Value.ToString(), out DateTime selectedDate))
                {
                    string yapilacakIslem = selectedRow.Cells["YapılacakIslem"].Value.ToString();

                    // Durumu güncelleme metodu çağır
                    DurumuGuncelle("Taburcu Olmuş", selectedDate, yapilacakIslem);
                }
                else
                {
                    MessageBox.Show("Seçili satırın tarih bilgisi alınamadı.");
                }
            }
            else
            {
                MessageBox.Show("Lütfen güncellemek istediğiniz satırı seçin.");
            }
        }

        private void DurumuGuncelle(string yeniDurum, DateTime tarih, string yapilacakIslem)
        {
            try
            {
                con.Open();

                string query = "UPDATE Islemler SET Durum = @YeniDurum WHERE Tarih = @Tarih AND YapılacakIslem = @YapilacakIslem";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@YeniDurum", yeniDurum);
                cmd.Parameters.AddWithValue("@Tarih", tarih);
                cmd.Parameters.AddWithValue("@YapilacakIslem", yapilacakIslem);

                int affectedRows = cmd.ExecuteNonQuery();

                if (affectedRows > 0)
                {
                    MessageBox.Show("Durum başarıyla güncellendi.");
                }
                else
                {
                    MessageBox.Show("Güncellenecek durumu içeren kayıt bulunamadı.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata oluştu: " + ex.Message);
            }
            finally
            {
                YenileDataGridView();
                con.Close();
                FiyatHesaplama();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // İkinci formu aç
            Dosyabulmaca ikinciForm = new Dosyabulmaca();
            ikinciForm.ShowDialog();


            textBox1.Text = dosysacek.dosyaNo;
            
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form2 form2 = new Form2();
            form2.Show();
        }
    }
}
