using Library;
using MirDB;
using Server.Envir;
using System;

namespace Server.DBModels
{
    [UserObject]
    public sealed class FriendInfo : DBObject
    {

        [Association("Friends")]
        public CharacterInfo Character
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

        [Association("Friends")]
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

        [Association("Friends")]
        public DateTime AddDate
        {
            get { return _AddDate; }
            set
            {
                if (_AddDate == value) return;

                var oldValue = _AddDate;
                _AddDate = value;

                OnChanged(oldValue, value, "AddDate");
            }
        }
        private DateTime _AddDate;

        [Association("Friends")]
        public string EMailAddress
        {
            get { return _EMailAddress; }
            set
            {
                if (_EMailAddress == value) return;

                var oldValue = _EMailAddress;
                _EMailAddress = value;

                OnChanged(oldValue, value, "EMailAddress");
            }
        }
        private string _EMailAddress;

        [Association("Friends")]
        public string LinkID
        {
            get { return _LinkID; }
            set
            {
                if (_LinkID == value) return;

                var oldValue = _LinkID;
                _LinkID = value;

                OnChanged(oldValue, value, "LinkID");
            }
        }
        private string _LinkID;


        protected override internal void OnCreated()
        {
            base.OnCreated();
        }
        protected override internal void OnLoaded()
        {
            base.OnLoaded();
        }

#if !ServerTool
        public ClientFriendInfo ToClientInfo()
        {
            return new ClientFriendInfo
            {
                Index = Index,
                AddDate = AddDate,
                Character = Character.CharacterName,
                Name = Name,
                LinkID = LinkID,
                Online = SEnvir.GetCharacter(Name)?.Player != null,
            };
        }
#endif
    }
}
