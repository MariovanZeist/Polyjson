using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Polyjson
{
	public class ConverterInfo
	{
		public const string TypeInfoName = "$$Type";

		static ConcurrentDictionary<Type, ConverterInfo> _converters = new ConcurrentDictionary<Type, ConverterInfo>();
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
		public static ConverterInfo GetConverterInfo(Type type) => _converters.GetOrAdd(type, t => BuildFrom(t));
	}
}
