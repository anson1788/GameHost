using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraController : MonoBehaviour
{

    // Start is called before the first frame update
    
    void Start()
    {;
        callLibFunction("connectSocket","wss://fmt0duuywk.execute-api.us-east-1.amazonaws.com/uat");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void JavaMessage(string _str){
        callLibFunction("printLogForUnity",_str);
    }

    void onWebSocketConnected(string _data){
        callLibFunction("socketSendMsg","{\"action\":\"registerType\",\"type\":\"gamehost\"}");
    }
    void onWebSocketReceiveData(string _data){
        JavaMessage(_data);
    }

    void callLibFunction(string funcName, string data){
        AndroidJavaClass unity = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
        AndroidJavaClass cSocketHelper = new AndroidJavaClass("com.example.gamehostlib.cSocketHelper");
        cSocketHelper.CallStatic(funcName,data);
    }
}
