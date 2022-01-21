using System.Runtime.InteropServices;

namespace ZlibNGSharpMinimal;

/// <summary>
/// Contains interop functions for zlib-ng's core functionalities.
/// </summary>
internal static unsafe class ZngInterop
{
    /// <summary>
    /// Gets the name of the zlib-ng library.
    /// </summary>
    public const string LibraryName = "zlib-ng2";

    /// <summary>
    /// Gets a pointer to the ANSI version string of the underlying zlib-ng library.
    /// </summary>
    /// <returns>A pointer to the version string.</returns>
    [DllImport(LibraryName)]
    public static extern byte* zlibng_version();
}
