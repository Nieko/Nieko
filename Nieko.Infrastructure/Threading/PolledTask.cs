using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.Threading
{
    /// <summary>
    /// Facade for a regularly repeated task executed on
    /// a different thread that returns a boolean indicating
    /// whether or not it has finished
    /// </summary>
    public class PolledTask
    {
        private object _LockObject = new object();
        private readonly Func<bool> _Task;
        private readonly int _Frequency;
        private System.Threading.Timer _Timer;
        private Action _FinishedCallback;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="task">Task to perform</param>
        /// <param name="frequency">Frequency at which to perform it</param>
        /// <param name="finishedCallback">Method to call when finished</param>
        public PolledTask(Func<bool> task, int frequency, Action finishedCallback)
        {
            _Task = task;
            _Frequency = frequency;
            _FinishedCallback = finishedCallback;
        }

        /// <summary>
        /// Begins execution
        /// </summary>
        public void Start()
        {
            _Timer = new System.Threading.Timer(Tick, null, 0, _Frequency);
        }

        private void Tick(object state)
        {
            lock (_LockObject)
            {
                if (_Task() && _Timer != null)
                {
                    _Timer.Dispose();
                    _Timer = null;
                    _FinishedCallback();
                }
            }
        }
    }
}