using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraSource : MonoBehaviour
{

    WebCamDevice[] _devices;
    [SerializeField] string _targetCamera = "Teslong Camera";
    [SerializeField] string _texturePropertyName = "_targetCamera";
    int _texturePropertyId;
    WebCamTexture _texture = null;
    [SerializeField] Material _material;



    void Start()
    {
        _devices = WebCamTexture.devices;

        Debug.LogFormat("Enumerating {0} devices", _devices.Length);
        for (int i = 0; i < _devices.Length; i++)
        {
            // Debug.Log(_devices[i].name);
            if (_devices[i].name.StartsWith(_targetCamera))
            {
                _texture = new WebCamTexture(_devices[i].name);
                _texture.Play();
            }
        }

        _texturePropertyId = Shader.PropertyToID(_texturePropertyName);
    }

    void OnGUI()
    {
        for (int i = 0; i < _devices.Length; i++)
        {
            GUI.Label(new Rect(0, i * 20, 500, 100),
                System.String.Format("Webcam: {0}", _devices[i].name));
        }
    }

    void Update()
    {
        _material.SetTexture(_texturePropertyId, _texture);
    }
}
