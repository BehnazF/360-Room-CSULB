using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using extOSC;
using Igloo;
using UnityEngine;


public class ReactVIsion : MonoBehaviour
{
    Vector4[] _TUIinfo = new Vector4[3];

    [SerializeField] OSCReceiver _oscReceiver;
    [SerializeField] string _TUIinfo0, _TUIinfo1, _TUIinfo2;
    [SerializeField] int[] _classIDs = new int[3];

    int _TUIinfoID0;
    int _TUIinfoID1;
    int _TUIinfoID2;

    [SerializeField] Material _material;

    // Start is called before the first frame update
    void Start()
    {
        _oscReceiver.Bind("/tuio/2Dobj", PlayerPositionMessage);
        _TUIinfoID0 = Shader.PropertyToID(_TUIinfo0);
        _TUIinfoID1 = Shader.PropertyToID(_TUIinfo1);
        _TUIinfoID2 = Shader.PropertyToID(_TUIinfo2);

    }

    private void PlayerPositionMessage(OSCMessage msg)
    {
        // string playerName = Utils.StringSplitter(msg.Address, new char[] { '/' }, 2);
        // if (OnPlayerPosition != null) OnPlayerPosition(playerName, new Vector3(msg.Values[0].FloatValue, msg.Values[1].FloatValue, msg.Values[2].FloatValue));
        // Debug.LogFormat("got message {0}", msg.Values.Count);
        // for (int i = 0; i < msg.Values.Count; i++)
        // {
        //     Debug.Log(msg.Values[i]);
        // }

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
            //TODO: change the index based on the classID
        }
    }

    // Update is called once per frame
    void Update()
    {
        _material.SetVector(_TUIinfoID0, _TUIinfo[0]);
        _material.SetVector(_TUIinfoID1, _TUIinfo[1]);
        _material.SetVector(_TUIinfoID2, _TUIinfo[2]);

    }
}
