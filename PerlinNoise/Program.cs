/**
 * Functions for generating Perlin noise. To run the demos, put "grass.png" 
 * and "sand.png" in the executable folder.
 **/

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace PerlinNoise
{
    class PerlinNoise
    {
        static Random random = new Random();

        public static void Main()
        {

            int width = 25;
            int height = 25;
            int octaveCount = 6;

            float[,] perlinNoise = GeneratePerlinNoise(width, height, octaveCount);
            int[,] terrain = Transform(perlinNoise);

			//PrintTerrain(terrain);
			for (int y = 0; y < 25; y++)
			{
				for (int x = 0; x < 25; x++)
				{
					Console.Write(terrain[y, x]);
				}
				Console.WriteLine();
			}

			int cost = LibAStar.AStar.findPath(terrain, 1, 1, 23, 23).Item2;

			Console.SetCursorPosition(0, 28);
			Console.WriteLine("Path Costs: " + cost);
        }

		public static int[,] GenerateTerrain(int width, int height, int smoothing)
		{
			float[,] perlinNoise = GeneratePerlinNoise(width, height, smoothing);
			int[,] terrain = Transform(perlinNoise);
			return terrain;
		}

        public static void PrintTerrain(int[,] array)
        {
            Console.OutputEncoding = Encoding.UTF8;
            string pid = System.Environment.OSVersion.Platform.ToString();
            Console.WriteLine();
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    //Console.Write(Math.Round(element * 10));
                    //Console.Write(element + " ");
                    if (array[i,j] == 0)
                    {
						//Console.BackgroundColor = ConsoleColor.DarkBlue;
						//Console.ForegroundColor = ConsoleColor.DarkGreen;
						//Console.Write('\u2591');
						//Console.BackgroundColor = ConsoleColor.Black;
						//Console.ForegroundColor = ConsoleColor.Black;
						Console.Write(1);
                    }
                    else if (array[i,j] == 1)
                    {
                        //Console.BackgroundColor = ConsoleColor.DarkGreen;
                        //Console.ForegroundColor = ConsoleColor.DarkYellow;
                        //Console.Write('\u2591');
                        //Console.BackgroundColor = ConsoleColor.Black;
                        //Console.ForegroundColor = ConsoleColor.Black;
						Console.Write(2);
                    }
                    else
                    {
                        //Console.BackgroundColor = ConsoleColor.DarkGreen;
                        //Console.ForegroundColor = ConsoleColor.DarkYellow;
                        //Console.Write('\u25B2');
                        //Console.BackgroundColor = ConsoleColor.Black;
                        //Console.ForegroundColor = ConsoleColor.Black;
						Console.Write(3);
                    }
                }
                Console.WriteLine();
            }
            if (pid.Contains("Win")){
                Console.Read();
            }
        }

        public static int[,] Transform(float[,] A)
        {
            int[,] B = new int[A.GetLength(0), A.GetLength(1)];

            for (int i = 0; i < A.GetLength(0); i++)
            {
                for (int j = 0; j < A.GetLength(1); j++)
                {
                    if (Math.Round(A[i,j] * 10) < 4)
                    {
                        B[i, j] = 0;
                    }
                    else if (Math.Round(A[i,j] * 10) < 7)
                    {
                        B[i, j] = 1;
                    }
                    else
                    {
                        B[i, j] = 2;
                    }
                }
            }

            return B;
        }

        public static float[][] GenerateWhiteNoise(int width, int height)
        {
            float[][] noise = GetEmptyArray<float>(width, height);

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    noise[i][j] = (float)(random.NextDouble() % 1);
                }
            }

            return noise;
        }

        public static float Interpolate(float x0, float x1, float alpha)
        {
            return x0 * (1 - alpha) + alpha * x1;
        }

       

        public static T[][] GetEmptyArray<T>(int width, int height)
        {
            T[][] image = new T[width][];

            for (int i = 0; i < width; i++)
            {
                image[i] = new T[height];
            }

            return image;
        }

        public static float[][] GenerateSmoothNoise(float[][] baseNoise, int octave)
        {
            int width = baseNoise.Length;
            int height = baseNoise[0].Length;

            float[][] smoothNoise = GetEmptyArray<float>(width, height);

            int samplePeriod = 1 << octave; // calculates 2 ^ k
            float sampleFrequency = 1.0f / samplePeriod;

            for (int i = 0; i < width; i++)
            {
                //calculate the horizontal sampling indices
                int sample_i0 = (i / samplePeriod) * samplePeriod;
                int sample_i1 = (sample_i0 + samplePeriod) % width; //wrap around
                float horizontal_blend = (i - sample_i0) * sampleFrequency;

                for (int j = 0; j < height; j++)
                {
                    //calculate the vertical sampling indices
                    int sample_j0 = (j / samplePeriod) * samplePeriod;
                    int sample_j1 = (sample_j0 + samplePeriod) % height; //wrap around
                    float vertical_blend = (j - sample_j0) * sampleFrequency;

                    //blend the top two corners
                    float top = Interpolate(baseNoise[sample_i0][sample_j0],
                        baseNoise[sample_i1][sample_j0], horizontal_blend);

                    //blend the bottom two corners
                    float bottom = Interpolate(baseNoise[sample_i0][sample_j1],
                        baseNoise[sample_i1][sample_j1], horizontal_blend);

                    //final blend
                    smoothNoise[i][j] = Interpolate(top, bottom, vertical_blend);
                }
            }

            return smoothNoise;
        }

        public static float[,] GeneratePerlinNoise(float[][] baseNoise, int octaveCount)
        {
            int width = baseNoise.Length;
            int height = baseNoise[0].Length;

            float[][][] smoothNoise = new float[octaveCount][][]; //an array of 2D arrays containing

            float persistance = 0.7f;

            //generate smooth noise
            for (int i = 0; i < octaveCount; i++)
            {
                smoothNoise[i] = GenerateSmoothNoise(baseNoise, i);
            }

            float[,] perlinNoise = new float[width, height]; //an array of floats initialised to 0

            float amplitude = 1.0f;
            float totalAmplitude = 0.0f;

            //blend noise together
            for (int octave = octaveCount - 1; octave >= 0; octave--)
            {
                amplitude *= persistance;
                totalAmplitude += amplitude;

                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        perlinNoise[i,j] += smoothNoise[octave][i][j] * amplitude;
                    }
                }
            }

            //normalisation
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    perlinNoise[i,j] /= totalAmplitude;
                }
            }

            return perlinNoise;
        }

        public static float[,] GeneratePerlinNoise(int width, int height, int octaveCount)
        {
            float[][] baseNoise = GenerateWhiteNoise(width, height);

            return GeneratePerlinNoise(baseNoise, octaveCount);
        }
    }
}