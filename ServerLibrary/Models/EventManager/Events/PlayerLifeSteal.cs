using System;

namespace Server.Models.EventManager.Events
{
    /*
     * Concrete implementation of an event
     */
    public class PlayerLifeSteal : IEvent
    {
        public PlayerLifeSteal(object eventType, object eventData)
        {
            this.EventType = eventType;
            this.EventData = eventData;

        }

        public object EventType { get; private set; }

        public object EventData { get; private set; }
    }

    public class PlayerLifeStealEventArgs : EventArgs
    {
        public MapObject target { get; set; }
        public decimal amount { get; set; }
    }
}
