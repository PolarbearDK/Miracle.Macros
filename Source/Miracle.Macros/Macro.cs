using System;
using System.Threading;

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
		/// <param name="macro"></param>
		public Macro(string macro) :
			base(macro)
		{
		}

		/// <summary>
		/// Construct macro parser using macro String and all macro markers.
		/// </summary>
		/// <param name="macro"></param>
		/// <param name="startMacro"></param>
		/// <param name="endMacro"></param>
		/// <param name="formatSeparator"></param>
		public Macro(string macro, string startMacro, string endMacro, string formatSeparator) :
			base(macro, startMacro, endMacro, formatSeparator)
		{
		}

		/// <summary>
		/// Implementation of FragmentFactory that searces for property in this order: 
		///   1. instance property on type "T"
		///   2. static property on System.DateTime type.
        ///   3. static property on System.Environment type.
        ///   4. static property on System.Thread type.
        /// </summary>
		/// <param name="propertyPath">property path</param>
		/// <param name="format">composite format</param>
		/// <returns></returns>
		protected override IMacroFragment<T> FragmentFactory(string propertyPath, string format)
		{
		    return PropertyMacroFragment<T>.Factory(propertyPath, format)
		           ?? StaticMacroFragment<T>.Factory(typeof(DateTime), propertyPath, format)
		           ?? StaticMacroFragment<T>.Factory(typeof(Environment), propertyPath, format)
		           ?? StaticMacroFragment<T>.Factory(typeof(Thread), propertyPath, format);
		}
	}
}