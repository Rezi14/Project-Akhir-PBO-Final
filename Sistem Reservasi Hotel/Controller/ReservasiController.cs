﻿using Sistem_Reservasi_Hotel.Models;
using System.Data;

namespace Sistem_Reservasi_Hotel.Controller
{
    public class ReservasiController
    {
        public static DataTable GetReservasi()
        {
            return Reservasi.GetReservasi();
        }

        public static void Insertreservasi(Reservasi reservasi)
        {
            Reservasi.Insertreservasi(reservasi);
        }

        public static decimal Checkout(int idReservasi, string metodePembayaran)
        {
            return Reservasi.Checkout(idReservasi, metodePembayaran);
        }

        public static List<string> ValidateReservasi(
        object selectedKamarItem,
        string namaTamu,
        string nomorIdentitas,
        string nomorKontak,
        DateTime tanggalCheckIn,
        DateTime tanggalCheckOut)
        {
            var errors = new List<string>();

            // 1. Validasi kamar harus dipilih
            if (selectedKamarItem == null)
            {
                errors.Add("Nomor kamar wajib dipilih.");
            }

            // 2. Validasi input teks tidak boleh kosong atau hanya spasi
            if (string.IsNullOrWhiteSpace(namaTamu))
            {
                errors.Add("Nama tamu wajib diisi.");
            }
            if (string.IsNullOrWhiteSpace(nomorIdentitas))
            {
                errors.Add("Nomor identitas wajib diisi.");
            }
            if (string.IsNullOrWhiteSpace(nomorKontak))
            {
                errors.Add("Nomor kontak wajib diisi.");
            }
            else if (!nomorKontak.All(char.IsDigit)) // 3. Validasi nomor kontak harus numerik
            {
                errors.Add("Nomor kontak hanya boleh berisi angka.");
            }

            // 4. Validasi logika tanggal
            if (tanggalCheckOut.Date <= tanggalCheckIn.Date)
            {
                errors.Add("Tanggal Check-Out harus setelah Tanggal Check-In.");
            }

            return errors;
        }
    }
}