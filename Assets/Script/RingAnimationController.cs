using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System;

public class RingAnimationController : CommonController
{

    public GameObject ringHold;
    public GameObject bone;
    public GameObject lBox;
    public GameObject RBox;

    public string rotationKey = "rotate";
    public string scaleKey = "scalebond";
    public string lpositionKey = "lposition";
    public string rpositionKey = "rposition";

    AnimationClip rotation;
    AnimationClip scale;
    AnimationClip lposition;
    AnimationClip rposition;

    void Start() {
     
    }

    // Update is called once per frame
    void Update()
    {
    
    }

    public Animation getRingFrameAnimation(){
        return ringHold.GetComponent<Animation>();
    }


    public Animation getBoneFrameAnimation(){
        return bone.GetComponent<Animation>();
    }

    public Animation getLpositionFrameAnimation(){
        return lBox.GetComponent<Animation>();
    }

    public Animation getRpositionFrameAnimation(){
        return RBox.GetComponent<Animation>();
    }

    public void initRingAnimation(){
        if(rotation==null){
            rotation = new AnimationClip();
            rotation.legacy=true;
        }
        rotation.ClearCurves();
        float animationSpeed = 0.3f;
        AnimationCurve    curve_x,curve_y,curve_z,curve_w;
        curve_x = AnimationCurve.Linear(0, 0, animationSpeed, 0);
        curve_y = AnimationCurve.Linear(0, 0, animationSpeed, 0);
        curve_z = AnimationCurve.Linear(0, 0, animationSpeed, 0);
        curve_w = AnimationCurve.Linear(0, 0, animationSpeed, 0);
        rotation.SetCurve("", typeof(Transform), "localRotation.x", curve_x);
        rotation.SetCurve("", typeof(Transform), "localRotation.y", curve_y);
        rotation.SetCurve("", typeof(Transform), "localRotation.z", curve_z);
        rotation.SetCurve("", typeof(Transform), "localRotation.w", curve_w);

        getRingFrameAnimation().AddClip(rotation, rotationKey);
    
        if(scale==null){
            scale = new AnimationClip();
            scale.legacy=true;
        }    
        scale.ClearCurves();
        AnimationCurve    curveScale;
        curveScale = AnimationCurve.Linear(0, 0, animationSpeed, 0);
        scale.SetCurve("", typeof(Transform), "localScale.x", curveScale);
        getBoneFrameAnimation().AddClip(scale, scaleKey);


        if(lposition==null){
            lposition = new AnimationClip();
            lposition.legacy=true;
        }    
        lposition.ClearCurves();
        AnimationCurve  curveLPosition;
        curveLPosition = AnimationCurve.Linear(0, 0, animationSpeed, 0);
        lposition.SetCurve("", typeof(Transform), "localPosition.x", curveLPosition);
        getLpositionFrameAnimation().AddClip(lposition, lpositionKey);


        if(rposition==null){
            rposition = new AnimationClip();
            rposition.legacy=true;
        }    
        rposition.ClearCurves();
        AnimationCurve  curveRPosition;
        curveRPosition = AnimationCurve.Linear(0, 0, animationSpeed, 0);
        rposition.SetCurve("", typeof(Transform), "localPosition.x", curveRPosition);
        getRpositionFrameAnimation().AddClip(rposition, rpositionKey);
    }
    
    


