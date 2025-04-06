using Library.SystemModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;


namespace Library.Network.ServerPackets   //网络服务器数据包库
{
    public sealed class NewAccount : Packet  //新账号
    {
        public NewAccountResult Result { get; set; }
    }

    public sealed class NewCharacter : Packet  //新建角色
    {
        public NewCharacterResult Result { get; set; }

        public SelectInfo Character { get; set; }
    }

    public sealed class DeleteCharacter : Packet   //删除角色
    {
        public DeleteCharacterResult Result { get; set; }

        public int DeletedIndex { get; set; }
    }
    public sealed class ClientNameChanged : Packet   //客户端名字
    {
        public string ClientName { get; set; }
        public bool PhysicalResistanceSwitch { get; set; }

        //古墓任务序号发给客户端
        public int PenetraliumKeyA { get; set; }
        public int PenetraliumKeyB { get; set; }
        public int PenetraliumKeyC { get; set; }

        //沙巴克旗帜附近是否允许使用位移技能
        public bool AllowTeleportMagicNearFlag { get; set; }
        public int TeleportMagicRadiusRange { get; set; }
    }

    public sealed class Login : Packet  //登录
    {
        public LoginResult Result { get; set; }

        public List<SelectInfo> Characters { get; set; }
        public List<ClientUserItem> Items { get; set; }

        public List<ClientBlockInfo> BlockList { get; set; }

        public string Message { get; set; }
        public TimeSpan Duration { get; set; }

        public string Address { get; set; }

        public bool TestServer { get; set; }
        public int PlayCount { set; get; }
    }

    public sealed class ChangePassword : Packet  //修改密码
    {
        public ChangePasswordResult Result { get; set; }

        public string Message { get; set; }
        public TimeSpan Duration { get; set; }
    }

    public sealed class RequestPasswordReset : Packet  //请求密码重置
    {
        public RequestPasswordResetResult Result { get; set; }
        public string Message { get; set; }
        public TimeSpan Duration { get; set; }
    }
    public sealed class ResetPassword : Packet  //重置密码
    {
        public ResetPasswordResult Result { get; set; }
    }
    public sealed class Activation : Packet  //激活
    {
        public ActivationResult Result { get; set; }
    }
    public sealed class RequestActivationKey : Packet  //请求激活密匙
    {
        public RequestActivationKeyResult Result { get; set; }
        public TimeSpan Duration { get; set; }
    }

    public sealed class SelectLogout : Packet  //选择注销
    {
    }

    public sealed class GameLogout : Packet  //游戏注销
    {
        public List<SelectInfo> Characters { get; set; }
    }


    public sealed class RequestStartGame : Packet   //开启游戏
    {
        public ClientControl ClientControl { get; set; }
        public int CharacterIndex { get; set; }

        public StartGameResult Result { get; set; }
        public string Message { get; set; }
        public TimeSpan Duration { get; set; }
    }
    public sealed class StartGame : Packet   //开启游戏
    {
        public bool ShortcutEnabled { get; set; }//是否开启菜单功能栏

        public StartInformation StartInformation { get; set; }

        public StartGameResult Result { get; set; }

        public string Message { get; set; }
        public TimeSpan Duration { get; set; }
    }

    public sealed class ShowItemSource : Packet //客户端是否显示物品来源
    {
        public bool DisplayItemSource { get; set; }
        public bool DisplayGMSource { get; set; }
        public bool DisplayItemSuitInfo { get; set; }  //客户端是否显示物品来源
    }

    public sealed class MapChanged : Packet  //地图改变
    {
        public int MapIndex { get; set; }
    }

    public sealed class MapTime : Packet
    {
        public bool OnOff { get; set; }
        public TimeSpan MapRemaining { get; set; }
        public bool ExpiryOnff { get; set; }
        public TimeSpan ExpiryRemaining { get; set; }
    }
    public sealed class UserLocation : Packet  //角色坐标
    {
        public MirDirection Direction { get; set; }
        public Point Location { get; set; }
    }
    public sealed class ObjectRemove : Packet   //目标删除
    {
        public uint ObjectID { get; set; }
    }

    public sealed class ObjectTurn : Packet  //目标移动
    {
        public uint ObjectID { get; set; }
        public MirDirection Direction { get; set; }
        public Point Location { get; set; }
        public TimeSpan Slow { get; set; }
    }
    public sealed class ObjectHarvest : Packet  //目标收获
    {
        public uint ObjectID { get; set; }
        public MirDirection Direction { get; set; }
        public Point Location { get; set; }
        public TimeSpan Slow { get; set; }
    }

    public sealed class ObjectMount : Packet  //目标装载
    {
        public uint ObjectID { get; set; }
        public HorseType Horse { get; set; }

        public HorseType HorseType { get; set; }
    }



    public sealed class WarWeapLocation : Packet
    {
        public uint ObjectID { get; set; }
        public Point Location { get; set; }
    }
    public sealed class ObjectMove : Packet   //目标移动
    {
        public uint ObjectID { get; set; }
        public MirDirection Direction { get; set; }
        public Point Location { get; set; }
        public int Distance { get; set; }
        public TimeSpan Slow { get; set; }
    }
    public sealed class ObjectDash : Packet   //目标冲撞
    {
        public uint ObjectID { get; set; }
        public MirDirection Direction { get; set; }
        public Point Location { get; set; }
        public int Distance { get; set; }
        public MagicType Magic { get; set; }
    }

    public sealed class ObjectPushed : Packet   //目标推动
    {
        public uint ObjectID { get; set; }
        public MirDirection Direction { get; set; }
        public Point Location { get; set; }
    }

    public sealed class ObjectAttack : Packet  //目标攻击
    {
        public uint ObjectID { get; set; }

        public MirDirection Direction { get; set; }
        public Point Location { get; set; }

        public MagicType AttackMagic { get; set; }
        public Element AttackElement { get; set; }

        public uint TargetID { get; set; }

        public TimeSpan Slow { get; set; }
    }
    public sealed class ObjectRangeAttack : Packet   //目标距离攻击
    {
        public uint ObjectID { get; set; }

        public MirDirection Direction { get; set; }
        public Point Location { get; set; }

        public MagicType AttackMagic { get; set; }
        public Element AttackElement { get; set; }

        public List<uint> Targets { get; set; } = new List<uint>();
    }
    public sealed class ObjectMagic : Packet    //目标技能
    {
        public uint ObjectID { get; set; }

        public MirDirection Direction { get; set; }
        public Point CurrentLocation { get; set; }

        public MagicType Type { get; set; }
        public List<uint> Targets { get; set; } = new List<uint>();
        public List<Point> Locations { get; set; } = new List<Point>();
        public bool Cast { get; set; }
        public Element AttackElement { get; set; }
        public TimeSpan Slow { get; set; }
    }
    public sealed class ObjectMining : Packet   //目标挖矿
    {
        public uint ObjectID { get; set; }

        public MirDirection Direction { get; set; }
        public Point Location { get; set; }

        public TimeSpan Slow { get; set; }
        public bool Effect { get; set; }
    }

    public sealed class ObjectPetOwnerChanged : Packet  //目标宠物主人改变
    {
        public uint ObjectID { get; set; }
        public string PetOwner { get; set; }
    }

    public sealed class ObjectShow : Packet   //目标显示
    {
        public uint ObjectID { get; set; }

