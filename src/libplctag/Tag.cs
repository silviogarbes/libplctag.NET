using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using libplctag.NativeImport;

namespace libplctag
{

    public class Tag : IDisposable
    {

        private plctag.callback_func coreLibCallbackFuncDelegate;

        public Protocol Protocol { get; }
        public IPAddress Gateway { get; }
        public string Path { get; }
        public CpuType CPU { get; }
        public int ElementSize { get; }
        public int ElementCount { get; }
        public string Name { get; }
        public bool UseConnectedMessaging { get; }
        public DebugLevel DebugLevel
        {
            get => (DebugLevel)plctag.get_int_attribute(0, "debug_level", int.MinValue);
            set => plctag.set_debug_level((int)value);
        }
        public int ReadCacheMillisecondDuration
        {
            get => plctag.get_int_attribute(tagHandle, "read_cache_ms", int.MinValue);
            set => plctag.set_int_attribute(tagHandle, "read_cache_ms", value);
        }

        private readonly int tagHandle;

        /// <summary>
        /// Provides a new tag. If the CPU type is LGX, the port type and slot has to be specified.
        /// </summary>
        /// <param name="gateway">IP address of the gateway for this protocol. Could be the IP address of the PLC you want to access.</param>
        /// <param name="path">Required for LGX, Optional for PLC/SLC/MLGX IOI path to access the PLC from the gateway.
        /// <param name="cpuType">Allen-Bradley CPU model</param>
        /// <param name="elementSize">The size of an element in bytes. The tag is assumed to be composed of elements of the same size. For structure tags, use the total size of the structure.</param>
        /// <param name="name">The textual name of the tag to access. The name is anything allowed by the protocol. E.g. myDataStruct.rotationTimer.ACC, myDINTArray[42] etc.</param>
        /// <param name="elementCount">elements count: 1- single, n-array.</param>
        /// <param name="millisecondTimeout"></param>
        /// <param name="debugLevel"></param>
        /// <param name="protocol">Currently only ab_eip supported.</param>
        /// <param name="readCacheMillisecondDuration">Set the amount of time to cache read results</param>
        /// <param name="useConnectedMessaging">Control whether to use connected or unconnected messaging.</param>
        public Tag(IPAddress gateway, string path, CpuType cpuType, int elementSize, string name, int millisecondTimeout, int elementCount = 1, DebugLevel debugLevel = DebugLevel.None, Protocol protocol = Protocol.ab_eip, int readCacheMillisecondDuration = default, bool useConnectedMessaging = true)
        {

            Protocol = protocol;
            Gateway = gateway;
            Path = path;
            CPU = cpuType;
            ElementSize = elementSize;
            ElementCount = elementCount;
            Name = name;
            UseConnectedMessaging = useConnectedMessaging;

            var attributeString = GetAttributeString(protocol, gateway, path, cpuType, elementSize, elementCount, name, debugLevel, readCacheMillisecondDuration, useConnectedMessaging);

            var createResult = plctag.create(attributeString, millisecondTimeout);
            Console.WriteLine(createResult);
            //Console.Read();

            if (createResult >= 0)
                tagHandle = createResult;
            else  // If the result is less than 0, it is an error code
                throw new LibPlcTagException((Status)createResult);

            SetUpEvents();

        }

        void SetUpEvents()
        {

            // Used to finalize the asynchronous read/write task completion sources
            ReadCompleted += ReadTaskCompleter;
            WriteCompleted += WriteTaskCompleter;

            // Need to keep a reference to the delegate in memory so it doesn't get garbage collected
            coreLibCallbackFuncDelegate = new plctag.callback_func(coreLibEventCallback);
            
            plctag.register_callback(tagHandle, coreLibCallbackFuncDelegate);

            if (GetStatus() != Status.Ok)
                throw new LibPlcTagException(GetStatus());

        }


        ~Tag()
        {
            Dispose();
        }

