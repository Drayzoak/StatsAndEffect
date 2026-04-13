
using UnityEngine;
namespace Common.TickSystem
{
    public class TickDebugger : MonoBehaviour
    {
        public bool show = true;

        private float deltaTime;

        void Update()
        {
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        }

        void OnGUI()
        {
            if (!show) return;

            float fps = 1.0f / deltaTime;

            GUILayout.BeginArea(new Rect(10, 10, 300, 200), GUI.skin.box);

            GUILayout.Label("<b>TICK SYSTEM DEBUG</b>");

            GUILayout.Label($"FPS: {fps:F1}");
            GUILayout.Space(5);

            GUILayout.Label($"Running: {TickManager.Debug_RunningCount}");
            GUILayout.Label($"Paused: {TickManager.Debug_PausedCount}");
            GUILayout.Label($"Total: {TickManager.Debug_RunningCount + TickManager.Debug_PausedCount}");

            GUILayout.Space(5);

            GUILayout.Label($"Tick Calls/frame: {TickManager.Debug_TickCalls}");
            GUILayout.Label($"Finished/frame: {TickManager.Debug_FinishedThisFrame}");
            GUILayout.Label($"Avg Delta: {TickManager.Debug_AvgDelta:F2}");

            GUILayout.EndArea();
        }
    }
}