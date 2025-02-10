using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using FMOD.Studio;
using UnityEngine.Serialization;

namespace rhythmhero.audio
{
    public class FmodEvnets : MonoBehaviour
    {
        public static FmodEvnets instance { get; private set; }
        
        void Awake()
        {
            if (instance != null)
            {
                Debug.LogError("More than one instance of FmodEventManager found!");
            }
            instance = this;
        }
        

    }
}
