
using System;

namespace Common.TickSystem
{

    public interface ITicker
    {
        public int Index { get; set; }
        public bool IsPaused { get; set; }
        public void OnTick(int delta);
        public void OnFinished();
    }

}
