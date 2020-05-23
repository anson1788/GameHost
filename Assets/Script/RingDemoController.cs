using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using UnityEngine.UI;
using System;

public class RingDemoController : RingAnimationController
{

    public Text centerText;
    public GameObject ringText;

    public GameObject[] mGameObj;
    public GameObject ringHolder;

    public GameObject conductor;

    public GameObject PlayerInputUI;
    public MotionObject mMotionObj = new MotionObject();
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
        for(int i = 0; i < mGameObj.Length; i++){
            mGameObj[i].SetActive(false);
        }

        StartCoroutine(requestCalibration());
       
    }


    void startGame(){
        for(int i = 0; i < mGameObj.Length; i++){
            mGameObj[i].SetActive(true);
        }
        ringText.SetActive(false);
        ringHolder.SetActive(false);
        conductor.GetComponent<Conductor>().makeGameActive();
    }

    IEnumerator requestCalibration() {
        yield return new WaitForSeconds(1.5f);
        callLibFunction("TriggerCalibration","");
    }


    public void triggerAnimationLoop(){
       // JavaMessage("Animation Background loop");
        StartCoroutine(performRegularAnimation());
    }


    IEnumerator performRegularAnimation() {
        if(calibrationReady){
            rotateAnimation(mMotionObj.mYaw,mMotionObj.mPitch,mMotionObj.mRotation,mMotionObj.mTime);
        }
        scaleAnimation(mMotionObj.mScaleVal,mMotionObj.mTime);
        yield return new WaitForSeconds(0.06f);
        triggerAnimationLoop();
    }


    public bool calibrationReady =false;
    void StartCalibration(string _data){
        centerText.enabled = true;
        centerText.text = "Calibrating";
        calibrationReady = false;
    }


    void StopCalibration(string _data){
        centerText.enabled = false;
        centerText.text = "Gaming";
        calibrationReady = true;
        startGame();
    }
    

    float isSmall = 1.04f;
    float currentDate;
    bool isAnimationLoopStart = false ;
    // Update is called once per frame
    void Update()
    {
        if(isAnimationLoopStart==false){
            isAnimationLoopStart = true;
            triggerAnimationLoop();
        }
       // scaleAnimation(mMotionObj.mScaleVal,mMotionObj.mTime);
        if (Application.platform != RuntimePlatform.Android){
            currentDate += Time.deltaTime * 1000;
            if(currentDate>900){
                currentDate = 0;
                if(isSmall==1.04){
                    isSmall = 1.05f;
                }else{
                    isSmall = 1.04f;
                }
            }
        }

        if (Input.GetKeyDown("space")){
            //rotateAnimation(0f,0f,20f,0.3f);
            StopCalibration("");
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
        if(N["calibratingFlag"]!=null){
            var calibratingFlag = N["calibratingFlag"].Value;  
            centerText.enabled = false;
            if(calibratingFlag.Trim().Equals("true")){
                centerText.enabled = true;
            }
        }
        
        if(N["yawData"] != null){
            handleMPUData(_data);
        }


        if(N["flexPercentage"] != null){
            handleFlex(_data);
        }
    }

    void handleMPUData(string _jsonData){

        var JsonObj = JSON.Parse(_jsonData); 
        /*
        var mpuDataTime = JsonObj["mpuDataTime"].Value;  
        float time = float.Parse(mpuDataTime);
        */

        var yawData = JsonObj["yawData"].Value;  
        float yawAngle = float.Parse(yawData);

        var pitchData = JsonObj["pitchData"].Value;  
        float pitchAngle = float.Parse(pitchData);

        var rollData = JsonObj["rollData"].Value;  
        float rollAngle = float.Parse(rollData);
        
        JavaMessage("yaw   "+ yawAngle);   
        JavaMessage("pitch   "+ pitchAngle);   
        JavaMessage("roll   "+ rollAngle);           
        
        mMotionObj.mYaw = yawAngle;
        mMotionObj.mPitch = pitchAngle;
        mMotionObj.mRotation = rollAngle;

        PlayerInputUI.GetComponent<PlayerInputControl>().updateYawVal(yawAngle);
       
    }

    void handleFlex(string _jsonData){
        var JsonObj = JSON.Parse(_jsonData); 
        var flexPercentage = JsonObj["flexPercentage"].Value;  
        float flexPercentageFloat = float.Parse(flexPercentage);
        /*
        var flexTime = JsonObj["flexDataTime"].Value;  
        float flexTimeFloat = float.Parse(flexTime);
        */
        mMotionObj.mScaleVal = flexPercentageFloat;
        if(flexPercentageFloat<0.75){
         PlayerInputUI.GetComponent<PlayerInputControl>().spacePause();  
        }
        //scaleAnimation(flexPercentageFloat,flexTimeFloat);
    }


}
