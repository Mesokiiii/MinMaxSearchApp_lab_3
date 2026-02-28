using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace MinMaxSearchApp
{
    public partial class Form1 : Form
    {
        private TextBox txtArraySize;
        private ComboBox cmbArrayType;
        private Button btnRun;
        private Button btnClear;
        private Button btnHelp;
        private DataGridView gridResults;

        public Form1()
        {
            InitializeComponent();
            InitializeCustomUI();
            this.Text = "Анализ алгоритмов поиска экстремумов";
            this.Size = new Size(920, 450);
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void InitializeCustomUI()
        {
            Panel topPanel = new Panel { Dock = DockStyle.Top, Height = 60, Padding = new Padding(10) };

            Label lblSize = new Label { Text = "Размер массива (N):", Width = 120, Location = new Point(10, 20) };

            txtArraySize = new TextBox { Text = "10000", Width = 100, Location = new Point(130, 18) };
            txtArraySize.TextChanged += TxtArraySize_TextChanged;

            Label lblType = new Label { Text = "Тип массива:", Width = 80, Location = new Point(240, 20) };
            cmbArrayType = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 140, Location = new Point(320, 18) };
            cmbArrayType.Items.AddRange(new object[] { "Случайный", "По возрастанию", "По убыванию" });
            cmbArrayType.SelectedIndex = 0;

            btnRun = new Button { Text = "Запустить анализ", Width = 130, Location = new Point(480, 16), BackColor = Color.LightGreen, Cursor = Cursors.Hand };
            btnRun.Click += BtnRun_Click;

            btnClear = new Button { Text = "Очистить!", Width = 90, Location = new Point(620, 16), BackColor = Color.LightCoral, Cursor = Cursors.Hand };
            btnClear.Click += BtnClear_Click;

            btnHelp = new Button { Text = "Справка", Width = 90, Location = new Point(720, 16), BackColor = Color.LightYellow, Cursor = Cursors.Hand };
            btnHelp.Click += BtnHelp_Click;

            topPanel.Controls.Add(lblSize);
            topPanel.Controls.Add(txtArraySize);
            topPanel.Controls.Add(lblType);
            topPanel.Controls.Add(cmbArrayType);
            topPanel.Controls.Add(btnRun);
            topPanel.Controls.Add(btnClear);
            topPanel.Controls.Add(btnHelp);

            gridResults = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true,
                RowHeadersVisible = false,
                AllowUserToResizeRows = false
            };
            gridResults.Columns.Add("Algo", "Алгоритм");
            gridResults.Columns.Add("Min", "Минимум");
            gridResults.Columns.Add("Max", "Максимум");
            gridResults.Columns.Add("Comp", "Операций сравнения");
            gridResults.Columns.Add("Time", "Время (мс) - ср. за 10 зап.");

            this.Controls.Add(gridResults);
            this.Controls.Add(topPanel);
        }

        private void TxtArraySize_TextChanged(object sender, EventArgs e)
        {
            // Проверка: число ли это и входит ли в диапазон 1 - 1 000 0001
            bool isNumber = int.TryParse(txtArraySize.Text, out int size);

            if (isNumber && size >= 1 && size <= 1000000)
            {
                txtArraySize.BackColor = Color.White;
                btnRun.Enabled = true;
            }
            else
            {
                txtArraySize.BackColor = Color.LightPink;
                btnRun.Enabled = false;
            }
        }

        private void BtnRun_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtArraySize.Text, out int n) || n < 1 || n > 1000000)
            {
                MessageBox.Show("Ошибка! Введите целое число в диапазоне от 1 до 1 000 000.",
                                "Неверный ввод", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int[] data = GenerateArray(n, cmbArrayType.SelectedIndex);
            gridResults.Rows.Clear();

            RunAndDisplay(data, "Последовательный перебор", SearchAlgorithms.FindSequential);
            RunAndDisplay(data, "Разделяй и властвуй", SearchAlgorithms.FindDivideAndConquer);
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            txtArraySize.Text = "10000";
            cmbArrayType.SelectedIndex = 0;
            gridResults.Rows.Clear();
        }

        private void BtnHelp_Click(object sender, EventArgs e)
        {
            string helpText =
                "Добро пожаловать в систему анализа алгоритмов поиска!\n\n" +
                "ИНСТРУКЦИЯ ПО ЗАПУСКУ:\n" +
                "1. Укажите «Размер массива» (от 1 до 1 000 000). При вводе некорректных данных поле станет красным.\n" +
                "2. Выберите «Тип массива»:\n" +
                "   • Случайный — стандартный набор данных.\n" +
                "   • По возрастанию/убыванию — проверка поведения алгоритмов в крайних случаях.\n" +
                "3. Нажмите кнопку «Запустить анализ».\n\n" +
                "ЧТО ЗНАЧАТ КОЛОНКИ В ТАБЛИЦЕ?\n" +
                "• Минимум/Максимум: Найденные экстремумы.\n" +
                "• Операций сравнения: Математическое число проверок (if).\n" +
                "• Время (мс): Усредненное реальное время выполнения за 10 запусков.\n\n" +
                "ОЖИДАЕМЫЕ РЕЗУЛЬТАТЫ:\n" +
                "Метод «Разделяй и властвуй» совершает меньше операций сравнения, но может работать дольше из-за рекурсии.";

            MessageBox.Show(helpText, "Инструкция и описание работы", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private int[] GenerateArray(int size, int type)
        {
            int[] arr = new int[size];
            Random rnd = new Random();
            for (int i = 0; i < size; i++) arr[i] = rnd.Next(-10000, 10000);

            if (type == 1) Array.Sort(arr);
            if (type == 2) { Array.Sort(arr); Array.Reverse(arr); }
            return arr;
        }

        private void RunAndDisplay(int[] data, string algoName, Func<int[], MinMaxResult> searchMethod)
        {
            searchMethod(data); // Прогрев JIT

            int iterations = 10;
            Stopwatch sw = new Stopwatch();
            MinMaxResult result = new MinMaxResult();

            sw.Start();
            for (int i = 0; i < iterations; i++)
            {
                result = searchMethod(data);
            }
            sw.Stop();

            double avgTimeMs = sw.Elapsed.TotalMilliseconds / iterations;
            gridResults.Rows.Add(algoName, result.Min, result.Max, result.Comparisons, avgTimeMs.ToString("F4"));
        }
    }
}