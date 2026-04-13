using System;
using Common.TickSystem;

namespace StatAndEffects.Effects
{
    [Serializable]
    internal class EffectOverTimePipeline : EffectPipeline, ITicker
    {
        public float tickInterval = 1f;
        public int maxTicks = 5;

        public int Index { get; set; } = -1;
        public bool IsPaused { get; set; } = false;
        
        private float inputValue;
        public override void Initialize(IEntityStats stats)
        {
            base.Initialize(stats);
            TickManager.RegisterPaused(this, new TickData(this.tickInterval, this.maxTicks));

        }
        public override void ExecuteEffect(float value = 0)
        {
            TickManager.Resume(this);
            inputValue = value;
        }

        public void OnTick(int delta)
        {
            if (IsPaused) return;
            for (int i = 0; i < delta; i++)
                ProcessInternal(this.inputValue);
        }

        public void OnFinished()
        {
            inputValue = 0;
        }
    }
}