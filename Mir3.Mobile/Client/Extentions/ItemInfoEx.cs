using Client.Envir;
using Library;
using Library.SystemModels;

namespace Client.Extentions
{

    public static class ItemInfoEx
    {

        public static void PlaySound(this ItemInfo info)
        {
            var type = info.ItemType;
            if (info.Effect == ItemEffect.Gold)
            {
                DXSoundManager.Play(SoundIndex.ItemGold);
            }
            else if (type == ItemType.Weapon)
            {
                DXSoundManager.Play(SoundIndex.ItemWeapon);
            }
            else if (type == ItemType.Consumable)
            {
                if (info.Shape == 0)//药品
                {

                    DXSoundManager.Play(SoundIndex.ItemPotion);
                    return;
                }
                DXSoundManager.Play(SoundIndex.ItemDefault);
            }
            else if (type == ItemType.Armour)
            {
                DXSoundManager.Play(SoundIndex.ItemArmour);
            }
            else if (type == ItemType.Helmet)
            {
                DXSoundManager.Play(SoundIndex.ItemHelmet);
            }
            else if (type == ItemType.Necklace)
            {
                DXSoundManager.Play(SoundIndex.ItemNecklace);
            }
            else if (type == ItemType.Bracelet)
            {
                DXSoundManager.Play(SoundIndex.ItemBracelet);
            }
            else if (type == ItemType.Ring)
            {
                DXSoundManager.Play(SoundIndex.ItemRing);
            }
            else if (type == ItemType.Shoes)
            {
                DXSoundManager.Play(SoundIndex.ItemShoes);
            }
            else
            {
                DXSoundManager.Play(SoundIndex.ItemDefault);
            }
        }
    }

}