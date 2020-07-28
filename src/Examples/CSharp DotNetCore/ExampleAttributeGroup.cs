using libplctag;
using System;
using System.Net;
using System.Threading;

namespace CSharpDotNetCore
{
    class ExampleAttributeGroup
    {
        public static void Run()
        {

            const int TIMEOUT = 5000;

            var myPlc = new AttributeGroup()
            {
                Gateway = "192.168.0.10",
                Path = "1,0",
                PlcType = PlcType.ControlLogix,
                Protocol = Protocol.ab_eip
            };

            var myTag = new Tag()
            {
                Name = "MY_DINT_1D",
                ElementSize = DataType.DINT
            };
            
            myTag.Initialize(TIMEOUT);

            myTag.SetInt32(0, 3737);

            myTag.Write(TIMEOUT);

            myTag.Read(TIMEOUT);

            int myDint = myTag.GetInt32(0);

            Console.WriteLine(myDint);
        }
    }
}
