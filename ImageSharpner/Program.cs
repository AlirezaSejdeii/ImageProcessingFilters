using System.Drawing;
using System.Drawing.Imaging;

// Load the input image
Bitmap inputImage = new Bitmap("input.png");

Bitmap output = ApplyLaplacianFilter(inputImage);

output.Save("sharp-image-output.jpg", ImageFormat.Jpeg);


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