using System;
using System.Threading.Tasks;

namespace CSharpDotNetCore
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await ExampleAsync.Run();
            await ExampleAsync.SyncAsyncComparison();
            //ExampleRW.Run();
            //ExampleArray.Run();
            //NativeImportExample.Run();
            //NativeImportExample.RunCallbackExample();
            //NativeImportExample.RunLoggerExample();

            Console.ReadKey();
        }
    }
}