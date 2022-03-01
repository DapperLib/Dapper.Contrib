using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace Dapper.Contrib.Extensions
{
    /// <summary>
    /// Map <see cref="PropertyInfo"/> to database column extensions
    /// </summary>
    public static class PropertyInfoExtensions
    {
        private static readonly ConcurrentDictionary<PropertyInfo, string> PropertyColumnName = new ConcurrentDictionary<PropertyInfo, string>();
        
        /// <summary>
        /// The function to get a column name from a given <see cref="PropertyInfo"/>
        /// </summary>
        /// <param name="property">The <see cref="PropertyInfo"/> to get a column name for.</param>
        public delegate string ColumnNameMapperDelegate(PropertyInfo property);
        
        /// <summary>
        /// Specify a custom column name mapper based on the POCO type name
        /// </summary>
#pragma warning disable CA2211 // Non-constant fields should not be visible - I agree with you, but we can't do that until we break the API
        public static ColumnNameMapperDelegate ColumnNameMapper;
#pragma warning restore CA2211 // Non-constant fields should not be visible

        /// <summary>
        /// Get database column name from <see cref="PropertyInfo"/>
        /// </summary>
        /// <param name="property">The <see cref="PropertyInfo"/> to get a column name for.</param>
        public static string ColumnName(this PropertyInfo property)
        {
            if (PropertyColumnName.TryGetValue(property, out string name)) return name;

            if (ColumnNameMapper != null)
            {
                name = ColumnNameMapper(property);
            }
            else
            {
                //NOTE: This as dynamic trick falls back to handle both our own Column-attribute as well as the one in EntityFramework 
                var columnAttrName =
                    property.GetCustomAttribute<ColumnAttribute>(true)?.Name
                    ?? (property.GetCustomAttributes(true).FirstOrDefault(attr => attr.GetType().Name == "ColumnAttribute") as dynamic)?.Name;

                name = columnAttrName != null 
                    ? columnAttrName 
                    : property.Name;
            }

            PropertyColumnName[property] = name;
            return name;
        }
    }
}
