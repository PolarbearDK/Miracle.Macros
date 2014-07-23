using System;

namespace Miracle.Macros
{
	/// <summary>
	/// Interface representing a macro fragment 
	/// </summary>
	/// <typeparam name="T">Type of data object</typeparam>
	public interface IMacroFragment<T>
	{
	    /// <summary>
	    /// Method to get string representation of macro fragment.
	    /// </summary>
	    /// <param name="obj">The object to get any properties from</param>
	    /// <param name="formatProvider">Format provider used to format values</param>
	    /// <returns></returns>
	    string GetValue(T obj, IFormatProvider formatProvider);
	}
}