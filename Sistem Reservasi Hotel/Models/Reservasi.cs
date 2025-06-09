﻿using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using static Sistem_Reservasi_Hotel.Models.Akun;

namespace Sistem_Reservasi_Hotel.Models
{
    public class Reservasi
    {
        public int IDReservasi { get; set; }
        public int IDKamar { get; set; }
        public int IDAkun { get; set; }
        public string NamaTamu { get; set; }
        public string NomorIdentitasTamu { get; set; }
        public string NomorKontakTamu { get; set; }
        public DateTime TanggalCheckIn { get; set; }
        public DateTime TanggalCheckOut { get; set; }
        public bool StatusReservasi { get; set; }
        public List<Fasilitas> FasilitasTambahan { get; set; } = new List<Fasilitas>();

        public static DataTable GetReservasi()
        {
            DataTable dt = new DataTable();
            using (var conn = DbContext.GetConnection())
            {
                conn.Open();
                string query = @"
                    SELECT DISTINCT
                        r.id_reservasi,
                        a.id_akun,
                        k.nomor_kamar,
                        string_agg(f.nama_fasilitas, ', ') AS nama_fasilitas_list,
                        r.nama_tamu,
                        r.nomor_identitas_tamu,
                        r.nomor_kontak_tamu,
                        r.tanggal_checkin,
                        r.tanggal_checkout,
                        r.status_reservasi
                    FROM
                        reservasi r
                    JOIN kamar k ON r.id_kamar = k.id_kamar
                    JOIN akun a on a.id_akun = r.id_akun
                    LEFT JOIN reservasi_fasilitas rf ON r.id_reservasi = rf.id_reservasi
                    LEFT JOIN fasilitas f ON rf.id_fasilitas = f.id_fasilitas
                    WHERE r.status_reservasi = true
                    GROUP BY r.id_reservasi, k.nomor_kamar, a.id_akun
                    ORDER BY r.id_reservasi ASC";
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        dt.Load(reader);
                    }
                }
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
                        string insertReservasiQuery = $"INSERT INTO reservasi (id_kamar, id_akun,nama_tamu, nomor_identitas_tamu, nomor_kontak_tamu, tanggal_checkin, tanggal_checkout, status_reservasi) VALUES(@id_kamar, @id_akun,@nama_tamu, @nomor_identitas_tamu, @nomor_kontak_tamu, @tanggal_checkin, @tanggal_checkout, @status_reservasi) RETURNING id_reservasi";
                        int newReservasiId;
                        using (var cmd = new NpgsqlCommand(insertReservasiQuery, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@id_kamar", reservasi.IDKamar);
                            cmd.Parameters.AddWithValue("@id_akun", Session.CurrentUserId);
                            cmd.Parameters.AddWithValue("@nama_tamu", reservasi.NamaTamu);
                            cmd.Parameters.AddWithValue("@nomor_identitas_tamu", reservasi.NomorIdentitasTamu);
                            cmd.Parameters.AddWithValue("@nomor_kontak_tamu", reservasi.NomorKontakTamu);
                            cmd.Parameters.AddWithValue("@tanggal_checkin", reservasi.TanggalCheckIn);
                            cmd.Parameters.AddWithValue("@tanggal_checkout", reservasi.TanggalCheckOut);
                            cmd.Parameters.AddWithValue("@status_reservasi", reservasi.StatusReservasi);
                            newReservasiId = (int)cmd.ExecuteScalar();
                        }

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
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show("Error saat reservasi: " + ex.Message);
                    }
                }
            }
        }

        public static decimal Checkout(int idReservasi, string metodePembayaran)
        {
            decimal totalBiaya = 0;
            int idAkun;
            using (var conn = DbContext.GetConnection())
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        string query = @"
                            SELECT r.id_reservasi, r.id_akun, r.nama_tamu, r.nomor_identitas_tamu, r.tanggal_checkin, r.tanggal_checkout,
                            k.id_kamar, k.nomor_kamar, t.harga_per_malam
                            FROM reservasi r
                            JOIN kamar k ON r.id_kamar = k.id_kamar
                            JOIN tipe_kamar t ON k.id_tipe_kamar = t.id_tipe_kamar
                            WHERE r.id_reservasi = @id";

                        DateTime checkin, checkout;
                        decimal hargaPerMalam;
                        string namaTamu, nomorIdentitas, nomorKamar;
                        int idKamar;

                        using (var cmd = new NpgsqlCommand(query, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@id", idReservasi);
                            using (var reader = cmd.ExecuteReader())
                            {
                                if (!reader.Read())
                                {
                                    transaction.Rollback();
                                    return 0;
                                }
                                idAkun = (int)reader["id_akun"];
                                idKamar = (int)reader["id_kamar"];
                                checkin = (DateTime)reader["tanggal_checkin"];
                                checkout = (DateTime)reader["tanggal_checkout"];
                                hargaPerMalam = (decimal)reader["harga_per_malam"];
                                namaTamu = reader["nama_tamu"].ToString();
                                nomorIdentitas = reader["nomor_identitas_tamu"].ToString();
                                nomorKamar = reader["nomor_kamar"].ToString();
                            }
                        }

                        int jumlahMalam = (checkout - checkin).Days;
                        if (jumlahMalam <= 0) jumlahMalam = 1;

                        decimal biayaKamar = hargaPerMalam * jumlahMalam;
                        totalBiaya += biayaKamar;

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

                        RiwayatTransaksi.Insert(
                            new RiwayatTransaksi
                            {
                                IDReservasi = idReservasi,
                                IDAkun = idAkun,
                                NamaTamu = namaTamu,
                                NomorIdentitasTamu = nomorIdentitas,
                                NomorKamar = nomorKamar,
                                NamaFasilitas = namaFasilitasGabungan,
                                TanggalCheckIn = checkin,
                                TanggalCheckOut = checkout,
                                TotalBiaya = totalBiaya,
                                MetodePembayaran = metodePembayaran,
                                TanggalTransaksi = DateTime.Now
                            }, conn, transaction);

                        string updateReservasi = "UPDATE reservasi SET status_reservasi = false WHERE id_reservasi = @id";
                        using (var updateCmd = new NpgsqlCommand(updateReservasi, conn, transaction))
                        {
                            updateCmd.Parameters.AddWithValue("@id", idReservasi);
                            updateCmd.ExecuteNonQuery();
                        }

                        Kamar.UpdateStatusKamar(idKamar, true);
                        transaction.Commit();
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