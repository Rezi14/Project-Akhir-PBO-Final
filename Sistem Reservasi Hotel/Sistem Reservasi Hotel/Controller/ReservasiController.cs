// Sistem_Reservasi_Hotel/Controller/ReservasiController.cs
using Npgsql;
using Sistem_Reservasi_Hotel.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq; // Tambahkan ini
using System.Windows.Forms; // Pastikan ini ada untuk MessageBox

namespace Sistem_Reservasi_Hotel.Controller
{
    public class ReservasiController
    {
        public static DataTable GetReservasi()
        {
            DataTable dt = new DataTable();
            using (var conn = DbContext.GetConnection())
            {
                conn.Open();
                string query = @"
                SELECT DISTINCT
                    r.id_reservasi,
                    k.nomor_kamar,
                    string_agg(f.nama_fasilitas, ', ') AS nama_fasilitas_list, -- Menggabungkan nama fasilitas
                    r.nama_tamu,
                    r.nomor_identitas_tamu,
                    r.nomor_kontak_tamu,
                    r.tanggal_checkin,
                    r.tanggal_checkout,
                    r.status_reservasi
                FROM
                    reservasi r
                JOIN kamar k ON r.id_kamar = k.id_kamar
                LEFT JOIN reservasi_fasilitas rf ON r.id_reservasi = rf.id_reservasi -- Join ke tabel penghubung
                LEFT JOIN fasilitas f ON rf.id_fasilitas = f.id_fasilitas -- Join ke tabel fasilitas
                WHERE r.status_reservasi = true
                GROUP BY r.id_reservasi, k.nomor_kamar, r.nama_tamu, r.nomor_identitas_tamu, r.nomor_kontak_tamu, r.tanggal_checkin, r.tanggal_checkout, r.status_reservasi
                ORDER BY id_reservasi ASC";
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        dt.Load(reader);
                    }
                }
                conn.Close();
            }
            return dt;
        }

        public static void Insertreservasi(Reservasi reservasi)
        {
            using (var conn = DbContext.GetConnection())
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // 1. Insert ke tabel reservasi
                        string insertReservasiQuery = $"INSERT INTO reservasi (id_kamar, nama_tamu, nomor_identitas_tamu, nomor_kontak_tamu, tanggal_checkin, tanggal_checkout, status_reservasi) " +
                                                       $"VALUES(@id_kamar, @nama_tamu, @nomor_identitas_tamu, @nomor_kontak_tamu, @tanggal_checkin, @tanggal_checkout, @status_reservasi) RETURNING id_reservasi";

                        int newReservasiId;
                        using (var cmd = new NpgsqlCommand(insertReservasiQuery, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@id_kamar", reservasi.IDKamar);
                            cmd.Parameters.AddWithValue("@nama_tamu", reservasi.NamaTamu);
                            cmd.Parameters.AddWithValue("@nomor_identitas_tamu", reservasi.NomorIdentitasTamu);
                            cmd.Parameters.AddWithValue("@nomor_kontak_tamu", reservasi.NomorKontakTamu);
                            cmd.Parameters.AddWithValue("@tanggal_checkin", reservasi.TanggalCheckIn);
                            cmd.Parameters.AddWithValue("@tanggal_checkout", reservasi.TanggalCheckOut);
                            cmd.Parameters.AddWithValue("@status_reservasi", reservasi.StatusReservasi);
                            newReservasiId = (int)cmd.ExecuteScalar();
                        }

                        // 2. Insert fasilitas yang dipilih ke tabel reservasi_fasilitas
                        string insertReservasiFasilitasQuery = "INSERT INTO reservasi_fasilitas (id_reservasi, id_fasilitas) VALUES (@id_reservasi, @id_fasilitas)";
                        foreach (var fasilitas in reservasi.FasilitasTambahan)
                        {
                            using (var cmdFasilitas = new NpgsqlCommand(insertReservasiFasilitasQuery, conn, transaction))
                            {
                                cmdFasilitas.Parameters.AddWithValue("@id_reservasi", newReservasiId);
                                cmdFasilitas.Parameters.AddWithValue("@id_fasilitas", fasilitas.IDFasilitas);
                                cmdFasilitas.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                    }
                    catch (NpgsqlException ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show("Error database saat reservasi: " + ex.Message);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show("Error saat reservasi: " + ex.Message);
                    }
                }
                conn.Close();
            }
        }


        public static decimal Checkout(int idReservasi, string metodePembayaran)
        {
            decimal totalBiaya = 0;

            using (var conn = DbContext.GetConnection())
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        string query = @"
                        SELECT r.id_reservasi, r.nama_tamu, r.nomor_identitas_tamu, r.tanggal_checkin, r.tanggal_checkout,
                               k.nomor_kamar, t.harga_per_malam
                        FROM reservasi r
                        JOIN kamar k ON r.id_kamar = k.id_kamar
                        JOIN tipe_kamar t ON k.id_tipe_kamar = t.id_tipe_kamar
                        WHERE r.id_reservasi = @id";

                        DateTime checkin, checkout;
                        decimal hargaPerMalam;
                        string namaTamu, nomorIdentitas, nomorKamar;

                        using (var cmd = new NpgsqlCommand(query, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@id", idReservasi);
                            using (var reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    checkin = (DateTime)reader["tanggal_checkin"];
                                    checkout = (DateTime)reader["tanggal_checkout"];
                                    hargaPerMalam = (decimal)reader["harga_per_malam"];
                                    namaTamu = reader["nama_tamu"].ToString();
                                    nomorIdentitas = reader["nomor_identitas_tamu"].ToString();
                                    nomorKamar = reader["nomor_kamar"].ToString();
                                }
                                else
                                {
                                    transaction.Rollback();
                                    return 0; // Reservasi tidak ditemukan
                                }
                            }
                        }

                        int jumlahMalam = (checkout - checkin).Days;
                        if (jumlahMalam <= 0) jumlahMalam = 1;

                        decimal biayaKamar = hargaPerMalam * jumlahMalam;
                        totalBiaya += biayaKamar;

                        // Ambil biaya fasilitas tambahan
                        string fasilitasQuery = @"
                        SELECT f.nama_fasilitas, f.biaya_tambahan
                        FROM reservasi_fasilitas rf
                        JOIN fasilitas f ON rf.id_fasilitas = f.id_fasilitas
                        WHERE rf.id_reservasi = @id_reservasi";

                        List<string> namaFasilitasList = new List<string>();
                        using (var fasilitasCmd = new NpgsqlCommand(fasilitasQuery, conn, transaction))
                        {
                            fasilitasCmd.Parameters.AddWithValue("@id_reservasi", idReservasi);
                            using (var fasilitasReader = fasilitasCmd.ExecuteReader())
                            {
                                while (fasilitasReader.Read())
                                {
                                    totalBiaya += (decimal)fasilitasReader["biaya_tambahan"];
                                    namaFasilitasList.Add(fasilitasReader["nama_fasilitas"].ToString());
                                }
                            }
                        }
                        string namaFasilitasGabungan = string.Join(", ", namaFasilitasList);

                        // Simpan ke riwayat_transaksi
                        string insertRiwayat = @"
                        INSERT INTO riwayat_transaksi
                        (id_reservasi, nama_tamu, nomor_identitas_tamu, nomor_kamar, nama_fasilitas,
                         tanggal_checkin, tanggal_checkout, total_biaya, metode_pembayaran, tanggal_transaksi)
                        VALUES
                        (@id_reservasi, @nama_tamu, @nomor_identitas_tamu, @nomor_kamar, @nama_fasilitas,
                         @checkin, @checkout, @total_biaya, @metode_pembayaran, @tanggal_transaksi)";

                        using (var insertCmd = new NpgsqlCommand(insertRiwayat, conn, transaction))
                        {
                            insertCmd.Parameters.AddWithValue("@id_reservasi", idReservasi);
                            insertCmd.Parameters.AddWithValue("@nama_tamu", namaTamu);
                            insertCmd.Parameters.AddWithValue("@nomor_identitas_tamu", nomorIdentitas);
                            insertCmd.Parameters.AddWithValue("@nomor_kamar", nomorKamar);
                            insertCmd.Parameters.AddWithValue("@nama_fasilitas", namaFasilitasGabungan);
                            insertCmd.Parameters.AddWithValue("@checkin", checkin);
                            insertCmd.Parameters.AddWithValue("@checkout", checkout);
                            insertCmd.Parameters.AddWithValue("@total_biaya", totalBiaya);
                            insertCmd.Parameters.AddWithValue("@metode_pembayaran", metodePembayaran);
                            insertCmd.Parameters.AddWithValue("@tanggal_transaksi", DateTime.Now);
                            insertCmd.ExecuteNonQuery();
                        }

                        // Update status reservasi
                        string updateReservasi = "UPDATE reservasi SET status_reservasi = false WHERE id_reservasi = @id";
                        using (var updateCmd = new NpgsqlCommand(updateReservasi, conn, transaction))
                        {
                            updateCmd.Parameters.AddWithValue("@id", idReservasi);
                            updateCmd.ExecuteNonQuery();
                        }

                        // Update status kamar
                        string updateKamar = @"
                        UPDATE kamar
                        SET status_kamar = true
                        WHERE id_kamar = (SELECT id_kamar FROM reservasi WHERE id_reservasi = @id)";
                        using (var updateKamarCmd = new NpgsqlCommand(updateKamar, conn, transaction))
                        {
                            updateKamarCmd.Parameters.AddWithValue("@id", idReservasi);
                            updateKamarCmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                    catch (NpgsqlException ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show("Error database saat checkout: " + ex.Message);
                        return 0;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show("Error saat checkout: " + ex.Message);
                        return 0;
                    }
                }
            }
            return totalBiaya;
        }
    }
}