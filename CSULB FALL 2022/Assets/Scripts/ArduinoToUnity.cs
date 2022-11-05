using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;

public class ArduinoToUnity : MonoBehaviour
{
    public float speed;
    private float amontToMove;
    SerialPort sp = new SerialPort("COM6", 9600);

    // Start is called before the first frame update
    void Start()
    {
        sp.Open();
        sp.ReadTimeout = 1;
    }

    // Update is called once per frame
    void Update()
    {
        amontToMove = speed * Time.deltaTime;
        if (sp.IsOpen)
        {
            try
            {
                moveObject(sp.ReadByte());
                print(sp.ReadByte());
            }
            catch (System.Exception)
            {

            }
        }
    }
    void moveObject(int direction)
    {
        if (direction == 1)
        {
            //move left right: 
            // transform.Translate(Vector3.left * amontToMove, Space.World);
            //rotation:
            transform.Rotate(transform.TransformDirection(Vector3.up), speed * Time.deltaTime);
        }
        if (direction == 2)

        {
            //move left right:  
            // transform.Translate(Vector3.right * amontToMove, Space.World);
            //rotation:
            transform.Rotate(transform.TransformDirection(Vector3.up), -speed * Time.deltaTime);
        }
    }
}
