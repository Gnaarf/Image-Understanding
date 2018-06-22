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
    [Serializable]
    public class SVMClassifierInitializationData
    {
        public SVM.SvmType svmType = SVM.SvmType.CSvc;
        public SVM.SvmKernelType kernelType = SVM.SvmKernelType.Rbf;
        public double c = 1;
        public double coef0 = 1;
        public double degree = 1;
        public double gamma = 0.1;
        public double nu = 1;
        public double p = 1;
    }

    public class SVMClassifier : Classifier<TaggedImage, string, float>
    {
        SVM _svm;

        public SVMClassifier()
        {
            _svm = new SVM();
        }

        public override void InitializeViaConfig(ImageUnderstandingConfig config)
        {
            _svm.C = config.svmInitializationData.c;
            _svm.Coef0 = config.svmInitializationData.coef0;
            _svm.Degree = config.svmInitializationData.degree;
            _svm.Gamma = config.svmInitializationData.gamma;
            _svm.Nu = config.svmInitializationData.nu;
            _svm.P = config.svmInitializationData.p;
            _svm.SetKernel(config.svmInitializationData.kernelType);
            _svm.Type = config.svmInitializationData.svmType;
            Console.WriteLine("c = " + _svm.C + ", gamma = " + _svm.Gamma);
        }

        public override void Train(List<TaggedImage> trainingDataSet)
        {
            if(trainingDataSet.Count == 0)
            {
                throw new Exception("trainingDataSet is empty");
            }
            
            float[,] featureVectors = new float[trainingDataSet.Count, trainingDataSet[0].FeatureVector.Count];
            int[,] labels = new int[trainingDataSet.Count, 1];

            for (int i = 0; i < trainingDataSet.Count; ++i)
            {
                TaggedImage image = trainingDataSet[i];
                
                for (int j = 0; j < image.FeatureVector.Count; ++j)
                {
                    featureVectors[i, j] = image.FeatureVector[j];
                }
                
                labels[i, 0] = image.TagIndex;
            }

            Matrix<float> featureVectorsMatrix = new Matrix<float>(featureVectors);
            Matrix<int> labelsMatrix = new Matrix<int>(labels);

            TrainData td = new TrainData(featureVectorsMatrix, Emgu.CV.ML.MlEnum.DataLayoutType.RowSample, labelsMatrix);

            _svm.Train(td);

            return;
        }

        public override string Evaluate(TaggedImage dataSample)
        {
            float[,] featureVector2D = new float[1, dataSample.FeatureVector.Count];

            List<float> featureVector = dataSample.FeatureVector;

            for (int j = 0; j < featureVector.Count; ++j)
            {
                featureVector2D[0, j] = featureVector[j];
            }
            
            Matrix<float> featureVectorMatrix = new Matrix<float>(featureVector2D);

            string res = TaggedImage.GetStringFromIndex((int)_svm.Predict(featureVectorMatrix));
            
            return res;
        }
        
        public override void Dispose()
        {
            _svm.Dispose();
        }
    }
}