    public void rotateAnimation(float yaw, float pitch,float roll, float _speed){
        getRingFrameAnimation().Stop(rotationKey);
        rotation.ClearCurves();
        /*     
        float targetYaw =  (yaw-ringHold.transform.rotation.y);
        callLibFunction("printLogForUnity","target Yaw " + targetYaw + " | " + yaw + " | " + ringHold.transform.rotation.y);
        ringHold.transform.Rotate(0, targetYaw, pitch, Space.World);
        Quaternion target = ringHold.transform.rotation;
        ringHold.transform.rotation = crtTransform;
        */
        Quaternion crtTransform = ringHold.transform.rotation; 
        
        Quaternion target =  Quaternion.Euler(roll, yaw, -pitch);
        float animationSpeed = _speed;
        AnimationCurve    curve_x,curve_y,curve_z,curve_w;
        curve_x = AnimationCurve.Linear(0, crtTransform.x, animationSpeed, target.x);
        curve_y = AnimationCurve.Linear(0, crtTransform.y, animationSpeed, target.y);
        curve_z = AnimationCurve.Linear(0, crtTransform.z, animationSpeed, target.z);
        curve_w = AnimationCurve.Linear(0, crtTransform.w, animationSpeed, target.w);
        rotation.legacy=true;
        rotation.SetCurve("", typeof(Transform), "localRotation.x", curve_x);
        rotation.SetCurve("", typeof(Transform), "localRotation.y", curve_y);
        rotation.SetCurve("", typeof(Transform), "localRotation.z", curve_z);
        rotation.SetCurve("", typeof(Transform), "localRotation.w", curve_w);
        getRingFrameAnimation().RemoveClip(rotationKey);
        getRingFrameAnimation().AddClip(rotation, rotationKey);
        getRingFrameAnimation().Play(rotationKey);
        
    }

    public void scaleAnimation( float position,float time){
        /*
        Vector3 ringScale = bone.transform.localScale; 
        float scaleYCorrt = (position-1f)*0.15f/0.3f;
        bone.transform.localScale = new Vector3(1f,position,1-scaleYCorrt);
        
        Vector3 Lpos = lBox.transform.localPosition; 
        lBox.transform.localPosition = new Vector3(position,Lpos.y,Lpos.z);
        Vector3 Rpos = RBox.transform.localPosition; 
        RBox.transform.localPosition = new Vector3(-position,Rpos.y,Rpos.z);
        */
        
        Vector3 ringScale = bone.transform.localScale; 
        float scaleYCorrt = (position-1f)*0.15f/0.3f;
        getRingFrameAnimation().Stop(scaleKey);
        scale.ClearCurves();
        AnimationCurve    curveScaleX,curveScaleY,curveScaleZ;
        curveScaleX = AnimationCurve.EaseInOut(0, 1, time, 1);
        curveScaleY = AnimationCurve.EaseInOut(0, ringScale.y, time, position);
        curveScaleZ = AnimationCurve.EaseInOut(0, ringScale.z, time, 1-scaleYCorrt);
        scale.SetCurve("", typeof(Transform), "localScale.x", curveScaleX);
        scale.SetCurve("", typeof(Transform), "localScale.y", curveScaleY);
        scale.SetCurve("", typeof(Transform), "localScale.z", curveScaleZ);
        getBoneFrameAnimation().RemoveClip(scaleKey);
        getBoneFrameAnimation().AddClip(scale, scaleKey);
        getBoneFrameAnimation().Play(scaleKey);
   

 
        Vector3 Lpos = lBox.transform.localPosition; 
        getLpositionFrameAnimation().Stop(lpositionKey);
        lposition.ClearCurves();
        AnimationCurve    curveLPosX;
        curveLPosX = AnimationCurve.EaseInOut(0, Lpos.x, time, position);
        lposition.SetCurve("", typeof(Transform), "localPosition.x", curveLPosX);
        getLpositionFrameAnimation().RemoveClip(lpositionKey);
        getLpositionFrameAnimation().AddClip(lposition, lpositionKey);
        getLpositionFrameAnimation().Play(lpositionKey);

        Vector3 Rpos = RBox.transform.localPosition; 
        getRpositionFrameAnimation().Stop(rpositionKey);
        rposition.ClearCurves();
        AnimationCurve    curveRPosX;
        curveRPosX = AnimationCurve.EaseInOut(0, Rpos.x, time, -position);
        rposition.SetCurve("", typeof(Transform), "localPosition.x", curveRPosX);
        getRpositionFrameAnimation().RemoveClip(rpositionKey);
        getRpositionFrameAnimation().AddClip(rposition, rpositionKey);
        getRpositionFrameAnimation().Play(rpositionKey);
                
    }
}
