using Npgsql;
using System;

namespace Sistem_Reservasi_Hotel.Models
{
    public class Akun
    {
        public int IDAkun { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public Akun() { }

    }
}