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
		public string FormatSeparator { get; private set; }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="macro">String containing macros</param>
		protected GenericMacro(string macro)
            : this(macro, "${", "}", ":")
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="macro">String containing macros</param>
		/// <param name="startMacro">Marker that indicate "start of macro"</param>
		/// <param name="endMacro">Marker used to indicate "end of macro"</param>
		/// <param name="formatSeparator">Marker used to separate macro from format specifier.</param>
		protected GenericMacro(string macro, string startMacro, string endMacro, string formatSeparator)
		{
			StartMacro = startMacro;
			EndMacro = endMacro;
			FormatSeparator = formatSeparator;
			Parse(macro);
		}

		/// <summary>
		/// Parse macrostring into macro fragments
		/// </summary>
		/// <param name="macro"></param>
		private void Parse(string macro)
		{
			int lastTextPos = 0;
			var list = new List<IMacroFragment<T>>();

			for (int startMacroPos = macro.IndexOf(StartMacro); startMacroPos != -1;)
			{
				int endMacroPos = macro.IndexOf(EndMacro, startMacroPos + StartMacro.Length);
				if (endMacroPos > 0)
				{
					IMacroFragment<T> macroFragment =
						FragmentFactory(macro.Substring(startMacroPos + StartMacro.Length,
						                                      endMacroPos - startMacroPos - StartMacro.Length));

					if (macroFragment != null)
					{
						// Get text before macro
						if (lastTextPos < startMacroPos)
							list.Add(new ConstantMacroFragment<T>(macro.Substring(lastTextPos, startMacroPos - lastTextPos)));
						lastTextPos = endMacroPos + EndMacro.Length;
						list.Add(macroFragment);
						startMacroPos = lastTextPos;
					}
					else
						startMacroPos += StartMacro.Length;
					startMacroPos = macro.IndexOf(StartMacro, startMacroPos);
				}
				else
					startMacroPos = -1;
			}

			// Add remaining text
			if (lastTextPos < macro.Length)
				list.Add(new ConstantMacroFragment<T>(macro.Substring(lastTextPos)));

			_macroFragments = list.ToArray();
		}

		/// <summary>
		/// Construct a macro fragment from a macro string
		/// </summary>
		/// <param name="macro">macro string</param>
		/// <returns></returns>
		protected virtual IMacroFragment<T> FragmentFactory(string macro)
		{
			int macroSeparatorPosition = macro.IndexOf(FormatSeparator);
			string propertyPath = macroSeparatorPosition > 0 ? macro.Substring(0, macroSeparatorPosition) : macro;
			string format = macroSeparatorPosition > 0 ? macro.Substring(macroSeparatorPosition + FormatSeparator.Length) : null;

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