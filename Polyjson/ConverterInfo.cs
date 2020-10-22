using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Polyjson
{
	public class ConverterInfo
	{
		static ConcurrentDictionary<string, Type> _knownTypes = new ConcurrentDictionary<string, Type>();

		public static string GetTypeName(Type type)
		{
			var s = type.Name;
			_knownTypes.TryAdd(s, type);
			return s;
		}

		public static Type FindType(string typeName)
		{
			return _knownTypes[typeName];
		}

		public IEnumerable<ConvertProperty> PropertyConverters { get; }
		public ConverterInfo(IEnumerable<ConvertProperty> propertyConverters) => PropertyConverters = propertyConverters;
	}
}
