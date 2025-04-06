using Client.Controls;
using Client.Envir;
using Client.UserModels;
using Library;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Client.Scenes.Views
{
    /// <summary>
    /// 右边聊天信息显示框
    /// </summary>
    public class ChatRDialog : DXWindow
    {
        #region Properties
        public override WindowType Type => WindowType.None;

        public override bool CustomSize => false;

        public override bool AutomaticVisibility => false;

        public List<DXLabel> History = new List<DXLabel>();
        #endregion

        /// <summary>
        /// 右边聊天信息显示框
        /// </summary>
        public ChatRDialog()
        {
            Movable = false;
            Opacity = 0F;
            HasTitle = false;
            HasTopBorder = false;
            CloseButton.Visible = false;
            Border = false;
            PassThrough = true;

            Size = new Size(GameScene.Game.ChatBox.Size.Width + 30, 70);
            History = new List<DXLabel>();
        }

        #region method
        private readonly object obj = 1;
        /// <summary>
        /// 显示信息内容
        /// </summary>
        /// <param name="message"></param>
        /// <param name="type"></param>
        public void ReceiveChat(string message, MessageType type)
        {
            var label = new DXLabel
            {
                Tag = CEnvir.Now,
                Parent = this,
                PassThrough = true,
            };
            var prefix = "";
            switch (type)   //只显示 战斗信息  公告信息  系统信息  提示信息
            {
                case MessageType.Combat:
                    //if (!GameScene.Game.ChatBox.IsAll) return;
                    label.ForeColour = Color.White;
                    break;
                case MessageType.Announcement:
                    label.ForeColour = Color.White;
                    break;
                case MessageType.System:
                    //if (!(GameScene.Game.ChatBox.IsSystem || GameScene.Game.ChatBox.IsAll)) return;
                    label.ForeColour = Color.FromArgb(255, 0, 0);
                    break;
                case MessageType.Hint:
                    //if (!GameScene.Game.ChatBox.IsAll) return;
                    label.ForeColour = Color.Red;
                    break;
                case MessageType.DurabilityTips:    //持久提示
                    label.ForeColour = Color.Yellow;
                    break;
                case MessageType.BossTips:          //boss提示
                    label.ForeColour = Color.Yellow;
                    break;
                case MessageType.UseItem:       //道具使用提示
                    //label.BackColour = Color.FromArgb(255, 255, 128, 0);  //背景色 橙色
                    label.ForeColour = Color.White;          //文字颜色 白色
                    //label.Outline = false;
                    break;
                default:
                    return;
            }
            label.Tag = CEnvir.Now;
            label.Text = prefix + (message ?? "");
            Size size = DXLabel.GetHeight(label, Size.Width - 40);
            label.Size = new Size(size.Width, size.Height);
            lock (obj)
            {
                UpdateItems(label.Size.Height);
                label.Location = new Point(0, Size.Height - label.Size.Height - 2);

                History.Add(label);
                Visible = true;
            }
        }
        /// <summary>
        /// 过程
        /// </summary>
        public override void Process()
        {
            base.Process();

            Visible = History.Count(p => p.Visible) != 0;

            if (History.Count > 0)
            {
                lock (obj)
                {
                    var old = History.FirstOrDefault(p => p.Visible);
                    if (old != null && !old.IsDisposed && (CEnvir.Now - (DateTime)old.Tag).Seconds > 7)  //信息停留时间
                    {
                        old.Visible = false;
                    }
                }
            }
        }
        /// <summary>
        /// 更新项目
        /// </summary>
        /// <param name="height"></param>
        private void UpdateItems(int height)
        {
            while (History.Count > 3)
            {
                var oldLabel = History[0];
                History.Remove(oldLabel);
                oldLabel.Dispose();
            }
            foreach (var item in History)
            {
                item.Location = new Point(item.Location.X, item.Location.Y - height - 2);
            }
        }
        #endregion

        #region IDisposable
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (History != null)
                {
                    for (int i = 0; i < History.Count; i++)
                    {
                        if (History[i] != null)
                        {
                            if (!History[i].IsDisposed)
                                History[i].Dispose();

                            History[i] = null;
                        }
                    }

                    History.Clear();
                    History = null;
                }
            }
        }
        #endregion
    }
}
