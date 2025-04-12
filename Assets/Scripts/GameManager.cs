using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        }

        public GameState gameState = GameState.ThirdPerson;

        void Update()
        {
            if (gameState == GameState.ThirdPerson)
            {
                PlayerState.instance.Tick();
            }
            else if (gameState == GameState.InDialogue)
            {
                DialogueManager.instance.Tick();
            }
        }

        void FixedUpdate()
        {
            if (gameState == GameState.ThirdPerson)
            {
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