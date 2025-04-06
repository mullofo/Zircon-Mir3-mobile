using Library;
using MirDB;
using Server.Envir;
using System;
using System.Linq;

namespace Server.DBModels
{
    [UserObject]
    public sealed class MailInfo : DBObject   //邮件信息
    {
        [Association("Mail")]
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

        public string Sender   //发件人
        {
            get { return _Sender; }
            set
            {
                if (_Sender == value) return;

                var oldValue = _Sender;
                _Sender = value;

                OnChanged(oldValue, value, "Sender");
            }
        }
        private string _Sender;


        public DateTime Date   //发件时间
        {
            get { return _Date; }
            set
            {
                if (_Date == value) return;

                var oldValue = _Date;
                _Date = value;

                OnChanged(oldValue, value, "Date");
            }
        }
        private DateTime _Date;

        public string Subject   //邮件主题
        {
            get { return _Subject; }
            set
            {
                if (_Subject == value) return;

                var oldValue = _Subject;
                _Subject = value;

                OnChanged(oldValue, value, "Subject");
            }
        }
        private string _Subject;

        public string Message    //邮件内容
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

        public bool Opened   //是否阅读
        {
            get { return _Opened; }
            set
            {
                if (_Opened == value) return;

                var oldValue = _Opened;
                _Opened = value;

                OnChanged(oldValue, value, "Opened");
            }
        }
        private bool _Opened;

        public bool HasItem   //是否有道具
        {
            get { return _HasItem; }
            set
            {
                if (_HasItem == value) return;

                var oldValue = _HasItem;
                _HasItem = value;

                OnChanged(oldValue, value, "HasItem");
            }
        }
        private bool _HasItem;


        [Association("Mail")]   //邮件道具
        public DBBindingList<UserItem> Items { get; set; }

        protected override internal void OnDeleted()   //删除时
        {
            Account = null;

            base.OnDeleted();
        }

        protected override internal void OnCreated()   //创建时
        {
            base.OnCreated();

            Date = SEnvir.Now;
        }

#if !ServerTool
        public ClientMailInfo ToClientInfo()    //更新到客户端信息
        {
            return new ClientMailInfo
            {
                Index = Index,
                Subject = Subject,
                Sender = Sender,
                Date = Date,
                Message = Message,
                HasItem = HasItem,
                Opened = Opened,
                Items = Items.Select(x => x.ToClientInfo()).ToList()
            };
        }
#endif

        public override string ToString()
        {
            return Account?.EMailAddress ?? string.Empty;
        }
    }
}
