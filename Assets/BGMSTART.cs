using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.SceneManagement;


public class BGMSTART : MonoBehaviour
{
    public EventReference start;


    public EventInstance Instance;

    void Start()
    {
        Instance = RuntimeManager.CreateInstance(start);
        
        Instance.start();
    }

    public void StopBGM()
    {
        Instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        Instance.release();
        
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        Instance.release();
    }
}
