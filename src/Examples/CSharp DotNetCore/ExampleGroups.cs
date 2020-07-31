using libplctag;
using libplctag.DataTypes;
using System;
using System.Net;
using System.Threading;

namespace CSharpDotNetCore
{
    class ExampleGroups
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

            var myTag = new Tag(myPlc)
            {
                Name = "MY_DINT_1D",
                ElementSize = 4
            };
            
            myTag.Initialize(TIMEOUT);

            myTag.SetInt32(0, 3737);

            myTag.Write(TIMEOUT);

            myTag.Read(TIMEOUT);

            int myDint = myTag.GetInt32(0);

            Console.WriteLine(myDint);
        }

        public void MultiPlcGroup()
        {

            var myPlcA = new AttributeGroup()
            {
                Gateway = "192.168.0.10",
                Path = "1,0",
                PlcType = PlcType.ControlLogix,
                Protocol = Protocol.ab_eip
            };

            var myPlcB = new AttributeGroup()
            {
                Gateway = "192.168.0.11",
                Path = "1,0",
                PlcType = PlcType.ControlLogix,
                Protocol = Protocol.ab_eip
            };

            var dint1a = new Tag(myPlcA){ ElementCount = 1, Name = "MY_DINT_1D[0]" };
            var dint2a = new Tag(myPlcA){ ElementCount = 1, Name = "MY_DINT_1D[1]" };
            var dint1b = new Tag(myPlcB){ ElementCount = 1, Name = "MY_DINT_1D[2]" };
            var dint2b = new Tag(myPlcB){ ElementCount = 1, Name = "MY_DINT_1D[3]" };

            var tags = new TagGroup()
            {
                dint1a, dint2a, dint1b, dint2b
            };


            var timeout = 1000;
            tags.InitializeAll(timeout);
            tags.ReadAll(timeout);

            var value = dint1a.GetInt32(0);

            Console.WriteLine(value);

        }

        public static void GenericTagGroup()
        {

            var myPlcA = new AttributeGroup()
            {
                Gateway = "192.168.0.10",
                Path = "1,0",
                PlcType = PlcType.ControlLogix,
                Protocol = Protocol.ab_eip
            };

            var myPlcB = new AttributeGroup()
            {
                Gateway = "192.168.0.10",
                Path = "1,0",
                PlcType = PlcType.ControlLogix,
                Protocol = Protocol.ab_eip
            };

            var dint1a = new Tag<DintMarshaller, int>(myPlcA, "MY_DINT_1D[0]");
            var dint2a = new Tag<DintMarshaller, int>(myPlcA, "MY_DINT_1D[1]");
            var dint1b = new Tag<DintMarshaller, int>(myPlcB, "MY_DINT_1D[2]");
            var dint2b = new Tag<DintMarshaller, int>(myPlcB, "MY_DINT_1D[3]");

            var tags = new TagGroup()
            {
                dint1a, dint2a, dint1b, dint2b
            };

            var timeout = 1000;
            tags.InitializeAll(timeout);
            tags.ReadAll(timeout);

            var value = dint2b.Value[0];

            Console.WriteLine(value);

        }
    }
}
