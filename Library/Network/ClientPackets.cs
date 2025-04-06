using System;
using System.Collections.Generic;
using System.Drawing;


namespace Library.Network.ClientPackets  //网络客户端数据包库
{
    /// <summary>
    /// 新建账号
    /// </summary>
    public sealed class NewAccount : Packet
    {
        /// <summary>
        /// 推荐人
        /// </summary>
        public string Referral { get; set; }
        /// <summary>
        /// 安全码
        /// </summary>
        public string CheckSum { get; set; }
        /// <summary>
        /// 邮箱账号
        /// </summary>
        public string EMailAddress { get; set; }
        /// <summary>
        /// 邮箱密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 出生日期
        /// </summary>
        public DateTime BirthDate { get; set; }
        /// <summary>
        /// 真实名字
        /// </summary>
        public string RealName { get; set; }
        /// <summary>
        /// 邀请码
        /// </summary>
        public string InviteCode { get; set; }
    }
    /// <summary>
    /// 修改密码
    /// </summary>
    public sealed class ChangePassword : Packet
    {
        /// <summary>
        /// 邮箱账号
        /// </summary>
        public string EMailAddress { get; set; }
        /// <summary>
        /// 安全码
        /// </summary>
        public string CheckSum { get; set; }
        /// <summary>
        /// 当前密码
        /// </summary>
        public string CurrentPassword { get; set; }
        /// <summary>
        /// 新密码
        /// </summary>
        public string NewPassword { get; set; }
    }
    /// <summary>
    /// 激活
    /// </summary>
    public sealed class Activation : Packet
    {
        /// <summary>
        /// 激活KEY
        /// </summary>
        public string ActivationKey { get; set; }
        /// <summary>
        /// 安全码
        /// </summary>
        public string CheckSum { get; set; }
    }
    /// <summary>
    /// 请求激活密匙
    /// </summary>
    public sealed class RequestActivationKey : Packet
    {
        /// <summary>
        /// 邮箱账号
        /// </summary>
        public string EMailAddress { get; set; }
        /// <summary>
        /// 安全码
        /// </summary>
        public string CheckSum { get; set; }
    }
    /// <summary>
    /// 请求密码重置
    /// </summary>
    public sealed class RequestPasswordReset : Packet
    {
        /// <summary>
        /// 邮箱账号
        /// </summary>
        public string EMailAddress { get; set; }
        /// <summary>
        /// 安全码
        /// </summary>
        public string CheckSum { get; set; }
    }
    /// <summary>
    /// 重置密码
    /// </summary>
    public sealed class ResetPassword : Packet
    {
        /// <summary>
        /// 重置KEY
        /// </summary>
        public string ResetKey { get; set; }
        /// <summary>
        /// 新密码
        /// </summary>
        public string NewPassword { get; set; }
        /// <summary>
        /// 安全码
        /// </summary>
        public string CheckSum { get; set; }
    }

    /// <summary>
    /// 选择语言
    /// </summary>
    public sealed class SelectLanguage : Packet
    {
        /// <summary>
        /// 语种
        /// </summary>
        public Language Language { get; set; }
    }
    /// <summary>
    /// 登录
    /// </summary>
    public sealed class Login : Packet
    {
        /// <summary>
        /// 安全码
        /// </summary>
        public string CheckSum { get; set; }
        /// <summary>
        /// 邮箱账号
        /// </summary>
        public string EMailAddress { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
    }
    /// <summary>
    /// 退出
    /// </summary>
    public sealed class Logout : Packet { }
    /// <summary>
    /// 尝试开始游戏
    /// </summary>
    public sealed class RequestStartGame : Packet
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        public int CharacterIndex { get; set; }
        public string ClientMACInfo { get; set; }
        public string ClientCPUInfo { get; set; }
        public string ClientHDDInfo { get; set; }
    }
    /// <summary>
    /// 开始游戏
    /// </summary>
    public sealed class StartGame : Packet
    {
        public Platform Platform { get; set; }
        /// <summary>
        /// 角色ID
        /// </summary>
        public int CharacterIndex { get; set; }
        public string ClientMACInfo { get; set; }
        public string ClientCPUInfo { get; set; }
        public string ClientHDDInfo { get; set; }
    }
    /// <summary>
    /// 新建角色
    /// </summary>
    public sealed class NewCharacter : Packet
    {
        /// <summary>
        /// 新建角色名
        /// </summary>
        public string CharacterName { get; set; }
        /// <summary>
        /// 头发类型
        /// </summary>
        public int HairType { get; set; }
        /// <summary>
        /// 头发颜色
        /// </summary>
        public Color HairColour { get; set; }
        /// <summary>
        /// 衣服颜色
        /// </summary>
        public Color ArmourColour { get; set; }
        /// <summary>
        /// 安全码
        /// </summary>
        public string CheckSum { get; set; }
        /// <summary>
        /// 职业
        /// </summary>
        public MirClass Class { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public MirGender Gender { get; set; }
    }
    /// <summary>
    /// 删除角色
    /// </summary>
    public sealed class DeleteCharacter : Packet
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        public int CharacterIndex { get; set; }
        /// <summary>
        /// 安全码
        /// </summary>
        public string CheckSum { get; set; }
    }
    /// <summary>
    /// 旋转 转向
    /// </summary>
    public sealed class Turn : Packet
    {
        /// <summary>
        /// 方向
        /// </summary>
        public MirDirection Direction { get; set; }
    }
    /// <summary>
    /// 收割 割肉 挖
    /// </summary>
    public sealed class Harvest : Packet
    {
        /// <summary>
        /// 方向
        /// </summary>
        public MirDirection Direction { get; set; }
    }
    /// <summary>
    /// 城镇复活
    /// </summary>
    public sealed class TownRevive : Packet { }
    /// <summary>
    /// 移动
    /// </summary>
    public sealed class Move : Packet
    {
        /// <summary>
        /// 方向
        /// </summary>
        public MirDirection Direction { get; set; }
        /// <summary>
        /// 距离
        /// </summary>
        public int Distance { get; set; }
    }
    /// <summary>
    /// 上马
    /// </summary>
    public sealed class Mount : Packet { }
    /// <summary>
    /// 挖矿
    /// </summary>
    public sealed class Mining : Packet
    {
        /// <summary>
        /// 方向
        /// </summary>
        public MirDirection Direction { get; set; }
    }
    /// <summary>
    /// 攻击
    /// </summary>
    public sealed class Attack : Packet
    {
        /// <summary>
        /// 方向
        /// </summary>
        public MirDirection Direction { get; set; }
        /// <summary>
        /// 物理攻击
        /// </summary>
        public MirAction Action { get; set; }
        /// <summary>
        /// 魔法技能攻击类型
        /// </summary>
        public MagicType AttackMagic { get; set; }
    }
    /// <summary>
    /// 魔法技能
    /// </summary>
    public sealed class Magic : Packet
    {
        /// <summary>
        /// 方向
        /// </summary>
        public MirDirection Direction { get; set; }
        /// <summary>
        /// 攻击对象
        /// </summary>
        public MirAction Action { get; set; }
        /// <summary>
        /// 魔法技能类型
        /// </summary>
        public MagicType Type { get; set; }
        /// <summary>
        /// 目标
        /// </summary>
        public uint Target { get; set; }
        /// <summary>
        /// 坐标
        /// </summary>
        public Point Location { get; set; }
    }
    /// <summary>
    /// 道具移动
    /// </summary>
    public sealed class ItemMove : Packet
    {
        /// <summary>
        /// 格子类型 从格子里
        /// </summary>
        public GridType FromGrid { get; set; }
        /// <summary>
        /// 格子类型 到格子里
        /// </summary>
        public GridType ToGrid { get; set; }
        /// <summary>
        /// 从单元格
        /// </summary>
        public int FromSlot { get; set; }
        /// <summary>
        /// 到单元格
        /// </summary>
        public int ToSlot { get; set; }
        /// <summary>
        /// 合并道具
        /// </summary>
        public bool MergeItem { get; set; }
    }
    /// <summary>
    /// 包裹整理
    /// </summary>
    public sealed class InventoryTidy : Packet { }

