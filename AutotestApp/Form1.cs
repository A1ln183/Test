using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace AutotestApp
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

        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Title = "Выберите исходный файл программы";
                dlg.InitialDirectory = Application.StartupPath;
                dlg.Filter = "Файлы C# (*.cs)|*.cs|Текстовые документы (*.txt)|*.txt";

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    textBox1.Text = dlg.FileName;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Title = "Выберите файл с тестами";
                dlg.InitialDirectory = Application.StartupPath;
                dlg.Filter = "Текстовые документы (*.txt)|*.txt"; //Файлы тестов (*.test)|*.test|

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    textBox2.Text = dlg.FileName;
                }
            }
        }

        string csc_path = @"C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe";

        private void button3_Click(object sender, EventArgs e)
        {
            textBox3.Clear();

            // Compile Program

            string result = Run(csc_path, $"/out:test.exe {textBox1.Text}");

            if (result.Contains("error"))
            {
                textBox3.Text += "Ошибка при компиляции программы!\r\n";

                string[] errors = result.Split("\r\n".ToCharArray());
                for (int i = 10; i < errors.Length; i++)
                {
                    if (errors[i].Contains("error")) textBox3.Text += "\r\n" + errors[i];
                }

                return;
            }

            textBox3.Text += "Программа скомпилирована успешно!\r\n\r\n";

            // Parse .test file and check program

            string[] line = File.ReadAllLines(textBox2.Text);
            short j = 1;

            for (int i = 0; i < line.Length - 1; i += 2)
            {
                File.WriteAllText("input.txt", line[i].Replace(@"\n", "\r\n"));

                if (Run("cmd.exe", $"/c test.exe < input.txt") == line[i + 1].Replace(@"\n", "\r\n"))
                {
                    textBox3.Text += $"Тест №{j} пройден\r\n";
                }
                else
                {
                    textBox3.Text += $"Тест №{j} не пройден\r\n";
                    textBox3.Text += $"\r\nЗадание не выполнено, программа работает некорректно\r\n";
                    return;
                }

                j++;
            }

            textBox3.Text += $"\r\nЗадание выполнено успешно, программа принята!\r\n";

            if (File.Exists("input.txt")) File.Delete("input.txt");
            if (File.Exists("test.exe")) File.Delete("test.exe");
        }

        string Run(string file, string arguments)
        {
            Process process = new Process();
            process.StartInfo = new ProcessStartInfo()
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                FileName = file,
                Arguments = arguments,
                StandardOutputEncoding = Encoding.GetEncoding(866),
                RedirectStandardOutput = true
            };
            process.Start();

            string result = process.StandardOutput.ReadToEnd();
            //process.WaitForExit();
            
            return result;
        }
    }
}
