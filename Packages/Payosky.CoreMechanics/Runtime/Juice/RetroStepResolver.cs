using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Payosky.CoreMechanics.Juice
{
    public class RetroStepResolver : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;

        [SerializeField] private AudioClip leftStep;

        [SerializeField] private AudioClip rightStep;

        public enum StepType
        {
            Left,

            Right
        }

        public void Step(StepType step)
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(step == StepType.Left ? leftStep : rightStep);
        }
    }
}