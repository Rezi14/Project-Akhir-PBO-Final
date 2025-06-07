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
    public partial class FormKelolaKamar : Form
    {
        public FormKelolaKamar()
        {
            InitializeComponent();
        }

        private void FormKelolaKamar_Load(object sender, EventArgs e)
        {
            tampilkankamar();

        }
        private void tampilkankamar()
        {
            List<Kamar> listKamar = KamarController.GetAllKamar();
            dataGridViewKelolaKamar.DataSource = null;
            dataGridViewKelolaKamar.DataSource = listKamar;

            // Tambahkan kolom Edit jika belum ada
            if (!dataGridViewKelolaKamar.Columns.Contains("Edit"))
            {
                DataGridViewButtonColumn btnedit = new DataGridViewButtonColumn();
                btnedit.Text = "Edit";
                btnedit.Name = "Edit";
                btnedit.HeaderText = "Edit";
                btnedit.UseColumnTextForButtonValue = true;
                dataGridViewKelolaKamar.Columns.Add(btnedit);
            }

            // Tambahkan kolom Hapus jika belum ada
            if (!dataGridViewKelolaKamar.Columns.Contains("Hapus"))
            {
                DataGridViewButtonColumn btnhapus = new DataGridViewButtonColumn();
                btnhapus.Text = "Hapus";
                btnhapus.Name = "Hapus";
                btnhapus.HeaderText = "Hapus";
                btnhapus.UseColumnTextForButtonValue = true;
                dataGridViewKelolaKamar.Columns.Add(btnhapus);
            }

            if (dataGridViewKelolaKamar.Columns.Contains("IDKamar"))
                dataGridViewKelolaKamar.Columns["IDKamar"].HeaderText = "ID Kamar";

            if (dataGridViewKelolaKamar.Columns.Contains("NomorKamar"))
                dataGridViewKelolaKamar.Columns["NomorKamar"].HeaderText = "Nomor Kamar";

            if (dataGridViewKelolaKamar.Columns.Contains("IDTipeKamar"))
                dataGridViewKelolaKamar.Columns["IDTipeKamar"].HeaderText = "ID Tipe Kamar";

            if (dataGridViewKelolaKamar.Columns.Contains("StatusKamar"))
                dataGridViewKelolaKamar.Columns["StatusKamar"].HeaderText = "Status Kamar";

            if (dataGridViewKelolaKamar.Columns.Contains("Deskripsi"))
                dataGridViewKelolaKamar.Columns["Deskripsi"].HeaderText = "Deskripsi Kamar";


            // Reset urutan kolom
            ResetKolomUrutan();
        }

        private void dataGridViewKelolaKamar_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                string columnName = dataGridViewKelolaKamar.Columns[e.ColumnIndex].Name;

                if (columnName == "Edit")
                {
                    // Ambil data dari baris yang dipilih
                    Kamar selectedKamar = new Kamar
                    {
                        IDKamar = Convert.ToInt32(dataGridViewKelolaKamar.Rows[e.RowIndex].Cells["IDKamar"].Value),
                        NomorKamar = dataGridViewKelolaKamar.Rows[e.RowIndex].Cells["NomorKamar"].Value.ToString(),
                        IDTipeKamar = Convert.ToInt32(dataGridViewKelolaKamar.Rows[e.RowIndex].Cells["IDTipeKamar"].Value),
                        StatusKamar = Convert.ToBoolean(dataGridViewKelolaKamar.Rows[e.RowIndex].Cells["StatusKamar"].Value),
                        Deskripsi = dataGridViewKelolaKamar.Rows[e.RowIndex].Cells["Deskripsi"].Value.ToString()
                    };

                    // Buka form edit dengan data kamar yang dipilih
                    EditKamar editForm = new EditKamar(selectedKamar);
                    editForm.ShowDialog();

                    // Refresh data setelah form edit ditutup
                    tampilkankamar();
                }
                else if (columnName == "Hapus")
                {
                    // Konfirmasi dari user
                    DialogResult result = MessageBox.Show("Apakah Anda yakin ingin menghapus kamar ini?", "Konfirmasi Hapus", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (result == DialogResult.Yes)
                    {
                        int idKamar = Convert.ToInt32(dataGridViewKelolaKamar.Rows[e.RowIndex].Cells["IDKamar"].Value);

                        try
                        {
                            // Panggil controller untuk hapus
                            KamarController.Delete(idKamar);
                            MessageBox.Show("Data kamar berhasil dihapus.");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Terjadi kesalahan saat menghapus data kamar: " + ex.Message);
                        }

                        // Refresh tampilan
                        tampilkankamar();
                    }
                }
            }
        }

        private void btnKembali_Click(object sender, EventArgs e)
        {
            Dasboard dasboard = new Dasboard();
            dasboard.Show();
            this.Hide();
        }
        private void ResetKolomUrutan()
        {
            if (dataGridViewKelolaKamar.Columns.Contains("Edit"))
                dataGridViewKelolaKamar.Columns["Edit"].DisplayIndex = dataGridViewKelolaKamar.Columns.Count - 1;

            if (dataGridViewKelolaKamar.Columns.Contains("Hapus"))
                dataGridViewKelolaKamar.Columns["Hapus"].DisplayIndex = dataGridViewKelolaKamar.Columns.Count - 1;
        }


        private void btnTambahKamar1_Click(object sender, EventArgs e)
        {
            TambahKamar tmbhkmr = new TambahKamar();
            tmbhkmr.Show();
            this.Hide();
        }

        
    }
}
