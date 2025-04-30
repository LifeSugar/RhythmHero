using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace rhythmhero
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;
        
        [Header("这里是所有的交互点")]
        public List<GameObject> interactionPoints;
        
        [Header("当前的交互点")]
        public GameObject currentInteractionPoint; //当前的交互点

        void Awake()
        {
            if (instance != null && instance != this)
            {
                Debug.LogError("there is more than one GameManager in scene!");
            }
            instance = this;
            
            DontDestroyOnLoad(this.gameObject);

            SceneManager.sceneLoaded += OnsceneLoaded;
        }
        
        
        private void OnsceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            Debug.Log("Scene Loaded: " + arg0.name);

            if (arg0.name == "Sing_1")
            {
                gameState = GameState.TopDown;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        public GameState gameState = GameState.ThirdPerson;

        void Start()
        {
            FogRenderFeature.instance.SetupFogIntensity(30);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                SceneManager.LoadScene("Sing_1");
            }
            if (gameState == GameState.ThirdPerson)
            {
                if (PlayerState.instance != null)
                {
                    PlayerState.instance.Tick();
                }
                
            }
            else if (gameState == GameState.InDialogue)
            {
                if (PlayerState.instance != null)
                {
                    PlayerState.instance.inputDirection = Vector2.zero;
                    PlayerState.instance.StopRunning();
                }
                DialogueManager.instance.Tick();
            }
        }

        void FixedUpdate()
        {
            if (gameState == GameState.ThirdPerson)
            {
                if (PlayerState.instance != null)
                    PlayerState.instance.FixedTick();
            }
        }

    }

    

    public enum GameState
    {
        Paused = 0,
        ThirdPerson = 1,
        InDialogue = 2,
        TopDown = 3
    }

}