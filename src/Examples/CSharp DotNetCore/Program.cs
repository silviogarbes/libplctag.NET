using System;

namespace CSharpDotNetCore
{
    class Program
    {
        static void Main(string[] args)
        {
            ExampleAsync.Run();
            ExampleRW.Run();
            ExampleArray.Run();
            //NativeImportExample.Run();
            //NativeImportExample.RunCallbackExample();
            //NativeImportExample.RunLoggerExample();

            Console.ReadKey();
        }
    }
}