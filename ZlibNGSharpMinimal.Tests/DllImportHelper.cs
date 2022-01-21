using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace ZlibNGSharpMinimal.Tests;

public static class DllImportHelper
{
    public static IntPtr ZngDllImportResolver(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
    {
        const string nativeFolder = "native";
        string libPath = "runtimes";

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            libPath = Path.Combine(libPath, "win-x64", nativeFolder, libraryName + ".dll");
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            libPath = Path.Combine(libPath, "linux-x64", nativeFolder, libraryName + ".so");
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            libPath = Path.Combine(libPath, "osx-x64", nativeFolder, libraryName + ".dylib");

        if (NativeLibrary.TryLoad(libPath, out IntPtr libPtr))
            return libPtr;

        return NativeLibrary.Load(libraryName, assembly, searchPath);
    }

    public static void SetupZngResolver()
        => NativeLibrary.SetDllImportResolver(typeof(Zng).Assembly, ZngDllImportResolver);
}
