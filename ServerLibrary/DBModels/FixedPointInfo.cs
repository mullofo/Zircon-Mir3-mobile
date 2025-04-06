using Library;
using MirDB;
using System.Drawing;

namespace Server.DBModels
{
    [UserObject]
    public class FixedPointInfo : DBObject
    {
        [Association("FixedPoints")]
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

        public Color NameColour
        {
            get { return _NameColour; }
            set
            {
                if (_NameColour == value) return;

                var oldValue = _NameColour;
                _NameColour = value;

                OnChanged(oldValue, value, "NameColor");
            }
        }
        private Color _NameColour;

        public int QuestIndex
        {
            get { return _QuestIndex; }
            set
            {
                if (_QuestIndex == value) return;

                var oldValue = _QuestIndex;
                _QuestIndex = value;

                OnChanged(oldValue, value, "QuestIndex");
            }
        }
        private int _QuestIndex;

        public Point Pos
        {
            get { return _Pos; }
            set
            {
                if (_Pos == value) return;

                var oldValue = _Pos;
                _Pos = value;

                OnChanged(oldValue, value, "Pos");
            }
        }

        private Point _Pos { get; set; }

        protected override internal void OnDeleted()
        {
            Character = null;
            base.OnDeleted();
        }

#if !ServerTool
        public ClientFixedPointInfo ToClientInfo()
        {
            return new ClientFixedPointInfo
            {
                Uind = new FixeUnit(QuestIndex, Pos),
                Name = Name,
                NameColour = NameColour,
            };

        }
#endif

    };
}
