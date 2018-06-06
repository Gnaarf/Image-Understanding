﻿using System;
using System.Collections.Generic;
using Emgu.CV;
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using Emgu.CV.XFeatures2D;
using Emgu.CV.ML;
using Emgu.CV.Util;

namespace ImageUnderstanding.Classifier
{
    public class MyClassifier : Classifier<T, string, float>
    {
        KNearest kNearest;

        public MyClassifier()
        {
            kNearest = new KNearest();
        }

        public override void Train(List<T> trainingDataSet)
        {
            if(trainingDataSet.Count == 0)
            {
                throw new Exception("trainingDataSet is empty");
            }

            float[,] featureVectors = new float[trainingDataSet.Count, trainingDataSet[0].FeatureVector.Count];
            int[,] labels = new int[trainingDataSet.Count, 1];

            for (int i = 0; i < trainingDataSet.Count; ++i)
            {
                T image = trainingDataSet[i];
                
                for (int j = 0; j < image.FeatureVector.Count; ++j)
                {
                    featureVectors[i, j] = image.FeatureVector[j];
                }
                
                labels[i, 0] = image.TagIndex;
            }

            Matrix<float> featureVectorsMatrix = new Matrix<float>(featureVectors);
            Matrix<int> labelsMatrix = new Matrix<int>(labels);

            TrainData td = new TrainData(featureVectorsMatrix, Emgu.CV.ML.MlEnum.DataLayoutType.RowSample, labelsMatrix);

            kNearest.Train(td);

            return;
        }

        public override string Evaluate(T dataSample)
        {
            float[,] featureVector2D = new float[1, dataSample.FeatureVector.Count];

            List<float> featureVector = dataSample.FeatureVector;

            for (int j = 0; j < featureVector.Count; ++j)
            {
                featureVector2D[0, j] = featureVector[j];
            }
            
            Matrix<float> featureVectorMatrix = new Matrix<float>(featureVector2D);

            string res = T.GetStringFromIndex((int)kNearest.Predict(featureVectorMatrix));
            
            return res;
        }
        
        public override void Dispose()
        {
            kNearest.Dispose();
        }
    }
}
