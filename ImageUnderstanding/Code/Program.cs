using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Emgu.CV;
using Emgu.Util;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

using ImageUnderstanding;
using ImageUnderstanding.DataSet;

namespace ImageUnderstanding
{
    class Program
    {
        static void Main(string[] args)
        {
            // some parameters
            string path = MachineDepententConstants.caltech101Path;
            int foldCount = 10;
            int testFoldCount = 1;

            List<string> restrictTo = new List<string>();// { "camera", "cannon", "brontosaurus", "ibis", "inline_skate" };
            List<string> ignoreTags = new List<string>() { "BACKGROUND_Google" };

            // get all images
            List<TaggedImage> images = new List<TaggedImage>();
            Dictionary<string, int> tagIndices = new Dictionary<string, int>();
            
            foreach (string folderPath in System.IO.Directory.GetDirectories(path))
            {
                foreach (string imagePath in System.IO.Directory.GetFiles(folderPath))
                {
                    TaggedImage image = new TaggedImage(imagePath);

                    if (ignoreTags.Contains(image.Tag) || (restrictTo.Count != 0 && !restrictTo.Contains(image.Tag)))
                    {
                        continue;
                    }

                    images.Add(image);

                    if (!tagIndices.ContainsKey(image.Tag))
                    {
                        tagIndices.Add(image.Tag, tagIndices.Count);
                    }

                }
            }

            // fill in all images into FoldOragnizer
            FoldOrganizer<TaggedImage, string> foldOrganizer = new FoldOrganizer<TaggedImage, string>(images, foldCount, testFoldCount);
            
            Mat confusionMatrix = new Mat(tagIndices.Count, tagIndices.Count, DepthType.Cv32F, 1); //Create a 3 channel image of 400x200

            for(int iteration = 0; iteration < foldCount; ++iteration)
            {
                Console.WriteLine("current iteration: " + iteration);

                // train classifier
                Console.WriteLine("start training");

                Classifier.Classifier<TaggedImage, string> classifier = new Classifier.MyClassifier();

                classifier.Train(foldOrganizer.GetTrainingData(iteration));

                // evaluate Test set
                Console.WriteLine("start testing");

                List<TaggedImage> testSet = foldOrganizer.GetTestData(iteration);

                foreach (TaggedImage testDataSample in testSet)
                {
                    string evaluatedTag = classifier.Evaluate(testDataSample);

                    int indexOfRealTag = tagIndices[testDataSample.Tag];
                    int indexOfEvaluatedTag = tagIndices[evaluatedTag];

                    float value = confusionMatrix.GetValue(indexOfRealTag, indexOfEvaluatedTag);
                    value += 1F / (float)foldOrganizer.GetTotalDataCount(testDataSample.Tag);

                    confusionMatrix.SetValue(indexOfRealTag, indexOfEvaluatedTag, value);
                }

                if (classifier is Classifier.MyClassifier)
                    (classifier as Classifier.MyClassifier).Dispose();
                
                foreach (KeyValuePair<string, int> tagIndexPair in tagIndices)
                {
                    float accuracy = confusionMatrix.GetValue(tagIndexPair.Value, tagIndexPair.Value);
                    Console.WriteLine("accuracy = " + accuracy + ",\t " + tagIndexPair.Key);
                }
            }

            float totalAccuracy = 0F;
            foreach(KeyValuePair<string, int> tagIndexPair in tagIndices)
            {
                totalAccuracy += confusionMatrix.GetValue(tagIndexPair.Value, tagIndexPair.Value) / tagIndices.Count;
            }
            Console.WriteLine("total accuracy = " + totalAccuracy);


            for (int x = 0; x < tagIndices.Count; ++x)
            {
                for(int y = 0; y < tagIndices.Count; ++y)
                {
                    confusionMatrix.SetValue(x, y, (float)Math.Sqrt(Math.Sqrt(confusionMatrix.GetValue(x, y))));
                }
            }

            //TODO: visualize accuracy
            {

                String win1 = "Confusion Matrix"; //The name of the window
                CvInvoke.NamedWindow(win1); //Create the window using the specific name

                CvInvoke.Imshow(win1, confusionMatrix); //Show the image
                CvInvoke.WaitKey(0);  //Wait for the key pressing event
                CvInvoke.DestroyWindow(win1); //Destroy the window if key is pressed
            }
        }
    }
}
