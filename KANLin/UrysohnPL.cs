using System;
using System.Collections.Generic;
using System.Text;

namespace KANLin
{
    class UrysohnPL
    {
        public UnivariatePL[] _univariateList = null;

        public UrysohnPL(double[] xmin, double[] xmax, double targetMin, double targetMax, int[] layers)
        {
            double ymin = targetMin / layers.Length;
            double ymax = targetMax / layers.Length;
            _univariateList = new UnivariatePL[layers.Length];
            for (int i = 0; i < _univariateList.Length; ++i)
            {
                _univariateList[i] = new UnivariatePL(xmin[i], xmax[i], ymin, ymax, layers[i]);
            }
        }

        public UrysohnPL(UrysohnPL obj)
        {
            _univariateList = new UnivariatePL[obj._univariateList.Length];
            for (int i = 0; i < obj._univariateList.Length; ++i)
            {
                _univariateList[i] = new UnivariatePL(obj._univariateList[i]);
            }
        }

        public void UpdateUsingInput(double delta, double[] inputs, double mu)
        {
            delta /= _univariateList.Length;
            for (int i = 0; i < _univariateList.Length; ++i)
            {
                _univariateList[i].UpdateUsingInput(inputs[i], delta, mu);
            }
        }

        public void UpdateUsingMemory(double delta, double mu)
        {
            delta /= _univariateList.Length;
            for (int i = 0; i < _univariateList.Length; ++i)
            {
                _univariateList[i].UpdateUsingMemory(delta, mu);
            }
        }

        public double GetValueUsingInput(double[] inputs)
        {
            double f = 0.0;
            for (int i = 0; i < _univariateList.Length; ++i)
            {
                f += _univariateList[i].GetFunctionUsingInput(inputs[i]);
            }
            return f;
        }
    }
}
