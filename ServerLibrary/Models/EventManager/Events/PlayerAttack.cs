using Library;
using System;

namespace Server.Models.EventManager.Events
{
    /*
     * Concrete implementation of an event
     */
    public class PlayerAttack : IEvent
    {
        public PlayerAttack(object eventType, object eventData)
        {
            this.EventType = eventType;
            this.EventData = eventData;

        }

        public object EventType { get; private set; }

        public object EventData { get; private set; }
    }

    public class PlayerAttackEventArgs : EventArgs
    {
        public MapObject target { get; set; }
        public int damage { get; set; }
        public Element element { get; set; }

    }
}
