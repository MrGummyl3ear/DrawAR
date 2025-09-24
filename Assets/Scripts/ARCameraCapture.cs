using UnityEngine;
using UnityEngine.UI;

public class ARCameraCapture : MonoBehaviour
{
    public Camera arCamera; // reference your AR Camera in Inspector

    public Texture2D CaptureCameraView()
    {
        // Create a temporary RenderTexture
        RenderTexture rt = new RenderTexture(Screen.width, Screen.height, 24);
        arCamera.targetTexture = rt;
        arCamera.Render();

        // Activate RT and read pixels
        RenderTexture.active = rt;
        Texture2D snapshot = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);
        snapshot.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        snapshot.Apply();

        // Cleanup
        arCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);


        return snapshot;
    }
    public Texture2D CaptureAndCropUI(RectTransform cropRectTransform)
    {
        Vector3[] corners = new Vector3[4];
        cropRectTransform.GetWorldCorners(corners);

        var bl = RectTransformUtility.WorldToScreenPoint(arCamera, corners[0]);
        var tr = RectTransformUtility.WorldToScreenPoint(arCamera, corners[2]);

        var width = tr.x - bl.x;
        var height = tr.y - bl.y;

        Rect cropRect = new Rect(bl.x, bl.y, width, height);

        RenderTexture rt = new RenderTexture(Screen.width, Screen.height, 24);
        arCamera.targetTexture = rt;
        arCamera.Render();
        RenderTexture.active = rt;

        Texture2D croppedTexture = new Texture2D((int)width, (int)height, TextureFormat.RGB24, false);
        croppedTexture.ReadPixels(cropRect, 0, 0);
        croppedTexture.Apply();

        arCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);

        return croppedTexture;
    }
}

