using libplctag.NativeImport;
using System;

namespace libplctag
{
    public static class LibPlcTag
    {
        private const int LIB_ATTRIBUTE_POINTER = 0;

        static public int VersionMajor => plctag.get_int_attribute(LIB_ATTRIBUTE_POINTER, "version_major", int.MinValue);
        static public int VersionMinor => plctag.get_int_attribute(LIB_ATTRIBUTE_POINTER, "version_minor", int.MinValue);
        static public int VersionPatch => plctag.get_int_attribute(LIB_ATTRIBUTE_POINTER, "version_patch", int.MinValue);
        static public bool IsRequiredVersion(int requiredMajor, int requiredMinor, int requiredPatch)
        {
            var result = (StatusCode)plctag.check_lib_version(requiredMajor, requiredMinor, requiredPatch);

            if (result == StatusCode.StatusOk)
                return true;
            else if (result == StatusCode.ErrorUnsupported)
                return false;
            else
                throw new NotImplementedException();
        }

        public static event EventHandler<LibPlcTagLogEventArgs> LogEventOccurred;

        static void OnLogEventOccurred(LibPlcTagLogEventArgs e)
        {
            EventHandler<LibPlcTagLogEventArgs> handler = LogEventOccurred;
            handler?.Invoke(typeof(LibPlcTag), e);
        }

        static LibPlcTag()
        {

            Action<int, int, string> callback = delegate (int tagPointer, int debugLevel, string message)
            {
                OnLogEventOccurred(new LibPlcTagLogEventArgs() { })

            };

            plctag.register_callback(LIB_ATTRIBUTE_POINTER, new plctag.log_callback_func(callback));

        }

    }
}
