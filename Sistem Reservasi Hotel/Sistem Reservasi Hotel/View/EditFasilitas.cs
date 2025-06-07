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
    public partial class EditFasilitas : Form
    {
        private Fasilitas FasilitasToEdit;
        public EditFasilitas(Fasilitas fasilitas)
        {
            InitializeComponent();
            FasilitasToEdit = fasilitas;
        }

        private void EditFasilitas_Load(object sender, EventArgs e)
        {
            if (FasilitasToEdit != null)
            {
                textNamaFasilitas2.Text = FasilitasToEdit.NamaFasilitas;
                textBiayaTambahan2.Text = FasilitasToEdit.BiayaTambahan.ToString();
                textDeskripsi2.Text = FasilitasToEdit.Deskripsi;
            }
        }

        private void btnSimpan_Click(object sender, EventArgs e)
        {
            string nama_fasilitas = textNamaFasilitas2.Text;
            decimal biaya_tambahan = decimal.Parse(textBiayaTambahan2.Text);
            string deskripsi = textDeskripsi2.Text;

            Fasilitas fasilitas = new Fasilitas
            {
                IDFasilitas = FasilitasToEdit.IDFasilitas, 
                NamaFasilitas = nama_fasilitas,
                BiayaTambahan = biaya_tambahan,
                Deskripsi = deskripsi
            };

            FasilitasController.Update(fasilitas);
            MessageBox.Show("Fasilitas berhasil diperbarui!");
            this.Hide();

        }

        private void btnBatal_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
