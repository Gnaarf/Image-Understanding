using ImageUnderstanding.Classifier;
using ImageUnderstanding.FeatureGenerator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ImageUnderstanding
{

    public class ImageUnderstandingConfig
    {
        public string path = "C:\\Users\\JinxPliskin\\Desktop\\Projects\\SelectedTopicsInImageUnderstanding\\caltech101\\";
        public int FoldCount = 10;  // default value, usually will be overwritten by the values from the config file
        public int TestFoldCount = 1; // default value, see above

        public string[] RestrictTo = new string[] { "camera", "cannon", "brontosaurus", "ibis", "inline_skate" }; // default value, see above
        public string[] IgnoreTags = new string[] { "BACKGROUND_Google" }; // default value, see above

        public bool UseSoftNormalization = false;

        public enum FeatureGeneratorMethod
        {
            HOG,
            Random,
            SIFT,
            Unity,
            My,
        }

        public FeatureGeneratorMethod featureGeneratorMethod = FeatureGeneratorMethod.HOG;

        public HOGFeatureGeneratorInitializationData HOGInitializationData = new HOGFeatureGeneratorInitializationData();
        public SIFTFeatureGeneratorInitializationData SIFTInitializationData = new SIFTFeatureGeneratorInitializationData();
        public MyFeatureGeneratorInitializationData MyInitializationData = new MyFeatureGeneratorInitializationData();
        public RandomFeatureGeneratorInitializationData RandomFGInitializationData = new RandomFeatureGeneratorInitializationData();
        public UnityFeatureGeneratorInitializationData UnityInitializationData = new UnityFeatureGeneratorInitializationData();

        public enum ClassifierMethod
        {
            KNearest,
            Random,
            SingleResult,
            SVM,
        }

        public ClassifierMethod classifierMethod = ClassifierMethod.KNearest; // default value, see above

        public KNearestClassifierInitiaizationData kNearestInitiaizationData = new KNearestClassifierInitiaizationData();
        public RandomClassifierInitializationData randomCInitializationData = new RandomClassifierInitializationData();
        public SingleResultClassifierInitializationData singleResultInitializationData = new SingleResultClassifierInitializationData();
        public SVMClassifierInitializationData svmInitializationData = new SVMClassifierInitializationData();




        public static ImageUnderstandingConfig ImportSettings()
        {
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(ImageUnderstandingConfig));
            FileStream fileStream = File.Open(".\\ImageUnderstandingConfig.txt", FileMode.OpenOrCreate);

            ImageUnderstandingConfig config = new ImageUnderstandingConfig();
            try
            {
                config = (ImageUnderstandingConfig)serializer.Deserialize(fileStream);
            }
            catch(Exception e) //em all
            {
            }

            fileStream.Close();

            File.Delete(".\\ImageUnderstandingConfig.txt");
            fileStream = File.Create(".\\ImageUnderstandingConfig.txt");
            serializer.Serialize(fileStream, config);
            fileStream.Close();

            return config;
        }
    }
}
