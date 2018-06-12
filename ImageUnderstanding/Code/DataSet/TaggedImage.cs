using System.Collections.Generic;

namespace ImageUnderstanding
{
    public class TaggedImage : Image, Taggable<string>, FeatureHolder<float>
    {
        public string Tag => _tag;
        string _tag;

        public int TagIndex { get; private set; }

        public List<float> FeatureVector { get; set; }

        static List<string> intToStringLookUp = new List<string>();
        static Dictionary<string, int> stringToIntLookUp = new Dictionary<string, int>();

        public TaggedImage(string path)
            : base(path)
        {
            FeatureVector = new List<float>();

            _tag = GetTagFromPath(_path);

            if(!stringToIntLookUp.ContainsKey(_tag))
            {
                stringToIntLookUp[_tag] = intToStringLookUp.Count;
                intToStringLookUp.Add(_tag);
            }

            TagIndex = stringToIntLookUp[_tag];
        }

        private static string GetTagFromPath(string path)
        {
            // the folder that contains the image is named according to tag
            string[] pathSegments = path.Split('/', '\\');
            return pathSegments[pathSegments.Length - 2];
        }

        public static int GetTagIndexFromString(string s)
        {
            return stringToIntLookUp[s];
        }

        public static string GetStringFromIndex(int index)
        {
            return intToStringLookUp[index];
        }
    }
}