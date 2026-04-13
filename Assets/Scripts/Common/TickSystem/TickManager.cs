using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Common.TickSystem
{
    public static class TickManager  
    {
        private const int Capacity = 16384;

        private static NativeArray<TickData> runningTicks;
        private static NativeArray<TickData> pausedTicks;
        
        private static JobHandle lastHandle;
        private static NativeArray<int> lastTickDeltas;
        private static NativeList<int> lastPauseRequests;

        private static List<ITicker> runningTickers;
        private static List<ITicker> pausedTickers;

        private static int runningCount;
        private static int pausedCount;

        private static TickerUpdateJob job;

#if UNITY_EDITOR
        public static int Debug_RunningCount;
        public static int Debug_PausedCount;
        public static int Debug_TickCalls;
        public static int Debug_FinishedThisFrame;
        public static float Debug_AvgDelta;
#endif
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Init()
        {
            Debug_TickCalls = 0;
            Debug_FinishedThisFrame = 0;

            
            runningTicks = new NativeArray<TickData>(Capacity, Allocator.Persistent);
            pausedTicks  = new NativeArray<TickData>(Capacity, Allocator.Persistent);

            lastTickDeltas = new NativeArray<int>(Capacity, Allocator.Persistent);
            lastPauseRequests = new NativeList<int>(Capacity, Allocator.Persistent);
            
            runningTickers = new List<ITicker>(1024);
            pausedTickers  = new List<ITicker>(1024);

            runningCount = 0;
            pausedCount = 0;
        }
        
        public static int RegisterRunning(ITicker ticker, TickData data)
        {
            int index = runningCount++;
            
            ticker.Index = index;
            ticker.IsPaused = false;
            
            runningTickers.Add(ticker);
            runningTicks[index] = data;

            return index;
        }
        
        public static int RegisterPaused(ITicker ticker, TickData data)
        {
            int index = pausedCount++;

            ticker.Index = index;
            ticker.IsPaused = true;

            pausedTickers.Add(ticker);
            pausedTicks[index] = data;

            return index;
        }

        public static void Unregister(ITicker ticker)
        {
            if (ticker.Index < 0)
                return;

            if (ticker.IsPaused)
                SwapRemovePaused(ticker.Index);
            else
                SwapRemoveRunning(ticker.Index);

            ticker.Index = -1;
        }

        public static void Pause(ITicker ticker)
        {
            if (ticker.IsPaused)
                return;

            MoveToPaused(ticker.Index);
        }

        public static void Resume(ITicker ticker)
        {
            if (!ticker.IsPaused)
                return;

            int pausedIndex = ticker.Index;
            int runIndex = runningCount++;

            ticker.Index = runIndex;
            ticker.IsPaused = false;

            runningTickers.Add(ticker);
            runningTicks[runIndex] = pausedTicks[pausedIndex];

            SwapRemovePaused(pausedIndex);
        }

        public static void Update()
        {
            
            if (runningCount == 0)
                return;
            
            lastHandle.Complete();
            
            for (int i = 0; i < runningCount; i++)
            {
                int delta = lastTickDeltas[i];
                if (delta > 0)
                {
                    runningTickers[i].OnTick(delta);
                    Debug_TickCalls++;
                }
            }

            for (int i = lastPauseRequests.Length - 1; i >= 0; i--)
            {
                int index = lastPauseRequests[i];
                var ticker = runningTickers[index];
                MoveToPaused(index);
                ticker.OnFinished();;
                Debug_FinishedThisFrame++;
            }

            lastPauseRequests.Clear();
            
            job.deltaTime = Time.deltaTime;
            job.runningTicks = runningTicks;
            job.tickDeltas = lastTickDeltas;
            job.pauseRequests = lastPauseRequests.AsParallelWriter();

            lastHandle = job.Schedule(runningCount, 64);
            Debug_RunningCount = runningCount;
            Debug_PausedCount = pausedCount;

        }


        private static void MoveToPaused(int index)
        {
            pausedTicks[pausedCount] = runningTicks[index];
            pausedTickers.Add(runningTickers[index]);
            pausedCount++;

            SwapRemoveRunning(index);
        }
        
        private static void SwapRemoveRunning(int index)
        {
            int last = runningCount - 1;
            if (index != last)
            {
                runningTicks[index] = runningTicks[last];
                runningTickers[index] = runningTickers[last];

                runningTickers[index].Index = index; 
            }
            runningTickers.RemoveAt(last);
            runningCount--;
        }

        private static void SwapRemovePaused(int index)
        {
            int last = pausedCount - 1;
            if (index != last)
            {
                pausedTicks[index] = pausedTicks[last];
                pausedTickers[index] = pausedTickers[last];

                pausedTickers[index].Index = index; 
            }

            pausedTickers.RemoveAt(last);
            pausedCount--;
        }

        public static void Shutdown()
        {
            
            lastHandle.Complete();

            if (runningTicks.IsCreated) runningTicks.Dispose();
            if (pausedTicks.IsCreated) pausedTicks.Dispose();

            if (lastTickDeltas.IsCreated) lastTickDeltas.Dispose();
            if (lastPauseRequests.IsCreated) lastPauseRequests.Dispose();
        }
    }

}
