﻿using Npgsql;
using System;
using System.Collections.Generic;

namespace Sistem_Reservasi_Hotel.Models
{
    public class RiwayatTransaksi
    {
        public int IDRiwayatTransaksi { get; set; }
        public int IDReservasi { get; set; }
        public int IDAkun { get; set; }
        public string NamaTamu { get; set; }
        public string NomorIdentitasTamu { get; set; }
        public string NomorKamar { get; set; }
        public string NamaFasilitas { get; set; }
        public DateTime TanggalCheckIn { get; set; }
        public DateTime TanggalCheckOut { get; set; }
        public decimal TotalBiaya { get; set; }
        public string MetodePembayaran { get; set; }
        public DateTime TanggalTransaksi { get; set; }

        public static List<RiwayatTransaksi> GetAllRiwayat()
        {
            List<RiwayatTransaksi> listRiwayat = new List<RiwayatTransaksi>();
            using (var conn = DbContext.GetConnection())
            {
                conn.Open();
                string query = "SELECT * FROM riwayat_transaksi ORDER BY id_riwayat_transaksi";
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            RiwayatTransaksi riwayat = new RiwayatTransaksi
                            {
                                IDRiwayatTransaksi = (int)reader["id_riwayat_transaksi"],
                                IDReservasi = (int)reader["id_reservasi"],
                                IDAkun = (int)reader["id_akun"],
                                NamaTamu = reader["nama_tamu"].ToString(),
                                NomorIdentitasTamu = reader["nomor_identitas_tamu"].ToString(),
                                NomorKamar = reader["nomor_kamar"].ToString(),
                                NamaFasilitas = reader["nama_fasilitas"].ToString(),
                                TanggalCheckIn = (DateTime)reader["tanggal_checkin"],
                                TanggalCheckOut = (DateTime)reader["tanggal_checkout"],
                                TotalBiaya = (decimal)reader["total_biaya"],
                                MetodePembayaran = reader["metode_pembayaran"].ToString(),
                                TanggalTransaksi = (DateTime)reader["tanggal_transaksi"]
                            };
                            listRiwayat.Add(riwayat);
                        }
                    }
                }
            }
            return listRiwayat;
        }

        public static List<RiwayatTransaksi> GetRiwayatTransaksiByBulan(int bulan, int tahun)
        {
            var list = new List<RiwayatTransaksi>();
            using (var conn = DbContext.GetConnection())
            {
                conn.Open();
                string query = @"
                    SELECT * FROM riwayat_transaksi
                    WHERE EXTRACT(MONTH FROM tanggal_transaksi) = @bulan AND EXTRACT(YEAR FROM tanggal_transaksi) = @tahun
                    ORDER BY id_riwayat_transaksi";
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@bulan", bulan);
                    cmd.Parameters.AddWithValue("@tahun", tahun);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new RiwayatTransaksi
                            {
                                IDRiwayatTransaksi = reader.GetInt32(0),
                                IDReservasi = reader.GetInt32(1),
                                IDAkun = reader.GetInt32(2),
                                NamaTamu = reader.GetString(3),
                                NomorIdentitasTamu = reader.GetString(4),
                                NomorKamar = reader.GetString(5),
                                NamaFasilitas = reader.GetString(6),
                                TanggalCheckIn = reader.GetDateTime(7),
                                TanggalCheckOut = reader.GetDateTime(8),
                                TotalBiaya = reader.GetDecimal(9),
                                MetodePembayaran = reader.GetString(10),
                                TanggalTransaksi = reader.GetDateTime(11)
                            });
                        }
                    }
                }
            }
            return list;
        }

        public static void Insert(RiwayatTransaksi riwayat, NpgsqlConnection conn, NpgsqlTransaction transaction)
        {
            string insertRiwayat = @"
                INSERT INTO riwayat_transaksi
                (id_reservasi, id_akun, nama_tamu, nomor_identitas_tamu, nomor_kamar, nama_fasilitas,
                tanggal_checkin, tanggal_checkout, total_biaya, metode_pembayaran, tanggal_transaksi)
                VALUES
                (@id_reservasi, @id_akun, @nama_tamu, @nomor_identitas_tamu, @nomor_kamar, @nama_fasilitas,
                @checkin, @checkout, @total_biaya, @metode_pembayaran, @tanggal_transaksi)";

            using (var insertCmd = new NpgsqlCommand(insertRiwayat, conn, transaction))
            {
                insertCmd.Parameters.AddWithValue("@id_reservasi", riwayat.IDReservasi);
                insertCmd.Parameters.AddWithValue("@id_akun", riwayat.IDAkun);
                insertCmd.Parameters.AddWithValue("@nama_tamu", riwayat.NamaTamu);
                insertCmd.Parameters.AddWithValue("@nomor_identitas_tamu", riwayat.NomorIdentitasTamu);
                insertCmd.Parameters.AddWithValue("@nomor_kamar", riwayat.NomorKamar);
                insertCmd.Parameters.AddWithValue("@nama_fasilitas", riwayat.NamaFasilitas);
                insertCmd.Parameters.AddWithValue("@checkin", riwayat.TanggalCheckIn);
                insertCmd.Parameters.AddWithValue("@checkout", riwayat.TanggalCheckOut);
                insertCmd.Parameters.AddWithValue("@total_biaya", riwayat.TotalBiaya);
                insertCmd.Parameters.AddWithValue("@metode_pembayaran", riwayat.MetodePembayaran);
                insertCmd.Parameters.AddWithValue("@tanggal_transaksi", riwayat.TanggalTransaksi);
                insertCmd.ExecuteNonQuery();
            }
        }
    }
}