using Library;
using Library.Network;
using Server.DBModels;
using Server.Envir;
using System;
using S = Library.Network.ServerPackets;

namespace Server.Models.Monsters
{
    public class MirrorImage : MonsterObject   //镜像 分身
    {
        /// <summary>
        /// 玩家对象定义
        /// </summary>
        public PlayerObject Player;
        /// <summary>
        /// 是否可以攻击
        /// </summary>
        public override bool CanAttack
        {
            get
            {
                //return base.CanAttack && (Poison & PoisonType.Silenced) != PoisonType.Silenced && AttackDelay > 0 && (PetOwner == null || PetOwner.PetMode == PetMode.Both || PetOwner.PetMode == PetMode.Attack || PetOwner.PetMode == PetMode.PvP);
                return base.CanAttack && (Poison & PoisonType.Silenced) != PoisonType.Silenced && AttackDelay > 0;
            }
        }
        /// <summary>
        /// 是否可以移动
        /// </summary>
        public override bool CanMove => Config.MirrorImageCanMove;
        /// <summary>
        /// 石头属性
        /// </summary>
        public Stats DarkStoneStats;
        /// <summary>
        /// 技能
        /// </summary>
        public UserMagic Skill;
        private DateTime CastTime;
        /// <summary>
        /// 延迟消失时间设置
        /// </summary>
        public DateTime ExplodeTime = SEnvir.Now.AddSeconds(60);

        /// <summary>
        /// 过程
        /// </summary>
        public override void Process()
        {
            base.Process();

            if (Player?.Node == null)   //玩家为空 移除对象
            {
                Despawn();
                return;
            }

            // 如果超过了过期时间 移除对象
            if (Dead || SEnvir.Now < ExplodeTime || Player.Dead) return;

            SetHP(0); //设置为0 死亡
        }
        /// <summary>
        /// AI过程
        /// </summary>
        public override void ProcessAI()
        {
            if (Dead) return;

            //下面注释的内容是是否启用宠物攻击模式及超出一定范围自动飞到主人身边
            if (PetOwner?.Node != null)
            {
                if (Target != null)
                {
                    if (PetOwner.PetMode == PetMode.PvP && Target.Race != ObjectType.Player)
                        Target = null;

                    if (PetOwner.PetMode == PetMode.None || PetOwner.PetMode == PetMode.Move)
                        Target = null;
                }
                if (SEnvir.Now > TameTime)
                    UnTame();
                else if (Visible && !PetOwner.VisibleObjects.Contains(this) && (PetOwner.PetMode == PetMode.Both || PetOwner.PetMode == PetMode.Move || PetOwner.PetMode == PetMode.PvP))
                    PetRecall();
            }

            //ProcessRegen();
            ProcessSearch();
            ProcessRoam();
            ProcessTarget();
        }
        public override void ProcessTarget()
        {
            if (Target == null) return;
            if (!CanAttack) return;
            //魔法释放频率
            if (SEnvir.Now > CastTime)
            {
                RangeAttack();
                CastTime = SEnvir.Now.AddSeconds(2);
            }
        }

        public virtual void RangeAttack()
        {
            if (Skill == null) return;
            Element element = DarkStoneStats.GetAffinityElement();
            if (element == Element.None) return;
            if (Functions.InRange(Target.CurrentLocation, CurrentLocation, Globals.MagicRange))
                AttackAoE(1, Skill.Info.Magic, element);
        }
        /// <summary>
        /// 刷新的时候
        /// </summary>
        protected override void OnSpawned()
        {
            base.OnSpawned();

            //刷新时给分身赋值对应的出现动画特效
            CurrentMap.Broadcast(CurrentLocation, new S.MapEffect { Location = CurrentLocation, Effect = Effect.MirrorImage, Direction = Direction });

            Level = Player.Level;
            ActionTime = SEnvir.Now.AddSeconds(2);
        }
        /// <summary>
        /// 已经生成
        /// </summary>
        public override void OnDespawned()
        {
            base.OnDespawned();

            Player?.Pets.Remove(this);
        }
        /// <summary>
        /// 安全生成
        /// </summary>
        public override void OnSafeDespawn()
        {
            base.OnSafeDespawn();

            Player?.Pets.Remove(this);
        }
        public override void RefreshStats()
        {
            //base.RefreshStats();

            Stats.Clear();
            Stats.Add(Player.Stats);
            //道具复活时间最大，防止镜像复活
            ItemReviveTime = DateTime.MaxValue;

            MoveDelayBase = MonsterInfo.MoveDelay;
            AttackDelayBase = MonsterInfo.AttackDelay;

            S.DataObjectMaxHealthMana p = new S.DataObjectMaxHealthMana { ObjectID = ObjectID, Stats = Stats };

            foreach (PlayerObject player in DataSeenByPlayers)
                player.Enqueue(p);

            if (CurrentHP > Stats[Stat.Health]) SetHP(Stats[Stat.Health]);
            if (CurrentMP > Stats[Stat.Mana]) SetMP(Stats[Stat.Mana]);
        }
        /// <summary>
        /// 死亡
        /// </summary>
        public override void Die()
        {
            base.Die();
            //尸体时间设为当前，交由主线程执行清理Despawn()
            DeadTime = SEnvir.Now;

            if (Player?.Node == null) return;  //如果角色为空

            //死亡时给分身赋值对应的出现动画特效
            CurrentMap.Broadcast(CurrentLocation, new S.MapEffect { Location = CurrentLocation, Effect = Effect.MirrorImageDie, Direction = Direction });

            ActionTime = SEnvir.Now.AddSeconds(2);
        }

        /// <summary>
        /// 获取信息封包
        /// </summary>
        /// <param name="ob">玩家对象</param>
        /// <returns></returns>
        public override Packet GetInfoPacket(PlayerObject ob)
        {
            if (Player?.Node == null) return null;

            S.ObjectPlayer packet = (S.ObjectPlayer)Player.GetInfoPacket(null);

            packet.ObjectID = ObjectID;
            packet.Location = CurrentLocation;
            packet.Direction = Direction;
            packet.Dead = Dead;

            return packet;
        }
    }
}