        public MirDirection Direction { get; set; }
        public Point Location { get; set; }
    }
    public sealed class ObjectHide : Packet   //目标隐藏
    {
        public uint ObjectID { get; set; }

        public MirDirection Direction { get; set; }
        public Point Location { get; set; }
    }
    public sealed class ObjectEffect : Packet   //目标效果
    {
        public uint ObjectID { get; set; }

        public Effect Effect { get; set; }

        public bool Loop { get; set; } = false;
    }
    public sealed class MapEffect : Packet   //地图效果
    {
        public Point Location { get; set; }
        public Effect Effect { get; set; }
        public MirDirection Direction { get; set; }
    }

    public sealed class ObjectBuffAdd : Packet   //目标BUFF增加
    {
        public uint ObjectID { get; set; }
        public BuffType Type { get; set; }
        public int Index { get; set; }
    }
    public sealed class ObjectBuffRemove : Packet  //目标BUFF移除
    {
        public uint ObjectID { get; set; }
        public BuffType Type { get; set; }
        public int Index { get; set; }
    }
    public sealed class ObjectPoison : Packet    //目标中毒效果
    {
        public uint ObjectID { get; set; }
        public PoisonType Poison { get; set; }
    }
    public sealed class ObjectPlayer : Packet   //目标玩家
    {
        public int Index { get; set; }

        public uint ObjectID { get; set; }         //目标ID
        public string Name { get; set; }           //名字
        public Color NameColour { get; set; }      //名字颜色
        public string GuildName { get; set; }      //行会名字

        public MirDirection Direction { get; set; }   //方向
        public Point Location { get; set; }     //坐标

        public MirClass Class { get; set; }        //职业
        public MirGender Gender { get; set; }      //性别

        public int HairType { get; set; }         //头发类型
        public Color HairColour { get; set; }     //头发颜色
        public int Weapon { get; set; }  //武器
        public int Shield { get; set; }  //盾牌
        public int Armour { get; set; }   //衣服
        public int Emblem { get; set; }   //徽章
        public Color ArmourColour { get; set; }    //衣服颜色
        public int ArmourImage { get; set; }   //衣服图像
        public int WeaponImage { get; set; }  //武器图像

        public int ArmourIndex { get; set; }   //衣服index
        public int WeaponIndex { get; set; }  //武器index

        public int Light { get; set; }    //光效
        public bool Dead { get; set; }    //死亡
        public PoisonType Poison { get; set; }   //毒

        public List<BuffType> Buffs { get; set; }   //BUFF类型

        public List<int> CustomIndexs { get; set; } //自定义BUFF序号

        public HorseType Horse { get; set; }  //坐骑

        public HorseType HorseType { get; set; }

        public int Helmet { get; set; }   //头盔

        public int Fashion { get; set; }  //时装

        public int FashionImage { get; set; }  //时装图像

        public int HorseShape { get; set; }   //坐骑外形

        public int CraftLevel { get; set; }  //制造等级
        public int CraftExp { get; set; }  // 制造熟练度
        public DateTime CraftFinishTime { get; set; }  //制造完成时间
        public CraftItemInfo BookmarkedCraftItemInfo { get; set; }  //快捷列表物品

        public string AchievementTitle { get; set; }  //佩戴的成就
    }
    public sealed class ObjectMonster : Packet         //目标怪物
    {
        public uint ObjectID { get; set; }
        public int MonsterIndex { get; set; }      //序号
        public Color NameColour { get; set; }      //名字颜色
        public string PetOwner { get; set; }       //宠物主人

        public MirDirection Direction { get; set; }  //方向
        public Point Location { get; set; }       //坐标

        public bool Dead { get; set; }   //死亡
        public bool Skeleton { get; set; }   //骨架

        public PoisonType Poison { get; set; }  //毒

        public bool EasterEvent { get; set; }   //复活节活动
        public bool HalloweenEvent { get; set; }  //万圣节活动
        public bool ChristmasEvent { get; set; }  //圣诞节活动

        public List<BuffType> Buffs { get; set; }   //BUFF类型
        public bool Extra { get; set; }   //额外的

        public ClientCompanionObject CompanionObject { get; set; }   //宠物

        //public bool Extra { get; set; }
        //public int ExtraInt { get; set; }

    }
    public sealed class CustomNpc : Packet   //攻城NPC
    {
        public uint ObjectID { get; set; }
        public string Name { get; set; }
        public LibraryFile library { get; set; }
        public int ImagIndex { get; set; }
        public Color OverlayColor { get; set; }
        public Point CurrentLocation { get; set; }
        public MirDirection Direction { get; set; }
    }
    public sealed class ObjectNPC : Packet   //目标NPC
    {
        public uint ObjectID { get; set; }

        public int NPCIndex { get; set; }
        public Point CurrentLocation { get; set; }

        public MirDirection Direction { get; set; }
    }
    public sealed class ObjectItem : Packet   //目标道具
    {
        public uint ObjectID { get; set; }

        public ClientUserItem Item { get; set; }

        public Point Location { get; set; }
    }
    /// <summary>
    /// 包裹刷新
    /// </summary>
    public sealed class InventoryRefresh : Packet
    {
        public GridType GridType { get; set; }
        public List<ClientUserItem> Items { get; set; }

        public bool Success { get; set; }
    }

    public sealed class ObjectSpell : Packet   //目标施法
    {
        public uint ObjectID { get; set; }
        public MirDirection Direction { get; set; }
        public Point Location { get; set; }
        public SpellEffect Effect { get; set; }
        public int Power { get; set; }

    }
    public sealed class ObjectSpellChanged : Packet   //目标施法改变
    {
        public uint ObjectID { get; set; }
        public int Power { get; set; }
    }
    public sealed class ObjectNameColour : Packet   //目标名字颜色
    {
        public uint ObjectID { get; set; }
        public Color Colour { get; set; }
    }

    public sealed class PlayerUpdate : Packet   //玩家更新
    {
        public uint ObjectID { get; set; }
        /// <summary>
        /// 武器
        /// </summary>
        public int Weapon { get; set; }
        /// <summary>
        /// 盾牌
        /// </summary>
        public int Shield { get; set; }
        /// <summary>
        /// 徽章效果
        /// </summary>
        public int Emblem { get; set; }
        /// <summary>
        /// 衣服
        /// </summary>  
        public int Armour { get; set; }
        /// <summary>
        /// 衣服颜色
        /// </summary>
        public Color ArmourColour { get; set; }
        /// <summary>
        /// 衣服图像
        /// </summary>
        public int ArmourImage { get; set; }
        /// <summary>
        /// 武器图像
        /// </summary>
        public int WeaponImage { get; set; }
        /// <summary>
        /// 衣服 index
        /// </summary>
        public int ArmourIndex { get; set; }
        /// <summary>
        /// 武器 index
        /// </summary>
        public int WeaponIndex { get; set; }
        /// <summary>
        /// 马甲
        /// </summary>  
        public int HorseArmour { get; set; }
        /// <summary>
        /// 头盔
        /// </summary>
        public int Helmet { get; set; }
        /// <summary>
        /// 时装
        /// </summary>
        public int Fashion { get; set; }
        /// <summary>
        /// 时装图像 
        /// </summary>
        public int FashionImage { get; set; }
        /// <summary>
        /// 光效 
        /// </summary>
        public int Light { get; set; }
    }


    public sealed class MagicToggle : Packet   //技能开关
    {
        public MagicType Magic { get; set; }
        public bool CanUse { get; set; }
    }


