using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using Unity.Barracuda;

/* Preprocessing to scale down WebCamTexture 
   @author Third Aurora
   https://github.com/Third-Aurora/Barracuda-Image-Classification
*/

public class Preprocess : MonoBehaviour
{
    RenderTexture renderTexture;
    Vector2 scale = new Vector2(1, 1);
    Vector2 offset = Vector2.zero;

    UnityAction<byte[]> callback;

    public void ScaleAndCropImage(WebCamTexture webCamTexture, int desiredSize, UnityAction<byte[]> callback) {

        this.callback = callback;

        if (renderTexture == null) {
            renderTexture = new RenderTexture(desiredSize, desiredSize,0,RenderTextureFormat.ARGB32);
        }

        scale.x = (float)webCamTexture.height / (float)webCamTexture.width;
        offset.x = (1 - scale.x) / 2f;
        Graphics.Blit(webCamTexture, renderTexture, scale, offset);
        AsyncGPUReadback.Request(renderTexture, 0, TextureFormat.RGB24, OnCompleteReadback);
    }

    void OnCompleteReadback(AsyncGPUReadbackRequest request) {

        if (request.hasError) {
            Debug.Log("GPU readback error detected.");
            return;
        }

        callback.Invoke(request.GetData<byte>().ToArray());
    }

    private Texture2D Scale(Texture2D texture, int imageSize)
    {
        var scaled = TextureTools.scaled(texture, imageSize, imageSize, FilterMode.Bilinear);

        return scaled;
    }


    private Color32[] Rotate(Color32[] pixels, int width, int height)
    {
        return TextureTools.RotateImageMatrix(
                pixels, width, height, -90);
    }

    // Function to preprocess image for YOLO v2
    // From https://github.com/Syn-McJ/TFClassify-Unity-Barracuda/blob/master/Assets/Scripts/PhoneCamera.cs
    public IEnumerator ProcessImage(WebCamTexture wct, int inputSize, System.Action<Color32[]> callback)
    {
        yield return StartCoroutine(TextureTools.CropSquare(wct,
            TextureTools.RectOptions.Center, snap =>
            {
                var scaled = Scale(snap, inputSize);
                //var rotated = Rotate(scaled.GetPixels32(), scaled.width, scaled.height);
                callback(scaled.GetPixels32());
            }));
    }

    // Process with various cropping
    public IEnumerator ProcessImage(WebCamTexture wct, int inputSize, int crop, System.Action<Color32[]> callback)
    {
        yield return StartCoroutine(TextureTools.CropSquare(wct,
        (TextureTools.RectOptions)crop, snap =>
            {
                var scaled = Scale(snap, inputSize);
                //var rotated = Rotate(scaled.GetPixels32(), scaled.width, scaled.height);
                callback(scaled.GetPixels32());
            }));
    }

    // Transform Color32 data to Tensor for YOLO v2
    // From https://github.com/Syn-McJ/TFClassify-Unity-Barracuda/blob/master/Assets/Scripts/Detector.cs
    public Tensor TransformImageData(Color32[] pic, int width, int height, int img_mean, float img_std)
    {
        float[] floatValues = new float[width * height * 3];

        for (int i = 0; i < pic.Length; ++i)
        {
            var color = pic[i];

            floatValues[i * 3 + 0] = (color.r - img_mean) / img_std;
            floatValues[i * 3 + 1] = (color.g - img_mean) / img_std;
            floatValues[i * 3 + 2] = (color.b - img_mean) / img_std;
        }

        return new Tensor(1, height, width, 3, floatValues);
    }
}
