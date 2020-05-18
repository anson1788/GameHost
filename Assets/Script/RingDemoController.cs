using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using UnityEngine.UI;
using System;

public class RingDemoController : RingAnimationController
{

    public Animator anim;
    public Text centerText;

    /*
    Quaternion        resulting;
    Quaternion        beginning;
    int                i;
    */

    // Start is called before the first frame update
    void Start()
    {
        initRingAnimation();
        centerText.enabled = true;
        centerText.text = "Please put your ring vertical and wait for Calibration";
        currentDate = 0;
        StartCoroutine(requestCalibration());
    }

    
    IEnumerator requestCalibration() {
        yield return new WaitForSeconds(2.0f);
        callLibFunction("TriggerCalibration","");
    }

    float isSmall = 1.04f;
    float currentDate;
    // Update is called once per frame
    void Update()
    {
        if (Application.platform != RuntimePlatform.Android){
            currentDate += Time.deltaTime * 1000;
            if(currentDate>900){
                currentDate = 0;
              //  rotateAnimation(0f,0F,-50f,0.13f);
                if(isSmall==1.04){
                    isSmall = 1.05f;
                }else{
                    isSmall = 1.04f;
                }
               // scaleAnimation(isSmall,0.13f);
            }
        }

        /*
       foreach(Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                  anim.speed = 2.0f;
                  anim.Play("OutboundIn");
            }
        }*/
        if (Input.GetKeyDown("space")){
            rotateAnimation(0f,0f,0f,0.3f);
        }
        if (Input.GetKeyDown("a")){
            float zRotation = float.Parse("-10");
            zRotation = zRotation/2;
            scaleAnimation(1.2f,0.13f);
        }

        if (Input.GetKeyDown("b")){
            float zRotation = float.Parse("-10");
            zRotation = zRotation/2;
            scaleAnimation(0.8f,0.13f);
        }
    }




    void onWebSocketReceiveData(string _data){
        JavaMessage(_data);
        var N = JSON.Parse(_data);
       // callLibFunction("printLogForUnity","get data " + _data);
        /*
        if(N["ringData"] != null){
            var ringData = N["ringData"].Value;  
            if(ringData=="outBoundIn"){
                  anim.speed = 2.0f;
                  anim.Play("OutboundIn");
            }else if(ringData=="InboundOut"){
                  anim.speed = 2.0f;
                  anim.Play("InboundOut");
            }else if(ringData=="Outbound"){
                  anim.speed = 2.0f;
                  anim.Play("Outbound");
            }else if(ringData=="Inbound"){
                  anim.speed = 2.0f;
                  anim.Play("Inbound");
            }
        }*/
        
        if(N["calibratingFlag"]!=null){
            var calibratingFlag = N["calibratingFlag"].Value;  
            centerText.enabled = false;
            if(calibratingFlag.Trim().Equals("true")){
                centerText.enabled = true;
            }
        }
        
        if(N["mpuDataTime"] != null){
            handleMPUData(_data);
        }


        if(N["flexPercentage"] != null){
            handleFlex(_data);
        }
    }

    void handleMPUData(string _jsonData){

        var JsonObj = JSON.Parse(_jsonData); 
        var mpuDataTime = JsonObj["mpuDataTime"].Value;  
        float time = float.Parse(mpuDataTime);


        var yawData = JsonObj["yawData"].Value;  
        float yawAngle = float.Parse(yawData);

        var pitchData = JsonObj["pitchData"].Value;  
        float pitchAngle = float.Parse(pitchData);

        var rollData = JsonObj["rollData"].Value;  
        float rollAngle = float.Parse(rollData);

        /*
        var pitchData = JsonObj["pitchData"].Value;  
        float yPitch = float.Parse(pitchData);
        */
        
        JavaMessage("yaw   "+ yawAngle);   
        JavaMessage("pitch   "+ pitchAngle);   
        JavaMessage("roll   "+ rollAngle);           
        JavaMessage("time   "+ time);   


        rotateAnimation(yawAngle,pitchAngle,rollAngle,time);
        //rotateAnimation(0f,0f,yPitch,time);
    }

    void handleFlex(string _jsonData){
        var JsonObj = JSON.Parse(_jsonData); 
        var flexPercentage = JsonObj["flexPercentage"].Value;  
        float flexPercentageFloat = float.Parse(flexPercentage);
        var flexTime = JsonObj["flexDataTime"].Value;  
        float flexTimeFloat = float.Parse(flexTime);
        scaleAnimation(flexPercentageFloat,flexTimeFloat);
    }
}
