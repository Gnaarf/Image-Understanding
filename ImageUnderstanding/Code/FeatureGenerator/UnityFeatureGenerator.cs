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
    public class UnityFeatureGenerator : FeatureGenerator<TaggedImage, float>
    {
        int _featureCount;

        static Random rand = new Random();

        public UnityFeatureGenerator(int featureCount = 50)
        {
            _featureCount = featureCount;
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
