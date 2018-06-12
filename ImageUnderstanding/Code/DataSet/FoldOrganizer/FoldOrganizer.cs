using System.Collections.Generic;

namespace ImageUnderstanding.DataSet
{
    public class FoldOrganizer<T, TagT> where T : Taggable<TagT>
    {

        Dictionary<TagT, List<T>> Data;
        public int FoldCount { get; private set; }
        public int TestFoldCount { get; private set; }
        public int TrainingFoldCount { get { return FoldCount - TestFoldCount; } }

        public int TotalDataSampleCount { get; private set; }

        public FoldOrganizer(List<T> data, int foldCount, int testFoldCount)
        {
            // remember some important meta data
            ChangeFoldCount(foldCount, testFoldCount);

            // fill into own dataformat
            Data = new Dictionary<TagT, List<T>>();

            AddDataSamples(data.ToArray());
        }

        public void AddDataSamples(params T[] dataSamples)
        {
            foreach (T data in dataSamples)
            {
                TagT tag = data.Tag;

                if (!Data.ContainsKey(tag))
                {
                    Data[tag] = new List<T>();
                }

                Data[tag].Add(data);
            }

            TotalDataSampleCount += dataSamples.Length;

            foreach(TagT tagType in Data.Keys)
            {
                if(Data[tagType].Count < FoldCount)
                {
                    throw new System.Exception("Sample Size too small. Only " + Data[tagType].Count + " instances of <" + tagType + ">. Can't perform " + FoldCount + "-fold test.");
                }
            }
        }

        public void ChangeFoldCount(int foldCount, int testFoldCount)
        {
            FoldCount = foldCount;
            TestFoldCount = testFoldCount;
        }

        public List<T> GetTestData(int iteration)
        {
            if (iteration >= FoldCount)
            {
                throw new System.IndexOutOfRangeException("Can't get the " + iteration + "th iteration on a " + FoldCount + "-fold Data Set");
            }

            int estimatedValdationDataCount = (int)(TestFoldCount / (float)FoldCount) * TotalDataSampleCount;
            List<T> testData = new List<T>(estimatedValdationDataCount);

            foreach (List<T> samplesWithSingleTag in Data.Values)
            {
                int foldSize = samplesWithSingleTag.Count / FoldCount;

                int startIndex = iteration * foldSize;
                int aboveEndIndex = ((iteration + TestFoldCount) >= FoldCount) ? samplesWithSingleTag.Count : (iteration + TestFoldCount) * foldSize;

                for (int i = startIndex; i < aboveEndIndex; ++i)
                {
                    testData.Add(samplesWithSingleTag[i]);
                }
            }

            return testData;
        }

        public List<T> GetTrainingData(int iteration)
        {
            if (iteration >= FoldCount)
            {
                throw new System.IndexOutOfRangeException("Can't get the " + iteration + "th iteration on a " + FoldCount + "-fold Data Set");
            }

            int estimatedTrainingDataCount = (int)(TestFoldCount / (float)FoldCount) * TotalDataSampleCount;
            List<T> trainingData = new List<T>(estimatedTrainingDataCount);
            
            foreach (List<T> samplesWithSingleTag in Data.Values)
            {
                int foldSize = samplesWithSingleTag.Count / FoldCount;

                int skipStartIndex = iteration * foldSize;
                int skipAboveEndIndex = ((iteration + TestFoldCount) >= FoldCount) ? samplesWithSingleTag.Count : (iteration + TestFoldCount) * foldSize;

                for (int i = 0; i < samplesWithSingleTag.Count; ++i)
                {
                    if (i == skipStartIndex)
                    {
                        i = skipAboveEndIndex - 1;
                        continue;
                    }

                    trainingData.Add(samplesWithSingleTag[i]);
                }
            }

            return trainingData;
        }

        public int GetTestDataCount(TagT tag, int iteration = 0)
        {
            return Data[tag].Count * TestFoldCount / FoldCount;
        }

        public int GetTrainingDataCount(TagT tag, int iteration = 0)
        {
            return Data[tag].Count * TrainingFoldCount / FoldCount;
        }

        public int GetTotalDataCount(TagT tag)
        {
            return Data[tag].Count;
        }
    }
}