    public sealed class DayChanged : Packet   //天改变
    {
        public float DayTime { get; set; }
    }


    public sealed class LevelChanged : Packet   //级别改变
    {
        public int Level { get; set; }
        public decimal Experience { get; set; }
    }
    public sealed class ObjectLeveled : Packet   //目标级别
    {
        public uint ObjectID { get; set; }
    }
    public sealed class ObjectWeaponLeveled : Packet
    {
        public uint ObjectID { get; set; }       //武器升级
    }

    public sealed class ObjectUseItem : Packet
    {
        public uint ObjectID { get; set; }       //使用道具
    }

    public sealed class ObjectRevive : Packet    //目标复活
    {
        public uint ObjectID { get; set; }
        public Point Location { get; set; }
        public bool Effect { get; set; }
    }
    public sealed class GainedExperience : Packet   //获得的经验
    {
        public decimal Amount { get; set; }       //普通经验
        public decimal WeapEx { get; set; }
        public decimal BonusEx { get; set; }
    }

    public sealed class NewMagic : Packet    //新技能
    {
        public ClientUserMagic Magic { get; set; }
    }
    public sealed class MagicLeveled : Packet   //技能等级
    {
        public int InfoIndex { get; set; }
        public MagicInfo Info;
        public int Level { get; set; }
        public long Experience { get; set; }

        [CompleteObject]
        public void Complete()
        {
            Info = Globals.MagicInfoList.Binding.FirstOrDefault(x => x.Index == InfoIndex);
        }
    }

    public sealed class MagicCooldown : Packet   //技能冷却
    {
        public int InfoIndex { get; set; }
        public int Delay { get; set; }
        public MagicInfo Info;

        [CompleteObject]
        public void Complete()
        {
            Info = Globals.MagicInfoList.Binding.FirstOrDefault(x => x.Index == InfoIndex);
        }
    }

    public sealed class StatsUpdate : Packet   //状态更新
    {
        public Stats Stats { get; set; }
        public Stats HermitStats { get; set; }
        public int HermitPoints { get; set; }
    }
    public sealed class HealthChanged : Packet    //身体状态更新
    {
        public uint ObjectID { get; set; }
        public int Change { get; set; }
        public bool Miss { get; set; }
        public bool Block { get; set; }
        public bool Critical { get; set; }
        public bool FatalAttack { get; set; }
        public bool CriticalHit { get; set; }
        public bool DamageAdd { get; set; }
        public bool GreenPosionPro { get; set; }
        public bool SmokingMP { get; set; }
    }
    public sealed class ObjectStats : Packet    //目标状态
    {
        public uint ObjectID { get; set; }
        public Stats Stats { get; set; }
    }

    public sealed class ManaChanged : Packet   //法力值变化
    {
        public uint ObjectID { get; set; }
        public int Change { get; set; }
    }

    public sealed class ObjectStruck : Packet   //目标被击中
    {
        public uint ObjectID { get; set; }
        public MirDirection Direction { get; set; }
        public Point Location { get; set; }
        public uint AttackerID { get; set; }
        public Element Element { get; set; }
    }
    public sealed class ObjectDied : Packet   //目标死亡
    {
        public uint ObjectID { get; set; }
        public MirDirection Direction { get; set; }
        public Point Location { get; set; }
    }
    public sealed class ObjectHarvested : Packet  //目标割肉
    {
        public uint ObjectID { get; set; }
        public MirDirection Direction { get; set; }
        public Point Location { get; set; }
    }

    /// <summary>
    /// 服务端传给客户端的聊天框道具信息
    /// </summary>
    public sealed class ChatItem : Packet
    {
        public ClientUserItem Item { get; set; }
    }
    /// <summary>
    /// 仓库刷新
    /// </summary>
    public sealed class StorageItemRefresh : Packet
    {
        public List<ClientUserItem> Items { get; set; }
    }
    /// <summary>
    /// 宠物包裹刷新
    /// </summary>
    public sealed class CompanionGridRefresh : Packet
    {
        public List<ClientUserItem> Items { get; set; }
    }

    public sealed class ItemsGained : Packet   //获得的道具
    {
        public List<ClientUserItem> Items { get; set; }
    }

    //通知客户端刷新一个itemcell
    public sealed class ItemCellRefresh : Packet
    {
        public ClientUserItem Item { get; set; }
        public GridType Grid { get; set; }
        //public int Slot { get; set; }
    }
    public sealed class ItemMove : Packet   //道具移动
    {
        public GridType FromGrid { get; set; }
        public GridType ToGrid { get; set; }
        public int FromSlot { get; set; }
        public int ToSlot { get; set; }
        public bool MergeItem { get; set; }

        public bool Success { get; set; }
    }

    public sealed class AmmAmmunition : Packet
    {
        public bool Success { get; set; }
        public int Count { get; set; }
    }

    public sealed class ItemSplit : Packet   //道具分开
    {
        public GridType Grid { get; set; }
        public int Slot { get; set; }
        public long Count { get; set; }
        public int NewSlot { get; set; }

        public bool Success { get; set; }
    }

    public sealed class ItemLock : Packet   //道具锁定
    {
        public GridType Grid { get; set; }
        public int Slot { get; set; }
        public bool Locked { get; set; }

    }

    public sealed class ItemUseDelay : Packet  //道具使用延迟
    {
        public TimeSpan Delay { get; set; }
    }
    public sealed class ItemChanged : Packet
    {
        public CellLinkInfo Link { get; set; }
        public bool Success { get; set; }
    }

    public sealed class ItemRefineChange : Packet
    {
        public int ErrorID { get; set; }
        public bool Refine { get; set; }
        public ClientUserItem Item { get; set; }

    }

    public sealed class ItemStatsChanged : Packet  //道具属性改变
    {
        public GridType GridType { get; set; }
        public int Slot { get; set; }
        public Stats NewStats { get; set; }
        public List<FullItemStat> FullItemStats { get; set; } = null;   //宝石镶嵌属性
        public bool Refine { get; set; }
    }

    public sealed class ItemStatsRefreshed : Packet   //道具属性刷新
    {
        public GridType GridType { get; set; }
        public int Slot { get; set; }
        public Stats NewStats { get; set; }

        public List<FullItemStat> FullItemStats { get; set; } = null;   //宝石镶嵌属性
    }

    public sealed class ItemDurability : Packet   //道具持久
    {
        public GridType GridType { get; set; }
        public int Slot { get; set; }
        public int CurrentDurability { get; set; }
    }

    public sealed class GoldChanged : Packet   //金币改变
    {
        public long Gold { get; set; }
    }

    public sealed class ItemExperience : Packet   //道具经验
    {
        public CellLinkInfo Target { get; set; }
        public decimal Experience { get; set; }
        public int Level { get; set; }
        public UserItemFlags Flags { get; set; }
    }

    public sealed class Chat : Packet   //聊天
    {
        public uint ObjectID { get; set; }
        public string Text { get; set; }
        public MessageType Type { get; set; }
        /// <summary>
        /// npc素材索引
        /// </summary>
        public int NpcFace { get; set; }
        //public List<ClientUserItem> Items { get; set; }
    }
    public sealed class NPCBuyBack : Packet
    {

        /// <summary>
        /// 当前页
        /// </summary>
        public int PageIndex { get; set; }
        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPage { get; set; }
        /// <summary>
        /// 回购商品分类
        /// </summary>
        public List<ClientNPCGood> BackGoods { get; set; }

