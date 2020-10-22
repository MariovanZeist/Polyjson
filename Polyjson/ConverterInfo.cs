using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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

		public static Type FindType(string typeName) => _knownTypes[typeName];

		public IEnumerable<PropertyInfo> Properties { get; private set; }

		protected ConverterInfo() {	}
		public static ConverterInfo BuildFrom<T>() => BuildFrom(typeof(T));

		public static ConverterInfo BuildFrom(Type type) => new ConverterInfo { Properties = type.GetProperties().Where(p => p.CanWrite && p.CanRead).ToArray() };
	}
}
