# ZlibNGSharpMinimal

A .NET 6 C# interop wrapper for [zlib-ng's](https://github.com/zlib-ng/zlib-ng) basic inflate and deflate operations.
Written for and bundled with zlib-ng v2.0.6.

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