        [CompleteObject]
        public void Complete()
        {
            if (BackGoods != null)
            {
                for (var i = 0; i < BackGoods.Count; i++)
                {
                    BackGoods[i].Item = Globals.ItemInfoList.Binding.First(x => x.ItemName == BackGoods[i].ItemName);
                    BackGoods[i].Cost = (int)Math.Round(BackGoods[i].Item.Price * BackGoods[i].Rate);
                }
            }
        }
    }

    public sealed class NPCBuyBackSeach : Packet
    {
        /// <summary>
        /// 当前页
        /// </summary>
        public int PageIndex { get; set; }
        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPage { get; set; }
        /// <summary>
        /// 商品列表
        /// </summary>
        public List<ClientNPCGood> BackGoods { get; set; }

        [CompleteObject]
        public void Complete()
        {
            if (BackGoods != null)
            {
                for (var i = 0; i < BackGoods.Count; i++)
                {
                    BackGoods[i].Item = Globals.ItemInfoList.Binding.First(x => x.ItemName == BackGoods[i].ItemName);
                    BackGoods[i].Cost = (int)Math.Round(BackGoods[i].Item.Price * BackGoods[i].Rate);
                }
            }
        }
    }
    public sealed class NPCResponse : Packet  //NPC响应
    {
        public uint ObjectID { get; set; }
        public int Index { get; set; }
        public List<ClientRefineInfo> Extra { get; set; }

        public ClientNPCPage NpcPage { get; set; }

        public List<string> Page { get; set; }

        /// <summary>
        /// 商品当前页
        /// </summary>
        public int CurrentPageIndex { get; set; }
        /// <summary>
        /// 商品总页数
        /// </summary>
        public int TotalPageIndex { get; set; }
        /// <summary>
        /// 商品
        /// </summary>
        public List<ClientNPCGood> Goods { get; set; }
        public NPCDialogType NPCDialogType { get; set; }

        [CompleteObject]
        public void Complete()
        {
            if (Goods != null)
            {
                for (var i = 0; i < Goods.Count; i++)
                {
                    Goods[i].Item = Globals.ItemInfoList.Binding.First(x => x.ItemName == Goods[i].ItemName);
                    Goods[i].Cost = (int)Math.Round(Goods[i].Item.Price * Goods[i].Rate);
                }
            }

        }
    }


    public sealed class DKey : Packet           //D键菜单
    {
        public List<DKeyPage> Options { get; set; }

    }

    public sealed class ItemsChanged : Packet   //道具改变
    {
        public List<CellLinkInfo> Links { get; set; }
        public bool Success { get; set; }
    }
    public sealed class NPCRepair : Packet  //NPC修理
    {
        public List<CellLinkInfo> Links { get; set; }
        public bool Special { get; set; }
        public bool Success { get; set; }
        public TimeSpan SpecialRepairDelay { get; set; }
    }
    public sealed class NPCSpecialRepair : Packet  //NPC特修
    {
        public List<CellLinkInfo> Links { get; set; }
        public bool Special { get; set; }
        public bool Success { get; set; }
        public TimeSpan SpecialRepairDelay { get; set; }
    }
    public sealed class NPCRefinementStone : Packet  //NPC精炼石
    {
        public List<CellLinkInfo> IronOres { get; set; }
        public List<CellLinkInfo> SilverOres { get; set; }
        public List<CellLinkInfo> DiamondOres { get; set; }
        public List<CellLinkInfo> GoldOres { get; set; }
        public List<CellLinkInfo> Crystal { get; set; }
    }
    public sealed class NPCRefine : Packet  //NPC精炼
    {
        public RefineType RefineType { get; set; }
        public RefineQuality RefineQuality { get; set; }
        public List<CellLinkInfo> Ores { get; set; }
        public List<CellLinkInfo> Items { get; set; }
        public List<CellLinkInfo> Specials { get; set; }
        public bool Success { get; set; }
    }
    public sealed class NPCMasterRefine : Packet  //NPC大师精炼
    {
        public List<CellLinkInfo> Fragment1s { get; set; }
        public List<CellLinkInfo> Fragment2s { get; set; }
        public List<CellLinkInfo> Fragment3s { get; set; }
        public List<CellLinkInfo> Stones { get; set; }
        public List<CellLinkInfo> Specials { get; set; }

        public bool Success { get; set; }
    }
    public sealed class NPCClose : Packet  //NPC关闭
    {
    }

    public sealed class NPCAccessoryLevelUp : Packet  //NPC配件等级升级
    {
        public CellLinkInfo Target { get; set; }
        public List<CellLinkInfo> Links { get; set; }
    }

    public sealed class NPCAccessoryUpgrade : Packet   //NPC配件升级
    {
        public CellLinkInfo Target { get; set; }
        public RefineType RefineType { get; set; }
        public bool Success { get; set; }
    }


    public sealed class NPCRefineRetrieve : Packet  //NPC精炼检索
    {
        public int Index { get; set; }
        public bool IsNewWeaponUpgrade { get; set; } = false;
    }
    public sealed class RefineList : Packet  //精炼列表
    {
        public List<ClientRefineInfo> List { get; set; }
        public bool IsNewWeaponUpgrade { get; set; } = false;
    }

    public sealed class GroupSwitch : Packet  //组队开关
    {
        public bool Allow { get; set; }
    }
    public sealed class GroupMember : Packet   //组队成员
    {
        public uint ObjectID { get; set; }
        public string Name { get; set; }
    }
    public sealed class GroupRemove : Packet  //组队删除
    {
        public uint ObjectID { get; set; }
    }
    public sealed class GroupInvite : Packet  //组队邀请
    {
        public string Name { get; set; }
    }

    public sealed class BuffAdd : Packet   //BUFF增加
    {
        public ClientBuffInfo Buff { get; set; }
    }
    public sealed class BuffRemove : Packet   //BUFF移除
    {
        public int Index { get; set; }
    }
    public sealed class BuffChanged : Packet   //BUFF改变
    {
        public int Index { get; set; }
        public Stats Stats { get; set; }
        public TimeSpan RemainingTime { get; set; }
    }
    public sealed class BuffTime : Packet   //BUFF时效
    {
        public int Index { get; set; }
        public TimeSpan Time { get; set; }
    }
    public sealed class BuffPaused : Packet  //BUFF暂停
    {
        public int Index { get; set; }
        public bool Paused { get; set; }
    }
    public sealed class SafeZoneChanged : Packet   //安全区改变
    {
        public bool InSafeZone { get; set; }
    }
    public sealed class CombatTime : Packet   //战斗时间
    {

    }
    public sealed class Inspect : Packet   //观察员
    {
        public string Name { get; set; }    //名字
        public string GuildName { get; set; }   //行会名字
        public int GuildFlag { get; set; }  //行会旗帜
        public Color GuildFlagColor { get; set; } //行会旗帜颜色
        public string GuildRank { get; set; }   //行会等级
        public string Partner { get; set; }  //配偶
        public MirClass Class { get; set; }   //职业
        public int Level { get; set; }   //等级
        public MirGender Gender { get; set; }   //性别
        public Stats Stats { get; set; }   //状态
        public Stats HermitStats { get; set; }   //额外加点状态
        public int HermitPoints { get; set; }   //额外点数
        public List<ClientUserItem> Items { get; set; }  //道具
        public int Hair { get; set; }  //发型
        public Color HairColour { get; set; }   //发型颜色

