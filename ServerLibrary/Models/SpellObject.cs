using Library;
using Library.Network;
using Server.DBModels;
using Server.Envir;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using S = Library.Network.ServerPackets;


namespace Server.Models
{
    public sealed class SpellObject : MapObject  //施法对象
    {
        public override ObjectType Race => ObjectType.Spell;

        public override bool Blocking => false;

        public Point DisplayLocation;
        public SpellEffect Effect;
        public int TickCount;
        public TimeSpan TickFrequency;
        public DateTime TickTime;
        public MapObject Owner;
        public UserMagic Magic;
        public int Power;

        public List<MapObject> Targets = new List<MapObject>();

        public override bool CanBeSeenBy(PlayerObject ob)  //可见
        {
            return Visible && base.CanBeSeenBy(ob);
        }

        public override void Process()  //过程
        {
            base.Process();

            if (Owner != null && (Owner.Node == null || Owner.Dead))
            {
                Despawn();
                return;
            }

            if (SEnvir.Now < TickTime) return;

            if (TickCount-- <= 0)
            {
                switch (Effect)
                {
                    case SpellEffect.SwordOfVengeance:
                        PlayerObject player = Owner as PlayerObject;
                        if (player == null) break;

                        List<Cell> cells = CurrentMap.GetCells(CurrentLocation, 0, 3);

                        foreach (Cell cell in cells)
                        {
                            if (cell.Objects != null)
                            {
                                for (int i = cell.Objects.Count - 1; i >= 0; i--)
                                {
                                    if (i >= cell.Objects.Count) continue;
                                    MapObject target = cell.Objects[i];

                                    if (!player.CanAttackTarget(target)) continue;

                                    int damage = player.MagicAttack(new List<UserMagic> { Magic }, target, true);

                                    ActionList.Add(new DelayedAction(
                                        SEnvir.Now.AddMilliseconds(500),
                                        ActionType.DelayMagic,
                                        new List<UserMagic> { Magic },
                                        target));
                                }
                            }
                        }

                        break;
                    case SpellEffect.MonsterDeathCloud:
                        MonsterObject monster = Owner as MonsterObject;
                        if (monster == null) break;

                        for (int i = CurrentCell.Objects.Count - 1; i >= 0; i--)
                        {
                            if (i >= CurrentCell.Objects.Count) continue;

                            MapObject ob = CurrentCell.Objects[i];

                            if (!monster.CanAttackTarget(ob)) continue;

                            monster.Attack(ob, 4000, Element.None);
                            //monster.Attack(ob, 4000, Element.None);
                        }

                        break;
                }

                Despawn();
                return;
            }

            TickTime = SEnvir.Now + TickFrequency;

            switch (Effect)
            {
                case SpellEffect.TrapOctagon:

                    for (int i = Targets.Count - 1; i >= 0; i--)
                    {
                        MapObject ob = Targets[i];

                        if (ob.Node != null && ob.ShockTime != DateTime.MinValue) continue;

                        Targets.Remove(ob);
                    }

                    if (Targets.Count == 0) Despawn();
                    break;
                default:

                    if (CurrentCell == null)
                    {
                        SEnvir.Log($"[错误] {Effect} 角色单元为空.");
                        return;
                    }

                    if (CurrentCell.Objects == null)
                    {
                        SEnvir.Log($"[错误] {Effect} 角色单元的对象为空.");
                        return;
                    }

                    for (int i = CurrentCell.Objects.Count - 1; i >= 0; i--)
                    {
                        if (i >= CurrentCell.Objects.Count) continue;
                        if (CurrentCell.Objects[i] == this) continue;

                        ProcessSpell(CurrentCell.Objects[i]);

                        if (CurrentCell == null)
                        {
                            SEnvir.Log($"[错误] {Effect} 角色单元为空循环.");
                            return;
                        }

                        if (CurrentCell.Objects == null)
                        {
                            SEnvir.Log($"[错误] {Effect} 角色单元的对象为空循环.");
                            return;
                        }
                    }
                    break;
            }
        }

