using libplctag;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

            while (myTag.GetStatus() == Status.Pending)
                Thread.Sleep(100);
            if (myTag.GetStatus() != Status.Ok)
                throw new LibPlcTagException(myTag.GetStatus());

            myTag.SetInt32(0, 3737);

            await myTag.WriteAsync();
            if (myTag.GetStatus() != Status.Ok)
                throw new LibPlcTagException(myTag.GetStatus());

            await myTag.ReadAsync();
            if (myTag.GetStatus() != Status.Ok)
                throw new LibPlcTagException(myTag.GetStatus());

            int myDint = myTag.GetInt32(0);

            Console.WriteLine(myDint);
        }


        public static async void SyncAsyncComparison()
        {

            Console.WriteLine("This method measures the speed of synchronous vs asynchronous reads");
            var myTag = new Tag(IPAddress.Parse("192.168.0.10"), "1,0", CpuType.Logix, DataType.DINT, "Dummy", 5000);

            while (myTag.GetStatus() == Status.Pending)
                Thread.Sleep(100);
            if (myTag.GetStatus() != Status.Ok)
                throw new LibPlcTagException(myTag.GetStatus());

            int repetitions = 1000;

            Console.Write($"Running {repetitions} Read() calls...");
            var syncStopWatch = Stopwatch.StartNew();
            for (int ii = 0; ii < repetitions; ii++)
            {
                // We know that it takes less than 1000ms per read, so it will return as soon as it is finished
                myTag.Read(1000);
            }
            syncStopWatch.Stop();
            Console.WriteLine($"\ttook {(float)syncStopWatch.ElapsedMilliseconds / (float)repetitions}ms on average");


            Console.Write($"Running {repetitions} ReadAsync() calls...");
            var asyncStopWatch = Stopwatch.StartNew();
            for (int ii = 0; ii < repetitions; ii++)
            {
                await myTag.ReadAsync();
            }
            asyncStopWatch.Stop();
            Console.WriteLine($"\ttook {(float)asyncStopWatch.ElapsedMilliseconds / (float)repetitions}ms on average");

        }

        public static void ParallelBlockingReads()
        {

            Console.WriteLine("This method measures the speed of synchronous vs asynchronous reads");
            var myTag = new Tag(IPAddress.Parse("192.168.0.10"), "1,0", CpuType.Logix, DataType.DINT, "Dummy", 5000);

            while (myTag.GetStatus() == Status.Pending)
                Thread.Sleep(100);
            if (myTag.GetStatus() != Status.Ok)
                throw new LibPlcTagException(myTag.GetStatus());

            int repetitions = 100;

            Console.Write($"Running {repetitions} calls...");
            var sw = Stopwatch.StartNew();
            for (int ii = 0; ii < repetitions; ii++)
            {
                Task.WaitAll(
                    Task.Run(() => myTag.Read(1000)),
                    Task.Run(() => myTag.Read(1000))
                    );
            }
            sw.Stop();

            Console.WriteLine($"\ttook {(float)sw.ElapsedMilliseconds / (float)repetitions}ms on average");

        }

        public static void SyncAsyncMultipleTagComparison(int repetitions = 100)
        {
            Console.WriteLine("This method measures the speed of synchronous vs asynchronous reads for multiple tags simultaneously");

            SyncAsyncMultipleTagComparisonSingleRun(1, repetitions);
            SyncAsyncMultipleTagComparisonSingleRun(2, repetitions);
            SyncAsyncMultipleTagComparisonSingleRun(3, repetitions);
            SyncAsyncMultipleTagComparisonSingleRun(4, repetitions);
            SyncAsyncMultipleTagComparisonSingleRun(5, repetitions);
            SyncAsyncMultipleTagComparisonSingleRun(6, repetitions);
            SyncAsyncMultipleTagComparisonSingleRun(7, repetitions);
            SyncAsyncMultipleTagComparisonSingleRun(8, repetitions);
            SyncAsyncMultipleTagComparisonSingleRun(9, repetitions);
            SyncAsyncMultipleTagComparisonSingleRun(10, repetitions);
            //SyncAsyncMultipleTagComparisonSingleRun(11);
            //SyncAsyncMultipleTagComparisonSingleRun(12);
            //SyncAsyncMultipleTagComparisonSingleRun(13);
            //SyncAsyncMultipleTagComparisonSingleRun(14);
            //SyncAsyncMultipleTagComparisonSingleRun(15);
            //SyncAsyncMultipleTagComparisonSingleRun(16);
            //SyncAsyncMultipleTagComparisonSingleRun(17);
            //SyncAsyncMultipleTagComparisonSingleRun(18);
            //SyncAsyncMultipleTagComparisonSingleRun(19);
            //SyncAsyncMultipleTagComparisonSingleRun(20);
            //SyncAsyncMultipleTagComparisonSingleRun(25);
            //SyncAsyncMultipleTagComparisonSingleRun(30);
            //SyncAsyncMultipleTagComparisonSingleRun(35);
            //SyncAsyncMultipleTagComparisonSingleRun(40);
            //SyncAsyncMultipleTagComparisonSingleRun(45);
            //SyncAsyncMultipleTagComparisonSingleRun(50);
            //SyncAsyncMultipleTagComparisonSingleRun(60);
            //SyncAsyncMultipleTagComparisonSingleRun(70);
            //SyncAsyncMultipleTagComparisonSingleRun(80);
            //SyncAsyncMultipleTagComparisonSingleRun(90);
            //SyncAsyncMultipleTagComparisonSingleRun(100);
            //SyncAsyncMultipleTagComparisonSingleRun(200);
            //SyncAsyncMultipleTagComparisonSingleRun(300);
            //SyncAsyncMultipleTagComparisonSingleRun(400);
            //SyncAsyncMultipleTagComparisonSingleRun(500);
            //SyncAsyncMultipleTagComparisonSingleRun(600);
            //SyncAsyncMultipleTagComparisonSingleRun(700);
            //SyncAsyncMultipleTagComparisonSingleRun(800);
            //SyncAsyncMultipleTagComparisonSingleRun(900);
            //SyncAsyncMultipleTagComparisonSingleRun(1000);
        }

        private static void SyncAsyncMultipleTagComparisonSingleRun(int maxTags, int repetitions = 10)
        {

            var myTags = Enumerable.Range(0, maxTags).Select(i => new Tag(IPAddress.Parse("192.168.0.10"), "1,0", CpuType.Logix, DataType.DINT, $"MY_DINT_ARRAY_1000[{i}]", 5000));

            Console.Write($"Running {repetitions} ReadAsync() calls on {myTags.Count()} tags simultaneously...");
            var asyncStopWatch = Stopwatch.StartNew();

            Task.WaitAll(myTags.Select(tag =>
            {
                return Task.Run(async () =>
                {
                    for (int ii = 0; ii < repetitions; ii++)
                    {
                        //Console.WriteLine($"{ii} {tag.Name}");
                            await tag.ReadAsync();
                        //try
                        //{
                        //    await tag.ReadAsync();
                        //}
                        //catch (LibPlcTagException e)
                        //{
                        //    Debug.WriteLine($"{ii} {e.Message}");
                        //}
                    }
                        
                });
            }).ToArray());

            asyncStopWatch.Stop();
            Console.WriteLine($"\ttook {(float)asyncStopWatch.ElapsedMilliseconds / (float)repetitions}ms on average");

        }
    }
}
