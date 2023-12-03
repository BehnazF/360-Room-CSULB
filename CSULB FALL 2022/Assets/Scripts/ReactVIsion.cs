using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using extOSC;
using Igloo;
using UnityEngine;


public class ReactVIsion : MonoBehaviour
{
    Vector4[] _TUIinfo = new Vector4[6];

    [SerializeField] OSCReceiver _oscReceiver;
    [SerializeField] string _TUIinfo0, _TUIinfo1, _TUIinfo2, _TUIinfo3, _TUIinfo4, _TUIinfo5;

    [SerializeField] int[] _classIDs = new int[6];
    [SerializeField] float _fadeScale = 1;

    int _TUIinfoID0;
    int _TUIinfoID1;
    int _TUIinfoID2;
    int _TUIinfoID3;
    int _TUIinfoID4;
    int _TUIinfoID5;

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
                    _TUIinfo[i] = new Vector4(1, x, y, angle);
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
        for (int i = 0; i < _classIDs.Length; i++)
        {
            //Fade out calculation for videos
            _TUIinfo[i] = new Vector4(Mathf.MoveTowards(_TUIinfo[i][0], 0, Time.deltaTime * _fadeScale), _TUIinfo[i][1], _TUIinfo[i][2], _TUIinfo[i][3]);
        }
    }
}
