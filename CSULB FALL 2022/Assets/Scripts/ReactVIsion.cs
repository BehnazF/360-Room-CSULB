using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using extOSC;
using Igloo;
using UnityEngine;


public class ReactVIsion : MonoBehaviour
{
    Vector4[] _TUIinfo = new Vector4[12];
    [SerializeField] Vector2[] _tableMinMax = new Vector2[2];
    [SerializeField] Vector2[] _roomMinMax = new Vector2[2];
    [SerializeField] OSCReceiver _oscReceiver;
    [SerializeField]
    string _TUIinfo0, _TUIinfo1, _TUIinfo2, _TUIinfo3, _TUIinfo4, _TUIinfo5,
    _TUIinfo6, _TUIinfo7, _TUIinfo8, _TUIinfo9, _TUIinfo10, _TUIinfo11;

    [SerializeField] int[] _classIDs = new int[12];
    [SerializeField] float _fadeScale = 1;

    int _TUIinfoID0;
    int _TUIinfoID1;
    int _TUIinfoID2;
    int _TUIinfoID3;
    int _TUIinfoID4;
    int _TUIinfoID5;
    int _TUIinfoID6;
    int _TUIinfoID7;
    int _TUIinfoID8;
    int _TUIinfoID9;
    int _TUIinfoID10;
    int _TUIinfoID11;

    [SerializeField] Material _material;

    // Start is called before the first frame update
    void Start()
    {
        _oscReceiver.Bind("/tuio/2Dobj", gotTUIinfo);

        _TUIinfoID0 = Shader.PropertyToID(_TUIinfo0);
        _TUIinfoID1 = Shader.PropertyToID(_TUIinfo1);
        _TUIinfoID2 = Shader.PropertyToID(_TUIinfo2);
        _TUIinfoID3 = Shader.PropertyToID(_TUIinfo3);
        _TUIinfoID4 = Shader.PropertyToID(_TUIinfo4);
        _TUIinfoID5 = Shader.PropertyToID(_TUIinfo5);
        _TUIinfoID6 = Shader.PropertyToID(_TUIinfo6);
        _TUIinfoID7 = Shader.PropertyToID(_TUIinfo7);
        _TUIinfoID8 = Shader.PropertyToID(_TUIinfo8);
        _TUIinfoID9 = Shader.PropertyToID(_TUIinfo9);
        _TUIinfoID10 = Shader.PropertyToID(_TUIinfo10);
        _TUIinfoID11 = Shader.PropertyToID(_TUIinfo11);
    }

    private void gotTUIinfo(OSCMessage msg)
    {

        if (msg.Values.Count == 11)
        {
            int sessionID = msg.Values[1].IntValue;
            int classID = msg.Values[2].IntValue;
            float x = msg.Values[3].FloatValue;
            float y = 1 - (msg.Values[4].FloatValue);
            float angle = msg.Values[5].FloatValue;
            Debug.LogFormat("{0} {1} {2} {3} {4}", sessionID, classID, x, y, angle);
            for (int i = 0; i < _classIDs.Length; i++)
            {
                if (classID == _classIDs[i])
                {
                    _TUIinfo[i] = new Vector4(1,
                        Mathf.Lerp(_roomMinMax[0].x, _roomMinMax[1].x, Mathf.InverseLerp(_tableMinMax[0].x, _tableMinMax[1].x, x)),
                        Mathf.Lerp(_roomMinMax[0].y, _roomMinMax[1].y, Mathf.InverseLerp(_tableMinMax[0].y, _tableMinMax[1].y, y)),
                        angle);
                }
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        _material.SetVector(_TUIinfoID0, _TUIinfo[0]);
        _material.SetVector(_TUIinfoID1, _TUIinfo[1]);
        _material.SetVector(_TUIinfoID2, _TUIinfo[2]);
        _material.SetVector(_TUIinfoID3, _TUIinfo[3]);
        _material.SetVector(_TUIinfoID4, _TUIinfo[4]);
        _material.SetVector(_TUIinfoID5, _TUIinfo[5]);
        _material.SetVector(_TUIinfoID6, _TUIinfo[6]);
        _material.SetVector(_TUIinfoID7, _TUIinfo[7]);
        _material.SetVector(_TUIinfoID8, _TUIinfo[8]);
        _material.SetVector(_TUIinfoID9, _TUIinfo[9]);
        _material.SetVector(_TUIinfoID10, _TUIinfo[10]);
        _material.SetVector(_TUIinfoID11, _TUIinfo[11]);
        for (int i = 0; i < _classIDs.Length; i++)
        {
            //Fade out calculation for videos
            _TUIinfo[i] = new Vector4(Mathf.MoveTowards(_TUIinfo[i][0], 0, Time.deltaTime * _fadeScale), _TUIinfo[i][1], _TUIinfo[i][2], _TUIinfo[i][3]);
        }
    }
}
