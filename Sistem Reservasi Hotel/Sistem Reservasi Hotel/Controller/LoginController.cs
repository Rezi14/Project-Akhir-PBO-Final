﻿using Sistem_Reservasi_Hotel.Models;
using Npgsql;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace Sistem_Reservasi_Hotel.Controller
{
    public class LoginController
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

        public Akun ValidasiLogin(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return null;

            string hashedPassword = HashPassword(password);

            try
            {
                using (var conn = DbContext.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT * FROM akun WHERE username = @username AND password = @password";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@password", hashedPassword);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Akun
                                {
                                    IDAkun = reader.GetInt32(reader.GetOrdinal("id_akun")),
                                    Username = reader.GetString(reader.GetOrdinal("username")),
                                    Password = reader.GetString(reader.GetOrdinal("password"))
                                };
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine("[Database Error] ValidasiLogin: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("[Error] ValidasiLogin: " + ex.Message);
            }

            return null;
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