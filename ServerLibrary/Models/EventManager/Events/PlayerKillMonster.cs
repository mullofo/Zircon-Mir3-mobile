using Library.SystemModels;
using System;

namespace Server.Models.EventManager.Events
{
    /*
     * Concrete implementation of an event
     */
    public class PlayerKillMonster : IEvent
    {
        public PlayerKillMonster(object eventType, object eventData)
        {
            this.EventType = eventType;
            this.EventData = eventData;
        }

        public object EventType { get; private set; }

        public object EventData { get; private set; }
    }

    public class PlayerKillMonsterEventArgs : EventArgs
    {
        public MonsterInfo KilledMonster { get; set; }

    }
}
