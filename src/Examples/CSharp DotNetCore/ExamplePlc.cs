using libplctag;
using System;
using System.Net;
using System.Threading;

namespace CSharpDotNetCore
{
    class ExamplePlc
    {
        public static void Run()
        {

            const int TIMEOUT = 5000;

            var myPlc = new Plc()
            {
                Gateway = "192.168.0.10",
                Path = "1,0",
                Type = PlcType.ControlLogix
            };

            var myTag = myPlc.CreateTag("PROGRAM: SomeProgram.SomeDINT", DataType.DINT);
            
            myTag.Initialize(TIMEOUT);

            myTag.SetInt32(0, 3737);

            myTag.Write(TIMEOUT);

            myTag.Read(TIMEOUT);

            int myDint = myTag.GetInt32(0);

            Console.WriteLine(myDint);
        }
    }
}
