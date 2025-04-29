using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using rhythmhero.audio;
using UnityEngine;

public class coconutcollider : MonoBehaviour
{
    public GameObject stone;
    public EventReference hit;
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<hitcollider>())
        {
            AudioManager.instance.PlayOneShot(hit, this.transform.position);
            stone.GetComponent<Rigidbody>().isKinematic = false;
            this.GetComponent<Collider>().enabled = false;
        }
    }
}
