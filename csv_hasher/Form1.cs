using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CsvHelper;
using System.IO;
using System.Security.Cryptography;

namespace csv_hasher
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        Dictionary<int, string> headers;
        Dictionary<int, string> hash;
        Dictionary<int, string> export;

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();

            listBox1.Items.Clear();
            listBox2.Items.Clear();
            headers = new Dictionary<int, string>();
            hash = new Dictionary<int, string>();
            export = new Dictionary<int, string>();

            using (TextReader file = File.OpenText(openFileDialog1.FileName))
            {
                using (CsvParser parser = new CsvParser(file))
                {
                    using (CsvReader reader = new CsvReader(parser))
                    {
                        reader.Read();
                        int count = 0;
                        foreach (var header in reader.FieldHeaders)
                        {
                            listBox1.Items.Add(header);
                            listBox2.Items.Add(header);
                            headers.Add(count++, header);
                        }
                    }
                }
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            hash = new Dictionary<int, string>();
            foreach (var selected in listBox1.SelectedIndices)
            {
                if (!listBox2.SelectedIndices.Contains((int)selected))
                {
                    listBox2.SelectedIndices.Add((int)selected);
                    hash.Add((int)selected, headers[(int)selected]);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();

            using (TextReader file = File.OpenText(openFileDialog1.FileName))
            {
                using (CsvParser parser = new CsvParser(file))
                {
                    using (CsvReader reader = new CsvReader(parser))
                    {
                        using (TextWriter newfile = File.CreateText(saveFileDialog1.FileName))
                        {
                            using (CsvWriter writer = new CsvWriter(newfile))
                            {
                                reader.Read();

                                foreach (var header in export)
                                {
                                    writer.WriteField(header.Value);
                                }

                                while (reader.Read())
                                {
                                    writer.NextRecord();
                                    foreach (var field in export)
                                    {
                                        var content = reader.GetField(field.Key);
                                        if (hash.ContainsKey(field.Key))
                                        {
                                            byte[] buffer = Encoding.UTF8.GetBytes(content);
                                            var sha = SHA1Managed.Create();
                                            content = BitConverter.ToString(sha.ComputeHash(buffer)).Replace("-", "").ToLower();
                                        }
                                        writer.WriteField(content);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            export = new Dictionary<int, string>();

            foreach (var selected in listBox2.SelectedIndices)
            {
                if (!export.ContainsKey((int)selected))
                    export.Add((int)selected, headers[(int)selected]);
            }
        }
    }
}
