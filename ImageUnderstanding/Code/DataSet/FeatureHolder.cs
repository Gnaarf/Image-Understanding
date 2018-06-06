
using System.Collections.Generic;

namespace ImageUnderstanding
{
    public interface FeatureHolder<T>
    {
        List<T> FeatureVector { get; set; }
    }
}