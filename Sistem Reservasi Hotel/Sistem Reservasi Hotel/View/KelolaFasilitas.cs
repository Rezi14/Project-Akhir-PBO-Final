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
    public partial class KelolaFasilitas : Form
    {
        public KelolaFasilitas()
        {
            InitializeComponent();
        }

        private void dataGridViewKelolaFasilitas_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                string columnName = dataGridViewKelolaFasilitas.Columns[e.ColumnIndex].Name;

                if (columnName == "Edit")
                {
                    // Ambil data dari baris yang dipilih
                    Fasilitas selectedFasilitas = new Fasilitas
                    {
                        IDFasilitas = Convert.ToInt32(dataGridViewKelolaFasilitas.Rows[e.RowIndex].Cells["IDFasilitas"].Value),
                        NamaFasilitas = dataGridViewKelolaFasilitas.Rows[e.RowIndex].Cells["NamaFasilitas"].Value.ToString(),
                        BiayaTambahan = Convert.ToDecimal(dataGridViewKelolaFasilitas.Rows[e.RowIndex].Cells["BiayaTambahan"].Value),
                        Deskripsi = dataGridViewKelolaFasilitas.Rows[e.RowIndex].Cells["Deskripsi"].Value.ToString()
                    };

                    // Buka form edit dengan data kamar yang dipilih
                    EditFasilitas editForm = new EditFasilitas(selectedFasilitas);
                    editForm.ShowDialog();

                    // Refresh data setelah form edit ditutup
                    tampilkanfasilitas();
                }
                else if (columnName == "Hapus")
                {
                    // Konfirmasi dari user
                    DialogResult result = MessageBox.Show("Apakah Anda yakin ingin menghapus Fasilitas ini?", "Konfirmasi Hapus", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (result == DialogResult.Yes)
                    {
                        int idFasilitas = Convert.ToInt32(dataGridViewKelolaFasilitas.Rows[e.RowIndex].Cells["IDFasilitas"].Value);

                        try
                        {
                            // Panggil controller untuk hapus
                            FasilitasController.Delete(idFasilitas);
                            MessageBox.Show("Data Fasilitas berhasil dihapus.");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Terjadi kesalahan saat menghapus data kamar: " + ex.Message);
                        }

                        // Refresh tampilan
                        tampilkanfasilitas();
                    }
                }
            }
        }
        private void tampilkanfasilitas()
        {
            // Ambil data
            List<Fasilitas> listfasilitas = FasilitasController.GetAllFasilitas();

            // Set ulang DataSource
            dataGridViewKelolaFasilitas.DataSource = null;
            dataGridViewKelolaFasilitas.DataSource = listfasilitas;

            // Cek dan tambahkan kolom Edit jika belum ada
            if (!dataGridViewKelolaFasilitas.Columns.Contains("Edit"))
            {
                DataGridViewButtonColumn btnedit = new DataGridViewButtonColumn();
                btnedit.Text = "Edit";
                btnedit.Name = "Edit";
                btnedit.HeaderText = "Edit";
                btnedit.UseColumnTextForButtonValue = true;
                dataGridViewKelolaFasilitas.Columns.Add(btnedit);
            }

            // Cek dan tambahkan kolom Hapus jika belum ada
            if (!dataGridViewKelolaFasilitas.Columns.Contains("Hapus"))
            {
                DataGridViewButtonColumn btnhapus = new DataGridViewButtonColumn();
                btnhapus.Text = "Hapus";
                btnhapus.Name = "Hapus";
                btnhapus.HeaderText = "Hapus";
                btnhapus.UseColumnTextForButtonValue = true;
                dataGridViewKelolaFasilitas.Columns.Add(btnhapus);
            }

            if (dataGridViewKelolaFasilitas.Columns.Contains("IDFasilitas"))
                dataGridViewKelolaFasilitas.Columns["IDFasilitas"].HeaderText = "ID Fasilitas";

            if (dataGridViewKelolaFasilitas.Columns.Contains("NamaFasilitas"))
                dataGridViewKelolaFasilitas.Columns["NamaFasilitas"].HeaderText = "Nama Fasilitas";

            if (dataGridViewKelolaFasilitas.Columns.Contains("BiayaTambahan"))
                dataGridViewKelolaFasilitas.Columns["BiayaTambahan"].HeaderText = "Biaya Tambahan";

            if (dataGridViewKelolaFasilitas.Columns.Contains("Deskripsi"))
                dataGridViewKelolaFasilitas.Columns["Deskripsi"].HeaderText = "Deskripsi";
            // Atur ulang urutan kolom agar Edit & Hapus di kanan
            ResetKolomUrutan();
        }


        private void KelolaFasilitas_Load(object sender, EventArgs e)
        {
            tampilkanfasilitas();
        }

        private void btnKembali_Click(object sender, EventArgs e)
        {
            Dasboard dasboard = new Dasboard();
            dasboard.Show();
            this.Hide();
        }

        private void btnTambahFasilitas1_Click(object sender, EventArgs e)
        {
            TambahFasilitas tambahFasilitas = new TambahFasilitas();
            tambahFasilitas.Show();
            this.Hide();
        }
        private void ResetKolomUrutan()
        {
            if (dataGridViewKelolaFasilitas.Columns.Contains("Edit"))
                dataGridViewKelolaFasilitas.Columns["Edit"].DisplayIndex = dataGridViewKelolaFasilitas.Columns.Count - 1;

            if (dataGridViewKelolaFasilitas.Columns.Contains("Hapus"))
                dataGridViewKelolaFasilitas.Columns["Hapus"].DisplayIndex = dataGridViewKelolaFasilitas.Columns.Count - 1;
        }

    }
}
