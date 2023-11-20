using System;
using System.Collections.Generic;
using UnityEngine;

public class XPCollector : MonoBehaviour
{
    private ParticleSystem particleSystem;
    private List<ParticleSystem.Particle> particles = new();

    public static Action<int> OnXPCollected;
    
    private void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();

        SphereCollider collector = GameObject.FindGameObjectWithTag("XP Collector").GetComponent<SphereCollider>();
        particleSystem.trigger.AddCollider(collector);
    }

    private void OnParticleTrigger()
    {
        int triggeredParticles = particleSystem.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, particles);

        for (int i = 0; i < triggeredParticles; i++)
        {
            ParticleSystem.Particle particle = particles[i];
            particle.remainingLifetime = 0;
            
            OnXPCollected?.Invoke(1);

            particles[i] = particle;
        }
        
        particleSystem.SetTriggerParticles(ParticleSystemTriggerEventType.Enter, particles);
    }
}
