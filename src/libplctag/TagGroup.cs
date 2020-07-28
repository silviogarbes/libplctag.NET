using libplctag.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace libplctag
{
    public class TagGroup
    {
        private readonly List<ITag> _tags = new List<ITag>();

        public void Add(ITag tag) => _tags.Add(tag);
        public void Remove(ITag tag) => _tags.Remove(tag);

        public Tag CreateTag(string name, int? elementSize = null, AttributeGroup attributeGroup = default)
        {

            var newTag = new Tag(attributeGroup)
            {
                Name = name,
                ElementSize = elementSize
            };

            _tags.Add(newTag);

            return newTag;

        }

        public GenericTag<TPlcType, TDotNetType> CreateGenericTag<TPlcType, TDotNetType>(string name, AttributeGroup attributeGroup = default)
             where TPlcType : IPlcType<TDotNetType>, new()
        {

            var newTag = new GenericTag<TPlcType, TDotNetType>(attributeGroup)
            {
                Name = name
            };

            _tags.Add(newTag);

            return newTag;

        }

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
