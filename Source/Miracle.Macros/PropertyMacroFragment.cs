using System;
using System.Reflection;

namespace Miracle.Macros
{
	/// <summary>
	/// A macro fragment that obtains data from properties on data object
	/// </summary>
	/// <typeparam name="T">Type of data object</typeparam>
	public class PropertyMacroFragment<T> : FormatMacroFragment<T>
	{
	    /// <summary>
		/// Nested property wrapper
		/// </summary>
		private class NestedProperty
		{
			private readonly NestedProperty _child;
			private readonly PropertyInfo _property;

			private NestedProperty(Type type, string property)
			{
				int dotPos = property.IndexOf('.');

				if (dotPos == -1)
				{
					_property = type.GetProperty(property);
				}
				else
				{
					_property = type.GetProperty(property.Substring(0, dotPos));
					if (_property != null)
						_child = new NestedProperty(_property.PropertyType, property.Substring(dotPos + 1));
				}
			}

			private bool IsValid
			{
				get { return _property != null && (_child == null || _child.IsValid); }
			}

			/// <summary>
			/// Method to get value of property.
			/// </summary>
			/// <param name="obj">The object to get any properties from</param>
			/// <returns></returns>
			public object GetValue(object obj)
			{
				if (obj == null) return null;
				obj = _property.GetValue(obj, null);
				return _child != null ? _child.GetValue(obj) : obj;
			}

			/// <summary>
			/// Create a nested property wrapper
			/// </summary>
			/// <param name="type">Type of object at this level</param>
			/// <param name="property">Property on the format "Property.SubPropery.SubSubProperty"...</param>
			/// <returns>Value of property</returns>
			public static NestedProperty Factory(Type type, string property)
			{
				var getter = new NestedProperty(type, property);
				return getter.IsValid
				       	? getter
				       	: null;
			}
		}

	    /// <summary>
		/// Reference to nested property
		/// </summary>
		private readonly NestedProperty _nestedProperty;

		private PropertyMacroFragment(NestedProperty nestedProperty, string format)
			: base(format)
		{
			_nestedProperty = nestedProperty;
		}

        /// <summary>
        /// Get raw un-formatted value of macro fragment.
        /// </summary>
        /// <param name="data">The data object to optionally get data from</param>
        /// <returns>formatted value</returns>
        protected override object GetRawValue(T data)
		{
			return _nestedProperty.GetValue(data);
		}

		/// <summary>
		/// Static method to create a PropertyMacroFragment
		/// </summary>
		/// <param name="property"></param>
		/// <param name="format"></param>
		/// <returns>An initialized PropertymacroFragment, or null if nested property was not found</returns>
		public static PropertyMacroFragment<T> Factory(string property, string format)
		{
			NestedProperty nestedProperty = NestedProperty.Factory(typeof (T), property);
			return nestedProperty != null ? new PropertyMacroFragment<T>(nestedProperty, format) : null;
		}
	}
}