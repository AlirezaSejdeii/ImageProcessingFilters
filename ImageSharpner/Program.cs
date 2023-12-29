using System.Drawing;
using System.Drawing.Imaging;

// Load the input image
Bitmap inputImage = new Bitmap("input.png");

Bitmap laplacianFilter = ApplyLaplacianFilter(inputImage);
Bitmap sharpeningFilter = ApplySharpeningFilter(inputImage);

laplacianFilter.Save("laplacian-image-output.jpg", ImageFormat.Jpeg);
sharpeningFilter.Save("sharpening-image-output.jpg", ImageFormat.Jpeg);


static Bitmap ApplyLaplacianFilter(Bitmap input)
{
    // Create a new bitmap to store the processed image
    Bitmap output = new Bitmap(input.Width, input.Height);

    // Define the Laplacian kernel
    int[,] kernel = new int[,] { { 0, -1, 0 }, { -1, 5, -1 }, { 0, -1, 0 } };

    // Loop over each pixel in the image
    for (int x = 1; x < input.Width - 1; x++)
    {
        for (int y = 1; y < input.Height - 1; y++)
        {
            int laplacianValue = 0;

            // Apply the Laplacian kernel to the current pixel
            for (int kx = -1; kx <= 1; kx++)
            {
                for (int ky = -1; ky <= 1; ky++)
                {
                    Color pixelColor = input.GetPixel(x + kx, y + ky);
                    int grayValue = (int)(pixelColor.R * 0.299 + pixelColor.G * 0.587 + pixelColor.B * 0.114);
                    laplacianValue += grayValue * kernel[kx + 1, ky + 1];
                }
            }

            // Normalize the value and set the pixel in the output image
            laplacianValue = Math.Min(Math.Max(laplacianValue, 0), 255);
            output.SetPixel(x, y, Color.FromArgb(laplacianValue, laplacianValue, laplacianValue));
        }
    }

    return output;
}

static Bitmap ApplySharpeningFilter(Bitmap input)
{
    int width = input.Width;
    int height = input.Height;
    Bitmap output = new Bitmap(width, height);

    // Sharpening Kernel
    int[,] kernel = new int[,]
    {
        { -1, -1, -1 },
        { -1, 9, -1 },
        { -1, -1, -1 }
    };

    for (int i = 1; i < width - 1; i++)
    {
        for (int j = 1; j < height - 1; j++)
        {
            output.SetPixel(i, j, ApplyKernel(input, kernel, i, j));
        }
    }

    return output;
}

static Color ApplyKernel(Bitmap input, int[,] kernel, int x, int y)
{
    int red = 0, green = 0, blue = 0;
    int kernelWidth = kernel.GetLength(0);
    int kernelHeight = kernel.GetLength(1);
    int halfKernelWidth = kernelWidth / 2;
    int halfKernelHeight = kernelHeight / 2;

    for (int i = 0; i < kernelWidth; i++)
    {
        for (int j = 0; j < kernelHeight; j++)
        {
            int pixelX = x + (i - halfKernelWidth);
            int pixelY = y + (j - halfKernelHeight);

            Color pixelColor = input.GetPixel(pixelX, pixelY);
            red += pixelColor.R * kernel[i, j];
            green += pixelColor.G * kernel[i, j];
            blue += pixelColor.B * kernel[i, j];
        }
    }

    // Clamp color values to be between 0 and 255
    red = Math.Max(0, Math.Min(255, red));
    green = Math.Max(0, Math.Min(255, green));
    blue = Math.Max(0, Math.Min(255, blue));

    return Color.FromArgb(red, green, blue);
}