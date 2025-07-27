namespace Hydrus.NET;

using System;

/// <summary>
/// Represents an error returned by the Hydrus API.
/// </summary>
public sealed class HydrusException : Exception
{
    /// <summary>
    /// Represents the kind of the exception.
    /// </summary>
    public string Type { get; }
    
    /// <summary>
    /// Contains the error's message.
    /// </summary>
    public string HydrusError { get; }

    /// <summary>
    /// Creates a new instance of the exception.
    /// </summary>
    /// <param name="type">The type of the error.</param>
    /// <param name="hydrusError">The error message.</param>
    public HydrusException(string type, string hydrusError) : base(hydrusError)
    {
        Type = type;
        HydrusError = hydrusError;
    }

    /// <summary>
    /// <inheritdoc cref="Object.ToString()"/>
    /// </summary>
    public override string ToString() => $"{Type}: {HydrusError}";
}
