using System;

namespace MinMaxSearchApp
{
    // Структура для хранения результатов
    public struct MinMaxResult
    {
        public int Min { get; }
        public int Max { get; }
        public long Comparisons { get; }

        public MinMaxResult(int min, int max, long comparisons)
        {
            Min = min;
            Max = max;
            Comparisons = comparisons;
        }
    }

    // Класс с алгоритмами
    public static class SearchAlgorithms
    {
        // 1. Последовательный перебор
        public static MinMaxResult FindSequential(int[] data)
        {
            if (data == null || data.Length == 0)
                throw new ArgumentException("Массив пуст или null");

            int min = data[0];
            int max = data[0];
            long comp = 0;

            for (int i = 1; i < data.Length; i++)
            {
                comp++;
                if (data[i] < min)
                {
                    min = data[i];
                }
                else
                {
                    comp++;
                    if (data[i] > max)
                        max = data[i];
                }
            }
            return new MinMaxResult(min, max, comp);
        }

        // 2. Метод "Разделяй и властвуй"
        public static MinMaxResult FindDivideAndConquer(int[] data)
        {
            if (data == null || data.Length == 0)
                throw new ArgumentException("Массив пуст или null");

            return FindRecursive(data, 0, data.Length - 1);
        }

        private static MinMaxResult FindRecursive(int[] arr, int left, int right)
        {
            // Базовый случай: 1 элемент
            if (left == right)
                return new MinMaxResult(arr[left], arr[left], 0);

            // Базовый случай: 2 элемента
            if (right - left == 1)
            {
                if (arr[left] < arr[right])
                    return new MinMaxResult(arr[left], arr[right], 1);
                else
                    return new MinMaxResult(arr[right], arr[left], 1);
            }

            // Разделение
            int mid = left + (right - left) / 2;
            var leftRes = FindRecursive(arr, left, mid);
            var rightRes = FindRecursive(arr, mid + 1, right);

            // Слияние
            int min = leftRes.Min < rightRes.Min ? leftRes.Min : rightRes.Min;
            int max = leftRes.Max > rightRes.Max ? leftRes.Max : rightRes.Max;
            long comp = leftRes.Comparisons + rightRes.Comparisons + 2;

            return new MinMaxResult(min, max, comp);
        }
    }
}