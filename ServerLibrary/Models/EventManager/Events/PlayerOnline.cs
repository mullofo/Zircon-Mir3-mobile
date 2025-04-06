using System;

namespace Server.Models.EventManager.Events
{
    /*
     * Concrete implementation of an event
     */
    public class PlayerOnline : IEvent
    {
        public PlayerOnline(object eventType, object eventData)
        {
            this.EventType = eventType;
            this.EventData = eventData;

        }

        public object EventType { get; private set; }

        public object EventData { get; private set; }
    }

    public class PlayerOnlineEventArgs : EventArgs
    {
        public TimeSpan OnlineTime;

    }
}