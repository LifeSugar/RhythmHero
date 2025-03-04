using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            // foreach (var (bgm, Targetposition) in BGMManager.instance.bgms.Zip(BGMManager.instance.bgmTransforms, (bgm,Targetposition) => (bgm,Targetposition)) )
            // {
            //     EventInstance eventInstance = CreatEventInstance(bgm);
            //     Debug.Log(Targetposition.position);
            //     eventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(Targetposition.position));
            //     BGMManager.instance.bgmInstances.Add(eventInstance);
            //     eventInstance.start();
            // }
            
            foreach (var (bgm, Targetposition) in BGMManager.instance.bgms.Zip(BGMManager.instance.bgmTransforms, (bgm, Targetposition) => (bgm, Targetposition)))
            {
                if (Targetposition == null)
                {
                    Debug.LogError("Targetposition is null!");
                    continue;
                }

                Vector3 pos = Targetposition.position;
                Debug.Log($"Targetposition: {pos}");

                if (float.IsNaN(pos.x) || float.IsInfinity(pos.x) ||
                    float.IsNaN(pos.y) || float.IsInfinity(pos.y) ||
                    float.IsNaN(pos.z) || float.IsInfinity(pos.z))
                {
                    Debug.LogError($"Invalid position detected: {pos}");
                    continue;
                }

                EventInstance eventInstance = CreatEventInstance(bgm);
                if (!eventInstance.isValid())
                {
                    Debug.LogError("FMOD EventInstance is not valid!");
                    continue;
                }

                var fmodAttributes = RuntimeUtils.To3DAttributes(pos);
                Debug.Log($"FMOD Attributes: {fmodAttributes.position.x}, {fmodAttributes.position.y}, {fmodAttributes.position.z}");

                eventInstance.set3DAttributes(fmodAttributes);
                BGMManager.instance.bgmInstances.Add(eventInstance);
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