        public int WearWeight { get; set; }  //穿戴负重
        public int HandWeight { get; set; }  //手负重
        public bool HideHelmet { get; set; } //隐藏头盔
        public bool HideFashion { get; set; } //隐藏时装
        public bool HideShield { get; set; } //隐藏盾牌
    }
    public sealed class Rankings : Packet   //排行版
    {
        public bool OnlineOnly { get; set; }
        public RequiredClass Class { get; set; }
        public int StartIndex { get; set; }
        public int Total { get; set; }

        public List<RankInfo> Ranks { get; set; }
    }
    /// <summary>
    /// 启动观察者
    /// </summary>
    public sealed class StartObserver : Packet
    {
        /// <summary>
        /// 客户端控件
        /// </summary>
        public ClientControl ClientControl { get; set; }
        /// <summary>
        /// 开始信息
        /// </summary>
        public StartInformation StartInformation { get; set; }
        public List<ClientUserItem> Items { get; set; }
    }
    public sealed class ObservableSwitch : Packet  //可观察开关
    {
        public bool Allow { get; set; }
    }

    public sealed class MarketPlaceHistory : Packet  //商城历史记录
    {
        public int Index { get; set; }
        public long SaleCount { get; set; }
        public long LastPrice { get; set; }
        public long AveragePrice { get; set; }
        public long GameGoldLastPrice { get; set; }
        public long GameGoldAveragePrice { get; set; }
        public int Display { get; set; }
    }

    public sealed class MarketPlaceConsign : Packet  //商城寄售委托
    {
        public List<ClientMarketPlaceInfo> Consignments { get; set; }
    }

    public sealed class MarketPlaceSearch : Packet  //商城物品搜索
    {
        public int Count { get; set; }
        public List<ClientMarketPlaceInfo> Results { get; set; }
    }
    public sealed class MarketPlaceSearchCount : Packet  //商城搜索计数
    {
        public int Count { get; set; }
    }

    public sealed class MarketPlaceSearchIndex : Packet  //商城搜索索引
    {
        public int Index { get; set; }
        public ClientMarketPlaceInfo Result { get; set; }
    }

    public sealed class MarketPlaceBuy : Packet  //商城购买
    {
        public int Index { get; set; }
        public long Count { get; set; }
        public bool Success { get; set; }
    }
    public sealed class MarketPlaceStoreBuy : Packet  //商城商店购买
    {
    }

    public sealed class MarketPlaceConsignChanged : Packet  //商城寄售更改
    {
        public int Index { get; set; }
        public long Count { get; set; }
    }

    public sealed class GoldMarketMyOrderList : Packet
    {
        public List<ClientGoldMarketMyOrderInfo> MyOrder { get; set; }
    }
    public sealed class MailList : Packet  //邮件列表
    {
        public List<ClientMailInfo> Mail { get; set; }
    }
    public sealed class MailNew : Packet  //新邮件
    {
        public ClientMailInfo Mail { get; set; }
    }
    public sealed class MailDelete : Packet  //邮件删除
    {
        public int Index { get; set; }
    }
    public sealed class MailItemDelete : Packet   //邮件道具删除
    {
        public int Index { get; set; }
        public int Slot { get; set; }
    }
    public sealed class MailSend : Packet   //邮件发送
    {
    }

    public sealed class FriendSwitch : Packet  //好友部分
    {
        public bool Allow { get; set; }
    }

    public sealed class FriendInvite : Packet
    {
        public string Name { get; set; }
    }

    public sealed class FriendList : Packet
    {
        public List<ClientFriendInfo> Friend { get; set; }
    }

    public sealed class GoldMarketList : Packet
    {
        public List<ClientGoldMarketInfo> SellList { get; set; }
        public List<ClientGoldMarketInfo> BuyList { get; set; }
    }

    public sealed class FriendListRefresh : Packet
    {
        public List<ClientFriendInfo> Friend { get; set; }
    }

    public sealed class FriendNew : Packet
    {
        public ClientFriendInfo Friend { get; set; }
    }

    public sealed class FriendDelete : Packet
    {
        public ClientFriendInfo Friend { get; set; }
        // 是否为发起者
        public bool isRequester { get; set; }
    }

    public sealed class FriendDeleteRequest : Packet
    {
        public ClientFriendInfo Friend { get; set; }
    }


    public sealed class ChangeAttackMode : Packet  //攻击模式
    {
        public AttackMode Mode { get; set; }
    }
    public sealed class ChangePetMode : Packet   //宠物模式
    {
        public PetMode Mode { get; set; }
    }

    public sealed class GameGoldChanged : Packet  //元宝改变
    {
        public int GameGold { get; set; }
    }

    public sealed class PrestigeChanged : Packet  //声望改变
    {
        public int Prestige { get; set; }
    }

    public sealed class ContributeChanged : Packet  //贡献改变
    {
        public int Contribute { get; set; }
    }

    public sealed class MountFailed : Packet  //坐骑加载失败
    {
        public HorseType Horse { get; set; }
    }

    public sealed class WeightUpdate : Packet  //重量更新
    {
        public int BagWeight { get; set; }
        public int WearWeight { get; set; }
        public int HandWeight { get; set; }
    }

    public sealed class HuntGoldChanged : Packet   //赏金改变
    {
        public int HuntGold { get; set; }
    }
    public sealed class AutoTimeChanged : Packet  //自动时间更改
    {
        public long AutoTime { get; set; }
    }
    public sealed class TreasureChest : Packet  //传奇宝箱
    {
        public int Count { get; set; }
        public int Cost { get; set; }
        public List<ClientUserItem> Items { get; set; }
    }
    public sealed class TreasureSel : Packet  //传奇宝箱选择
    {
        public int Count { get; set; }
        public int Slot { get; set; }
        public int Cost { get; set; }
        public ClientUserItem Item { get; set; }
    }
    public sealed class TradeRequest : Packet  //交易请求
    {
        public string Name { get; set; }
    }
    public sealed class TradeOpen : Packet  //交易打开
    {
        public string Name { get; set; }
    }

    public sealed class TradeClose : Packet   //交易完成
    {

    }

    public sealed class TradeAddItem : Packet  //交易增加道具
    {
        public CellLinkInfo Cell { get; set; }
        public bool Success { get; set; }
    }

    public sealed class TradeAddGold : Packet  //交易增加金币
    {
        public long Gold { get; set; }
    }

    public sealed class TradeItemAdded : Packet   //交易道具获得
    {
        public ClientUserItem Item { get; set; }
    }

    public sealed class TradeGoldAdded : Packet   //交易金币获得
    {
        public long Gold { get; set; }
    }
    public sealed class TradeUnlock : Packet   //交易解锁
    {

    }

    public sealed class GuildCreate : Packet  //行会创建
    {

    }
    public sealed class GuildInfo : Packet  //行会信息
    {
        public ClientGuildInfo Guild { get; set; }
    }
    public sealed class GuildNoticeChanged : Packet  //行会公告改变
    {
        public string Notice { get; set; }
    }

