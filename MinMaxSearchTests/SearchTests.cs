using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using MinMaxSearchApp;

namespace MinMaxSearchTests
{
    [TestClass]
    public class SearchTests
    {
        // Вспомогательный метод для проверки обоих алгоритмов сразу
        private void AssertBothAlgorithms(int[] input, int expectedMin, int expectedMax)
        {
            var seqResult = SearchAlgorithms.FindSequential(input);
            var dncResult = SearchAlgorithms.FindDivideAndConquer(input);

            Assert.AreEqual(expectedMin, seqResult.Min, "Seq Min failed");
            Assert.AreEqual(expectedMax, seqResult.Max, "Seq Max failed");

            Assert.AreEqual(expectedMin, dncResult.Min, "DnC Min failed");
            Assert.AreEqual(expectedMax, dncResult.Max, "DnC Max failed");
        }

        [TestMethod]
        public void T01_NormalRandomArray()
        {
            int[] arr = { 4, 2, 8, 1, 9, 3 };
            AssertBothAlgorithms(arr, 1, 9);
        }

        [TestMethod]
        public void T02_ArraySortedAscending()
        {
            int[] arr = { 1, 2, 3, 4, 5, 6 };
            AssertBothAlgorithms(arr, 1, 6);
        }

        [TestMethod]
        public void T03_ArraySortedDescending()
        {
            int[] arr = { 6, 5, 4, 3, 2, 1 };
            AssertBothAlgorithms(arr, 1, 6);
        }

        [TestMethod]
        public void T04_AllElementsIdentical()
        {
            int[] arr = { 5, 5, 5, 5, 5 };
            AssertBothAlgorithms(arr, 5, 5);
        }

        [TestMethod]
        public void T05_MinAtStartMaxAtEnd()
        {
            int[] arr = { -10, 0, 4, 8, 20 };
            AssertBothAlgorithms(arr, -10, 20);
        }

        [TestMethod]
        public void T06_MaxAtStartMinAtEnd()
        {
            int[] arr = { 50, 12, 4, 0, -5 };
            AssertBothAlgorithms(arr, -5, 50);
        }

        [TestMethod]
        public void T07_AllNegativeNumbers()
        {
            int[] arr = { -5, -1, -10, -3 };
            AssertBothAlgorithms(arr, -10, -1);
        }

        [TestMethod]
        public void T08_MixedPositiveNegative()
        {
            int[] arr = { -10, 5, 0, -3, 8 };
            AssertBothAlgorithms(arr, -10, 8);
        }

        [TestMethod]
        public void T09_SingleElementArray()
        {
            int[] arr = { 42 };
            AssertBothAlgorithms(arr, 42, 42);
        }

        [TestMethod]
        public void T10_TwoElementsArray()
        {
            int[] arr = { 15, 7 };
            AssertBothAlgorithms(arr, 7, 15);
        }

        [TestMethod]
        public void T11_EmptyArrayException()
        {
            int[] arr = new int[0];
            Assert.ThrowsException<ArgumentException>(() => SearchAlgorithms.FindSequential(arr));
            Assert.ThrowsException<ArgumentException>(() => SearchAlgorithms.FindDivideAndConquer(arr));
        }

        [TestMethod]
        public void T12_NullArrayException()
        {
            int[] arr = null;
            Assert.ThrowsException<ArgumentException>(() => SearchAlgorithms.FindSequential(arr));
            Assert.ThrowsException<ArgumentException>(() => SearchAlgorithms.FindDivideAndConquer(arr));
        }

        [TestMethod]
        public void T13_OddNumberOfElements()
        {
            int[] arr = { 1, 2, 3, 4, 5 };
            AssertBothAlgorithms(arr, 1, 5);
        }

        [TestMethod]
        public void T14_EvenNumberOfElements()
        {
            int[] arr = { 1, 2, 3, 4 };
            AssertBothAlgorithms(arr, 1, 4);
        }

        [TestMethod]
        public void T15_ExtremeIntegerValues()
        {
            int[] arr = { int.MinValue, 0, int.MaxValue };
            AssertBothAlgorithms(arr, int.MinValue, int.MaxValue);
        }
    }
}