using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
public class RingDemoController : CommonController
{

    public Animator anim;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Hello: " + mesh.mesh.bounds.size.x);
       foreach(Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                  anim.speed = 2.0f;
                  anim.Play("OutboundIn");
            }
        }
    }

    void onWebSocketReceiveData(string _data){
        JavaMessage(_data);
        var N = JSON.Parse(_data);
        callLibFunction("printLogForUnity","get data " + _data);
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
        }
    }
}
