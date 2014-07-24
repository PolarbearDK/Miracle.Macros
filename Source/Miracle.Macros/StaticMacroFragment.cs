using System;
using System.Reflection;

namespace Miracle.Macros
{
	/// <summary>
	/// Get dynamic property from static object
	/// </summary>
	/// <typeparam name="T">Type of data object</typeparam>
    public class StaticMacroFragment<T> : PropertyMacroFragment<T>
	{
	    private StaticMacroFragment(NestedProperty property, string format) 
            : base(property, format)
	    {
	    }

	    /// <summary>
	    /// Factory method to get macro static fragment
	    /// </summary>
	    /// <param name="staticTypeSource">type that exposes static properties</param>
        /// <param name="property">Property on the format "Property.SubPropery.SubSubProperty"...</param>
        /// <param name="format">Optional composite format specifier</param>
	    /// <returns></returns>
	    public static IMacroFragment<T> Factory(Type staticTypeSource, string property, string format)
		{
            NestedProperty nestedProperty = NestedProperty.Factory(staticTypeSource, property, BindingFlags.Static | BindingFlags.Public);
            return nestedProperty != null ? new StaticMacroFragment<T>(nestedProperty, format) : null;
		}
	}
}