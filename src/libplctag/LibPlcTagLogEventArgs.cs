using System;

namespace libplctag
{
    public class LibPlcTagLogEventArgs : EventArgs
    {
        public int TagHandle { get; set; }
        public DebugLevel DebugLevel { get; set; }
        public string Message { get; set; }
    }
}