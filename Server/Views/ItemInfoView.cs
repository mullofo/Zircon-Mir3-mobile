using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using Library;
using Library.SystemModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Windows.Forms;

namespace Server.Views
{
    public partial class ItemInfoView : DevExpress.XtraBars.Ribbon.RibbonForm
    {

        private WaitDialogForm wdf = null;
        public ItemInfoView()         //道具信息视图
        {
            InitializeComponent();

            ItemInfoGridControl.DataSource = SMain.Session.GetCollection<ItemInfo>().Binding;
            MonsterLookUpEdit.DataSource = SMain.Session.GetCollection<MonsterInfo>().Binding;
            SetLookUpEdit.DataSource = SMain.Session.GetCollection<SetInfo>().Binding;

            ItemTypeImageComboBox.Items.AddEnum<ItemType>();
            RequiredClassImageComboBox.Items.AddEnum<RequiredClass>();
            RequiredGenderImageComboBox.Items.AddEnum<RequiredGender>();
            StatImageComboBox.Items.AddEnum<Stat>();
            RequiredTypeImageComboBox.Items.AddEnum<RequiredType>();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SMain.SetUpView(ItemInfoGridView);
            SMain.SetUpView(ItemStatsGridView);
            SMain.SetUpView(DropsGridView);
        }

        private void SaveButton_ItemClick(object sender, ItemClickEventArgs e)    //保存
        {
            SMain.Session.Save(true, MirDB.SessionMode.Server);
        }

        private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)  //导出
        {
            try
            {
                Helpers.HelperExcel<ItemInfo>.ExportExcel(true);

            }
            catch (Exception ex)
            {
                MessageBox.Show("操作失败" + ex.Message, "提示");
            }
        }

        private void barButtonItem2_ItemClick(object sender, ItemClickEventArgs e)  //导入
        {
            try
            {
                Helpers.HelperExcel<ItemInfo>.ImportExcel(true);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("操作失败" + ex.Message, "提示");
            }
        }

        /*private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)    //导出
        {
            ItemInfoGridView.OptionsBehavior.AutoPopulateColumns = false;

            ExportImportHelp.ExportExcel(this.Text, ItemInfoGridView);
        }

        private void barButtonItem2_ItemClick_1(object sender, ItemClickEventArgs e)   //导入
        {
            try
            {
                DataTable dt = null;
                ExportImportHelp.ImportExcel(ItemInfoGridView, ref dt);
                if (dt != null && dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        ItemInfo ItemInfo = ItemInfoGridView.GetRow(i) as ItemInfo;
                        DataRow DataRow = dt.Rows[i];
                        ItemInfo.ItemType = ExportImportHelp.GetEnumName<ItemType>(Convert.ToString(DataRow["ItemType"]));
                        ItemInfo.RequiredClass = ExportImportHelp.GetEnumName<RequiredClass>(Convert.ToString(DataRow["RequiredClass"]));
                        ItemInfo.RequiredGender = ExportImportHelp.GetEnumName<RequiredGender>(Convert.ToString(DataRow["RequiredGender"]));
                        ItemInfo.RequiredType = ExportImportHelp.GetEnumName<RequiredType>(Convert.ToString(DataRow["RequiredType"]));
                        ItemInfo.Effect = (ItemEffect)Enum.Parse(typeof(ItemEffect), Convert.ToString(DataRow["Effect"]));
                        ItemInfo.Rarity = ExportImportHelp.GetEnumName<Rarity>(Convert.ToString(DataRow["Rarity"]));
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("操作失败" + ex.Message, "提示");

            }
            return;
        }*/

        private void OpenWaitDialog(string caption)
        {
            wdf = new WaitDialogForm(caption + "...", "请等待...");
            this.Cursor = Cursors.WaitCursor;
        }
        private void CloseWaitDialog()
        {
            if (wdf != null)
            {
                wdf.Close();
            }

            this.Cursor = Cursors.Default;
        }

