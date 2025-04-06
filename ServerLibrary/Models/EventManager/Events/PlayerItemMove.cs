using Library;
using Library.SystemModels;
using System;

namespace Server.Models.EventManager.Events
{
    /*
     * Concrete implementation of an event
     */
    public class PlayerItemMove : IEvent
    {
        public PlayerItemMove(object eventType, object eventData)
        {
            this.EventType = eventType;
            this.EventData = eventData;

        }

        public object EventType { get; private set; }

        public object EventData { get; private set; }
    }

    public class PlayerItemMoveEventArgs : EventArgs
    {
        public GridType FromGridType { get; set; }
        public GridType ToGridType { get; set; }
        public ItemInfo Item { get; set; }
    }
}