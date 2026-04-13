using Unity.Collections;
using Unity.Burst;
using Unity.Jobs;
namespace Common.TickSystem
{
    [BurstCompile]
    public struct TickerUpdateJob : IJobParallelFor
    {
        public float deltaTime;
        
        public NativeArray<TickData> runningTicks;
        public NativeArray<int> tickDeltas;
        
        public NativeList<int>.ParallelWriter pauseRequests;
        
        public void Execute(int index)
        {
            TickData data = runningTicks[index];
            int delta = data.Update(deltaTime);

            tickDeltas[index] = delta;

            if (data.ReachedMaxTick())
                pauseRequests.AddNoResize(index);
            runningTicks[index] = data;
            
        }
    }
}