        int[] SelectRows = null;
        private void ItemInfoGridView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.C)
            {
                SelectRows = this.ItemInfoGridView.GetSelectedRows();


                e.Handled = true;
                List<ItemInfo> ItemInfoList = new List<ItemInfo>();
                IList<ItemInfoCopy> ItemInfoCopyList = new List<ItemInfoCopy>();
                foreach (int RowIndex in SelectRows)
                {
                    ItemInfo ItemInfo = ItemInfoGridView.GetRow(RowIndex) as ItemInfo;
                    ItemInfo.Drops.ToList().ForEach(o => o.StrMonsterName = o.Monster.MonsterName);//用于对应新对象中的StrMonsterName字段
                    ItemInfoList.Add(ItemInfo);
                }

                ItemInfoCopyList = TypeUtil.CopyList<ItemInfo, ItemInfoCopy>(ItemInfoList);
                for (int i = 0; i < ItemInfoCopyList.Count; i++)
                {
                    ItemInfoCopyList[i].ItemStats = TypeUtil.CopyList<ItemInfoStat, ItemInfoStatCopy>(ItemInfoList.FirstOrDefault(o => o.ItemName == ItemInfoCopyList[i].ItemName).ItemStats);
                    ItemInfoCopyList[i].Drops = TypeUtil.CopyList<DropInfo, DropInfoCopy>(ItemInfoList.FirstOrDefault(o => o.ItemName == ItemInfoCopyList[i].ItemName).Drops);
                    //ItemInfoCopyList[i].Set = TypeUtil.Copy<SetInfo, SetInfoCopy>(ItemInfoList.FirstOrDefault(o => o.ItemName == ItemInfoCopyList[i].ItemName).Set);
                }

                string strJson = JsonHelper.SerializeObject(ItemInfoCopyList);
                Clipboard.SetDataObject(strJson, true);

            }
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.V)
            {
                if (SelectRows != null)
                {
                    foreach (int RowIndex in SelectRows)
                    {
                        ItemInfo ItemInfo = ItemInfoGridView.GetRow(RowIndex) as ItemInfo;
                        ItemInfo ItemInfoTemp = SMain.Session.GetCollection<ItemInfo>().CreateNewObject();

                        ItemInfoTemp.ItemName = ItemInfo.ItemName;
                        ItemInfoTemp.ItemType = ItemInfo.ItemType;
                        ItemInfoTemp.WeaponType = ItemInfo.WeaponType;
                        ItemInfoTemp.RequiredClass = ItemInfo.RequiredClass;
                        ItemInfoTemp.RequiredGender = ItemInfo.RequiredGender;
                        ItemInfoTemp.RequiredType = ItemInfo.RequiredType;
                        ItemInfoTemp.RequiredAmount = ItemInfo.RequiredAmount;
                        ItemInfoTemp.Shape = ItemInfo.Shape;
                        ItemInfoTemp.Effect = ItemInfo.Effect;
                        ItemInfoTemp.Image = ItemInfo.Image;
                        ItemInfoTemp.Durability = ItemInfo.Durability;
                        ItemInfoTemp.Price = ItemInfo.Price;
                        ItemInfoTemp.Weight = ItemInfo.Weight;
                        ItemInfoTemp.StackSize = ItemInfo.StackSize;
                        ItemInfoTemp.StartItem = ItemInfo.StartItem;
                        ItemInfoTemp.SellRate = ItemInfo.SellRate;
                        ItemInfoTemp.CanRepair = ItemInfo.CanRepair;
                        ItemInfoTemp.CanSell = ItemInfo.CanSell;
                        ItemInfoTemp.CanStore = ItemInfo.CanStore;
                        ItemInfoTemp.CanTreasure = ItemInfo.CanTreasure;
                        ItemInfoTemp.CanTrade = ItemInfo.CanTrade;
                        ItemInfoTemp.NoMake = ItemInfo.NoMake;
                        ItemInfoTemp.CanDrop = ItemInfo.CanDrop;
                        ItemInfoTemp.CanDeathDrop = ItemInfo.CanDeathDrop;
                        ItemInfoTemp.Description = ItemInfo.Description;
                        ItemInfoTemp.Rarity = ItemInfo.Rarity;
                        ItemInfoTemp.CanAutoPot = ItemInfo.CanAutoPot;
                        ItemInfoTemp.BuffIcon = ItemInfo.BuffIcon;
                        ItemInfoTemp.PartCount = ItemInfo.PartCount;
                        //ItemInfoTemp.Set = ItemInfo.Set;
                        ItemInfoTemp.ItemTypeName = ItemInfo.ItemTypeName;
                        ItemInfoTemp.RequiredClassName = ItemInfo.RequiredClassName;
                        ItemInfoTemp.RequiredGenderName = ItemInfo.RequiredGenderName;
                        ItemInfoTemp.RequiredTypeName = ItemInfo.RequiredTypeName;
                        ItemInfoTemp.EffectName = ItemInfo.EffectName;

                        foreach (ItemInfoStat ItemInfoStat in ItemInfo.ItemStats)
                        {
                            ItemInfoStat ItemInfoStatTemp = ItemInfoTemp.ItemStats.AddNew();
                            ItemInfoStatTemp.Item = ItemInfoTemp;
                            ItemInfoStatTemp.Stat = ItemInfoStat.Stat;
                            ItemInfoStatTemp.Amount = ItemInfoStat.Amount;
                        }
                        foreach (DropInfo DropInfo in ItemInfo.Drops)
                        {
                            DropInfo DropInfoTemp = ItemInfoTemp.Drops.AddNew();
                            DropInfoTemp.Item = ItemInfoTemp;
                            DropInfoTemp.Monster = DropInfo.Monster;
                            DropInfoTemp.Chance = DropInfo.Chance;
                            DropInfoTemp.Amount = DropInfo.Amount;
                            DropInfoTemp.DropSet = DropInfo.DropSet;
                            DropInfoTemp.PartOnly = DropInfo.PartOnly;
                            DropInfoTemp.EasterEvent = DropInfo.EasterEvent;

                        }
                    }
                }
                else
                {
                    IDataObject iData = Clipboard.GetDataObject();
                    if (iData.GetDataPresent(DataFormats.Text))
                    {
                        string strJson = (string)iData.GetData(DataFormats.Text);
                        List<ItemInfoCopy> ItemInfoCopyList = null;
                        try
                        {
                            ItemInfoCopyList = JsonHelper.DeserializeJsonToList<ItemInfoCopy>(strJson);
                        }
                        catch (System.Exception)
                        {
                            XtraMessageBox.Show("复制内容错误，请确认复制来源理否正确！", "错误");
                            return;
                        }

                        IList<MonsterInfo> MonsterInfoList = SMain.Session.GetCollection<MonsterInfo>().Binding;
                        IList<SetInfo> SetInfoList = SMain.Session.GetCollection<SetInfo>().Binding;

                        for (int i = 0; i < ItemInfoCopyList.Count; i++)
                        {
                            ItemInfoCopy ItemInfo = ItemInfoCopyList[i];
                            ItemInfo ItemInfoTemp = SMain.Session.GetCollection<ItemInfo>().CreateNewObject();

                            ItemInfoTemp.ItemName = ItemInfo.ItemName;
                            ItemInfoTemp.ItemType = ItemInfo.ItemType;
                            ItemInfoTemp.WeaponType = ItemInfo.WeaponType;
                            ItemInfoTemp.RequiredClass = ItemInfo.RequiredClass;
                            ItemInfoTemp.RequiredGender = ItemInfo.RequiredGender;
                            ItemInfoTemp.RequiredType = ItemInfo.RequiredType;
                            ItemInfoTemp.RequiredAmount = ItemInfo.RequiredAmount;
                            ItemInfoTemp.Shape = ItemInfo.Shape;
                            ItemInfoTemp.Effect = ItemInfo.Effect;
                            ItemInfoTemp.Image = ItemInfo.Image;
                            ItemInfoTemp.Durability = ItemInfo.Durability;
                            ItemInfoTemp.Price = ItemInfo.Price;
                            ItemInfoTemp.Weight = ItemInfo.Weight;
                            ItemInfoTemp.StackSize = ItemInfo.StackSize;
                            ItemInfoTemp.StartItem = ItemInfo.StartItem;
                            ItemInfoTemp.SellRate = ItemInfo.SellRate;
                            ItemInfoTemp.CanRepair = ItemInfo.CanRepair;
                            ItemInfoTemp.CanSell = ItemInfo.CanSell;
                            ItemInfoTemp.CanStore = ItemInfo.CanStore;
                            ItemInfoTemp.CanTreasure = ItemInfo.CanTreasure;
                            ItemInfoTemp.CanTrade = ItemInfo.CanTrade;
                            ItemInfoTemp.NoMake = ItemInfo.NoMake;
                            ItemInfoTemp.CanDrop = ItemInfo.CanDrop;
                            ItemInfoTemp.CanDeathDrop = ItemInfo.CanDeathDrop;
                            ItemInfoTemp.Description = ItemInfo.Description;
                            ItemInfoTemp.Rarity = ItemInfo.Rarity;
                            ItemInfoTemp.CanAutoPot = ItemInfo.CanAutoPot;
                            ItemInfoTemp.BuffIcon = ItemInfo.BuffIcon;
                            ItemInfoTemp.PartCount = ItemInfo.PartCount;
                            if (ItemInfo.Set == null)
                            {
                                //ItemInfoTemp.Set = null;
                            }
                            else
                            {
                                //ItemInfoTemp.Set = SetInfoList.FirstOrDefault(o => o.SetName == ItemInfo.Set.SetName);
                            }

                            foreach (ItemInfoStatCopy ItemInfoStatCopy in ItemInfo.ItemStats)
                            {
                                ItemInfoStat ItemInfoStatTemp = ItemInfoTemp.ItemStats.AddNew();
                                ItemInfoStatTemp.Item = ItemInfoTemp;
                                ItemInfoStatTemp.Stat = ItemInfoStatCopy.Stat;
                                ItemInfoStatTemp.Amount = ItemInfoStatCopy.Amount;
                            }
                            foreach (DropInfoCopy DropInfoCopy in ItemInfo.Drops)
                            {
                                DropInfo DropInfoTemp = ItemInfoTemp.Drops.AddNew();
                                DropInfoTemp.Item = ItemInfoTemp;
                                DropInfoTemp.Monster = MonsterInfoList.FirstOrDefault(o => o.MonsterName == DropInfoCopy.StrMonsterName);
                                DropInfoTemp.Chance = DropInfoCopy.Chance;
                                DropInfoTemp.Amount = DropInfoCopy.Amount;
                                DropInfoTemp.DropSet = DropInfoCopy.DropSet;
                                DropInfoTemp.PartOnly = DropInfoCopy.PartOnly;
                                DropInfoTemp.EasterEvent = DropInfoCopy.EasterEvent;
                            }
                        }
                    }
                    else
                    {
                        XtraMessageBox.Show("目前剪贴板中数据不可转换为文本", "错误");
                        return;
                    }
                }
                XtraMessageBox.Show("复制成功，请到最后一行查看。", "提示");
            }
        }

        public class DataTableEntityBuilder<Entity>
        {
            private static readonly MethodInfo getValueMethod = typeof(DataRow).GetMethod("get_Item", new Type[] { typeof(int) });
            private static readonly MethodInfo isDBNullMethod = typeof(DataRow).GetMethod("IsNull", new Type[] { typeof(int) });
            private delegate Entity Load(DataRow dataRecord);
            private Load handler;
            private DataTableEntityBuilder() { }
            public Entity Build(DataRow dataRecord)
            {
                return handler(dataRecord);
            }
            public static DataTableEntityBuilder<Entity> CreateBuilder(DataRow dataRecord)
            {
                DataTableEntityBuilder<Entity> dynamicBuilder = new DataTableEntityBuilder<Entity>();
                DynamicMethod method = new DynamicMethod("DynamicCreateEntity", typeof(Entity), new Type[] { typeof(DataRow) }, typeof(Entity), true);
                ILGenerator generator = method.GetILGenerator();
                LocalBuilder result = generator.DeclareLocal(typeof(Entity));
                generator.Emit(OpCodes.Newobj, typeof(Entity).GetConstructor(Type.EmptyTypes));
                generator.Emit(OpCodes.Stloc, result);
                for (int i = 0; i < dataRecord.ItemArray.Length; i++)
                {
                    PropertyInfo propertyInfo = typeof(Entity).GetProperty(dataRecord.Table.Columns[i].ColumnName);
                    System.Reflection.Emit.Label endIfLabel = generator.DefineLabel();
                    if (propertyInfo != null && propertyInfo.GetSetMethod() != null)
                    {
                        generator.Emit(OpCodes.Ldarg_0);
                        generator.Emit(OpCodes.Ldc_I4, i);
                        generator.Emit(OpCodes.Callvirt, isDBNullMethod);
                        generator.Emit(OpCodes.Brtrue, endIfLabel);
                        generator.Emit(OpCodes.Ldloc, result);
                        generator.Emit(OpCodes.Ldarg_0);
                        generator.Emit(OpCodes.Ldc_I4, i);
                        generator.Emit(OpCodes.Callvirt, getValueMethod);
                        generator.Emit(OpCodes.Unbox_Any, propertyInfo.PropertyType);
                        generator.Emit(OpCodes.Callvirt, propertyInfo.GetSetMethod());
                        generator.MarkLabel(endIfLabel);
                    }
                }
                generator.Emit(OpCodes.Ldloc, result);
                generator.Emit(OpCodes.Ret);
                dynamicBuilder.handler = (Load)method.CreateDelegate(typeof(Load));
                return dynamicBuilder;
            }
        }
    }

    public class ItemInfoCopy
    {
        public string ItemName
        {
            get { return _ItemName; }
            set
            {
                if (_ItemName == value) return;

                var oldValue = _ItemName;
                _ItemName = value;


            }
        }
        private string _ItemName;

        public ItemType ItemType
        {
            get { return _ItemType; }
            set
            {
                if (_ItemType == value) return;

                var oldValue = _ItemType;
                _ItemType = value;


            }
        }
        private ItemType _ItemType;

        public WeaponType WeaponType
        {
            get { return _WeaponType; }
            set
            {
                if (_WeaponType == value) return;

                var oldValue = _WeaponType;
                _WeaponType = value;


            }
        }
        private WeaponType _WeaponType;

        public RequiredClass RequiredClass
        {
            get { return _RequiredClass; }
            set
            {
                if (_RequiredClass == value) return;

                var oldValue = _RequiredClass;
                _RequiredClass = value;


            }
        }
        private RequiredClass _RequiredClass;

        public RequiredGender RequiredGender
        {
            get { return _RequiredGender; }
            set
            {
                if (_RequiredGender == value) return;

                var oldValue = _RequiredGender;
                _RequiredGender = value;


            }
        }
        private RequiredGender _RequiredGender;

        public RequiredType RequiredType
        {
            get { return _RequiredType; }
            set
            {
                if (_RequiredType == value) return;

                var oldValue = _RequiredType;
                _RequiredType = value;


            }
        }
        private RequiredType _RequiredType;

        public int RequiredAmount
        {
            get { return _RequiredAmount; }
            set
            {
                if (_RequiredAmount == value) return;

                var oldValue = _RequiredAmount;
                _RequiredAmount = value;


            }
        }
        private int _RequiredAmount;

        public int Shape
        {
            get { return _Shape; }
            set
            {
                if (_Shape == value) return;

                var oldValue = _Shape;
                _Shape = value;


            }
        }
        private int _Shape;

        public ItemEffect Effect
        {
            get { return _Effect; }
            set
            {
                if (_Effect == value) return;

                var oldValue = _Effect;
                _Effect = value;


            }
        }
        private ItemEffect _Effect;

        public int Image
        {
            get { return _Image; }
            set
            {
                if (_Image == value) return;

                var oldValue = _Image;
                _Image = value;

            }
        }
        private int _Image;

        public int Durability
        {
            get { return _Durability; }
            set
            {
                if (_Durability == value) return;

                var oldValue = _Durability;
                _Durability = value;


            }
        }
        private int _Durability;

        public int Price
        {
            get { return _Price; }
            set
            {
                if (_Price == value) return;

                var oldValue = _Price;
                _Price = value;


            }
        }
        private int _Price;

        public int Weight
        {
            get { return _Weight; }
            set
            {
                if (_Weight == value) return;

                var oldValue = _Weight;
                _Weight = value;


            }
        }
        private int _Weight;

        public int StackSize
        {
            get { return _StackSize; }
            set
            {
                if (_StackSize == value) return;

                var oldValue = _StackSize;
                _StackSize = value;


            }
        }
        private int _StackSize;

        public bool StartItem
        {
            get { return _StartItem; }
            set
            {
                if (_StartItem == value) return;

                var oldValue = _StartItem;
                _StartItem = value;


            }
        }
        private bool _StartItem;

        public decimal SellRate
        {
            get { return _SellRate; }
            set
            {
                if (_SellRate == value) return;

                var oldValue = _SellRate;
                _SellRate = value;


            }
        }
        private decimal _SellRate;

        public bool CanRepair
        {
            get { return _CanRepair; }
            set
            {
                if (_CanRepair == value) return;

                var oldValue = _CanRepair;
                _CanRepair = value;


            }
        }
        private bool _CanRepair;

        public bool CanSell
        {
            get { return _CanSell; }
            set
            {
                if (_CanSell == value) return;

                var oldValue = _CanSell;
                _CanSell = value;


            }
        }
        private bool _CanSell;

        public bool CanStore
        {
            get { return _CanStore; }
            set
            {
                if (_CanStore == value) return;

                var oldValue = _CanStore;
                _CanStore = value;


            }
        }
        private bool _CanStore;

        public bool CanTreasure
        {
            get { return _CanTreasure; }
            set
            {
                if (_CanTreasure == value) return;

                var oldValue = _CanTreasure;
                _CanTreasure = value;


            }
        }
        private bool _CanTreasure;

        public bool CanTrade
        {
            get { return _CanTrade; }
            set
            {
                if (_CanTrade == value) return;

                var oldValue = _CanTrade;
                _CanTrade = value;


            }
        }
        private bool _CanTrade;

        public bool NoMake
        {
            get { return _NoMake; }
            set
            {
                if (_NoMake == value) return;

                var oldValue = _NoMake;
                _NoMake = value;


            }
        }
        private bool _NoMake;

        public bool CanDrop
        {
            get { return _CanDrop; }
            set
            {
                if (_CanDrop == value) return;

                var oldValue = _CanDrop;
                _CanDrop = value;


            }
        }
        private bool _CanDrop;

        public bool CanDeathDrop
        {
            get { return _CanDeathDrop; }
            set
            {
                if (_CanDeathDrop == value) return;

                var oldValue = _CanDeathDrop;
                _CanDeathDrop = value;


            }
        }
        private bool _CanDeathDrop;

        public string Description
        {
            get { return _Description; }
            set
            {
                if (_Description == value) return;

                var oldValue = _Description;
                _Description = value;


            }
        }
        private string _Description;

        public Rarity Rarity
        {
            get { return _Rarity; }
            set
            {
                if (_Rarity == value) return;

                var oldValue = _Rarity;
                _Rarity = value;


            }
        }
        private Rarity _Rarity;

        public bool CanAutoPot
        {
            get { return _CanAutoPot; }
            set
            {
                if (_CanAutoPot == value) return;

                var oldValue = _CanAutoPot;
                _CanAutoPot = value;


            }
        }
        private bool _CanAutoPot;

        public int BuffIcon
        {
            get { return _BuffIcon; }
            set
            {
                if (_BuffIcon == value) return;

                var oldValue = _BuffIcon;
                _BuffIcon = value;


            }
        }
        private int _BuffIcon;

        public int PartCount
        {
            get { return _PartCount; }
            set
            {
                if (_PartCount == value) return;

                var oldValue = _PartCount;
                _PartCount = value;


            }
        }
        private int _PartCount;

        public SetInfoCopy Set { get; set; }

        public IList<ItemInfoStatCopy> ItemStats { get; set; }

        public IList<DropInfoCopy> Drops { get; set; }
    }

    public sealed class ItemInfoStatCopy
    {
        public Stat Stat
        {
            get { return _Stat; }
            set
            {
                if (_Stat == value) return;

                var oldValue = _Stat;
                _Stat = value;
            }
        }
        private Stat _Stat;

        public int Amount
        {
            get { return _Amount; }
            set
            {
                if (_Amount == value) return;

                var oldValue = _Amount;
                _Amount = value;
            }
        }
        private int _Amount;
    }

    public sealed class DropInfoCopy
    {

        public MonsterInfo MonsterInfo
        {
            get { return _MonsterInfo; }
            set
            {
                if (_MonsterInfo == value) return;

                var oldValue = _MonsterInfo;
                _MonsterInfo = value;
            }
        }
        private MonsterInfo _MonsterInfo;

        public string StrMonsterName { get; set; }

        public int Chance
        {
            get { return _Chance; }
            set
            {
                if (_Chance == value) return;

                var oldValue = _Chance;
                _Chance = value;
            }
        }
        private int _Chance;

        public int Amount
        {
            get { return _Amount; }
            set
            {
                if (_Amount == value) return;

                var oldValue = _Amount;
                _Amount = value;
            }
        }
        private int _Amount;

        public int DropSet
        {
            get { return _DropSet; }
            set
            {
                if (_DropSet == value) return;

                var oldValue = _DropSet;
                _DropSet = value;
            }
        }
        private int _DropSet;

        public bool PartOnly
        {
            get { return _PartOnly; }
            set
            {
                if (_PartOnly == value) return;

                var oldValue = _PartOnly;
                _PartOnly = value;
            }
        }
        private bool _PartOnly;

        public bool EasterEvent
        {
            get { return _EasterEvent; }
            set
            {
                if (_EasterEvent == value) return;

                var oldValue = _EasterEvent;
                _EasterEvent = value;
            }
        }
        private bool _EasterEvent;

        public string StrItemName { get; set; }
    }

    public sealed class SetInfoCopy
    {
        public string SetName
        {
            get { return _SetName; }
            set
            {
                if (_SetName == value) return;

                var oldValue = _SetName;
                _SetName = value;
            }
        }
        private string _SetName;
    }
}