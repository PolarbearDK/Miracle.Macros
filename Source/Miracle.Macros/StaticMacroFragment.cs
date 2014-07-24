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
	    protected StaticMacroFragment(NestedProperty property, string format) 
            : base(property, format)
	    {
	    }

	    /// <summary>
	    /// Factory method to get macro fragment
	    /// </summary>
	    /// <param name="staticTypeSource">type that exposes static property</param>
        /// <param name="staticProperty">name or nested name of object property</param>
	    /// <param name="format"> </param>
	    /// <returns></returns>
	    public static StaticMacroFragment<T> Factory(Type staticTypeSource, string staticProperty, string format)
		{
            NestedProperty nestedProperty = NestedProperty.Factory(staticTypeSource, staticProperty, BindingFlags.Static | BindingFlags.Public);
            return nestedProperty != null ? new StaticMacroFragment<T>(nestedProperty, format) : null;
		}
	}
}