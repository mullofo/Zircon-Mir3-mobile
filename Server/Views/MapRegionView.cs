using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using Library.SystemModels;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Server.Views
{
    public partial class MapRegionView : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public MapRegionView()       //地图区域视图
        {
            InitializeComponent();

            MapRegionGridControl.DataSource = SMain.Session.GetCollection<MapRegion>().Binding;
            MapLookUpEdit.DataSource = SMain.Session.GetCollection<MapInfo>().Binding;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SMain.SetUpView(MapRegionGridView);
        }

        private void SaveButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            SMain.Session.Save(true, MirDB.SessionMode.Server);
        }

        private void EditButtonEdit_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (MapViewer.CurrentViewer == null)
            {
                MapViewer.CurrentViewer = new MapViewer();
                MapViewer.CurrentViewer.Show();
            }

            MapViewer.CurrentViewer.BringToFront();

            GridView view = MapRegionGridControl.FocusedView as GridView;

            if (view == null) return;

            MapViewer.CurrentViewer.Save();

            MapViewer.CurrentViewer.MapRegion = view.GetFocusedRow() as MapRegion;
        }


        int[] SelectRows = null;



        private void MapRegionGridView_KeyDown(object sender, KeyEventArgs e)
        {//复制关联，请注意，在SMain.cs里如果判断是npc地图链接，会不执行那里面的方，否则会发生冲突，那里面也是处理复制粘贴(PasteData_KeyPress()方法的第一句话)

            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.C)
            {

                SelectRows = this.MapRegionGridView.GetSelectedRows();


                e.Handled = true;//
                List<MapRegionCopy> MapRegionCopyList = new List<MapRegionCopy>();
                foreach (int RowIndex in SelectRows)
                {
                    MapRegion MapRegion = MapRegionGridView.GetRow(RowIndex) as MapRegion;
                    bool[] ArrayBitRegionTemp = null;
                    if (MapRegion.BitRegion != null)
                    {
                        ArrayBitRegionTemp = new bool[MapRegion.BitRegion.Length];
                        MapRegion.BitRegion.CopyTo(ArrayBitRegionTemp, 0);
                    }

                    MapRegionCopyList.Add(new MapRegionCopy() { StrMapFileName = MapRegion.Map.FileName, StrMapDescription = MapRegion.Map.Description, Description = MapRegion.Description, Size = MapRegion.Size, BitRegion = MapRegion.BitRegion, ArrayBitRegion = ArrayBitRegionTemp, PointRegion = MapRegion.PointRegion });
                }

                string strJson = JsonHelper.SerializeObject(MapRegionCopyList);
                Clipboard.SetDataObject(strJson, true);
            }

            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.V)
            {

                if (SelectRows != null)
                {//如果是本程序里内复制
                    foreach (int RowIndex in SelectRows)
                    {

                        MapRegion MapRegion = MapRegionGridView.GetRow(RowIndex) as MapRegion;
                        MapRegion MapRegionTemp = SMain.Session.GetCollection<MapRegion>().CreateNewObject();

                        MapRegionTemp.Map = MapRegion.Map;
                        MapRegionTemp.Description = MapRegion.Description;
                        MapRegionTemp.Size = MapRegion.Size;
                        MapRegionTemp.BitRegion = MapRegion.BitRegion;
                        MapRegionTemp.PointRegion = MapRegion.PointRegion;
                    };
                }
                else
                {
                    IDataObject iData = Clipboard.GetDataObject();
                    if (iData.GetDataPresent(DataFormats.Text))
                    {
                        string strJson = (string)iData.GetData(DataFormats.Text);
                        List<MapRegionCopy> MapRegionCopyList = null;
                        try
                        {
                            MapRegionCopyList = JsonHelper.DeserializeJsonToList<MapRegionCopy>(strJson);
                        }
                        catch (System.Exception ex)
                        {
                            XtraMessageBox.Show("复制内容错误，请确认复制来源理否正确！" + ex.Message, "错误");
                            return;
                        }
                        IList<MapInfo> MapInfoList = SMain.Session.GetCollection<MapInfo>().Binding;
                        for (int i = 0; i < MapRegionCopyList.Count; i++)
                        {
                            MapInfo MapInfo = MapInfoList.FirstOrDefault(o => o.FileName == MapRegionCopyList[i].StrMapFileName);
                            if (MapInfo == null || MapInfo.Index <= 0)
                            {
                                XtraMessageBox.Show("根据FileName" + MapRegionCopyList[i].StrMapFileName + "在本区未找到相关地图信息,无法复制！", "错误");
                                continue;
                            }
                            MapRegionCopyList[i].MapInfo = MapInfo;


                            MapRegion MapRegionTemp = SMain.Session.GetCollection<MapRegion>().CreateNewObject();

                            MapRegionTemp.Map = MapRegionCopyList[i].MapInfo;
                            MapRegionTemp.Description = MapRegionCopyList[i].Description;
                            MapRegionTemp.Size = MapRegionCopyList[i].Size;
                            if (MapRegionCopyList[i].ArrayBitRegion != null)
                                MapRegionTemp.BitRegion = new BitArray(MapRegionCopyList[i].ArrayBitRegion);
                            MapRegionTemp.PointRegion = MapRegionCopyList[i].PointRegion;

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

        private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)  //导出
        {
            try
            {
                Helpers.HelperExcel<MapRegion>.ExportExcel();

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
                Helpers.HelperExcel<MapRegion>.ImportExcel();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("操作失败" + ex.Message, "提示");
            }
        }
    }



    public class MapRegionCopy
    {

        public MapInfo MapInfo
        {
            get { return _MapInfo; }
            set
            {
                if (_MapInfo == value) return;

                var oldValue = _MapInfo;
                _MapInfo = value;

            }
        }
        private MapInfo _MapInfo;


        public string StrMapDescription
        {
            get { return _StrMapDescription; }
            set
            {
                if (_StrMapDescription == value) return;

                var oldValue = _StrMapDescription;
                _StrMapDescription = value;

            }
        }
        private string _StrMapDescription;


        public string StrMapFileName
        {
            get { return _StrMapFileName; }
            set
            {
                if (_StrMapFileName == value) return;

                var oldValue = _StrMapFileName;
                _StrMapFileName = value;


            }
        }

        private string _StrMapFileName;

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


        [JsonIgnore]
        public BitArray BitRegion
        {
            get { return _BitRegion; }
            set
            {
                if (_BitRegion == value) return;

                var oldValue = _BitRegion;
                _BitRegion = value;


            }
        }
        private BitArray _BitRegion;

        public bool[] ArrayBitRegion { get; set; }


        public Point[] PointRegion
        {
            get { return _PointRegion; }
            set
            {
                if (_PointRegion == value) return;

                var oldValue = _PointRegion;
                _PointRegion = value;


            }
        }
        private Point[] _PointRegion;

        public int Size
        {
            get { return _Size; }
            set
            {
                if (_Size == value) return;

                var oldValue = _Size;
                _Size = value;


            }
        }
        private int _Size;


    }
}