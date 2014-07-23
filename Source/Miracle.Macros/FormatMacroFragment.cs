using System;

namespace Miracle.Macros
{
	/// <summary>
	/// Abstract macro to add composite formatting capability to macro fragments.
	/// </summary>
	/// <typeparam name="T">Type of data object</typeparam>
	public abstract class FormatMacroFragment<T> : IMacroFragment<T>
	{
		private readonly string _format;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="format">composite format string for this instance</param>
		protected FormatMacroFragment(string format)
		{
			_format = format;
		}

		#region IMacroFragment Members

	    /// <summary>
	    /// Method to get formatted string representation of macro.
	    /// </summary>
	    /// <param name="data">The object to get any properties from</param>
	    /// <param name="formatProvider">Format provider used to format value</param>
	    /// <returns></returns>
	    public string GetValue(T data, IFormatProvider formatProvider)
	    {
	        var value = GetRawValue(data);
            if (value != null)
            {
                return _format != null
                        ? ((IFormattable)value).ToString(_format, formatProvider)
                        : value.ToString();
            }

            return null;
        }

	    #endregion

	    /// <summary>
	    /// Get raw un-formatted value of macro fragment.
	    /// </summary>
        /// <param name="data">The data object to optionally get data from</param>
        /// <returns>formatted value</returns>
	    protected abstract object GetRawValue(T data);
	}
}