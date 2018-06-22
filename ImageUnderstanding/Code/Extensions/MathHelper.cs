using Emgu.CV.CvEnum;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Emgu.CV
{
    public static class FloatArrayExtension
    {
        public static float Mean(this float[] values)
        {
            float result = 0F;
            for(int i = 0; i < values.Length; ++i)
            {
                result += values[i];
            }
            return result / (float)values.Length;
        }

        public static float StdDeviation(this float[] values)
        {
            return values.StdDeviation(values.Mean());
        }

        public static float StdDeviation(this float[] values, float mean)
        {
            float result = 0F;

            for (int i = 0; i < values.Length; ++i)
            {
                float f = values[i] - mean;
                result += (f * f) / (float)(values.Length - 1);
            }

            return (float)Math.Sqrt(result);
        }
    }
}
