
![Alt Text](CSULB FALL 2022/Unity01.png)


**360 ROOM, CSULB, UNITY SETUP**



This project aims to help my students at CSULB for setting up content for Igloo 360 Room:

The main scenes is under "Scenes"> "Example1". 

Please note that you have three different setup options here (you can enable each option and use it in your project):

1. Room Simple:
    has a basic image texture material using Universal Render Pipelinie.
    It responds to Arduino input (pressing two button makes the room video changes). 
    *note you have to change the "port name". eg. "COM6" to your port number.
    *you will also need to add four videos 

    For displaying video in surface of the room using Render Texture, you can watch this tutorial: https://www.youtube.com/watch?v=KG2aq_CY7pU&ab_channel=Unity

    Using ultrasonic sensor: https://randomnerdtutorials.com/complete-guide-for-ultrasonic-sensor-hc-sr04/

2. Room with Graph Shader: 
    has a basic graph shader with a glowing effect which respond to ultrasonic sensor value (distance). 
    * Please note for Graph Shaders to be working you need to install Universal Render Pipeline. Here is a basic tutorial on Graph Shaders: https://www.youtube.com/watch?v=Ar9eIn4z6XE
    * Here is the instruction on how to install the Universal Render Pipeline into an existing Project: https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@7.1/manual/InstallURPIntoAProject.html

