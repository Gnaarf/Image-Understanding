using System;
using System.Collections.Generic;

using ImageUnderstanding;
using ImageUnderstanding.DataSet;

namespace Test
{
    public class FoldOrganizer_Test : Test
    {
        struct TagName : Taggable<char>
        {
            public char Tag { get { return _tag; } }

            char _tag;
            public string Name;

            public TagName(string name)
            {
                _tag = name[0];
                Name = name;
            }
        }

        public override bool PerformTest(out string message)
        {
            message = "";

            // generate test set
            Dictionary<char, int> letterCountBindings = new Dictionary<char, int>();
            letterCountBindings['K'] = 9;
            letterCountBindings['L'] = 3;
            letterCountBindings['M'] = 10;
            letterCountBindings['O'] = 40;
            letterCountBindings['P'] = 7;

            List<TagName> tagNames = new List<TagName>();

            foreach(KeyValuePair<char, int> letterCountBinding in letterCountBindings)
            {
                for (int i = 0; i < letterCountBinding.Value; ++i)
                {
                    tagNames.Add(new TagName(letterCountBinding.Key + " " + i));
                }
            }

            // initialize FoldOrganizer
            int foldCount = 3;
            int validationFoldCount = 1;
            
            FoldOrganizer<TagName, char> foldOrganizer = new FoldOrganizer<TagName, char>(tagNames, foldCount, validationFoldCount);


            // run test cases
            int targetSampleCount = 0;
            foreach (KeyValuePair<char, int> letterCountBinding in letterCountBindings)
            {
                targetSampleCount += letterCountBinding.Value;
            }

            if(targetSampleCount != foldOrganizer.TotalDataSampleCount)
            {
                message = "FoldOrganizer claims to have total Sample Count of " + foldOrganizer.TotalDataSampleCount + ". But should be " + targetSampleCount;

                return false;
            }

            for (int iteration = 0; iteration < foldCount; ++iteration)
            {
                List<TagName> learningData = foldOrganizer.GetLearningData(iteration);
                List<TagName> validationData = foldOrganizer.GetValidationData(iteration);

                foreach (TagName tagName in tagNames)
                {
                    if (!(learningData.Contains(tagName) ^ validationData.Contains(tagName)))
                    {
                        if (learningData.Contains(tagName) && validationData.Contains(tagName))
                        {
                            message = tagName.Name + " is contained in learning Set AND in validation Set.";
                        }
                        else
                        {
                            message = tagName.Name + " is contained NEITHER in learning Set NOR in validation Set.";
                        }

                        return false;
                    }
                }
                /*
                foreach(KeyValuePair<char, int> letterCountBinding in letterCountBindings)
                {
                    int minLearningSize = letterCountBinding.Value / foldOrganizer.FoldCount * foldOrganizer.LearningFoldCount;
                    int minValidationSize = letterCountBinding.Value / foldOrganizer.FoldCount * foldOrganizer.ValidationFoldCount;

                    if (learningData.FindAll(item => item.Tag == letterCountBinding.Key).Count < minLearningSize)
                    {
                        message = "Learning Data Set contains only " + learningData.FindAll(item => item.Tag == letterCountBinding.Key).Count + ". Should at least contain " + minLearningSize;

                        return false;
                    }

                    if (validationData.FindAll(item => item.Tag == letterCountBinding.Key).Count < minValidationSize)
                    {
                        message = "Validation Data Set contains only " + validationData.FindAll(item => item.Tag == letterCountBinding.Key).Count + ". Should at least contain " + minValidationSize;

                        return false;
                    }
                }*/
            }

            return true;
        }
    }
}
