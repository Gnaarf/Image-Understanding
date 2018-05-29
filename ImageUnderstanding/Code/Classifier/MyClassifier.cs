using System;
using System.Collections.Generic;
using Emgu.CV;
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using Emgu.CV.XFeatures2D;

namespace ImageUnderstanding.Classifier
{
    public class MyClassifier : Classifier<TaggedImage, string>
    {
        SIFT sift;

        public MyClassifier()
        {
            sift = new SIFT();
        }

        public override void Train(List<TaggedImage> trainingDataSet)
        {
            foreach (TaggedImage image in trainingDataSet)
            {
                List<float> featureVector = ExtractSiftFeatureVector(image, 20, SiftSorting.Response, true);

            }

            return;
        }

        public override string Evaluate(TaggedImage dataSample)
        {
            throw new NotImplementedException();
        }

        enum SiftSorting
        {
            Response,
            Size,
            None,
        }

        List<float> ExtractSiftFeatureVector(TaggedImage image, int keyPointCount, SiftSorting sorting, bool doDrawEverySingleImage)
        {
            // use the emgu functions to gather keypoints

            Emgu.CV.Util.VectorOfKeyPoint vectorOfKeypoints = new Emgu.CV.Util.VectorOfKeyPoint();

            Mat output = image.GetMat().Clone(); // only needed for drawing

            sift.DetectAndCompute(image.GetMat(), null, vectorOfKeypoints, output, false);

            // put it into useful data formats

            List<MKeyPoint> keyPoints = new List<MKeyPoint>(vectorOfKeypoints.ToArray());

            // sort

            switch (sorting)
            {
                case SiftSorting.Response:
                    keyPoints.Sort((p1, p2) => p1.Response < p2.Response ? 1 : (p1.Response == p2.Response ? 0 : -1));
                    break;

                case SiftSorting.Size:
                    keyPoints.Sort((p1, p2) => p1.Size < p2.Size ? 1 : (p1.Size == p2.Size ? 0 : -1));
                    break;

                case SiftSorting.None:
                default:
                    break;
            }

            // trim

            keyPoints.RemoveRange(keyPointCount, keyPoints.Count - keyPointCount);

            // convert into list

            List<float> result = new List<float>(keyPointCount * 4);
            for (int i = 0; i < keyPointCount; ++i)
            {
                MKeyPoint current = keyPoints[i];

                result.Add(current.Point.X);
                result.Add(current.Point.Y);
                result.Add(current.Size);
                result.Add(current.Angle);
            }

            // visualize

            if (doDrawEverySingleImage)
            {
                vectorOfKeypoints = new Emgu.CV.Util.VectorOfKeyPoint(keyPoints.ToArray());

                Features2DToolbox.DrawKeypoints(image.GetMat(), vectorOfKeypoints, output, new Bgr(0, 0, 255), Features2DToolbox.KeypointDrawType.DrawRichKeypoints);


                String win1 = "SIFT"; //The name of the window
                CvInvoke.NamedWindow(win1); //Create the window using the specific name

                CvInvoke.Imshow(win1, output); //Show the image
                CvInvoke.WaitKey(0);  //Wait for the key pressing event
                CvInvoke.DestroyWindow(win1); //Destroy the window if key is pressed
            }

            return result;
        }
    }
}
