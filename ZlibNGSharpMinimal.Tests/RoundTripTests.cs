using Xunit;
using ZlibNGSharpMinimal.Deflate;
using ZlibNGSharpMinimal.Inflate;

namespace ZlibNGSharpMinimal.Tests;

public class RoundTripTests
{
    private readonly byte[] Data;

    static RoundTripTests()
    {
        DllImportHelper.SetupZngResolver();
    }

    public RoundTripTests()
    {
        Data = new byte[512];

        for (int i = 0; i < Data.Length; i++)
            Data[i] = (byte)(i % 8);
    }

    [Fact]
    public void TestRoundTrip()
    {
        byte[] deflated = new byte[Data.Length];
        byte[] inflated = new byte[Data.Length];

        using ZngDeflater deflater = new();
        deflater.Deflate(Data, deflated);

        using ZngInflater inflater = new();
        inflater.Inflate(deflated, inflated);

        Assert.Equal(Data, inflated);
    }
}