        private static string GetAttributeString(Protocol protocol, IPAddress gateway, string path, CpuType CPU, int elementSize, int elementCount, string name, DebugLevel debugLevel, int readCacheMillisecondDuration, bool useConnectedMessaging)
        {

            var attributes = new Dictionary<string, string>();

            attributes.Add("protocol", protocol.ToString());
            attributes.Add("gateway", gateway.ToString());

            if (!string.IsNullOrEmpty(path))
                attributes.Add("path", path);

            attributes.Add("cpu", CPU.ToString().ToLower());
            attributes.Add("elem_size", elementSize.ToString());
            attributes.Add("elem_count", elementCount.ToString());
            attributes.Add("name", name);

            if (debugLevel > DebugLevel.None)
                attributes.Add("debug", ((int)debugLevel).ToString());

            if (readCacheMillisecondDuration > 0)
                attributes.Add("read_cache_ms", readCacheMillisecondDuration.ToString());

            attributes.Add("use_connected_msg", useConnectedMessaging ? "1" : "0");

            string separator = "&";
            return string.Join(separator, attributes.Select(attr => $"{attr.Key}={attr.Value}"));

        }

        public void Dispose() => plctag.destroy(tagHandle);

        void Abort() => plctag.abort(tagHandle);

        public void Read(int millisecondTimeout)
        {
            if (millisecondTimeout <= 0)
                throw new ArgumentOutOfRangeException(nameof(millisecondTimeout), "Must be greater than 0 for a synchronous read");
            var result = (Status)plctag.read(tagHandle, millisecondTimeout);
        }

        public void Write(int millisecondTimeout)
        {
            if (millisecondTimeout <= 0)
                throw new ArgumentOutOfRangeException(nameof(millisecondTimeout), "Must be greater than 0 for a synchronous write");
            plctag.write(tagHandle, millisecondTimeout);
        }


        private TaskCompletionSource<object> readTask;
        public Task ReadAsync(CancellationToken cancellationToken = default)
        {

            using (cancellationToken.Register(() =>
            {
                Abort();
                cancellationToken.ThrowIfCancellationRequested();
            }))
            {
                var initiateReadResult = (Status)plctag.read(tagHandle, 0);
                switch (initiateReadResult)
                {
                    case Status.Ok:
                        readTask = null;
                        return Task.CompletedTask;
                    case Status.Pending:
                        readTask = new TaskCompletionSource<object>();
                        return readTask.Task;
                    default:
                        readTask = null;
                        return Task.FromException(new LibPlcTagException(initiateReadResult));
                }
            }

        }

        private TaskCompletionSource<object> writeTask;
        public Task WriteAsync(CancellationToken cancellationToken = default)
        {

            using (cancellationToken.Register(() =>
            {
                Abort();
                cancellationToken.ThrowIfCancellationRequested();
            }))
            {

                // This statement acts as our thread-safety mechanism
                var initiateWriteResult = (Status)plctag.write(tagHandle, 0);

                switch (initiateWriteResult)
                {
                    case Status.Ok:
                        return Task.CompletedTask;
                    case Status.Pending:
                        writeTask = new TaskCompletionSource<object>();
                        return writeTask.Task;
                    default:
                        return Task.FromException(new LibPlcTagException(initiateWriteResult));
                }

            }

        }

        // These are invoked by the tag event callbacks
        void ReadTaskCompleter(object sender, LibPlcTagEventArgs e)
        {
            switch (e.Status)
            {
                case Status.Ok:
                    readTask?.SetResult(null);
                    break;
                case Status.Pending:
                    // Do nothing, wait for another ReadCompleted callback when Status is Ok.
                    break;
                default:
                    readTask?.SetException(new LibPlcTagException(e.Status));
                    break;
            }
        }

        void WriteTaskCompleter(object sender, LibPlcTagEventArgs e)
        {
            switch (e.Status)
            {
                case Status.Ok:
                    writeTask?.SetResult(null);
                    break;
                case Status.Pending:
                    // Do nothing, wait for another WriteCompleted callback when Status is Ok.
                    break;
                default:
                    writeTask?.SetException(new LibPlcTagException(e.Status));
                    break;
            }
        }


        public int GetSize() => plctag.get_size(tagHandle);

        public Status GetStatus() => (Status)plctag.status(tagHandle);

        public ulong GetUInt64(int offset) => plctag.get_uint64(tagHandle, offset);
        public void SetUInt64(int offset, ulong value) => plctag.set_uint64(tagHandle, offset, value);

        public long GetInt64(int offset) => plctag.get_int64(tagHandle, offset);
        public void SetInt64(int offset, long value) => plctag.set_int64(tagHandle, offset, value);

        public uint GetUInt32(int offset) => plctag.get_uint32(tagHandle, offset);
        public void SetUInt32(int offset, uint value) => plctag.set_uint32(tagHandle, offset, value);

