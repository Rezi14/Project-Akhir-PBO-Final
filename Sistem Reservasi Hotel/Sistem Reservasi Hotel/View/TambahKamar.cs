using Npgsql;
using Sistem_Reservasi_Hotel.Controller;
using Sistem_Reservasi_Hotel.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sistem_Reservasi_Hotel.View
{
    public partial class TambahKamar : Form
    {
        public TambahKamar()
        {
            InitializeComponent();
            loadtipekamar();
        }

        private void btnTambahKamar2_Click(object sender, EventArgs e)
        {
            string nomor_kamar = textNomorKamar1.Text;
            //int id_tipe_kamar = int.Parse(comboBoxTipeKamar.Text);
            string selected = comboBoxTipeKamar.SelectedItem.ToString();
            int id_tipe_kamar = int.Parse(selected.Split('-')[0].Trim());
            bool status_kamar = cbStatusKamar.Checked;
            string deskripsi = textDeskripsi1.Text;

            // Validasi sederhana
            if (string.IsNullOrEmpty(nomor_kamar) || id_tipe_kamar == 0)
            {
                MessageBox.Show("Nomor kamar dan status kamar wajib diisi.");
                return;
            }

            // Buat objek kamar
            Kamar kamar = new Kamar
            {
                NomorKamar = nomor_kamar,
                IDTipeKamar = id_tipe_kamar,
                StatusKamar = status_kamar,
                Deskripsi = deskripsi
            };

            // Masukkan ke database
            KamarController.InsertKamar(kamar);

            // Refresh tampilan atau kosongkan form
            MessageBox.Show("Kamar berhasil ditambahkan!");


        }
        private void loadtipekamar()
        {
            comboBoxTipeKamar.Items.Clear();
            try
            {
                using (var conn = DbContext.GetConnection())
                {
                    conn.Open();
                    string query = @"
                    SELECT id_tipe_kamar, nama_tipe_kamar 
                    FROM tipe_kamar
                    ORDER BY id_tipe_kamar";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            comboBoxTipeKamar.Items.Add(reader.GetInt32(0) + " - " + reader.GetString(1));

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal load layanan:\n" + ex.Message);
            }

        }

        private void btnBatal_Click(object sender, EventArgs e)
        {
            FormKelolaKamar formKelolaKamar = new FormKelolaKamar();
            formKelolaKamar.Show();
            this.Hide();
        }

        private void textDeskripsi1_TextChanged(object sender, EventArgs e)
        {

        }

        private void TambahKamar_Load(object sender, EventArgs e)
        {

        }
    }
}
