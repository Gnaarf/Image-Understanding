using System;
using System.Collections.Generic;

namespace ImageUnderstanding.Classifier
{
    [Serializable]
    public class SingleResultClassifierInitializationData
    {
        public string resultTag = "accordion";
    }

    public class SingleResultClassifier : Classifier<TaggedImage, string, float>
    {
        string _resultTag;

        public SingleResultClassifier()
            : this (new SingleResultClassifierInitializationData())
        { }

        public SingleResultClassifier(SingleResultClassifierInitializationData initializationData)
        {
            _resultTag = initializationData.resultTag;
        }

        public override void InitializeViaConfig(ImageUnderstandingConfig config)
        {
            _resultTag = config.singleResultInitializationData.resultTag;
        }

        public override void Train(List<TaggedImage> trainingDataSet)
        {
            return;
        }

        public override string Evaluate(TaggedImage dataSample)
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
