using System;
using System.Collections.Generic;
using Emgu.CV;
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using Emgu.CV.XFeatures2D;
using Emgu.CV.ML;
using Emgu.CV.Util;

namespace ImageUnderstanding.Classifier
{
    public class MyClassifier : Classifier<TaggedImage, string>
    {
        SIFT sift;
        int _siftKeyPointCount;

        KNearest kNearest;

        public MyClassifier(int siftKeypointCount = 100)
        {
            sift = new SIFT();
            _siftKeyPointCount = siftKeypointCount;

            kNearest = new KNearest();
        }

        string consoleFeedBackString = "";

        public override void Train(List<TaggedImage> trainingDataSet)
        {
            float[,] featureVectors = new float[trainingDataSet.Count, 5 * _siftKeyPointCount];
            int[,] labels = new int[trainingDataSet.Count, 1];

            for (int i = 0; i < trainingDataSet.Count; ++i)
            {
                TaggedImage image = trainingDataSet[i];

                if (consoleFeedBackString != image.Tag)
                {
                    consoleFeedBackString = image.Tag;
                    Console.WriteLine("processing: " + consoleFeedBackString);
                }

                List<float> featureVector = ExtractSiftFeatureVector(image, _siftKeyPointCount, SiftSortingMethod.Response, false);

                for (int j = 0; j < featureVector.Count; ++j)
                {
                    featureVectors[i, j] = featureVector[j];
                }

                labels[i, 0] = image.TagIndex;
            }

            Matrix<float> featureVectorsMatrix = new Matrix<float>(featureVectors);
            Matrix<int> labelsMatrix = new Matrix<int>(labels);

            TrainData td = new TrainData(featureVectorsMatrix, Emgu.CV.ML.MlEnum.DataLayoutType.RowSample, labelsMatrix);

            kNearest.Train(td);

            consoleFeedBackString = "";

            return;
        }

        public override string Evaluate(TaggedImage dataSample)
        {
            float[,] featureVector2D = new float[1, 5 * _siftKeyPointCount];

            List<float> featureVector = ExtractSiftFeatureVector(dataSample, _siftKeyPointCount, SiftSortingMethod.Response, false);

            for (int j = 0; j < featureVector.Count; ++j)
            {
                featureVector2D[0, j] = featureVector[j];
            }
            
            Matrix<float> featureVectorMatrix = new Matrix<float>(featureVector2D);

            string res= TaggedImage.GetStringFromIndex((int)kNearest.Predict(featureVectorMatrix));
            
            return res;
        }

        enum SiftSortingMethod
        {
            Response,
            Size,
            None,
        }

        List<float> ExtractSiftFeatureVector(TaggedImage image, int keyPointCount, SiftSortingMethod sorting1ethod, bool doDrawImage)
        {
            // use the emgu functions to gather keypoints

            VectorOfKeyPoint vectorOfKeypoints = new VectorOfKeyPoint();

            Mat output = image.GetMat().Clone(); // only needed for drawing

            sift.DetectAndCompute(image.GetMat(), null, vectorOfKeypoints, output, false);

            // put it into useful data formats

            List<MKeyPoint> keyPoints = new List<MKeyPoint>(vectorOfKeypoints.ToArray());

            // sort

            switch (sorting1ethod)
            {
                case SiftSortingMethod.Response:
                    keyPoints.Sort((p1, p2) => p1.Response < p2.Response ? 1 : (p1.Response == p2.Response ? 0 : -1));
                    break;

                case SiftSortingMethod.Size:
                    keyPoints.Sort((p1, p2) => p1.Size < p2.Size ? 1 : (p1.Size == p2.Size ? 0 : -1));
                    break;

                case SiftSortingMethod.None:
                default:
                    break;
            }

            // expand/trim

            while (keyPoints.Count < keyPointCount)
            {
                keyPoints.Add(new MKeyPoint());
            }

            if (keyPoints.Count > keyPointCount)
            {
                keyPoints.RemoveRange(keyPointCount, keyPoints.Count - keyPointCount);
            }

            // visualize

            if (doDrawImage)
            {
                vectorOfKeypoints = new VectorOfKeyPoint(keyPoints.ToArray());

                Features2DToolbox.DrawKeypoints(image.GetMat(), vectorOfKeypoints, output, new Bgr(0, 0, 255), Features2DToolbox.KeypointDrawType.DrawRichKeypoints);

                String win1 = "SIFT"; //The name of the window
                CvInvoke.NamedWindow(win1); //Create the window using the specific name

                CvInvoke.Imshow(win1, output); //Show the image
                CvInvoke.WaitKey(0);  //Wait for the key pressing event
                CvInvoke.DestroyWindow(win1); //Destroy the window if key is pressed
            }

            // convert to list

            List<float> result = new List<float>(5 * keyPointCount);

            for (int i = 0; i < keyPoints.Count; ++i)
            {
                MKeyPoint current = keyPoints[i];

                result.Add(current.Point.X);
                result.Add(current.Point.Y);
                result.Add(current.Size);
                result.Add(current.Angle);
                result.Add(current.Response);
            }

            return result;
        }

        public void Dispose()
        {
            kNearest.Dispose();
            sift.Dispose();
        }
    }
}
