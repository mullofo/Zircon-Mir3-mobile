﻿using Library;
using Server.Envir;

namespace Server.Models.Monsters
{
    public class OmaMage : SkeletonAxeThrower    //半兽法师
    {
        protected override void Attack()
        {
            switch (SEnvir.Random.Next(3))
            {
                case 0:
                    AttackMagic(MagicType.LightningBall, Element.Lightning, false, GetDC() * 2 / 3);
                    break;
                case 1:
                    AttackMagic(MagicType.ThunderBolt, Element.Lightning, false, GetDC());
                    break;
                case 2:
                    AttackAoE(1, MagicType.LightningWave, Element.Lightning, GetDC() * 2 / 3);
                    break;
            }
        }
    }
}
