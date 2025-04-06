using Library;
using MirDB;

namespace Server.DBModels
{
    /// <summary>
    /// 寄售信息
    /// </summary>
    [UserObject]
    public sealed class AuctionInfo : DBObject
    {
        [Association("Auctions")]
        public AccountInfo Account   //账号
        {
            get { return _Account; }
            set
            {
                if (_Account == value) return;

                var oldValue = _Account;
                _Account = value;

                OnChanged(oldValue, value, "Account");
            }
        }
        private AccountInfo _Account;

        [Association("Auction")]
        public UserItem Item   //道具
        {
            get { return _Item; }
            set
            {
                if (_Item == value) return;

                var oldValue = _Item;
                _Item = value;

                OnChanged(oldValue, value, "Item");
            }
        }
        private UserItem _Item;

        public CharacterInfo Character   //角色
        {
            get { return _Character; }
            set
            {
                if (_Character == value) return;

                var oldValue = _Character;
                _Character = value;

                OnChanged(oldValue, value, "Character");
            }
        }
        private CharacterInfo _Character;

        public int Price   //价格
        {
            get { return _Price; }
            set
            {
                if (_Price == value) return;

                var oldValue = _Price;
                _Price = value;

                OnChanged(oldValue, value, "Price");
            }
        }
        private int _Price;

        public CurrencyType PriceType   //价格类型
        {
            get { return _PriceType; }
            set
            {
                if (_PriceType == value) return;

                var oldValue = _PriceType;
                _PriceType = value;

                OnChanged(oldValue, value, "PriceType");
            }
        }

        private CurrencyType _PriceType;

        public string Message   //说明
        {
            get { return _Message; }
            set
            {
                if (_Message == value) return;

                var oldValue = _Message;
                _Message = value;

                OnChanged(oldValue, value, "Message");
            }
        }
        private string _Message;

        protected override internal void OnDeleted()  //删除时
        {
            Account = null;
            Item = null;
            Character = null;

            base.OnDeleted();
        }

#if !ServerTool
        public ClientMarketPlaceInfo ToClientInfo(AccountInfo account)
        {
            return new ClientMarketPlaceInfo  //客户寄售信息
            {
                Index = Index,

                Item = Item?.ToClientInfo(),

                Seller = Character?.CharacterName,

                Message = Message,

                IsOwner = account == Account,

                Price = Price,
                PriceType = PriceType,
                CreatTime = CreatTime,
            };
        }
#endif


        public override string ToString()
        {
            return Account?.ToString() ?? string.Empty;
        }

    }
}
