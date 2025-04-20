using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;


namespace Knihovna
{
    public partial class Form1: Form
    {
        List<Kniha> books = new List<Kniha>();
        int selectedIndex = -1;
        string connectionString = "Data Source=knihovna.db;Version=3;";
        public Form1()

        {
            InitializeComponent();
            NactiKnihyZDatabaze();

        }
        private void RefreshGrid()
        {
            dataGridViewBooks.DataSource = null;
            dataGridViewBooks.DataSource = books;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            Kniha kniha = new Kniha
            {
                Nazev = txtTitle.Text,
                Autor = txtAuthor.Text,
                Zanr = txtGenre.Text,
                Rok = int.TryParse(txtYear.Text, out int year) ? year : 0
            };

            books.Add(kniha);
            RefreshGrid();
            ClearFields();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (selectedIndex >= 0 && selectedIndex < books.Count)
            {
                books[selectedIndex].Nazev = txtTitle.Text;
                books[selectedIndex].Autor = txtAuthor.Text;
                books[selectedIndex].Zanr = txtGenre.Text;
                books[selectedIndex].Rok = int.TryParse(txtYear.Text, out int year) ? year : 0;

                RefreshGrid();
                ClearFields();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedIndex >= 0 && selectedIndex < books.Count)
            {
                books.RemoveAt(selectedIndex);
                RefreshGrid();
                ClearFields();
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        private void ClearFields()
        {
            txtTitle.Text = "";
            txtAuthor.Text = "";
            txtGenre.Text = "";
            txtYear.Text = "";
            selectedIndex = -1;
            dataGridViewBooks.ClearSelection();
        }

        private void dataGridViewBooks_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                selectedIndex = e.RowIndex;
                var book = books[selectedIndex];
                txtTitle.Text = book.Nazev;
                txtAuthor.Text = book.Autor;
                txtGenre.Text = book.Zanr;
                txtYear.Text = book.Rok.ToString();
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string query = txtSearch.Text.ToLower();
            var filtered = books.Where(b =>
                b.Nazev.ToLower().Contains(query) ||
                b.Autor.ToLower().Contains(query) ||
                b.Zanr.ToLower().Contains(query)
            ).ToList();

            dataGridViewBooks.DataSource = null;
            dataGridViewBooks.DataSource = filtered;
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                RefreshGrid();
            }
        }

        private void btnExportCsv_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "CSV files (*.csv)|*.csv";
            saveFileDialog.Title = "Export knih do CSV";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                using (StreamWriter sw = new StreamWriter(saveFileDialog.FileName))
                {
                    // Hlavička
                    sw.WriteLine("Nazev,Autor,Zanr,Rok");

                    // Data
                    foreach (var book in books)
                    {
                        string line = $"{Escape(book.Nazev)},{Escape(book.Autor)},{Escape(book.Zanr)},{book.Rok}";
                        sw.WriteLine(line);
                    }
                }

                MessageBox.Show("Export dokončen!", "Hotovo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private string Escape(string input)
        {
            // Ošetříme čárky a uvozovky v textu
            if (input.Contains(",") || input.Contains("\""))
            {
                input = "\"" + input.Replace("\"", "\"\"") + "\"";
            }
            return input;
        }

        private void btnSort_Click(object sender, EventArgs e)
        {
            string sortBy = cmbSortBy.SelectedItem.ToString();

            switch (sortBy)
            {
                case "Název":
                    books = books.OrderBy(b => b.Nazev).ToList();
                    break;
                case "Autor":
                    books = books.OrderBy(b => b.Autor).ToList();
                    break;
                case "Žánr":
                    books = books.OrderBy(b => b.Zanr).ToList();
                    break;
                case "Rok":
                    books = books.OrderBy(b => b.Rok).ToList();
                    break;
            }

            RefreshGrid();
        }
        private void NactiKnihyZDatabaze()
        {
            books.Clear();

            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM Knihy";
                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        books.Add(new Kniha
                        {
                            Nazev = reader["Nazev"].ToString(),
                            Autor = reader["Autor"].ToString(),
                            Zanr = reader["Zanr"].ToString(),
                            Rok = Convert.ToInt32(reader["Rok"])
                        });
                    }
                }
            }

            RefreshGrid();
            dataGridViewBooks.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

        }

    }
}
