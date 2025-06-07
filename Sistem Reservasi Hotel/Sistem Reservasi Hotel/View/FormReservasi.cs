// Sistem_Reservasi_Hotel/View/FormReservasi.cs
using Npgsql;
using Sistem_Reservasi_Hotel.Controller;
using Sistem_Reservasi_Hotel.Models;
using System;
using System.Collections.Generic;
using System.Linq; // Pastikan ini ada
using System.Windows.Forms;

namespace Sistem_Reservasi_Hotel.View
{
    public partial class FormReservasi : Form
    {
        private List<Fasilitas> allFasilitas; // Simpan semua fasilitas yang tersedia

        public FormReservasi()
        {
            InitializeComponent();
            LoadNomorKamar();
            LoadFasilitas();
        }

        private void btnReservasi_Click(object sender, EventArgs e)
        {
            if (cbNomorKamar1.SelectedItem == null)
            {
                MessageBox.Show("Nomor kamar wajib diisi.");
                return;
            }

            string selectedKamar = cbNomorKamar1.SelectedItem.ToString();
            int id_kamar = int.Parse(selectedKamar.Split('-')[0].Trim());

            string nama_tamu = textNamaTamu1.Text;
            string nomor_identitas = textNomorIdentitas1.Text;
            string nomor_kontak = textNomorKontak1.Text;
            DateTime tanggal_check_in = dateTimeCheckIn1.Value;
            DateTime tanggal_check_out = dateTimeCheckOut1.Value;
            bool status_reservasi = checkStatusReservasi1.Checked;

            // Validasi sederhana lainnya
            if (string.IsNullOrEmpty(nama_tamu) || string.IsNullOrEmpty(nomor_identitas) || string.IsNullOrEmpty(nomor_kontak))
            {
                MessageBox.Show("Nama tamu, nomor identitas, dan nomor kontak wajib diisi.");
                return;
            }

            // Ambil fasilitas yang dipilih dari CheckedListBox
            List<Fasilitas> selectedFasilitas = new List<Fasilitas>();
            foreach (object itemChecked in checkedListBoxFasilitas.CheckedItems)
            {
                // Item dalam CheckedListBox disimpan sebagai string "ID - NamaFasilitas"
                string itemText = itemChecked.ToString();
                int idFasilitas = int.Parse(itemText.Split('-')[0].Trim());
                // Cari objek Fasilitas yang sesuai dari daftar allFasilitas
                Fasilitas f = allFasilitas.FirstOrDefault(fas => fas.IDFasilitas == idFasilitas);
                if (f != null)
                {
                    selectedFasilitas.Add(f);
                }
            }

            // Buat objek reservasi
            Reservasi reservasi = new Reservasi
            {
                IDKamar = id_kamar,
                NamaTamu = nama_tamu,
                NomorIdentitasTamu = nomor_identitas,
                NomorKontakTamu = nomor_kontak,
                TanggalCheckIn = tanggal_check_in,
                TanggalCheckOut = tanggal_check_out,
                StatusReservasi = status_reservasi,
                FasilitasTambahan = selectedFasilitas // Tetapkan daftar fasilitas yang dipilih
            };

            // Masukkan ke database
            ReservasiController.Insertreservasi(reservasi);
            KamarController.UpdateStatusKamar(id_kamar, false); // Asumsi kamar menjadi tidak tersedia setelah reservasi

            // Refresh tampilan atau kosongkan form
            MessageBox.Show("Reservasi berhasil ditambahkan!");
            this.Close(); // Tutup form setelah reservasi berhasil
            new Dasboard().Show(); // Kembali ke Dasboard
        }

        private void LoadNomorKamar()
        {
            cbNomorKamar1.Items.Clear();
            try
            {
                using (var conn = DbContext.GetConnection())
                {
                    conn.Open();
                    string query = @"
                    SELECT id_kamar, nomor_kamar
                    from kamar
                    where status_kamar = true
                    ORDER BY id_kamar";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cbNomorKamar1.Items.Add(reader.GetInt32(0) + " - " + reader.GetString(1));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal load nomor kamar:\n" + ex.Message);
            }
        }

        private void LoadFasilitas()
        {
            checkedListBoxFasilitas.Items.Clear();
            allFasilitas = FasilitasController.GetAllFasilitas(); // Dapatkan semua fasilitas
            foreach (var fasilitas in allFasilitas)
            {
                checkedListBoxFasilitas.Items.Add(fasilitas.IDFasilitas + " - " + fasilitas.NamaFasilitas);
            }
        }

        private void FormReservasi_Load(object sender, EventArgs e)
        {
            LoadNomorKamar();
            LoadFasilitas();
        }

        private void btnBatal_Click(object sender, EventArgs e)
        {
            Dasboard dasboard = new Dasboard();
            dasboard.Show();
            this.Hide();
        }

        // Anda mungkin tidak memerlukan event handler ini lagi jika menggunakan CheckedListBox
        private void cbNomorKamar1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Logika jika ada yang perlu dilakukan saat nomor kamar berubah
        }
    }
}