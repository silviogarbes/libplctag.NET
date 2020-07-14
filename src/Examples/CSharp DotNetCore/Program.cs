using System;
using System.Threading.Tasks;

namespace CSharpDotNetCore
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //ExampleAsync.Run();
            ExampleAsync.SyncAsyncComparison();
            //ExampleRW.Run();
            //ExampleArray.Run();
            //NativeImportExample.Run();
            //NativeImportExample.RunCallbackExample();
            //NativeImportExample.RunLoggerExample();

            Console.ReadKey();
        }
    }
}