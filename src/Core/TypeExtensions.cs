using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Jeevan.ObjectDumper;

internal static class TypeExtensions
{
    /// <summary>
    ///     Returns whether a type is a collection, i.e. it implements the <see cref="IEnumerable"/>
    ///     or <see cref="IEnumerable{T}"/> interface.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <param name="elementType">The type of the collection element.</param>
    /// <returns><c>True</c> if the type is a collection, otherwise <c>false</c>.</returns>
    internal static bool IsCollection(this Type type, [NotNullWhen(true)] out Type? elementType)
    {
        // Exclude strings, as they are considered value types.
        if (type == typeof(string))
        {
            elementType = typeof(char);
            return false;
        }

        Type[] intfTypes = type.GetInterfaces();

        // Check for IEnumerable<T>
        foreach (Type intfType in intfTypes)
        {
            if (intfType.IsGenericType && intfType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                elementType = intfType.GetGenericArguments()[0];
                return true;
            }
        }

        // Check for IEnumerable
        bool isCollection = Array.Exists(intfTypes, t => typeof(IEnumerable).IsAssignableFrom(t));
        elementType = isCollection ? typeof(object) : null;
        return isCollection;
    }

    /// <summary>
    ///     Returns whether a type is a dictionary.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <param name="keyType">The type of the dictionary key.</param>
    /// <param name="valueType">The type of the dictionary value.</param>
    /// <returns><c>True</c> if the type is a dictionary, otherwise <c>false</c>.</returns>
    /// <remarks>
    ///     Since <see cref="IDictionary{TKey,TValue}"/> does not inherit from <see cref="IDictionary"/>,
    ///     the logic here is slightly different from the <see cref="IsCollection"/> method.
    ///     <br/>
    ///     The type is considered a dictionary if it implements the non-generic <see cref="IDictionary"/>
    ///     interface. However, we still want to check whether it implements the generic interface
    ///     so that we can properly assign the <paramref name="keyType"/> and <paramref name="valueType"/>
    ///     parameters. If the generic type is not implemented, then the key and value types are
    ///     <see cref="object"/>.
    /// </remarks>
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
            // Check if the generic dictionary interface is implemented. If it is, we can assign the
            // key and value types, but this is still not considered to be a dictionary.
            if (intfType.IsGenericType && intfType.GetGenericTypeDefinition() == typeof(IDictionary<,>))
            {
                Type[] genericArguments = intfType.GetGenericArguments();
                keyType = genericArguments[0];
                valueType = genericArguments[1];
                foundGeneric = true;
            }

            // Check if non-generic IDictionary interface is implemented. If it is, then the type
            // is a dictionary.
            foundDictionary = Array.Exists(intfTypes, t => typeof(IDictionary).IsAssignableFrom(t));

            // We only exit the loop if both the generic and non-generic interfaces are implemented.
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
