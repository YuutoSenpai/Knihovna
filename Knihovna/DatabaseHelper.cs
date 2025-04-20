using Knihovna;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

public static class DatabaseHelper
{
    private static string connString = "Data Source=library.db;Version=3;";

    public static void InitializeDatabase()
    {
        using (var conn = new SQLiteConnection(connString))
        {
            conn.Open();
            string sql = @"CREATE TABLE IF NOT EXISTS Books (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            Title TEXT NOT NULL,
                            Author TEXT NOT NULL,
                            Genre TEXT,
                            Year INTEGER
                        )";
            var cmd = new SQLiteCommand(sql, conn);
            cmd.ExecuteNonQuery();
        }
    }

    public static void PridejKnihu(Kniha kniha)
    {
        using (var conn = new SQLiteConnection(connString))
        {
            conn.Open();
            string sql = "INSERT INTO Books (Title, Author, Genre, Year) VALUES (@Title, @Author, @Genre, @Year)";
            var cmd = new SQLiteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Title", kniha.Title);
            cmd.Parameters.AddWithValue("@Author", kniha.Author);
            cmd.Parameters.AddWithValue("@Genre", kniha.Genre);
            cmd.Parameters.AddWithValue("@Year", kniha.Year);
            cmd.ExecuteNonQuery();
        }
    }

    public static DataTable ZiskejKnihu()
    {
        DataTable dt = new DataTable();
        using (var conn = new SQLiteConnection(connString))
        {
            conn.Open();
            string sql = "SELECT * FROM Books";
            var da = new SQLiteDataAdapter(sql, conn);
            da.Fill(dt);
        }
        return dt;
    }
}
