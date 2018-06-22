using Emgu.CV;
using Emgu.CV.Features2D;
using Emgu.CV.ML;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV.XFeatures2D;
using ImageUnderstanding;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImageUnderstanding.FeatureGenerator
{
    [Serializable]
    public class MyFeatureGeneratorInitializationData
    {
        public int cellCountX = 10;
        public int cellCountY = 10;
    }

    public class MyFeatureGenerator : FeatureGenerator<TaggedImage, float>
    {
        HOGDescriptor _hog;
        int _cellCountX;
        int _cellCountY;

        public MyFeatureGenerator()
            : this(new MyFeatureGeneratorInitializationData())
        { }

        public MyFeatureGenerator(MyFeatureGeneratorInitializationData initiailzationData)
        {
            _cellCountX = initiailzationData.cellCountX;
            _cellCountY = initiailzationData.cellCountY;
            _hog = new HOGDescriptor();
        }

        public override void InitializeViaConfig(ImageUnderstandingConfig config)
        {
            _cellCountX = config.MyInitializationData.cellCountX;
            _cellCountX = config.MyInitializationData.cellCountY;
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
