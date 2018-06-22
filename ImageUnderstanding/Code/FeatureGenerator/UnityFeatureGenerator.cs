using Emgu.CV;
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV.XFeatures2D;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImageUnderstanding.FeatureGenerator
{
    [Serializable]
    public class UnityFeatureGeneratorInitializationData
    {
        public int featureCount = 50;
    }

    public class UnityFeatureGenerator : FeatureGenerator<TaggedImage, float>
    {
        int _featureCount;

        static Random rand = new Random();

        public UnityFeatureGenerator()
            : this(new UnityFeatureGeneratorInitializationData())
        { }

        public UnityFeatureGenerator(UnityFeatureGeneratorInitializationData initializationData)
        {
            _featureCount = initializationData.featureCount;
        }

        public override void InitializeViaConfig(ImageUnderstandingConfig config)
        {
            _featureCount = config.UnityInitializationData.featureCount;
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
                res[i] = 1F;
            }

            return res;
        }
    }
}
