using System.Drawing;

Bitmap input = new Bitmap("input.png");

Bitmap grayscale = ConvertToGrayscale(input);
Bitmap histogramEqualization = HistogramEqualization(grayscale);
Bitmap histogramStretching = HistogramStretching(grayscale);
histogramEqualization.Save("histogram-equalization-image.png");
histogramStretching.Save("histogram-stretching-image.png");

static Bitmap ConvertToGrayscale(Bitmap original)
{
    Bitmap grayscale = new Bitmap(original.Width, original.Height);

    for (int y = 0; y < original.Height; y++)
    {
        for (int x = 0; x < original.Width; x++)
        {
            Color pixelColor = original.GetPixel(x, y);
            int grayValue = (int)(pixelColor.R * 0.3 + pixelColor.G * 0.59 + pixelColor.B * 0.11);
            Color grayColor = Color.FromArgb(grayValue, grayValue, grayValue);
            grayscale.SetPixel(x, y, grayColor);
        }
    }

    return grayscale;
}

// Perform histogram equalization
static Bitmap HistogramEqualization(Bitmap grayscale)
{
    int[] histogram = new int[256];
    int totalPixels = grayscale.Width * grayscale.Height;

    // Calculate histogram
    for (int y = 0; y < grayscale.Height; y++)
    {
        for (int x = 0; x < grayscale.Width; x++)
        {
            Color pixelColor = grayscale.GetPixel(x, y);
            histogram[pixelColor.R]++;
        }
    }

    // Calculate cumulative distribution function (CDF)
    int[] cdf = new int[256];
    int sum = 0;
    for (int i = 0; i < 256; i++)
    {
        sum += histogram[i];
        cdf[i] = sum;
    }

    // Normalize CDF
    int minCdf = cdf[0];
    for (int i = 0; i < 256; i++)
    {
        cdf[i] = (int)(((double)(cdf[i] - minCdf) / (totalPixels - minCdf)) * 255);
    }

    // Apply equalization
    Bitmap equalized = new Bitmap(grayscale.Width, grayscale.Height);
    for (int y = 0; y < grayscale.Height; y++)
    {
        for (int x = 0; x < grayscale.Width; x++)
        {
            Color pixelColor = grayscale.GetPixel(x, y);
            int equalizedValue = cdf[pixelColor.R];
            equalized.SetPixel(x, y, Color.FromArgb(equalizedValue, equalizedValue, equalizedValue));
        }
    }

    return equalized;
}

// Perform histogram stretching
static Bitmap HistogramStretching(Bitmap grayscale)
{
    int min = 255;
    int max = 0;

    // Find min and max pixel values
    for (int y = 0; y < grayscale.Height; y++)
    {
        for (int x = 0; x < grayscale.Width; x++)
        {
            Color pixelColor = grayscale.GetPixel(x, y);
            int pixelValue = pixelColor.R;

            if (pixelValue < min)
                min = pixelValue;

            if (pixelValue > max)
                max = pixelValue;
        }
    }

    // Stretch the histogram
    Bitmap stretched = new Bitmap(grayscale.Width, grayscale.Height);
    for (int y = 0; y < grayscale.Height; y++)
    {
        for (int x = 0; x < grayscale.Width; x++)
        {
            Color pixelColor = grayscale.GetPixel(x, y);
            int pixelValue = pixelColor.R;

            int stretchedValue = (int)((double)(pixelValue - min) / (max - min) * 255);
            stretched.SetPixel(x, y, Color.FromArgb(stretchedValue, stretchedValue, stretchedValue));
        }
    }

    return stretched;
}