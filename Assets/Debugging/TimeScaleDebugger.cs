using UnityEngine;

namespace SpriteShadows.Debugging
{
    public class TimeScaleDebugger : MonoBehaviour
    {
        [Range(0, 10f)] public float mTimeScale = 1;

        private void OnValidate()
        {
            Time.timeScale = mTimeScale;
        }
    }
}