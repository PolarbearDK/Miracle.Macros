using System;
using System.Collections.Generic;
using System.Text;

namespace Miracle.Macros
{
	/// <summary>
	/// Macro parser for expandng "macros" in strings using this notation:
	/// Bla bla bla ${Property} ${Property.SubProperty:Format}
	/// </summary>
	/// <typeparam name="T">Type of data object</typeparam>
	public abstract class GenericMacro<T>
	{
		private IMacroFragment<T>[] _macroFragments;

		/// <summary>
		/// Marker used to indicate "start of macro"
		/// </summary>
		public string StartMacro { get; private set; }

		/// <summary>
		/// Marker used to indicate "end of macro"
		/// </summary>
		public string EndMacro { get; private set; }

		/// <summary>
		/// Marker used to separate macro from format specifier.
		/// </summary>
		public string MacroSeparator { get; private set; }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="macroString">String containing macros</param>
		protected GenericMacro(string macroString)
		{
			StartMacro = "${";
			EndMacro = "}";
			MacroSeparator = ":";
			Parse(macroString);
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="macroString">String containing macros</param>
		/// <param name="startMacro">Marker that indicate "start of macro"</param>
		/// <param name="endMacro">Marker used to indicate "end of macro"</param>
		/// <param name="macroSeparator">Marker used to separate macro from format specifier.</param>
		protected GenericMacro(string macroString, string startMacro, string endMacro, string macroSeparator)
		{
			StartMacro = startMacro;
			EndMacro = endMacro;
			MacroSeparator = macroSeparator;
			Parse(macroString);
		}

		/// <summary>
		/// Parse macrostring, and find macros
		/// </summary>
		/// <param name="macroString"></param>
		private void Parse(string macroString)
		{
			int lastTextPos = 0;
			var list = new List<IMacroFragment<T>>();

			for (int startMacroPos = macroString.IndexOf(StartMacro); startMacroPos != -1;)
			{
				int endMacroPos = macroString.IndexOf(EndMacro, startMacroPos + StartMacro.Length);
				if (endMacroPos > 0)
				{
					IMacroFragment<T> macroFragment =
						FragmentFactory(macroString.Substring(startMacroPos + StartMacro.Length,
						                                      endMacroPos - startMacroPos - StartMacro.Length));

					if (macroFragment != null)
					{
						// Get text before macro
						if (lastTextPos < startMacroPos)
							list.Add(new ConstantMacroFragment<T>(macroString.Substring(lastTextPos, startMacroPos - lastTextPos)));
						lastTextPos = endMacroPos + EndMacro.Length;
						list.Add(macroFragment);
						startMacroPos = lastTextPos;
					}
					else
						startMacroPos += StartMacro.Length;
					startMacroPos = macroString.IndexOf(StartMacro, startMacroPos);
				}
				else
					startMacroPos = -1;
			}

			// Add remaining text
			if (lastTextPos < macroString.Length)
				list.Add(new ConstantMacroFragment<T>(macroString.Substring(lastTextPos)));

			_macroFragments = list.ToArray();
		}

		/// <summary>
		/// Construct a macro fragment from a macro string
		/// </summary>
		/// <param name="macro">macro string</param>
		/// <returns></returns>
		protected virtual IMacroFragment<T> FragmentFactory(string macro)
		{
			int macroSeparatorPosition = macro.IndexOf(MacroSeparator);
			string propertyPath = macroSeparatorPosition > 0 ? macro.Substring(0, macroSeparatorPosition) : macro;
			string format = macroSeparatorPosition > 0 ? macro.Substring(macroSeparatorPosition + MacroSeparator.Length) : null;

			return FragmentFactory(propertyPath, format);
		}

		/// <summary>
		/// Construct a macro fragment from a macro string
		/// </summary>
		/// <param name="propertyPath">property path</param>
		/// <param name="format">composite format</param>
		/// <returns></returns>
		protected abstract IMacroFragment<T> FragmentFactory(string propertyPath, string format);

		/// <summary>
		/// Expand macro
		/// </summary>
		/// <param name="macroObject">data to expand macro with</param>
        /// <param name="formatProvider">Format provider used to format value</param>
        /// <returns>Expanded macro</returns>
		public string Expand(T macroObject, IFormatProvider formatProvider = null)
		{
			var sb = new StringBuilder();
			foreach (var fragment in _macroFragments)
			{
				sb.Append(fragment.GetValue(macroObject, formatProvider));
			}
			return sb.ToString();
		}
	}
}