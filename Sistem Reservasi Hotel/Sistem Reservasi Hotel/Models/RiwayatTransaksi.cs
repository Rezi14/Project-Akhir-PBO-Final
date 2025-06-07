using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistem_Reservasi_Hotel.Models
{
    public class RiwayatTransaksi
    {
        public int IDRiwayatTransaksi { get; set; } // Akan di-generate otomatis oleh database jika SERIAL
        public int IDReservasi { get; set; } // ID dari reservasi asli
        public string NamaTamu { get; set; }
        public string NomorIdentitasTamu { get; set; }
        public string NomorKamar { get; set; }
        public string NamaFasilitas { get; set; } // Bisa juga ID Fasilitas jika ingin lebih detail
        public DateTime TanggalCheckIn { get; set; }
        public DateTime TanggalCheckOut { get; set; }
        public decimal TotalBiaya { get; set; }
        public string MetodePembayaran { get; set; }
        public DateTime TanggalTransaksi { get; set; }
    }
}
