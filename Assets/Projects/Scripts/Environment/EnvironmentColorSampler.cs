using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Unity.Collections;

[RequireComponent(typeof(ARCameraManager))]
public class EnvironmentColorSampler : MonoBehaviour
{
    private ARCameraManager cameraManager;

    private void Awake()
    {
        cameraManager = GetComponent<ARCameraManager>();
    }

    public bool TryGetAverageColor(Vector2 screenPosition, out Color resultColor)
    {
        resultColor = Color.white;

        if (!cameraManager.TryAcquireLatestCpuImage(out XRCpuImage cpuImage))
            return false;

        var conversionParams = new XRCpuImage.ConversionParams
        {
            inputRect = new RectInt(0, 0, cpuImage.width, cpuImage.height),
            outputDimensions = new Vector2Int(64, 64), // downsample for performance
            outputFormat = TextureFormat.RGB24,
            transformation = XRCpuImage.Transformation.MirrorY
        };

        int size = cpuImage.GetConvertedDataSize(conversionParams);

        NativeArray<byte> buffer = new NativeArray<byte>(size, Allocator.Temp);
        cpuImage.Convert(conversionParams, buffer);

        cpuImage.Dispose();

        Texture2D texture = new Texture2D(64, 64, TextureFormat.RGB24, false);
        texture.LoadRawTextureData(buffer);
        texture.Apply();

        buffer.Dispose();

        // Convert screen position to texture coordinate
        Vector2 normalized = new Vector2(
            screenPosition.x / Screen.width,
            screenPosition.y / Screen.height
        );

        int x = Mathf.Clamp((int)(normalized.x * texture.width), 0, texture.width - 1);
        int y = Mathf.Clamp((int)(normalized.y * texture.height), 0, texture.height - 1);

        // Sample small area (3x3 pixels)
        Color total = Color.black;
        int sampleCount = 0;

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                int sx = Mathf.Clamp(x + i, 0, texture.width - 1);
                int sy = Mathf.Clamp(y + j, 0, texture.height - 1);

                total += texture.GetPixel(sx, sy);
                sampleCount++;
            }
        }

        Color avg = total / sampleCount;

        Destroy(texture);

        // Convert to HSV to stabilize lighting
        Color.RGBToHSV(avg, out float h, out float s, out float v);

        s = Mathf.Clamp(s, 0.2f, 0.8f);   // avoid neon or gray
        v = Mathf.Clamp(v, 0.4f, 0.9f);   // avoid too dark/light

        resultColor = Color.HSVToRGB(h, s, v);

        return true;
    }
}
