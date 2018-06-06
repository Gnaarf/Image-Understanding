using System;
using System.Collections.Generic;

namespace ImageUnderstanding.Classifier
{
    public class SingleResultClassifier<Datatype, TagDatatype, FeatureT> : Classifier<Datatype, TagDatatype, FeatureT> where Datatype : Taggable<TagDatatype>, FeatureHolder<FeatureT>
    {
        TagDatatype _resultTag;

        public SingleResultClassifier(TagDatatype resultTag)
        {
            _resultTag = resultTag;
        }

        public override void Train(List<Datatype> trainingDataSet)
        {
            return;
        }

        public override TagDatatype Evaluate(Datatype dataSample)
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
