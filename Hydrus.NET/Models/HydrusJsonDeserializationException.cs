namespace Hydrus.NET;

public class HydrusJsonDeserializationException(Type type)
    : Exception($"Failed to deserialize response to type {type.Name}");