using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GHPC.Event;
using UnityEngine;

namespace CustomMissionUtility.Events
{
    public class MissionEvent {
        GameObject event_holder;
        public GhpcEvent m_event;

        public MissionEvent(GhpcEvent.EventTriggerOperatorMode mode) {
            event_holder = new GameObject();
            m_event = event_holder.AddComponent<GhpcEvent>();
            m_event._label = "";
            m_event._mode = mode;
        }

        public T Add<T>() {
            return (T) Convert.ChangeType(event_holder.AddComponent(typeof(T)), typeof(T));
        }
    } 
}
