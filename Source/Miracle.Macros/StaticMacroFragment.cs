using System;
using System.Reflection;

namespace Miracle.Macros
{
	/// <summary>
	/// Get dynamic property from static object
	/// </summary>
	/// <typeparam name="T">Type of data object</typeparam>
	public class StaticMacroFragment<T> : FormatMacroFragment<T>
	{
		private readonly PropertyInfo _propertyInfo;

		private StaticMacroFragment(PropertyInfo propertyInfo, string format)
			: base(format)
		{
			_propertyInfo = propertyInfo;
		}

        /// <summary>
        /// Get raw un-formatted value of macro fragment.
        /// </summary>
        /// <param name="data">The data object to optionally get data from</param>
        /// <returns>formatted value</returns>
        protected override object GetRawValue(T data)
        {
            return _propertyInfo.GetValue(null, null);
        }
        
	    /// <summary>
	    /// Factory method to get macro fragment
	    /// </summary>
	    /// <param name="staticTypeSource">type that exposes static property</param>
        /// <param name="staticPropertyName">name of object property</param>
	    /// <param name="format"> </param>
	    /// <returns></returns>
	    public static StaticMacroFragment<T> Factory(Type staticTypeSource, string staticPropertyName, string format)
		{
			PropertyInfo info = staticTypeSource.GetProperty(staticPropertyName, BindingFlags.Static | BindingFlags.Public);
			return info != null ? new StaticMacroFragment<T>(info, format) : null;
		}
	}
}