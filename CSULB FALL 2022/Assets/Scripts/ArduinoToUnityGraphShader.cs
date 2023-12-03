using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;

public class ArduinoToUnityGraphShader : MonoBehaviour
{
    [SerializeField] UnityEngine.Video.VideoPlayer _videoPlayer;
    [SerializeField] UnityEngine.Video.VideoClip[] _videoClips;
    [SerializeField] bool _showDebugUI = true;
    [SerializeField] string _serialPortIdentifier = "COM4";
    [SerializeField] string _shaderProperty;
    int _shaderId;
    [SerializeField] Material _material;
    SerialPort _serialPort;
    bool _buttonPressed = false;
    bool _button2Pressed = false;
    int _ultrasonicDistanceCm = 0;

    float _smoothedUltrasonicDistance = 0;
    [SerializeField] float _smoothValue = 10;

    // Start is called before the first frame update
    void Start()
    {
        _serialPort = new SerialPort(_serialPortIdentifier, 9600);
        _serialPort.Open();
        _serialPort.ReadTimeout = 1;
        _shaderId = Shader.PropertyToID(_shaderProperty);
    }

    // Update is called once per frame
    void Update()
    {
        ReadMessagesFromArduino();

        if (!_buttonPressed && !_button2Pressed)
        {
            _videoPlayer.clip = _videoClips[0];
        }

        else if (!_buttonPressed && _button2Pressed)
        {
            _videoPlayer.clip = _videoClips[1];
        }

        else if (_buttonPressed && !_button2Pressed)
        {
            _videoPlayer.clip = _videoClips[2];
        }

        else if (_buttonPressed && _button2Pressed)
        {
            _videoPlayer.clip = _videoClips[3];
        }

        _smoothedUltrasonicDistance = Mathf.MoveTowards(_smoothedUltrasonicDistance, _ultrasonicDistanceCm, Time.deltaTime * _smoothValue);
        _material.SetFloat(_shaderId, _smoothedUltrasonicDistance);

    }

    void OnGUI()
    {
        if (_showDebugUI)
        {
            GUI.Label(new Rect(0, 0, 500, 100),
                System.String.Format("Button:{0} Button2:{1} Ultrasonic:{2}", _buttonPressed, _button2Pressed, _ultrasonicDistanceCm));
            GUI.Label(new Rect(0, 50, 500, 100),
                System.String.Format("Video Time: {0}", _videoPlayer.time));
        }
    }

    enum SensorKind
    {
        None = 0,
        Button = 1,
        Ultrasonic = 2,
        Button2 = 3,
    }

    void HandleMessage(byte[] buffer)
    {
        // Debug.LogFormat("Data: {0} {1} {2} {3}", buffer[0], buffer[1], buffer[2], buffer[3]);
        switch (buffer[1])
        {
            case (byte)SensorKind.Button:
                {
                    _buttonPressed = buffer[2] != 0;
                    // Debug.LogFormat("Button: {0}", _buttonPressed);
                }
                break;
            case (byte)SensorKind.Button2:
                {
                    _button2Pressed = buffer[2] != 0;
                    // Debug.LogFormat("Button2: {0}", _button2Pressed);
                }
                break;
            case (byte)SensorKind.Ultrasonic:
                {
                    _ultrasonicDistanceCm = buffer[2] | (buffer[3] << 8);
                    // Debug.LogFormat("Ultrasonic: {0}", _ultrasonicDistanceCm);
                }
                break;
        }
    }

    // Where to write into _bufferFromArduino next
    int _bufferIndex = 0;
    byte[] _bufferFromArduino = new byte[4];
    const int MSG_HEADER = 254;
    void ReadMessagesFromArduino()
    {
        if (_serialPort.IsOpen)
        {
            try
            {
                int maxBytesToReadPerFrame = _bufferFromArduino.Length * 100;
                for (int readAttempt = 0; readAttempt < maxBytesToReadPerFrame; readAttempt++)
                {
                    int nextByte = _serialPort.ReadByte();
                    if (nextByte != -1)
                    {
                        if (_bufferIndex == 0)
                        {
                            if (nextByte == MSG_HEADER)
                            {
                                _bufferFromArduino[_bufferIndex] = (byte)nextByte;
                                _bufferIndex += 1;
                            }
                        }
                        else
                        {
                            _bufferFromArduino[_bufferIndex] = (byte)nextByte;
                            _bufferIndex = _bufferIndex + 1;
                        }
                    }
                    else
                    {
                        break;
                    }

                    if (_bufferIndex >= _bufferFromArduino.Length)
                    {
                        _bufferIndex = 0;
                        HandleMessage(_bufferFromArduino);
                    }
                }
            }
            catch (System.Exception)
            {

            }
        }
    }
}
