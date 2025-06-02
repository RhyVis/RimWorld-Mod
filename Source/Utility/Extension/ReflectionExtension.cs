using System.Reflection;

namespace Overclock.Utility.Extension;

public static class ReflectionExtension
{
    public static T GetPrivateField<T>(this object obj, string fieldName)
    {
        if (obj == null)
            throw new ArgumentNullException(nameof(obj));
        if (string.IsNullOrEmpty(fieldName))
            throw new ArgumentNullException(nameof(fieldName));

        var field =
            obj.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance)
            ?? throw new ArgumentException(
                $"Field '{fieldName}' not found in type '{obj.GetType().FullName}'."
            );
        return (T)field.GetValue(obj);
    }

    public static void SetPrivateField<T>(this object obj, string fieldName, T value)
    {
        if (obj == null)
            throw new ArgumentNullException(nameof(obj));
        if (string.IsNullOrEmpty(fieldName))
            throw new ArgumentNullException(nameof(fieldName));

        var field =
            obj.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance)
            ?? throw new ArgumentException(
                $"Field '{fieldName}' not found in type '{obj.GetType().FullName}'."
            );
        field.SetValue(obj, value);
    }

    public static T GetPrivateProperty<T>(this object obj, string propertyName)
    {
        if (obj == null)
            throw new ArgumentNullException(nameof(obj));
        if (string.IsNullOrEmpty(propertyName))
            throw new ArgumentNullException(nameof(propertyName));

        var property =
            obj.GetType().GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Instance)
            ?? throw new ArgumentException(
                $"Property '{propertyName}' not found in type '{obj.GetType().FullName}'."
            );
        return (T)property.GetValue(obj);
    }

    public static void SetPrivateProperty<T>(this object obj, string propertyName, T value)
    {
        if (obj == null)
            throw new ArgumentNullException(nameof(obj));
        if (string.IsNullOrEmpty(propertyName))
            throw new ArgumentNullException(nameof(propertyName));

        var property =
            obj.GetType().GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Instance)
            ?? throw new ArgumentException(
                $"Property '{propertyName}' not found in type '{obj.GetType().FullName}'."
            );
        property.SetValue(obj, value);
    }
}
