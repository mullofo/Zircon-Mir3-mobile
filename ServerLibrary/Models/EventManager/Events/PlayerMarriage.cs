using Server.DBModels;
using System;

namespace Server.Models.EventManager.Events
{
    /*
     * Concrete implementation of an event
     */
    public class PlayerMarriage : IEvent
    {
        public PlayerMarriage(object eventType, object eventData)
        {
            this.EventType = eventType;
            this.EventData = eventData;
        }

        public object EventType { get; private set; }

        public object EventData { get; private set; }
    }

    public class PlayerMarriageEventArgs : EventArgs
    {
        public bool IsDivorce;

        public CharacterInfo Partner;
    }
}