using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MinMaxSearchApp
{
    public partial class Form1 : Form
    {
        private NumericUpDown numArraySize;
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
            this.Size = new Size(900, 450); // Немного увеличили ширину окна для новых кнопок
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void InitializeCustomUI()
        {
            // Настройка панели управления
            Panel topPanel = new Panel { Dock = DockStyle.Top, Height = 60, Padding = new Padding(10) };

            Label lblSize = new Label { Text = "Размер массива (N):", Width = 120, Location = new Point(10, 20) };
            numArraySize = new NumericUpDown { Minimum = 10, Maximum = 10000000, Value = 10000, Width = 100, Location = new Point(130, 18) };

            Label lblType = new Label { Text = "Тип массива:", Width = 80, Location = new Point(240, 20) };
            cmbArrayType = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 140, Location = new Point(320, 18) };
            cmbArrayType.Items.AddRange(new object[] { "Случайный", "По возрастанию", "По убыванию" });
            cmbArrayType.SelectedIndex = 0;

            // Кнопка ЗАПУСКА
            btnRun = new Button { Text = "Запустить анализ", Width = 130, Location = new Point(480, 16), BackColor = Color.LightGreen, Cursor = Cursors.Hand };
            btnRun.Click += BtnRun_Click;

            // Кнопка ОЧИСТКИ
            btnClear = new Button { Text = "Очистить", Width = 90, Location = new Point(620, 16), BackColor = Color.LightCoral, Cursor = Cursors.Hand };
            btnClear.Click += BtnClear_Click;

            // Кнопка СПРАВКИ
            btnHelp = new Button { Text = "Справка", Width = 90, Location = new Point(720, 16), BackColor = Color.LightYellow, Cursor = Cursors.Hand };
            btnHelp.Click += BtnHelp_Click;

            topPanel.Controls.Add(lblSize);
            topPanel.Controls.Add(numArraySize);
            topPanel.Controls.Add(lblType);
            topPanel.Controls.Add(cmbArrayType);
            topPanel.Controls.Add(btnRun);
            topPanel.Controls.Add(btnClear);
            topPanel.Controls.Add(btnHelp);

            // Настройка таблицы результатов
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

        private void BtnRun_Click(object sender, EventArgs e)
        {
            int n = (int)numArraySize.Value;
            int[] data = GenerateArray(n, cmbArrayType.SelectedIndex);

            gridResults.Rows.Clear();

            // 1. Последовательный алгоритм
            RunAndDisplay(data, "Последовательный перебор", SearchAlgorithms.FindSequential);

            // 2. Разделяй и властвуй
            RunAndDisplay(data, "Разделяй и властвуй", SearchAlgorithms.FindDivideAndConquer);
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            // Сброс значений к значениям по умолчанию
            numArraySize.Value = 10000;
            cmbArrayType.SelectedIndex = 0;

            // Очистка таблицы
            gridResults.Rows.Clear();
        }

        private void BtnHelp_Click(object sender, EventArgs e)
        {
            string helpText =
                "Добро пожаловать в систему анализа алгоритмов поиска!\n\n" +
                "ИНСТРУКЦИЯ ПО ЗАПУСКУ:\n" +
                "1. Укажите «Размер массива» (от 10 до 10 000 000). Чем больше число, тем заметнее разница во времени работы.\n" +
                "2. Выберите «Тип массива»:\n" +
                "   • Случайный — стандартный набор данных.\n" +
                "   • По возрастанию/убыванию — проверка поведения алгоритмов в крайних (лучших/худших) случаях.\n" +
                "3. Нажмите кнопку «Запустить анализ».\n\n" +
                "ЧТО ЗНАЧАТ КОЛОНКИ В ТАБЛИЦЕ?\n" +
                "• Минимум/Максимум: Найденные экстремумы (для проверки корректности алгоритма).\n" +
                "• Операций сравнения: Точное математическое число проверок (if), которые выполнил алгоритм.\n" +
                "• Время (мс): Реальное время выполнения в миллисекундах. Значение усредняется за 10 запусков для точности.\n\n" +
                "ОЖИДАЕМЫЕ РЕЗУЛЬТАТЫ (Согласно отчету):\n" +
                "Метод «Разделяй и властвуй» математически совершает на ~25% МЕНЬШЕ операций сравнения. " +
                "Однако, реального времени (мс) он может тратить БОЛЬШЕ из-за накладных расходов процессора " +
                "на рекурсивные вызовы функции и кэш-промахи. Это нормальное явление, доказывающее разницу между " +
                "«алгоритмической» и «практической» сложностью.\n\n" +
                "Кнопка «Очистить» вернет интерфейс в исходное состояние.";

            MessageBox.Show(helpText, "Инструкция и описание работы", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private int[] GenerateArray(int size, int type)
        {
            int[] arr = new int[size];
            Random rnd = new Random();
            for (int i = 0; i < size; i++) arr[i] = rnd.Next(-10000, 10000);

            if (type == 1) Array.Sort(arr); // По возрастанию
            if (type == 2) { Array.Sort(arr); Array.Reverse(arr); } // По убыванию
            return arr;
        }

        private void RunAndDisplay(int[] data, string algoName, Func<int[], MinMaxResult> searchMethod)
        {
            // Прогрев (JIT компиляция), чтобы не было ложных задержек при первом запуске
            searchMethod(data);

            int iterations = 10; // Как указано в отчете
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