    public sealed class GuildVaultNoticeChanged : Packet  //行会金库公告改变
    {
        public string Notice { get; set; }
    }
    public sealed class GuildNewItem : Packet   //行会新道具存入
    {
        public int Slot { get; set; }
        public ClientUserItem Item { get; set; }
        //public int Count { get; set; }
    }
    public sealed class GuildGetItem : Packet   //行会道具取出
    {
        public GridType Grid { get; set; }
        public int Slot { get; set; }
        public ClientUserItem Item { get; set; }
    }
    public sealed class GuildUpdate : Packet   //行会升级
    {
        public long GuildExp { get; set; }
        public long ActiveCount { get; set; }

        public long DailyActiveCount { get; set; }
        public int MemberLimit { get; set; }
        public int StorageLimit { get; set; }

        public long GuildFunds { get; set; }
        public long DailyGrowth { get; set; }

        public int GuildLevel { get; set; }
        public int Tax { get; set; }

        public int Flag { get; set; }
        public Color FlagColor { get; set; }

        public long TotalContribution { get; set; }
        public long DailyContribution { get; set; }

        public string DefaultRank { get; set; }
        public GuildPermission DefaultPermission { get; set; }

        public List<ClientGuildMemberInfo> Members { get; set; }

        public List<ClientGuildFundRankInfo> FundRanks { get; set; }
    }

    /// <summary>
    /// 行会赞助币数量变化
    /// </summary>
    public sealed class UpdateGuildGameTotal : Packet
    {
        public long Amount { get; set; }
    }
    public sealed class GuildKick : Packet   //行会踢出
    {
        public int Index { get; set; }
    }
    public sealed class GuildTax : Packet   //行会税收
    {

    }
    public sealed class GuildIncreaseMember : Packet   //行会增加成员
    {

    }
    public sealed class GuildIncreaseStorage : Packet   //行会增加仓库容量
    {

    }
    public sealed class GuildInviteMember : Packet   //行会邀请成员
    {

    }
    public sealed class GuildInvite : Packet  //行会邀请
    {
        public string Name { get; set; }
        public string GuildName { get; set; }
    }
    public sealed class GuildStats : Packet   //行会状态
    {
        public int Index { get; set; }
        public Stats Stats { get; set; }

    }

    public sealed class GuildMemberOffline : Packet  //行会成员离线
    {
        public int Index { get; set; }
    }
    public sealed class GuildMemberOnline : Packet   //行会成员在线
    {
        public int Index { get; set; }

        public string Name { get; set; }
        public uint ObjectID { get; set; }
    }
    public sealed class GuildAllyOffline : Packet        //行会离线
    {
        public int Index { get; set; }
    }
    public sealed class GuildAllyOnline : Packet           //行会在线
    {
        public int Index { get; set; }
    }

    public sealed class GuildActiveCountChange : Packet   //自己的行会贡献变化
    {
        public long DailyActiveCount { get; set; }
        public long TotalActiveCount { get; set; }
    }
    public sealed class GuildMemberContribution : Packet  //行会成员贡献
    {
        public int Index { get; set; }

        public long Contribution { get; set; }

        public long ActiveCount { get; set; }

        public bool IsVoluntary { get; set; } = false;
    }
    public sealed class GuildDayReset : Packet  //行会天重置
    {

    }
    public sealed class GuildFundsChanged : Packet  //行会资金改变
    {
        public long Change { get; set; }
    }
    public sealed class GuildChanged : Packet   //行会改变
    {
        public uint ObjectID { get; set; }
        public string GuildName { get; set; }
        public string GuildRank { get; set; }
    }

    public sealed class GuildWarFinished : Packet  //行会站结束
    {
        public string GuildName { get; set; }
    }

    public sealed class GuildWar : Packet   //行会站
    {
        public bool Success { get; set; }
    }

    public sealed class GuildAlliance : Packet  //行会联盟
    {
        public bool Success { get; set; }
    }

    public sealed class GuildWarStarted : Packet  //行会站开启
    {
        public string GuildName { get; set; }
        public TimeSpan Duration { get; set; }
    }

    public sealed class GuildAllianceStarted : Packet  //行会联盟开始
    {
        public ClientGuildAllianceInfo AllianceInfo { get; set; }
    }

    public sealed class GuildAllianceEnded : Packet   //行会联盟结束
    {
        public string GuildName { get; set; }
    }

    public sealed class GuildConquestDate : Packet  //行会攻城日期
    {
        public int Index { get; set; }
        public TimeSpan WarTime { get; set; }

        public DateTime WarDate;

        [CompleteObject]
        public void Update()
        {
            if (WarTime == TimeSpan.MinValue)
                WarDate = DateTime.MinValue;
            else
                WarDate = Time.Now + WarTime;
        }
    }
    public sealed class GuildCastleInfo : Packet  //行会城堡信息
    {
        public int Index { get; set; }
        public string Owner { get; set; }
        public string Guild { get; set; }
    }

    public sealed class GuildConquestStarted : Packet  //行会攻城开启
    {
        public int Index { get; set; }
        public Point FlagLocation { get; set; }
    }

    public sealed class GuildConquestFinished : Packet  //行会攻城结束
    {
        public int Index { get; set; }
        public Point FlagLocation { get; set; }
    }

    public sealed class ReviveTimers : Packet  //复活计时
    {
        public TimeSpan ItemReviveTime { get; set; }
        public TimeSpan ReincarnationPillTime { get; set; }
    }

    public sealed class QuestChanged : Packet  //任务改变
    {
        public ClientUserQuest Quest { get; set; }
    }

    public sealed class QuestRemoved : Packet   //任务已删除
    {
        public ClientUserQuest Quest { get; set; }
        public int DailyQuestRemains { get; set; }
        public int RepeatableQuestRemains { get; set; }
    }

    public sealed class CompanionUnlock : Packet  //宠物解锁
    {
        public int Index { get; set; }
    }

    public sealed class CompanionAutoFeedUnlocked : Packet  //宠物自动粮仓
    {
        public int Index { get; set; }
        public bool AutoFeedUnlocked { get; set; }
    }

    public sealed class CompanionAdopt : Packet   //宠物领养
    {
        public ClientUserCompanion UserCompanion { get; set; }
    }
    public sealed class CompanionRetrieve : Packet   //宠物取回
    {
        public int Index { get; set; }
    }
    public sealed class CompanionStore : Packet  //宠物商店
    {
    }
    public sealed class CompanionWeightUpdate : Packet  //宠物重量更新
    {
        public int BagWeight { get; set; }
        public int MaxBagWeight { get; set; }
        public int InventorySize { get; set; }
    }
    public sealed class CompanionShapeUpdate : Packet  //宠物形状更新
    {
        public uint ObjectID { get; set; }
        public int HeadShape { get; set; }
        public int BackShape { get; set; }
    }
    public sealed class CompanionItemsGained : Packet  //宠物获得物品
    {
        public List<ClientUserItem> Items { get; set; }
    }
    public sealed class CompanionUpdate : Packet  //宠物更新
    {
        public int Level { get; set; }
        public int Experience { get; set; }
        public int Hunger { get; set; }
    }
    public sealed class CompanionSkillUpdate : Packet  //宠物技能更新
    {
        public Stats Level3 { get; set; }
        public Stats Level5 { get; set; }
        public Stats Level7 { get; set; }
        public Stats Level10 { get; set; }
        public Stats Level11 { get; set; }
        public Stats Level13 { get; set; }
        public Stats Level15 { get; set; }
    }


