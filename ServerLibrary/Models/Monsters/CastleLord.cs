using Library;
using Server.DBModels;
using Server.Envir;
using System;
using System.Linq;
using S = Library.Network.ServerPackets;

namespace Server.Models.Monsters
{
    /// <summary>
    /// 沙巴克BOSS
    /// </summary>
    public class CastleLord : JinchonDevil
    {
        /// <summary>
        /// 沙巴克BOSS
        /// </summary>
        public ConquestWarBoss War;
        /// <summary>
        /// 攻击
        /// </summary>
        /// <param name="attacker"></param>
        /// <param name="power"></param>
        /// <param name="element"></param>
        /// <param name="canReflect"></param>
        /// <param name="ignoreShield"></param>
        /// <param name="canCrit"></param>
        /// <param name="canStruck"></param>
        /// <returns></returns>
        public override int Attacked(MapObject attacker, int power, Element element, bool canReflect = true, bool ignoreShield = false, bool canCrit = true, bool canStruck = true)
        {
            if (attacker == null || attacker.Race != ObjectType.Player) return 0;

            PlayerObject player = (PlayerObject)attacker;

            if (War == null) return 0;

            if (player.Character.Account.GuildMember == null) return 0;

            if (player.Character.Account.GuildMember.Guild.Castle != null) return 0;

            if (War.Participants.Count > 0 && !War.Participants.Contains(player.Character.Account.GuildMember.Guild)) return 0;

            int result = base.Attacked(attacker, 1, element, canReflect, ignoreShield, canCrit);

            #region Conquest Stats

            switch (attacker.Race)
            {
                case ObjectType.Player:
                    UserConquestStats conquest = SEnvir.GetConquestStats((PlayerObject)attacker);

                    if (conquest != null)
                        conquest.BossDamageDealt += result;
                    break;
                case ObjectType.Monster:
                    MonsterObject mob = (MonsterObject)attacker;
                    if (mob.PetOwner != null)
                    {
                        conquest = SEnvir.GetConquestStats(mob.PetOwner);

                        if (conquest != null)
                            conquest.BossDamageDealt += result;
                    }
                    break;
            }

            #endregion


            EXPOwner = null;


            return result;
        }
        /// <summary>
        /// 施毒
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public override bool ApplyPoison(Poison p)
        {
            return false;
        }
        /// <summary>
        /// 重生过程
        /// </summary>
        public override void ProcessRegen() { }
        /// <summary>
        /// 锁定攻击目标
        /// </summary>
        /// <param name="ob"></param>
        /// <returns></returns>
        public override bool ShouldAttackTarget(MapObject ob)
        {
            if (Passive || ob == this || ob?.Node == null || ob.Dead || !ob.Visible || ob is Guard || ob is CastleLord) return false;

            switch (ob.Race)
            {
                case ObjectType.Item:
                case ObjectType.NPC:
                case ObjectType.Spell:
                case ObjectType.Monster:
                    return false;
            }

            if (ob.Buffs.Any(x => x.Type == BuffType.Invisibility) && !CoolEye) return false;
            if (ob.Buffs.Any(x => x.Type == BuffType.Cloak))
            {
                if (!CoolEye) return false;
                if (!Functions.InRange(ob.CurrentLocation, CurrentLocation, 2)) return false;
                if (ob.Level >= Level) return false;
            }
            if (ob.Buffs.Any(x => x.Type == BuffType.Transparency)) return false;

            switch (ob.Race)
            {
                case ObjectType.Player:
                    PlayerObject player = (PlayerObject)ob;
                    //if (player.GameMaster) return false;
                    if (player.Observer) return false;

                    return player.Character.Account.GuildMember?.Guild.Castle != War.info;
                default:
                    throw new NotImplementedException();
            }
        }
        /// <summary>
        /// 可以攻击的目标
        /// </summary>
        /// <param name="ob"></param>
        /// <returns></returns>
        public override bool CanAttackTarget(MapObject ob)
        {
            if (ob == this || ob?.Node == null || ob.Dead || !ob.Visible || ob is Guard || War == null) return false;

            switch (ob.Race)
            {
                case ObjectType.Item:
                case ObjectType.NPC:
                case ObjectType.Spell:
                case ObjectType.Monster:
                    return false;
            }

            switch (ob.Race)
            {
                case ObjectType.Player:
                    PlayerObject player = (PlayerObject)ob;
                    //if (player.GameMaster) return false;
                    if (player.Observer) return false;

                    return player.Character.Account.GuildMember?.Guild.Castle != War.info;
                default:
                    throw new NotImplementedException();
            }

        }
        /// <summary>
        /// 死亡
        /// </summary>
        public override void Die()
        {
            if (War != null)
            {
                if (EXPOwner?.Node == null) return;

                if (EXPOwner.Character.Account.GuildMember == null) return;

                if (EXPOwner.Character.Account.GuildMember.Guild.Castle != null) return;

                if (War.Participants.Count > 0 && !War.Participants.Contains(EXPOwner.Character.Account.GuildMember.Guild)) return;

                #region Conquest Stats

                UserConquestStats conquest = SEnvir.GetConquestStats((PlayerObject)EXPOwner);

                if (conquest != null)
                    conquest.BossKillCount++;

                #endregion

                GuildInfo ownerGuild = SEnvir.GuildInfoList.Binding.FirstOrDefault(x => x.Castle == War.info);

                if (ownerGuild != null)
                    ownerGuild.Castle = null;

                EXPOwner.Character.Account.GuildMember.Guild.Castle = War.info;

                foreach (SConnection con in SEnvir.Connections)
                    con.ReceiveChat("Conquest.ConquestCapture".Lang(con.Language, EXPOwner.Character.Account.GuildMember.Guild.GuildName, War.info.Name), MessageType.System);

                SEnvir.Broadcast(new S.GuildCastleInfo { Index = War.info.Index, Owner = EXPOwner.Character.Account.GuildMember.Guild.GuildName });

                War.CastleBoss = null;

                War.PingPlayers();
                War.SpawnBoss();

                if (War.EndTime - SEnvir.Now < TimeSpan.FromMinutes(15))
                    War.EndTime = SEnvir.Now.AddMinutes(15);

                //foreach (PlayerObject player in SEnvir.Players)
                //player.ApplyCastleBuff();

                War = null;
            }

            base.Die();
        }
    }
}
