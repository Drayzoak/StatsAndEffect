using System;
using UnityEngine;
namespace Common.TickSystem
{
    public abstract class MonoTicker : MonoBehaviour , ITicker
    {
        [Header("Tick Settings")]
        public TickData tickData;

        public int Index { get; set; } = -1;
        public bool IsPaused { get; set; }

        protected virtual void OnEnable()
        {
            Index = TickManager.RegisterRunning(this,this.tickData );
        }
        
        protected virtual void OnDisable()
        {
            TickManager.Unregister(this);
        }

        public void Pause()
        {
            TickManager.Pause(this);
        }

        public void Resume()
        {
            TickManager.Resume(this);
        }
        
        public abstract void OnTick(int delta);
        public abstract void OnFinished();
    }
}