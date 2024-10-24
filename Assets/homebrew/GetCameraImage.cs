using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetCameraImage : MonoBehaviour
{
    // Notes for later
    // https://answers.unity.com/questions/730857/webcamtexture-on-a-spriterenderer.html
    // https://www.youtube.com/watch?v=4vIpNRJHZCQ
    // https://forum.unity.com/threads/webcamtexture-texture2d.154057/ Reply #13
    // https://docs.unity3d.com/ScriptReference/WebCamTexture.html
    // https://docs.unity3d.com/ScriptReference/Transform.Rotate.html

    // For image gabbing:
    // https://learn.microsoft.com/en-us/windows/mixed-reality/develop/unity/locatable-camera-in-unity
    public MeshRenderer outputMesh;
    public Image outputImage;

    private int camNum = 0;
    private WebCamDevice[] devices;
    private WebCamTexture webCamTexture;

    // Start is called before the first frame update
    void Start()
    {

        // https://docs.unity3d.com/ScriptReference/WebCamTexture-deviceName.html

        devices = WebCamTexture.devices;
        webCamTexture = new WebCamTexture();

        if (devices.Length > 0)
        {
            // 
            webCamTexture.deviceName = devices[camNum].name;
            outputImage.material.mainTexture = webCamTexture;
            webCamTexture.Play();
        }
        else
        {
            // Connect a camera screen
        }
    }

    public void cycleCamera()
    {
        Debug.Log("camNum: " + camNum);
        Debug.Log("devices.Length: " + devices.Length);
        if (camNum + 1 == devices.Length)
        {
            camNum = 0;
        }
        else
        {
            camNum++;
        }
        webCamTexture.Stop();
        webCamTexture.deviceName = devices[camNum].name;
        webCamTexture.Play();
    }
}
