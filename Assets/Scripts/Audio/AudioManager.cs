using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

namespace rhythmhero.audio
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager instance { get; private set; }
        
        private List<EventInstance> eventInstances = new List<EventInstance>();
        private List<StudioEventEmitter> eventEmitters = new List<StudioEventEmitter>();

        public int instanceCout;

        public void Awake()
        {
            if (instance != null)
            {
                Debug.LogWarning("More than one AudioManager in scene!");
            }
            instance = this;
        }
        
        public void PlayOneShot(EventReference eventRef, Vector3 worldPosition)
        {
            RuntimeManager.PlayOneShot(eventRef, worldPosition);
        }
        
        public StudioEventEmitter InitializeEventEmitters(EventReference sound, GameObject emitterGameObject)
        {
            StudioEventEmitter emitter = emitterGameObject.GetComponent<StudioEventEmitter>();
            emitter.EventReference = sound;
            eventEmitters.Add(emitter);
            return emitter;
        }

        public void InitializeBGMInstance()
        {
            foreach (var bgm in BGMManager.instance.bgms)
            {
                EventInstance eventInstance = CreatEventInstance(bgm);
                BGMManager.instance.bgmInstances.Add(eventInstance);
                eventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(Vector3.zero));
                eventInstance.start();
            }
        }
        
        public EventInstance CreatEventInstance(EventReference eventRef)
        {
            EventInstance eventInstance = RuntimeManager.CreateInstance(eventRef);
            eventInstances.Add(eventInstance);
            instanceCout++;
            return eventInstance;
        }
        
    }
}