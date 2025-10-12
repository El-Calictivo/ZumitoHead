using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Payosky.CoreMechanics.Juice
{
    public class RetroStepResolver : MonoBehaviour
    {
        [SerializeField] private LayerMask layerMask;

        [SerializeField] private AudioSource audioSource;

        [SerializeField] private ParticleSystem particleSystem;

        [SerializeField] private AudioClip leftStep;

        [SerializeField] private AudioClip rightStep;

        public enum StepType
        {
            Left,

            Right
        }

        public void Step(StepType step)
        {
            var objectBelow = Physics2D.Raycast(transform.position, Vector3.down, 1f, layerMask);
            if (objectBelow && objectBelow.transform.TryGetComponent(out RetroSteppable retroSteppable))
            {
                audioSource.pitch = Random.Range(0.9f, 1.1f);
                audioSource.PlayOneShot(step == StepType.Left ? leftStep : rightStep);
                particleSystem.textureSheetAnimation.SetSprite(0, retroSteppable.Particle);
                particleSystem.Play();
            }
        }
    }
}