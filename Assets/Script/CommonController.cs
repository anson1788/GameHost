using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class CommonController : MonoBehaviour
{

    public enum ConnectionMode {WifiMode,BLEMode};
    public static ConnectionMode mConnectionMode = ConnectionMode.WifiMode;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    


    // Update is called once per frame
    void Update()
    {
        
    }
    
    void BLEScanStop(string _data){
         JavaMessage("Stop ble");   
    }

    void BLEScanStart(string _data){
         JavaMessage("Start ble");   
    }

     void displayCalibration(string _data){
         JavaMessage("display calibration");   
    }

    void StartCalibration(string _data){

    }


    void StopCalibration(string _data){
    }
    
    protected void JavaMessage(string _str){
        callLibFunction("printLogForUnity",_str);
    }

    protected void callLibFunction(string funcName, string data){
        AndroidJavaClass cSocketHelper = new AndroidJavaClass("com.example.gamehostlib.cSocketHelper");
        cSocketHelper.CallStatic(funcName,data);
    }

      
    

}
