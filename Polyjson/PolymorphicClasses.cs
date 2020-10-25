using System;
using System.Collections.Generic;

namespace Polyjson
{
	public class Zoo
	{
		public IEnumerable<Animal> Animals { get; set; }
	}

	[TypeDiscriminator("ANI")]
	public abstract class Animal
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
	}

	[TypeDiscriminator("MAM")]
	public abstract class Mammal : Animal
	{
		public int Legs { get; set; }
		public SpiritAnimal SpiritAnimal { get; set; }
	}

	[TypeDiscriminator("GIR")]
	public class Giraffe : Mammal
	{
		public int NeckLength { get; set; }
		public bool IsCool { get; set; }
	}

	[TypeDiscriminator("TIG")]
	public class Tiger : Mammal
	{
		public bool Striped { get; set; }
		public string[] Countries { get; set; }
		public bool IsCool { get; set; }
	}

	[TypeDiscriminator("ANT")]
	public class AntlerFish : Animal
	{
		public int AntlerLength { get; set; }
		public DateTime DiscoveryDate { get; set; }
	}

	[TypeDiscriminator("SPI")]
	public class SpiritAnimal : Animal
	{

	}
}
