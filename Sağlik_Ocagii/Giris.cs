using Sağlik_Ocagii;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp
{
    public partial class Giris : Form
    {
        public Giris()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private bool ValidateCredentials(string username, string password)
        {
            // Kullanıcı adı ve şifre kontrolü burada yapılır
            string filePath = "users.txt";

            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);
                foreach (var line in lines)
                {
                    string[] parts = line.Split(',');
                    if (parts.Length == 2 && parts[0] == username && parts[1] == password)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
     

        private void button2_Click_1(object sender, EventArgs e)
        {
            kayitol emlakkayit = new kayitol();
            this.Hide();
            emlakkayit.ShowDialog();
            this.Show();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            string username = girisKullanicAdi.Text;
            string password = girisSifre.Text;

            if (ValidateCredentials(username, password))
            {
                Form2 anaForm = new Form2();
                anaForm.Show();

                this.Hide();
            }
            else
            {
                MessageBox.Show("Kullanıcı adı veya şifre hatalı.");
            }

        }
    }
}
