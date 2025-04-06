using Library;
using Library.Network;
using Server.Envir;
using System;
using S = Library.Network.ServerPackets;

namespace Server.Models.Monsters
{
    /// <summary>
    /// 神兽
    /// </summary>
    public class Shinsu : SpittingSpider
    {
        /// <summary>
        /// 攻击模式
        /// </summary>
        public bool Mode;
        /// <summary>
        /// 模式时间
        /// </summary>
        public DateTime ModeTime;
        /// <summary>
        /// 攻击开关对应模式选择
        /// </summary>
        public override bool CanAttack => base.CanAttack && Mode;
        /// <summary>
        /// 神兽
        /// </summary>
        public Shinsu()
        {
            Visible = false;
            ActionList.Add(new DelayedAction(SEnvir.Now.AddSeconds(1), ActionType.Function));
        }
        /// <summary>
        /// 神兽刷新时
        /// </summary>
        protected override void OnSpawned()
        {
            base.OnSpawned();
            //显示神兽出现
            CurrentMap.Broadcast(CurrentLocation, new S.MapEffect { Location = CurrentLocation, Effect = Effect.SummonShinsu, Direction = Direction });

            ActionTime = SEnvir.Now.AddSeconds(2);
        }
        /// <summary>
        /// 可以被看到
        /// </summary>
        /// <param name="ob">角色对象</param>
        /// <returns></returns>
        public override bool CanBeSeenBy(PlayerObject ob)
        {
            return Visible && base.CanBeSeenBy(ob);
        }
        /// <summary>
        /// 动作过程
        /// </summary>
        /// <param name="action">动作行为</param>
        public override void ProcessAction(DelayedAction action)
        {
            switch (action.Type)
            {
                case ActionType.Function:
                    Appear();
                    return;
            }

            base.ProcessAction(action);
        }

        public void Appear()
        {
            Visible = true;
            AddAllObjects();
        }
        public override void Process()
        {
            if (!Dead && SEnvir.Now > ActionTime)
            {
                if (Target != null) ModeTime = SEnvir.Now.AddSeconds(10);

                if (!Mode && SEnvir.Now < ModeTime)
                {
                    Mode = true;
                    Broadcast(new S.ObjectShow { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });
                    ActionTime = SEnvir.Now.AddSeconds(2);
                }
                else if (Mode && SEnvir.Now > ModeTime)
                {
                    Mode = false;
                    Broadcast(new S.ObjectHide() { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });
                    ActionTime = SEnvir.Now.AddSeconds(2);
                }
            }
            base.Process();
        }

        public override Packet GetInfoPacket(PlayerObject ob)
        {
            S.ObjectMonster packet = (S.ObjectMonster)base.GetInfoPacket(ob);

            packet.Extra = Mode;

            return packet;
        }
    }
}