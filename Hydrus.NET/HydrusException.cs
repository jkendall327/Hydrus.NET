namespace Hydrus.NET;

using System;

public sealed class HydrusException : Exception
{
    public string HydrusExceptionType { get; }
    public string HydrusError { get; }

    public HydrusException(string hydrusExceptionType, string hydrusError) : base(hydrusError)
    {
        HydrusExceptionType = hydrusExceptionType;
        HydrusError = hydrusError;
    }

    public HydrusException(string hydrusExceptionType, string hydrusError, Exception innerException)
        : base(hydrusError, innerException)
    {
        HydrusExceptionType = hydrusExceptionType;
        HydrusError = hydrusError;
    }

    public override string ToString() => $"{HydrusExceptionType}: {HydrusError}";
}
