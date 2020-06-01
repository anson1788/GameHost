/*
 * Copyright (c) 2015 Allan Pichardo
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *  http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using UnityEngine;
using System;
using System.Collections;

public class Example : MonoBehaviour
{
    
	void Start ()
	{
		//Select the instance of AudioProcessor and pass a reference
		//to this object
		AudioProcessor processor = FindObjectOfType<AudioProcessor> ();
		processor.onBeat.AddListener (onOnbeatDetected);
		processor.onSpectrum.AddListener (onSpectrum);
		StartMonitor();
	}


	float lastBeat = 999;
	float idx = -1;
	void StartMonitor(){
		 StartCoroutine(WaitAndPrint());
	}

	string ValString = "";
	IEnumerator WaitAndPrint()
    {
		idx = idx + 0.5f ;
		float currentTime = Time.time;
		float diff = currentTime-lastBeat;
		if(diff>0 && diff < 0.09){
			ValString  +=idx+",";
			Debug.Log(ValString);
		}
        yield return new WaitForSeconds(60f/(155f*2f));
		StartMonitor();
    }

	//this event will be called every time a beat is detected.
	//Change the threshold parameter in the inspector
	//to adjust the sensitivity
	void onOnbeatDetected ()
	{
		//Debug.Log ("Beat!!!");
		
		float currentTime = Time.time;
		lastBeat = currentTime;
	}

	//This event will be called every frame while music is playing
	void onSpectrum (float[] spectrum)
	{
		//The spectrum is logarithmically averaged
		//to 12 bands

		for (int i = 0; i < spectrum.Length; ++i) {
			Vector3 start = new Vector3 (i, 0, 0);
			Vector3 end = new Vector3 (i, spectrum [i], 0);
			Debug.DrawLine (start, end);
		}
	}
}
