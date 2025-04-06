using System;

namespace Server.Models.EventManager.Events
{
    /*
     * Concrete implementation of an event
     */
    public class PlayerKilled : IEvent
    {
        public PlayerKilled(object eventType, object eventData)
        {
            this.EventType = eventType;
            this.EventData = eventData;
        }

        public object EventType { get; private set; }

        public object EventData { get; private set; }
    }

    public class PlayerKilledEventArgs : EventArgs
    {
        public MapObject Killer { get; set; }

    }
}
