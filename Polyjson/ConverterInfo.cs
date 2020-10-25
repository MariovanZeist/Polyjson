using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Polyjson
{
    /// <summary>
    /// Helper class for type discriminated/polymorphic serialization and deserialization.
    /// </summary>
    public class ConverterInfo
    {
        protected ConverterInfo() { }

        /// <summary>
        /// The JSON tag to be used for type discriminator data.
        /// </summary>
        public const string TypeInfoName = "$$TypeDiscriminator";

        /// <summary>
        /// Properties of a class that require serialization or deserialization. Applies only to properties that
        /// can be read and written.
        /// </summary>
        public ICollection<PropertyInfo> Properties { get; init; }

        public string TypeDiscriminator { get; init; }

        public Type Type { get; init; }

        /// <summary>
        /// A type-safe concurrent dictionary of all convertible properties (an ICollection) keyed by Type.
        /// </summary>
        private static readonly ConcurrentDictionary<Type, ConverterInfo> _converters = new ConcurrentDictionary<Type, ConverterInfo>();
        private static readonly ConcurrentDictionary<string, ConverterInfo> _knownTypes = new ConcurrentDictionary<string, ConverterInfo>();

		/// <summary>
		/// Builds a new ConvertInfo object relevant to the given Type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		private static ConverterInfo BuildFrom(Type type, bool defaultToClassName = false)
		{
            var converterInfo = new ConverterInfo
            {
                Type = type,
                Properties = type.GetProperties().Where(p => p.CanWrite && p.CanRead).ToArray(),
                TypeDiscriminator = type.GetCustomAttribute<TypeDiscriminatorAttribute>()?.Property ?? (defaultToClassName ? type.FullName : throw new NullReferenceException($"{type} failed to find the required '{nameof(TypeDiscriminatorAttribute)}'"))
			};
            _knownTypes[converterInfo.TypeDiscriminator] = converterInfo;
            return converterInfo;
        }

        /// <summary>
        /// Returns the ConvertInfo relevant to the the given Type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static ConverterInfo GetConverterInfo(Type type) => _converters.GetOrAdd(type, t => BuildFrom(t));


        public static ConverterInfo GetConverterInfo(string typeDiscriminator)
        {
			if (!_knownTypes.TryGetValue(typeDiscriminator, out var converterInfo))
			{
                throw new NullReferenceException($"failed to find the required type for '{typeDiscriminator}'");
			}
            return converterInfo;
        }

        public static void BuildConverterInfo(Assembly assembly, bool defaultToClassName = false)
		{
            var discriminatedTypes = assembly.GetTypes().Where(type => type.GetCustomAttribute<TypeDiscriminatorAttribute>() != null);

			foreach (var discriminatedType in discriminatedTypes)
			{
                _converters.TryAdd(discriminatedType, BuildFrom(discriminatedType, defaultToClassName));
            }
        }
    }


    /// <summary>
    /// Represents the <see cref="Attribute"/> used to indicate the property used to discriminate derived types of the marked class. Not intended to be used for abstract classes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class TypeDiscriminatorAttribute : Attribute
    {
        /// <summary>
        /// Gets the name of the property used to discriminate derived types of the class marked by the <see cref="DiscriminatorAttribute"/>
        /// </summary>
        public readonly string Property;


        /// <summary>
        /// Initializes a new <see cref="DiscriminatorAttribute"/>
        /// </summary>
        /// <param name="property">The name of the property used to discriminate derived types of the class marked by the <see cref="DiscriminatorAttribute"/></param>
        public TypeDiscriminatorAttribute(string property)
        {
            Property = property;
        }
    }
}
