using UnityEngine;

namespace Payosky.Utilities
{
    public class FrameRateSwitcher : MonoBehaviour
    {
        [SerializeField] private int targetFrameRate = -1;

        private void OnValidate()
        {
            Application.targetFrameRate = targetFrameRate;
        }
    }
}