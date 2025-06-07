using Npgsql;
using Sistem_Reservasi_Hotel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistem_Reservasi_Hotel.Controller
{
    public class FasilitasController
    {
        public static List<Fasilitas> GetAllFasilitas()
        {
            List<Fasilitas> listfasilitas = new List<Fasilitas>();
            using (var conn = DbContext.GetConnection())
            {
                conn.Open();
                string query = "SELECT * FROM fasilitas ORDER BY id_fasilitas ASC";
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Fasilitas fasilitas = new Fasilitas();
                            fasilitas.IDFasilitas = (int)reader["id_fasilitas"];
                            fasilitas.NamaFasilitas = (string)reader["nama_fasilitas"];
                            fasilitas.BiayaTambahan = (decimal)reader["biaya_tambahan"];
                            fasilitas.Deskripsi = (string)reader["deskripsi"];
                            listfasilitas.Add(fasilitas);
                        }
                    }
                }
                //DbContext.CloseConnection();
            }
            return listfasilitas;
        }
        public static void InsertFasilitas(Fasilitas fasilitas)
        {
            string query = 
                $"INSERT INTO fasilitas (nama_fasilitas, biaya_tambahan, deskripsi) " +
                $"VALUES(@nama_fasilitas, @biaya_tambahan, @deskripsi)";
            try
            {
                using (var conn = DbContext.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@nama_fasilitas", fasilitas.NamaFasilitas);
                        cmd.Parameters.AddWithValue("@biaya_tambahan", fasilitas.BiayaTambahan);
                        cmd.Parameters.AddWithValue("@deskripsi", fasilitas.Deskripsi);
                        cmd.ExecuteNonQuery();

                    }
                    conn.Close();
                }
                //Database.CloseConnection();

            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show("Error: " + ex.Message);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);

            }
        }
        public static void Update(Fasilitas fasilitas)
        {
            //string conStr = "Server=localhost;port=5432;User Id=postgres;Password=postgres;Database=pbo akhir;";
            using (var conn = DbContext.GetConnection())
            {
                conn.Open();
                string sql = @" UPDATE fasilitas
                             SET nama_fasilitas = @nama_fasilitas,
                                 biaya_tambahan = @biaya_tambahan,
                                 deskripsi = @deskripsi
                                 WHERE id_fasilitas = @id_fasilitas";

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id_fasilitas", fasilitas.IDFasilitas);
                    cmd.Parameters.AddWithValue("@nama_fasilitas", fasilitas.NamaFasilitas ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@biaya_tambahan", fasilitas.BiayaTambahan);
                    cmd.Parameters.AddWithValue("@deskripsi", fasilitas.Deskripsi ?? (object)DBNull.Value);

                    cmd.ExecuteNonQuery();
                }
                conn.Close();
            }
        }
        public static void Delete(int id_fasilitas)
        {
            using (var conn = DbContext.GetConnection())
            {
                conn.Open();
                string sql = "DELETE FROM fasilitas WHERE id_fasilitas = @id_fasilitas";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id_fasilitas", id_fasilitas);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        
    }
}
