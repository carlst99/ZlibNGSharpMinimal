using System;
using System.Runtime.InteropServices;
using ZlibNGSharpMinimal.Exceptions;

namespace ZlibNGSharpMinimal.Deflate;

/// <summary>
/// Represents an interface to zlib-ng's deflation algorithm.
/// </summary>
public unsafe class ZngDeflater : IDisposable
{
    private readonly ZngStream* _streamPtr;

    /// <summary>
    /// Gets or sets a value indicating whether or not this <see cref="ZngDeflater"/> instance has been disposed.
    /// </summary>
    public bool IsDisposed { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ZngDeflater"/> class.
    /// </summary>
    /// <exception cref="ZngCompressionException"></exception>
    public ZngDeflater(CompressionLevel compressionLevel = CompressionLevel.BestCompression)
    {
        ZngStream stream = new()
        {
            AllocationFunction = null,
            FreeFunction = null,
            Opaque = IntPtr.Zero,
        };

        _streamPtr = (ZngStream*)NativeMemory.Alloc((nuint)sizeof(ZngStream));
        Marshal.StructureToPtr(stream, (IntPtr)_streamPtr, false);

        CompressionResult initResult = ZngDeflateNative.zng_deflateInit_(_streamPtr, compressionLevel, ZngInterop.zlibng_version(), sizeof(ZngStream));
        if (initResult is not CompressionResult.OK)
            GenerateCompressionError(initResult, "Failed to initialize deflater");
    }

    /// <summary>
    /// Deflates a buffer.
    /// </summary>
    /// <param name="input">The buffer.</param>
    /// <param name="output">The output buffer.</param>
    /// <param name="flushMethod">The flush method to use.</param>
    /// <returns>The number of deflated bytes that were produced.</returns>
    /// <exception cref="ZngCompressionException"></exception>
    public ulong Deflate(ReadOnlySpan<byte> input, Span<byte> output, DeflateFlushMethod flushMethod = DeflateFlushMethod.Finish)
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

                CompressionResult deflateResult = ZngDeflateNative.zng_deflate(_streamPtr, flushMethod);
                if (deflateResult is not CompressionResult.StreamEnd)
                    GenerateCompressionError(deflateResult, "Failed to inflate");
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

        _streamPtr->TotalOut = UIntPtr.Zero;

        CompressionResult result = ZngDeflateNative.zng_deflateReset(_streamPtr);
        if (result is not CompressionResult.OK)
            GenerateCompressionError(result, "Failed to reset deflater");
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

        ZngDeflateNative.zng_deflateEnd(_streamPtr);
        Marshal.DestroyStructure<ZngStream>((IntPtr)_streamPtr);
        NativeMemory.Free(_streamPtr);

        IsDisposed = true;
    }

    private void Checks()
    {
        Zng.ThrowIfVersionMismatch();

        if (IsDisposed)
            throw new ObjectDisposedException(nameof(ZngDeflater));
    }

    private void GenerateCompressionError(CompressionResult result, string genericMessage)
    {
        string? msg = _streamPtr->ErrorMessage != null
                ? Marshal.PtrToStringAnsi((IntPtr)_streamPtr->ErrorMessage)
                : genericMessage;

        throw new ZngCompressionException(result, msg);
    }

    ~ZngDeflater()
    {
        Dispose(false);
    }
}
