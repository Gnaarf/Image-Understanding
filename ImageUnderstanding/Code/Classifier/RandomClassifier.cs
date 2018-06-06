using System;
using System.Collections.Generic;

namespace ImageUnderstanding.Classifier
{
    public class RandomClassifier<T, TagT, FeatureT> : Classifier<T, TagT, FeatureT> where T : Taggable<TagT>, FeatureHolder<FeatureT>
    {
        List<TagT> tags = new List<TagT>();

        static Random rand = new Random();

        public override void Train(List<T> trainingDataSet)
        {
            HashSet<TagT> hashSet = new HashSet<TagT>();

            foreach(T dataSample in trainingDataSet)
            {
                if(!hashSet.Contains(dataSample.Tag))
                {
                    hashSet.Add(dataSample.Tag);
                }
            }

            tags = new List<TagT>(hashSet);

            return;
        }

        public override TagT Evaluate(T dataSample)
        {
            return tags[rand.Next(0, tags.Count)];
        }

        public override void Dispose()
        {
            // nothing to dispose of
            return;
        }
    }
}
