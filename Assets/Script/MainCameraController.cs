using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;


public class MainCameraController : MonoBehaviour
{

    // Start is called before the first frame update
    public Text centerText;
    void Start()
    {;
        startConnectToSocket();
    }

    

    // Update is called once per frame
    void Update()
    {
        
    }

    void startConnectToSocket(){
        updateCenterText("Start Connecting to Socket");
        callLibFunction("connectSocket","wss://fmt0duuywk.execute-api.us-east-1.amazonaws.com/uat");
    }

    void updateCenterText(string _str){
        centerText.text = _str;
    }
    void JavaMessage(string _str){
        callLibFunction("printLogForUnity",_str);
    }

    void onWebSocketConnected(string _data){
        updateCenterText("Socket connected , joining the room");
        callLibFunction("socketSendMsg","{\"action\":\"registerType\",\"type\":\"gamehost\"}");
    }
    void onWebSocketReceiveData(string _data){
        JavaMessage(_data);
        var N = JSON.Parse(_data);
        var roomid = N["roomid"].Value;  
        updateCenterText("Room ID : "+roomid);
    }

    void callLibFunction(string funcName, string data){
        AndroidJavaClass unity = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
        AndroidJavaClass cSocketHelper = new AndroidJavaClass("com.example.gamehostlib.cSocketHelper");
        cSocketHelper.CallStatic(funcName,data);
    }
}
