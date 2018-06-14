using Emgu.CV;
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV.XFeatures2D;
using ImageUnderstanding;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImageUnderstanding.FeatureGenerator
{
    public class MyFeatureGenerator : FeatureGenerator<TaggedImage, float>
    {
        HOGDescriptor _hog;
        int _cellCountX;
        int _cellCountY;

        public MyFeatureGenerator()
            : this(10, 10)
        { }

        public MyFeatureGenerator(int cellCountX, int cellCountY)
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
            return new List<float>();
        }

        List<float> ExtractSiftFeatureVector(TaggedImage image, int keyPointCount, SiftSortingMethod sortingMethod, bool doDrawImage)
        {
            List<float> result = new List<float>(5 * keyPointCount);


            return result;
        }

    }
}
