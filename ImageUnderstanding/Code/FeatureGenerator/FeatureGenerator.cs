using System;
using System.Collections.Generic;
using System.Text;

namespace ImageUnderstanding.FeatureGenerator
{
    [Serializable]
    public abstract class FeatureGenerator<Datatype, FeatureType>
    {
        public abstract void InitializeViaConfig(ImageUnderstandingConfig config);

        public abstract List<FeatureType> GetFeatureVector(Datatype data);

        /// <summary>
        /// dispose of memory leaks if need be
        /// </summary>
        public abstract void Dispose();
    }
}
