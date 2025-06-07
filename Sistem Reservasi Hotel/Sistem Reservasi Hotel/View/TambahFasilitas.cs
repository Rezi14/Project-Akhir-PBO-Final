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
    public partial class TambahFasilitas : Form
    {
        public TambahFasilitas()
        {
            InitializeComponent();
        }


        private void btnTambahFasilitas2_Click(object sender, EventArgs e)
        {
            string nama_fasilitas = textNamaFasilitas.Text;
            string biayaTambahanStr = textBiayaTambahan.Text;
            string deskripsi = textDeskripsi.Text;

            // Validasi sederhana
            if (string.IsNullOrWhiteSpace(nama_fasilitas) || string.IsNullOrWhiteSpace(biayaTambahanStr))
            {
                MessageBox.Show("Nama Fasilitas dan Biaya Tambahan wajib diisi.");
                return;
            }

            // Pastikan nilai biaya tambahan bisa di-convert
            if (!decimal.TryParse(biayaTambahanStr, out decimal biaya_tambahan))
            {
                MessageBox.Show("Biaya Tambahan harus berupa angka.");
                return;
            }
            Fasilitas fasilitas = new Fasilitas
            {
                NamaFasilitas = nama_fasilitas,
                BiayaTambahan = biaya_tambahan,
                Deskripsi = deskripsi
            };

            // Masukkan ke database
            FasilitasController.InsertFasilitas(fasilitas);

            // Refresh tampilan atau kosongkan form
            MessageBox.Show("Fasilitas berhasil ditambahkan!");
        }

        private void btnBatal_Click(object sender, EventArgs e)
        {
            KelolaFasilitas kelolaFasilitas = new KelolaFasilitas();
            kelolaFasilitas.Show();
            this.Hide();
        }
    }
}


