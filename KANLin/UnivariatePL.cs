using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace KANLin
{
    class UnivariatePL
    {
        public int _points;
        public double _xmin;
        public double _xmax;
        public double _ymin;
        public double _ymax;
        private double _deltax;
        private int _lastLeftIndex;
        private double _lastLeftOffset;
        private double[] _y = null;
        private Random _rnd = new Random();

        public UnivariatePL(double xmin, double xmax, double ymin, double ymax, int points)
        {
            _points = points;
            _xmin = xmin;
            _xmax = xmax;
            _ymin = ymin;
            _ymax = ymax;
            SetLimits();
            SetRandomFunction();
        }

        public UnivariatePL(UnivariatePL obj)
        {
            this._points = obj._points;
            this._xmin = obj._xmin;
            this._xmax = obj._xmax;
            this._ymin = obj._ymin;
            this._ymax = obj._ymax;
            this._deltax = (this._xmax - this._xmin) / (this._points - 1);
            _y = new double[_points];
            for (int i = 0; i < _points; i++)
            {
                _y[i] = obj._y[i];
            }
        }

        private void SetRandomFunction()
        {
            _y = new double[_points];
            for (int i = 0; i < _points; ++i)
            {
                _y[i] = _rnd.Next(10, 1000) / 1000.0;
            }
            double min = _y.Min();
            double max = _y.Max();
            if (min == max) max = min + 1.0;
            for (int i = 0; i < _points; ++i)
            {
                _y[i] = _y[i] * (_ymax - _ymin) + _ymin;
            }
        }

        private void SetLimits()
        {
            double range = _xmax - _xmin;
            _xmin -= 0.01 * range;
            _xmax += 0.01 * range;
            _deltax = (_xmax - _xmin) / (_points - 1);
        }

        private void FitDefinition(double x)
        {
            if (x < _xmin)
            {
                _xmin = x;
                SetLimits();
            }
            if (x > _xmax)
            {
                _xmax = x;
                SetLimits();
            }
        }

        public double GetDerrivative(double x)
        {
            int low = (int)((x - _xmin) / _deltax);
            return (_y[low + 1] - _y[low]) / _deltax;
        }

        public void UpdateUsingInput(double x, double delta, double mu)
        {
            FitDefinition(x);
            delta *= mu;
            double offset = (x - _xmin) / _deltax;
            int left = (int)(offset);
            double leftx = offset - left;
            _y[left + 1] += delta * leftx;
            _y[left] += delta * (1.0 - leftx);
        }

        public void UpdateUsingMemory(double delta, double mu)
        {
            delta *= mu;
            _y[_lastLeftIndex + 1] += delta * _lastLeftOffset;
            _y[_lastLeftIndex] += delta * (1.0 - _lastLeftOffset);
        }

        public double GetFunctionUsingInput(double x)
        {
            FitDefinition(x);
            double offset = (x - _xmin) / _deltax;
            int leftIndex = (int)(offset);
            double leftOffset = offset - leftIndex;
            _lastLeftIndex = leftIndex;
            _lastLeftOffset = leftOffset;
            return _y[leftIndex] + (_y[leftIndex + 1] - _y[leftIndex]) * leftOffset;
        }
    }
}