        public void ProcessSpell(MapObject ob)  //施法过程
        {
            switch (Effect)
            {
                case SpellEffect.PoisonousCloud:
                    if (!Owner.CanHelpTarget(ob)) return;

                    BuffInfo buff = ob.Buffs.FirstOrDefault(x => x.Type == BuffType.PoisonousCloud);
                    TimeSpan remaining = TickTime - SEnvir.Now;

                    if (buff != null)
                        if (buff.RemainingTime > remaining) return;

                    ob.BuffAdd(BuffType.PoisonousCloud, remaining, new Stats { [Stat.Agility] = Power }, false, false, TimeSpan.Zero);
                    break;
                case SpellEffect.FireWall:
                case SpellEffect.Tempest:
                    PlayerObject player = Owner as PlayerObject;
                    if (player == null || !player.CanAttackTarget(ob)) return;

                    int damage = player.MagicAttack(new List<UserMagic> { Magic }, ob, true);

                    if (damage > 0 && ob.Race == ObjectType.Player)
                    {
                        foreach (SpellObject spell in player.SpellList)
                        {
                            if (spell.Effect != Effect) continue;

                            spell.TickCount--;
                        }
                    }
                    break;
                case SpellEffect.MonsterFireWall:
                    MonsterObject monster = Owner as MonsterObject;
                    if (monster == null || !monster.CanAttackTarget(ob)) return;

                    monster.Attack(ob, monster.GetDC(), Element.Fire);
                    break;
                case SpellEffect.DarkSoulPrison:
                    player = Owner as PlayerObject;
                    if (player == null || !player.CanAttackTarget(ob)) return;

                    damage = player.MagicAttack(new List<UserMagic> { Magic }, ob, true);

                    if (damage > 0 && ob.Race == ObjectType.Player)
                    {
                        foreach (SpellObject spell in player.SpellList)
                        {
                            if (spell.Effect != Effect) continue;

                            spell.TickCount--;
                        }
                    }
                    break;
                case SpellEffect.SwordOfVengeance:
                    player = Owner as PlayerObject;
                    if (player == null) break;

                    List<Cell> cells = CurrentMap.GetCells(CurrentLocation, 0, 3);

                    foreach (Cell cell in cells)
                    {
                        if (cell.Objects != null)
                        {
                            foreach (MapObject target in cell.Objects)
                            {
                                if (player.CanAttackTarget(target))
                                {
                                    TickCount = 0; ;
                                    break;
                                }
                            }
                        }
                    }
                    break;
                case SpellEffect.IceRain:
                    player = Owner as PlayerObject;

                    if (player == null || !player.CanAttackTarget(ob)) return;

                    damage = player.MagicAttack(new List<UserMagic> { Magic }, ob, true);

                    if (damage > 0 && ob.Race == ObjectType.Player)
                    {
                        foreach (SpellObject spell in player.SpellList)
                        {
                            if (spell.Effect != Effect) continue;

                            spell.TickCount--;
                        }
                    }
                    break;
            }
        }
        protected override void OnSpawned()  //在生成时
        {
            base.OnSpawned();

            Owner?.SpellList.Add(this);

            AddAllObjects();

            Activate();
        }
        public override void OnDespawned()  //已生成时
        {
            base.OnDespawned();

            Owner?.SpellList.Remove(this);
        }
        public override void OnSafeDespawn()  //安全生成时
        {
            base.OnSafeDespawn();

            Owner?.SpellList.Remove(this);
        }

        public override void CleanUp()  //清理
        {
            base.CleanUp();

            Owner = null;
            Magic = null;

            Targets?.Clear();
        }

        public override Packet GetInfoPacket(PlayerObject ob)  //获取信息包
        {
            return new S.ObjectSpell
            {
                ObjectID = ObjectID,
                Location = DisplayLocation,
                Effect = Effect,
                Direction = Direction,
                Power = Power,
            };
        }
        public override Packet GetDataPacket(PlayerObject ob)  //获取数据包
        {
            return null;
        }

        public override bool CanDataBeSeenBy(PlayerObject ob)  //数据包能看到
        {
            return false;
        }

        public override void Activate()  //激活
        {
            if (Activated) return;

            if (Effect == SpellEffect.SafeZone) return;

            Activated = true;
            SEnvir.ActiveObjects.Add(this);
        }
        public override void DeActivate()  //取消激活
        {
            return;
        }

        public override void ProcessHPMP()  //HP MP过程
        {
        }
        public override void ProcessNameColour()  //名字颜色过程
        {
        }
        public override void ProcessBuff()   //BUFF过程
        {
        }
        public override void ProcessPoison()  //施毒过程
        {
        }
    }
}
