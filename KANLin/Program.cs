//Concept: Andrew Polar and Mike Poluektov
//Developer Andrew Polar

//License
//In case if end user finds the way of making a profit by using this code and earns
//billions of US dollars and meet developer bagging change in the street near McDonalds,
//he or she is not in obligation to buy him a sandwich.

//Symmetricity
//In case developer became rich and famous by publishing this code and meet misfortunate
//end user who went bankrupt by using this code, he is also not in obligation to buy
//end user a sandwich.

//Publications:
//https://www.sciencedirect.com/science/article/abs/pii/S0016003220301149
//https://www.sciencedirect.com/science/article/abs/pii/S0952197620303742
//https://arxiv.org/abs/2305.08194

//Formula4 is area of triangles as a function of vertices, others are just algebraic expressions.

using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace KANLin
{
    internal class Program
    {
        private static (double[] xmin, double[] xmax, double targetMins, double targetMax) 
        FindMinMax(List<double[]> inputs, List<double> target)
        {
            int size = inputs[0].Length;
            double[] xmin = new double[size];
            double[] xmax = new double[size];

            for (int i = 0; i < size; ++i)
            {
                xmin[i] = double.MaxValue;
                xmax[i] = double.MinValue;
            }

            for (int i = 0; i < inputs.Count; ++i)
            {
                for (int j = 0; j < inputs[i].Length; ++j)
                {
                    if (inputs[i][j] < xmin[j]) xmin[j] = inputs[i][j];
                    if (inputs[i][j] > xmax[j]) xmax[j] = inputs[i][j];
                }

            }

            double targetMin = double.MaxValue;
            double targetMax = double.MinValue;
            for (int j = 0; j < target.Count; ++j)
            {
                if (target[j] < targetMin) targetMin = target[j];
                if (target[j] > targetMax) targetMax = target[j];
            }

            return (xmin, xmax, targetMin, targetMax);
        }

        static void Main(string[] args)
        {
            Formula3 f3 = new Formula3();
            (List<double[]> inputs, List<double> target) = f3.GenerateData(10000);

            (double[] xmin, double[] xmax, double targetMin, double targetMax) = FindMinMax(inputs, target);

            DateTime start = DateTime.Now;

            int nModels = 11;
            double zmin = targetMin / nModels;
            double zmax = targetMax / nModels;
            KANAddendPL[] addends = new KANAddendPL[nModels];
            for (int i = 0; i < nModels; ++i)
            {
                addends[i] = new KANAddendPL(xmin, xmax, zmin, zmax, 6, 12, 0.01, 0.01);
            }

            for (int epoch = 0; epoch < 51; ++epoch)
            {
                double error2 = 0.0;
                for (int i = 0; i < inputs.Count; ++i)
                {
                    double residual = target[i];
                    for (int j = 0; j < addends.Length; ++j)
                    {
                        residual -= addends[j].ComputeUsingInput(inputs[i]);
                    }
                    for (int j = 0; j < addends.Length; ++j)
                    {
                        //this method reuses properties computed in ComputeUsingInput
                        addends[j].UpdateUsingMemory(residual);

                        //next method updates independently without reusing properties computed
                        //by ComputeUsingInput
                        //addends[j].UpdateUsingInput(inputs[i], residual);
                    }
                    error2 += residual * residual;
                }
                error2 /= inputs.Count;
                error2 = Math.Sqrt(error2);
                error2 /= (targetMax - targetMin);
                if (0 == epoch % 25)
                {
                    Console.WriteLine("Training step {0}, relative RMSE {1:0.0000}", epoch, error2);
                }
            }

            DateTime end = DateTime.Now;
            TimeSpan duration = end - start;
            double time = duration.Minutes * 60.0 + duration.Seconds + duration.Milliseconds / 1000.0;
            Console.WriteLine("Time for building representation {0:####.00} seconds", time);

            //// Object copy test //////
            KANAddendPL[] addendsCopy = new KANAddendPL[addends.Length];
            for (int i = 0; i < addends.Length; ++i)
            {
                addendsCopy[i] = new KANAddendPL(addends[i]);
            }

            double error = 0.0;
            double error3 = 0.0;
            int NTests = 100;
            for (int i = 0; i < NTests; ++i)
            {
                double[] test_input = f3.GetInput();
                double test_target = f3.GetTarget(test_input);

                double model1 = 0.0;
                foreach (KANAddendPL modelPL in addends)
                {
                    model1 += modelPL.ComputeUsingInput(test_input);
                }

                double model2 = 0.0;
                foreach (KANAddendPL modelPL in addendsCopy)
                {
                    model2 += modelPL.ComputeUsingInput(test_input);
                }

                error += (test_target - model1) * (test_target - model1);
                error3 += (test_target - model2) * (test_target - model2);
            }
            error /= NTests;
            error = Math.Sqrt(error);
            error /= (targetMax - targetMin);
            error3 /= NTests;
            error3 = Math.Sqrt(error3);
            error3 /= (targetMax - targetMin);

            Console.WriteLine("\nRelative RMSE for unseen data {0:0.0000}, object copy {1:0.0000}", error, error3);
        }
    }
}

