using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Jeevan.ObjectDumper;

internal static class TypeExtensions
{
    internal static bool IsCollection(this Type type, [NotNullWhen(true)] out Type? elementType)
    {
        // Exclude strings, as they are considered value types.
        if (type == typeof(string))
        {
            elementType = typeof(char);
            return false;
        }

        Type[] intfTypes = type.GetInterfaces();

        foreach (Type intfType in intfTypes)
        {
            if (intfType.IsGenericType && intfType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                elementType = intfType.GetGenericArguments()[0];
                return true;
            }
        }

        elementType = typeof(object);
        return Array.Exists(intfTypes, t => typeof(IEnumerable).IsAssignableFrom(t));
    }

    internal static bool IsDictionary(this Type type, [NotNullWhen(true)] out Type? keyType,
        [NotNullWhen(true)] out Type? valueType)
    {
        keyType = null;
        valueType = null;

        Type[] intfTypes = type.GetInterfaces();

        bool foundGeneric = false;
        bool foundDictionary = false;

        foreach (Type intfType in intfTypes)
        {
            if (intfType.IsGenericType && intfType.GetGenericTypeDefinition() == typeof(IDictionary<,>))
            {
                Type[] genericArguments = intfType.GetGenericArguments();
                keyType = genericArguments[0];
                valueType = genericArguments[1];
                foundGeneric = true;
            }

            foundDictionary = Array.Exists(intfTypes, t => typeof(IDictionary).IsAssignableFrom(t));

            if (foundGeneric && foundDictionary)
                break;
        }

        if (!foundDictionary)
            return false;

        if (!foundGeneric)
        {
            keyType = typeof(object);
            valueType = typeof(object);
        }

        Debug.Assert(keyType is not null, "Dictionary key type was null.");
        Debug.Assert(valueType is not null, "Dictionary value type was null.");
        return true;
    }

    /// <summary>
    ///     Returns whether this type has a value and we don't need to drill into it further.
    ///     <br/>
    ///     This includes:<br/>
    ///     o Built-in primitive types like int, double, char, etc. (Type.IsPrimitive == true)<br/>
    ///     o Enums<br/>
    ///     o Strings<br/>
    ///     o Any type that implements <see cref="IFormattable"/>. This will cover most non-built-in
    ///         primitive types like <see cref="Guid"/>, <see cref="DateTime"/>, etc.
    /// </summary>
    internal static bool IsValue(this Type type) =>
        type.IsPrimitive ||
        type.IsEnum ||
        type == typeof(string) ||
        typeof(IFormattable).IsAssignableFrom(type);

    internal static Type GetActualType(this Type type) =>
        Nullable.GetUnderlyingType(type) ?? type;
}
