using Library;
using Server.Envir;
using System;
using System.Collections.Generic;
using System.Drawing;
using DateTime = System.DateTime;
using S = Library.Network.ServerPackets;

namespace Server.Models.Monsters
{
    public class SiegeEquipment : MonsterObject  //攻城器械
    {
        public DateTime FearTime;
        /// <summary>
        /// 攻击开关对应模式选择
        /// </summary>
        public override bool CanAttack => base.CanAttack;
        public bool MoveMode;
        public override bool CanMove => base.CanMove && MoveMode;

        public bool AttackStop = true;
        /// <summary>
        /// 攻击范围
        /// </summary>
        public int AttackRange = 4;

        public int CurrentZidan
        {
            get { return _CurrentZidan; }
            set
            {
                if (_CurrentZidan == value) return;
                var oldValue = _CurrentZidan;
                _CurrentZidan = value;

                CurrentZidanChanged(oldValue, value);
            }
        }
        private int _CurrentZidan;

        private void CurrentZidanChanged(int oldValue, int value)
        {
            PetOwner?.Enqueue(new S.AmmAmmunition { Success = true, Count = CurrentZidan });
        }
        public int MaxZidan = 0;

        public Point TargetPoint;
        /// <summary>
        /// 在攻击范围内
        /// </summary>
        /// <returns></returns>
        protected override bool InAttackRange()
        {
            return CurrentMap == Target.CurrentMap && Functions.InRange(CurrentLocation, Target.CurrentLocation, AttackRange);
        }

        public override void ProcessTarget()
        {
            if (PetOwner == null || PetOwner.Dead) return;
            if (CurrentZidan == 0) return;
            if (TargetPoint == Point.Empty) return;

            if (AttackStop || SEnvir.Now < FearTime) return;

            FearTime = SEnvir.Now.AddSeconds(1);
            RangeAttack();
        }
        public override int Attacked(MapObject attacker, int power, Element element, bool canReflect = true, bool ignoreShield = false, bool canCrit = true, bool canStruck = true)
        {
            if (attacker == null) return 0;

            foreach (var pet in PetOwner.Pets)
            {
                if (pet.MonsterInfo.Image == MonsterImage.Catapult)  //投石车
                {
                    PetOwner?.Connection.ReceiveChat("投石车 被攻击。", MessageType.System);
                    break;
                }
                if (pet.MonsterInfo.Image == MonsterImage.Ballista)  //弩车
                {
                    PetOwner?.Connection.ReceiveChat("弩车 被攻击。", MessageType.System);
                    break;
                }
            }

            return base.Attacked(attacker, power, element, canReflect, ignoreShield, canCrit, canStruck);
        }

        private void RangeAttack()
        {
            CurrentZidan--;
            Target = null;

            Point realPoint = TargetPoint;
            realPoint.X = SEnvir.Random.Next(TargetPoint.X - 4, TargetPoint.X + 4);
            realPoint.Y = SEnvir.Random.Next(TargetPoint.Y - 4, TargetPoint.Y + 4);
            Cell cell = CurrentMap.Cells[realPoint.X, realPoint.Y];

            Direction = Functions.DirectionFromPoint(CurrentLocation, TargetPoint);

            SpellObject ob = new SpellObject
            {
                DisplayLocation = realPoint,
                TickCount = 1,
                TickFrequency = TimeSpan.FromSeconds(1),
                Owner = this,
                Effect = SpellEffect.WarWeaponShell,
                Direction = Functions.ShiftDirection(Direction, 4),
            };

            ob.Spawn(CurrentMap.Info, realPoint);

            if (cell == null || cell.Objects == null)
            {
                Broadcast(new S.ObjectAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });
                return;
            }
            int[] ary = { 530, 531, 532, 533 };
            bool flag = true;
            foreach (var obj in cell.Objects)
            {
                if (!CanAttackTarget(obj)) continue;
                if (obj != null && !obj.Dead && (obj.Race == ObjectType.Player || obj.Race == ObjectType.Monster))
                {
                    if (obj.Race == ObjectType.Monster && (Array.IndexOf(ary, (obj as MonsterObject).MonsterInfo.BodyShape) != -1) && (obj as MonsterObject).Direction == MirDirection.UpLeft) continue;
                    Target = obj;
                    base.Attack();
                    flag = false;
                }
            }
            if (flag) Broadcast(new S.ObjectAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });

            List<Cell> cells = CurrentMap.GetCells(realPoint, 1, 2);
            foreach (Cell cell1 in cells)
            {
                if (cell1.Objects == null) continue;

                foreach (MapObject ob1 in cell1.Objects)
                {
                    if (SEnvir.Random.Next(2) > 0) continue;
                    if (!CanAttackTarget(ob1)) continue;
                    if (ob1.Race == ObjectType.Monster && (Array.IndexOf(ary, (ob1 as MonsterObject).MonsterInfo.BodyShape) != -1) && (ob1 as MonsterObject).Direction == MirDirection.UpLeft) continue;

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(500),
                        ActionType.DelayAttack,
                        ob1,
                        GetDC(),
                        Element.Fire));
                }
            }
        }

        public void ChaXunZidan()
        {
            //TODO
            //fasong dangqian zidanshu dao kehuduan
            // dangqianggongji zhuangtai 
        }
        public bool ZhuangDan(int count)
        {
            CurrentZidan = Math.Min(300, CurrentZidan + count);
            // CurrentZidan += count;

            //TODO
            //fabao gaosukehuduan zhuangdan chengogng  daiqu currentzidan
            return true;
        }

        public void miaozhuangmubiao(int x, int y)
        {
            TargetPoint = new Point(x, y);
            AttackStop = false;
            /*
            Cell cell = CurrentMap.Cells[x, y];
            if (cell == null) return;
            if (cell.Objects == null) return;
            
            int[] ary = { 530, 531, 532, 533 };
            foreach (var obj in cell.Objects)
            {
                if (obj.Race == ObjectType.Monster && !obj.Dead && (Array.IndexOf(ary,(obj as MonsterObject).MonsterInfo.BodyShape) !=-1) && (obj as MonsterObject).Direction != MirDirection.UpLeft)//
                {
                    
                }
            }
            */
        }

        public void canelmubiao()
        {
            TargetPoint = Point.Empty;
        }
    }
}
