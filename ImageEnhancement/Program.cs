using System.Drawing;

Bitmap originalImage = new Bitmap("input.jpg");

Bitmap linearImage = LinearEnhancement(originalImage);
Bitmap piecewiseLinearImage = PiecewiseLinearEnhancement(originalImage);
Bitmap powerImage = PowerLawEnhancement(originalImage, 2.2);
Bitmap logarithmicImage = LogarithmicEnhancement(originalImage);
Bitmap gammaCorrectedImage = GammaCorrection(originalImage, 2.2);

linearImage.Save("linear_enhancement.jpg");
piecewiseLinearImage.Save("piecewise_linear_enhancement.jpg");
powerImage.Save("power_law_enhancement.jpg");
logarithmicImage.Save("logarithmic_enhancement.jpg");
gammaCorrectedImage.Save("gamma_correction.jpg");

// Linear Enhancement
static Bitmap LinearEnhancement(Bitmap input)
{
    Bitmap output = new Bitmap(input.Width, input.Height);
    // You can adjust these values
    int min = 0, max = 255;

    for (int x = 0; x < input.Width; x++)
    {
        for (int y = 0; y < input.Height; y++)
        {
            Color pixel = input.GetPixel(x, y);
            int red = Stretch(pixel.R, min, max);
            int green = Stretch(pixel.G, min, max);
            int blue = Stretch(pixel.B, min, max);

            output.SetPixel(x, y, Color.FromArgb(pixel.A, red, green, blue));
        }
    }

    return output;
}

static int Stretch(int value, int min, int max)
{
    return (value - min) * 255 / (max - min);
}

// Piecewise Linear Enhancement
static Bitmap PiecewiseLinearEnhancement(Bitmap input)
{
    Bitmap output = new Bitmap(input.Width, input.Height);

    for (int x = 0; x < input.Width; x++)
    {
        for (int y = 0; y < input.Height; y++)
        {
            Color pixel = input.GetPixel(x, y);
            int red = PiecewiseStretch(pixel.R);
            int green = PiecewiseStretch(pixel.G);
            int blue = PiecewiseStretch(pixel.B);

            output.SetPixel(x, y, Color.FromArgb(pixel.A, red, green, blue));
        }
    }

    return output;
}

static int PiecewiseStretch(int value)
{
    if (value < 128)
        return value * 2;
    else
        return value;
}

// Power-Law (Gamma) Enhancement
static Bitmap PowerLawEnhancement(Bitmap input, double gamma)
{
    Bitmap output = new Bitmap(input.Width, input.Height);

    for (int x = 0; x < input.Width; x++)
    {
        for (int y = 0; y < input.Height; y++)
        {
            Color pixel = input.GetPixel(x, y);
            double red = Math.Pow(pixel.R / 255.0, gamma) * 255;
            double green = Math.Pow(pixel.G / 255.0, gamma) * 255;
            double blue = Math.Pow(pixel.B / 255.0, gamma) * 255;

            output.SetPixel(x, y, Color.FromArgb(pixel.A, (int)red, (int)green, (int)blue));
        }
    }

    return output;
}

// Logarithmic Enhancement
static Bitmap LogarithmicEnhancement(Bitmap input)
{
    Bitmap output = new Bitmap(input.Width, input.Height);

    for (int x = 0; x < input.Width; x++)
    {
        for (int y = 0; y < input.Height; y++)
        {
            Color pixel = input.GetPixel(x, y);
            int red = (int)(255 * Math.Log(1 + pixel.R) / Math.Log(256));
            int green = (int)(255 * Math.Log(1 + pixel.G) / Math.Log(256));
            int blue = (int)(255 * Math.Log(1 + pixel.B) / Math.Log(256));

            output.SetPixel(x, y, Color.FromArgb(pixel.A, red, green, blue));
        }
    }

    return output;
}

// Gamma Correction
static Bitmap GammaCorrection(Bitmap input, double gamma)
{
    Bitmap output = new Bitmap(input.Width, input.Height);

    for (int x = 0; x < input.Width; x++)
    {
        for (int y = 0; y < input.Height; y++)
        {
            Color pixel = input.GetPixel(x, y);
            double red = Math.Pow(pixel.R / 255.0, 1 / gamma) * 255;
            double green = Math.Pow(pixel.G / 255.0, 1 / gamma) * 255;
            double blue = Math.Pow(pixel.B / 255.0, 1 / gamma) * 255;

            output.SetPixel(x, y, Color.FromArgb(pixel.A, (int)red, (int)green, (int)blue));
        }
    }

    return output;
}