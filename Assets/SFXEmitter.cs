using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXEmitter : MonoBehaviour
{
    public ParticleSystem ps;
    public void EmitSFX()
    {
        ps.Play();
    }
}
