using Server.DBModels;
using System;
using System.Collections.Generic;

namespace Server.Models.EventManager.Events
{
    /*
     * Concrete implementation of an event
     */
    public class PlayerGainItem : IEvent
    {
        public PlayerGainItem(object eventType, object eventData)
        {
            this.EventType = eventType;
            this.EventData = eventData;

        }

        public object EventType { get; private set; }

        public object EventData { get; private set; }
    }

    public class PlayerGainItemEventArgs : EventArgs
    {
        public List<UserItem> items { get; set; }

    }
}