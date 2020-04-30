using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
public class RingDemoController : RingAnimationController
{

    public Animator anim;


    /*
    Quaternion        resulting;
    Quaternion        beginning;
    int                i;
    */

    // Start is called before the first frame update
    void Start()
    {
        initRingAnimation();
    }

    // Update is called once per frame
    void Update()
    {
    
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
            float zRotation = float.Parse("-10");
            zRotation = zRotation/2;
            rotateAnimation(35.5f,0f,0f,0.13f);
           // rotateAnimation(zRotation,0f,0f,0.3f);
        }
        if (Input.GetKeyDown("a")){
            float zRotation = float.Parse("-10");
            zRotation = zRotation/2;
            scaleAnimation();
          
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

        
        if(N["rollDataTime"] != null){
            /*
            var yawData = N["yawData"].Value;  
            JavaMessage("yawData : " + yawData);
            float zRotation = float.Parse(yawData);
            zRotation = zRotation/2;
            ring.GetComponent<Animation>().Stop("rotate");
            beginning = ring.transform.rotation;        
            ring.transform.Rotate(0, zRotation, 0, Space.World);
            resulting = ring.transform.rotation;
            ring.transform.rotation = beginning;
            Animate(beginning, resulting);
            */
            handleYawData(_data);
        }
    }

    void handleYawData(string _jsonData){

        var JsonObj = JSON.Parse(_jsonData); 
        var rollDataTime = JsonObj["rollDataTime"].Value;  
        float time = float.Parse(rollDataTime);


        var yawData = JsonObj["yawData"].Value;  
        float yaw = float.Parse(yawData);

        var pitchData = JsonObj["pitchData"].Value;  
        float yPitch = float.Parse(pitchData);

        /*
        var pitchData = JsonObj["pitchData"].Value;  
        float yPitch = float.Parse(pitchData);
        */
        
        JavaMessage("yaw   "+ yaw);   
        JavaMessage("time   "+ time);   


        rotateAnimation(yaw,yPitch,0f,time);
        //rotateAnimation(0f,0f,yPitch,time);
    }
}