    /// <summary>
    /// 包裹刷新
    /// </summary>
    public sealed class InventoryRefresh : Packet
    {
        /// <summary>
        /// 包裹类型
        /// </summary>
        public GridType GridType { get; set; }
    }
    /// <summary>
    /// 仓库刷新
    /// </summary>
    public sealed class StorageItemRefresh : Packet { }
    /// <summary>
    /// 宠物包裹刷新
    /// </summary>
    public sealed class CompanionGridRefresh : Packet { }
    /// <summary>
    /// 道具拆分
    /// </summary>
    public sealed class ItemSplit : Packet
    {
        /// <summary>
        /// 格子类型
        /// </summary>
        public GridType Grid { get; set; }
        /// <summary>
        /// 单元格
        /// </summary>
        public int Slot { get; set; }
        /// <summary>
        /// 计数
        /// </summary>
        public long Count { get; set; }
    }
    /// <summary>
    /// 道具爆出
    /// </summary>
    public sealed class ItemDrop : Packet
    {
        /// <summary>
        /// 关联信息
        /// </summary>
        public CellLinkInfo Link { get; set; }
    }
    /// <summary>
    /// 道具删除
    /// </summary>
    public sealed class ItemDelete : Packet
    {
        /// <summary>
        /// 关联信息
        /// </summary>
        public CellLinkInfo Link { get; set; }
    }
    /// <summary>
    /// 金币爆出
    /// </summary>
    public sealed class GoldDrop : Packet
    {
        /// <summary>
        /// 数量
        /// </summary>
        public long Amount { get; set; }
    }
    /// <summary>
    /// 角色道具
    /// </summary>
    public sealed class ItemUse : Packet
    {
        /// <summary>
        /// 关联信息
        /// </summary>
        public CellLinkInfo Link { get; set; }
    }
    /// <summary>
    /// 道具锁定
    /// </summary>
    public sealed class ItemLock : Packet
    {
        /// <summary>
        /// 格子类型
        /// </summary>
        public GridType GridType { get; set; }
        /// <summary>
        /// 单元格索引值
        /// </summary>
        public int SlotIndex { get; set; }
        /// <summary>
        /// 是否锁定
        /// </summary>
        public bool Locked { get; set; }
    }
    /// <summary>
    /// 道具药品快捷栏变化
    /// </summary>
    public sealed class BeltLinkChanged : Packet
    {
        /// <summary>
        /// 单元格
        /// </summary>
        public int Slot { get; set; }
        /// <summary>
        /// 关联索引值
        /// </summary>
        public int LinkIndex { get; set; }
        /// <summary>
        /// 关联道具ID
        /// </summary>
        public int LinkItemIndex { get; set; }
    }
    /// <summary>
    /// 自动喝药栏变化
    /// </summary>
    public sealed class AutoPotionLinkChanged : Packet
    {
        /// <summary>
        /// 单元
        /// </summary>
        public int Slot { get; set; }
        /// <summary>
        /// 关联索引值
        /// </summary>
        public int LinkIndex { get; set; }
        /// <summary>
        /// 加红值
        /// </summary>
        public int Health { get; set; }
        /// <summary>
        /// 加蓝值
        /// </summary>
        public int Mana { get; set; }
        /// <summary>
        /// 是否开启
        /// </summary>
        public bool Enabled { get; set; }
    }
    /// <summary>
    /// 自动战斗状态变化
    /// </summary>
    public sealed class AutoFightConfChanged : Packet
    {
        /// <summary>
        /// 自动设置配置栏
        /// </summary>
        public AutoSetConf Slot { get; set; }
        /// <summary>
        /// 魔法技能类型索引值
        /// </summary>
        public MagicType MagicIndex { get; set; }
        /// <summary>
        /// 时间计数
        /// </summary>
        public int TimeCount { get; set; }
        /// <summary>
        /// 是否开启
        /// </summary>
        public bool Enabled { get; set; }
    }
    /// <summary>
    /// 道具属性排序 普通 高级 稀世
    /// </summary>
    public sealed class ComSortingConf1Changed : Packet
    {
        /// <summary>
        /// 类型栏
        /// </summary>
        public Rarity Slot { get; set; }
        /// <summary>
        /// 是否开启
        /// </summary>
        public bool Enabled { get; set; }
    }
    /// <summary>
    /// 道具属性排序 装备 药品 材料等
    /// </summary>
    public sealed class ComSortingConfChanged : Packet
    {
        /// <summary>
        /// 道具类型栏
        /// </summary>
        public ItemType Slot { get; set; }
        /// <summary>
        /// 是否开启
        /// </summary>
        public bool Enabled { get; set; }
    }
    /// <summary>
    /// 自动捡取
    /// </summary>
    public sealed class PickUp : Packet
    {
        /// <summary>
        /// 道具索引值
        /// </summary>
        public int ItemIdx { get; set; }
        /// <summary>
        /// X坐标
        /// </summary>
        public int Xpos { get; set; }
        /// <summary>
        /// Y坐标
        /// </summary>
        public int Ypos { get; set; }
    }
    /// <summary>
    /// 聊天信息
    /// </summary>
    public sealed class Chat : Packet
    {
        /// <summary>
        /// 聊天文本
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// 聊天道具信息列表
        /// </summary>
        public List<ChatItemInfo> Links { get; set; }
    }
    /// <summary>
    /// NPC点击呼出
    /// </summary>
    public sealed class NPCCall : Packet
    {
        /// <summary>
        /// 对象索引值
        /// </summary>
        public uint ObjectID { get; set; }
        /// <summary>
        /// 是否D菜单
        /// </summary>
        public bool isDKey { get; set; } = false;
        /// <summary>
        /// 用于二级确认
        /// </summary>
        public bool Confirmation { get; set; } = false;

