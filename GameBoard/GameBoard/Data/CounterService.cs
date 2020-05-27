using System;

namespace GameBoard.Data
{
    public delegate void CounterChangeDelegate(object sender, int value);
        
    public class CounterService
    {
        public event CounterChangeDelegate OnCounterChanged;
        private int counter = 0;
        
        public int GetCounter()
        {
            return counter;
        }

        public void IncrementCounter()
        {
            counter++;
            OnCounterChanged(this, counter);
        }
    }
}