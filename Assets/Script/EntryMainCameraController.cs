using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;
using UnityEngine.SceneManagement;

public class EntryMainCameraController : CommonController
{

    // Start is called before the first frame update
    public Text centerText;
    void Start()
    {
        startConnectToSocket();
    }

    

    // Update is called once per frame
    void Update()
    {
        
    }

    void startConnectToSocket(){
        updateCenterText("Start Connecting to Socket");
        callLibFunction("connectSocket","wss://fmt0duuywk.execute-api.us-east-1.amazonaws.com/uat");
    }

    void updateCenterText(string _str){
        centerText.text = _str;
    }


    void onWebSocketConnected(string _data){
        updateCenterText("Socket connected , joining the room");
        callLibFunction("socketSendMsg","{\"action\":\"registerType\",\"type\":\"gamehost\"}");
    }
    void onWebSocketReceiveData(string _data){
        JavaMessage(_data);
        var N = JSON.Parse(_data);
        if(N["roomid"] != null){
            var roomid = N["roomid"].Value;  
            updateCenterText("Room ID : "+roomid);
        }else if(N["action"] != null && N["action"].Value=="startGame"){
            goToNextScene();
        }
    }

    void goToNextScene(){
        StartCoroutine(LoadSceneAsync());
    }

    IEnumerator LoadSceneAsync()
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("RingDemoScene");
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
