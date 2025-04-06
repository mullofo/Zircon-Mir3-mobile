using Server.DBModels;
using System;

namespace Server.Models.EventManager.Events
{
    /*
     * Concrete implementation of an event
     */
    public class PlayerMine : IEvent
    {
        public PlayerMine(object eventType, object eventData)
        {
            this.EventType = eventType;
            this.EventData = eventData;

        }

        public object EventType { get; private set; }

        public object EventData { get; private set; }
    }

    public class PlayerMineEventArgs : EventArgs
    {
        public UserItem item { get; set; }
    }
}
