using System;

namespace Server.Models.EventManager.Events
{
    /*
     * Concrete implementation of an event
     */
    public class PlayerHarvest : IEvent
    {
        public PlayerHarvest(object eventType, object eventData)
        {
            this.EventType = eventType;
            this.EventData = eventData;

        }

        public object EventType { get; private set; }

        public object EventData { get; private set; }
    }

    public class PlayerHarvestEventArgs : EventArgs
    {
        public bool success { get; set; }
    }
}
