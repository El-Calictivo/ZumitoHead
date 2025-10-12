using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Payosky.CoreMechanics.Juice
{
    public class RetroSteppable : MonoBehaviour
    {
        [field: SerializeField] public Sprite Particle { get; private set; }
    }
}