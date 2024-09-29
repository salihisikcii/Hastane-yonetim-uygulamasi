using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp
{
    public partial class kayitol: Form
    {
        public kayitol()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

      
        private void KayitEkle(string kullaniciAdi, string sifre)
        {
            string filePath = "users.txt";

            // Kullanıcıyı kontrol et, daha önce aynı kullanıcı adı var mı?
            if (!KullaniciVarmi(filePath, kullaniciAdi))
            {
                // Yeni kullanıcıyı dosyaya ekle
                using (StreamWriter sw = File.AppendText(filePath))
                {
                    sw.WriteLine($"{kullaniciAdi},{sifre}");
                }

                MessageBox.Show("Kayıt başarıyla eklendi.");
            }
            else
            {
                MessageBox.Show("Bu kullanıcı adı zaten var, lütfen farklı bir kullanıcı adı seçin.");
            }
        }
        private bool KullaniciVarmi(string filePath, string kullaniciAdi)
        {
            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);
                foreach (var line in lines)
                {
                    string[] parts = line.Split(',');
                    if (parts.Length == 2 && parts[0] == kullaniciAdi)
                    {
                        return true; // Kullanıcı adı eşleşti
                    }
                }
            }
            return false; // Eşleşme bulunamadı
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            string yeniKullaniciAdi = kullaniciAdi.Text;
            string yeniSifre = sifre.Text;

            if (!string.IsNullOrEmpty(yeniKullaniciAdi) && !string.IsNullOrEmpty(yeniSifre))
            {
                // Kullanıcı adı ve şifre boş değilse kayıt işlemini gerçekleştir
                KayitEkle(yeniKullaniciAdi, yeniSifre);
            }
            else
            {
                MessageBox.Show("Kullanıcı adı ve şifre boş olamaz.");
            }
        }
    }
}
