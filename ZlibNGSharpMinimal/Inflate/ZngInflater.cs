using System;
using System.Runtime.InteropServices;
using ZlibNGSharpMinimal.Exceptions;

namespace ZlibNGSharpMinimal.Inflate;

/// <summary>
/// Represents an inflater utilising the zlib-ng algorithm.
/// </summary>
public unsafe class ZngInflater : IDisposable
{
    private readonly ZngStream* _streamPtr;

    /// <summary>
    /// Gets or sets a value indicating whether or not this instance has been disposed.
    /// </summary>
    public bool IsDisposed { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ZngInflater"/> class.
    /// </summary>
    /// <exception cref="ZngCompressionException"></exception>
    public ZngInflater()
    {
        ZngStream stream = new()
        {
            AllocationFunction = null,
            FreeFunction = null,
            Opaque = IntPtr.Zero,
        };

        _streamPtr = (ZngStream*)NativeMemory.Alloc((nuint)sizeof(ZngStream));
        Marshal.StructureToPtr(stream, (IntPtr)_streamPtr, false);

        CompressionResult initResult = ZngInflateNative.zng_inflateInit_(_streamPtr, ZngInterop.zlibng_version(), sizeof(ZngStream));
        if (initResult is not CompressionResult.OK)
            GenerateCompressionError(initResult, "Failed to initialize");
    }

    /// <summary>
    /// Inflates a compressed buffer. The buffer should contain the complete deflated sequence.
    /// </summary>
    /// <param name="input">The compressed buffer.</param>
    /// <param name="output">The output buffer.</param>
    /// <exception cref="ZngCompressionException"></exception>
    public ulong Inflate(ReadOnlySpan<byte> input, Span<byte> output)
    {
        Checks();

        fixed (byte* nextIn = input)
        {
            fixed (byte* nextOut = output)
            {
                _streamPtr->NextIn = nextIn;
                _streamPtr->AvailableIn = (uint)input.Length;

                _streamPtr->NextOut = nextOut;
                _streamPtr->AvailableOut = (uint)output.Length;

                CompressionResult inflateResult = ZngInflateNative.zng_inflate(_streamPtr, InflateFlushMethod.Finish);

                if (inflateResult is not CompressionResult.StreamEnd)
                    GenerateCompressionError(inflateResult, "Failed to inflate");
            }
        }

        return _streamPtr->TotalOut;
    }

    /// <summary>
    /// Resets the internal state of the inflater.
    /// </summary>
    /// <exception cref="ZngCompressionException"></exception>
    public void Reset()
    {
        Checks();

        _streamPtr->NextIn = null;
        _streamPtr->AvailableIn = 0;

        _streamPtr->NextOut = null;
        _streamPtr->AvailableOut = 0;

        CompressionResult result = ZngInflateNative.zng_inflateReset(_streamPtr);
        if (result is not CompressionResult.OK)
            GenerateCompressionError(result, "Failed to reset inflater");
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes of managed and unmanaged resources.
    /// </summary>
    /// <param name="disposeManaged">A value indicating whether or not to dispose of managed resources.</param>
    protected virtual void Dispose(bool disposeManaged)
    {
        if (IsDisposed)
            return;

        ZngInflateNative.zng_inflateEnd(_streamPtr);
        Marshal.DestroyStructure<ZngStream>((IntPtr)_streamPtr);
        NativeMemory.Free(_streamPtr);

        IsDisposed = true;
    }

    private void Checks()
    {
        Zng.ThrowIfVersionMismatch();

        if (IsDisposed)
            throw new ObjectDisposedException(nameof(ZngInflater));
    }

    private void GenerateCompressionError(CompressionResult result, string genericMessage)
    {
        string? msg = _streamPtr->ErrorMessage != null
                ? Marshal.PtrToStringAnsi((IntPtr)_streamPtr->ErrorMessage)
                : genericMessage;

        throw new ZngCompressionException(result, msg);
    }

    ~ZngInflater()
    {
        Dispose();
    }
}
