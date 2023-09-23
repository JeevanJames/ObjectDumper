using System.Collections;

namespace Jeevan.ObjectDumper;

internal static class TypeExtensions
{
    internal static bool IsCollection(this Type type, out Type elementType)
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
