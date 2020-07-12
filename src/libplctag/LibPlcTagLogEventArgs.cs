using System;

namespace libplctag
{
    public class LibPlcTagLogEventArgs : EventArgs
    {
        public int tagPointer { get; set; }
        public DebugLevel DebugLevel { get; set; }
    }
}