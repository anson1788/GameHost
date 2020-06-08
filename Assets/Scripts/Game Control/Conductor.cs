﻿using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Conductor : MonoBehaviour 
{
	public enum Rank {PERFECT, GOOD, BAD, MISS};

	public delegate void BeatOnHitAction(int trackNumber, Rank rank);
	public static event BeatOnHitAction beatOnHitEvent;

	//song completion
	public delegate void SongCompletedAction();
	public static event SongCompletedAction songCompletedEvent;


	private float songLength;

	//if the whole game is paused
	public static bool paused = true;
	private bool songStarted = false;

	public static float pauseTimeStamp = -1f; //negative means not managed
	private float pausedTime = 0f;

	private SongInfo songInfo;

	[Header("Spawn Points")]
	public float[] trackSpawnPosX;

	public float startLineY;
	public float finishLineY;

	public float removeLineY;

	public float badOffsetY;
	public float goodOffsetY;
	public float perfectOffsetY;
	private const float MobileOffsetMultiplier = 1.4f;

	//different colors for each track
	public Color[] trackColors;

	//current song position
	public static float songposition;

	//how many seconds for each beat
	public static float crotchet;

	//index for each tracks
	private int[] trackNextIndices;
	
	//queue, saving the MusicNodes which currently on screen
	private Queue<MusicNode>[] queueForTracks;

	//multi-times notes might be paused on the finish line, but already dequed
	private MusicNode[] previousMusicNodes; 

	//keep a reference of the sound tracks
	private SongInfo.Track[] tracks;

	private float dsptimesong;

	public static float BeatsShownOnScreen = 4f;

	//count down canvas
	private const int StartCountDown = 3;
	public GameObject countDownCanvas;
	public Text countDownText;

	public Text yawValText;

	public float yawVal;

	//layer each music node, so that the first one would be at the front
	private const float LayerOffsetZ = 0.001f;
	private const float FirstLayerZ = -6f;
	private float[] nextLayerZ;

	//total tracks
	private int len;
	private AudioSource audioSource { get { return GetComponent<AudioSource> (); } }

	void PlayerInputted(int trackNumber)
	{
		//check if multi-times node exists
		if (previousMusicNodes[trackNumber] != null)
		{
			//dispatch beat on hit event (multi-times node is always PERFECT)
			if (beatOnHitEvent != null) beatOnHitEvent(trackNumber, Rank.PERFECT);

			//check if the node should be removed
			if (previousMusicNodes[trackNumber].MultiTimesHit())
			{
				//print("Multi-Times Succeed!");
				previousMusicNodes[trackNumber] = null;
			}
		}
		else if (queueForTracks[trackNumber].Count != 0)
		{
			//peek the node in the queue
			MusicNode frontNode = queueForTracks[trackNumber].Peek();

			if (frontNode.times > 0) return; //multi-times node should be handled in the Update() func

			float offsetY = Mathf.Abs(frontNode.gameObject.transform.position.y - finishLineY);
			
			if (offsetY < perfectOffsetY) //perfect hit
			{
				frontNode.PerfectHit();
				//print("Perfect");

				//dispatch beat on hit event
				if (beatOnHitEvent != null) beatOnHitEvent(trackNumber, Rank.PERFECT);

				queueForTracks[trackNumber].Dequeue();
			}
			else if (offsetY < goodOffsetY) //good hit
			{
				frontNode.GoodHit();
				//print("Good");

				//dispatch beat on hit event
				if (beatOnHitEvent != null) beatOnHitEvent(trackNumber, Rank.GOOD);

				queueForTracks[trackNumber].Dequeue();
			}
			else if (offsetY < badOffsetY) //bad hit
			{
				frontNode.BadHit();

				//dispatch beat on hit event
				if (beatOnHitEvent != null) beatOnHitEvent(trackNumber, Rank.BAD);

				queueForTracks[trackNumber].Dequeue();
			}
		}
	}

	void Start()
	{
		//reset static variables
		paused = true;
		pauseTimeStamp = -1f;

		//if in mobile platforms, multiply the offsets
		#if UNITY_IOS || UNITY_ANDROID
		perfectOffsetY *= MobileOffsetMultiplier;
		goodOffsetY *= MobileOffsetMultiplier;
		badOffsetY *= MobileOffsetMultiplier;
		#endif

		//display countdown canvas
		countDownCanvas.SetActive(false);

		//get the song info from messenger
		songInfo = SongInfoMessenger.Instance.currentSong;

		//listen to player input
		PlayerInputControl.inputtedEvent += PlayerInputted;

		//initialize fields
		crotchet = 60f / songInfo.bpm;
		songLength = songInfo.song.length;

		//initialize arrays
		len = 3;
		trackNextIndices = new int[1];
		nextLayerZ = new float[len];
		queueForTracks = new Queue<MusicNode>[len];
		previousMusicNodes = new MusicNode[len];
		trackNextIndices[0] = 0;
		for (int i = 0; i < len; i++)
		{
			queueForTracks[i] = new Queue<MusicNode>();
			previousMusicNodes[i] = null;
			nextLayerZ[i] = FirstLayerZ;
		}

		tracks = songInfo.tracks; //keep a reference of the tracks
		//string songVal = "3.5,4,4.5,5,5.5,7,7.5,8,11,12.5,13.5,15,16,16.5,17,17.5,18.5,19.5,21,22.5,23,24,25.5,26,27.5,28,28.5,30.5,31,31.5,32,33,34.5,36,37.5,39.5,41,41.5,42.5,43,43.5,45,46,46.5,48.5,49.5,51,52.5,53,54.5,55,55.5,57,57.5,58.5,59.5,60.5,61,62.5,64,64.5,66.5,67.5,68,68.5,69,70,70.5,72.5,74,75,75.5,76,78,79,79.5,81,82,83.5,84.5,85,87,88.5,89,89.5,90,90.5,91,91.5,92.5,93,93.5,94,94.5,95.5,96.5,97,97.5,98.5,99,100.5,101.5,102,102.5,103.5,106,106.5,108,108.5,109.5,110,111,112,115,119.5,120,120.5,121,121.5,124,125.5,127,128.5,129,130,131.5,132,132.5,133.5,135,136,136.5,137,137.5,138,139,142.5,144,144.5,145,146.5,147.5,148,150.5,151,152,156.5,158,159.5,160,161,162,163,165.5,166,167,167.5,168.5,170,170.5,171.5,174.5,175,175.5,176,177,177.5,178,178.5,180.5,181,181.5,182,184.5,185.5,186.5,189,189.5,191,191.5,192,192.5,193.5,194,195,196.5,197.5,198,198.5,199,199.5,200,201,202.5,203,203.5,204,204.5,206,206.5,207.5,209,210,211,212,212.5,213,215,215.5,219,219.5,221.5,222,222.5,224,225,227,227.5,228.5,230,231,234.5,236,237,237.5,240,242,242.5,243,245.5,248,250,250.5,251,252.5,253,254,255,255.5";
		//string songVal = "7,8.5,11,12.5,13.5,14.5,16,18,19,21.5,22,23.5,25.5,27,29.5,30.5,31.5,32,34.5,35,51,55,57.5,58,59.5,60,61.5,63,64,65,67,68,70,72.5,75,85,86.5,105,107.5,111.5,112.5,114,115,125,127.5,130,133,135,138,140,145,148,151.5,165,166.5,169.5,174,180,182,184.5,187,192,197,199.5,216.5,221.5,230.5,250.5,251,255.5,257.5,258,259,260.5,261,262,263,264,264.5,267.5,280.5,283.5,294,297.5,304.5,306,309.5,313.5,317,319,319.5,322,322.5,324.5,326,327,327.5,329.5,332.5,335,335.5,337.5,340.5,344,348,350.5,351,352,353,355.5,359.5,361.5,363,371.5,378,380.5,382.5,388,391,392,392.5,393,393.5,395.5,397,400,404.5,406,407,408.5,409,412,413.5,414.5,416.5,417.5,424,428.5,431,436.5,439,440,441,444,444.5,447,447.5,448.5,451,452.5,454,456,457.5,459.5,462,462.5,464,464.5,474.5,480.5,483.5,488,490,491,491.5";
		string songVal = "7,7.5,8.5,9.5,10,11,12.5,13.5,14,14.5,16,16.5,18.5,20,21,21.5,22,24,25,25.5,26,26.5,27.5,28,28.5,29.5,30,30.5,32.5,35,35.5,38,39,40.5,41.5,43,44,45.5,47,49,49.5,50.5,51.5,54.5,55.5,56.5,57.5,58.5,60,60.5,62,62.5,63.5,64,64.5,65,65.5,67.5,68,68.5,72,74.5,75.5,76,77,77.5,78.5,79,79.5,81.5,82,82.5,83,84,84.5,85.5,88,91,91.5,92.5,93,94,94.5,95.5,97,99,100.5,102.5,103,105,106.5,107.5,108.5,110,113.5,115,116,117,118.5,120,122,123.5,124.5,126,126.5,127,127.5,128,130.5,132,133,135.5,136,136.5,137,138.5,139.5,140.5,142,143,144,145,145.5,147,147.5,149,150.5,153.5,154,155.5,156,158.5,159,159.5,161,161.5,163,163.5,164,164.5,165,167,167.5,168.5,169.5,170.5,172,173,175,175.5,176,177,178,179,181,181.5,182,184,187,187.5,189.5,191,192.5,193,193.5,195.5,196.5,198.5,199.5,201,201.5,202,202.5,203.5,204.5,205.5,207,208,209.5,210,210.5,211,211.5,212,213.5,214.5,215,215.5,216,217,218,218.5,219.5,220.5,221.5,223,223.5,224,225,226.5,227.5,228,228.5,229,231.5,233,235,237.5,240,241,242.5,243.5,245.5,246,246.5,248.5,249,249.5,250,250.5,252,253.5,257,257.5,258,259,259.5,260,260.5,261,261.5,262,262.5,263,263.5,264.5,265,266,267,268,269.5,271,271.5,272.5,273.5,274.5,275,276.5,279,279.5,280.5,281.5,282.5,286,287.5,291,292.5,294,294.5,296,297,298.5,299.5,302,303.5,304,305,305.5,306,308,309,309.5,310,310.5,311.5,312,313,314,316.5,317.5,319,321.5,322.5,324,325,326.5,327.5,328.5,329,330,330.5,331,331.5,333.5,335,336,336.5,339,339.5,342,342.5,343,344.5,345.5,347,347.5,350,350.5,351,351.5,353,354,356,358,358.5,359,359.5,360.5,361.5,364.5,365,366,366.5,369.5,371.5,372.5,375,375.5,376,378.5,380,380.5,381,381.5,382,382.5,383,383.5,384,385,387,388,389.5,392,393.5,394,394.5,395.5,396,398,400,400.5,401.5,403,404,404.5,405,405.5,406,407,407.5,408.5,409,409.5,411,411.5,412,412.5,414.5,415,415.5,418,419,420.5,421,422.5,425,426,426.5,428.5,431,434,434.5,435.5,437,437.5,438,439.5,440.5,441,442,443.5,444,444.5,445.5,446,447,447.5,448,449.5,450.5,451,452,453,453.5,454,455.5,456.5,458,462,462.5,463,464.5,465,467.5,471,472.5,473.5,476,479.5,482,484.5,485,485.5,486.5,487.5,488,489.5,493.5,495,496.5,498,499,503,503.5,505,507";
		string[] songArr = songVal.Split(char.Parse(","));
		Debug.Log("url "+songArr.Length);
		songInfo.tracks[0].notes = new SongInfo.Note[songArr.Length];
		int idx = 0;
		foreach (string note in songArr) {
			float x = float.Parse(note);
			SongInfo.Note tmp = new SongInfo.Note();
			tmp.times = 0;
			tmp.note = x;
			songInfo.tracks[0].notes[idx] = tmp;
			idx ++;
		}



		//initialize audioSource
		audioSource.clip = songInfo.song;

	}

	public void makeGameActive(){
		countDownCanvas.SetActive(true);
		StartCoroutine(CountDown());
	}

	void StartSong()
	{
		//get dsptime
		dsptimesong = (float) AudioSettings.dspTime;

		//play song
		audioSource.Play();

		//unpause
		Conductor.paused = false;
		songStarted = true;
	}


	float lastNote = -999;
	void Update()
	{
		//for count down
		if (!songStarted) return;
		
		//for pausing
		if (paused)
		{
			if (pauseTimeStamp < 0f) //not managed
			{
				pauseTimeStamp = (float) AudioSettings.dspTime;
				//print("pausetimestamp:" + pauseTimeStamp.ToString());
				audioSource.Pause();
			}

			return;
		}
		else if (pauseTimeStamp > 0f) //resume not managed
		{
			pausedTime += (float) AudioSettings.dspTime - pauseTimeStamp;
			//print("resumetimestamp:"+AudioSettings.dspTime.ToString());
			//print("offset"+pausedTime.ToString());
			audioSource.Play();

			pauseTimeStamp = -1f;
		}

		//calculate songposition
		songposition = (float) (AudioSettings.dspTime - dsptimesong - pausedTime) * audioSource.pitch - songInfo.songOffset;
		//print (songposition);

		//check if need to instantiate new nodes
		float beatToShow = songposition / crotchet + BeatsShownOnScreen;
		
	
		int nextIndex = trackNextIndices[0];
		SongInfo.Track currTrack = tracks[0];
		int nodeDisplayIdx = Random.Range(0, 3);	
		if (nextIndex < currTrack.notes.Length && currTrack.notes[nextIndex].note < beatToShow)	{
				SongInfo.Note currNote = currTrack.notes[nextIndex];

				if(currNote.note-lastNote>2){
					//set z position
					float layerZ = nextLayerZ[nodeDisplayIdx];
					nextLayerZ[nodeDisplayIdx] += LayerOffsetZ;

					//get a new node
					MusicNode musicNode = MusicNodePool.instance.GetNode(
						trackSpawnPosX[nodeDisplayIdx], 
						startLineY, 
						finishLineY, 
						removeLineY, 
						layerZ, 
						currNote.note,
						currNote.times,
						trackColors[nodeDisplayIdx]);
					lastNote = currNote.note;
					//enqueue
					queueForTracks[nodeDisplayIdx].Enqueue(musicNode);

					//update the next index
				}
				trackNextIndices[0]++;
		}

	

		//loop the queue to check if any of them reaches the finish line
		for (int i = 0; i < len; i++)
		{
			//empty queue, continue
			if (queueForTracks[i].Count == 0) continue;

			MusicNode currNode = queueForTracks[i].Peek();

			//multi-times note
			if (currNode.times > 0 && currNode.transform.position.y <= finishLineY + goodOffsetY)
			{
				//have previous note stuck on the finish line
				if (previousMusicNodes[i] != null)
				{
					previousMusicNodes[i].MultiTimesFailed();

					//dispatch miss event
					if (beatOnHitEvent != null) beatOnHitEvent(i, Rank.MISS);
				}

				//pause the note
				currNode.paused = true;

				//align to finish line
				currNode.transform.position = new Vector3(currNode.transform.position.x, finishLineY, currNode.transform.position.z);

				//deque, but keep a reference
				previousMusicNodes[i] = currNode;
				queueForTracks[i].Dequeue();
			}
			else if (currNode.transform.position.y <= finishLineY - goodOffsetY)   //single time note
			{
				//have previous note stuck on the finish line
				if (previousMusicNodes[i] != null)
				{
					previousMusicNodes[i].MultiTimesFailed();
					previousMusicNodes[i] = null;

					//dispatch miss event
					if (beatOnHitEvent != null) beatOnHitEvent(i, Rank.MISS);
				}

				//deque
				queueForTracks[i].Dequeue();

				//dispatch miss event (if a multi-times note is missed, its next single note would also be missed)
				if (beatOnHitEvent != null) beatOnHitEvent(i, Rank.MISS);
			}
		}


		//check to see if the song reaches its end
		if (songposition > songLength)
		{
			songStarted = false;

			if (songCompletedEvent != null)
				songCompletedEvent();
		}
	}

	IEnumerator CountDown()
	{
		yield return new WaitForSeconds(1f);

		for (int i = StartCountDown; i >= 1; i--)
		{
			countDownText.text = i.ToString();
			yield return new WaitForSeconds(1f);				
		}
		countDownCanvas.SetActive(false);

		StartSong();
	}

	void OnDestroy()
	{
		PlayerInputControl.inputtedEvent -= PlayerInputted;
	}
}
