using Npgsql;
using Sistem_Reservasi_Hotel.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistem_Reservasi_Hotel.Controller
{
    public class RiwayatTransaksiController
    {
        public static List<RiwayatTransaksi> GetAllRiwayat()
        {
            List<RiwayatTransaksi> listRiwayat = new List<RiwayatTransaksi>();

            using (var conn = DbContext.GetConnection())
            {
                conn.Open();
                string query = @"
            SELECT id_riwayat_transaksi, id_reservasi, nama_tamu, nomor_identitas_tamu, 
                   nomor_kamar, nama_fasilitas, tanggal_checkin, tanggal_checkout, 
                   total_biaya, metode_pembayaran, tanggal_transaksi
            FROM riwayat_transaksi
            ORDER BY id_riwayat_transaksi";

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
            SELECT id_riwayat_transaksi, id_reservasi, nama_tamu, nomor_identitas_tamu,
                   nomor_kamar, nama_fasilitas, tanggal_checkin, tanggal_checkout,
                   total_biaya, metode_pembayaran, tanggal_transaksi
            FROM riwayat_transaksi
            WHERE EXTRACT(MONTH FROM tanggal_transaksi) = @bulan
              AND EXTRACT(YEAR FROM tanggal_transaksi) = @tahun
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
                                NamaTamu = reader.GetString(2),
                                NomorIdentitasTamu = reader.GetString(3),
                                NomorKamar = reader.GetString(4),
                                NamaFasilitas = reader.GetString(5),
                                TanggalCheckIn = reader.GetDateTime(6),
                                TanggalCheckOut = reader.GetDateTime(7),
                                TotalBiaya = reader.GetDecimal(8),
                                MetodePembayaran = reader.GetString(9),
                                TanggalTransaksi = reader.GetDateTime(10)
                            });
                        }
                    }
                }
            }

            return list;
        }
    }
}
