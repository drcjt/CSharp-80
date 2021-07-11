using System;

namespace SampleDsk
{
    public static class Program
    {
        static void Main(string[] args)
        {
            // Dummy program - this solution is used just to build the DSK image
            // Note we cannot copy the output from each sample program to the DSK image
            // as part of these individual projects as TRSWRITE cannot cope with
            // another thread/process running TRSWRITE on the same DSK file at the same time
            // and MSBUILD/VS likes to build the projects on separate threads/processes
        }
    }
}
