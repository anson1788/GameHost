using UnityEngine;

public class PlayerInputControl : MonoBehaviour 
{

    public delegate void InputtedAction(int trackNumber);
    public static event InputtedAction inputtedEvent;

    public CircleCollider2D[] tappingSpheres;
	
    //in unity editor & standalone, input by keyboard
    #if UNITY_EDITOR || UNITY_STANDALONE

    private KeyCode[] keybindings;
    private KeyCode pauseKey;

    private KeyCode downArrow;
    private KeyCode leftArrow;
    private KeyCode rightArrow;
    private KeyCode space;


    #endif

    private AudioSource[] audioSources;

    //cache the number of tracks
    private int trackLength;

    //animation
    public SpriteRenderer[] innerCircles;
    private SpriteColorLerp[] innerAnimations;
    private Coroutine[] previousInnerAnimations;
    private const float StartAlphaForInner = 0.7f;


	public SongCollection[] songCollections;
	public SongInfoMessenger songInfoMessengerPrefab;

    public SpriteRenderer[] tappingSprite;
    public TextMesh[] tappingKeyMesh;

    private int CrtHighLightIdx = 0;
    void Start()
    {
        if(songInfoMessengerPrefab!=null){
            SongInfoMessenger.Instance = songInfoMessengerPrefab;  
        }
        SongInfoMessenger.Instance.recordedBeats = new AudioClip[4];
		SongInfoMessenger.Instance.recordedBeats[0] = songCollections[0].songSets[0].easy.defaultBeats[0];
        SongInfoMessenger.Instance.recordedBeats[1] = songCollections[0].songSets[0].easy.defaultBeats[1];
        SongInfoMessenger.Instance.recordedBeats[2] = songCollections[0].songSets[0].easy.defaultBeats[2];
        SongInfoMessenger.Instance.recordedBeats[3] = songCollections[0].songSets[0].easy.defaultBeats[3];
        SongInfoMessenger.Instance.currentSong = songCollections[0].songSets[0].easy;
        trackLength = tappingSpheres.Length;
        Debug.Log("dfdf "+trackLength);
        //init audio sources (cache them), and configure the recorded clips & tap animation
        audioSources = new AudioSource[trackLength];
        previousInnerAnimations = new Coroutine[trackLength];
        innerAnimations = new SpriteColorLerp[trackLength];
        for (int i = 0; i < trackLength; i++)
        {
            audioSources[i] = tappingSpheres[i].GetComponent<AudioSource>();

            audioSources[i].clip = SongInfoMessenger.Instance.recordedBeats[i];

            //init inner circle animation
            Color startColor = new Color(innerCircles[i].color.r, innerCircles[i].color.g, innerCircles[i].color.b, StartAlphaForInner);
            innerAnimations[i] = new SpriteColorLerp(innerCircles[i], startColor, innerCircles[i].color, audioSources[i].clip.length);
            previousInnerAnimations[i] = null;
        }

        //just for debugging
        #if UNITY_EDITOR || UNITY_STANDALONE
        keybindings = new KeyCode[4];
        keybindings[0] = KeyboardInputManager.instance.GetKeyCode(KeyboardInputManager.KeyBindings.Track1);
        keybindings[1] = KeyboardInputManager.instance.GetKeyCode(KeyboardInputManager.KeyBindings.Track2);
        keybindings[2] = KeyboardInputManager.instance.GetKeyCode(KeyboardInputManager.KeyBindings.Track3);
        keybindings[3] = KeyboardInputManager.instance.GetKeyCode(KeyboardInputManager.KeyBindings.Track4);
        pauseKey = KeyboardInputManager.instance.GetKeyCode(KeyboardInputManager.KeyBindings.Pause);
        downArrow = KeyboardInputManager.instance.GetKeyCode(KeyboardInputManager.KeyBindings.DownArrow);
        leftArrow = KeyboardInputManager.instance.GetKeyCode(KeyboardInputManager.KeyBindings.LeftArrow);
        rightArrow = KeyboardInputManager.instance.GetKeyCode(KeyboardInputManager.KeyBindings.RightArrow);
        space = KeyboardInputManager.instance.GetKeyCode(KeyboardInputManager.KeyBindings.space);


        KeyboardInputManager.keyChangedEvent += KeyChanged;
        #endif
        updateHighlightSection(0);

    }

    void updateHighlightSection(int idx){
       
        CrtHighLightIdx = idx;
           for (int i = 0; i < tappingSprite.Length; i++){
                tappingSprite[i].color = Color.grey;
                tappingKeyMesh[i].color = Color.grey;
           }
           tappingSprite[idx].color = Color.white;
           tappingKeyMesh[idx].color = Color.white;
    }

    #if UNITY_EDITOR || UNITY_STANDALONE
    void KeyChanged(KeyboardInputManager.KeyBindings keyBinding, KeyCode keyCode)
    {
        if (keyBinding == KeyboardInputManager.KeyBindings.Pause){
            pauseKey = keyCode;
        }else if(keyBinding == KeyboardInputManager.KeyBindings.DownArrow){
            downArrow = keyCode;
        }else if(keyBinding == KeyboardInputManager.KeyBindings.LeftArrow){
            leftArrow = keyCode;
        }else if(keyBinding == KeyboardInputManager.KeyBindings.RightArrow){
            rightArrow = keyCode;
        }else if(keyBinding == KeyboardInputManager.KeyBindings.space){
            space = keyCode;
        }else{
            keybindings[(int) keyBinding] = keyCode;
        }
    }

    void OnDestroy()
    {
        KeyboardInputManager.keyChangedEvent -= KeyChanged;
    }
    #endif
    

    void Update()
    {
        if (Conductor.paused) return;

        //keyboard input
        #if UNITY_EDITOR || UNITY_STANDALONE

        for (int i = 0; i < trackLength; i++)
        {
            if (Input.GetKeyDown(keybindings[i]))
            {
                Inputted(i);
            }
        }

        if (Input.GetKeyDown(downArrow))
        {
            updateHighlightSection(0);
        }
        if (Input.GetKeyDown(rightArrow))
        {
            updateHighlightSection(2);
        }

        if (Input.GetKeyDown(leftArrow))
        {
            updateHighlightSection(1);
        }

        if (Input.GetKeyDown(space))
        {
              Inputted(CrtHighLightIdx);
        }

        #endif

        //touch input
        #if UNITY_IOS || UNITY_ANDROID || UNITY_EDITOR

        //check touch input
        foreach (Touch touch in Input.touches)
        {
            
            //tap down
            if (touch.phase == TouchPhase.Began)
            {
                //check if on a tapping sphere
                for (int i = 0; i < trackLength; i++)
                {
                    if (tappingSpheres[i].OverlapPoint(Camera.main.ScreenToWorldPoint(touch.position)))
                    {
                        Inputted(i);
                    }
                }
            }
        }
        #endif
    }

    void Inputted(int i)
    {
        //inform Conductor and other interested classes
        if (inputtedEvent != null) inputtedEvent(i);

        //play audio clip
        audioSources[i].Play();

        //start inner circle animation
      
        if (previousInnerAnimations[i] != null)
        {
            StopCoroutine(previousInnerAnimations[i]);
        }
        previousInnerAnimations[i] = StartCoroutine(innerAnimations[i].AnimationCoroutine());
        
    }
}
