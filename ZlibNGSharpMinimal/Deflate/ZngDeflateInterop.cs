using System;
using System.Runtime.InteropServices;

namespace ZlibNGSharpMinimal.Deflate;

/// <summary>
/// Contains interop functions for zlib-ng's deflate functionalities.
/// </summary>
internal static unsafe class ZngDeflateNative
{
    /// <summary>
    /// Initializes the internal stream state for compression.
    /// The fields <see cref="ZngStream.AllocationFunction"/>, <see cref="ZngStream.FreeFunction"/>
    /// and <see cref="ZngStream.Opaque"/> must be initialized before by the caller.
    /// If <see cref="ZngStream.AllocationFunction"/> and <see cref="ZngStream.FreeFunction"/> are set to <see cref="IntPtr.Zero"/>,
    /// deflateInit updates them to use default allocation functions.
    /// </summary>
    /// <param name="stream">The stream to initialize.</param>
    /// <param name="level">The compression level to operate at.</param>
    /// <returns>
    /// <see cref="CompressionResult.OK"/> on success,
    /// <see cref="CompressionResult.MemoryError"/> if there was not enough memory,
    /// <see cref="CompressionResult.StreamEnd"/> if the level is not valid,
    /// or <see cref="CompressionResult.VersionError"/> if the zlib library version is incompatible with the caller's assumed version.
    /// </returns>
    [DllImport(ZngInterop.LibraryName)]
    public static extern CompressionResult zng_deflateInit(ZngStream* stream, CompressionLevel level);

    [DllImport(ZngInterop.LibraryName)]
    public static extern CompressionResult zng_deflate(ZngStream* stream, DeflateFlushMethod flushMethod);

    /// <summary>
    /// Frees any dynamically allocated data structures for the given stream.
    /// This function discards any unprocessed input and does not flush any pending output.
    /// </summary>
    /// <param name="stream">The stream to free.</param>
    /// <returns>
    /// <see cref="CompressionResult.OK"/> on success,
    /// <see cref="CompressionResult.DataError"/> if the stream was freed prematurely (some input or output was discarded).
    /// or <see cref="CompressionResult.StreamError"/> if the stream state was inconsistent.
    /// </returns>
    [DllImport(ZngInterop.LibraryName)]
    public static extern CompressionResult zng_deflateEnd(ZngStream* stream);

    /// <summary>
    /// This function is equivalent to <see cref="zng_deflateEnd(ZngStream*)"/>
    /// followed by <see cref="zng_deflateInit"/>,
    /// but does not free and reallocate the internal decompression state.
    /// The stream will keep attributes that may have been set by <see cref="zng_deflateInit"/>.
    /// </summary>
    /// <param name="stream">The stream to reset.</param>
    /// <returns>
    /// <see cref="CompressionResult.OK"/> on success,
    /// or <see cref="CompressionResult.StreamError"/> if the stream state was inconsistent.
    /// </returns>
    [DllImport(ZngInterop.LibraryName)]
    public static extern CompressionResult zng_deflateReset(ZngStream* stream);
}
