using System;
using System.Collections.Generic;

namespace Polyjson
{
	public class Zoo
	{
		public IEnumerable<Animal> Animals { get; set; }
	}

	public abstract class Animal
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
	}

	public abstract class Mammal : Animal
	{
		public int Legs { get; set; }
	}

	public class Giraffe : Mammal
	{
		public int NeckLength { get; set; }
	}

	public class Tiger : Mammal
	{
		public bool Striped { get; set; }
		public string[] Countries { get; set; }
	}

	public class AntlerFish : Animal
	{
		public int AntlerLength { get; set; }
		public DateTime DiscoveryDate { get; set; }
	}
}
