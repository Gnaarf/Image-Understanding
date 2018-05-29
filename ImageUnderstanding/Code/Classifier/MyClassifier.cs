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
            if(trainingDataSet.Count > 0)
            {
                TaggedImage image = trainingDataSet[0];

                Emgu.CV.Util.VectorOfKeyPoint vectorOfKeypoints = new Emgu.CV.Util.VectorOfKeyPoint();

                Mat output = image.GetMat().Clone();

                sift.DetectAndCompute(image.GetMat(), null, vectorOfKeypoints, output, false);

                Features2DToolbox.DrawKeypoints(image.GetMat(), vectorOfKeypoints, output, new Bgr(0, 0, 0));


                String win1 = "SIFT"; //The name of the window
                CvInvoke.NamedWindow(win1); //Create the window using the specific name

                CvInvoke.Imshow(win1, output); //Show the image
                CvInvoke.WaitKey(0);  //Wait for the key pressing event
                CvInvoke.DestroyWindow(win1); //Destroy the window if key is pressed
            }

            return;

            foreach(TaggedImage image in trainingDataSet)
            {

                Emgu.CV.Util.VectorOfKeyPoint vectorOfKeypoints = new Emgu.CV.Util.VectorOfKeyPoint();
                
                Mat output = image.GetMat().Clone();

                sift.DetectAndCompute(image.GetMat(), null, vectorOfKeypoints, output, false);
            }

            throw new NotImplementedException();
        }

        public override string Evaluate(TaggedImage dataSample)
        {
            throw new NotImplementedException();
        }
    }
}
