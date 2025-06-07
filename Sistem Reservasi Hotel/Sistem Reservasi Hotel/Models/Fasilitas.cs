using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistem_Reservasi_Hotel.Models
{
    public class Fasilitas
    {
        public int IDFasilitas { get; set; }
        public string NamaFasilitas {  get; set; }
        public decimal BiayaTambahan { get; set; }
        public string Deskripsi { get; set; }

    }
}
