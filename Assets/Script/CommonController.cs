using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected void JavaMessage(string _str){
        callLibFunction("printLogForUnity",_str);
    }
    protected void callLibFunction(string funcName, string data){
        AndroidJavaClass unity = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
        AndroidJavaClass cSocketHelper = new AndroidJavaClass("com.example.gamehostlib.cSocketHelper");
        cSocketHelper.CallStatic(funcName,data);
    }
}
