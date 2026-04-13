using Common.TickSystem;
using UnityEngine;
namespace TickSystem
{
    public class ExampleTicker : MonoTicker
    {
        
        public override void OnTick(int delta)
        {
            for (int i = 0; i < delta; i++)
                Debug.Log("Tick!" + this.name);
        }
        public override void OnFinished()
        {
            Debug.Log("Finished!" + this.name);
        }
    }
}