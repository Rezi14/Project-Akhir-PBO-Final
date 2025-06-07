using Sistem_Reservasi_Hotel.Models;
using Npgsql;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace Sistem_Reservasi_Hotel.Controller
{
    public class DaftarController
    {
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (var b in bytes)
                    builder.Append(b.ToString("x2"));
                return builder.ToString();
            }
        }

        public bool Daftar(string username, string password)
        {
            try
            {
                using (var conn = DbContext.GetConnection())
                {
                    conn.Open();

                    string checkQuery = "SELECT COUNT(*) FROM akun WHERE username = @username";
                    using (var checkCmd = new NpgsqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@username", username);
                        int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                        if (count > 0)
                        {
                            MessageBox.Show("Username sudah digunakan. Gunakan username lain.");
                            return false;
                        }
                    }

                    string hashedPassword = HashPassword(password);

                    string insertQuery = "INSERT INTO akun (username, password) VALUES (@username, @password)";
                    using (var insertCmd = new NpgsqlCommand(insertQuery, conn))
                    {
                        insertCmd.Parameters.AddWithValue("@username", username);
                        insertCmd.Parameters.AddWithValue("@password", hashedPassword);
                        insertCmd.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal mendaftar: " + ex.Message);
                return false;
            }
        }
    }
}