using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
public class RingAnimationController : CommonController
{

    public GameObject ringHold;
    public GameObject bone;
    public string rotationKey = "rotate";
    public string scaleKey = "scalebond";

    AnimationClip rotation;
    AnimationClip scale;

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
    public void initRingAnimation(){
        if(rotation==null){
            rotation = new AnimationClip();
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
        rotation.legacy=true;
        getRingFrameAnimation().AddClip(rotation, rotationKey);
    
        if(scale==null){
            scale = new AnimationClip();
        }    
        scale.ClearCurves();
        AnimationCurve    curveScale;
        curveScale = AnimationCurve.Linear(0, 0, animationSpeed, 0);
        scale.SetCurve("", typeof(Transform), "localScale.x", curveScale);
        scale.legacy=true;
        //getBoneFrameAnimation().AddClip(scale, scaleKey);
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
        Quaternion target =  Quaternion.Euler(0, yaw, pitch);
        float animationSpeed = _speed;
        AnimationCurve    curve_x,curve_y,curve_z,curve_w;
        curve_x = AnimationCurve.Linear(0, crtTransform.x, animationSpeed, target.x);
        curve_y = AnimationCurve.Linear(0, crtTransform.y, animationSpeed, target.y);
        curve_z = AnimationCurve.Linear(0, crtTransform.z, animationSpeed, target.z);
        curve_w = AnimationCurve.Linear(0, crtTransform.w, animationSpeed, target.w);
        rotation.SetCurve("", typeof(Transform), "localRotation.x", curve_x);
        rotation.SetCurve("", typeof(Transform), "localRotation.y", curve_y);
        rotation.SetCurve("", typeof(Transform), "localRotation.z", curve_z);
        rotation.SetCurve("", typeof(Transform), "localRotation.w", curve_w);
        rotation.legacy=true;
        getRingFrameAnimation().RemoveClip(rotationKey);
        getRingFrameAnimation().AddClip(rotation, rotationKey);
        getRingFrameAnimation().Play(rotationKey);
        
    }

    public void scaleAnimation(){
        getRingFrameAnimation().Stop(scaleKey);
        scale.ClearCurves();
        float animationSpeed = 0.3f;
        AnimationCurve    curveScale;
        curveScale = AnimationCurve.Linear(0, 23.0f, animationSpeed, 10f);
        scale.SetCurve("", typeof(Transform), "localScale.x", curveScale);
        scale.legacy=true;
        getBoneFrameAnimation().RemoveClip(scaleKey);
        getBoneFrameAnimation().AddClip(scale, scaleKey);
        getBoneFrameAnimation().Play(scaleKey);
    }
}
