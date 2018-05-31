using System.Collections.Generic;

namespace ImageUnderstanding.DataSet
{
    public class FoldOrganizer<Datatype, TagDatatype> where Datatype : Taggable<TagDatatype>
    {

        Dictionary<TagDatatype, List<Datatype>> Data;
        public int FoldCount { get; private set; }
        public int TestFoldCount { get; private set; }
        public int TrainingFoldCount { get { return FoldCount - TestFoldCount; } }

        public int TotalDataSampleCount { get; private set; }

        public FoldOrganizer(List<Datatype> data, int foldCount, int testFoldCount)
        {
            // remember some important meta data
            ChangeFoldCount(foldCount, testFoldCount);

            // fill into own dataformat
            Data = new Dictionary<TagDatatype, List<Datatype>>();

            AddDataSamples(data.ToArray());
        }

        public void AddDataSamples(params Datatype[] dataSamples)
        {
            foreach (Datatype data in dataSamples)
            {
                TagDatatype tag = data.Tag;

                if (!Data.ContainsKey(tag))
                {
                    Data[tag] = new List<Datatype>();
                }

                Data[tag].Add(data);
            }

            TotalDataSampleCount += dataSamples.Length;

            foreach(TagDatatype tagType in Data.Keys)
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

        public List<Datatype> GetTestData(int iteration)
        {
            if (iteration >= FoldCount)
            {
                throw new System.IndexOutOfRangeException("Can't get the " + iteration + "th iteration on a " + FoldCount + "-fold Data Set");
            }

            int estimatedValdationDataCount = (int)(TestFoldCount / (float)FoldCount) * TotalDataSampleCount;
            List<Datatype> testData = new List<Datatype>(estimatedValdationDataCount);

            foreach (List<Datatype> samplesWithSingleTag in Data.Values)
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

        public List<Datatype> GetTrainingData(int iteration)
        {
            if (iteration >= FoldCount)
            {
                throw new System.IndexOutOfRangeException("Can't get the " + iteration + "th iteration on a " + FoldCount + "-fold Data Set");
            }

            int estimatedTrainingDataCount = (int)(TestFoldCount / (float)FoldCount) * TotalDataSampleCount;
            List<Datatype> trainingData = new List<Datatype>(estimatedTrainingDataCount);
            
            foreach (List<Datatype> samplesWithSingleTag in Data.Values)
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

        public int GetTestDataCount(TagDatatype tag, int iteration = 0)
        {
            return Data[tag].Count * TestFoldCount / FoldCount;
        }

        public int GetTrainingDataCount(TagDatatype tag, int iteration = 0)
        {
            return Data[tag].Count * TrainingFoldCount / FoldCount;
        }

        public int GetTotalDataCount(TagDatatype tag)
        {
            return Data[tag].Count;
        }
    }
}