        /// <summary>
        /// txt脚本函数
        /// </summary>
        public string Key { get; set; }
    }
    /// <summary>
    /// D菜单
    /// </summary>
    public sealed class DKey : Packet { }
    /// <summary>
    /// NPC按键
    /// </summary>
    public sealed class NPCButton : Packet
    {
        /// <summary>
        /// 按钮索引值
        /// </summary>
        public int ButtonID { get; set; }
        /// <summary>
        /// 链接信息
        /// </summary>
        public List<CellLinkInfo> links { get; set; }
        /// <summary>
        /// 用户输入
        /// </summary>
        public string UserInput { get; set; }
    }
    /// <summary>
    /// NPC买
    /// </summary>
    public sealed class NPCBuy : Packet
    {
        /// <summary>
        /// 索引值
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public long Amount { get; set; }
        /// <summary>
        /// 行会资金
        /// </summary>
        public bool GuildFunds { get; set; }

        public bool IsBuyback { get; set; } = false;

        public int PageIndex { get; set; }
    }

    public sealed class NPCBuyBack : Packet
    {
        /// <summary>
        /// 当前页
        /// </summary>
        public int PageIndex { get; set; } = 1;
    }

    public sealed class NPCBuyBackSeach : Packet
    {
        /// <summary>
        /// 商品id
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 当前页数
        /// </summary>
        public int PageIndex { get; set; } = 1;
    }

