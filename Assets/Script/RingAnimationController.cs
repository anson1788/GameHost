using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
public class RingAnimationController : CommonController
{

    public GameObject ringHold;
    public string rotationKey = "rotate";

    AnimationClip rotation;
    void Start() {
     
    }

    // Update is called once per frame
    void Update()
    {
    
    }

    public Animation getRingFrameAnimation(){
        return ringHold.GetComponent<Animation>();
    }
    public void initRingAnimation(){
        if(rotation==null){
            rotation = new AnimationClip();
        }
        rotation.ClearCurves();
        float animationSpeed = 0.3f;
        AnimationCurve    curve_x,curve_y,curve_z,curve_w;
        curve_x = AnimationCurve.EaseInOut(0, 0, animationSpeed, 0);
        curve_y = AnimationCurve.EaseInOut(0, 0, animationSpeed, 0);
        curve_z = AnimationCurve.EaseInOut(0, 0, animationSpeed, 0);
        curve_w = AnimationCurve.EaseInOut(0, 0, animationSpeed, 0);
        rotation.SetCurve("", typeof(Transform), "localRotation.x", curve_x);
        rotation.SetCurve("", typeof(Transform), "localRotation.y", curve_y);
        rotation.SetCurve("", typeof(Transform), "localRotation.z", curve_z);
        rotation.SetCurve("", typeof(Transform), "localRotation.w", curve_w);
        rotation.legacy=true;
        getRingFrameAnimation().AddClip(rotation, rotationKey);
    }
    
    public void rotateAnimation(float yaw, float roll, float pitch, float _speed){

        Quaternion crtTransform = ringHold.transform.rotation;      
        ringHold.transform.Rotate(0, yaw, 0, Space.World);
        Quaternion target = ringHold.transform.rotation;
        ringHold.transform.rotation = crtTransform;
        getRingFrameAnimation().GetComponent<Animation>().Stop(rotationKey);
        rotation.ClearCurves();
        float animationSpeed = _speed;
        AnimationCurve    curve_x,curve_y,curve_z,curve_w;
        curve_x = AnimationCurve.EaseInOut(0, crtTransform.x, animationSpeed, target.x);
        curve_y = AnimationCurve.EaseInOut(0, crtTransform.y, animationSpeed, target.y);
        curve_z = AnimationCurve.EaseInOut(0, crtTransform.z, animationSpeed, target.z);
        curve_w = AnimationCurve.EaseInOut(0, crtTransform.w, animationSpeed, target.w);
        rotation.SetCurve("", typeof(Transform), "localRotation.x", curve_x);
        rotation.SetCurve("", typeof(Transform), "localRotation.y", curve_y);
        rotation.SetCurve("", typeof(Transform), "localRotation.z", curve_z);
        rotation.SetCurve("", typeof(Transform), "localRotation.w", curve_w);
        rotation.legacy=true;
        getRingFrameAnimation().RemoveClip(rotationKey);
        getRingFrameAnimation().AddClip(rotation, rotationKey);
        getRingFrameAnimation().Play(rotationKey);
        
    }
}
