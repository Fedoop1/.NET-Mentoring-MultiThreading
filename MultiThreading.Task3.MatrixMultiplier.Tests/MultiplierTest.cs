using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Engine.ClientProtocol;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MultiThreading.Task3.MatrixMultiplier.Matrices;
using MultiThreading.Task3.MatrixMultiplier.Multipliers;

namespace MultiThreading.Task3.MatrixMultiplier.Tests
{
    [TestClass]
    public class MultiplierTest
    {
        [TestMethod]
        public void MultiplyMatrix3On3Test()
        {
            TestMatrix3On3(new MatricesMultiplier());
            TestMatrix3On3(new MatricesMultiplierParallel());
        }

        [TestMethod]
        public void ParallelEfficiencyTest()
        {
            var matricesMultiplier = new MatricesMultiplier();
            var matricesMultiplierParallel = new MatricesMultiplierParallel();
            
            long syncExecTime = default;
            long asyncExecTime = default;
            var effectiveMatrixSize = 1;

            while (syncExecTime >= asyncExecTime)
            {
                effectiveMatrixSize++;

                var leftMatrix = new Matrix(effectiveMatrixSize, effectiveMatrixSize, true);
                var rightMatrix = new Matrix(effectiveMatrixSize, effectiveMatrixSize, true);

                var syncMultiplier = Task.Run(() => MeasureMultiplierTime(matricesMultiplier, leftMatrix, rightMatrix))
                    .ContinueWith(syncTime => syncExecTime = syncTime.Result);

                var asyncMultiplier = Task.Run(() => MeasureMultiplierTime(matricesMultiplierParallel, leftMatrix, rightMatrix))
                    .ContinueWith(asyncTime => asyncExecTime = asyncTime.Result);

                Task.WaitAll(syncMultiplier, asyncMultiplier);
            }

            Console.WriteLine($"Parallel multiplying effective from {effectiveMatrixSize}x{effectiveMatrixSize}");
        }

        #region private methods

        void TestMatrix3On3(IMatricesMultiplier matrixMultiplier)
        {
            if (matrixMultiplier == null)
            {
                throw new ArgumentNullException(nameof(matrixMultiplier));
            }

            var m1 = new Matrix(3, 3);
            m1.SetElement(0, 0, 34);
            m1.SetElement(0, 1, 2);
            m1.SetElement(0, 2, 6);

            m1.SetElement(1, 0, 5);
            m1.SetElement(1, 1, 4);
            m1.SetElement(1, 2, 54);

            m1.SetElement(2, 0, 2);
            m1.SetElement(2, 1, 9);
            m1.SetElement(2, 2, 8);

            var m2 = new Matrix(3, 3);
            m2.SetElement(0, 0, 12);
            m2.SetElement(0, 1, 52);
            m2.SetElement(0, 2, 85);

            m2.SetElement(1, 0, 5);
            m2.SetElement(1, 1, 5);
            m2.SetElement(1, 2, 54);

            m2.SetElement(2, 0, 5);
            m2.SetElement(2, 1, 8);
            m2.SetElement(2, 2, 9);

            var multiplied = matrixMultiplier.Multiply(m1, m2);
            Assert.AreEqual(448, multiplied.GetElement(0, 0));
            Assert.AreEqual(1826, multiplied.GetElement(0, 1));
            Assert.AreEqual(3052, multiplied.GetElement(0, 2));

            Assert.AreEqual(350, multiplied.GetElement(1, 0));
            Assert.AreEqual(712, multiplied.GetElement(1, 1));
            Assert.AreEqual(1127, multiplied.GetElement(1, 2));

            Assert.AreEqual(109, multiplied.GetElement(2, 0));
            Assert.AreEqual(213, multiplied.GetElement(2, 1));
            Assert.AreEqual(728, multiplied.GetElement(2, 2));
        }

        long MeasureMultiplierTime(IMatricesMultiplier multiplier, IMatrix leftMatrix, IMatrix rightMatrix)
        {
            var stopwatch = Stopwatch.StartNew();

            multiplier.Multiply(leftMatrix, rightMatrix);
            stopwatch.Stop();

            return stopwatch.ElapsedMilliseconds;
        }

        #endregion
    }
}