    public sealed class MarriageInvite : Packet   //结婚
    {
        public string Name { get; set; }
    }
    public sealed class MarriageInfo : Packet   //婚姻信息
    {
        public ClientPlayerInfo Partner { get; set; }
    }
    public sealed class MarriageRemoveRing : Packet  //拆下结婚戒指
    {

    }
    public sealed class MarriageMakeRing : Packet   //制作结婚戒指
    {

    }

    public sealed class MarriageOnlineChanged : Packet  //结婚在线状态改变
    {
        public uint ObjectID { get; set; }
    }

    public sealed class DataObjectRemove : Packet  //数据对象删除
    {
        public uint ObjectID { get; set; }
    }
    public sealed class DataObjectPlayer : Packet   //数据对象玩家
    {
        public uint ObjectID { get; set; }
        public int MapIndex { get; set; }
        public Point CurrentLocation { get; set; }

        public string Name { get; set; }

        public int Health { get; set; }
        public int Mana { get; set; }
        public bool Dead { get; set; }

        public int MaxHealth { get; set; }
        public int MaxMana { get; set; }
    }
    public sealed class DataObjectMonster : Packet   //数据对象怪物
    {
        public uint ObjectID { get; set; }

        public int MapIndex { get; set; }
        public Point CurrentLocation { get; set; }

        public MonsterInfo MonsterInfo;
        public int MonsterIndex { get; set; }
        public string PetOwner { get; set; }

        public int Health { get; set; }
        public Stats Stats { get; set; }
        public bool Dead { get; set; }

        [CompleteObject]
        public void OnComplete()
        {
            MonsterInfo = Globals.MonsterInfoList.Binding.First(x => x.Index == MonsterIndex);
        }
    }
    public sealed class DataObjectItem : Packet   //数据对象道具
    {
        public uint ObjectID { get; set; }

        public int MapIndex { get; set; }
        public Point CurrentLocation { get; set; }

        public ItemInfo ItemInfo;
        public int ItemIndex { get; set; }

        [CompleteObject]
        public void OnComplete()
        {
            ItemInfo = Globals.ItemInfoList.Binding.First(x => x.Index == ItemIndex);
        }
    }
    public sealed class DataObjectLocation : Packet   //数据对象坐标
    {
        public uint ObjectID { get; set; }
        public int MapIndex { get; set; }
        public Point CurrentLocation { get; set; }
    }
    public sealed class DataObjectHealthMana : Packet   //数据对象生命值和法力值
    {
        public uint ObjectID { get; set; }

        public int Health { get; set; }
        public int Mana { get; set; }
        public bool Dead { get; set; }
    }
    public sealed class DataObjectMaxHealthMana : Packet   //数据对象最大生命值
    {
        public uint ObjectID { get; set; }

        public int MaxHealth { get; set; }
        public int MaxMana { get; set; }
        public Stats Stats { get; set; }
    }
    public sealed class BlockAdd : Packet   //黑名单对象增加
    {
        public ClientBlockInfo Info { get; set; }
    }

    public sealed class BlockRemove : Packet  //黑名单对象移除
    {
        public int Index { get; set; }
    }

    /// <summary>
    /// 隐藏头盔
    /// </summary>
    public sealed class HelmetToggle : Packet
    {
        /// <summary>
        /// 隐藏头盔
        /// </summary>
        public bool HideHelmet { get; set; }
    }
    /// <summary>
    /// 隐藏盾牌
    /// </summary>
    public sealed class ShieldToggle : Packet
    {
        /// <summary>
        /// 隐藏盾牌
        /// </summary>
        public bool HideShield { get; set; }
    }
    /// <summary>
    /// 隐藏时装
    /// </summary>
    public sealed class FashionToggle : Packet
    {
        /// <summary>
        /// 隐藏时装
        /// </summary>
        public bool HideFashion { get; set; }
    }

    public sealed class StorageSize : Packet       //仓库容量
    {
        public int Size { get; set; }
    }

    public sealed class PatchGridSize : Packet    //碎片包裹容量
    {
        public int Size { get; set; }
    }

    public sealed class PlayerChangeUpdate : Packet  //玩家更改更新
    {

        public uint ObjectID { get; set; }
        public string Name { get; set; }
        public MirGender Gender { get; set; }
        public int HairType { get; set; }
        public Color HairColour { get; set; }
        public Color ArmourColour { get; set; }

    }

    public sealed class FortuneUpdate : Packet   //财富更新
    {
        public List<ClientFortuneInfo> Fortunes { get; set; }

    }
    public sealed class NPCWeaponCraft : Packet   //NPC武器工艺
    {
        public CellLinkInfo Template { get; set; }
        public CellLinkInfo Yellow { get; set; }
        public CellLinkInfo Blue { get; set; }
        public CellLinkInfo Red { get; set; }
        public CellLinkInfo Purple { get; set; }
        public CellLinkInfo Green { get; set; }
        public CellLinkInfo Grey { get; set; }

        public bool Success { get; set; }
    }

    public sealed class CraftStartFailed : Packet
    {
        public int TargetItemIndex { get; set; }
        public string Reason { get; set; }
    }

    public sealed class CraftAcknowledged : Packet
    {
        public int TargetItemIndex { get; set; }
        public DateTime CompleteTime { get; set; }
    }

    public sealed class CraftResult : Packet
    {
        public bool Succeed { get; set; }
        public string Reason { get; set; }
    }

    public sealed class CraftExpChanged : Packet
    {
        public int Exp { get; set; }
        public int Level { get; set; }
    }

    public sealed class AchievementProgressChanged : Packet   //成就进度变化
    {
        public List<ClientUserAchievement> Achievements { get; set; }
    }

    public sealed class AchievementTitleChanged : Packet  //成就名称变更
    {
        public uint ObjectID { get; set; }
        public string NewTitle { get; set; }
    }

    public sealed class sc_FixedPoint : Packet
    {
        public int Opt { get; set; } //0新增 1修改 2删除
        public ClientFixedPointInfo Info { get; set; }
    }

    public sealed class sc_FixedPointList : Packet
    {
        public int FixedPointTCount { get; set; } //传送符最大记录的格子数量
        public List<ClientFixedPointInfo> Info { get; set; }
    }
    public sealed class sc_FixedPointAdd : Packet
    {
        public int count { get; set; }
    }

    public sealed class ShowConfirmationBox : Packet
    {
        public string Msg { get; set; }
        public int MenuOption { get; set; }
    }

    public sealed class ShowInputBox : Packet
    {
        public string Msg { get; set; }
        public int MenuOption { get; set; }
    }

    public sealed class ShortcutsLoaded : Packet  //加载菜单栏
    {
        public List<int> Shortcuts { get; set; }
    }

    public sealed class FishingStarted : Packet
    {
        public DateTime StartTime { get; set; }
        public DateTime PerfectTime { get; set; }
        public DateTime EndTime { get; set; }
        public int FindingChance { get; set; }
    }

    public sealed class FishingEnded : Packet
    {

    }

    public sealed class UpdateNPCLook : Packet
    {
        public uint ObjectID { get; set; }
        public string NPCName { get; set; }
        public LibraryFile Library { get; set; }
        public int ImageIndex { get; set; }
        public Color OverlayColor { get; set; }

        public Color NameColor { get; set; }

        public int CharacterIndex { get; set; } = 0;
        public bool UpdateNPCIcon { get; set; } = false;
        public QuestIcon Icon { get; set; } = QuestIcon.None;
    }

    public sealed class ConquestWarFlagFightStarted : Packet //夺旗开始
    {
        public int MapIndex { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan DelayTime { get; set; }
    }

