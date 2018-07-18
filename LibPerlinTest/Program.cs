using System;
using LibPerlin;

namespace LibPerlinTest
{
    class MainClass
    {
        public static void Main(string[] args)
        {
			int height = 25;
			int width = 25;

            // Values to create decent terrain.
			var A = Perlin.GenerateTerrain(height, width, 5, 0.7f);
			for (int i = 0; i < height; i++) 
			{
				for (int j = 0; j < width; j++)
                {
					Console.Write(A[j,i]);
                }
				Console.WriteLine();
			}
        }
    }
}
