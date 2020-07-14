using libplctag;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace CSharpDotNetCore
{
    class ExampleAsync
    {
        public static async Task Run()
        {
            var myTag = new Tag(IPAddress.Parse("10.10.10.10"), "1,0", CpuType.Logix, DataType.DINT, "PROGRAM:SomeProgram.SomeDINT", 5000);

            while (myTag.GetStatus() == StatusCode.StatusPending)
                Thread.Sleep(100);
            if (myTag.GetStatus() != StatusCode.StatusOk)
                throw new LibPlcTagException(myTag.GetStatus());

            myTag.SetInt32(0, 3737);

            await myTag.WriteAsync();
            if (myTag.GetStatus() != StatusCode.StatusOk)
                throw new LibPlcTagException(myTag.GetStatus());

            await myTag.ReadAsync();
            if (myTag.GetStatus() != StatusCode.StatusOk)
                throw new LibPlcTagException(myTag.GetStatus());

            int myDint = myTag.GetInt32(0);

            Console.WriteLine(myDint);
        }


        public static async Task SyncAsyncComparison()
        {

            Console.WriteLine("This method measures the speed of synchronous vs asynchronous reads");
            var myTag = new Tag(IPAddress.Parse("10.10.10.10"), "1,0", CpuType.Logix, DataType.DINT, "PROGRAM:SomeProgram.SomeDINT", 5000);

            while (myTag.GetStatus() == StatusCode.StatusPending)
                Thread.Sleep(100);
            if (myTag.GetStatus() != StatusCode.StatusOk)
                throw new LibPlcTagException(myTag.GetStatus());

            int repetitions = 1000;

            Console.Write($"Running {repetitions} Read() calls...");
            var syncStopWatch = Stopwatch.StartNew();
            for (int ii = 0; ii < repetitions; ii++)
            {
                // We know that it takes less than 100ms per read, so it will return as soon as it is finished
                myTag.Read(100);
            }
            syncStopWatch.Stop();
            Console.WriteLine($"\ttook {syncStopWatch.ElapsedMilliseconds / repetitions}ms per Read on average");


            Console.Write($"Running {repetitions} ReadAsync() calls...");
            var asyncStopWatch = Stopwatch.StartNew();
            for (int ii = 0; ii < repetitions; ii++)
            {
                await myTag.ReadAsync();
            }
            asyncStopWatch.Stop();
            Console.WriteLine($"\ttook {asyncStopWatch.ElapsedMilliseconds / repetitions}ms per Read on average");

        }
    }
}
