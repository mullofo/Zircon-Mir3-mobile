﻿using Library;
using Server.Envir;

namespace Server.Models.Monsters
{
    public class WingedHorror : UmaKing   //灵牛鬼将军
    {

        public override void RangeAttack()
        {
            switch (SEnvir.Random.Next(3))
            {
                case 0:
                    MassLightningBall();
                    break;
                case 1:
                    LineAoE(10, 0, 8, MagicType.LightningBeam, Element.Lightning);
                    break;
                case 2:
                    MassThunderBolt();
                    break;
            }
        }
    }
}
