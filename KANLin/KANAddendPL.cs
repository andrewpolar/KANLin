using System;
using System.Collections.Generic;
using System.Text;

namespace KANLin
{
    class KANAddendPL
    {
        public double _targetMin;
        public double _targetMax;
        public double _muOuter;
        public double _muInner;
        private double _lastInnerValue;
        private UrysohnPL _uPL = null;
        private UnivariatePL _univariate = null;

        public KANAddendPL(
            double[] xmin, double[] xmax, double targetMin, double targetMax,
            int inner, int outer, double muInner, double muOuter)
        {
            _muInner = muInner;
            _muOuter = muOuter;
            _targetMin = targetMin;
            _targetMax = targetMax;
            int number_of_inputs = xmin.Length;

            int[] interior_structure = new int[number_of_inputs];
            for (int i = 0; i < number_of_inputs; i++)
            {
                interior_structure[i] = inner;
            }
            _uPL = new UrysohnPL(xmin, xmax, _targetMin, _targetMax, interior_structure);
            _univariate = new UnivariatePL(_targetMin, _targetMax, _targetMin, _targetMax, outer);
        }

        public KANAddendPL(KANAddendPL obj)
        {
            this._muInner = obj._muInner;
            this._muOuter = obj._muOuter;
            this._targetMin = obj._targetMin;
            this._targetMax = obj._targetMax;
            this._univariate = new UnivariatePL(obj._univariate);
            this._uPL = new UrysohnPL(obj._uPL);
        }

        public void UpdateUsingMemory(double diff)
        {
            double derrivative = _univariate.GetDerrivative(_lastInnerValue);
            _uPL.UpdateUsingMemory(diff * derrivative, _muInner);
            _univariate.UpdateUsingMemory(diff, _muOuter);
        }

        public void UpdateUsingInput(double[] input, double diff)
        {
            double value = _uPL.GetValueUsingInput(input);
            double derrivative = _univariate.GetDerrivative(value);
            _uPL.UpdateUsingInput(diff * derrivative, input, _muInner);
            _univariate.UpdateUsingInput(value, diff, _muOuter);
        }

        public double ComputeUsingInput(double[] input)
        {
            _lastInnerValue = _uPL.GetValueUsingInput(input);
            return _univariate.GetFunctionUsingInput(_lastInnerValue);
        }
    }
}
