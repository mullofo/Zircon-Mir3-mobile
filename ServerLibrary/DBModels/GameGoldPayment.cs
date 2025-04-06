using MirDB;

namespace Server.DBModels
{
    /// <summary>
    /// 游戏元宝充值支付记录
    /// </summary>
    [UserObject]
    public sealed class GameGoldPayment : DBObject
    {
        /// <summary>
        /// 原始邮件信息
        /// </summary>
        public string RawMessage
        {
            get { return _RawMessage; }
            set
            {
                if (_RawMessage == value) return;

                var oldValue = _RawMessage;
                _RawMessage = value;

                OnChanged(oldValue, value, "RawMessage");
            }
        }
        private string _RawMessage;
        /// <summary>
        /// 角色名字
        /// </summary>
        public string CharacterName
        {
            get { return _CharacterName; }
            set
            {
                if (_CharacterName == value) return;

                var oldValue = _CharacterName;
                _CharacterName = value;

                OnChanged(oldValue, value, "CharacterName");
            }
        }
        private string _CharacterName;
        /// <summary>
        /// 充值人名字
        /// </summary>
        public string Name
        {
            get { return _Name; }
            set
            {
                if (_Name == value) return;

                var oldValue = _Name;
                _Name = value;

                OnChanged(oldValue, value, "Name");
            }
        }
        private string _Name;
        /// <summary>
        /// 充值数据
        /// </summary>
        public string PaymentDate
        {
            get { return _PaymentDate; }
            set
            {
                if (_PaymentDate == value) return;

                var oldValue = _PaymentDate;
                _PaymentDate = value;

                OnChanged(oldValue, value, "PaymentDate");
            }
        }
        private string _PaymentDate;
        /// <summary>
        /// 账号
        /// </summary>
        [Association("Payments")]
        public AccountInfo Account
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
        /// <summary>
        /// 充值交易ID
        /// </summary>
        public string TransactionID
        {
            get { return _TransactionID; }
            set
            {
                if (_TransactionID == value) return;

                var oldValue = _TransactionID;
                _TransactionID = value;

                OnChanged(oldValue, value, "TransactionID");
            }
        }
        private string _TransactionID;
        /// <summary>
        /// 充值交易类型
        /// </summary>
        public string TransactionType
        {
            get { return _TransactionType; }
            set
            {
                if (_TransactionType == value) return;

                var oldValue = _TransactionType;
                _TransactionType = value;

                OnChanged(oldValue, value, "TransactionType");
            }
        }
        private string _TransactionType;
        /// <summary>
        /// 充值状态
        /// </summary>
        public string Status
        {
            get { return _Status; }
            set
            {
                if (_Status == value) return;

                var oldValue = _Status;
                _Status = value;

                OnChanged(oldValue, value, "Status");
            }
        }
        private string _Status;
        /// <summary>
        /// 充值元宝数量
        /// </summary>
        public int GameGoldAmount
        {
            get { return _GameGoldAmount; }
            set
            {
                if (_GameGoldAmount == value) return;

                var oldValue = _GameGoldAmount;
                _GameGoldAmount = value;

                OnChanged(oldValue, value, "GameGoldAmount");
            }
        }
        private int _GameGoldAmount;
        /// <summary>
        /// 收件人邮件
        /// </summary>
        public string Receiver_EMail
        {
            get { return _Receiver_EMail; }
            set
            {
                if (_Receiver_EMail == value) return;

                var oldValue = _Receiver_EMail;
                _Receiver_EMail = value;

                OnChanged(oldValue, value, "Receiver_EMail");
            }
        }
        private string _Receiver_EMail;
        /// <summary>
        /// 充值玩家邮件
        /// </summary>
        public string Payer_EMail
        {
            get { return _Payer_EMail; }
            set
            {
                if (_Payer_EMail == value) return;

                var oldValue = _Payer_EMail;
                _Payer_EMail = value;

                OnChanged(oldValue, value, "Payer_EMail");
            }
        }
        private string _Payer_EMail;
        /// <summary>
        /// 充值玩家ID
        /// </summary>
        public string Payer_ID
        {
            get { return _Payer_ID; }
            set
            {
                if (_Payer_ID == value) return;

                var oldValue = _Payer_ID;
                _Payer_ID = value;

                OnChanged(oldValue, value, "Payer_ID");
            }
        }
        private string _Payer_ID;
        /// <summary>
        /// 充值金额
        /// </summary>
        public decimal Price
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
        private decimal _Price;
        /// <summary>
        /// 货币
        /// </summary>
        public string Currency
        {
            get { return _Currency; }
            set
            {
                if (_Currency == value) return;

                var oldValue = _Currency;
                _Currency = value;

                OnChanged(oldValue, value, "Currency");
            }
        }
        private string _Currency;
        /// <summary>
        /// 费用
        /// </summary>
        public decimal Fee
        {
            get { return _Fee; }
            set
            {
                if (_Fee == value) return;

                var oldValue = _Fee;
                _Fee = value;

                OnChanged(oldValue, value, "Fee");
            }
        }
        private decimal _Fee;
        /// <summary>
        /// 错误信息记录
        /// </summary>
        public bool Error
        {
            get { return _Error; }
            set
            {
                if (_Error == value) return;

                var oldValue = _Error;
                _Error = value;

                OnChanged(oldValue, value, "Error");
            }
        }
        private bool _Error;

        /// <summary>
        /// 删除时
        /// </summary>
        protected override internal void OnDeleted()
        {
            Account = null;

            base.OnDeleted();
        }

    }
}
