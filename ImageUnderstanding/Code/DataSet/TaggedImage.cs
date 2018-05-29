
namespace ImageUnderstanding
{
    public class TaggedImage : Image, Taggable<string>
    {
        public string Tag => _tag;
        string _tag;

        public TaggedImage(string path)
            : base(path)
        {
            _tag = GetTagFromPath(_path);
        }

        private static string GetTagFromPath(string path)
        {
            // the folder that contains the image is named according to tag
            string[] pathSegments = path.Split('/', '\\');
            return pathSegments[pathSegments.Length - 2];
        }
    }
}