using System.Collections.Generic;

namespace ImageUnderstanding.DataSet
{
    public class FoldOrganizer<Datatype, TagDatatype> where Datatype : Taggable<TagDatatype>
    {

        Dictionary<TagDatatype, List<Datatype>> Data;
        public int FoldCount { get; private set; }
        public int ValidationFoldCount { get; private set; }
        public int LearningFoldCount { get { return FoldCount - ValidationFoldCount; } }

        public int TotalDataSampleCount { get; private set; }

        public FoldOrganizer(List<Datatype> data, int foldCount, int validationFoldCount)
        {
            // remember some important meta data
            ChangeFoldCount(foldCount, validationFoldCount);

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
                    throw new System.Exception("Sample Size too small. Only " + Data[tagType].Count + " instances of <" + tagType + ">. Can't perform " + FoldCount + "-fold validation.");
                }
            }
        }

        public void ChangeFoldCount(int foldCount, int validationFoldCount)
        {
            FoldCount = foldCount;
            ValidationFoldCount = validationFoldCount;
        }

        public List<Datatype> GetValidationData(int iteration)
        {
            if (iteration >= FoldCount)
            {
                throw new System.IndexOutOfRangeException("Can't get the " + iteration + "th iteration on a " + FoldCount + "-fold Data Set");
            }

            int estimatedValdationDataCount = (int)(ValidationFoldCount / (float)FoldCount) * TotalDataSampleCount;
            List<Datatype> validationData = new List<Datatype>(estimatedValdationDataCount);

            foreach (List<Datatype> samplesWithSingleTag in Data.Values)
            {
                int foldSize = samplesWithSingleTag.Count / FoldCount;

                int startIndex = iteration * foldSize;
                int aboveEndIndex = ((iteration + ValidationFoldCount) >= FoldCount) ? samplesWithSingleTag.Count : (iteration + ValidationFoldCount) * foldSize;

                for (int i = startIndex; i < aboveEndIndex; ++i)
                {
                    validationData.Add(samplesWithSingleTag[i]);
                }
            }

            return validationData;
        }

        public List<Datatype> GetLearningData(int iteration)
        {
            if (iteration >= FoldCount)
            {
                throw new System.IndexOutOfRangeException("Can't get the " + iteration + "th iteration on a " + FoldCount + "-fold Data Set");
            }

            int estimatedLearningDataCount = (int)(ValidationFoldCount / (float)FoldCount) * TotalDataSampleCount;
            List<Datatype> learningData = new List<Datatype>(estimatedLearningDataCount);
            
            foreach (List<Datatype> samplesWithSingleTag in Data.Values)
            {
                int foldSize = samplesWithSingleTag.Count / FoldCount;

                int skipStartIndex = iteration * foldSize;
                int skipAboveEndIndex = ((iteration + ValidationFoldCount) >= FoldCount) ? samplesWithSingleTag.Count : (iteration + ValidationFoldCount) * foldSize;

                for (int i = 0; i < samplesWithSingleTag.Count; ++i)
                {
                    if (i == skipStartIndex)
                    {
                        i = skipAboveEndIndex - 1;
                        continue;
                    }

                    learningData.Add(samplesWithSingleTag[i]);
                }
            }

            return learningData;
        }

    }

}
