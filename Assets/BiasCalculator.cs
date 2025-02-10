using System.Collections;
using System.Collections.Generic;
using rhythmhero.audio;
using UnityEngine;

namespace rhythmhero
{
    public class BiasCalculator : MonoBehaviour
    {
        public float oneBeatBias;
        public float fourBeatBias;
        
        public static BiasCalculator instance {get; private set;}

        void Awake()
        {
            instance = this;
        }

        void Start()
        {
            BGMManager.instance.OneBeat += CalculateOneBeatBias;
            BGMManager.instance.FourBeat += CalculateFourBeatBias;
        }

        void Update()
        {
            oneBeatBias += Time.deltaTime;
            fourBeatBias += Time.deltaTime;
        }

        void CalculateOneBeatBias()
        {
            oneBeatBias = 0f;
        }

        void CalculateFourBeatBias()
        {
            fourBeatBias = 0f;
        }
    }

}