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
using ImageUnderstanding.Classifier;

namespace ImageUnderstanding
{
    public class Program
    {
        static void Main(string[] args)
        {
            ImageUnderstandingConfig config = ImageUnderstandingConfig.ImportSettings();

            Console.WriteLine("Feature Generation: [" + config.featureGeneratorMethod + "]");
            Console.WriteLine("Classification:     [" + config.classifierMethod + "]\n");
            Console.WriteLine("SoftNormalization:  [" + config.UseSoftNormalization + "]\n");

            // some parameters
            string path = config.path;
            int foldCount = config.FoldCount;
            int testFoldCount = config.TestFoldCount;

            string[] restrictTo = config.RestrictTo;
            string[] ignoreTags = config.IgnoreTags;

            // get all images
            List<TaggedImage> images = new List<TaggedImage>();
            Dictionary<string, int> tagIndices = new Dictionary<string, int>();

            foreach (string folderPath in Directory.GetDirectories(path))
            {
                foreach (string imagePath in Directory.GetFiles(folderPath))
                {
                    TaggedImage image = new TaggedImage(imagePath);

                    if (ignoreTags.Contains(image.Tag) || (restrictTo.Length != 0 && !restrictTo.Contains(image.Tag)))
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

            Console.WriteLine("starting feature vector generation [" + config.featureGeneratorMethod + "]\n");

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            // FEATURE GENERATOR                                                                              //
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            FeatureGenerator<TaggedImage, float> featureGenerator;
            switch (config.featureGeneratorMethod)
            {
                case ImageUnderstandingConfig.FeatureGeneratorMethod.HOG:
                    featureGenerator = new HOGFeatureGenerator();
                    break;

                case ImageUnderstandingConfig.FeatureGeneratorMethod.SIFT:
                    featureGenerator = new SIFTFeatureGenerator();
                    break;

                case ImageUnderstandingConfig.FeatureGeneratorMethod.Random:
                    featureGenerator = new RandomFeatureGenerator();
                    break;

                case ImageUnderstandingConfig.FeatureGeneratorMethod.Unity:
                    featureGenerator = new UnityFeatureGenerator();
                    break;

                default:
                    throw new Exception("Unknown Feature Generator Method");
            }

            featureGenerator.InitializeViaConfig(config);
            

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

            if (config.UseSoftNormalization)
            {
                SoftNormalizeFeatureVectors(images);
            }

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

            Console.WriteLine("\nstarting Classification. [" + config.classifierMethod + "]");

            for (int iteration = 0; iteration < foldCount; ++iteration)
            {
                Console.WriteLine("\ncurrent iteration: " + iteration);

                Console.WriteLine("train classifier (" + iteration + ")");

                ////////////////////////////////////////////////////////////////////////////////////////////////////
                // CLASSIFIER                                                                                     //
                ////////////////////////////////////////////////////////////////////////////////////////////////////
                Classifier.Classifier<TaggedImage, string, float> classifier;
                
                switch(config.classifierMethod)
                {
                    case ImageUnderstandingConfig.ClassifierMethod.KNearest:
                        classifier = new KNearestClassifier();
                        break;

                    case ImageUnderstandingConfig.ClassifierMethod.Random:
                        classifier = new RandomClassifier<TaggedImage, string, float>();
                        break;

                    case ImageUnderstandingConfig.ClassifierMethod.SingleResult:
                        classifier = new SingleResultClassifier();
                        break;

                    case ImageUnderstandingConfig.ClassifierMethod.SVM:
                        classifier = new SVMClassifier();
                        break;

                    default:
                        throw new Exception("Unknown Classifier Method");

                }
                classifier.InitializeViaConfig(config);

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
                    Console.WriteLine("accuracy = " + string.Format("{0,12:#.0000}%", (accuracy* 100)) + ",\t " + tagIndexPair.Key);
                    //Console.WriteLine("accuracy = " + accuracy + ",\t " + tagIndexPair.Key);
                }
            }

            float totalAccuracy = 0F;
            List<float> accuracies = new List< float>();
            foreach (KeyValuePair<string, int> tagIndexPair in tagIndices)
            {
                accuracies.Add(confusionMatrix.GetValue(tagIndexPair.Value, tagIndexPair.Value));
                totalAccuracy += confusionMatrix.GetValue(tagIndexPair.Value, tagIndexPair.Value) / tagIndices.Count;
            }
            Console.WriteLine("total accuracy = " + string.Format("{0,12:#.0000}%", (totalAccuracy * 100)));
            Console.WriteLine("stdDeviation over all total accuracies = " + accuracies.ToArray().StdDeviation());


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

            //visualize accuracy
            {
                String win1 = "Confusion Matrix"; //The name of the window
                CvInvoke.NamedWindow(win1); //Create the window using the specific name

                int confusionMatrixScale = 5;

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

        private static void SoftNormalizeFeatureVectors(List<TaggedImage> images)
        {
            for (int dimension = 0; dimension < images[0].FeatureVector.Count; ++dimension)
            {
                float[] values = new float[images.Count];

                for (int i = 0; i < images.Count; ++i)
                {
                    values[i] = images[i].FeatureVector[dimension];
                }

                float mean = values.Mean();
                float stdDev = values.StdDeviation(mean);

                foreach (TaggedImage image in images)
                {
                    image.FeatureVector[dimension] = (image.FeatureVector[dimension] - mean) * 0.5F / stdDev;
                }
            }
        }
    }
}
