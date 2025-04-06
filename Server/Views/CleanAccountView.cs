using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using Server.DBModels;
using Server.Envir;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Server.Views
{
    public partial class CleanAccountView : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public CleanAccountView()
        {
            InitializeComponent();

            if (SEnvir.Session == null)
            {
                XtraMessageBox.Show("请先启动服务端", "无法清理", MessageBoxButtons.OK);
            }
            else
            {
                CleanAccountGridControl.DataSource = SEnvir.Session.GetCollection<AccountInfo>();
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SMain.SetUpView(CleanAccountGridView);
        }

        private void SaveButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            SEnvir.Session.Save(true, MirDB.SessionMode.Server);
        }

        private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)
        {
            //TODO 导出
        }

        private void barButtonItem2_ItemClick(object sender, ItemClickEventArgs e)
        {
            //TODO 导入
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            int[] selectedRows = CleanAccountGridView.GetSelectedRows();
            if (selectedRows.Length == 0)
            {
                XtraMessageBox.Show("请先选择至少一个账号", "无法清理", MessageBoxButtons.OK);
                return;
            }

            // 询问是否确认
            if (XtraMessageBox.Show($"是否确认清理选中的{selectedRows.Length}个账号？请先手动备份，删除操作不可撤销", "确认清理", MessageBoxButtons.YesNo) !=
                DialogResult.Yes)
            {
                return;
            }

            if (XtraMessageBox.Show("部分角色可能需要重启才能完全删除，删除后请手动检查", "注意事项", MessageBoxButtons.YesNo) != DialogResult.Yes)
            {
                return;
            }

            int successCount = 0;
            int failCount = 0;
            List<AccountInfo> deletedAccountInfos = new List<AccountInfo>();
            foreach (int rowHandle in selectedRows)
            {
                if (CleanAccountGridView.GetRow(rowHandle) is AccountInfo accountInfo)
                {
                    deletedAccountInfos.Add(accountInfo);
                    if (DeleteSingleAccount(accountInfo))
                    {
                        successCount++;
                    }
                    else
                    {
                        failCount++;
                    }
                }
            }

            foreach (AccountInfo deletedAccountInfo in deletedAccountInfos)
            {
                if (deletedAccountInfo != null)
                {
                    deletedAccountInfo.Delete();
                }
            }

            XtraMessageBox.Show($"清理完成，选择{selectedRows.Length}个，处理{successCount + failCount}个，成功{successCount}个，失败{failCount}个", "清理完成", MessageBoxButtons.OK);
            if (XtraMessageBox.Show("是否保存？", "保存", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                SEnvir.Session.Save(true, MirDB.SessionMode.Server);
                XtraMessageBox.Show("保存成功, 请关闭此页面并重新打开，查看结果", "保存成功", MessageBoxButtons.OK);
            }
        }

        #region 删除关联数据

        private void DeleteLinkedGuildInfo(AccountInfo info)
        {
            if (info.GuildMember == null)
            {
                SEnvir.Log($"[账号清理] 账号[{info.EMailAddress}]没有行会，跳过");
                return;
            }
            GuildMemberInfo guildMemberInfo = info.GuildMember;

            // 删除GuildMemberInfo
            guildMemberInfo.Delete();
            info.GuildMember = null;
            SEnvir.Log($"[账号清理] 账号[{info.EMailAddress}]的行会成员信息已删除");
        }

        private void DeleteLinkedUserItem(AccountInfo info)
        {
            List<UserItem> userItems = SEnvir.UserItemList.Binding.Where(x => x?.Account?.Index == info.Index).ToList();
            int counter = 0;
            foreach (UserItem userItem in userItems)
            {
                userItem.Delete();
                counter++;
            }
            SEnvir.Log($"[账号清理] 账号[{info.EMailAddress}]的{counter}个物品已删除");
        }

        // Buffs
        private void DeleteLinkedBuffs(AccountInfo info)
        {
            List<BuffInfo> buffInfos = SEnvir.BuffInfoList.Binding.Where(x => x?.Account?.Index == info.Index).ToList();
            int counter = 0;
            foreach (BuffInfo buffInfo in buffInfos)
            {
                buffInfo.Delete();
                counter++;
            }
            SEnvir.Log($"[账号清理] 账号[{info.EMailAddress}]的{counter}个Buff已删除");
        }

        // Auctions
        private void DeleteLinkedAuctions(AccountInfo info)
        {
            List<AuctionInfo> auctionInfos = SEnvir.AuctionInfoList.Binding.Where(x => x?.Account?.Index == info.Index).ToList();
            int counter = 0;
            foreach (AuctionInfo auctionInfo in auctionInfos)
            {
                auctionInfo.Delete();
                counter++;
            }
            SEnvir.Log($"[账号清理] 账号[{info.EMailAddress}]的{counter}个拍卖已删除");
        }

        // Mail
        private void DeleteLinkedMail(AccountInfo info)
        {
            List<MailInfo> mailInfos = SEnvir.MailInfoList.Binding.Where(x => x?.Account?.Index == info.Index).ToList();
            int counter = 0;
            foreach (MailInfo mailInfo in mailInfos)
            {
                mailInfo.Delete();
                counter++;
            }
            SEnvir.Log($"[账号清理] 账号[{info.EMailAddress}]的{counter}封邮件已删除");
        }

        // UserDrops
        private void DeleteLinkedUserDrops(AccountInfo info)
        {
            List<UserDrop> userDrops = SEnvir.UserDropList.Binding.Where(x => x?.Account?.Index == info.Index).ToList();
            int counter = 0;
            foreach (UserDrop userDrop in userDrops)
            {
                userDrop.Delete();
                counter++;
            }
            SEnvir.Log($"[账号清理] 账号[{info.EMailAddress}]的{counter}个掉落已删除");
        }

        // Companions
        private void DeleteLinkedCompanions(AccountInfo info)
        {
            List<UserCompanion> userCompanions = SEnvir.UserCompanionList.Binding.Where(x => x?.Account?.Index == info.Index).ToList();
            int counter = 0;
            foreach (UserCompanion userCompanion in userCompanions)
            {
                userCompanion.Delete();
                counter++;
            }
            SEnvir.Log($"[账号清理] 账号[{info.EMailAddress}]的{counter}个宠物已删除");
        }

        // CompanionUnlocks
        private void DeleteLinkedCompanionUnlocks(AccountInfo info)
        {
            List<UserCompanionUnlock> companionUnlocks = SEnvir.UserCompanionUnlockList.Binding.Where(x => x?.Account?.Index == info.Index).ToList();
            int counter = 0;
            foreach (UserCompanionUnlock companionUnlock in companionUnlocks)
            {
                companionUnlock.Delete();
                counter++;
            }
            SEnvir.Log($"[账号清理] 账号[{info.EMailAddress}]的{counter}个宠物解锁已删除");
        }


        // BlockingList
        private void DeleteLinkedBlockingList(AccountInfo info)
        {
            List<BlockInfo> blockingInfos = SEnvir.BlockInfoList.Binding.Where(x => x?.Account?.Index == info.Index).ToList();
            int counter = 0;
            foreach (BlockInfo blockingInfo in blockingInfos)
            {
                blockingInfo.Delete();
                counter++;
            }
            SEnvir.Log($"[账号清理] 账号[{info.EMailAddress}]的{counter}个屏蔽信息已删除");
        }

        // BlockedByList
        private void DeleteLinkedBlockedByList(AccountInfo info)
        {
            List<BlockInfo> blockedByInfos = SEnvir.BlockInfoList.Binding.Where(x => x?.BlockedAccount?.Index == info.Index).ToList();
            int counter = 0;
            foreach (BlockInfo blockedByInfo in blockedByInfos)
            {
                blockedByInfo.Delete();
                counter++;
            }
            SEnvir.Log($"[账号清理] 账号[{info.EMailAddress}]的{counter}被屏蔽信息已删除");
        }


        // Payments
        private void DeleteLinkedPayments(AccountInfo info)
        {
            List<GameGoldPayment> paymentInfos = SEnvir.GameGoldPaymentList.Binding.Where(x => x?.Account?.Index == info.Index).ToList();
            int counter = 0;
            foreach (GameGoldPayment paymentInfo in paymentInfos)
            {
                paymentInfo.Delete();
                counter++;
            }
            SEnvir.Log($"[账号清理] 账号[{info.EMailAddress}]的{counter}个充值信息已删除");
        }

        // StoreSales
        private void DeleteLinkedStoreSales(AccountInfo info)
        {
            List<GameStoreSale> storeSales = SEnvir.GameStoreSaleList.Binding.Where(x => x?.Account?.Index == info.Index).ToList();
            int counter = 0;
            foreach (GameStoreSale storeSale in storeSales)
            {
                storeSale.Delete();
                counter++;
            }
            SEnvir.Log($"[账号清理] 账号[{info.EMailAddress}]的{counter}个商店销售信息已删除");
        }

        // Fortunes
        private void DeleteLinkedFortunes(AccountInfo info)
        {
            List<UserFortuneInfo> fortuneInfos = SEnvir.UserFortuneInfoList.Binding.Where(x => x?.Account?.Index == info.Index).ToList();
            int counter = 0;
            foreach (UserFortuneInfo fortuneInfo in fortuneInfos)
            {
                fortuneInfo.Delete();
                counter++;
            }
            SEnvir.Log($"[账号清理] 账号[{info.EMailAddress}]的{counter}个财富信息已删除");
        }

        // CharacterShop
        private void DeleteLinkedCharacterShop(AccountInfo info)
        {
            List<CharacterShop> characterShopInfos = SEnvir.CharacterShopList.Binding.Where(x => x?.Account?.Index == info.Index).ToList();
            int counter = 0;
            foreach (CharacterShop characterShopInfo in characterShopInfos)
            {
                characterShopInfo.Delete();
                counter++;
            }
            SEnvir.Log($"[账号清理] 账号[{info.EMailAddress}]的{counter}个商店信息已删除");
        }


        private void DeleteLinkedCharacter(AccountInfo info)
        {
            List<CharacterInfo> characterInfos = new List<CharacterInfo>(info.Characters);
            int counter = 0;
            foreach (CharacterInfo characterInfo in characterInfos)
            {
                SEnvir.Log($"[账号清理] 正在清理[{info.EMailAddress}]的角色[{characterInfo.CharacterName}]...");
                // Companion
                DeleteCharacterLinkedCompanion(characterInfo);

                // UserItem
                DeleteCharacterLinkedUserItem(characterInfo);

                // GoldMarketInfo
                DeleteCharacterLinkedGoldMarketInfo(characterInfo);

                // BeltLinks
                DeleteCharacterLinkedBeltLink(characterInfo);

                // AutoPotionLinks
                DeleteCharacterLinkedAutoPotionLink(characterInfo);

                // AutoFightLinks
                DeleteCharacterLinkedAutoFightLink(characterInfo);

                //Magics
                DeleteCharacterLinkedMagics(characterInfo);

                // Buffs
                DeleteCharacterLinkedBuffs(characterInfo);

                // Refines
                DeleteCharacterLinkedRefines(characterInfo);

                // Quests
                DeleteCharacterLinkedQuests(characterInfo);

                // Friends
                DeleteCharacterLinkedFriends(characterInfo);

                // FPointLinks
                DeleteCharacterLinkedFPointLinks(characterInfo);

                // Values
                DeleteCharacterLinkedValues(characterInfo);

                // Achievements
                DeleteCharacterLinkedAchievements(characterInfo);

                SEnvir.Log($"[账号清理] 角色[{characterInfo.CharacterName}]的关联数据已删除");
                characterInfo.Delete();
                counter++;
            }
        }


        #region 角色关联数据

        private void DeleteCharacterLinkedCompanion(CharacterInfo info)
        {
            List<UserCompanion> userCompanions = SEnvir.UserCompanionList.Binding.Where(x => x.Character?.Index == info.Index).ToList();
            int counter = 0;
            foreach (UserCompanion companionInfo in userCompanions)
            {
                companionInfo.Delete();
                counter++;
            }
            SEnvir.Log($"[账号清理] 角色[{info.CharacterName}]的{counter}个宠物已删除");
        }

        private void DeleteCharacterLinkedUserItem(CharacterInfo info)
        {
            List<UserItem> userItems = SEnvir.UserItemList.Binding.Where(x => x.Character?.Index == info.Index).ToList();
            int counter = 0;
            foreach (UserItem userItem in userItems)
            {
                if (userItem == null)
                    continue;

                userItem.Delete();
                counter++;
            }
            SEnvir.Log($"[账号清理] 角色[{info.CharacterName}]的{counter}个物品已删除");
        }

        private void DeleteCharacterLinkedGoldMarketInfo(CharacterInfo info)
        {
            List<GoldMarketInfo> goldMarketInfos = SEnvir.GoldMarketInfoList.Binding.Where(x => x.Character?.Index == info.Index).ToList();
            int counter = 0;
            foreach (GoldMarketInfo goldMarketInfo in goldMarketInfos)
            {
                goldMarketInfo.Delete();
                counter++;
            }
            SEnvir.Log($"[账号清理] 角色[{info.CharacterName}]的{counter}个金币市场信息已删除");
        }

        private void DeleteCharacterLinkedBeltLink(CharacterInfo info)
        {
            List<CharacterBeltLink> beltLinkInfos = SEnvir.BeltLinkList.Binding.Where(x => x.Character?.Index == info.Index).ToList();
            int counter = 0;
            foreach (var beltLinkInfo in beltLinkInfos)
            {
                beltLinkInfo.Delete();
                counter++;
            }
            SEnvir.Log($"[账号清理] 角色[{info.CharacterName}]的{counter}个快捷药品信息已删除");
        }


        private void DeleteCharacterLinkedAutoPotionLink(CharacterInfo info)
        {
            List<AutoPotionLink> autoPotionLinks = SEnvir.AutoPotionLinkList.Binding.Where(x => x.Character?.Index == info.Index).ToList();
            int counter = 0;
            foreach (var autoPotionLink in autoPotionLinks)
            {
                autoPotionLink.Delete();
                counter++;
            }
            SEnvir.Log($"[账号清理] 角色[{info.CharacterName}]的{counter}个自动喝药信息已删除");
        }

        // AutoFightLinks
        private void DeleteCharacterLinkedAutoFightLink(CharacterInfo info)
        {
            List<AutoFightConfig> autoFightLinks = SEnvir.AutoFightConfList.Binding.Where(x => x.Character?.Index == info.Index).ToList();
            int counter = 0;
            foreach (var autoFightLink in autoFightLinks)
            {
                autoFightLink.Delete();
                counter++;
            }
            SEnvir.Log($"[账号清理] 角色[{info.CharacterName}]的{counter}个自动战斗信息已删除");
        }

        //Magics
        private void DeleteCharacterLinkedMagics(CharacterInfo info)
        {
            List<UserMagic> magics = SEnvir.UserMagicList.Binding.Where(x => x.Character?.Index == info.Index).ToList();
            int counter = 0;
            foreach (var magic in magics)
            {
                magic.Delete();
                counter++;
            }
            SEnvir.Log($"[账号清理] 角色[{info.CharacterName}]的{counter}个魔法信息已删除");
        }

        // Buffs
        private void DeleteCharacterLinkedBuffs(CharacterInfo info)
        {
            List<BuffInfo> buffs = SEnvir.BuffInfoList.Binding.Where(x => x.Character?.Index == info.Index).ToList();
            int counter = 0;
            foreach (var buff in buffs)
            {
                buff.Delete();
                counter++;
            }
            SEnvir.Log($"[账号清理] 角色[{info.CharacterName}]的{counter}个Buff信息已删除");
        }

        // Refines
        private void DeleteCharacterLinkedRefines(CharacterInfo info)
        {
            List<RefineInfo> refines = SEnvir.RefineInfoList.Binding.Where(x => x.Character?.Index == info.Index).ToList();
            int counter = 0;
            foreach (var userItem in refines)
            {
                userItem.Delete();
                counter++;
            }
            SEnvir.Log($"[账号清理] 角色[{info.CharacterName}]的{counter}个精炼信息已删除");
        }

        // Quests
        private void DeleteCharacterLinkedQuests(CharacterInfo info)
        {
            List<UserQuest> userQuests = SEnvir.UserQuestList.Binding.Where(x => x.Character?.Index == info.Index).ToList();
            int counter = 0;
            foreach (var userItem in userQuests)
            {
                userItem.Delete();
                counter++;
            }
            SEnvir.Log($"[账号清理] 角色[{info.CharacterName}]的{counter}个任务信息已删除");
        }

        // Friends
        private void DeleteCharacterLinkedFriends(CharacterInfo info)
        {
            List<FriendInfo> userItems = SEnvir.FriendsList.Binding.Where(x => x.Character?.Index == info.Index).ToList();
            int counter = 0;
            foreach (var userItem in userItems)
            {
                userItem.Delete();
                counter++;
            }
            SEnvir.Log($"[账号清理] 角色[{info.CharacterName}]的{counter}个好友信息已删除");
        }

        // FPointLinks
        private void DeleteCharacterLinkedFPointLinks(CharacterInfo info)
        {
            List<FixedPointInfo> userItems = SEnvir.FixedPointInfoList.Binding.Where(x => x.Character?.Index == info.Index).ToList();
            int counter = 0;
            foreach (var userItem in userItems)
            {
                userItem.Delete();
                counter++;
            }
            SEnvir.Log($"[账号清理] 角色[{info.CharacterName}]的{counter}个定点传送信息已删除");
        }

        // Values
        private void DeleteCharacterLinkedValues(CharacterInfo info)
        {
            List<UserValue> userItems = SEnvir.UserValueList.Binding.Where(x => x.Character?.Index == info.Index).ToList();
            int counter = 0;
            foreach (var userItem in userItems)
            {
                userItem.Delete();
                counter++;
            }
            SEnvir.Log($"[账号清理] 角色[{info.CharacterName}]的{counter}个个人变量已删除");
        }

        // Achievements
        private void DeleteCharacterLinkedAchievements(CharacterInfo info)
        {
            List<UserAchievement> userAchievements = SEnvir.UserAchievementList.Binding.Where(x => x.Character?.Index == info.Index).ToList();
            int counter = 0;
            foreach (var userItem in userAchievements)
            {
                userItem.Delete();
                counter++;
            }
            SEnvir.Log($"[账号清理] 角色[{info.CharacterName}]的{counter}个成就信息已删除");
        }

        private void DeleteRankingNodes(CharacterInfo info)
        {
            try
            {
                SEnvir.Rankings.Remove(info.RankingNode);
                info.RankingNode = null;
                SEnvir.Rankings.Remove(info);
            }
            catch { }
        }

        #endregion

        #endregion

        private bool DeleteSingleAccount(AccountInfo info)
        {
            try
            {
                SEnvir.Log($"[账号清理] 正在清理账号[{info.EMailAddress}]...");

                // Guilds
                DeleteLinkedGuildInfo(info);
                // UserItems
                DeleteLinkedUserItem(info);
                // Buffs
                DeleteLinkedBuffs(info);
                // Auctions
                DeleteLinkedAuctions(info);
                // Mail
                DeleteLinkedMail(info);
                // UserDrops
                DeleteLinkedUserDrops(info);
                // Companions
                DeleteLinkedCompanions(info);
                // CompanionUnlocks
                DeleteLinkedCompanionUnlocks(info);
                // BlockingList
                DeleteLinkedBlockingList(info);
                // BlockedByList
                DeleteLinkedBlockedByList(info);
                // Payments
                DeleteLinkedPayments(info);
                // StoreSales
                DeleteLinkedStoreSales(info);
                // Fortunes
                DeleteLinkedFortunes(info);
                // CharacterShop
                DeleteLinkedCharacterShop(info);

                // Characters
                DeleteLinkedCharacter(info);

                SEnvir.Log($"[账号清理] 账号[{info.EMailAddress}]清理完成");
                info.Delete();
                return true;
            }
            catch (Exception e)
            {
                SEnvir.Log($"[账号清理] 账号[{info.EMailAddress}]清理失败: {e.Message}");
                return false;
            }
        }

    }
}