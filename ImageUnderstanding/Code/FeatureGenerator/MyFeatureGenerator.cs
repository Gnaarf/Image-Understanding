using Emgu.CV;
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV.XFeatures2D;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImageUnderstanding.FeatureGenerator
{

    public enum SiftSortingMethod
    {
        Response,
        Size,
        None,
    }

    public class MyFeatureGenerator : FeatureGenerator<T, float>
    {
        int _siftKeyPointCount;
        SiftSortingMethod _sortingMethod;
        SIFT sift;

        public MyFeatureGenerator(int siftKeyPointCount, SiftSortingMethod sortingMethod)
        {
            sift = new SIFT();
            _siftKeyPointCount = siftKeyPointCount;
            _sortingMethod = sortingMethod;
        }

        public override List<float> GetFeatureVector(T data)
        {
            return ExtractSiftFeatureVector(data, _siftKeyPointCount, _sortingMethod, false);
        }

        List<float> ExtractSiftFeatureVector(T image, int keyPointCount, SiftSortingMethod sorting1ethod, bool doDrawImage)
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

        public override void Dispose()
        {
            sift.Dispose();
        }

    }
}
