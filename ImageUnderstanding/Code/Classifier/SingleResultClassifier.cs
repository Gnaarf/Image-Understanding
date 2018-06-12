using System;
using System.Collections.Generic;

namespace ImageUnderstanding.Classifier
{
    public class SingleResultClassifier<T, TagT, FeatureT> : Classifier<T, TagT, FeatureT> where T : Taggable<TagT>, FeatureHolder<FeatureT>
    {
        TagT _resultTag;

        public SingleResultClassifier(TagT resultTag)
        {
            _resultTag = resultTag;
        }

        public override void Train(List<T> trainingDataSet)
        {
            return;
        }

        public override TagT Evaluate(T dataSample)
        {
            return _resultTag;
        }

        public override void Dispose()
        {
            // nothing to dispose of
            return;
        }
    }
}
