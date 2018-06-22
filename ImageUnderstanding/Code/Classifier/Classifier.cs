using System;
using System.Collections.Generic;
using System.Text;

namespace ImageUnderstanding.Classifier
{
    public abstract class Classifier<T, TagT, FeatureT> where T : Taggable<TagT>, FeatureHolder<FeatureT>
    {
        public abstract void InitializeViaConfig(ImageUnderstandingConfig config);

        /// <summary>
        /// initialize and train the classifier. 
        /// </summary>
        /// <param name="trainingDataSet">Data to train the classifier with</param>
        public abstract void Train(List<T> trainingDataSet);

        /// <summary>
        /// call only after Train()! Evaluates the given dataSample based on the trained data
        /// </summary>
        /// <param name="dataSample">dataSample to be classified</param>
        /// <returns>evaluted tag of dataSample</returns>
        public abstract TagT Evaluate(T dataSample);

        /// <summary>
        /// dispose of memory leaks if need be
        /// </summary>
        public abstract void Dispose();
    }
}
