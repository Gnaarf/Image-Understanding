using System;
using System.Collections.Generic;
using System.Text;

namespace ImageUnderstanding.Classifier
{
    public abstract class Classifier<Datatype, TagDatatype> where Datatype : Taggable<TagDatatype>
    {
        /// <summary>
        /// initialize and train the classifier. 
        /// </summary>
        /// <param name="trainingDataSet">Data to train the classifier with</param>
        public abstract void Train(List<Datatype> trainingDataSet);

        /// <summary>
        /// call only after Train()! Evaluates the given dataSample based on the trained data
        /// </summary>
        /// <param name="dataSample">dataSample to be classified</param>
        /// <returns>evaluted tag of dataSample</returns>
        public abstract TagDatatype Evaluate(Datatype dataSample);
    }
}
