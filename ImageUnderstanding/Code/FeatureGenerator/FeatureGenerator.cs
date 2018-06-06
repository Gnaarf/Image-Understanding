using System;
using System.Collections.Generic;
using System.Text;

namespace ImageUnderstanding.FeatureGenerator
{
    public abstract class FeatureGenerator<Datatype, FeatureType>
    {
        public abstract List<FeatureType> GetFeatureVector(Datatype data);

        public abstract void Dispose();
    }
}
