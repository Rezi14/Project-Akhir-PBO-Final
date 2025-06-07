using Sistem_Reservasi_Hotel.Controller;
using System;
using System.Windows.Forms;

namespace Sistem_Reservasi_Hotel.View
{
    public partial class FormDaftar : Form
    {
        private DaftarController daftarController;

        public FormDaftar()
        {
            InitializeComponent();
            daftarController = new DaftarController();
        }

        private void btndaftar2_Click(object sender, EventArgs e)
        {
            string username = textusername2.Text.Trim();
            string password = textpassword2.Text;
            string konfirmasi = textkonfirmasi.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Username dan password harus diisi.", "Input Tidak Lengkap", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (password != konfirmasi)
            {
                MessageBox.Show("Password dan konfirmasi tidak cocok.", "Konfirmasi Gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool success = daftarController.Daftar(username, password);
            if (success)
            {
                MessageBox.Show("Akun berhasil ditambahkan!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
                FormLogin login = new FormLogin();
                login.Show();
            }
        }

        private void btnlogin2_Click(object sender, EventArgs e)
        {
            FormLogin formLogin = new FormLogin();
            formLogin.Show();
            this.Hide();
        }
    }
}
