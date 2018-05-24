using System;
using System.Collections.Generic;

namespace ImageUnderstanding.Classifier
{
    public class RandomClassifier<Datatype, TagDatatype> : Classifier<Datatype, TagDatatype> where Datatype : Taggable<TagDatatype>
    {
        List<TagDatatype> tags = new List<TagDatatype>();

        static Random rand = new Random();

        public override void Train(List<Datatype> trainingDataSet)
        {
            HashSet<TagDatatype> hashSet = new HashSet<TagDatatype>();

            foreach(Datatype dataSample in trainingDataSet)
            {
                if(!hashSet.Contains(dataSample.Tag))
                {
                    hashSet.Add(dataSample.Tag);
                }
            }

            tags = new List<TagDatatype>(hashSet);

            return;
        }

        public override TagDatatype Evaluate(Datatype dataSample)
        {
            return tags[rand.Next(0, tags.Count)];
        }
    }
}
