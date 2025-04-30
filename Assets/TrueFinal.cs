using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace rhythmhero
{
    public class TrueFinal : MonoBehaviour
    {
        // DialogueData dialogueData;

        public GameObject bg;

        public GameObject Quit;

        void Start()
        {
            // dialogueData.OnDialogueLineChanged += TruetrueEnd;
        }

        public void click()
        {
            bg.SetActive(true);
            FMODManager.singleton.StopMusic();
            // DialogueManager.instance.currentData = dialogueData;
            // DialogueManager.instance.StartDialogue();
        }

        void TruetrueEnd(int currentline)
        {
            // if (currentline == dialogueData.lines.Count)
            // {
            //     Quit.SetActive(true);
            // }
            
        }
    }

}