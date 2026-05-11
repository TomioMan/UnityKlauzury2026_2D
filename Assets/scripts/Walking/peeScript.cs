using UnityEngine;
using UnityEngine.InputSystem;

public class peeScript : MonoBehaviour
{
    public ParticleSystem dispenserParticles;

    void Start()
    {
        // The system needs to be 'Playing' for emission to work
        if (dispenserParticles != null && !dispenserParticles.isPlaying)
        {
            dispenserParticles.Play();
        }
    }

    void Update()
    {
        if (dispenserParticles == null) return;

        var emission = dispenserParticles.emission;

        // Using the New Input System syntax you provided
        if (Keyboard.current.pKey.isPressed)
        {
            emission.enabled = true;
        }
        else
        {
            emission.enabled = false;
        }
    }
}