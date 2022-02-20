using System;
using System.Collections.Generic;
using Stargate.Domain;
using UniLinq;

namespace Stargate.Utilities
{
    public class TickerWatch
    {
        public float TimeSeconds => _tick / 60f;

        private long _tick;
        private bool _isRunning;

        private ActionSchedule _currentAction;
        private List<ActionSchedule> _schedules = new List<ActionSchedule>();

        public void Start()
        {
            _tick = 0;
            _isRunning = true;
        }

        public void Stop()
        {
            _isRunning = false;
        }

        public void Tick()
        {
            if (!_isRunning)
            {
                return;
            }

            _tick++;

            if (_currentAction != null && _currentAction.Time < TimeSeconds)
            {
                _currentAction.Action();
                SetupNextCallback();
            }
        }

        private void SetupNextCallback()
        {
            _currentAction = _schedules.FirstOrDefault();
            if (_currentAction == null)
            {
                return;
            }

            _schedules.Remove(_currentAction);
        }

        public void DoAt(float time, Action action)
        {
            _schedules.Add(new ActionSchedule
            {
                Action = action,
                Time = time,
            });
            _schedules = _schedules
                .OrderBy(actionSchedule => actionSchedule.Time)
                .ToList();

            if (_currentAction == null)
            {
                SetupNextCallback();
            }
        }
    }
}
