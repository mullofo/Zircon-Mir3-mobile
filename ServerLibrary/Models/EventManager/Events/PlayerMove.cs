using Library;
using System;

namespace Server.Models.EventManager.Events
{
    /*
     * Concrete implementation of an event
     */
    public class PlayerMove : IEvent
    {
        public PlayerMove(object eventType, object eventData)
        {
            this.EventType = eventType;
            this.EventData = eventData;

        }

        public object EventType { get; private set; }

        public object EventData { get; private set; }
    }

    public class PlayerMoveEventArgs : EventArgs
    {
        public int numSteps { get; set; }
        public MirDirection direction { get; set; }
    }
}