    /// <summary>
    /// NPC卖
    /// </summary>
    public sealed class NPCSell : Packet
    {
        /// <summary>
        /// 链接信息
        /// </summary>
        public List<CellLinkInfo> Links { get; set; }
    }
    /// <summary>
    /// NPC一键出售
    /// </summary>
    public sealed class NPCRootSell : Packet
    {
        /// <summary>
        /// 链接信息
        /// </summary>
        public List<CellLinkInfo> Links { get; set; }
    }
    /// <summary>
    /// NPC碎片分解
    /// </summary>
    public sealed class NPCFragment : Packet
    {
        /// <summary>
        /// 链接信息
        /// </summary>
        public List<CellLinkInfo> Links { get; set; }
    }
    /// <summary>
    /// NPC修理
    /// </summary>
    public sealed class NPCRepair : Packet
    {
        /// <summary>
        /// 链接信息
        /// </summary>
        public List<CellLinkInfo> Links { get; set; }
        /// <summary>
        /// 特殊修理
        /// </summary>
        public bool Special { get; set; }
        /// <summary>
        /// 行会资金
        /// </summary>
        public bool GuildFunds { get; set; }
    }
    /// <summary>
    /// NPC特修
    /// </summary>
    public sealed class NPCSpecialRepair : Packet
    {
        /// <summary>
        /// 链接信息
        /// </summary>
        public List<CellLinkInfo> Links { get; set; }
        /// <summary>
        /// 特殊修理
        /// </summary>
        public bool Special { get; set; }
        /// <summary>
        /// 行会资金
        /// </summary>
        public bool GuildFunds { get; set; }
    }
    /// <summary>
    /// NPC精炼
    /// </summary>
    public sealed class NPCRefine : Packet
    {
        /// <summary>
        /// 精炼类型
        /// </summary>
        public RefineType RefineType { get; set; }
        /// <summary>
        /// 精炼时间
        /// </summary>
        public RefineQuality RefineQuality { get; set; }
        /// <summary>
        /// 需要的矿石材料
        /// </summary>
        public List<CellLinkInfo> Ores { get; set; }
        /// <summary>
        /// 需要的道具材料
        /// </summary>
        public List<CellLinkInfo> Items { get; set; }
        /// <summary>
        /// 需要的特殊材料
        /// </summary>
        public List<CellLinkInfo> Specials { get; set; }
    }
    /// <summary>
    /// NPC大师精炼
    /// </summary>
    public sealed class NPCMasterRefine : Packet
    {
        /// <summary>
        /// 精炼类型
        /// </summary>
        public RefineType RefineType { get; set; }
        /// <summary>
        /// 碎片1
        /// </summary>
        public List<CellLinkInfo> Fragment1s { get; set; }
        /// <summary>
        /// 碎片2
        /// </summary>
        public List<CellLinkInfo> Fragment2s { get; set; }
        /// <summary>
        /// 碎片3
        /// </summary>
        public List<CellLinkInfo> Fragment3s { get; set; }
        /// <summary>
        /// 精炼石
        /// </summary>
        public List<CellLinkInfo> Stones { get; set; }
        /// <summary>
        /// 需要的特殊材料
        /// </summary>
        public List<CellLinkInfo> Specials { get; set; }
    }
    /// <summary>
    /// NPC大师精炼评估
    /// </summary>
    public sealed class NPCMasterRefineEvaluate : Packet
    {
        /// <summary>
        /// 精炼类型
        /// </summary>
        public RefineType RefineType { get; set; }
        /// <summary>
        /// 碎片1
        /// </summary>
        public List<CellLinkInfo> Fragment1s { get; set; }
        /// <summary>
        /// 碎片2
        /// </summary>
        public List<CellLinkInfo> Fragment2s { get; set; }
        /// <summary>
        /// 碎片3
        /// </summary>
        public List<CellLinkInfo> Fragment3s { get; set; }
        /// <summary>
        /// 精炼石
        /// </summary>
        public List<CellLinkInfo> Stones { get; set; }
        /// <summary>
        /// 需要的特殊材料
        /// </summary>
        public List<CellLinkInfo> Specials { get; set; }
    }
    /// <summary>
    /// NPC精炼石
    /// </summary>
    public sealed class NPCRefinementStone : Packet
    {
        /// <summary>
        /// 铁矿
        /// </summary>
        public List<CellLinkInfo> IronOres { get; set; }
        /// <summary>
        /// 银矿
        /// </summary>
        public List<CellLinkInfo> SilverOres { get; set; }
        /// <summary>
        /// 金刚石
        /// </summary>
        public List<CellLinkInfo> DiamondOres { get; set; }
        /// <summary>
        /// 金矿
        /// </summary>
        public List<CellLinkInfo> GoldOres { get; set; }
        /// <summary>
        /// 结晶
        /// </summary>
        public List<CellLinkInfo> Crystal { get; set; }
        /// <summary>
        /// 金币
        /// </summary>
        public long Gold { get; set; }
    }
    /// <summary>
    /// NPC技能书合成
    /// </summary>
    public sealed class NPCBookRefine : Packet
    {
        /// <summary>
        /// 主技能书
        /// </summary>
        public CellLinkInfo OriginalBook { get; set; }
        /// <summary>
        /// 材料
        /// </summary>
        public List<CellLinkInfo> Material { get; set; }
    }
    /// <summary>
    /// NPC关闭
    /// </summary>
    public sealed class NPCClose : Packet { }
    /// <summary>
    /// NPC精炼取回
    /// </summary>
    public sealed class NPCRefineRetrieve : Packet
    {
        /// <summary>
        /// 道具索引值
        /// </summary>
        public int Index { get; set; }
    }
    /// <summary>
    /// NPC首饰等级升级
    /// </summary>
    public sealed class NPCAccessoryLevelUp : Packet
    {
        /// <summary>
        /// 对象信息
        /// </summary>
        public CellLinkInfo Target { get; set; }
        /// <summary>
        /// 关联链接
        /// </summary>
        public List<CellLinkInfo> Links { get; set; }
    }
    /// <summary>
    /// NPC首饰升级
    /// </summary>
    public sealed class NPCAccessoryUpgrade : Packet
    {
        /// <summary>
        /// 对象信息
        /// </summary>
        public CellLinkInfo Target { get; set; }
        /// <summary>
        /// 精炼类型
        /// </summary>
        public RefineType RefineType { get; set; }
    }
    /// <summary>
    /// NPC附魔石合成
    /// </summary>
    public sealed class NPCEnchantmentSynthesis : Packet
    {
        /// <summary>
        /// 宝石格子1
        /// </summary>
        public List<CellLinkInfo> MaterialGrid1 { get; set; }
        /// <summary>
        /// 宝石格子2
        /// </summary>
        public List<CellLinkInfo> MaterialGrid2 { get; set; }
        /// <summary>
        /// 宝石格子3
        /// </summary>
        public List<CellLinkInfo> MaterialGrid3 { get; set; }
    }
    /// <summary>
    /// 魔法技能锁定键栏
    /// </summary>
    public sealed class MagicKey : Packet
    {
        /// <summary>
        /// 魔法技能类型
        /// </summary>
        public MagicType Magic { get; set; }
        /// <summary>
        /// 键栏1
        /// </summary>
        public SpellKey Set1Key { get; set; }
        /// <summary>
        /// 键栏2
        /// </summary>
        public SpellKey Set2Key { get; set; }
        /// <summary>
        /// 键栏3
        /// </summary>
        public SpellKey Set3Key { get; set; }
        /// <summary>
        /// 键栏4
        /// </summary>
        public SpellKey Set4Key { get; set; }
    }
    /// <summary>
    /// 魔法技能开关
    /// </summary>
    public sealed class MagicToggle : Packet
    {
        /// <summary>
        /// 魔法技能类型
        /// </summary>
        public MagicType Magic { get; set; }
        /// <summary>
        /// 是否可以使用
        /// </summary>
        public bool CanUse { get; set; }
    }
    /// <summary>
    /// 组队开关
    /// </summary>
    public sealed class GroupSwitch : Packet
    {
        /// <summary>
        /// 是否开启组队
        /// </summary>
        public bool Allow { get; set; }
    }
    /// <summary>
    /// 组队邀请
    /// </summary>
    public sealed class GroupInvite : Packet
    {
        /// <summary>
        /// 名字输入
        /// </summary>
        public string Name { get; set; }
    }
    /// <summary>
    /// 组队删除
    /// </summary>
    public sealed class GroupRemove : Packet
    {
        /// <summary>
        /// 名字输入
        /// </summary>
        public string Name { get; set; }
    }
    /// <summary>
    /// 组队响应
    /// </summary>
    public sealed class GroupResponse : Packet
    {
        /// <summary>
        /// 是否接受组队
        /// </summary>
        public bool Accept { get; set; }
    }
    /// <summary>
    /// 检查效验
    /// </summary>
    public sealed class Inspect : Packet
    {
        /// <summary>
        /// 索引值
        /// </summary>
        public int Index { get; set; }
    }
    /// <summary>
    /// 等级要求
    /// </summary>
    public sealed class RankRequest : Packet
    {
        /// <summary>
        /// 职业需求
        /// </summary>
        public RequiredClass Class { get; set; }
        /// <summary>
        /// 是否在线
        /// </summary>
        public bool OnlineOnly { get; set; }
        /// <summary>
        /// 起始索引
        /// </summary>
        public int StartIndex { get; set; }
    }
    /// <summary>
    /// 观察者请求
    /// </summary>
    public sealed class ObserverRequest : Packet
    {
        /// <summary>
        /// 名字
        /// </summary>
        public string Name { get; set; }
    }
    /// <summary>
    /// 观察者开关
    /// </summary>
    public sealed class ObservableSwitch : Packet
    {
        /// <summary>
        /// 是否开启观察者
        /// </summary>
        public bool Allow { get; set; }
    }
    /// <summary>
    /// 好友切换
    /// </summary>
    public sealed class FriendSwitch : Packet
    {
        /// <summary>
        /// 允许
        /// </summary>
        public bool Allow { get; set; }
    }
    /// <summary>
    /// 好友的回应
    /// </summary>
    public sealed class FriendResponse : Packet
    {
        /// <summary>
        /// 是否允许
        /// </summary>
        public bool Accept { get; set; }
        /// <summary>
        /// 名字
        /// </summary>
        public string Name { get; set; }
    }
    /// <summary>
    /// 好友请求
    /// </summary>
    public sealed class FriendRequest : Packet
    {
        /// <summary>
        /// 名字
        /// </summary>
        public string Name { get; set; }

    }
    /// <summary>
    /// 好友删除请求
    /// </summary>
    public sealed class FriendDeleteRequest : Packet
    {
        /// <summary>
        /// 链接的ID
        /// </summary>
        public string LinkID { get; set; }
    }

