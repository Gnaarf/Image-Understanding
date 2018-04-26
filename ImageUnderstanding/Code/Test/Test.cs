using System.Collections;
using System.Collections.Generic;

namespace Test
{
    public abstract class Test
    {
        public abstract bool PerformTest(out string message);

        public bool PerformTest()
        {
            string tmp;
            return PerformTest(out tmp);
        }
    }
}
