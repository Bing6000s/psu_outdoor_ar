using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetCameraImage : MonoBehaviour
{
    public MeshRenderer outputMesh;
    public Image outputImage;

    private int camNum = 0;
    private WebCamDevice[] devices;
    private WebCamTexture webCamTexture;

    void Start()
    {
        InitializeCamera();
    }

    private void InitializeCamera()
    {
        devices = WebCamTexture.devices;

        if (devices.Length > 0)
        {
            if (webCamTexture == null || !webCamTexture.isPlaying)
            {
                webCamTexture = new WebCamTexture(devices[camNum].name);
                outputImage.material.mainTexture = webCamTexture;
                webCamTexture.Play();
            }
        }
        else
        {
            Debug.LogError("No camera devices found.");
        }
    }

    void OnEnable()
    {
        if (webCamTexture == null)
        {
            InitializeCamera();
        }
        else if (!webCamTexture.isPlaying)
        {
            webCamTexture.Play();
        }
    }

void OnDisable()
{
    if (webCamTexture != null)
    {
        webCamTexture.Stop();
        webCamTexture = null;
    }
}

void OnApplicationQuit()
{
    if (webCamTexture != null)
    {
        webCamTexture.Stop();
        webCamTexture = null;
    }
}
    public void cycleCamera()
    {
        if (devices.Length > 0)
        {
            webCamTexture.Stop();
            camNum = (camNum + 1) % devices.Length;
            webCamTexture.deviceName = devices[camNum].name;
            webCamTexture.Play();
        }
    }
}