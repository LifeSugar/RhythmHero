using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace rhythmhero
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;

        void Awake()
        {
            instance = this;
        }

        public GameState gameState = GameState.ThirdPerson;

        void Update()
        {
            if (gameState == GameState.ThirdPerson)
            {
                PlayerState.instance.Tick();
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