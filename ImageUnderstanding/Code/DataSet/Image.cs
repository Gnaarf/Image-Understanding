
using Emgu.CV;
using Emgu.CV.CvEnum;

namespace ImageUnderstanding
{
    public class Image
    {
        protected string _path;

        public Image(string path)
        {
            this._path = path;
        }

        public Mat GetMat()
        {
            return CvInvoke.Imread(_path, ImreadModes.Grayscale);
        }
    }
}