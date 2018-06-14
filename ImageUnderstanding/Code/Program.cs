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
using System.Xml.Linq;
using System.Xml.XPath;
using System.IO;
using ImageUnderstanding.FeatureGenerator;

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

            List<string> restrictTo = new List<string>() { "camera", "cannon", "brontosaurus", "ibis", "inline_skate" };
            List<string> ignoreTags = new List<string>() { "BACKGROUND_Google" };

            // get all images
            List<TaggedImage> images = new List<TaggedImage>();
            Dictionary<string, int> tagIndices = new Dictionary<string, int>();

            foreach (string folderPath in Directory.GetDirectories(path))
            {
                foreach (string imagePath in Directory.GetFiles(folderPath))
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

            Console.WriteLine("starting feature vector generation");

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            // FEATURE GENERATOR                                                                              //
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            FeatureGenerator<TaggedImage, float> featureGenerator = new HOGFeatureGenerator(10, 10);

            string tag = "";
            foreach (TaggedImage image in images)
            {
                if (tag != image.Tag)
                {
                    Console.WriteLine("generating feature vector: [" + image.Tag + "]");
                    tag = image.Tag;
                }
                image.FeatureVector = featureGenerator.GetFeatureVector(image);
            }

            featureGenerator.Dispose();

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            // FOLD ORGANIZER                                                                                 //
            ////////////////////////////////////////////////////////////////////////////////////////////////////
            FoldOrganizer<TaggedImage, string> foldOrganizer = new FoldOrganizer<TaggedImage, string>(images, foldCount, testFoldCount);

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            // CONFUSION MATRIX                                                                               //
            ////////////////////////////////////////////////////////////////////////////////////////////////////
            Mat confusionMatrix = new Mat(tagIndices.Count, tagIndices.Count, DepthType.Cv32F, 1);

            // initialize with zeros
            for (int y = 0; y < confusionMatrix.Rows; ++y)
            {
                for (int x = 0; x < confusionMatrix.Cols; ++x)
                {
                    confusionMatrix.SetValue(x, y, 0F);
                }
            }

            for (int iteration = 0; iteration < foldCount; ++iteration)
            {
                Console.WriteLine("\ncurrent iteration: " + iteration);

                Console.WriteLine("train classifier (" + iteration + ")");

                ////////////////////////////////////////////////////////////////////////////////////////////////////
                // CLASSIFIER                                                                                     //
                ////////////////////////////////////////////////////////////////////////////////////////////////////
                Classifier.Classifier<TaggedImage, string, float> classifier = new Classifier.KNearestClassifier(8);

                classifier.Train(foldOrganizer.GetTrainingData(iteration));

                // evaluate Test set
                Console.WriteLine("testing (" + iteration + ")");

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

                classifier.Dispose();

                foreach (KeyValuePair<string, int> tagIndexPair in tagIndices)
                {
                    float accuracy = confusionMatrix.GetValue(tagIndexPair.Value, tagIndexPair.Value);
                    Console.WriteLine("accuracy = " + accuracy + ",\t " + tagIndexPair.Key);
                }
            }

            float totalAccuracy = 0F;
            foreach (KeyValuePair<string, int> tagIndexPair in tagIndices)
            {
                totalAccuracy += confusionMatrix.GetValue(tagIndexPair.Value, tagIndexPair.Value) / tagIndices.Count;
            }
            Console.WriteLine("total accuracy = " + totalAccuracy);


            for (int x = 0; x < tagIndices.Count; ++x)
            {
                for (int y = 0; y < tagIndices.Count; ++y)
                {
                    confusionMatrix.SetValue(x, y, (float)Math.Sqrt(Math.Sqrt(confusionMatrix.GetValue(x, y))));
                }
            }

            // perform Tests
            string s;
            Test.Test t = new Test.FoldOrganizer_Test();
            t.PerformTest(out s);
            Console.WriteLine(s);

            //TODO: visualize accuracy
            {

                String win1 = "Confusion Matrix"; //The name of the window
                CvInvoke.NamedWindow(win1); //Create the window using the specific name

                int confusionMatrixScale = 12;

                Mat scaledConfusionMatrix = new Mat(confusionMatrix.Cols * confusionMatrixScale, confusionMatrix.Rows * confusionMatrixScale, confusionMatrix.Depth, confusionMatrix.NumberOfChannels);

                for(int y = 0; y < scaledConfusionMatrix.Rows; ++y)
                {
                    for(int x = 0; x < scaledConfusionMatrix.Cols; ++x)
                    {
                        scaledConfusionMatrix.SetValue(x, y, (float)confusionMatrix.GetValue(x / confusionMatrixScale, y / confusionMatrixScale));
                    }
                }

                CvInvoke.Imshow(win1, scaledConfusionMatrix); //Show the image
                CvInvoke.WaitKey(0);  //Wait for the key pressing event
                CvInvoke.DestroyWindow(win1); //Destroy the window if key is pressed
            }
        }
    }
}
