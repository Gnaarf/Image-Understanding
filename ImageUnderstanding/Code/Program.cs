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
            string path = "../../../../../caltech101/";
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
            
            Mat confusionMatrix = new Mat(tagIndices.Count, tagIndices.Count, DepthType.Cv8U, 1); //Create a 3 channel image of 400x200

            for(int iteration = 0; iteration < foldCount; ++iteration)
            {
                // TODO: get training set


                // TODO: foreach Image: generate feature vector
                //          SIFT
                //          HOG
                //          CoOccurrence Features
                //          Spectral Features

                // TODO: train classifier
                Classifier.Classifier<TaggedImage, string> classifier = new Classifier.RandomClassifier<TaggedImage, string>();

                classifier.Train(foldOrganizer.GetLearningData(iteration));

                // TODO: evaluate Validation set
                List<TaggedImage> validationSet = foldOrganizer.GetValidationData(iteration);

                foreach (TaggedImage validationDataSample in validationSet)
                {
                    string tag = classifier.Evaluate(validationDataSample);

                    int indexOfRealTag = tagIndices[validationDataSample.Tag];
                    int indexOfEvaluatedTag = tagIndices[tag];

                    byte value = confusionMatrix.GetValue(indexOfRealTag, indexOfEvaluatedTag);
                    value++;
                    confusionMatrix.SetValue(indexOfRealTag, indexOfEvaluatedTag, value); 
                }
            }



            // TODO: visualize accuracy
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
