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
            int validationFoldCount = 1;

            // get all images
            List<TaggedImage> images = new List<TaggedImage>();
            Dictionary<string, int> tagIndices = new Dictionary<string, int>();
            
            foreach (string folderPath in System.IO.Directory.GetDirectories(path))
            {
                foreach (string imagePath in System.IO.Directory.GetFiles(folderPath))
                {
                    TaggedImage image = new TaggedImage(imagePath);
                    images.Add(image);
                    if(!tagIndices.ContainsKey(image.Tag))
                    {
                        tagIndices.Add(image.Tag, tagIndices.Count);
                    }
                }
            }

            // TODO: fill in all images into FoldOragnizer
            FoldOrganizer<TaggedImage, string> foldOrganizer = new FoldOrganizer<TaggedImage, string>(images, foldCount, validationFoldCount);
            
            Mat confusionMatrix = new Mat(tagIndices.Count, tagIndices.Count, DepthType.Cv32F, 1); //Create a 3 channel image of 400x200

            for(int iteration = 0; iteration < foldCount; ++iteration)
            {
                // TODO: get training set


                // TODO: foreach Image: generate feature vector
                //          SIFT
                //          HOG
                //          CoOccurrence Features
                //          Spectral Features

                // TODO: train classifier
                Classifier.Classifier<TaggedImage, string> classifier = new Classifier.MyClassifier();

                classifier.Train(foldOrganizer.GetLearningData(iteration));

                // TODO: evaluate Validation set
                List<TaggedImage> validationSet = foldOrganizer.GetValidationData(iteration);

                foreach (TaggedImage validationDataSample in validationSet)
                {
                    string evaluatedTag = classifier.Evaluate(validationDataSample);

                    int indexOfRealTag = tagIndices[validationDataSample.Tag];
                    int indexOfEvaluatedTag = tagIndices[evaluatedTag];

                    float value = confusionMatrix.GetValue(indexOfRealTag, indexOfEvaluatedTag);
                    value += 1F / (float)foldOrganizer.GetTotalDataCount(validationDataSample.Tag);

                    confusionMatrix.SetValue(indexOfRealTag, indexOfEvaluatedTag, value);
                }
            }

            for(int x = 0; x < tagIndices.Count; ++x)
            {
                for(int y = 0; y < tagIndices.Count; ++y)
                {
                    confusionMatrix.SetValue(x, y, (float)Math.Sqrt(Math.Sqrt(confusionMatrix.GetValue(x, y))));
                }
            }

            // TODO: visualize accuracy
            {

                //String win1 = "Confusion Matrix"; //The name of the window
                //CvInvoke.NamedWindow(win1); //Create the window using the specific name

                //CvInvoke.Imshow(win1, confusionMatrix); //Show the image
                //CvInvoke.WaitKey(0);  //Wait for the key pressing event
                //CvInvoke.DestroyWindow(win1); //Destroy the window if key is pressed
            }
        }
    }
}
