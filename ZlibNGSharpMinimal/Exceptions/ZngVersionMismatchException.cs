using System;

namespace ZlibNGSharpMinimal.Exceptions;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "RCS1194:Implement exception constructors.")]
public class ZngVersionMismatchException : Exception
{
    public byte ExpectedMajorVersion { get; }
    public string? ActualVersion { get; }

    public ZngVersionMismatchException(byte expectedMajorVersion, string? actualVersion)
    {
        ExpectedMajorVersion = expectedMajorVersion;
        ActualVersion = actualVersion;
    }

    public override string ToString()
        => $"Expected Major Version: {(char)ExpectedMajorVersion} | Actual Version: {ActualVersion}";
}
