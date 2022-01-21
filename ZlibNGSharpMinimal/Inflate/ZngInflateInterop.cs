using System;
using System.Runtime.InteropServices;

namespace ZlibNGSharpMinimal.Inflate;

/// <summary>
/// Contains interop functions for zlib-ng's inflate functionalities.
/// </summary>
internal static unsafe class ZngInflateNative
{
    /// <summary>
    /// Initializes the internal stream state for decompression.
    /// The fields <see cref="ZngStream.AllocationFunction"/>, <see cref="ZngStream.FreeFunction"/>
    /// and <see cref="ZngStream.Opaque"/> must be initialized before by the caller.
    /// If <see cref="ZngStream.AllocationFunction"/> and <see cref="ZngStream.FreeFunction"/> are set to <see cref="IntPtr.Zero"/>,
    /// inflateInit updates them to use default allocation functions.
    /// </summary>
    /// <param name="stream">The stream to initialize.</param>
    /// <param name="version">The expected version of the zlib library.</param>
    /// <param name="streamSize">The size of the <see cref="ZngStream"/> object.</param>
    /// <returns>
    /// <see cref="CompressionResult.OK"/> on success,
    /// <see cref="CompressionResult.MemoryError"/> if there was not enough memory,
    /// <see cref="CompressionResult.VersionError"/> if the zlib library version is incompatible with the caller's assumed version,
    /// or <see cref="CompressionResult.StreamError"/> if the parameters are invalid.
    /// </returns>
    [DllImport(ZngInterop.LibraryName)]
    public static extern CompressionResult zng_inflateInit_(ZngStream* stream, byte* version, int streamSize);

    [DllImport(ZngInterop.LibraryName)]
    public static extern CompressionResult zng_inflate(ZngStream* stream, InflateFlushMethod flushMethod);

    /// <summary>
    /// Frees any dynamically allocated data structures for the given stream.
    /// This function discards any unprocessed input and does not flush any pending output.
    /// </summary>
    /// <param name="stream">The stream to free.</param>
    /// <returns>
    /// <see cref="CompressionResult.OK"/> on success,
    /// or <see cref="CompressionResult.StreamError"/> if the stream state was inconsistent.
    /// </returns>
    [DllImport(ZngInterop.LibraryName)]
    public static extern CompressionResult zng_inflateEnd(ZngStream* stream);

    /// <summary>
    /// This function is equivalent to <see cref="zng_inflateEnd(ZngStream*)"/>
    /// followed by <see cref="zng_inflateInit_(ZngStream*, byte*, int)"/>,
    /// but does not free and reallocate the internal decompression state.
    /// The stream will keep attributes that may have been set by inflateInit2.
    /// </summary>
    /// <param name="stream">The stream to reset.</param>
    /// <returns>
    /// <see cref="CompressionResult.OK"/> on success,
    /// or <see cref="CompressionResult.StreamError"/> if the stream state was inconsistent.
    /// </returns>
    [DllImport(ZngInterop.LibraryName)]
    public static extern CompressionResult zng_inflateReset(ZngStream* stream);
}
