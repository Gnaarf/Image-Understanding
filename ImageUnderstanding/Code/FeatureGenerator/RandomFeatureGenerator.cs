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
    [Serializable]
    public class RandomFeatureGeneratorInitializationData
    {
        public int featureCount = 50;
    }


    public class RandomFeatureGenerator : FeatureGenerator<TaggedImage, float>
    {
        int _featureCount;

        static Random rand = new Random();

        public RandomFeatureGenerator()
            : this(new RandomFeatureGeneratorInitializationData())
        { }

        public RandomFeatureGenerator(RandomFeatureGeneratorInitializationData initialzationData)
        {
            _featureCount = initialzationData.featureCount;
        }

        public override void InitializeViaConfig(ImageUnderstandingConfig config)
        {
            _featureCount = config.RandomFGInitializationData.featureCount;
        }

        public override void Dispose()
        {
            // nothing to do here
            return;
        }

        public override List<float> GetFeatureVector(TaggedImage data)
        {
            List<float> res = new List<float>(_featureCount);
            
            for(int i = 0; i < res.Count; ++i)
            {
                res[i] = (float)rand.NextDouble();
            }

            return res;
        }
    }
}