        public int GetInt32(int offset) => plctag.get_int32(tagHandle, offset);
        public void SetInt32(int offset, int value) => plctag.set_int32(tagHandle, offset, value);

        public ushort GetUInt16(int offset) => plctag.get_uint16(tagHandle, offset);
        public void SetUInt16(int offset, ushort value) => plctag.set_uint16(tagHandle, offset, value);

        public short GetInt16(int offset) => plctag.get_int16(tagHandle, offset);
        public void SetInt16(int offset, short value) => plctag.set_int16(tagHandle, offset, value);

        public byte GetUInt8(int offset) => plctag.get_uint8(tagHandle, offset);
        public void SetUInt8(int offset, byte value) => plctag.set_uint8(tagHandle, offset, value);

        public sbyte GetInt8(int offset) => plctag.get_int8(tagHandle, offset);
        public void SetInt8(int offset, sbyte value) => plctag.set_int8(tagHandle, offset, value);

        public double GetFloat64(int offset) => plctag.get_float64(tagHandle, offset);
        public void SetFloat64(int offset, double value) => plctag.set_float64(tagHandle, offset, value);

        public float GetFloat32(int offset) => plctag.get_float32(tagHandle, offset);
        public void SetFloat32(int offset, float value) => plctag.set_float32(tagHandle, offset, value);

        event EventHandler<LibPlcTagEventArgs> ReadStarted;
        event EventHandler<LibPlcTagEventArgs> ReadCompleted;
        event EventHandler<LibPlcTagEventArgs> WriteStarted;
        event EventHandler<LibPlcTagEventArgs> WriteCompleted;
        event EventHandler<LibPlcTagEventArgs> Aborted;
        event EventHandler<LibPlcTagEventArgs> Destroyed;

        protected virtual void OnReadStarted(LibPlcTagEventArgs e)
        {
            EventHandler<LibPlcTagEventArgs> handler = ReadStarted;
            handler?.Invoke(this, e);
        }

        protected virtual void OnReadCompleted(LibPlcTagEventArgs e)
        {
            EventHandler<LibPlcTagEventArgs> handler = ReadCompleted;
            handler?.Invoke(this, e);
        }

        protected virtual void OnWriteStarted(LibPlcTagEventArgs e)
        {
            EventHandler<LibPlcTagEventArgs> handler = WriteStarted;
            handler?.Invoke(this, e);
        }

        protected virtual void OnWriteCompleted(LibPlcTagEventArgs e)
        {
            EventHandler<LibPlcTagEventArgs> handler = WriteCompleted;
            handler?.Invoke(this, e);
        }

        protected virtual void OnAborted(LibPlcTagEventArgs e)
        {
            EventHandler<LibPlcTagEventArgs> handler = Aborted;
            handler?.Invoke(this, e);
        }

        protected virtual void OnDestroyed(LibPlcTagEventArgs e)
        {
            EventHandler<LibPlcTagEventArgs> handler = Destroyed;
            handler?.Invoke(this, e);
        }

        void coreLibEventCallback(int tagPointer, int eventCode, int statusCode)
        {

            //Debug.WriteLine($"{tagPointer} {(Event)eventCode} {(Status)statusCode}");

            // Only proceed if this callback was triggered for this tag
            // This should not occur because the callback is registered per tag
            //if (tagPointer != tagHandle)
                //return;

            // Core library is sensitive to delays so invoke event handlers on a different thread
            Task.Run(() =>
            {
                switch ((Event)eventCode)
                {
                    case Event.ReadCompleted:
                        OnReadCompleted(new LibPlcTagEventArgs() { Status = (Status)statusCode });
                        break;
                    case Event.ReadStarted:
                        OnReadStarted(new LibPlcTagEventArgs() { Status = (Status)statusCode });
                        break;
                    case Event.WriteStarted:
                        OnWriteStarted(new LibPlcTagEventArgs() { Status = (Status)statusCode });
                        break;
                    case Event.WriteCompleted:
                        OnWriteCompleted(new LibPlcTagEventArgs() { Status = (Status)statusCode });
                        break;
                    case Event.Aborted:
                        OnAborted(new LibPlcTagEventArgs() { Status = (Status)statusCode });
                        break;
                    case Event.Destroyed:
                        OnDestroyed(new LibPlcTagEventArgs() { Status = (Status)statusCode });
                        break;
                    default:
                        throw new NotImplementedException();
                }
            });
        }

    }

}