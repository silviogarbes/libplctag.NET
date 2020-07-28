using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace libplctag
{
    public interface ITag
    {
        PlcType? PlcType { get; set; }
        int? ElementCount { get; set; }
        int? ElementSize { get; set; }
        string Gateway { get; set; }
        string Name { get; set; }
        string Path { get; set; }
        Protocol? Protocol { get; set; }
        int? ReadCacheMillisecondDuration { get; set; }
        bool? UseConnectedMessaging { get; set; }

        void Abort();
        void Dispose();
        int GetSize();
        Status GetStatus();

        void Read(int millisecondTimeout);
        Task ReadAsync(int millisecondTimeout, CancellationToken token = default);
        Task ReadAsync(CancellationToken token = default);

        void Write(int millisecondTimeout);
        Task WriteAsync(int millisecondTimeout, CancellationToken token = default);
        Task WriteAsync(CancellationToken token = default);

        void Initialize(int millisecondTimeout);
        Task InitializeAsync(int millisecondTimeout, CancellationToken token = default);
        Task InitializeAsync(CancellationToken token = default);
    }
}