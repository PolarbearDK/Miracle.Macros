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
	    protected class NestedProperty
		{
			private readonly NestedProperty _child;
			private readonly PropertyInfo _property;

            private NestedProperty(Type type, string property, BindingFlags bindingFlags)
			{
				int dotPos = property.IndexOf('.');

				if (dotPos == -1)
				{
					_property = type.GetProperty(property, bindingFlags);
				}
				else
				{
					_property = type.GetProperty(property.Substring(0, dotPos), bindingFlags);
					if (_property != null)
						_child = new NestedProperty(_property.PropertyType, property.Substring(dotPos + 1), BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
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
            /// <param name="bindingFlags">A bitmask comprised of one or more System.Reflection.BindingFlags that specify how the property search is conducted.</param>
	        /// <returns>Value of property</returns>
	        public static NestedProperty Factory(Type type, string property, BindingFlags bindingFlags)
			{
				var getter = new NestedProperty(type, property, bindingFlags);
				return getter.IsValid
				       	? getter
				       	: null;
			}
		}

	    /// <summary>
		/// Reference to nested property
		/// </summary>
	    protected readonly NestedProperty Property;

        /// <summary>
        /// Property macro fragment constructor.
        /// </summary>
        /// <param name="property">Property on the format "Property.SubPropery.SubSubProperty"...</param>
        /// <param name="format">Optional composite format specifier</param>
        protected PropertyMacroFragment(NestedProperty property, string format)
			: base(format)
		{
			Property = property;
		}

        /// <summary>
        /// Get raw un-formatted value of macro fragment.
        /// </summary>
        /// <param name="data">The data object to optionally get data from</param>
        /// <returns>formatted value</returns>
        protected override object GetRawValue(T data)
		{
			return Property.GetValue(data);
		}

		/// <summary>
		/// Static method to create a PropertyMacroFragment
		/// </summary>
        /// <param name="property">Property on the format "Property.SubPropery.SubSubProperty"...</param>
        /// <param name="format">Optional composite format specifier</param>
		/// <returns>An initialized PropertymacroFragment, or null if nested property was not found</returns>
		public static IMacroFragment<T> Factory(string property, string format)
		{
            NestedProperty nestedProperty = NestedProperty.Factory(typeof(T), property, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
			return nestedProperty != null ? new PropertyMacroFragment<T>(nestedProperty, format) : null;
		}
	}
}