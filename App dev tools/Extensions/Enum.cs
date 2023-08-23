using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace AppDevTools.Extensions
{
    public class Enum
    {
        #region Methods

        #region Public
        public static List<string> GetMemberValues<T>() where T : struct, IConvertible
        {
            List<string> memberValues = new();
            IEnumerable<MemberInfo>? members = typeof(T).GetTypeInfo().DeclaredMembers;

            foreach (var member in members)
            {
                string? value = member?.GetCustomAttribute<EnumMemberAttribute>(false)?.Value;
                if (!string.IsNullOrEmpty(value))
                {
                    memberValues.Add(value);
                }
            }

            return memberValues;
        }

        public static string? GetMemberValue<T>(T enumValue)
        {
            if (enumValue != null)
            {
                string? name = enumValue.ToString();
                if (name != null)
                {
                    MemberInfo[]? memberInfo = typeof(T).GetMember(name);
                    EnumMemberAttribute? attribute = memberInfo.FirstOrDefault()?.GetCustomAttributes(false).OfType<EnumMemberAttribute>().FirstOrDefault();
                    if (attribute != null)
                    {
                        return attribute.Value;
                    }
                }
            }

            return null;
        }

        public static T? GetEnumFromStr<T>(string str)
        {
            Type? enumType = typeof(T);
            foreach (string name in System.Enum.GetNames(enumType))
            {
                FieldInfo? fieldInfo = enumType.GetField(name);
                if (fieldInfo != null)
                {
                    EnumMemberAttribute? attribute = ((EnumMemberAttribute[])fieldInfo.GetCustomAttributes(typeof(EnumMemberAttribute), true)).Single();
                    if (attribute != null && attribute.Value == str)
                    {
                        return (T)System.Enum.Parse(enumType, name);
                    }
                }
            }

            return default;
        }
        #endregion Public

        #endregion Methods
    }
}