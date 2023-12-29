using System.Drawing;

Bitmap originalImage = new Bitmap("input.jpg");

Bitmap sobelImage = EdgeDetection.SobelFilter(originalImage);
Bitmap prewittImage = EdgeDetection.PrewittFilter(originalImage);
Bitmap robertsImage = EdgeDetection.RobertsFilter(originalImage);
Bitmap logImage = EdgeDetection.LoGFilter(originalImage);
Bitmap marrHildrethFilter = EdgeDetection.MarrHildrethFilter(originalImage);
Bitmap cannyEdgeBitmap = EdgeDetection.ApplyCanny(originalImage, 1.4, 20, 40);


cannyEdgeBitmap.Save("canny_result.jpg");
sobelImage.Save("sobel_result.jpg");
prewittImage.Save("prewitt_result.jpg");
robertsImage.Save("roberts_result.jpg");
marrHildrethFilter.Save("marr_hilderth.jpg");
logImage.Save("log_result.jpg");


public static class EdgeDetection
{
    // Define kernels
    private static readonly int[,] SobelXKernel = { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } };
    private static readonly int[,] SobelYKernel = { { -1, -2, -1 }, { 0, 0, 0 }, { 1, 2, 1 } };
    private static readonly int[,] PrewittXKernel = { { -1, 0, 1 }, { -1, 0, 1 }, { -1, 0, 1 } };
    private static readonly int[,] PrewittYKernel = { { -1, -1, -1 }, { 0, 0, 0 }, { 1, 1, 1 } };
    private static readonly int[,] RobertsXKernel = { { 1, 0 }, { 0, -1 } };
    private static readonly int[,] RobertsYKernel = { { 0, 1 }, { -1, 0 } };
    private static readonly int[,] LogKernel = { { 0, 1, 0 }, { 1, -4, 1 }, { 0, 1, 0 } };
    private static readonly int[,] LaplacianKernel = { { 0, -1, 0 }, { -1, 4, -1 }, { 0, -1, 0 } };

    public static Bitmap SobelFilter(Bitmap input) => ApplyConvolutionFilter(input, SobelXKernel, SobelYKernel);
    public static Bitmap PrewittFilter(Bitmap input) => ApplyConvolutionFilter(input, PrewittXKernel, PrewittYKernel);
    public static Bitmap RobertsFilter(Bitmap input) => ApplyConvolutionFilter(input, RobertsXKernel, RobertsYKernel);
    public static Bitmap LoGFilter(Bitmap input) => ApplyConvolutionFilter(input, LogKernel, null!);

    public static Bitmap MarrHildrethFilter(Bitmap input)
    {
        // Apply Gaussian blur
        Bitmap blurredImage = GaussianBlur(input);
        // Apply Laplacian
        return ApplyConvolutionFilter(blurredImage, LaplacianKernel, null);
    }

    private static Bitmap ApplyConvolutionFilter(Bitmap input, int[,] xKernel, int[,] yKernel)
    {
        Bitmap output = new Bitmap(input.Width, input.Height);

        for (int i = 1; i < input.Width - 1; i++)
        {
            for (int j = 1; j < input.Height - 1; j++)
            {
                int? gx = xKernel != null ? ApplyKernel(input, xKernel, i, j) : null;
                int? gy = yKernel != null ? ApplyKernel(input, yKernel, i, j) : null;

                int gradientMagnitude = (int)Math.Sqrt((gx ?? 0) * (gx ?? 0) + (gy ?? 0) * (gy ?? 0));
                gradientMagnitude = Math.Min(255, Math.Max(0, gradientMagnitude));
                output.SetPixel(i, j, Color.FromArgb(gradientMagnitude, gradientMagnitude, gradientMagnitude));
            }
        }

        return output;
    }

    private static int ApplyKernel(Bitmap input, int[,] kernel, int x, int y)
    {
        int kernelWidth = kernel.GetLength(0);
        int kernelHeight = kernel.GetLength(1);
        int halfKernelWidth = kernelWidth / 2;
        int halfKernelHeight = kernelHeight / 2;
        int kernelSum = 0;

        for (int i = 0; i < kernelWidth; i++)
        {
            for (int j = 0; j < kernelHeight; j++)
            {
                int pixelX = x + (i - halfKernelWidth);
                int pixelY = y + (j - halfKernelHeight);

                if (pixelX < 0 || pixelY < 0 || pixelX >= input.Width || pixelY >= input.Height)
                {
                    continue;
                }

                Color pixelColor = input.GetPixel(pixelX, pixelY);
                int grayValue = (pixelColor.R + pixelColor.G + pixelColor.B) / 3;
                kernelSum += grayValue * kernel[i, j];
            }
        }

        return kernelSum;
    }

    private static Bitmap GaussianBlur(Bitmap input)
    {
        int[,] gaussianKernel = { { 1, 2, 1 }, { 2, 4, 2 }, { 1, 2, 1 } }; // Example 3x3 Gaussian kernel
        return ApplyConvolutionFilter(input, gaussianKernel, null!);
    }

    public static Bitmap ApplyCanny(Bitmap input, double sigma, double thresholdLow, double thresholdHigh)
    {
        // Step 1: Gaussian Blur
        Bitmap blurred = GaussianBlur(input, 1.4);

        // Step 2: Gradient Calculation using Sobel operator
        var gradients = CalculateGradients(blurred);
        Bitmap gradientMagnitude = gradients.Item1;
        Bitmap gradientDirection = gradients.Item2;

        // Step 3: Non-Maximum Suppression
        Bitmap nmsResult = NonMaximumSuppression(gradientMagnitude, gradientDirection);

        // Step 4: Hysteresis Thresholding
        Bitmap finalEdges = Hysteresis(nmsResult, thresholdLow, thresholdHigh);

        return finalEdges;
    }

    private static Bitmap GaussianBlur(Bitmap input, double sigma)
    {
        int size = (int)(6 * sigma) | 1; // Kernel size (ensure it's odd)
        double[,] kernel = CreateGaussianKernel(size, sigma);
        return ApplyConvolution(input, kernel);
    }

    private static Bitmap ApplyConvolution(Bitmap input, double[,] kernel)
    {
        Bitmap output = new Bitmap(input.Width, input.Height);
        int kernelWidth = kernel.GetLength(0);
        int kernelHeight = kernel.GetLength(1);
        int halfKernelWidth = kernelWidth / 2;
        int halfKernelHeight = kernelHeight / 2;

        for (int i = halfKernelWidth; i < input.Width - halfKernelWidth; i++)
        {
            for (int j = halfKernelHeight; j < input.Height - halfKernelHeight; j++)
            {
                double sumR = 0, sumG = 0, sumB = 0;
                for (int k = -halfKernelWidth; k <= halfKernelWidth; k++)
                {
                    for (int l = -halfKernelHeight; l <= halfKernelHeight; l++)
                    {
                        Color pixelColor = input.GetPixel(i + k, j + l);
                        double kernelVal = kernel[k + halfKernelWidth, l + halfKernelHeight];

                        sumR += pixelColor.R * kernelVal;
                        sumG += pixelColor.G * kernelVal;
                        sumB += pixelColor.B * kernelVal;
                    }
                }

                int R = Math.Min(Math.Max((int)sumR, 0), 255);
                int G = Math.Min(Math.Max((int)sumG, 0), 255);
                int B = Math.Min(Math.Max((int)sumB, 0), 255);

                output.SetPixel(i, j, Color.FromArgb(R, G, B));
            }
        }

        return output;
    }

    private static double[,] CreateGaussianKernel(int size, double sigma)
    {
        double[,] kernel = new double[size, size];
        double sumTotal = 0;
        int kernelRadius = size / 2;

        for (int y = -kernelRadius; y <= kernelRadius; y++)
        {
            for (int x = -kernelRadius; x <= kernelRadius; x++)
            {
                double distance = ((x * x) + (y * y)) / (2 * (sigma * sigma));
                kernel[y + kernelRadius, x + kernelRadius] = Math.Exp(-distance) / (Math.PI * 2 * sigma * sigma);
                sumTotal += kernel[y + kernelRadius, x + kernelRadius];
            }
        }

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                kernel[y, x] = kernel[y, x] * (1.0 / sumTotal); // Normalize
            }
        }

        return kernel;
    }

    private static Tuple<Bitmap, Bitmap> CalculateGradients(Bitmap input)
    {
        int width = input.Width;
        int height = input.Height;
        Bitmap gradientMagnitude = new Bitmap(width, height);
        Bitmap gradientDirection = new Bitmap(width, height);

        // Sobel Operator Kernels
        int[,] xKernel = { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } };
        int[,] yKernel = { { -1, -2, -1 }, { 0, 0, 0 }, { 1, 2, 1 } };

        for (int i = 1; i < width - 1; i++)
        {
            for (int j = 1; j < height - 1; j++)
            {
                int gx = ApplyKernel(input, xKernel, i, j);
                int gy = ApplyKernel(input, yKernel, i, j);

                // Calculate gradient magnitude and clamp it within [0, 255]
                int mag = (int)Math.Sqrt(gx * gx + gy * gy);
                mag = Math.Min(Math.Max(mag, 0), 255);

                gradientMagnitude.SetPixel(i, j, Color.FromArgb(mag, mag, mag));
                gradientDirection.SetPixel(i, j,
                    Color.FromArgb(ClampAngle(Math.Atan2(gy, gx) * (180 / Math.PI)), 0, 0));
            }
        }

        return new Tuple<Bitmap, Bitmap>(gradientMagnitude, gradientDirection);
    }

    private static int ClampAngle(double angle)
    {
        // Ensure angle is within [0, 255]
        return Math.Min(Math.Max((int)angle, 0), 255);
    }

    private static Bitmap NonMaximumSuppression(Bitmap gradientMagnitude, Bitmap gradientDirection)
    {
        int width = gradientMagnitude.Width;
        int height = gradientMagnitude.Height;
        Bitmap nmsResult = new Bitmap(width, height);

        for (int i = 1; i < width - 1; i++)
        {
            for (int j = 1; j < height - 1; j++)
            {
                float angle = gradientDirection.GetPixel(i, j).R; // Angle in degrees
                int q = 255, r = 255;
                int currentMagnitude = gradientMagnitude.GetPixel(i, j).R;

                // Approximate the angle to one of four possible directions
                if ((angle >= 0 && angle < 22.5) || (angle >= 157.5 && angle <= 180))
                {
                    q = gradientMagnitude.GetPixel(i, j + 1).R;
                    r = gradientMagnitude.GetPixel(i, j - 1).R;
                }
                else if (angle >= 22.5 && angle < 67.5)
                {
                    q = gradientMagnitude.GetPixel(i + 1, j - 1).R;
                    r = gradientMagnitude.GetPixel(i - 1, j + 1).R;
                }
                else if (angle >= 67.5 && angle < 112.5)
                {
                    q = gradientMagnitude.GetPixel(i + 1, j).R;
                    r = gradientMagnitude.GetPixel(i - 1, j).R;
                }
                else if (angle >= 112.5 && angle < 157.5)
                {
                    q = gradientMagnitude.GetPixel(i - 1, j - 1).R;
                    r = gradientMagnitude.GetPixel(i + 1, j + 1).R;
                }

                // Suppress pixels at the non-maxima
                if (currentMagnitude >= q && currentMagnitude >= r)
                    nmsResult.SetPixel(i, j, Color.FromArgb(currentMagnitude, currentMagnitude, currentMagnitude));
                else
                    nmsResult.SetPixel(i, j, Color.FromArgb(0, 0, 0));
            }
        }

        return nmsResult;
    }

    private static Bitmap Hysteresis(Bitmap input, double thresholdLow, double thresholdHigh)
    {
        int width = input.Width;
        int height = input.Height;
        Bitmap output = new Bitmap(width, height);

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                int intensity = input.GetPixel(i, j).R;
                if (intensity >= thresholdHigh)
                {
                    output.SetPixel(i, j, Color.FromArgb(255, 255, 255));
                }
                else if (intensity < thresholdLow)
                {
                    output.SetPixel(i, j, Color.FromArgb(0, 0, 0));
                }
                else
                {
                    bool connectedToStrongEdge = false;
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        for (int dy = -1; dy <= 1; dy++)
                        {
                            if (dx == 0 && dy == 0)
                                continue;

                            int x = i + dx, y = j + dy;
                            if (x < 0 || y < 0 || x >= width || y >= height)
                                continue;

                            if (input.GetPixel(x, y).R >= thresholdHigh)
                            {
                                connectedToStrongEdge = true;
                                break;
                            }
                        }

                        if (connectedToStrongEdge)
                            break;
                    }

                    if (connectedToStrongEdge)
                        output.SetPixel(i, j, Color.FromArgb(255, 255, 255));
                    else
                        output.SetPixel(i, j, Color.FromArgb(0, 0, 0));
                }
            }
        }

        return output;
    }
}