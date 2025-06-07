using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistem_Reservasi_Hotel.Models
{
    public class Kamar
    {
        public int IDKamar { get; set; }
        public string NomorKamar { get; set; }
        public int IDTipeKamar { get; set; }
        public bool StatusKamar { get; set; }
        public string Deskripsi {  get; set; }
    }
}
