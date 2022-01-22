# ZlibNGSharpMinimal

A .NET 6 C# interop wrapper for [zlib-ng's](https://github.com/zlib-ng/zlib-ng) basic inflate and deflate operations.
Written for and bundled with zlib-ng v2.0.6, for `win-x64` and `linux-x64`.

[![Nuget | carlst99.ZlibNGSharp](https://img.shields.io/nuget/v/carlst99.ZlibNGSharp?label=NuGet%20|%20carlst99.ZlibNGSharp)](https://www.nuget.org/packages/carlst99.ZlibNGSharp)\
[![Nuget | carlst99.ZlibNGSharp.Native](https://img.shields.io/nuget/v/carlst99.ZlibNGSharp.Native?label=NuGet%20|%20carlst99.ZlibNGSharp.Native)](https://www.nuget.org/packages/carlst99.ZlibNGSharp.Native)

```csharp
byte[] data = new byte[512];
byte[] deflated = new byte[Data.Length];
byte[] inflated = new byte[Data.Length];

for (int i = 0; i < data.Length; i++)
    data[i] = (byte)(i % 8);

using ZngDeflater deflater = new();
deflater.Deflate(Data, deflated);

using ZngInflater inflater = new();
inflater.Inflate(deflated, inflated);
```

## Installation

You can install ZlibNGSharpMinimal from NuGet.

```sh
# .NET CLI
dotnet add package carlst99.ZlibNGSharp

# Visual Studio Package Manager
Install-Package carlst99.ZlibNGSharp
```