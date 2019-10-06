using Game.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Game.Logic
{
    public class ActionScheduling
    {
        private int _time;
        private readonly SortedDictionary<int, List<IScheduleable>> _schedule;

        public ActionScheduling()
        {
            _time = 0;
            _schedule = new SortedDictionary<int, List<IScheduleable>>();
        }

        // Add a new action to the schedule
        // Place it at the current time plus its Time property
        public void Add(IScheduleable scheduleable)
        {
            int key = _time + scheduleable.Time;
            if(!_schedule.ContainsKey(key))
            {
                _schedule.Add(key, new List<IScheduleable>());
            }
            _schedule[key].Add(scheduleable);
        }

        // Remove a specific object from the schedule
        // It's used when a monster dies to remove it before its action comes up
        public void Remove(IScheduleable scheduleable)
        {
            KeyValuePair<int, List<IScheduleable>> scheduleableListFound
                = new KeyValuePair<int, List<IScheduleable>>(-1, null);

            foreach(var scheduleablesList in _schedule)
            {
                if(scheduleablesList.Value.Contains(scheduleable))
                {
                    scheduleableListFound = scheduleablesList;
                    break;
                }
            }
            if(scheduleableListFound.Value != null)
            {
                scheduleableListFound.Value.Remove(scheduleable);
                if(scheduleableListFound.Value.Count <= 0)
                {
                    _schedule.Remove(scheduleableListFound.Key);
                }
            }
        }

        // Get the next objects whose turn it is from the schedule. Advance time if necessary
        public IScheduleable Get()
        {
            var firstScheduleableGroup = _schedule.First();
            var firstScheduleable = firstScheduleableGroup.Value.First();
            Remove(firstScheduleable);
            _time = firstScheduleableGroup.Key;
            return firstScheduleable;
        }

        // Get the current time(turn) for the schedule
        public int GetTime()
        {
            return _time;
        }

        // Reset the time and clear out the schedule
        public void Clear()
        {
            _time = 0;
            _schedule.Clear();
        }


    }
}
