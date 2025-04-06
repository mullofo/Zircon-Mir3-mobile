using Server.DBModels;
using Server.Envir;
using System.Collections.Generic;


namespace Server.Models
{
    public partial class PlayerObject : MapObject //玩家变量
    {

        public Dictionary<int, object> TempValue = new Dictionary<int, object>();
        private Dictionary<int, UserValue> Values = new Dictionary<int, UserValue>();
        public void SetValues(int key, int value)
        {
            UserValue ob;
            if (Values.TryGetValue(key, out ob))
            {
                ob.Key = key;
                ob.Value = value;

            }
            else
            {
                ob = SEnvir.UserValueList.CreateNewObject();
                ob.Character = Character;
                ob.Key = key;
                ob.Value = value;
                Values[key] = ob;
            }
        }
        public int GetValues(int key)
        {
            UserValue ob;
            int ret = 0;
            if (Values.TryGetValue(key, out ob))
            {
                ret = ob.Value;

            }
            return ret;
        }

        public object GetObValues(int key)
        {
            UserValue ob;
            object ret = null;
            if (Values.TryGetValue(key, out ob))
            {
                ret = ob.ObjctValue;

            }
            return ret;
        }

        public void SetObValues(int key, object value)
        {
            UserValue ob;
            if (Values.TryGetValue(key, out ob))
            {
                ob.Key = key;
                ob.ObjctValue = value;

            }
            else
            {
                ob = SEnvir.UserValueList.CreateNewObject();
                ob.Character = Character;
                ob.Key = key;
                ob.ObjctValue = value;
                Values[key] = ob;
            }
        }
    }
}