    /// <summary>
    /// 额外属性加点
    /// </summary>
    public sealed class Hermit : Packet
    {
        /// <summary>
        /// 属性状态
        /// </summary>
        public Stat Stat { get; set; }
    }
    /// <summary>
    /// 商城历史记录
    /// </summary>
    public sealed class MarketPlaceHistory : Packet
    {
        /// <summary>
        /// 索引值
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 显示
        /// </summary>
        public int Display { get; set; }
        /// <summary>
        /// 道具索引
        /// </summary>
        public int PartIndex { get; set; }
    }
    /// <summary>
    /// 商城委托寄售
    /// </summary>
    public sealed class MarketPlaceConsign : Packet
    {
        /// <summary>
        /// 关联信息
        /// </summary>
        public CellLinkInfo Link { get; set; }
        /// <summary>
        /// 价格
        /// </summary>
        public int Price { get; set; }
        /// <summary>
        /// 货币类型
        /// </summary>
        public CurrencyType PriceType { get; set; }
        /// <summary>
        /// 信息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 行会资金
        /// </summary>
        public bool GuildFunds { get; set; }
    }
    /// <summary>
    /// 商城搜索
    /// </summary>
    public sealed class MarketPlaceSearch : Packet
    {
        /// <summary>
        /// 名字
        /// </summary>
        public string Name { get; set; }
        public string SellName { get; set; }
        /// <summary>
        /// 道具类型筛选
        /// </summary>
        public bool ItemTypeFilter { get; set; }
        /// <summary>
        /// 道具类型
        /// </summary>
        public ItemType ItemType { get; set; }
        /// <summary>
        /// 商城分类
        /// </summary>
        public MarketPlaceSort Sort { get; set; }
        public CurrencyType PriceType { get; set; }
    }
    /// <summary>
    /// 商城搜索索引
    /// </summary>
    public sealed class MarketPlaceSearchIndex : Packet
    {
        /// <summary>
        /// 索引值
        /// </summary>
        public int Index { get; set; }
    }
    /// <summary>
    /// 商城取消寄售
    /// </summary>
    public sealed class MarketPlaceCancelConsign : Packet
    {
        /// <summary>
        /// 索引值
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public long Count { get; set; }
    }
    /// <summary>
    /// 商城购买
    /// </summary>
    public sealed class MarketPlaceBuy : Packet
    {
        /// <summary>
        /// 索引值
        /// </summary>
        public long Index { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public long Count { get; set; }
        /// <summary>
        /// 行会资金
        /// </summary>
        public bool GuildFunds { get; set; }
    }
    /// <summary>
    /// 商城购买
    /// </summary>
    public sealed class MarketPlaceStoreBuy : Packet
    {
        /// <summary>
        /// 索引值
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public long Count { get; set; }
        /// <summary>
        /// 角色赏金购买
        /// </summary>
        public bool UseHuntGold { get; set; }
    }
    /// <summary>
    /// 邮件打开过的
    /// </summary>
    public sealed class MailOpened : Packet
    {
        /// <summary>
        /// 索引值
        /// </summary>
        public int Index { get; set; }
    }
    /// <summary>
    /// 邮件里获得的道具
    /// </summary>
    public sealed class MailGetItem : Packet
    {
        /// <summary>
        /// 索引值
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 单元格
        /// </summary>
        public int Slot { get; set; }
    }
    /// <summary>
    /// 邮件删除
    /// </summary>
    public sealed class MailDelete : Packet
    {
        /// <summary>
        /// 索引值
        /// </summary>
        public int Index { get; set; }
    }
    /// <summary>
    /// 邮件发送
    /// </summary>
    public sealed class MailSend : Packet
    {
        /// <summary>
        /// 关联
        /// </summary>
        public List<CellLinkInfo> Links { get; set; }
        /// <summary>
        /// 收件人
        /// </summary>
        public string Recipient { get; set; }
        /// <summary>
        /// 主题
        /// </summary>
        public string Subject { get; set; }
        /// <summary>
        /// 信件内容
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 金币
        /// </summary>
        public long Gold { get; set; }
    }
    /// <summary>
    /// 攻击模式改变
    /// </summary>
    public sealed class ChangeAttackMode : Packet
    {
        /// <summary>
        /// 攻击模式
        /// </summary>
        public AttackMode Mode { get; set; }
    }
    /// <summary>
    /// 宠物攻击模式改变
    /// </summary>
    public sealed class ChangePetMode : Packet
    {
        /// <summary>
        /// 宠物攻击模式
        /// </summary>
        public PetMode Mode { get; set; }
    }
    /// <summary>
    /// 元宝
    /// </summary>
    public sealed class GameGoldRecharge : Packet { }
    /// <summary>
    /// 声望
    /// </summary>
    public sealed class PrestigeRecharge : Packet { }
    /// <summary>
    /// 贡献
    /// </summary>
    public sealed class ContributeRecharge : Packet { }
    /// <summary>
    /// 交易请求
    /// </summary>
    public sealed class TradeRequest : Packet
    {
        /// <summary>
        /// 索引值
        /// </summary>
        public int Index { get; set; }
    }
    /// <summary>
    /// 交易请求响应
    /// </summary>
    public sealed class TradeRequestResponse : Packet
    {
        /// <summary>
        /// 接受
        /// </summary>
        public bool Accept { get; set; }
    }
    /// <summary>
    /// 交易完成
    /// </summary>
    public sealed class TradeClose : Packet { }
    /// <summary>
    /// 交易增加金币
    /// </summary>
    public sealed class TradeAddGold : Packet
    {
        /// <summary>
        /// 金币
        /// </summary>
        public long Gold { get; set; }
    }
    /// <summary>
    /// 交易增加道具
    /// </summary>
    public sealed class TradeAddItem : Packet
    {
        /// <summary>
        /// 关联信息
        /// </summary>
        public CellLinkInfo Cell { get; set; }
    }
    /// <summary>
    /// 交易确认
    /// </summary>
    public sealed class TradeConfirm : Packet { }
    /// <summary>
    /// 获取记忆坐标点信息
    /// </summary>
    public sealed class GetFixedPointinfo : Packet { }
    /// <summary>
    /// 行会创建
    /// </summary>
    public sealed class GuildCreate : Packet
    {
        /// <summary>
        /// 行会名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 行会金币
        /// </summary>
        public bool UseGold { get; set; }
        /// <summary>
        /// 行会成员
        /// </summary>
        public int Members { get; set; }
        /// <summary>
        /// 行会仓库
        /// </summary>
        public int Storage { get; set; }
    }
    /// <summary>
    /// 行会编辑公告
    /// </summary>
    public sealed class GuildEditNotice : Packet
    {
        /// <summary>
        /// 公告信息
        /// </summary>
        public string Notice { get; set; }
    }
    /// <summary>
    /// 编辑金库公告
    /// </summary>
    public sealed class GuildEditVaultNotice : Packet
    {
        /// <summary>
        /// 公告信息
        /// </summary>
        public string Notice { get; set; }
    }
    /// <summary>
    /// 捐献金币
    /// </summary>
    public sealed class GuildDonation : Packet
    {
        public long Amount { get; set; }
    }

