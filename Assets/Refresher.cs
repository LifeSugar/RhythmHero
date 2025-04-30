using System.Collections;
using System.Collections.Generic;
using rhythmhero;
using rhythmhero.audio;
using UnityEngine;

public class Refresher : MonoBehaviour
{
    void Start()
    {
        if (GameManager.instance != null)
        {
            Destroy(GameManager.instance.gameObject);
        }
        if (AudioManager.instance != null)
        {
            Destroy(AudioManager.instance.gameObject);
            
        }

        if (BGMManager.instance != null)
        {
            Destroy(BGMManager.instance.gameObject);
        }

        if (DialogueManager.instance != null)
        {
            Destroy(DialogueManager.instance.gameObject);
        }
    }
}
