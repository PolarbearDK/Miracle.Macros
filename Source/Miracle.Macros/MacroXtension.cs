using System;
using System.Reflection;

namespace Miracle.Macros
{
	/// <summary>
	/// Static class for macro extensions
	/// Note! If macro is used several times, then use Macro class to parse onse, and call expand several times.
	/// </summary>
	public static class MacroXtension
	{
	    /// <summary>
	    /// Expand macros
	    /// </summary>
	    /// <typeparam name="T">Type of data object </typeparam>
	    /// <param name="macroString">Macro string</param>
	    /// <param name="data">Macro data object</param>
        /// <param name="formatProvider">Format provider used to format values (optional)</param>
        /// <returns>Expanded string</returns>
	    public static string ExpandMacros<T>(this string macroString, T data, IFormatProvider formatProvider = null)
		{
			var macro = new Macro<T>(macroString);
			return macro.Expand(data, formatProvider);
		}

		/// <summary>
		/// Expand macros using supplied 
		/// </summary>
		/// <param name="macroString">Macro string</param>
		/// <param name="data">Macro data object</param>
        /// <param name="formatProvider">Format provider used to format values (optional)</param>
        /// <returns>Expanded string</returns>
		public static string ExpandMacros(this string macroString, object data, IFormatProvider formatProvider = null)
		{
            // Call generic version of ExpandMacros
			foreach (var mi in typeof (MacroXtension).GetMethods(BindingFlags.Public | BindingFlags.Static))
			{
				if (mi.IsGenericMethod && mi.Name == "ExpandMacros")
				{
					var genericMethod = mi.MakeGenericMethod(new[] {data.GetType()});
					return (string) genericMethod.Invoke(null, new[] {macroString, data, formatProvider});
				}
			}
			throw new MethodAccessException("Method not found");
		}

        /// <summary>
        /// Expand macros without providing a data object (only rely on buildin DateTime and Environment statics)
        /// </summary>
        /// <param name="macroString">Macro string</param>
        /// <param name="formatProvider">Format provider used to format values (optional)</param>
        /// <returns>Expanded string</returns>
        public static string ExpandMacros(this string macroString, IFormatProvider formatProvider = null)
        {
            return ExpandMacros<object>(macroString, new object(), formatProvider);
        }
    }
}