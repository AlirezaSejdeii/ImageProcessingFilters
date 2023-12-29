using System.Drawing;
using System.Drawing.Imaging;

// Load the input image
Bitmap inputImage = new Bitmap("input.jpg");

// Apply a 3x3 average filter
Bitmap average3X3 = ApplyAverageFilter(inputImage, 3);

// Apply a 5x5 average filter
Bitmap average5X5 = ApplyAverageFilter(inputImage, 5);

// Apply a 10*10 average filter
Bitmap average10X10 = ApplyAverageFilter(inputImage, 10);

// Apply a Gaussian filter
Bitmap gaussian = ApplyGaussianFilter(inputImage);

// Save the filtered images
average3X3.Save("average3x3.jpg", ImageFormat.Jpeg);
average5X5.Save("average5x5.jpg", ImageFormat.Jpeg);
average10X10.Save("average10x10.jpg", ImageFormat.Jpeg);
gaussian.Save("gaussian.jpg", ImageFormat.Jpeg);

Console.WriteLine("Filtering complete.");


// Apply a 2D average filter to an image
static Bitmap ApplyAverageFilter(Bitmap inputImage, int filterSize)
{
    int width = inputImage.Width;
    int height = inputImage.Height;
    Bitmap outputImage = new Bitmap(width, height);

    int[,] filter = new int[filterSize, filterSize];
    int filterSum = filterSize * filterSize;

    for (int i = 0; i < filterSize; i++)
    {
        for (int j = 0; j < filterSize; j++)
        {
            filter[i, j] = 1;
        }
    }

    for (int y = 0; y < height; y++)
    {
        for (int x = 0; x < width; x++)
        {
            int sumR = 0;
            int sumG = 0;
            int sumB = 0;

            for (int i = 0; i < filterSize; i++)
            {
                for (int j = 0; j < filterSize; j++)
                {
                    int offsetX = x - filterSize / 2 + i;
                    int offsetY = y - filterSize / 2 + j;

                    if (offsetX >= 0 && offsetX < width && offsetY >= 0 && offsetY < height)
                    {
                        Color pixel = inputImage.GetPixel(offsetX, offsetY);
                        sumR += pixel.R;
                        sumG += pixel.G;
                        sumB += pixel.B;
                    }
                }
            }

            Color filteredPixel = Color.FromArgb(sumR / filterSum, sumG / filterSum, sumB / filterSum);
            outputImage.SetPixel(x, y, filteredPixel);
        }
    }

    return outputImage;
}

// Apply a Gaussian filter to an image
static Bitmap ApplyGaussianFilter(Bitmap inputImage)
{
    int width = inputImage.Width;
    int height = inputImage.Height;
    Bitmap outputImage = new Bitmap(width, height);

    // Define the Gaussian kernel
    double[,] kernel =
    {
        { 1, 2, 1 },
        { 2, 4, 2 },
        { 1, 2, 1 }
    };

    double kernelSum = 16.0;

    for (int y = 0; y < height; y++)
    {
        for (int x = 0; x < width; x++)
        {
            double sumR = 0;
            double sumG = 0;
            double sumB = 0;

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    int offsetX = x + j;
                    int offsetY = y + i;

                    if (offsetX >= 0 && offsetX < width && offsetY >= 0 && offsetY < height)
                    {
                        Color pixel = inputImage.GetPixel(offsetX, offsetY);
                        double factor = kernel[i + 1, j + 1] / kernelSum;
                        sumR += pixel.R * factor;
                        sumG += pixel.G * factor;
                        sumB += pixel.B * factor;
                    }
                }
            }

            int filteredR = (int)Math.Round(sumR);
            int filteredG = (int)Math.Round(sumG);
            int filteredB = (int)Math.Round(sumB);

            filteredR = Math.Max(0, Math.Min(255, filteredR));
            filteredG = Math.Max(0, Math.Min(255, filteredG));
            filteredB = Math.Max(0, Math.Min(255, filteredB));

            Color filteredPixel = Color.FromArgb(filteredR, filteredG, filteredB);
            outputImage.SetPixel(x, y, filteredPixel);
        }
    }

    return outputImage;
}