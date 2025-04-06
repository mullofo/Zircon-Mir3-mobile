using Server.Envir;
using Server.Models.Monsters;
using System.Linq;

namespace Server.Models
{
    /// <summary>
    /// 攻城类型BOSS玩法
    /// </summary>
    public sealed class ConquestWarBoss : ConquestWar
    {
        /// <summary>
        /// 城堡领主BOSS
        /// </summary>
        public CastleLord CastleBoss;

        protected override void OnStartWar()
        {
            base.OnStartWar();
            SpawnBoss();
        }
        protected override void OnEndWar()
        {
            base.OnEndWar();
            DespawnBoss();
        }
        /// <summary>
        /// 结束时清除BOSS
        /// </summary>
        void DespawnBoss()
        {
            if (CastleBoss == null) return;

            CastleBoss.EXPOwner = null;
            CastleBoss.War = null;
            CastleBoss.Die();
            CastleBoss.Despawn();
            CastleBoss = null;
        }
        /// <summary>
        /// 开始时刷新BOSS
        /// </summary>
        public void SpawnBoss()
        {
            CastleBoss = new CastleLord
            {
                MonsterInfo = SEnvir.MonsterInfoList.Binding.FirstOrDefault(n => n.Index == info.InfoID),
                War = this,
            };
            CastleBoss.Spawn(info.CastleRegion);
        }
    }
}
