using System;
using Unity.Mathematics;
namespace Common.TickSystem
{
    [Serializable]
    public struct TickData
    {
        public int tickCount;
        public int maxTick;
        
        public float tickInterval;
        public float elapsedTime;
        
        public float invTickInterval;
        public TickData(float tickInterval, int maxTick)
        {
            this.tickCount = 0;

            this.maxTick = maxTick;
            this.tickInterval = math.max(0.0001f, tickInterval);
            this.invTickInterval = 1f / this.tickInterval;
            this.elapsedTime = 0;
        }
            
        public int Update(float deltaTime)
        {
            elapsedTime += deltaTime;

            int delta = (int)(elapsedTime * invTickInterval);
            if (delta <= 0)
                return 0;

            elapsedTime -= delta * tickInterval;
            tickCount += delta;

            return delta;
        }

        public bool ReachedMaxTick()
        {
            return this.tickCount >= this.maxTick && this.maxTick != -1;
        }
    }
}