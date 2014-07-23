using System;

namespace Miracle.Macros
{
	/// <summary>
	/// A constant macro that doesnt change with data.
	/// </summary>
	/// <typeparam name="T">Type of data object</typeparam>
	public class ConstantMacroFragment<T> : IMacroFragment<T>
	{
		private readonly string _value;

		/// <summary>
		/// Construct constant macro fragment
		/// </summary>
		/// <param name="value">String value of this macro</param>
		public ConstantMacroFragment(string value)
		{
			_value = value;
		}

		#region IMacroFragment Members

		/// <summary>
		/// Method to get string representation of macro fragment.
		/// </summary>
		/// <param name="obj">The object to get any properties from</param>
        /// <param name="formatProvider">Format provider used to format value (ignored here)</param>
        /// <returns></returns>
		public string GetValue(T obj, IFormatProvider formatProvider)
		{
			return _value;
		}

		#endregion
	}
}