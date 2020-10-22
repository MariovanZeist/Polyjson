using System;
using System.IO;

namespace Polyjson
{
	class Program
	{
		public static Giraffe Giraffe = new Giraffe { Name = "Marco", Legs = 4, NeckLength = 400 };
		public static AntlerFish AntlerFish = new AntlerFish { Name = "Fishy", AntlerLength = 2 };
		public static Tiger Tiger = new Tiger { Name = "ElTigro", Legs = 4, Striped = true };
		public static Zoo Zoo = new Zoo { Animals = new Animal[] { Giraffe, AntlerFish, Tiger } };

		static void Main(string[] args)
		{
			Console.WriteLine("Hello World!");
			HandleZoo();
		}


		public static void HandleZoo()
		{
			var ns1 = NS.Serialize(Zoo);
			WriteFile("D:\\NS.json", ns1);
			var st1 = ST.Serialize(Zoo);
			WriteFile("D:\\ST.json", st1);
			var zoo = ST.DeSerialize<Zoo>(st1);
		}

		static void WriteFile(string filename, string json)
		{
			using var sw = new StreamWriter(filename);
			sw.Write(json);
		}
	}
}
