using Npgsql;
using Sistem_Reservasi_Hotel.Controller;
using Sistem_Reservasi_Hotel.Models;
using Sistem_Reservasi_Hotel.View;
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
    public partial class EditKamar : Form
    {
        private Kamar kamarToEdit;
        public EditKamar(Kamar kamar)
        {
            InitializeComponent();
            kamarToEdit = kamar;
           
        }

        private void EditKamar_Load(object sender, EventArgs e)
        {
            loadtipekamar();
            if (kamarToEdit != null)
            {
                textNomorKamar2.Text = kamarToEdit.NomorKamar;
                cbStatusKamar2.Checked = kamarToEdit.StatusKamar;
                textDeskripsi2.Text = kamarToEdit.Deskripsi;

                // Pilih item sesuai id_tipe_kamar
                foreach (var item in comboBoxTipeKamar2.Items)
                {
                    if (item.ToString().StartsWith(kamarToEdit.IDTipeKamar.ToString()))
                    {
                        comboBoxTipeKamar2.SelectedItem = item;
                        break;
                    }
                }
            }
        }

        private void btnBatal2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSimpan_Click(object sender, EventArgs e)
        {
            string nomor_kamar = textNomorKamar2.Text;
            string selected = comboBoxTipeKamar2.SelectedItem.ToString();
            int id_tipe_kamar = int.Parse(selected.Split('-')[0].Trim());
            bool status_kamar = cbStatusKamar2.Checked;
            string deskripsi = textDeskripsi2.Text;

            Kamar kamar = new Kamar
            {
                IDKamar = kamarToEdit.IDKamar, // <== penting!
                NomorKamar = nomor_kamar,
                IDTipeKamar = id_tipe_kamar,
                StatusKamar = status_kamar,
                Deskripsi = deskripsi
            };

            KamarController.Update(kamar);
            MessageBox.Show("Kamar berhasil diperbarui!");
            this.Hide();

        }
    
    private void loadtipekamar()
        {
            comboBoxTipeKamar2.Items.Clear();
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
                            comboBoxTipeKamar2.Items.Add(reader.GetInt32(0) + " - " + reader.GetString(1));

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal load layanan:\n" + ex.Message);
            }

        }
    } 
}
