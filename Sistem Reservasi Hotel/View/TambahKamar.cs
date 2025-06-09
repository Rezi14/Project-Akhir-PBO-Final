﻿using Npgsql;
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
            var selected_tipe_kamar = comboBoxTipeKamar.SelectedItem;

            // 2. Panggil Controller untuk validasi
            List<string> validationErrors = KamarController.ValidateKamar(nomor_kamar, selected_tipe_kamar);

            // 3. Periksa hasil validasi
            if (validationErrors.Any())
            {
                string allErrors = string.Join("\n", validationErrors);
                MessageBox.Show(allErrors, "Input Tidak Valid", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Hentikan proses jika ada error
            }

            // --- Jika validasi lolos, lanjutkan proses ---
            try
            {
                string selected = selected_tipe_kamar.ToString();
                int id_tipe_kamar = int.Parse(selected.Split('-')[0].Trim());
                bool status_kamar = cbStatusKamar.Checked;
                string deskripsi = textDeskripsi1.Text;

                Kamar kamar = new Kamar
                {
                    NomorKamar = nomor_kamar.Trim(),
                    IDTipeKamar = id_tipe_kamar,
                    StatusKamar = status_kamar,
                    Deskripsi = deskripsi
                };

                KamarController.InsertKamar(kamar);

                MessageBox.Show("Kamar berhasil ditambahkan!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Beri tahu form pemanggil bahwa proses SUKSES
                this.DialogResult = DialogResult.OK;
                this.Close(); // Tutup form
            }
            catch (Exception ex)
            {
                MessageBox.Show("Terjadi kesalahan saat menyimpan data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
            this.DialogResult = DialogResult.Cancel;
            this.Close(); // Cukup tutup form ini
        }

        private void textDeskripsi1_TextChanged(object sender, EventArgs e)
        {

        }

        private void TambahKamar_Load(object sender, EventArgs e)
        {

        }
    }
}
