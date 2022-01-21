using System;
using System.Runtime.InteropServices;
using ZlibNGSharpMinimal.Exceptions;

namespace ZlibNGSharpMinimal;

public static unsafe class Zng
{
    /// <summary>
    /// Gets the major version of zlib-ng that this wrapper was based on.
    /// </summary>
    private const byte ExpectedMajorVersion = (byte)'2';

    /// <summary>
    /// Gets the version of the underlying zlib-ng library.
    /// </summary>
    public static readonly string? Version = Marshal.PtrToStringAnsi((IntPtr)ZngInterop.zlibng_version());

    /// <summary>
    /// Throws a <see cref="ZngVersionMismatchException"/> if the major version of the
    /// underlying zlib-ng library is not suitable for this wrapper.
    /// </summary>
    /// <exception cref="ZngVersionMismatchException"></exception>
    public static void ThrowIfVersionMismatch()
    {
        if (*ZngInterop.zlibng_version() != ExpectedMajorVersion)
            throw new ZngVersionMismatchException(ExpectedMajorVersion, Version);
    }
}
