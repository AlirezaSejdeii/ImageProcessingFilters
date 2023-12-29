using System.Drawing;

Bitmap originalImage = new Bitmap("input.jpg"); // Replace with your image path

Bitmap gaussianNoiseImage = Helper.AddGaussianNoise(originalImage, 0, 25);
Bitmap saltAndPepperImage = Helper.AddSaltAndPepperNoise(originalImage, 0.05);
Bitmap poissonNoiseImage = Helper.AddPoissonNoise(originalImage);

Bitmap denoisedImage = Helper.RemoveNoise(saltAndPepperImage);

// Save or display images
gaussianNoiseImage.Save("gaussian_noise.jpg");
saltAndPepperImage.Save("salt_and_pepper_noise.jpg");
poissonNoiseImage.Save("poisson_noise.jpg");
denoisedImage.Save("denoised_image.jpg");

public static class Helper
{
     private static readonly Random Rand = new();

    public static Bitmap AddGaussianNoise(Bitmap input, double mean, double stdDev)
    {
        int width = input.Width;
        int height = input.Height;
        Bitmap noisyImage = new Bitmap(width, height);

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Color originalColor = input.GetPixel(i, j);
                int r = Clamp((int)(originalColor.R + GaussianRandom(mean, stdDev)), 0, 255);
                int g = Clamp((int)(originalColor.G + GaussianRandom(mean, stdDev)), 0, 255);
                int b = Clamp((int)(originalColor.B + GaussianRandom(mean, stdDev)), 0, 255);
                noisyImage.SetPixel(i, j, Color.FromArgb(r, g, b));
            }
        }

        return noisyImage;
    }

    public static Bitmap AddSaltAndPepperNoise(Bitmap input, double noiseAmount)
    {
        int width = input.Width;
        int height = input.Height;
        Bitmap noisyImage = new Bitmap(width, height);
        int noisePixels = (int)(width * height * noiseAmount);

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                noisyImage.SetPixel(i, j, input.GetPixel(i, j));
            }
        }

        for (int k = 0; k < noisePixels; k++)
        {
            int i = Rand.Next(width);
            int j = Rand.Next(height);
            Color noiseColor = Rand.NextDouble() < 0.5 ? Color.Black : Color.White;
            noisyImage.SetPixel(i, j, noiseColor);
        }

        return noisyImage;
    }

    public static Bitmap AddPoissonNoise(Bitmap input)
    {
        int width = input.Width;
        int height = input.Height;
        Bitmap noisyImage = new Bitmap(width, height);

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Color originalColor = input.GetPixel(i, j);
                int r = GeneratePoissonNoise(originalColor.R);
                int g = GeneratePoissonNoise(originalColor.G);
                int b = GeneratePoissonNoise(originalColor.B);

                noisyImage.SetPixel(i, j, Color.FromArgb(Clamp(r, 0, 255), Clamp(g, 0, 255), Clamp(b, 0, 255)));
            }
        }

        return noisyImage;
    }

    private static int GeneratePoissonNoise(double lambda)
    {
        double L = Math.Exp(-lambda);
        int k = 0;
        double p = 1.0;

        do
        {
            k++;
            p *= Rand.NextDouble();
        }
        while (p > L);

        return k - 1;
    }

    public static Bitmap RemoveNoise(Bitmap input)
    {
        // Implementing a simple median filter
        int width = input.Width;
        int height = input.Height;
        Bitmap output = new Bitmap(width, height);

        for (int i = 1; i < width - 1; i++)
        {
            for (int j = 1; j < height - 1; j++)
            {
                output.SetPixel(i, j, GetMedianPixel(input, i, j));
            }
        }

        return output;
    }

    private static Color GetMedianPixel(Bitmap input, int x, int y)
    {
        var pixels = new[]
        {
            input.GetPixel(x - 1, y - 1), input.GetPixel(x, y - 1), input.GetPixel(x + 1, y - 1),
            input.GetPixel(x - 1, y), input.GetPixel(x, y), input.GetPixel(x + 1, y),
            input.GetPixel(x - 1, y + 1), input.GetPixel(x, y + 1), input.GetPixel(x + 1, y + 1)
        };

        byte[] rValues = pixels.Select(p => p.R).OrderBy(v => v).ToArray();
        byte[] gValues = pixels.Select(p => p.G).OrderBy(v => v).ToArray();
        byte[] bValues = pixels.Select(p => p.B).OrderBy(v => v).ToArray();

        return Color.FromArgb(rValues[4], gValues[4], bValues[4]);
    }

    private static double GaussianRandom(double mean, double stdDev)
    {
        double u1 = 1.0 - Rand.NextDouble(); //uniform(0,1] random doubles
        double u2 = 1.0 - Rand.NextDouble();
        double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
        return mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)
    }

    private static int Clamp(int value, int min, int max)
    {
        return (value < min) ? min : (value > max) ? max : value;
    }
}