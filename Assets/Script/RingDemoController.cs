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
            rotateAnimation(zRotation,0f,0f,0.3f);
        }
    }


    void onWebSocketReceiveData(string _data){
        JavaMessage(_data);
        var N = JSON.Parse(_data);
        callLibFunction("printLogForUnity","get data " + _data);
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
        var yawData = JsonObj["yawData"].Value;  
        var rollDataTime = JsonObj["rollDataTime"].Value;  
        float zRotation = float.Parse(yawData);
        float time = float.Parse(rollDataTime);
        
        rotateAnimation(zRotation,0f,0f,0.05f);
    }
}
