﻿using libplctag.DataTypes;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace libplctag
{
    /// <summary>
    /// A class that allows for strongly-typed objects tied to PLC tags
    /// </summary>
    /// <typeparam name="M">A <see cref="IPlcMapper{T}"/> class that handles data conversion</typeparam>
    /// <typeparam name="T">The desired C# type of Tag.Value</typeparam>
    public class Tag<M, T> : IDisposable, ITag where M : IPlcMapper<T>, new()
    {

        private readonly Tag _tag;
        private readonly IPlcMapper<T> _plcMapper;

        public Tag()
        {
            _plcMapper = new M();
            _tag = new Tag()
            {
                ElementSize = _plcMapper.ElementSize,
            };
        }

        /// <inheritdoc cref="Tag.Protocol"/>
        public Protocol? Protocol
        {
            get => _tag.Protocol;
            set => _tag.Protocol = value;
        }

        /// <inheritdoc cref="Tag.Gateway"/>
        public string Gateway
        {
            get => _tag.Gateway;
            set => _tag.Gateway = value;
        }

        /// <inheritdoc cref="Tag.Path"/>
        public string Path
        {
            get => _tag.Path;
            set => _tag.Path = value;
        }

        /// <inheritdoc cref="Tag.PlcType"/>
        public PlcType? PlcType
        {
            get => _tag.PlcType;
            set => _tag.PlcType = value;
        }

        /// <inheritdoc cref="Tag.Name"/>
        public string Name
        {
            get => _tag.Name;
            set => _tag.Name = value;
        }

        /// <inheritdoc cref="Tag.UseConnectedMessaging"/>
        public bool? UseConnectedMessaging
        {
            get => _tag.UseConnectedMessaging;
            set => _tag.UseConnectedMessaging = value;
        }

        /// <inheritdoc cref="Tag.ReadCacheMillisecondDuration"/>
        public int? ReadCacheMillisecondDuration
        {
            get => _tag.ReadCacheMillisecondDuration;
            set => _tag.ReadCacheMillisecondDuration = value;
        }

        /// <inheritdoc cref="Tag.Timeout"/>
        public TimeSpan Timeout
        {
            get => _tag.Timeout;
            set => _tag.Timeout = value;
        }

        /// <summary>
        /// Dimensions of Value if it is an array
        /// Ex. {2, 10} for a 2 column, 10 row array
        /// Non-arrays can use null (default)
        /// </summary>
        public int[] ArrayDimensions
        {
            get => _plcMapper.ArrayDimensions;
            set
            {
                _plcMapper.ArrayDimensions = value;
                _tag.ElementCount = _plcMapper.GetElementCount();
            }
        }

        /// <inheritdoc cref="Tag.Initialize"/>
        public void Initialize()
        {
            _tag.Initialize();
            DecodeAll();
        }

        /// <inheritdoc cref="Tag.InitializeAsync"/>
        public async Task InitializeAsync(CancellationToken token = default)
        {
            await _tag.InitializeAsync(token);
            DecodeAll();
        }

        /// <inheritdoc cref="Tag.ReadAsync"/>
        public async Task ReadAsync(CancellationToken token = default)
        {
            await _tag.ReadAsync(token);
            DecodeAll();
        }

        /// <inheritdoc cref="Tag.Read"/>
        public void Read()
        {
            _tag.Read();
            DecodeAll();
        }

        /// <inheritdoc cref="Tag.WriteAsync"/>
        public Task WriteAsync(CancellationToken token = default)
        {
            EncodeAll();
            return _tag.WriteAsync(token);
        }

        /// <inheritdoc cref="Tag.Write"/>
        public void Write()
        {
            EncodeAll();
            _tag.Write();
        }

        void DecodeAll()
        {
            Value = _plcMapper.Decode(_tag);
        }

        void EncodeAll()
        {
            _plcMapper.Encode(_tag, Value);
        }

        /// <inheritdoc cref="Tag.GetStatus"/>
        public Status GetStatus() => _tag.GetStatus();

        public void Dispose() => _tag.Dispose();

        ~Tag()
        {
            Dispose();
        }

        /// <summary>
        /// The local memory value that can be transferred to/from the PLC
        /// </summary>
        public T Value { get; set; }

    }
}
