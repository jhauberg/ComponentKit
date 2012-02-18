/// ###TL;DR..
/// 
/// Too bad, because this part is missing right now ^_^

/// ##Source
using System;
using System.Text;

namespace ComponentKit {
    internal static class TypeExtensions {
        /// > See http://stackoverflow.com/a/401824/144433
        public static string GetPrettyName(this Type type) {
            bool isArray = false;

            if (type.IsArray) {
                type = type.GetElementType();

                isArray = true;
            }

            if (type.IsGenericParameter) {
                return type.Name;
            }

            if (!type.IsGenericType) {
                return type.Name;
            }

            string name = type.Name;
            
            StringBuilder output = new StringBuilder();

            output.AppendFormat("{0}", name.Substring(0, name.IndexOf("`")));
            output.Append('<');

            bool first = true;

            foreach (Type typeArgument in type.GetGenericArguments()) {
                if (first) {
                    first = false;
                } else {
                    output.Append(", ");
                }

                output.Append(typeArgument.GetPrettyName());
            }

            output.Append('>');

            if (isArray) {
                output.Append("[]");
            }

            return output.ToString();
        }
    }
}

/// Copyright 2012 Jacob H. Hansen.