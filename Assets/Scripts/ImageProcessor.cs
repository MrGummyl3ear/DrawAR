using UnityEngine;

public static class ImageProcessor
{
    public static Texture2D SpriteToTexture2D(Sprite sprite)
    {
        Texture2D tex = new Texture2D((int)sprite.textureRect.width, (int)sprite.textureRect.height);
        Color[] pixels = sprite.texture.GetPixels(
            (int)sprite.textureRect.x,
            (int)sprite.textureRect.y,
            (int)sprite.textureRect.width,
            (int)sprite.textureRect.height
        );
        tex.SetPixels(pixels);
        tex.Apply();
        return tex;
    }

    public static Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
    {
        Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, false);
        RenderTexture rt = RenderTexture.GetTemporary(targetWidth, targetHeight);
        rt.filterMode = FilterMode.Bilinear;
        RenderTexture.active = rt;
        Graphics.Blit(source, rt);
        result.ReadPixels(new Rect(0, 0, targetWidth, targetHeight), 0, 0);
        result.Apply();
        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(rt);
        return result;
    }

    public static Texture2D CreateGrayscaleTexture(Texture2D source)
    {
        Texture2D grayscaleTexture = new Texture2D(source.width, source.height);
        Color[] pixels = source.GetPixels();
        for (int i = 0; i < pixels.Length; i++)
        {
            float brightness = pixels[i].grayscale;
            pixels[i] = new Color(brightness, brightness, brightness, 1.0f);
        }
        grayscaleTexture.SetPixels(pixels);
        grayscaleTexture.Apply();
        return grayscaleTexture;
    }

    public static Texture2D DetectEdges(Texture2D source, float threshold = 0.2f)
    {
        Texture2D grayscale = CreateGrayscaleTexture(source);
        Texture2D edgeTexture = new Texture2D(grayscale.width, grayscale.height);
        Color[] pixels = grayscale.GetPixels();
        Color[] edgePixels = new Color[pixels.Length];
        float[,] gx = new float[,] { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } };
        float[,] gy = new float[,] { { 1, 2, 1 }, { 0, 0, 0 }, { -1, -2, -1 } };
        int width = grayscale.width;
        int height = grayscale.height;

        for (int y = 1; y < height - 1; y++)
        {
            for (int x = 1; x < width - 1; x++)
            {
                float newGx = 0;
                float newGy = 0;
                for (int ky = -1; ky <= 1; ky++)
                {
                    for (int kx = -1; kx <= 1; kx++)
                    {
                        int pixelIndex = (y + ky) * width + (x + kx);
                        float pixelValue = pixels[pixelIndex].r;
                        newGx += pixelValue * gx[ky + 1, kx + 1];
                        newGy += pixelValue * gy[ky + 1, kx + 1];
                    }
                }
                float gradient = Mathf.Sqrt((newGx * newGx) + (newGy * newGy));
                int resultIndex = y * width + x;
                edgePixels[resultIndex] = (gradient > threshold) ? Color.black : Color.white;
            }
        }
        edgeTexture.SetPixels(edgePixels);
        edgeTexture.Apply();
        return edgeTexture;
    }
}