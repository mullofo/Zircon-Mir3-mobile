using System;
namespace Mir3.Mobile
{
    public interface IUI
    {
        /// <summary>
        /// 密码
        /// </summary>
        string Pwd { get; set; }
        /// <summary>
        /// 账户
        /// </summary>
        string Account { get; set; }
        /// <summary>
        /// 确认密码
        /// </summary>
        string CPwd { get; set; }
        /// <summary>
        /// 确认账户
        /// </summary>
        string CAccount { get; set; }
        /// <summary>
        /// 隐藏弹窗
        /// </summary>
        void HidePopWindow();
        /// <summary>
        /// 显示遮罩
        /// </summary>
        bool ShowLayout { get; set; }
        /// <summary>
        /// 退出应用
        /// </summary>
        void Exit();
#if ANDROID
        /// <summary>
        /// 加载登录界面
        /// </summary>
        void InitLogin();
#endif
        /// <summary>
        /// 提示消息
        /// </summary>
        /// <param name="msg"></param>
        void ShowMsg(string msg);
        /// <summary>
        /// UI主线程委托
        /// </summary>
        void BeginInvoke(Action action);
        /// <summary>
        /// 销毁进程
        /// </summary>
        void Finish();

        bool IsAllScreen { get; }

        /// <summary>
        /// 刘海偏离适配x左间距，y右间距
        /// </summary>
        Microsoft.Xna.Framework.Point ScreenOffset { get; set; }

#if IOS
        int Height { get; set; }

        void Push(CocoaUiBinding.ITable controller);
#endif
    }
}