    public sealed class ConquestWarFlagFightEnd : Packet { } //夺旗失败

    public sealed class NPCWeaponUpgrade : Packet
    {
        public List<CellLinkInfo> items { get; set; }
    }

    public sealed class PlaySound : Packet
    {
        public SoundIndex Sound { get; set; }
    }

    public sealed class PyTextBox : Packet
    {
        public string Message { get; set; }
        public string ID { get; set; }
    }

    /*
    public sealed class UpdateAncientTombstone : Packet
    {
        public int FirstChar { get; set; } = -1;
        public int SecondChar { get; set; } = -1;
        public int ThirdChar { get; set; } = -1;
    }
    */
    public sealed class RemoveEffects : Packet
    {
        public uint ObjectID { get; set; }
    }

    public sealed class DiyObjectMagic : Packet
    {
        public uint ObjectID { get; set; }

        public MirDirection Direction { get; set; }
        public Point CurrentLocation { get; set; }

        public int SpellMagicID { get; set; }
        public Element AttackElement { get; set; }

        public List<uint> Targets { get; set; } = new List<uint>();
        public List<Point> Locations { get; set; } = new List<Point>();
        public bool Cast { get; set; }

        public MirAnimation ActID { get; set; }
        public TimeSpan Slow { get; set; }
    }

    public sealed class DiyObjectEffect : Packet
    {
        public uint ObjectID { get; set; }

        public MirDirection Direction { get; set; }
        public Point CurrentLocation { get; set; }

        public List<uint> Targets { get; set; } = new List<uint>();
        public List<Point> Locations { get; set; } = new List<Point>();
        public bool Cast { get; set; }

        public MirAnimation ActID { get; set; }
        public int EffectID { get; set; }
        public TimeSpan Slow { get; set; }
    }

    public sealed class DiyObjectAttack : Packet
    {
        public uint ObjectID { get; set; }
        public MirDirection Direction { get; set; }
        public Point Location { get; set; }
        public int SpellMagicID { get; set; }
        public Element AttackElement { get; set; }
        public uint TargetID { get; set; }
        public MirAnimation ActID { get; set; }
        public TimeSpan Slow { get; set; }
    }
    public sealed class GuildWithDrawal : Packet
    {
        public List<string> WithDrawals { get; set; }
    }
    public sealed class GuildApplications : Packet
    {
        public List<string> Applicants { get; set; }
    }

    public sealed class ObjectFishing : Packet
    {
        public uint ObjectID { get; set; }
        public MirDirection Direction { get; set; }
        public MirAction Action { get; set; }
        public Point Location { get; set; }
    }

    public sealed class TaishanBuffChanged : Packet
    {
        public int BuffIndex1 { get; set; }
        public int BuffIndex2 { get; set; }
        public int BuffIndex3 { get; set; }
        public int BuffIndex4 { get; set; }
        public int BuffIndex5 { get; set; }
        public int BuffIndex6 { get; set; }
    }

    public sealed class FreeCoinCountChanged : Packet
    {
        public int Count { get; set; }
    }

    public sealed class CoinTossOnTarget : Packet
    {
        public bool IsOnTarget { get; set; }
    }

    //神舰 赤龙 开关门信息
    public sealed class GateInformation : Packet
    {
        public DateTime NetherworldCloseTime { get; set; }
        public DateTime LairCloseTime { get; set; }
        public Point NetherworldLocation { get; set; }
        public Point LairLocation { get; set; }
    }

    public sealed class AccessoryCombineResult : Packet
    {

    }

    // 特定地图技能限制
    public sealed class MapMagicRestriction : Packet
    {
        public string MapName { get; set; }
        public List<MagicType> Magics { get; set; }
        public bool ClearAll { get; set; } = false;
    }

    /// <summary>
    /// 奖金池更新
    /// </summary>
    public sealed class RewardPoolUpdate : Packet
    {
        public ClientRewardPoolInfo RewardPoolInfo { get; set; }
    }

    /// <summary>
    /// 个人的彩池币改变
    /// </summary>
    public sealed class RewardPoolCoinChanged : Packet
    {
        public decimal RewardPoolCoin { get; set; }
    }
    /// <summary>
    /// 奖池币排行
    /// </summary>
    public sealed class RewardPoolCoinRankChanged : Packet
    {
        public ClientRewardPoolRanks First { get; set; }
        public ClientRewardPoolRanks Second { get; set; }
        public ClientRewardPoolRanks Third { get; set; }
        public ClientRewardPoolRanks Myself { get; set; }

    }

    /// <summary>
    /// 单个红包的信息
    /// </summary>
    public sealed class RedPacketUpdate : Packet
    {
        public ClientRedPacketInfo RedPacket { get; set; }
    }

    /// <summary>
    /// 最近红包列表（24小时内）
    /// </summary>
    public sealed class RecentRedPackets : Packet
    {
        public List<ClientRedPacketInfo> RedPacketList { get; set; }
    }

    public sealed class HuiShengToggle : Packet
    {
        public bool HuiSheng { get; set; }
    }

    public sealed class ReCallToggle : Packet
    {
        public bool Recall { get; set; }
    }

    public sealed class TradeToggle : Packet
    {
        public bool Trade { get; set; }
    }

    public sealed class GuildToggle : Packet
    {
        public bool Guild { get; set; }
    }

    public sealed class FriendToggle : Packet
    {
        public bool Friend { get; set; }
    }

    public sealed class NewAuctionFlash : Packet
    {
        public List<ClientNewAuction> NewAuctionsList { get; set; }
    }

    public sealed class NewAuctionFlashIndex : Packet  //商城搜索索引
    {
        public int Index { get; set; }
        public ClientNewAuction Result { get; set; }
    }

    public sealed class ChangeWeather : Packet
    {
        public int MapIndex { get; set; }
        public WeatherSetting Weather { get; set; }
    }

    public sealed class RequestProcessHash : Packet
    {

    }

    public sealed class ReFlashSellCharState : Packet
    {
        public int CharacterIndex { get; set; }
    }

    public sealed class SellCharacterSearch : Packet
    {

        public int Count { get; set; }
        public List<ClientAccountConsignmentInfo> Results { get; set; }
    }

    public sealed class Equipment : Packet
    {
        public string Name { get; set; }

        public string GuildName { get; set; }

        public int GuildFlag { get; set; }

        public Color GuildFlagColor { get; set; }

        public string GuildRank { get; set; }

        public string Partner { get; set; }

        public MirClass Class { get; set; }

        public int Level { get; set; }

        public MirGender Gender { get; set; }

        public Stats Stats { get; set; }

        public Stats HermitStats { get; set; }

        public int HermitPoints { get; set; }

        public List<ClientUserItem> Items { get; set; }

        public int Hair { get; set; }

        public Color HairColour { get; set; }

        public int WearWeight { get; set; }

        public int HandWeight { get; set; }

        public bool HideHelmet { get; set; }

        public bool HideFashion { get; set; }

        public bool HideShield { get; set; }
    }

    public sealed class InspectMagery : Packet
    {
        public string Name { get; set; }
        public List<ClientUserMagic> Items { get; set; }
    }

    public sealed class InspectPackSack : Packet
    {
        public string Name { get; set; }

        public List<ClientUserItem> Items { get; set; }
    }

    public sealed class MarketPlaceJiaoseBuy : Packet
    {
        public bool Success { get; set; }
    }
}

