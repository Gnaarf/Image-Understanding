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
using System.Runtime.Serialization.Json;
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
            // For that you will need to add reference to System.Runtime.Serialization
            var jsonReader = JsonReaderWriterFactory.CreateJsonReader(Encoding.UTF8.GetBytes(@"{ ""Name"": ""Jon Smith"", ""Address"": { ""City"": ""New York"", ""State"": ""NY"" }, ""Age"": 42 }"), new System.Xml.XmlDictionaryReaderQuotas());

            // For that you will need to add reference to System.Xml and System.Xml.Linq
            var root = XElement.Load(jsonReader);
            Console.WriteLine(root.XPathSelectElement("//Name").Value);
            Console.WriteLine(root.XPathSelectElement("//Address/State").Value);

            File.Create("..\\netcoreapp2.0\\hello.txt");

            //// For that you will need to add reference to System.Web.Helpers
            //dynamic json = System.Web.Helpers.Json.Decode(@"{ ""Name"": ""Jon Smith"", ""Address"": { ""City"": ""New York"", ""State"": ""NY"" }, ""Age"": 42 }");
            //Console.WriteLine(json.Name);
            //Console.WriteLine(json.Address.State);

            //string name = stuff.Name;
            //string address = stuff.Address.City;

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

            // Generate Feature Vector for every image

            Console.WriteLine("starting feature vector generation");

            FeatureGenerator<TaggedImage, float> featureGenerator = new MyFeatureGenerator();

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

            // fill all images into FoldOragnizer
            FoldOrganizer<TaggedImage, string> foldOrganizer = new FoldOrganizer<TaggedImage, string>(images, foldCount, testFoldCount);

            Mat confusionMatrix = new Mat(tagIndices.Count, tagIndices.Count, DepthType.Cv32F, 1); //Create a 3 channel image of 400x200

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

                // train classifier
                Console.WriteLine("train classifier (" + iteration + ")");

                //Classifier.Classifier<TaggedImage, string, float> classifier = new Classifier.KNearestClassifier();
                Classifier.Classifier<TaggedImage, string, float> classifier = new Classifier.SingleResultClassifier<TaggedImage, string, float>("cannon");

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

                CvInvoke.Imshow(win1, confusionMatrix); //Show the image
                CvInvoke.WaitKey(0);  //Wait for the key pressing event
                CvInvoke.DestroyWindow(win1); //Destroy the window if key is pressed
            }
        }
    }
}
