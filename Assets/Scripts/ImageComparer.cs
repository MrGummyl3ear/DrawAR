using UnityEngine;

public static class ImageComparer
{
    public static float CompareWithTolerance(Texture2D userDrawing, Texture2D sourceOutline, int tolerance)
    {
        Color[] userPixels = userDrawing.GetPixels();
        Color[] sourcePixels = sourceOutline.GetPixels();
        int width = sourceOutline.width;
        int height = sourceOutline.height;
        int totalSourcePixels = 0;
        int matchedSourcePixels = 0;

        for (int i = 0; i < sourcePixels.Length; i++)
        {
            if (sourcePixels[i] == Color.black)
            {
                totalSourcePixels++;
            }
        }

        if (totalSourcePixels == 0) return 1.0f;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int index = y * width + x;
                if (sourcePixels[index] == Color.black)
                {
                    bool foundMatch = false;
                    for (int ty = -tolerance; ty <= tolerance; ty++)
                    {
                        for (int tx = -tolerance; tx <= tolerance; tx++)
                        {
                            int checkX = x + tx;
                            int checkY = y + ty;
                            if (checkX >= 0 && checkX < width && checkY >= 0 && checkY < height)
                            {
                                int userIndex = checkY * width + checkX;
                                if (userPixels[userIndex] == Color.black)
                                {
                                    matchedSourcePixels++;
                                    foundMatch = true;
                                    break;
                                }
                            }
                        }
                        if (foundMatch) break;
                    }
                }
            }
        }
        return (float)matchedSourcePixels / totalSourcePixels;
    }
}