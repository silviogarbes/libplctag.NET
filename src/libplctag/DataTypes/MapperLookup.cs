using Microsoft.MinIoC;
using System;
using System.Collections.Generic;
using System.Text;

namespace libplctag.DataTypes
{
    public static class MapperLookup
    {
        private static Container container;

        static MapperLookup()
        {
            //Apparently static constructors are a thing

            container = new Container();

            //These are the default registrations

            SetMapper<bool, BoolPlcMapper>();
            SetMapper<bool[], BoolPlcMapper>();
            SetMapper<bool[,], BoolPlcMapper>();
            SetMapper<bool[,,], BoolPlcMapper>();

            SetMapper<sbyte, SintPlcMapper>();
            SetMapper<sbyte[], SintPlcMapper>();
            SetMapper<sbyte[,], SintPlcMapper>();
            SetMapper<sbyte[,,], SintPlcMapper>();

            SetMapper<short, IntPlcMapper>();
            SetMapper<short[], IntPlcMapper>();
            SetMapper<short[,], IntPlcMapper>();
            SetMapper<short[,,], IntPlcMapper>();

            SetMapper<int, DintPlcMapper>();
            SetMapper<int[], DintPlcMapper>();
            SetMapper<int[,], DintPlcMapper>();
            SetMapper<int[,,], DintPlcMapper>();

            SetMapper<long, LintPlcMapper>();
            SetMapper<long[], LintPlcMapper>();
            SetMapper<long[,], LintPlcMapper>();
            SetMapper<long[,,], LintPlcMapper>();

            SetMapper<float, RealPlcMapper>();
            SetMapper<float[], RealPlcMapper>();
            SetMapper<float[,], RealPlcMapper>();
            SetMapper<float[,,], RealPlcMapper>();

            SetMapper<double, LrealPlcMapper>();
            SetMapper<double[], LrealPlcMapper>();
            SetMapper<double[,], LrealPlcMapper>();
            SetMapper<double[,,], LrealPlcMapper>();

            SetMapper<string, StringPlcMapper>();
            SetMapper<string[], StringPlcMapper>();
            SetMapper<string[,], StringPlcMapper>();
            SetMapper<string[,,], StringPlcMapper>();

        }

        public static IPlcMapper<T> GetMapper<T>() => container.Resolve<IPlcMapper<T>>();

        public static void SetMapper<T, M>() where M : IPlcMapper<T>, new() => container.Register<IPlcMapper<T>>(typeof(M));
    }
}
