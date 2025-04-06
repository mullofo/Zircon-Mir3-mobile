using MirDB;

namespace Server.DBModels
{
    /// <summary>
    /// 游戏币购买
    /// </summary>
    [UserObject]
    public class GameGoldSet : DBObject
    {
        /// <summary>
        /// 价格
        /// </summary>
        public decimal Price
        {
            get { return _price; }
            set
            {
                if (_price == value) return;

                var oldValue = _price;
                _price = value;

                OnChanged(oldValue, value, "price");
            }
        }
        private decimal _price;
        /// <summary>
        /// 元宝数量
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
    }
}
