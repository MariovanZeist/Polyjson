using System;
using System.IO;
using System.Reflection;

namespace Polyjson
{
	class Program
	{
		public static Giraffe Giraffe = new Giraffe { Id = Guid.NewGuid(), Name = "Marco", Legs = 4, NeckLength = 400 , SpiritAnimal = new SpiritAnimal { Id = Guid.NewGuid(), Name = "Simba" }, IsCool=true };
		public static AntlerFish AntlerFish = new AntlerFish { Id = Guid.NewGuid(), Name = "Fishy", AntlerLength = 2, DiscoveryDate = DateTime.Now.AddYears(-100)};
		public static Tiger Tiger = new Tiger { Id = Guid.NewGuid(), Name = "ElTigro", Legs = 4, Striped = true , Countries = new []{ "India", "Taiwan", "Bonaire" }, IsCool = true };
		public static Zoo Zoo = new Zoo { Animals = new Animal[] { Giraffe, AntlerFish, Tiger } };

		static void Main(string[] args)
		{
			Console.WriteLine("Hello World!");
			HandleZoo();
		}


		public static void HandleZoo()
		{
			ConverterInfo.BuildConverterInfo(Assembly.GetExecutingAssembly());
			var ns1 = NS.Serialize(Zoo);
			WriteFile("D:\\NS.json", ns1);
			var st1 = ST.Serialize(Zoo);
			WriteFile("D:\\ST.json", st1);
			var zooNS = ST.DeSerialize<Zoo>(ns1);
			var zooST = NS.DeSerialize<Zoo>(st1);
		}

		static void WriteFile(string filename, string json)
		{
			using var sw = new StreamWriter(filename);
			sw.Write(json);
		}
	}
}
