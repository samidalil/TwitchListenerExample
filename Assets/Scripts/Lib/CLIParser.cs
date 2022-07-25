
using System;
using System.Collections.Generic;
using System.Reflection;

namespace TwitchListener.Lib
{
    /// <summary>
    /// CLI arguments parser methods - Totalement remplaçable par le paquet CommandLineParser
    /// </summary>
    public static class CLIParser
    {
        #region Private Fields

        /// <summary>
        /// Gets the information on the CastTo method, for performance purposes
        /// </summary>
        private static readonly MethodInfo _castToMethodInfo = typeof(CLIParser)
            .GetMethod(nameof(CLIParser.CastTo), BindingFlags.Static | BindingFlags.Public);

        #endregion

        #region Public Methods

        /// <summary>
        /// Parses an array of pairs name-value to populate a object of a given runtime type instance
        /// </summary>
        /// <param name="arguments">Arguments of a command</param>
        /// <param name="type">Runtime type instance</param>
        /// <returns>A populated object of given type</returns>
        public static object Parse(List<(string, string)> arguments, Type type)
        {
            object data = Activator.CreateInstance(type);

            foreach (var (name, value) in arguments)
            {
                PropertyInfo property = type.GetProperty(name);

                if (property.PropertyType == typeof(string))
                    property.SetValue(data, value, null);
                else if (property.PropertyType == typeof(int))
                    property.SetValue(data, int.Parse(value), null);
                else if (property.PropertyType == typeof(float))
                    property.SetValue(data, float.Parse(value), null);
                else if (property.PropertyType == typeof(double))
                    property.SetValue(data, double.Parse(value), null);
                else if (property.PropertyType == typeof(bool))
                    property.SetValue(data, bool.Parse(value), null);
                else if (property.PropertyType == typeof(short))
                    property.SetValue(data, short.Parse(value), null);
                else if (property.PropertyType == typeof(ushort))
                    property.SetValue(data, ushort.Parse(value), null);
                else if (property.PropertyType == typeof(uint))
                    property.SetValue(data, uint.Parse(value), null);
                else if (property.PropertyType == typeof(ulong))
                    property.SetValue(data, ulong.Parse(value), null);
                else if (property.PropertyType == typeof(decimal))
                    property.SetValue(data, decimal.Parse(value), null);
                else if (property.PropertyType == typeof(char))
                    property.SetValue(data, char.Parse(value), null);
                else
                    property.SetValue(data, CLIParser.CastTo(value, property.PropertyType), null);
            }

            return data;
        }

        /// <summary>
        /// Parses an array of pairs name-value to populate a object of a given runtime type instance
        /// </summary>
        /// <typeparam name="T">Reference type for parsing</typeparam>
        /// <param name="arguments">Arguments of a command</param>
        /// <returns>A populated object of given type</returns>
        public static T Parse<T>(List<(string, string)> arguments)
        {
            return (T)CLIParser.Parse(arguments, typeof(T));
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Casts object to a given generic type
        /// </summary>
        /// <typeparam name="T">Cast type</typeparam>
        /// <param name="o">Object to cast</param>
        /// <returns>Casted object</returns>
        private static T CastTo<T>(object o) => (T)o;

        /// <summary>
        /// Cast object to a given type runtime instance
        /// </summary>
        /// <param name="o">Object to cast</param>
        /// <param name="type">Runtime type instance</param>
        /// <returns>Casted object</returns>
        private static object CastTo(object o, Type type)
        {
            return CLIParser
                ._castToMethodInfo?
                .MakeGenericMethod(new Type[] { type })?
                .Invoke(null, new[] { o });
        }

        #endregion
    }
}
