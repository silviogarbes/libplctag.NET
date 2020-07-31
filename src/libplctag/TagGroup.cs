using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace libplctag
{
    /// <summary>
    /// Provides simultaneous read/write functionality for a collection of tags.
    /// </summary>
    public class TagGroup : IEnumerable<ITag>
    {

        private readonly List<ITag> _tags = new List<ITag>();

        public void Add(ITag tag) => _tags.Add(tag);
        public void Remove(ITag tag) => _tags.Remove(tag);
        public void Clear() => _tags.Clear();

        public IEnumerator<ITag> GetEnumerator() => _tags.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();


        public void ReadAll(int millisecondTimeout) => Task.WaitAll(_tags.Select(t => t.ReadAsync(millisecondTimeout)).ToArray());
        public async Task ReadAllAsync(int millisecondTimeout, CancellationToken token = default) => await Task.WhenAll(_tags.Select(t => t.ReadAsync(millisecondTimeout, token)).ToArray());
        public async Task ReadAllAsync(CancellationToken token = default) => await Task.WhenAll(_tags.Select(t => t.ReadAsync(token)).ToArray());


        public void WriteAll(int millisecondTimeout) => Task.WaitAll(_tags.Select(t => t.WriteAsync(millisecondTimeout)).ToArray());
        public async Task WriteAllAsync(int millisecondTimeout, CancellationToken token = default) => await Task.WhenAll(_tags.Select(t => t.WriteAsync(millisecondTimeout, token)).ToArray());
        public async Task WriteAllAsync(CancellationToken token = default) => await Task.WhenAll(_tags.Select(t => t.WriteAsync(token)).ToArray());


        public void InitializeAll(int millisecondTimeout) => Task.WaitAll(_tags.Select(t => t.InitializeAsync(millisecondTimeout)).ToArray());
        public async Task InitializeAllAsync(int millisecondTimeout, CancellationToken token = default) => await Task.WhenAll(_tags.Select(t => t.InitializeAsync(millisecondTimeout, token)).ToArray());
        public async Task InitializeAllAsync(CancellationToken token = default) => await Task.WhenAll(_tags.Select(t => t.InitializeAsync(token)).ToArray());


    }
}
