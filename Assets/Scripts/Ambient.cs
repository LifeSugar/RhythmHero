using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using rhythmhero.audio;
using UnityEngine;

namespace rhythmhero
{
    public class Ambient : MonoBehaviour
    {
        public EventReference ambientEvent;
        public EventInstance AmbientEventInstance;

        void Start()
        {
            AmbientEventInstance = AudioManager.instance.CreatEventInstance(ambientEvent);
            AmbientEventInstance.start();
        }
    }

}