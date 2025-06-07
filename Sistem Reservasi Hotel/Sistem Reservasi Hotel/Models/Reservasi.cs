using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistem_Reservasi_Hotel.Models
{
    public class Reservasi
    {
        public int IDReservasi { get; set; }
        public int IDKamar { get; set; }
        // Hapus baris ini: public int IDFasilitas { get; set; }
        public string NamaTamu { get; set; }
        public string NomorIdentitasTamu { get; set; }
        public string NomorKontakTamu { get; set; }
        public DateTime TanggalCheckIn { get; set; }
        public DateTime TanggalCheckOut { get; set; }
        public bool StatusReservasi { get; set; }

        // Tambahkan properti untuk menampung daftar fasilitas yang dipilih
        public List<Fasilitas> FasilitasTambahan { get; set; } = new List<Fasilitas>();
    }
}
