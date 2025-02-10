using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using rhythmhero.audio;

public class AnimationScript : MonoBehaviour
{
    public EventReference stepSound;

    public void PlayStepSound()
    {
        AudioManager.instance.PlayOneShot(stepSound, this.transform.parent.transform.position);
    }
}
