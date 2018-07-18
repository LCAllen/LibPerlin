using System;
namespace LibPerlin
{
	public static class Perlin
	{

		public static int[,] GenerateTerrain(int width, int height)
        {
            float[,] perlinNoise = GeneratePerlinNoise(width, height, 6, 0.7f);
            int[,] terrain = Transform(perlinNoise);
            return terrain;
		}

		public static int[,] GenerateTerrain(int width, int height, int smoothing)
        {
            float[,] perlinNoise = GeneratePerlinNoise(width, height, smoothing, 0.7f);
            int[,] terrain = Transform(perlinNoise);
            return terrain;
        }

		public static int[,] GenerateTerrain(int width, int height, int smoothing, float persistance)
		{
			float[,] perlinNoise = GeneratePerlinNoise(width, height, smoothing, persistance);
			int[,] terrain = Transform(perlinNoise);
			return terrain;
		}

		private static int[,] Transform(float[,] A)
		{
			int[,] B = new int[A.GetLength(0), A.GetLength(1)];

			for (int i = 0; i < A.GetLength(0); i++)
			{
				for (int j = 0; j < A.GetLength(1); j++)
				{

                    // Take the 1 - 10 output range and convert it to 0,1,2.
					if (Math.Round(A[i, j] * 10) < 4)
					{
						B[i, j] = 0;
					}
					else if (Math.Round(A[i, j] * 10) < 7)
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

		// Modified from http://devmag.org.za/2009/04/25/perlin-noise/
        // which is in turn based on Ken Perlin's original paper.

        // Modifications include:
        // Compilation into easy-to use library.
        // Terrain generation function calls.
        // Code now outputs numbers in the range of 0, 1, 2.

		private static float[][] GenerateWhiteNoise(int width, int height)
		{
			Random random = new Random();
			float[][] noise = GetEmptyArray<float>(width, height);

			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					// Convert noise to whole numbers only
					noise[i][j] = (float)(random.NextDouble() % 1);
				}
			}

			return noise;
		}

		private static float Interpolate(float x0, float x1, float alpha)
		{
			return x0 * (1 - alpha) + alpha * x1;
		}

		private static T[][] GetEmptyArray<T>(int width, int height)
		{
			T[][] image = new T[width][];

			for (int i = 0; i < width; i++)
			{
				image[i] = new T[height];
			}

			return image;
		}

		private static float[][] GenerateSmoothNoise(float[][] baseNoise, int octave)
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

		private static float[,] GeneratePerlinNoise(float[][] baseNoise, int octaveCount, float persistance)
		{
			int width = baseNoise.Length;
			int height = baseNoise[0].Length;

			float[][][] smoothNoise = new float[octaveCount][][]; //an array of 2D arrays containing

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
						perlinNoise[i, j] += smoothNoise[octave][i][j] * amplitude;
					}
				}
			}

			//normalisation
			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					perlinNoise[i, j] /= totalAmplitude;
				}
			}

			return perlinNoise;
		}

		private static float[,] GeneratePerlinNoise(int width, int height, int octaveCount, float persistance)
		{
			float[][] baseNoise = GenerateWhiteNoise(width, height);

			return GeneratePerlinNoise(baseNoise, octaveCount, persistance);
		}
	}
}
