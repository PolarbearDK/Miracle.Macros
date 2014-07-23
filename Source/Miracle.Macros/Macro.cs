using System;

namespace Miracle.Macros
{
	/// <summary>
	/// Macro parser for expandng "macros" in strings using this notation:
	/// Bla bla bla ${Property} ${Property.SubProperty:Format}
	/// 
	/// Propertie are found as properties on data object followed by static properties on System.DateTime type.
	/// </summary>
	/// <typeparam name="T">Type of data object</typeparam>
	public class Macro<T> : GenericMacro<T>
	{
		/// <summary>
		/// Construct macro parser using macro String
		/// </summary>
		/// <param name="macroString"></param>
		public Macro(string macroString) :
			base(macroString)
		{
		}

		/// <summary>
		/// Construct macro parser using macro String and all macro markers.
		/// </summary>
		/// <param name="macroString"></param>
		/// <param name="startMacro"></param>
		/// <param name="endMacro"></param>
		/// <param name="macroSeparator"></param>
		public Macro(string macroString, string startMacro, string endMacro, string macroSeparator) :
			base(macroString, startMacro, endMacro, macroSeparator)
		{
		}

		/// <summary>
		/// Implementation of FragmentFactory that searces for property in this order: 
		///   1. instance property on type "T"
		///   2. static property on System.DateTime type.
        ///   3. static property on System.Environment type.
		/// </summary>
		/// <param name="propertyPath">property path</param>
		/// <param name="format">composite format</param>
		/// <returns></returns>
		protected override IMacroFragment<T> FragmentFactory(string propertyPath, string format)
		{
		    return PropertyMacroFragment<T>.Factory(propertyPath, format)
		           ?? (IMacroFragment<T>) StaticMacroFragment<T>.Factory(typeof(DateTime), propertyPath, format)
		           ?? (IMacroFragment<T>) StaticMacroFragment<T>.Factory(typeof(Environment), propertyPath, format);
		}
	}
}