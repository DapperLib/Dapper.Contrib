using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace Dapper.Contrib.Extensions
{
    /// <summary>
    /// Map <see cref="Type"/> to database table extensions
    /// </summary>
    public static class TypeExtensions
    {
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, string> TypeTableName = new ConcurrentDictionary<RuntimeTypeHandle, string>();
        
        /// <summary>
        /// The function to get a table name from a given <see cref="Type"/>
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to get a table name for.</param>
        public delegate string TableNameMapperDelegate(Type type);

        /// <summary>
        /// Specify a custom table name mapper based on the POCO type name
        /// </summary>
#pragma warning disable CA2211 // Non-constant fields should not be visible - I agree with you, but we can't do that until we break the API
        public static TableNameMapperDelegate TableNameMapper;
#pragma warning restore CA2211 // Non-constant fields should not be visible
        
        /// <summary>
        /// Get database table name from <see cref="Type"/>
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to get a table name for.</param>
        public static string TableName(this Type type)
        {
            if (TypeTableName.TryGetValue(type.TypeHandle, out string name)) return name;

            if (TableNameMapper != null)
            {
                name = TableNameMapper(type);
            }
            else
            {
                //NOTE: This as dynamic trick falls back to handle both our own Table-attribute as well as the one in EntityFramework 
                var tableAttrName =
                    type.GetCustomAttribute<TableAttribute>(false)?.Name
                    ?? (type.GetCustomAttributes(false).FirstOrDefault(attr => attr.GetType().Name == "TableAttribute") as dynamic)?.Name;

                if (tableAttrName != null)
                {
                    name = tableAttrName;
                }
                else
                {
                    name = type.Name + "s";
                    if (type.IsInterface && name.StartsWith("I"))
                        name = name.Substring(1);
                }
            }

            TypeTableName[type.TypeHandle] = name;
            return name;
        }
    }
}