    /// <summary>
    /// 行会赞助币操作 0 为捐献 1为提取
    /// </summary>
    public sealed class GuildGameGold : Packet
    {
        public int Type { get; set; }
        public long Amount { get; set; }
    }

    public sealed class GuildUpdate : Packet
    {

    }
    /// <summary>
    /// 行会取款
    /// </summary>
    public sealed class GuildWithdrawFund : Packet
    {
        public long Amount { get; set; }
    }
    /// <summary>
    /// 行会编辑成员
    /// </summary>
    public sealed class GuildEditMember : Packet
    {
        /// <summary>
        /// 索引值
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 地位
        /// </summary>
        public string Rank { get; set; }
        /// <summary>
        /// 行会权限许可
        /// </summary>
        public GuildPermission Permission { get; set; }
    }
    /// <summary>
    /// 行会邀请成员
    /// </summary>
    public sealed class GuildInviteMember : Packet
    {
        /// <summary>
        /// 名字
        /// </summary>
        public string Name { get; set; }
    }
    /// <summary>
    /// 行会踢除成员
    /// </summary>
    public sealed class GuildKickMember : Packet
    {
        /// <summary>
        /// 索引值
        /// </summary>
        public int Index { get; set; }
    }
    /// <summary>
    /// 行会税收
    /// </summary>
    public sealed class GuildTax : Packet
    {
        /// <summary>
        /// 税率
        /// </summary>
        public long Tax { get; set; }
    }
    /// <summary>
    /// 行会旗帜
    /// </summary>
    public sealed class GuildFlag : Packet
    {
        /// <summary>
        /// 行会旗帜
        /// </summary>
        public int Flag { get; set; }
        /// <summary>
        /// 行会旗帜颜色
        /// </summary>
        public Color Color { get; set; }
    }
    /// <summary>
    /// 行会增加成员数
    /// </summary>
    public sealed class GuildIncreaseMember : Packet { }
    /// <summary>
    /// 行会增加仓库空间
    /// </summary>
    public sealed class GuildIncreaseStorage : Packet { }
    /// <summary>
    /// 行会邀请响应
    /// </summary>
    public sealed class GuildResponse : Packet
    {
        /// <summary>
        /// 接受
        /// </summary>
        public bool Accept { get; set; }
    }
    /// <summary>
    /// 行会战
    /// </summary>
    public sealed class GuildWar : Packet
    {
        /// <summary>
        /// 行会名字
        /// </summary>
        public string GuildName { get; set; }
    }
    /// <summary>
    /// 行会联盟
    /// </summary>
    public sealed class GuildAlliance : Packet
    {
        /// <summary>
        /// 行会名字
        /// </summary>
        public string GuildName { get; set; }
    }
    /// <summary>
    /// 行会结束联盟
    /// </summary>
    public sealed class EndGuildAlliance : Packet
    {
        /// <summary>
        /// 行会名字
        /// </summary>
        public string GuildName { get; set; }
    }
    /// <summary>
    /// 行会攻城申请
    /// </summary>
    public sealed class GuildRequestConquest : Packet
    {
        /// <summary>
        /// 索引值
        /// </summary>
        public int Index { get; set; }
    }
    /// <summary>
    /// 接受任务
    /// </summary>
    public sealed class QuestAccept : Packet
    {
        /// <summary>
        /// 索引值
        /// </summary>
        public int Index { get; set; }
    }
    /// <summary>
    /// 完成任务
    /// </summary>
    public sealed class QuestComplete : Packet
    {
        /// <summary>
        /// 索引值
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 选择索引
        /// </summary>
        public int ChoiceIndex { get; set; }
    }
    /// <summary>
    /// 任务跟踪
    /// </summary>
    public sealed class QuestTrack : Packet
    {
        /// <summary>
        /// 索引值
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 是否跟踪
        /// </summary>
        public bool Track { get; set; }
    }
    /// <summary>
    /// 宠物解锁
    /// </summary>
    public sealed class CompanionUnlock : Packet
    {
        /// <summary>
        /// 索引值
        /// </summary>
        public int Index { get; set; }
    }
    /// <summary>
    /// 宠物自动进食解锁
    /// </summary>
    public sealed class CompanionAutoFeedUnlock : Packet
    {
        /// <summary>
        /// 索引值
        /// </summary>
        public int Index { get; set; }
    }
    /// <summary>
    /// 宠物购买
    /// </summary>
    public sealed class CompanionAdopt : Packet
    {
        /// <summary>
        /// 索引值
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 名字
        /// </summary>
        public string Name { get; set; }
    }
    /// <summary>
    /// 宠物取回
    /// </summary>
    public sealed class CompanionRetrieve : Packet
    {
        /// <summary>
        /// 索引值
        /// </summary>
        public int Index { get; set; }
    }
    /// <summary>
    /// 宠物寄存
    /// </summary>
    public sealed class CompanionStore : Packet
    {
        /// <summary>
        /// 索引值
        /// </summary>
        public int Index { get; set; }
    }
    /// <summary>
    /// 结婚响应
    /// </summary>
    public sealed class MarriageResponse : Packet
    {
        /// <summary>
        /// 接受
        /// </summary>
        public bool Accept { get; set; }
    }
    /// <summary>
    /// 结婚戒指
    /// </summary>
    public sealed class MarriageMakeRing : Packet
    {
        /// <summary>
        /// 单元格
        /// </summary>
        public int Slot { get; set; }
    }
    /// <summary>
    /// 结婚传送
    /// </summary>
    public sealed class MarriageTeleport : Packet { }
    /// <summary>
    /// 指定玩家增加到黑名单
    /// </summary>
    public sealed class BlockAdd : Packet
    {
        /// <summary>
        /// 名字
        /// </summary>
        public string Name { get; set; }
    }
    /// <summary>
    /// 指定玩家移出黑名单
    /// </summary>
    public sealed class BlockRemove : Packet
    {
        /// <summary>
        /// 索引值
        /// </summary>
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
    /// <summary>
    /// 性别改变
    /// </summary>
    public sealed class GenderChange : Packet
    {
        /// <summary>
        /// 性别
        /// </summary>
        public MirGender Gender { get; set; }
        /// <summary>
        /// 头发类型
        /// </summary>
        public int HairType { get; set; }
        /// <summary>
        /// 头发颜色
        /// </summary>
        public Color HairColour { get; set; }
    }
    /// <summary>
    /// 发型改变
    /// </summary>
    public sealed class HairChange : Packet
    {
        /// <summary>
        /// 发型类型
        /// </summary>
        public int HairType { get; set; }
        /// <summary>
        /// 发型颜色
        /// </summary>
        public Color HairColour { get; set; }
    }
    /// <summary>
    /// 衣服染色
    /// </summary>
    public sealed class ArmourDye : Packet
    {
        /// <summary>
        /// 衣服颜色
        /// </summary>
        public Color ArmourColour { get; set; }
    }
    /// <summary>
    /// 名字改变
    /// </summary>
    public sealed class NameChange : Packet
    {
        /// <summary>
        /// 名字
        /// </summary>
        public string Name { get; set; }
    }
    /// <summary>
    /// 财富变化
    /// </summary>
    public sealed class FortuneCheck : Packet
    {
        /// <summary>
        /// 道具索引值
        /// </summary>
        public int ItemIndex { get; set; }
    }
    /// <summary>
    /// 传送戒指
    /// </summary>
    public sealed class TeleportRing : Packet
    {
        /// <summary>
        /// 坐标
        /// </summary>
        public Point Location { get; set; }
        /// <summary>
        /// 索引值
        /// </summary>
        public int Index { get; set; }
    }
    /// <summary>
    /// 加入新手行会
    /// </summary>
    public sealed class JoinStarterGuild : Packet { }
    /// <summary>
    /// NPC首饰重置
    /// </summary>
    public sealed class NPCAccessoryReset : Packet
    {
        /// <summary>
        /// 关联信息
        /// </summary>
        public CellLinkInfo Cell { get; set; }
    }
    /// <summary>
    /// NPC武器工艺
    /// </summary>
    public sealed class NPCWeaponCraft : Packet
    {
        /// <summary>
        /// 职业
        /// </summary>
        public RequiredClass Class { get; set; }
        /// <summary>
        /// 模板
        /// </summary>
        public CellLinkInfo Template { get; set; }
        /// <summary>
        /// 黄石头
        /// </summary>
        public CellLinkInfo Yellow { get; set; }
        /// <summary>
        /// 蓝石头
        /// </summary>
        public CellLinkInfo Blue { get; set; }
        /// <summary>
        /// 红石头
        /// </summary>
        public CellLinkInfo Red { get; set; }
        /// <summary>
        /// 紫石头
        /// </summary>
        public CellLinkInfo Purple { get; set; }
        /// <summary>
        /// 绿石头
        /// </summary>
        public CellLinkInfo Green { get; set; }
        /// <summary>
        /// 灰石头
        /// </summary>
        public CellLinkInfo Grey { get; set; }
    }
    /// <summary>
    /// 制作开启
    /// </summary>
    public sealed class CraftStart : Packet
    {
        /// <summary>
        /// 目标道具索引值
        /// </summary>
        public int TargetItemIndex { get; set; }
    }
    /// <summary>
    /// 制作取消
    /// </summary>
    public sealed class CraftCancel : Packet { }
    /// <summary>
    /// 传奇宝箱变化
    /// </summary>
    public sealed class TreasureChange : Packet { }
    /// <summary>
    /// 传奇宝箱开奖
    /// </summary>
    public sealed class TreasureSelect : Packet
    {
        /// <summary>
        /// 道具单元格
        /// </summary>
        public int Slot { get; set; }
    }
    /// <summary>
    /// 宝石镶嵌
    /// </summary>
    public sealed class AttachGem : Packet
    {
        /// <summary>
        /// 格子类型 从格子里
        /// </summary>
        public GridType FromGrid { get; set; }
        /// <summary>
        /// 从单元格
        /// </summary>
        public int FromSlot { get; set; }
        /// <summary>
        /// 格子类型 到格子里
        /// </summary>
        public GridType ToGrid { get; set; }
        /// <summary>
        /// 到单元格
        /// </summary>
        public int ToSlot { get; set; }
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; }
    }
    /// <summary>
    /// 宝石打孔
    /// </summary>
    public sealed class AddHole : Packet
    {
        /// <summary>
        /// 格子类型 从格子里
        /// </summary>
        public GridType FromGrid { get; set; }
        /// <summary>
        /// 从单元格
        /// </summary>
        public int FromSlot { get; set; }
        /// <summary>
        /// 格子类型 到格子里
        /// </summary>
        public GridType ToGrid { get; set; }
        /// <summary>
        /// 到单元格
        /// </summary>
        public int ToSlot { get; set; }
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; }
    }
    /// <summary>
    /// 取下宝石
    /// </summary>
    public sealed class RemoveGem : Packet
    {
        /// <summary>
        /// 格子类型 从格子里
        /// </summary>
        public GridType FromGrid { get; set; }
        /// <summary>
        /// 从单元格
        /// </summary>
        public int FromSlot { get; set; }
        /// <summary>
        /// 格子类型 到格子里
        /// </summary>
        public GridType ToGrid { get; set; }
        /// <summary>
        /// 到单元格
        /// </summary>
        public int ToSlot { get; set; }
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; }
    }
    /// <summary>
    /// 佩戴成就称号
    /// </summary>
    public sealed class WearAchievementTitle : Packet
    {
        /// <summary>
        /// 成就称号索引值
        /// </summary>
        public int AchievementIndex { get; set; }
    }
    /// <summary>
    /// 移除成就称号
    /// </summary>
    public sealed class TakeOffAchievementTitle : Packet { }
    /// <summary>
    /// 记忆传送记录
    /// </summary>
    public sealed class cs_FixedPoint : Packet
    {
        /// <summary>
        /// 0新增 1修改 2删除
        /// </summary>
        public int Opt { get; set; }
        /// <summary>
        /// 记忆传送记录信息
        /// </summary>
        public ClientFixedPointInfo Info { get; set; }
    }
    /// <summary>
    /// 记忆传送移动
    /// </summary>
    public sealed class cs_FixedPointMove : Packet
    {
        /// <summary>
        /// 固定移动
        /// </summary>
        public FixeUnit Uind { get; set; }
    }

    /// <summary>
    /// 获取每日任务
    /// </summary>
    public sealed class GetDailyQuest : Packet { }
    /// <summary>
    /// 菜单栏
    /// </summary>
    public sealed class ShortcutDialogClicked : Packet { }
    /// <summary>
    /// 钓鱼抛竿
    /// </summary>
    public sealed class FishingCast : Packet { }
    /// <summary>
    /// 钓鱼收杆
    /// </summary>
    public sealed class FishingReel : Packet
    {

    }

    /// <summary>
    /// 新版武器升级
    /// </summary>
    public sealed class NPCWeaponUpgrade : Packet
    {
        public RefineType refineType { get; set; }
        public CellLinkInfo weapon { get; set; }
        public List<CellLinkInfo> iron { get; set; }
        public List<CellLinkInfo> frag1 { get; set; }
        public List<CellLinkInfo> frag2 { get; set; }
        public List<CellLinkInfo> frag3 { get; set; }
        public List<CellLinkInfo> stone { get; set; }
    }

    public sealed class NPCWeaponUpgradeRetrieve : Packet
    {
        public int Index { get; set; }
    }

    public sealed class PyTextBoxResponse : Packet
    {
        public string ID { get; set; }
        public bool IsOK { get; set; }
        public string UserInput { get; set; }
    }

    public sealed class EnterAncientTomb : Packet
    {
        public string FirstChar { get; set; }
        public string SecondChar { get; set; }
        public string ThirdChar { get; set; }
    }

    public sealed class GuildAllowApplyChanged : Packet
    {
        public bool AllowApply { get; set; }
    }
    public sealed class ApplyJoinGuildPacket : Packet
    {
        public int GuildIndex { get; set; }
    }

    public sealed class GetGuileWithDrawal : Packet
    {

    }
    public sealed class GetGuildApplications : Packet
    {

    }
    public sealed class GuileWithdrawalApplyChoice : Packet
    {
        public bool Approved { get; set; }
        public int PlayerIndex { get; set; }
    }

    public sealed class GuildApplyChoice : Packet
    {
        public bool Approved { get; set; }
        public int PlayerIndex { get; set; }
    }

    public sealed class TossCoin : Packet
    {
        public int Angle { get; set; }
        public double InitialDistance { get; set; }
        public double SelectedDistance { get; set; }
        public TossCoinOption TossOption { get; set; }

    }

    public sealed class RemoveTaishanBuff : Packet
    {
        public int BuffIndex { get; set; }
    }


    public sealed class GiveUpQuest : Packet
    {
        public int QuestIndex { get; set; }
    }

    // 新版首饰升级
    public sealed class AccessoryCombineRequest : Packet
    {
        public JewelryRefineType RefineType { get; set; }
        public CellLinkInfo MainItem { get; set; }
        public CellLinkInfo ExtraItem1 { get; set; }
        public CellLinkInfo ExtraItem2 { get; set; }
        public CellLinkInfo Corundum { get; set; }
        public CellLinkInfo Crystal { get; set; }
    }

    // 拾取过滤
    public sealed class CompanionPickUpSkipUpdate : Packet
    {
        public List<int> CompanionPickupSkipList { get; set; }
    }
    // 抢红包
    public sealed class ClaimRedpacket : Packet
    {
        public int RedpacketIndex { get; set; }
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
    public sealed class GoldMarketTrade : Packet
    {
        public long Gold { set; get; }
        public long GameGold { set; get; }
        public TradeType TradeType { get; set; }
    }
    public sealed class GoldMarketCannel : Packet
    {
        public int Index { get; set; }
    }
    public sealed class GoldMarketFlash : Packet
    {

    }
    public sealed class GoldMarketGetOwnOrder : Packet
    {

    }

    public sealed class AutionsFlash : Packet
    {
        public string Name { get; set; }
    }

    public sealed class AutionsFlashIndex : Packet
    {
        public int Index { get; set; }
    }
    public sealed class AutionsAdd : Packet
    {
        public CellLinkInfo Link { get; set; }
        public long Price { set; get; }
        public long BuyItNowPrice { set; get; }
        public long PerAddPrice { set; get; }

    }
    public sealed class AutinsBuy : Packet
    {
        public int Index { get; set; }
        public long BuyPrice { set; get; }
    }

    /// <summary>
    /// 装填弹药
    /// </summary>
    public sealed class Ammunition : Packet
    {
        /// <summary>
        /// 格子类型
        /// </summary>
        public GridType Grid { get; set; }
        /// <summary>
        /// 单元格
        /// </summary>
        public int Slot { get; set; }
        /// <summary>
        /// 计数
        /// </summary>
        public int Count { get; set; }
    }

    public sealed class WarWeapAttackCoordinates : Packet
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

    public sealed class WarWeapAttackStop : Packet
    {

    }

    public sealed class WarWeapMove : Packet
    {

    }

    public sealed class ResponseProcessHash : Packet
    {
        public List<string> HashList { get; set; }
        public DateTime DateTime { get; set; }
    }

    public sealed class SellCharacter : Packet
    {
        public long Price { set; get; }
    }
    public sealed class CanelSellCharacter : Packet
    {
        public int CharacterIndex { set; get; }
    }
    public sealed class SellCharacterSearch : Packet
    {
        /// <summary>
        /// 角色名字
        /// </summary>
        public string Name { get; set; }
        public MirClass Class { get; set; }   //职业
        public int Level { get; set; }   //等级
        public MirGender Gender { get; set; }   //性别
        /// <summary>
        /// 商城销售价格
        /// </summary>
        public long Price { get; set; }

    }
    public sealed class Equipment : Packet
    {
        public int Index { get; set; }
    }

    public sealed class InspectMagery : Packet
    {
        public int Index { get; set; }
    }

    public sealed class InspectPackSack : Packet
    {
        public int Index { get; set; }
    }

    public sealed class MarketPlaceJiaoseBuy : Packet
    {
        public int Index { get; set; }
        public long Count { get; set; }
    }
}

