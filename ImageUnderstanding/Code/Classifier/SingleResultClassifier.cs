using System;
using System.Collections.Generic;

namespace ImageUnderstanding.Classifier
{
    public class SingleResultClassifier<Datatype, TagDatatype> : Classifier<Datatype, TagDatatype> where Datatype : Taggable<TagDatatype>
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
    }
}
