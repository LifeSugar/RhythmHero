using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using rhythmhero;
using rhythmhero.audio;
using UnityEngine;

public class hitground : MonoBehaviour
{
    public EventReference hit;
    private bool played = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<InteractionCoconut>() && !played)
        {
            AudioManager.instance.PlayOneShot(hit, this.transform.position);
            played = true;
        }
    }
}
