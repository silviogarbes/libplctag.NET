using libplctag;
using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace CSharpDotNetCore
{
    class ExampleDisposable
    {
        public static void Run()
        {
            for (int ii = 0; ii < 100; ii++)
            {
                Console.WriteLine(ii);

                var myTags = Enumerable.Range(0, 1000)
                    .Select(i => new Tag(IPAddress.Parse("192.168.0.10"), "1,0", CpuType.Logix, DataType.DINT, $"MY_DINT_ARRAY_1000[{i}]", 5000, debugLevel: DebugLevel.None))
                    .ToList();


                Task.WaitAll(myTags.Select(tag => tag.ReadAsync()).ToArray());


                myTags.ForEach(tag => tag.Dispose());

                //using (var myTag = new Tag(IPAddress.Parse("192.168.0.10"), "1,0", CpuType.Logix, DataType.DINT, "Dummy", 5000))
                //{
                //    myTag.ReadAsync().GetAwaiter().GetResult();
                //}

                //var myTag = new Tag(IPAddress.Parse("192.168.0.10"), "1,0", CpuType.Logix, DataType.DINT, "Dummy", 5000);
                //myTag.ReadAsync().GetAwaiter().GetResult();
                //myTag.Dispose();
            }
        }
    }
}
