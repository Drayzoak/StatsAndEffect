using Common.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;
namespace Common.TickSystem
{
    internal static class TickBootstrapper
    {
        static PlayerLoopSystem tickSystem;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        internal static void Initialize()
        {
            PlayerLoopSystem currentPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();

            tickSystem = new PlayerLoopSystem()
            {
                type = typeof(TickManager),
                updateDelegate = TickManager.Update,
                subSystemList = null
            };
            
            if (!PlayerLoopUtils.InsertSystem<Update>(ref currentPlayerLoop,in tickSystem,0))
            {
                Debug.LogWarning("TickManager not assigned  into the Update loop.");
                return;
            }
            
            PlayerLoop.SetPlayerLoop(currentPlayerLoop);
            
#if UNITY_EDITOR
            EditorApplication.playModeStateChanged -= OnPlayModeState;
            EditorApplication.playModeStateChanged += OnPlayModeState;
            
            static void OnPlayModeState(PlayModeStateChange state) {
                if (state == PlayModeStateChange.ExitingPlayMode) {
                    PlayerLoopSystem currentPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();
                    PlayerLoopUtils.RemoveSystem<Update>(ref currentPlayerLoop, in tickSystem);
                    PlayerLoop.SetPlayerLoop(currentPlayerLoop);
                    TickManager.Shutdown();
                }
            }
#endif
        } 
    }
}