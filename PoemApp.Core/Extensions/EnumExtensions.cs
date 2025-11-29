using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace PoemApp.Core.Extensions;

public static class EnumExtensions
{
    public static string GetDisplayName(this Enum enumValue)
    {
        return enumValue.GetType()
            .GetMember(enumValue.ToString())
            .First()
            .GetCustomAttribute<DisplayAttribute>()
            ?.GetName() ?? enumValue.ToString();
    }

    public static T GetEnumFromDisplayName<T>(string displayName) where T : Enum
    {
        foreach (var field in typeof(T).GetFields())
        {
            if (Attribute.GetCustomAttribute(field, typeof(DisplayAttribute)) is DisplayAttribute attribute)
            {
                if (attribute.Name == displayName)
                {
                    object? value = field.GetValue(null);
                    if (value is T enumValue)
                        return enumValue;
                }
            }
        }

        throw new ArgumentException($"No enum value with display name '{displayName}' found in {typeof(T)}");
    }
}