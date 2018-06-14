using Emgu.CV;
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV.XFeatures2D;
using ImageUnderstanding;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace ImageUnderstanding.FeatureGenerator
{
    public class HOGFeatureGenerator : FeatureGenerator<TaggedImage, float>
    {
        HOGDescriptor _hog;
        int _cellCountX;
        int _cellCountY;

        public HOGFeatureGenerator()
            : this(10, 10)
        { }

        public HOGFeatureGenerator(int cellCountX, int cellCountY)
        {
            _cellCountX = cellCountX;
            _cellCountY = cellCountY;
            _hog = new HOGDescriptor();
        }

        public override void Dispose()
        {
            _hog.Dispose();
        }

        public override List<float> GetFeatureVector(TaggedImage data)
        {
            Mat image = data.GetMat();
            
            Size cellSize = new Size(image.Cols / _cellCountX, image.Rows / _cellCountY);
            Size blockSize = new Size(2 * cellSize.Width, 2 * cellSize.Height);
            Size blockStride = new Size(cellSize.Width, cellSize.Height);
            Size winSize = new Size(image.Cols - (image.Cols % cellSize.Width), image.Rows - (image.Rows % cellSize.Height));

            _hog = new HOGDescriptor(winSize, blockSize, blockStride, cellSize);

            float[] result = _hog.Compute(image);

            //Console.WriteLine(data.Tag + " , " + result.Length);

            return new List<float>(result);
        }
    }
}
