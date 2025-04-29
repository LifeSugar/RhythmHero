using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class coconutcollider : MonoBehaviour
{
    public GameObject stone;
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<hitcollider>())
        {
            stone.GetComponent<Rigidbody>().isKinematic = false;
            this.GetComponent<Collider>().enabled = false;
        }
    }
}
