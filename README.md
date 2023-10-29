
**360 ROOM, CSULB, UNITY SETUP**

This project aims to help my students at CSULB for setting up content for Igloo 360 Room:

The main scenes is under "Scenes"> "Example1". 

Please note that you have three different setup options here (you can enable each option and use it in your project):

1. Room Simple:
    has a basic image texture material using Universal Render Pipelinie.
    It responds to Arduino input (pressing the button makes the ​room rotates in the opposite direction). 
    *note you have to change the "port name" under the "Arduino to Unity" code. eg. SerialPort sp = new SerialPort("COM6", 9600); >> Change "COM6" to your port number.

    For displaying video in surface of the room using Render Texture, you can watch this tutorial: https://www.youtube.com/watch?v=KG2aq_CY7pU&ab_channel=Unity

2. Room with Graph Shader: 
    has a basic graph shader with a glowing effect. 
    * Please note for Graph Shaders to be working you need to install Universal Render Pipeline. Here is a basic tutorial on Graph Shaders: https://www.youtube.com/watch?v=Ar9eIn4z6XE
    * Here is the instruction on how to install the Universal Render Pipeline into an existing Project: https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@7.1/manual/InstallURPIntoAProject.html

3. Room HAP:
    This is for including high res videos in the room. I am using KlakHAP is a Unity plugin that allows playing back a video stream encoded with the HAP video codecs. https://github.com/keijiro/KlakHap
