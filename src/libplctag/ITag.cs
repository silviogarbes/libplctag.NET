using System;
using System.Threading;
using System.Threading.Tasks;

namespace libplctag
{
    public interface ITag : IDisposable
    {

        void Dispose();

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