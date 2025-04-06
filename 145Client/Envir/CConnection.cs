using Client.Controls;
using Client.Extentions;
using Client.Models;
using Client.Scenes;
using Client.Scenes.Configs;
using Client.Scenes.Views;
using Library;
using Library.Network;
using Library.SystemModels;
using Sentry;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography;
using System.Windows.Forms;
using C = Library.Network.ClientPackets;
using Color = System.Drawing.Color;
using G = Library.Network.GeneralPackets;
using Point = System.Drawing.Point;
using S = Library.Network.ServerPackets;

namespace Client.Envir
{
    /// <summary>
    /// 线程触发
    /// 数据库连接
    /// </summary>
    public sealed class CConnection : BaseConnection
    {
        protected override TimeSpan TimeOutDelay => NetworkConfig.TimeOutDuration;
        private DateTime DurWarnDelay;
        public bool ServerConnected { get; set; }

        public int Ping;

        public CConnection(TcpClient client)  //连接客户端
            : base(client)
        {
            OnException += (o, e) =>
            {
                if (Config.SentryEnabled)
                {
                    if (!(e is SocketException))
                    {
                        SentrySdk.CaptureException(e);
                    }
                }
                CEnvir.SaveError(e.ToString());
            };
            Output += (o, e) =>
            {
                CEnvir.SaveError(e);
            };

            UpdateTimeOut();

            AdditionalLogging = true;

            BeginReceive();
        }

        public override void TryDisconnect()  //尝试断开链接
        {
            Disconnect();
        }
        public override void Disconnect()  //断开链接
        {
            base.Disconnect();

            if (CEnvir.Connection == this)
            {
                CEnvir.Connection = null;

                var scene = DXControl.ActiveScene as LoginScene;
                if (scene != null)
                {
                    scene.Disconnected();
                }
                else
                {
                    DXMessageBox.Show("与服务器断开连接\n原因：连接超时".Lang(), "已断开连接".Lang(), DialogAction.ReturnToLogin);
                }
            }

            CEnvir.Storage = null;  //仓库
            CEnvir.PatchGrid = null; //碎片包裹
            CEnvir.CompanionGrid = null; //宠物包裹
        }
        public override void TrySendDisconnect(Packet p)  //尝试发送断开链接
        {
            SendDisconnect(p);
        }

        public void Process(G.Ping p)  //PING
        {
            Enqueue(new G.Ping());
        }
        public void Process(G.PingResponse p)  //PING响应
        {
            Ping = p.Ping;
        }

        public void Process(G.Disconnect p)  //断开
        {
            Disconnecting = true;

            var scene = DXControl.ActiveScene as LoginScene;

            if (scene != null)
            {
                if (p.Reason == DisconnectReason.WrongVersion || p.Reason == DisconnectReason.WrongClientSystemDBVersion)
                {
                    CEnvir.WrongVersion = true;

                    if (p.Reason == DisconnectReason.WrongVersion)
                        DXMessageBox.Show("与服务器断开连接\n原因：客户端版本校验错误".Lang(), "已断开连接".Lang(), DialogAction.Close).Modal = false;

                    if (p.Reason == DisconnectReason.WrongClientSystemDBVersion)
                        DXMessageBox.Show("与服务器断开连接\n原因：客户端数据库版本校验错误".Lang(), "已断开连接".Lang(), DialogAction.Close).Modal = false;
                }

                scene.Disconnected();
                return;
            }

            switch (p.Reason)
            {
                case DisconnectReason.Unknown:
                    DXMessageBox.Show("与服务器断开连接\n原因：未知".Lang(), "已断开连接".Lang(), DialogAction.ReturnToLogin);
                    break;
                case DisconnectReason.TimedOut:
                    DXMessageBox.Show("与服务器断开连接\n原因：连接超时".Lang(), "已断开连接".Lang(), DialogAction.ReturnToLogin);
                    break;
                case DisconnectReason.ServerClosing:
                    DXMessageBox.Show("与服务器断开连接\n原因：服务器主动关闭".Lang(), "已断开连接".Lang(), DialogAction.ReturnToLogin);
                    break;
                case DisconnectReason.AnotherUser:
                    DXMessageBox.Show("与服务器断开连接\n原因：其他用户登录了你的帐户".Lang(), "已断开连接".Lang(), DialogAction.ReturnToLogin);
                    break;
                case DisconnectReason.AnotherUserAdmin:
                    DXMessageBox.Show("与服务器断开连接\n原因：管理员登录你的帐户".Lang(), "已断开连接".Lang(), DialogAction.ReturnToLogin);
                    break;
                case DisconnectReason.Banned:
                    DXMessageBox.Show("与服务器断开连接\n原因：你已被封号".Lang(), "已断开连接".Lang(), DialogAction.ReturnToLogin);
                    break;
                case DisconnectReason.Crashed:
                    DXMessageBox.Show("与服务器断开连接\n原因：服务器崩溃".Lang(), "已断开连接".Lang(), DialogAction.ReturnToLogin);
                    break;
                case DisconnectReason.PlugInDetection:
                    //DXConfirmWindow.Show("与服务器断开连接\n原因：请勿使用非法辅助！！！".Lang(), DialogAction.ReturnToLogin);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (this == CEnvir.Connection)
                CEnvir.Connection = null;
        }

        //客户端与服务端建立连接后收到的第一个Connected封包，向服务端发送版本验证信息
        public void Process(G.Connected p)
        {

            byte[] clientHash, clientSystemDBHash;
            using (MD5 md5 = MD5.Create())
            {
                using (FileStream stream = File.OpenRead(Application.ExecutablePath))
                    clientHash = md5.ComputeHash(stream);
                using (FileStream stream = File.OpenRead(@".\Data\ClientSystem.db"))
                    clientSystemDBHash = md5.ComputeHash(stream);
            }

            Enqueue(new G.Version
            {
                Platform = Platform.Desktop,
                ClientHash = clientHash,
                ClientSystemDBHash = clientSystemDBHash,
                ClientMACInfo = CEnvir.MacInfo,  //检查MAC
                ClientCPUInfo = CEnvir.CpuInfo,  //检查CPU
                ClientHDDInfo = CEnvir.HDDnfo,   //检查HDD
            });
        }
        public void Process(G.GoodVersion p)  //正确版本
        {
            //真正通过版本验证后 才算建立连接
            ServerConnected = true;

            //数据库加密相关
            CEnvir.DBEncrypted = p.DBEncrypted;
            CEnvir.DBPassword = p.DBPassword;
            CEnvir.DBInfoReceived = true;

            //服务区列表
            NetworkConfig.Server1Name = p.Server1Name;
            if (p.PlayCount < 5)
            {
                NetworkConfig.Server1Name += " (良好)";
            }
            else if (p.PlayCount < 20)
            {
                NetworkConfig.Server1Name += " (繁忙)";
            }
            else
            {
                NetworkConfig.Server1Name += " (满员)";
            }

            LoginScene scene = DXControl.ActiveScene as LoginScene;
            scene?.ShowLogin();
            if (!Enum.TryParse(Config.Language, out Language lang))
            {
                lang = Language.SimplifiedChinese;
            }
            Enqueue(new C.SelectLanguage { Language = lang });
        }

        public void Process(S.ClientNameChanged p)   //客户端区服名称
        {
            CEnvir.Target.Text = p.ClientName;
            NetworkConfig.ClientName = p.ClientName;
            //古墓任务序号
            CEnvir.PenetraliumKeyA = p.PenetraliumKeyA;
            CEnvir.PenetraliumKeyB = p.PenetraliumKeyB;
            CEnvir.PenetraliumKeyC = p.PenetraliumKeyC;

            //沙巴克旗帜附近是否允许使用位移技能
            CEnvir.AllowTeleportMagicNearFlag = p.AllowTeleportMagicNearFlag;
            CEnvir.TeleportMagicRadiusRange = p.TeleportMagicRadiusRange;
        }

        public void Process(S.ShowItemSource p) //是否显示物品来源信息
        {
            CEnvir.DisplayItemSource = p.DisplayItemSource;
            CEnvir.DisplayGMItemSource = p.DisplayGMSource;
        }

        public void Process(S.NewAccount p)  //新建账号
        {
            LoginScene login = DXControl.ActiveScene as LoginScene;
            if (login == null) return;

            login.AccountBox.CreateAttempted = false;

            switch (p.Result)
            {
                case NewAccountResult.Disabled:
                    login.AccountBox.Clear();
                    DXMessageBox.Show("系统禁止创建账号".Lang(), "账号创建".Lang());
                    break;
                case NewAccountResult.BadEMail:
                    login.AccountBox.EMailTextBox.SetFocus();
                    DXMessageBox.Show("E-Mail地址格式错误".Lang(), "账号创建".Lang());
                    break;
                case NewAccountResult.BadPassword:
                    login.AccountBox.Password1TextBox.SetFocus();
                    DXMessageBox.Show("密码格式错误".Lang(), "账号创建".Lang());
                    break;
                case NewAccountResult.BadRealName:
                    login.AccountBox.RealNameTextBox.SetFocus();
                    DXMessageBox.Show("真实姓名格式错误".Lang(), "账号创建".Lang());
                    break;
                case NewAccountResult.AlreadyExists:
                    login.AccountBox.EMailTextBox.TextBox.Text = string.Empty;
                    login.AccountBox.EMailTextBox.SetFocus();
                    DXMessageBox.Show("E-Mail地址已被使用".Lang(), "账号创建".Lang());
                    break;
                case NewAccountResult.BadReferral:
                    login.AccountBox.ReferralTextBox.SetFocus();
                    DXMessageBox.Show("推荐人的E-Mail地址错误".Lang(), "账号创建".Lang());
                    break;
                case NewAccountResult.ReferralNotFound:
                    login.AccountBox.ReferralTextBox.SetFocus();
                    DXMessageBox.Show("找不到该推荐人的E-Mail地址".Lang(), "账号创建".Lang());
                    break;
                case NewAccountResult.ReferralNotActivated:
                    login.AccountBox.EMailTextBox.SetFocus();
                    DXMessageBox.Show("推荐人的E-Mail地址没有被激活".Lang(), "账号创建".Lang());
                    break;
                case NewAccountResult.BadInviteCode:
                    login.AccountBox.InviteCodeTextBox.SetFocus();
                    DXMessageBox.Show("激活码无效".Lang(), "账号创建".Lang());
                    break;
                case NewAccountResult.Success:
                    login.LoginBox.EMailTextBox.TextBox.Text = login.AccountBox.EMailTextBox.TextBox.Text;
                    login.LoginBox.PasswordTextBox.TextBox.Text = login.AccountBox.Password1TextBox.TextBox.Text;
                    login.AccountBox.Clear();
                    DXMessageBox.Show("你的账号已成功创建。".Lang(), "账号创建".Lang());  //\n请按照发送到你的电子邮件说明进行激活
                    break;
            }
        }

        public void Process(S.ChangePassword p)  //修改密码
        {
            LoginScene login = DXControl.ActiveScene as LoginScene;
            if (login == null) return;

            login.ChangeBox.ChangeAttempted = false;

            switch (p.Result)
            {
                case ChangePasswordResult.Disabled:
                    login.ChangeBox.Clear();
                    DXMessageBox.Show("系统禁止修改密码".Lang(), "修改密码".Lang());
                    break;
                case ChangePasswordResult.BadEMail:
                    login.ChangeBox.EMailTextBox.SetFocus();
                    DXMessageBox.Show("E-Mail格式错误".Lang(), "修改密码".Lang());
                    break;
                case ChangePasswordResult.BadCurrentPassword:
                    login.ChangeBox.CurrentPasswordTextBox.SetFocus();
                    DXMessageBox.Show("当前密码格式错误".Lang(), "修改密码".Lang());
                    break;
                case ChangePasswordResult.BadNewPassword:
                    login.ChangeBox.NewPassword1TextBox.SetFocus();
                    DXMessageBox.Show("新密码格式错误".Lang(), "修改密码".Lang());
                    break;
                case ChangePasswordResult.AccountNotFound:
                    login.ChangeBox.EMailTextBox.SetFocus();
                    DXMessageBox.Show("账号不存在".Lang(), "修改密码".Lang());
                    break;
                case ChangePasswordResult.AccountNotActivated:
                    login.ShowActivationBox(login.ChangeBox);
                    break;
                case ChangePasswordResult.WrongPassword:
                    login.ChangeBox.CurrentPasswordTextBox.SetFocus();
                    DXMessageBox.Show("密码错误".Lang(), "修改密码".Lang());
                    break;
                case ChangePasswordResult.Banned:
                    DateTime expiry = CEnvir.Now.Add(p.Duration);
                    DXMessageBox box = DXMessageBox.Show($"此账号已被禁止登录".Lang() + "\n\n" +
                                                         $"原因".Lang() + $": {p.Message}\n" +
                                                         $"到期日".Lang() + $": {expiry}\n" +
                                                         $"Cconnection.Duration".Lang(Math.Floor(p.Duration.TotalHours).ToString("#,##0"), p.Duration.Minutes, p.Duration.Seconds), "修改密码".Lang());

                    box.ProcessAction = () =>
                    {
                        if (CEnvir.Now > expiry)
                        {
                            if (login.ChangeBox.CanChange)
                                login.ChangeBox.Change();
                            box.ProcessAction = null;
                            return;
                        }

                        TimeSpan remaining = expiry - CEnvir.Now;

                        box.Label.Text = $"此账号已被禁止登录".Lang() + "\n\n" +
                                         $"原因".Lang() + $": {p.Message}\n" +
                                         $"到期日".Lang() + $": {expiry}\n" +
                                         $"Cconnection.Duration".Lang(Math.Floor(p.Duration.TotalHours).ToString("#,##0"), p.Duration.Minutes, p.Duration.Seconds);
                    };
                    break;
                case ChangePasswordResult.Success:
                    login.ChangeBox.Clear();
                    DXMessageBox.Show("密码修改成功".Lang(), "修改密码".Lang());
                    break;
            }

        }
        public void Process(S.RequestPasswordReset p)  //请求密码重置
        {
            LoginScene login = DXControl.ActiveScene as LoginScene;
            if (login == null) return;

            login.RequestPassswordBox.RequestAttempted = false;

            DateTime expiry;
            DXMessageBox box;
            switch (p.Result)
            {
                case RequestPasswordResetResult.Disabled:
                    login.RequestPassswordBox.Clear();
                    DXMessageBox.Show("系统禁止重置密码".Lang(), "重置密码".Lang());
                    break;
                case RequestPasswordResetResult.BadEMail:
                    login.RequestPassswordBox.EMailTextBox.SetFocus();
                    DXMessageBox.Show("E-Mail地址错误".Lang(), "重置密码".Lang());
                    break;
                case RequestPasswordResetResult.AccountNotFound:
                    login.RequestPassswordBox.EMailTextBox.SetFocus();
                    DXMessageBox.Show("帐户不存在".Lang(), "重置密码".Lang());
                    break;
                case RequestPasswordResetResult.AccountNotActivated:
                    login.ShowActivationBox(login.RequestPassswordBox);
                    break;
                case RequestPasswordResetResult.ResetDelay:
                    expiry = CEnvir.Now.Add(p.Duration);
                    box = DXMessageBox.Show($"你不能这么快就请求另一个重置密码".Lang() + "\n" +
                                            $"下次可重置".Lang() + $": {expiry}\n" +
                                            $"Cconnection.Duration".Lang(Math.Floor(p.Duration.TotalHours).ToString("#,##0"), p.Duration.Minutes, p.Duration.Seconds), "重置密码".Lang());

                    box.ProcessAction = () =>
                    {
                        if (CEnvir.Now != expiry) //密码检查
                        {
                            if (login.RequestPassswordBox.CanReset)
                                login.RequestPassswordBox.Request();
                            box.ProcessAction = null;
                            return;
                        }

                        TimeSpan remaining = expiry - CEnvir.Now;

                        box.Label.Text = $"你不能这么快就请求另一个重置密码".Lang() + "\n" +
                                         $"下次可重置".Lang() + $": {expiry}\n" +
                                         $"Cconnection.Duration".Lang(Math.Floor(p.Duration.TotalHours).ToString("#,##0"), p.Duration.Minutes, p.Duration.Seconds);
                    };
                    break;
                case RequestPasswordResetResult.Banned:
                    expiry = CEnvir.Now.Add(p.Duration);
                    box = DXMessageBox.Show($"此账号已被禁止登录".Lang() + "\n\n" +
                                            $"原因".Lang() + $": {p.Message}\n" +
                                            $"到期日".Lang() + $": {expiry}\n" +
                                            $"Cconnection.Duration".Lang(Math.Floor(p.Duration.TotalHours).ToString("#,##0"), p.Duration.Minutes, p.Duration.Seconds), "重置密码");

                    box.ProcessAction = () =>
                    {
                        if (CEnvir.Now > expiry)
                        {
                            if (login.RequestPassswordBox.CanReset)
                                login.RequestPassswordBox.Request();
                            box.ProcessAction = null;
                            return;
                        }

                        TimeSpan remaining = expiry - CEnvir.Now;

                        box.Label.Text = $"此账号已被禁止登录".Lang() + "\n\n" +
                                         $"原因".Lang() + $": {p.Message}\n" +
                                         $"到期日".Lang() + $": {expiry}\n" +
                                         $"Cconnection.Duration".Lang(Math.Floor(p.Duration.TotalHours).ToString("#,##0"), p.Duration.Minutes, p.Duration.Seconds);
                    };
                    break;
                case RequestPasswordResetResult.Success:
                    login.RequestPassswordBox.Clear();
                    DXMessageBox.Show("重置密码请求成功".Lang() + "\n" +
                                      "请查看你的电子邮件以获取进一步的说明".Lang(), "重置密码".Lang());
                    break;
            }

        }
        public void Process(S.ResetPassword p)  //重置密码
        {
            LoginScene login = DXControl.ActiveScene as LoginScene;
            if (login == null) return;

            login.ResetBox.ResetAttempted = false;

            switch (p.Result)
            {
                case ResetPasswordResult.Disabled:
                    login.ResetBox.Clear();
                    DXMessageBox.Show("系统禁止手动重置密码".Lang(), "重置密码".Lang());
                    break;
                case ResetPasswordResult.BadNewPassword:
                    login.ResetBox.NewPassword1TextBox.SetFocus();
                    DXMessageBox.Show("新密码格式错误无法使用".Lang(), "重置密码".Lang());
                    break;
                case ResetPasswordResult.AccountNotFound:
                    login.ResetBox.ResetKeyTextBox.SetFocus();
                    DXMessageBox.Show("无法找到账号".Lang(), "重置密码".Lang());
                    break;
                case ResetPasswordResult.Success:
                    login.ResetBox.Clear();
                    DXMessageBox.Show("密码重置成功".Lang(), "重置密码".Lang());
                    break;
            }
        }
        public void Process(S.Activation p)  //激活
        {
            LoginScene login = DXControl.ActiveScene as LoginScene;
            if (login == null) return;

            login.ActivationBox.ActivationAttempted = false;

            switch (p.Result)
            {
                case ActivationResult.Disabled:
                    login.ActivationBox.Clear();
                    DXMessageBox.Show("系统禁止手动激活".Lang(), "激活".Lang());
                    break;
                case ActivationResult.AccountNotFound:
                    login.ActivationBox.ActivationKeyTextBox.SetFocus();
                    DXMessageBox.Show("无法找到账号".Lang(), "激活".Lang());
                    break;
                case ActivationResult.Success:
                    login.ActivationBox.Clear();
                    DXMessageBox.Show("你的账号已成功激活".Lang() + "\n", "激活".Lang());
                    break;
            }

        }
        public void Process(S.RequestActivationKey p)  //申请激活KEY
        {
            LoginScene login = DXControl.ActiveScene as LoginScene;
            if (login == null) return;

            login.RequestActivationBox.RequestAttempted = false;

            DateTime expiry;
            DXMessageBox box;
            switch (p.Result)
            {
                case RequestActivationKeyResult.Disabled:
                    login.RequestActivationBox.Clear();
                    DXMessageBox.Show("系统禁止申请激活密匙".Lang(), "申请激活密钥".Lang());
                    break;
                case RequestActivationKeyResult.BadEMail:
                    login.RequestActivationBox.EMailTextBox.SetFocus();
                    DXMessageBox.Show("电子邮件错误".Lang(), "申请激活密钥".Lang());
                    break;
                case RequestActivationKeyResult.AccountNotFound:
                    login.RequestActivationBox.EMailTextBox.SetFocus();
                    DXMessageBox.Show("账号不存在".Lang(), "申请激活密钥".Lang());
                    break;
                case RequestActivationKeyResult.AlreadyActivated:
                    login.RequestActivationBox.Clear();
                    DXMessageBox.Show("账号已激活".Lang(), "申请激活密钥".Lang());
                    break;
                case RequestActivationKeyResult.RequestDelay:
                    expiry = CEnvir.Now.Add(p.Duration);
                    box = DXMessageBox.Show($"不能这么快就请求另一个激活邮件".Lang() + "\n" +
                                            $"下次请求".Lang() + $": {expiry}\n" +
                                            $"Cconnection.Duration".Lang(Math.Floor(p.Duration.TotalHours).ToString("#,##0"), p.Duration.Minutes, p.Duration.Seconds), "申请激活密钥");

                    box.ProcessAction = () =>
                    {
                        if (CEnvir.Now > expiry)
                        {
                            if (login.RequestActivationBox.CanRequest)
                                login.RequestActivationBox.Request();
                            box.ProcessAction = null;
                            return;
                        }

                        TimeSpan remaining = expiry - CEnvir.Now;

                        box.Label.Text = $"不能这么快就请求另一个激活邮件".Lang() + "\n" +
                                         $"下次请求".Lang() + $": {expiry}\n" +
                                         $"Cconnection.Duration".Lang(Math.Floor(p.Duration.TotalHours).ToString("#,##0"), p.Duration.Minutes, p.Duration.Seconds);
                    };
                    break;
                case RequestActivationKeyResult.Success:
                    login.RequestActivationBox.Clear();
                    DXMessageBox.Show("激活电子邮件请求成功发送".Lang() + "\n" +
                                      "请查看你的电子邮件以获取进一步的说明".Lang(), "申请激活密钥".Lang());
                    break;
            }
        }
        public void Process(S.Login p)  //登录
        {
            LoginScene login = DXControl.ActiveScene as LoginScene;
            if (login == null) return;

            login.LoginBox.LoginAttempted = false;

            switch (p.Result)
            {
                case LoginResult.Disabled:
                    DXMessageBox.Show("系统当前禁止账号登录".Lang(), "登录".Lang());
                    break;
                case LoginResult.BadEMail:
                    login.LoginBox.EMailTextBox.SetFocus();
                    DXMessageBox.Show("用户名格式不正确".Lang(), "登录".Lang());
                    break;
                case LoginResult.BadPassword:
                    login.LoginBox.PasswordTextBox.SetFocus();
                    DXMessageBox.Show("目前的密码格式不正确".Lang(), "登录".Lang());
                    break;
                case LoginResult.AccountNotExists:
                    login.LoginBox.EMailTextBox.SetFocus();
                    DXMessageBox.Show("账号不存在".Lang(), "登录".Lang());
                    break;
                case LoginResult.AccountNotActivated:
                    login.ShowActivationBox(login.LoginBox);
                    break;
                case LoginResult.WrongPassword:
                    login.LoginBox.PasswordTextBox.SetFocus();
                    DXMessageBox.Show("密码错误".Lang(), "登录".Lang());
                    break;
                case LoginResult.LockIp://限制登录IP返回提示信息
                    login.LoginBox.PasswordTextBox.SetFocus();
                    DXMessageBox.Show("你的IP被限制登录请联系管理员".Lang(), "登录".Lang());
                    break;
                case LoginResult.MaxConnectionExceeded: //限制多开返回提示信息
                    login.LoginBox.PasswordTextBox.SetFocus();
                    DXMessageBox.Show("超出同IP连接限制无法在开启客户端".Lang(), "登录".Lang());
                    break;
                case LoginResult.Banned:
                    DateTime expiry = CEnvir.Now.Add(p.Duration);

                    DXMessageBox box = DXMessageBox.Show($"此账号已被封禁" + "\n\n" +
                                                         $"原因" + $": {p.Message}\n" +
                                                         $"到期日" + $": {expiry}\n" +
                                                         $"Cconnection.Duration".Lang(Math.Floor(p.Duration.TotalHours).ToString("#,##0"), p.Duration.Minutes, p.Duration.Seconds), "登录".Lang());

                    box.ProcessAction = () =>
                    {
                        if (CEnvir.Now > expiry)
                        {
                            if (login.LoginBox.CanLogin)
                                login.LoginBox.Login();
                            box.ProcessAction = null;
                            return;
                        }

                        TimeSpan remaining = expiry - CEnvir.Now;

                        box.Label.Text = $"此账号已被封禁" + "\n\n" +
                                         $"原因" + $": {p.Message}\n" +
                                         $"到期日" + $": {expiry}\n" +
                                         $"Cconnection.Duration".Lang(Math.Floor(p.Duration.TotalHours).ToString("#,##0"), p.Duration.Minutes, p.Duration.Seconds);
                    };
                    break;
                case LoginResult.AlreadyLoggedIn:
                    login.LoginBox.EMailTextBox.SetFocus();
                    DXMessageBox.Show("账号目前正在使用中请稍后再试".Lang(), "登录".Lang());
                    break;
                case LoginResult.AlreadyLoggedInPassword:
                    login.LoginBox.EMailTextBox.SetFocus();
                    DXMessageBox.Show("账号目前正在使用中请稍后再试".Lang(), "登录".Lang());
                    break;
                case LoginResult.AlreadyLoggedInAdmin:
                    login.LoginBox.EMailTextBox.SetFocus();
                    DXMessageBox.Show("账号目前正由管理员使用".Lang(), "登录".Lang());
                    break;
                case LoginResult.Success:   //登录结果成功
                    login.LoginBox.Visible = false;
                    login.AccountBox.Visible = false;
                    login.ChangeBox.Visible = false;
                    login.RequestPassswordBox.Visible = false;
                    login.ResetBox.Visible = false;
                    login.ActivationBox.Visible = false;
                    login.RequestActivationBox.Visible = false;

                    CEnvir.TestServer = p.TestServer;

                    if (Config.RememberDetails)
                    {
                        Config.RememberedEMail = login.LoginBox.EMailTextBox.TextBox.Text;
                        Config.RememberedPassword = login.LoginBox.PasswordTextBox.TextBox.Text;
                    }

                    login.Dispose();

                    p.Characters.Sort((x1, x2) => x2.LastLogin.CompareTo(x1.LastLogin));

                    var serverScence = new ServerScene(Config.IntroSceneSize)
                    {
                        ServersBox = { CharacterList = p.Characters }
                    };
                    DXControl.ActiveScene = serverScence;
                    if (p.PlayCount < 5)
                        serverScence.ServersBox.Server1.ForeColour = Color.Blue;
                    else if (p.PlayCount < 20)
                        serverScence.ServersBox.Server1.ForeColour = Color.Green;
                    else
                        serverScence.ServersBox.Server1.ForeColour = Color.Red;
                    login.Dispose();

                    CEnvir.BuyAddress = p.Address;
                    CEnvir.FillStorage(p.Items, false); //仓库
                    CEnvir.FillCompanionGrid(p.Items, false); //宠物包裹
                    //CEnvir.FillPatchGrid(p.Items, false); //碎片包裹

                    CEnvir.BlockList = p.BlockList;

                    if (!string.IsNullOrEmpty(p.Message)) DXMessageBox.Show(p.Message, "登录消息".Lang());

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        public void Process(S.SelectLogout p)  //选择注销
        {
            CEnvir.ReturnToLogin();
        }
        public void Process(S.GameLogout p)   //游戏注销
        {
            DXSoundManager.StopAllSounds();

            GameScene.Game.Dispose();
            GameScene.Game = null;
            DXManager.ForceReworkTextures(forceNow: true);

            DXSoundManager.Play(SoundIndex.SelChrBgm145);

            SelectScene scene;

            p.Characters.Sort((x1, x2) => x2.LastLogin.CompareTo(x1.LastLogin));

            DXControl.ActiveScene = scene = new SelectScene(Config.IntroSceneSize)
            {
                SelectBox =
                {
                    CharacterList = p.Characters
                },
                IsVisible = true,
            };

            CEnvir.Storage = CEnvir.MainStorage;               //仓库
            CEnvir.PatchGrid = CEnvir.MainPatchGrid;          //碎片包裹
            CEnvir.CompanionGrid = CEnvir.MainCompanionGrid;  //宠物包裹

            scene.SelectBox.UpdateCharacters();
        }

        public void Process(S.NewCharacter p)   //新建角色
        {
            SelectScene select = DXControl.ActiveScene as SelectScene;
            if (select == null) return;

            select.CharacterBox.CreateAttempted = false;

            switch (p.Result)
            {
                case NewCharacterResult.Disabled:
                    select.CharacterBox.Close();
                    DXMessageBox.Show("目前已禁止创建角色".Lang(), "角色创建".Lang());
                    break;
                case NewCharacterResult.BadCharacterName:
                    select.CharacterBox.CharacterNameTextBox.SetFocus();
                    DXMessageBox.Show("角色名称不符合要求".Lang(), "角色创建".Lang());
                    break;
                case NewCharacterResult.BadHairType:
                    DXMessageBox.Show("错误无效的头发类型".Lang(), "角色创建".Lang());
                    break;
                case NewCharacterResult.BadHairColour:
                    DXMessageBox.Show("错误无效的头发颜色".Lang(), "角色创建".Lang());
                    break;
                case NewCharacterResult.BadArmourColour:
                    DXMessageBox.Show("错误衣服颜色无效".Lang(), "角色创建".Lang());
                    break;
                case NewCharacterResult.BadGender:
                    select.CharacterBox.SelectedGender = MirGender.Male;
                    DXMessageBox.Show("错误选择了无效的性别".Lang(), "角色创建".Lang());
                    break;
                case NewCharacterResult.BadClass:
                    select.CharacterBox.SelectedClass = MirClass.Warrior;
                    DXMessageBox.Show("错误选择了无效的职业".Lang(), "角色创建".Lang());
                    break;
                case NewCharacterResult.ClassDisabled:
                    DXMessageBox.Show("所选职业目前无法使用".Lang(), "角色创建".Lang());
                    break;
                case NewCharacterResult.MaxCharacters:
                    select.CharacterBox.Close();
                    DXMessageBox.Show("已达到角色数量限制最多只能创建2个".Lang(), "角色创建".Lang());
                    break;
                case NewCharacterResult.AlreadyExists:
                    select.CharacterBox.CharacterNameTextBox.SetFocus();
                    DXMessageBox.Show("角色已经存在".Lang(), "角色创建".Lang());
                    break;
                case NewCharacterResult.Success:
                    select.CharacterBox.Close();
                    List<SelectInfo> list = new List<SelectInfo>();
                    list.Add(p.Character);
                    foreach (SelectInfo character in select.SelectBox.CharacterList)
                        list.Add(character);
                    select.SelectBox.CharacterList = list;
                    select.SelectBox.CurrentPage = 0;
                    select.SelectBox.UpdateCharacters();
                    DXMessageBox.Show("角色创建成功".Lang(), "角色创建".Lang());
                    break;
            }
        }
        public void Process(S.DeleteCharacter p)   //删除角色
        {
            SelectScene select = DXControl.ActiveScene as SelectScene;
            if (select == null) return;

            select.SelectBox.DeleteAttempted = false;
            switch (p.Result)
            {
                case DeleteCharacterResult.Disabled:
                    DXMessageBox.Show("目前已禁止删除角色".Lang(), "删除角色".Lang());
                    break;
                case DeleteCharacterResult.AlreadyDeleted:
                    DXMessageBox.Show("角色已被删除".Lang(), "删除角色".Lang());
                    break;
                case DeleteCharacterResult.NotFound:
                    DXMessageBox.Show("找不到角色".Lang(), "删除角色".Lang());
                    break;
                case DeleteCharacterResult.Success:
                    {
                        for (int i = select.SelectBox.CharacterList.Count - 1; i >= 0; i--)
                        {
                            var si = select.SelectBox.CharacterList[i];
                            if (si.CharacterIndex != p.DeletedIndex) continue;

                            select.SelectBox.CharacterList.Remove(si);
                            break;
                        }
                        select.SelectBox.UpdateCharacters();

                        DXMessageBox.Show("角色已被删除".Lang(), "删除角色".Lang());
                    }
                    break;
            }
        }
        public void Process(S.RequestStartGame p)  //尝试开始游戏
        {
            try
            {
                SelectScene select = DXControl.ActiveScene as SelectScene;
                if (select == null) return;

                DXMessageBox box;
                DateTime expiry;
                switch (p.Result)
                {
                    case StartGameResult.Sell:
                        box = DXMessageBox.Show("当前角色出售中，禁止开始游戏".Lang(), "开始游戏".Lang());
                        box.OKButton.MouseClick += (o, e) => select.SelectBox.StartGameAttempted = false;
                        break;
                    case StartGameResult.Disabled:
                        box = DXMessageBox.Show("目前已禁止开始游戏".Lang(), "开始游戏".Lang());
                        box.OKButton.MouseClick += (o, e) => select.SelectBox.StartGameAttempted = false;
                        break;
                    case StartGameResult.Deleted:
                        box = DXMessageBox.Show("你无法使用已删除的角色开始游戏".Lang(), "开始游戏".Lang());
                        box.OKButton.MouseClick += (o, e) => select.SelectBox.StartGameAttempted = false;
                        break;
                    case StartGameResult.Delayed:
                        expiry = CEnvir.Now.Add(p.Duration);

                        box = DXMessageBox.Show($"该角色刚刚退出游戏请稍候".Lang() + "\n" +
                                                $"Cconnection.Duration".Lang(Math.Floor(p.Duration.TotalHours).ToString("#,##0"), p.Duration.Minutes, p.Duration.Seconds), "开始游戏".Lang());

                        box.OKButton.MouseClick += (o, e) => select.SelectBox.StartGameAttempted = false;

                        box.ProcessAction = () =>
                        {
                            if (CEnvir.Now > expiry)
                            {
                                box.ProcessAction = null;
                                box.Visible = false;
                                select.SelectBox.StartGameAttempted = false;
                                if (select.SelectBox.CanStartGame)
                                {
                                    select.SelectBox.DoStartGame();
                                }
                                return;
                            }

                            TimeSpan remaining = expiry - CEnvir.Now;

                            box.Label.Text = $"该角色刚刚退出游戏请稍候".Lang() + "\n" +
                                             $"Cconnection.Duration".Lang(Math.Floor(remaining.TotalHours).ToString("#,##0"), remaining.Minutes, remaining.Seconds);
                        };
                        break;
                    case StartGameResult.UnableToSpawn:
                        box = DXMessageBox.Show("无法启动游戏无法生成角色".Lang(), "开始游戏".Lang());
                        box.OKButton.MouseClick += (o, e) => select.SelectBox.StartGameAttempted = false;
                        break;
                    case StartGameResult.NotFound:
                        box = DXMessageBox.Show("无法启动游戏找不到角色".Lang(), "开始游戏".Lang());
                        box.OKButton.MouseClick += (o, e) => select.SelectBox.StartGameAttempted = false;
                        break;
                    case StartGameResult.Success:

                        //重新初始化大补帖配置，以免加载到上一位角色配置
                        BigPatchConfig.Init();
                        //优先加载角色本地配置文件
                        CConfigReader.Load(select.SelectBox.CurrSelCharacter.CharacterName, select.SelectBox.CurrSelCharacter.CharacterIndex);

                        HookHelper.LoadAutoPotionConfig();
                        //HookHelper.LoadPickFilterConfig(value);  //相聚没有物品过滤
                        HookHelper.LoadBossFilterConfig();

                        DXAnimatedControl obj = new DXAnimatedControl
                        {
                            BaseIndex = 500,
                            LibraryFile = LibraryFile.Wemade,
                            AnimationDelay = TimeSpan.FromMilliseconds(1368),
                            AnimationStart = DateTime.MinValue,
                            FrameCount = 40,
                            Parent = select.SelectBox,
                            Loop = false,
                            UseOffSet = true,
                            Visible = true,
                        };
                        DXSoundManager.StopAllSounds();
                        DXSoundManager.Play(SoundIndex.StartGame145);
                        obj.AfterAnimation += (o, e) =>
                        {
                            obj.Visible = false;

                            select.SelectBox.StartGameAttempted = false;
                            select.CharIndex = p.CharacterIndex;
                            select.NextScence = true;

                            CEnvir.ClientControl = p.ClientControl;
                        };
                        break;
                }
            }
            catch (Exception e)
            {
                CEnvir.SaveError(e.ToString());
                throw;
            }
        }
        public void Process(S.StartGame p)  //开始游戏
        {
            Config.ShortcutEnabled = p.ShortcutEnabled;

            try
            {
                LoadScene load = DXControl.ActiveScene as LoadScene;
                if (load == null) return;

                if (p.Result == StartGameResult.Success)
                {
                    var scene = GameScene.Game;
                    scene.MapControl.MapInfo = Globals.MapInfoList.Binding.FirstOrDefault(x => x.Index == p.StartInformation.MapIndex);
                    scene.User = new UserObject(p.StartInformation);

                    scene.IsVisible = true;
                    DXControl.ActiveScene = scene;
                    load.StartGameAttempted = false;
                    load.Dispose();
                    DXManager.ForceReworkTextures(forceNow: true);

                    if (scene.User.Level == 0)   //第一次登录游戏的时候
                    {
                        CEnvir.ResetKeyBinds();          //重置按键绑定
                        CEnvir.ResetChatColourBinds();   //重置文字颜色
                    }

                    GameScene.Game.QuestLog = p.StartInformation.Quests;
                    GameScene.Game.QuestBox.PopulateQuests();
                    GameScene.Game.QuestTrackerBox.PopulateQuests();
                    GameScene.Game.CheckNewQuests();
                    GameScene.Game.AchievementLog = p.StartInformation.Achievements;  //成就日志

                    GameScene.Game.NPCAdoptCompanionBox.AvailableCompanions = p.StartInformation.AvailableCompanions;
                    GameScene.Game.NPCAdoptCompanionBox.RefreshUnlockButton();

                    GameScene.Game.NPCCompanionStorageBox.Companions = p.StartInformation.Companions;
                    GameScene.Game.NPCCompanionStorageBox.UpdateScrollBar();

                    GameScene.Game.Companion = GameScene.Game.NPCCompanionStorageBox.Companions.FirstOrDefault(x => x.Index == p.StartInformation.Companion);
                    GameScene.Game.PatchGridSize = p.StartInformation.PatchGridSize;//碎片包裹
                    GameScene.Game.BuffBox.BuffsChanged();
                    GameScene.Game.RankingBox.Observable = p.StartInformation.Observable;

                    GameScene.Game.StorageSize = p.StartInformation.StorageSize; //仓库

                    CEnvir.Enqueue(new C.StorageItemRefresh());  //仓库道具刷新
                    GameScene.Game.VowBox.RemainingFreeTosses = p.StartInformation.FreeTossCount; //剩余免费投币次数

                    CEnvir.Enqueue(new C.ShortcutDialogClicked { });  //显示UI按钮发包

                    if (!string.IsNullOrEmpty(p.Message)) DXMessageBox.Show(p.Message, "开始游戏".Lang());
                }
                else
                {
                    var login = new LoginScene(Config.IntroSceneSize);
                    DXControl.ActiveScene = login;
                    load.Dispose();
                }
            }
            catch (Exception e)
            {
                CEnvir.SaveError(e.ToString());
                throw;
            }
        }

        public void Process(S.MapChanged p)  //地图改变
        {
            GameScene.Game.MapControl.MapInfo = Globals.MapInfoList.Binding.FirstOrDefault(x => x.Index == p.MapIndex);

            MapObject.User.NameChanged();
        }

        public void Process(S.MapTime p)  //地图时间
        {
            if (p.OnOff == false)
            {
                GameScene.Game.NPCTopTagBox.Visible = false;
                GameScene.Game.NPCTopTagBox.ProcessAction = null;
            }
            else
            {
                //DateTime expiry;
                GameScene.Game.NPCTopTagBox.Expiry = CEnvir.Now.Add(p.MapRemaining);
                //GameScene.Game.NPCTopTagBox.MapLabel.Text = GameScene.Game.MapControl.MapInfo.Description;
                GameScene.Game.NPCTopTagBox.MapLabel.Text = "";
                GameScene.Game.NPCTopTagBox.Visible = true;
                if (GameScene.Game.NPCTopTagBox.ProcessAction == null)
                {
                    GameScene.Game.NPCTopTagBox.ProcessAction = () =>
                    {
                        if (CEnvir.Now > GameScene.Game.NPCTopTagBox.Expiry)
                        {
                            GameScene.Game.NPCTopTagBox.Visible = false;
                            GameScene.Game.NPCTopTagBox.ProcessAction = null;
                            return;
                        }
                        TimeSpan remaining = GameScene.Game.NPCTopTagBox.Expiry - CEnvir.Now;

                        GameScene.Game.NPCTopTagBox.TimeLabel.Text = $"{Math.Floor(remaining.TotalHours):#,##0}:{remaining.Minutes:#,##0}:{remaining.Seconds}";
                    };
                }
            }

            if (p.ExpiryOnff == false)
            {
                GameScene.Game.NPCReplicaBox.Visible = false;
                GameScene.Game.NPCReplicaBox.ProcessAction = null;
            }
            else
            {
                //DateTime expiry;
                GameScene.Game.NPCReplicaBox.Expiry = CEnvir.Now.Add(p.ExpiryRemaining);
                GameScene.Game.NPCReplicaBox.ExplainLabel.Text = GameScene.Game.MapControl.MapInfo.Description;
                GameScene.Game.NPCReplicaBox.Visible = true;
                if (GameScene.Game.NPCReplicaBox.ProcessAction == null)
                {
                    GameScene.Game.NPCReplicaBox.ProcessAction = () =>
                    {
                        if (CEnvir.Now > GameScene.Game.NPCReplicaBox.Expiry)
                        {
                            GameScene.Game.NPCReplicaBox.Visible = false;
                            GameScene.Game.NPCReplicaBox.ProcessAction = null;
                            return;
                        }
                        TimeSpan remaining = GameScene.Game.NPCReplicaBox.Expiry - CEnvir.Now;

                        GameScene.Game.NPCReplicaBox.TimeLabel.Text = $"{Math.Floor(remaining.TotalHours):#,##0}:{remaining.Minutes:#,##0}:{remaining.Seconds}";
                    };
                }
            }
        }
        public void Process(S.DayChanged p)  //天数改变
        {
            GameScene.Game.DayTime = p.DayTime;
        }

        public void Process(S.UserLocation p)  //角色位置
        {
            GameScene.Game.Displacement(p.Direction, p.Location);
        }
        public void Process(S.ObjectRemove p)  //对象删除
        {
            if (p.ObjectID == GameScene.Game.NPCID)
                GameScene.Game.NPCBox.Visible = false;

            if (MapObject.TargetObject != null && MapObject.TargetObject.ObjectID == p.ObjectID)
                MapObject.TargetObject = null;

            if (MapObject.MouseObject != null && MapObject.MouseObject.ObjectID == p.ObjectID)
                MapObject.MouseObject = null;

            if (MapObject.MagicObject != null && MapObject.MagicObject.ObjectID == p.ObjectID)
                MapObject.MagicObject = null;

            if (GameScene.Game.FocusObject != null && GameScene.Game.FocusObject.ObjectID == p.ObjectID)
                GameScene.Game.FocusObject = null;

            if (GameScene.Game.MonsterBox.Monster != null && GameScene.Game.MonsterBox.Monster.ObjectID == p.ObjectID)
                GameScene.Game.MonsterBox.Monster = null;

            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
            {
                if (ob.ObjectID != p.ObjectID) continue;

                ob.Remove();
                return;
            }
        }
        public void Process(S.ObjectPlayer p)   //玩家对象
        {
            new PlayerObject(p);
        }
        public void Process(S.ObjectItem p)   //道具对象
        {
            new ItemObject(p);
        }

        public void Process(S.InventoryRefresh p)     //刷新包裹
        {
            if (p.Success)
                GameScene.Game.RefreshItems(p.Items, p.GridType);
        }

        public void Process(S.ObjectMonster p)   //怪物对象
        {
            new MonsterObject(p);
        }
        /// <summary>
        /// 攻城旗帜
        /// </summary>
        /// <param name="p"></param>
        public void Process(S.CustomNpc p)
        {
            new NPCObject(p);
        }
        public void Process(S.ObjectNPC p)    //NPC对象
        {
            new NPCObject(p);
        }
        public void Process(S.ObjectSpell p)   //目标施法对象
        {
            new SpellObject(p);
        }
        public void Process(S.ObjectSpellChanged p)   //目标施法对象改变
        {
            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
            {
                if (ob.ObjectID != p.ObjectID) continue;

                SpellObject spell = (SpellObject)ob;
                spell.Power = p.Power;
                spell.UpdateLibraries();
                return;
            }
        }
        public void Process(S.PlayerUpdate p)  //玩家更新
        {
            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
            {
                if (ob.Race != ObjectType.Player || ob.ObjectID != p.ObjectID) continue;

                PlayerObject player = (PlayerObject)ob;

                player.LibraryWeaponShape = p.Weapon;    //武器
                player.ArmourShape = p.Armour;           //衣服
                player.ArmourColour = p.ArmourColour;    //衣服颜色
                player.HelmetShape = p.Helmet;           //头盔
                player.HorseShape = p.HorseArmour;       //坐骑
                player.ShieldShape = p.Shield;           //盾牌
                player.ArmourImage = p.ArmourImage;      //衣服图像
                player.WeaponImage = p.WeaponImage;      //武器图像
                player.EmblemShape = p.Emblem;           //徽章效果
                player.FashionShape = p.Fashion;         //时装
                player.FashionImage = p.FashionImage;    //时装图像
                player.ArmourIndex = p.ArmourIndex;      //衣服序号
                player.WeaponIndex = p.WeaponIndex;      //武器序号

                player.Light = p.Light;
                if (player == MapObject.User)
                    player.Light = Math.Max(p.Light, 3);

                player.UpdateLibraries();
                return;
            }
        }

        public void Process(S.ObjectTurn p)   //转动对象
        {
            if (MapObject.User.ObjectID == p.ObjectID && !GameScene.Game.Observer)
            {
                if (MapObject.User.CurrentLocation != p.Location || MapObject.User.Direction != p.Direction)
                    GameScene.Game.Displacement(p.Direction, p.Location);

                MapObject.User.ServerTime = DateTime.MinValue;

                MapObject.User.NextActionTime += p.Slow;
                return;
            }

            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
            {
                if (ob.ObjectID != p.ObjectID) continue;

                ob.ActionQueue.Add(new ObjectAction(MirAction.Standing, p.Direction, p.Location));
                return;
            }
        }
        public void Process(S.ObjectHarvest p)   //收割对象 挖肉
        {
            if (MapObject.User.ObjectID == p.ObjectID && !GameScene.Game.Observer)
            {
                if (MapObject.User.CurrentLocation != p.Location || MapObject.User.Direction != p.Direction)
                    GameScene.Game.Displacement(p.Direction, p.Location);

                MapObject.User.ServerTime = DateTime.MinValue;
                MapObject.User.NextActionTime += p.Slow;
                return;
            }

            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
            {
                if (ob.ObjectID != p.ObjectID) continue;

                ob.ActionQueue.Add(new ObjectAction(MirAction.Harvest, p.Direction, p.Location));
                return;
            }
        }
        public void Process(S.ObjectShow p)  //显示对象
        {
            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
            {
                if (ob.ObjectID != p.ObjectID) continue;
                if (ob.Race == ObjectType.Monster)
                {
                    switch (((MonsterObject)ob).Image)
                    {
                        case MonsterImage.VoraciousGhost:
                        case MonsterImage.DevouringGhost:
                        case MonsterImage.CorpseRaisingGhost:
                            ob.Visible = true;
                            ob.Dead = false;
                            break;
                    }
                }

                ob.ActionQueue.Add(new ObjectAction(MirAction.Show, p.Direction, p.Location));
                return;
            }
        }
        public void Process(S.ObjectHide p)  //隐藏对象
        {
            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
            {
                if (ob.ObjectID != p.ObjectID) continue;

                ob.ActionQueue.Add(new ObjectAction(MirAction.Hide, p.Direction, p.Location));
                return;
            }
        }
        public void Process(S.ObjectMove p)  //移动对象
        {
            if (MapObject.User.ObjectID == p.ObjectID && !GameScene.Game.Observer)
            {
                if (MapObject.User.CurrentLocation != p.Location || MapObject.User.Direction != p.Direction)
                    GameScene.Game.Displacement(p.Direction, p.Location);
                MapObject.User.ServerTime = DateTime.MinValue;

                MapObject.User.NextActionTime += p.Slow;
                return;
            }

            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
            {
                if (ob.ObjectID != p.ObjectID) continue;

                ob.ActionQueue.Add(new ObjectAction(MirAction.Moving, p.Direction, p.Location, p.Distance, MagicType.None));
                return;
            }
        }
        public void Process(S.ObjectPushed p)  //推动对象
        {
            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
            {
                if (ob.ObjectID != p.ObjectID) continue;
                GameScene.Game.CanPush = true;
                if (MapObject.User.ObjectID == p.ObjectID) MapObject.User.NextRunTime = CEnvir.Now.AddSeconds(2);
                ob.ActionQueue.Add(new ObjectAction(MirAction.Pushed, p.Direction, p.Location));
                return;
            }
        }
        public void Process(S.ObjectNameColour p)   //名字颜色对象
        {
            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
            {
                if (ob.ObjectID != p.ObjectID) continue;

                ob.NameColour = p.Colour;
                return;
            }
        }
        public void Process(S.ObjectMount p)   //骑马对象
        {
            if (MapObject.User.ObjectID == p.ObjectID)
            {
                MapObject.User.ServerTime = DateTime.MinValue;
                MapObject.User.NextActionTime = CEnvir.Now + TimeSpan.FromMilliseconds(CEnvir.ClientControl.GlobalsTurnTime);
            }

            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
            {
                if (ob.ObjectID != p.ObjectID) continue;

                if (ob.Race != ObjectType.Player) return;

                PlayerObject player = (PlayerObject)ob;

                player.Horse = p.Horse;
                player.HorseType = p.HorseType;

                if (player.Interupt)
                    player.FrameStart = DateTime.MinValue;
                return;
            }
        }
        public void Process(S.MountFailed p)  //骑马失败
        {
            MapObject.User.ServerTime = DateTime.MinValue;
            GameScene.Game.User.Horse = p.Horse;
        }

        public void Process(S.ObjectStruck p)   //击打对象
        {
            uint AttackObImage = 0;
            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
            {
                if (ob.ObjectID == p.ObjectID)
                {

                    if (ob.Race == ObjectType.Monster)
                    {
                        MonsterObject AttackMon = ob as MonsterObject;
                        if (AttackMon.Image == MonsterImage.DiyMonsMon)
                        {
                            AttackObImage = (uint)AttackMon.MonsterInfo.AI;
                        }
                        else
                        {
                            AttackObImage = (uint)AttackMon.Image;
                        }
                    }
                    break;
                }
            }

            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
            {
                if (ob.ObjectID != p.ObjectID) continue;

                if (ob == MapObject.User)
                {
                    if (CEnvir.ClientControl.OnRockCheck && BigPatchConfig.ChkAvertShake) continue;//稳如泰山
                    //GameScene.Game.CanRun = false;  
                    //MapObject.User.NextRunTime = CEnvir.Now.AddMilliseconds(600);
                    //MapObject.User.NextActionTime = CEnvir.Now.AddMilliseconds(300);

                    /*if (MapObject.User.ServerTime > DateTime.MinValue) //fix desyncing attack timers and being struck
                    {
                        switch (MapObject.User.CurrentAction)
                        {
                            case MirAction.Attack:
                            case MirAction.RangeAttack:
                                MapObject.User.AttackTime += TimeSpan.FromMilliseconds(300);
                                break;
                            case MirAction.Spell:
                                MapObject.User.NextMagicTime += TimeSpan.FromMilliseconds(300);
                                break;
                        }
                    }*/
                }

                Point loc = ob.ActionQueue.Count > 0 ? ob.ActionQueue[ob.ActionQueue.Count - 1].Location : ob.CurrentLocation;

                ob.ActionQueue.Add(new ObjectAction(MirAction.Struck, p.Direction, loc, p.AttackerID, p.Element));

                ob.Struck(AttackObImage, p.Element);

                //ob.Struck(p.AttackerID, p.Element);

                return;
            }
        }
        public void Process(S.ObjectDash p)  //突进冲撞对象
        {
            if (MapObject.User.ObjectID == p.ObjectID && !GameScene.Game.Observer)
                MapObject.User.ServerTime = DateTime.MinValue;

            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
            {
                if (ob.ObjectID != p.ObjectID) continue;

                ob.StanceTime = CEnvir.Now.AddSeconds(3);
                GameScene.Game.CanPush = true;
                //if (MapObject.User.ObjectID == p.ObjectID) MapObject.User.NextRunTime = CEnvir.Now.AddSeconds(2);
                MapObject.User.YeManCanRun = true;
                ob.ActionQueue.Add(new ObjectAction(MirAction.Standing, p.Direction, Functions.Move(p.Location, p.Direction, -p.Distance)));

                for (int i = 1; i <= p.Distance; i++)
                    ob.ActionQueue.Add(new ObjectAction(MirAction.Moving, p.Direction, Functions.Move(p.Location, p.Direction, i - p.Distance), 1, p.Magic));

                return;
            }
        }
        public void Process(S.ObjectAttack p)   //攻击对象
        {
            if (MapObject.User.ObjectID == p.ObjectID && !GameScene.Game.Observer && p.AttackMagic != MagicType.DanceOfSwallow)
            {
                if (MapObject.User.CurrentLocation != p.Location || MapObject.User.Direction != p.Direction)
                    GameScene.Game.Displacement(p.Direction, p.Location);

                MapObject.User.ServerTime = DateTime.MinValue;

                MapObject.User.NextActionTime += p.Slow;
                return;
            }

            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
            {
                if (ob.ObjectID != p.ObjectID) continue;

                ob.ActionQueue.Add(new ObjectAction(MirAction.Attack, p.Direction, p.Location, p.TargetID, p.AttackMagic, p.AttackElement));
                return;
            }
        }
        public void Process(S.ObjectMining p)  //挖矿对象
        {
            if (MapObject.User.ObjectID == p.ObjectID && !GameScene.Game.Observer)
            {
                if (MapObject.User.CurrentLocation != p.Location || MapObject.User.Direction != p.Direction)
                    GameScene.Game.Displacement(p.Direction, p.Location);

                MapObject.User.ServerTime = DateTime.MinValue;

                MapObject.User.NextActionTime += p.Slow;
                MapObject.User.MiningEffect = p.Effect;
                return;
            }

            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
            {
                if (ob.ObjectID != p.ObjectID) continue;

                ob.ActionQueue.Add(new ObjectAction(MirAction.Mining, p.Direction, p.Location, p.Effect));
                return;
            }
        }
        public void Process(S.DiyObjectMagic p)  //怪物自定义
        {
            //加入已有技能判断，使用新的模式，魔法技能走这里
            //MagicType Magictype = MagicType.None;
            //if (Enum.IsDefined(typeof(MagicType), (object)p.SpellMagicID))
            //{
            //    Magictype = (MagicType)p.SpellMagicID;
            //}

            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
            {
                if (ob.ObjectID != p.ObjectID) continue;

                ob.ActionQueue.Add(new ObjectAction(MirAction.DiySpell, p.Direction, p.CurrentLocation, p.SpellMagicID, p.Targets, p.Locations, p.Cast, p.AttackElement, p.ActID));
                return;
            }
        }

        public void Process(S.DiyObjectAttack p)
        {
            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
            {
                if (ob.ObjectID != p.ObjectID) continue;

                ob.ActionQueue.Add(new ObjectAction(MirAction.DiyAttack, p.Direction, p.Location, p.TargetID, p.SpellMagicID, p.AttackElement, p.ActID));
                return;
            }
        }
        public void Process(S.ObjectRangeAttack p)  //远程攻击对象
        {
            if (MapObject.User.ObjectID == p.ObjectID && !GameScene.Game.Observer)
            {
                if (MapObject.User.CurrentLocation != p.Location || MapObject.User.Direction != p.Direction)
                    GameScene.Game.Displacement(p.Direction, p.Location);

                MapObject.User.ServerTime = DateTime.MinValue;
                return;
            }

            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
            {
                if (ob.ObjectID != p.ObjectID) continue;

                ob.ActionQueue.Add(new ObjectAction(MirAction.RangeAttack, p.Direction, p.Location, p.Targets, p.AttackMagic, p.AttackElement));
                return;
            }
        }
        public void Process(S.ObjectMagic p)  //魔法技能对象
        {
            if (MapObject.User.ObjectID == p.ObjectID && !GameScene.Game.Observer)
            {
                if (MapObject.User.CurrentLocation != p.CurrentLocation || MapObject.User.Direction != p.Direction)
                    GameScene.Game.Displacement(p.Direction, p.CurrentLocation);

                MapObject.User.ServerTime = DateTime.MinValue;

                MapObject.User.AttackTargets = new List<MapObject>();

                foreach (uint target in p.Targets)
                {
                    MapObject attackTarget = GameScene.Game.MapControl.Objects.FirstOrDefault(x => x.ObjectID == target);

                    if (attackTarget == null) continue;

                    MapObject.User.AttackTargets.Add(attackTarget);
                }

                MapObject.User.MagicLocations = p.Locations;
                MapObject.User.MagicCast = p.Cast;
                MapObject.User.NextActionTime += p.Slow;
                return;
            }

            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
            {
                if (ob.ObjectID != p.ObjectID) continue;

                ob.ActionQueue.Add(new ObjectAction(MirAction.Spell, p.Direction, p.CurrentLocation, p.Type, p.Targets, p.Locations, p.Cast, p.AttackElement));
                return;
            }
        }
        public void Process(S.ObjectDied p)  //死亡对象
        {
            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
            {
                if (ob.ObjectID != p.ObjectID) continue;

                ob.Dead = true;
                ob.ActionQueue.Add(new ObjectAction(MirAction.Die, p.Direction, p.Location));

                if (ob == MapObject.User)
                    GameScene.Game.User.WarReviveTime = CEnvir.Now;
                //GameScene.Game.ChatBox.ReceiveChat($"Chat.Revive".Lang(), MessageType.Revive);

                if (ob.ObjectID == GameScene.Game.WarWeaponID)
                {
                    GameScene.Game.WarWeaponID = 0;
                    GameScene.Game.WarWeaponBox.Visible = false;
                }
                return;
            }
        }
        public void Process(S.ObjectHarvested p)  //收获对象 捕猎
        {
            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
            {
                if (ob.ObjectID != p.ObjectID) continue;

                ob.Skeleton = true;

                ob.ActionQueue.Add(new ObjectAction(MirAction.Dead, p.Direction, p.Location));

                return;
            }
        }
        public void Process(S.ObjectEffect p)  //特效对象
        {
            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
            {
                if (ob.ObjectID != p.ObjectID) continue;

                switch (p.Effect)
                {
                    case Effect.TeleportOut:  //传出去
                        ob.Effects.Add(new MirEffect(110, 10, TimeSpan.FromMilliseconds(50), LibraryFile.Magic, 30, 60, Color.White)
                        {
                            MapTarget = ob.CurrentLocation,
                            Blend = true,
                            Reversed = true,
                            Loop = p.Loop,
                            BlendRate = 0.6F
                        });

                        DXSoundManager.Play(SoundIndex.TeleportOut);
                        break;
                    case Effect.TeleportIn:  //传进来
                        ob.Effects.Add(new MirEffect(2390, 10, TimeSpan.FromMilliseconds(50), LibraryFile.Magic, 30, 60, Color.White)
                        {
                            Target = ob,
                            Blend = true,
                            Loop = p.Loop,
                            BlendRate = 0.6F
                        });

                        DXSoundManager.Play(SoundIndex.TeleportIn);
                        break;
                    case Effect.FullBloom:
                        ob.Effects.Add(new MirEffect(1700, 4, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx4, 30, 60, Color.White)
                        {
                            Target = ob,
                            Blend = true,
                            Loop = p.Loop,
                            BlendRate = 0.6F
                        });

                        DXSoundManager.Play(SoundIndex.FullBloom);
                        break;
                    case Effect.WhiteLotus:
                        ob.Effects.Add(new MirEffect(1600, 12, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx4, 30, 60, Color.White)
                        {
                            Target = ob,
                            Blend = true,
                            Loop = p.Loop,
                            BlendRate = 0.6F
                        });

                        DXSoundManager.Play(SoundIndex.WhiteLotus);
                        break;
                    case Effect.RedLotus:
                        ob.Effects.Add(new MirEffect(1700, 12, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx4, 30, 60, Color.White)
                        {
                            Target = ob,
                            Blend = true,
                            Loop = p.Loop,
                            BlendRate = 0.6F
                        });

                        DXSoundManager.Play(SoundIndex.RedLotus);
                        break;
                    case Effect.SweetBrier:
                        ob.Effects.Add(new MirEffect(1900, 10, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx4, 30, 60, Color.White)
                        {
                            Target = ob,
                            Blend = true,
                            Loop = p.Loop,
                            BlendRate = 0.6F
                        });

                        DXSoundManager.Play(SoundIndex.SweetBrier);
                        break;
                    case Effect.Karma:
                        ob.Effects.Add(new MirEffect(1800, 10, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx4, 30, 60, Color.White)
                        {
                            Target = ob,
                            Blend = true,
                            Loop = p.Loop,
                            BlendRate = 0.6F
                        });

                        DXSoundManager.Play(SoundIndex.Karma);
                        break;

                    case Effect.Puppet:
                        ob.Effects.Add(new MirEffect(820, 8, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx4, 30, 60, Globals.FireColour)
                        {
                            Target = ob,
                            Blend = true,
                            Loop = p.Loop,
                            BlendRate = 0.6F
                        });
                        break;
                    case Effect.PuppetFire:
                        ob.Effects.Add(new MirEffect(1546, 8, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx4, 30, 60, Globals.FireColour)
                        {
                            Target = ob,
                            Blend = true,
                            Loop = p.Loop,
                            BlendRate = 0.6F
                        });
                        break;
                    case Effect.PuppetIce:
                        ob.Effects.Add(new MirEffect(2700, 10, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx4, 30, 60, Globals.IceColour)
                        {
                            Target = ob,
                            Blend = true,
                            Loop = p.Loop,
                            BlendRate = 0.6F
                        });
                        break;
                    case Effect.PuppetLightning:
                        ob.Effects.Add(new MirEffect(2800, 10, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx4, 30, 60, Globals.LightningColour)
                        {
                            Target = ob,
                            Blend = true,
                            Loop = p.Loop,
                            BlendRate = 0.6F
                        });
                        break;
                    case Effect.PuppetWind:
                        ob.Effects.Add(new MirEffect(2900, 10, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx4, 30, 60, Globals.WindColour)
                        {
                            Target = ob,
                            Blend = true,
                            Loop = p.Loop,
                            BlendRate = 0.6F
                        });
                        break;

                    #region Thunder Bolt & Thunder Strike

                    case Effect.ThunderBolt:

                        ob.Effects.Add(new MirEffect(1450, 3, TimeSpan.FromMilliseconds(150), LibraryFile.Magic, 150, 50, Globals.LightningColour)
                        {
                            Blend = true,
                            Loop = p.Loop,
                            Target = ob
                        });

                        DXSoundManager.Play(SoundIndex.LightningStrikeEnd);
                        break;

                    #endregion

                    case Effect.DanceOfSwallow:
                        ob.Effects.Add(new MirEffect(1300, 8, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx4, 20, 70, Globals.NoneColour) //Element style?
                        {
                            Blend = true,
                            Loop = p.Loop,
                            Target = ob,
                        });

                        DXSoundManager.Play(SoundIndex.DanceOfSwallowsEnd);
                        break;
                    case Effect.FlashOfLight:
                        ob.Effects.Add(new MirEffect(2400, 5, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx4, 20, 70, Globals.NoneColour) //Element style?
                        {
                            Blend = true,
                            Loop = p.Loop,
                            Target = ob,
                        });

                        DXSoundManager.Play(SoundIndex.FlashOfLightEnd);
                        break;
                    case Effect.DemonExplosion:
                        ob.Effects.Add(new MirEffect(3300, 10, TimeSpan.FromMilliseconds(100), LibraryFile.MonMagicEx8, 30, 60, Globals.PhantomColour)
                        {
                            Target = ob,
                            Blend = true,
                            Loop = p.Loop,
                            BlendRate = 0.6F
                        });

                        //DXSoundManager.Play(SoundIndex.FlashOfLightEnd);
                        break;
                    case Effect.FrostBiteEnd:
                        ob.Effects.Add(new MirEffect(700, 7, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx5, 30, 60, Globals.IceColour)
                        {
                            Target = ob,
                            Blend = true,
                            Loop = p.Loop,
                            BlendRate = 0.6F
                        });

                        DXSoundManager.Play(SoundIndex.FireStormEnd);
                        break;
                    case Effect.Repulsion:
                        ob.Effects.Add(new MirEffect(90, 8, TimeSpan.FromMilliseconds(60), LibraryFile.Magic, 10, 35, Globals.WindColour)
                        {
                            Blend = true,
                            Target = ob,
                            Loop = p.Loop,
                            BlendRate = 0.6F,
                        });
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                return;
            }
        }
        public void Process(S.MapEffect p)  //地图特效
        {
            switch (p.Effect)
            {
                case Effect.SummonSkeleton:
                    new MirEffect(750, 10, TimeSpan.FromMilliseconds(100), LibraryFile.Magic, 30, 60, Globals.PhantomColour)
                    {
                        MapTarget = p.Location,
                        Blend = true,
                    };

                    DXSoundManager.Play(SoundIndex.SummonSkeletonEnd);
                    break;
                case Effect.SummonShinsu:
                    new MirEffect(9640, 10, TimeSpan.FromMilliseconds(100), LibraryFile.Mon_9, 30, 60, Globals.PhantomColour)
                    {
                        MapTarget = p.Location,
                        Direction = p.Direction,
                    };

                    DXSoundManager.Play(SoundIndex.SummonShinsuEnd);
                    break;
                case Effect.MirrorImage:
                    new MirEffect(1280, 10, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx2, 10, 35, Globals.IceColour)
                    {
                        MapTarget = p.Location,
                        Blend = true,
                    };
                    break;
                case Effect.MirrorImageDie:
                    new MirEffect(1300, 10, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx2, 10, 35, Globals.IceColour)
                    {
                        MapTarget = p.Location,
                        Blend = true,
                    };
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        public void Process(S.ObjectBuffAdd p)  //BUFF增加对象
        {
            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
            {
                if (ob.ObjectID != p.ObjectID) continue;

                ob.VisibleBuffs.Add(p.Type);
                if (p.Type == BuffType.CustomBuff)
                {
                    CustomBuffInfo Buff = Globals.CustomBuffInfoList.Binding.FirstOrDefault(x => x.Index == p.Index);
                    if (Buff != null)
                        ob.VisibleCustomBuffs.Add(Buff);
                }

                if (p.Type == BuffType.SuperiorMagicShield && ob.MagicShieldEffect != null)
                    ob.MagicShieldEnd();

                return;
            }
        }
        public void Process(S.ObjectBuffRemove p)   //BUFF移除对象
        {
            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
            {
                if (ob.ObjectID != p.ObjectID) continue;

                ob.VisibleBuffs.Remove(p.Type);
                if (p.Type == BuffType.CustomBuff)
                {
                    CustomBuffInfo Buff = Globals.CustomBuffInfoList.Binding.FirstOrDefault(x => x.Index == p.Index);
                    if (Buff != null)
                        ob.VisibleCustomBuffs.Remove(Buff);
                    if (Buff != null && Buff.OverheadTitle != 0 && ob.HeadTopEffect != null)
                        ob.HeadTopEnd();
                }
                return;
            }
        }
        public void Process(S.ObjectPoison p)   //毒对象
        {
            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
            {
                if (ob.ObjectID != p.ObjectID) continue;

                ob.Poison = p.Poison;
                return;
            }
        }
        public void Process(S.ObjectPetOwnerChanged p)   //宠物主人变更对象
        {
            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
            {
                if (ob.ObjectID != p.ObjectID) continue;

                if (ob.Race != ObjectType.Monster) return;

                MonsterObject mob = (MonsterObject)ob;
                mob.PetOwner = p.PetOwner;
                return;
            }
        }
        public void Process(S.ObjectStats p)   //属性状态对象
        {
            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
            {
                if (ob.ObjectID != p.ObjectID) continue;

                if (ob.Race == ObjectType.Monster)
                {
                    MonsterObject mob = (MonsterObject)ob;
                    p.Stats.Add(mob.MonsterInfo.Stats);
                }

                ob.Stats = p.Stats;
                return;
            }
        }
        public void Process(S.HealthChanged p)  //生命值改变
        {
            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
            {
                if (ob.ObjectID != p.ObjectID) continue;

                ob.CurrentHP += p.Change;
                ob.DrawHealthTime = CEnvir.Now.AddSeconds(300);
                ob.DamageList.Add(new DamageInfo { Value = p.Change, Block = p.Block, Critical = p.Critical, Miss = p.Miss, FatalAttack = p.FatalAttack, CriticalHit = p.CriticalHit, DamageAdd = p.DamageAdd, GreenPosionPro = p.GreenPosionPro, SmokingMP = p.SmokingMP });

                return;
            }
        }

        public void Process(S.NewMagic p)   //新魔法技能
        {
            MapObject.User.Magics[p.Magic.Info] = p.Magic;
            // GameScene.Game.MagicBox.Magics[p.Magic.Info].Info = p.Magic.Info;
            GameScene.Game.MagicBox?.Magics[p.Magic.Info].Refresh();  //更新魔法面板

            GameScene.Game.BigPatchBox.Commonly?.UpdateMagic();  //更新大补帖辅助技能下拉框
            GameScene.Game.BigPatchBox.Magic?.UpdateMagic();  //更新大补帖魔法技能
            GameScene.Game.BigPatchBox.Helper?.UpdateMagic();  //更新大补帖挂机技能
        }
        public void Process(S.MagicLeveled p)   //新魔法技能等级
        {
            if (p.Level == -1)
            {
                //移除技能
                if (MapObject.User.Magics.ContainsKey(p.Info))
                {
                    MapObject.User.Magics.Remove(p.Info);
                }
            }
            else
            {
                MapObject.User.Magics[p.Info].Level = p.Level;
                MapObject.User.Magics[p.Info].Experience = p.Experience;
            }
            MagicCell ToBeRefreshed; // 如果玩家拥有一个被空置的技能,此处用来防止崩溃
            if (GameScene.Game.MagicBox.Magics.TryGetValue(p.Info, out ToBeRefreshed))
                ToBeRefreshed.Refresh();
        }

        public void Process(S.MagicCooldown p)   //魔法技能冷却
        {
            MapObject.User.Magics[p.Info].NextCast = CEnvir.Now.AddMilliseconds(p.Delay);
        }
        public void Process(S.MagicToggle p)   //魔法技能切换
        {
            switch (p.Magic)
            {
                case MagicType.Slaying:
                    GameScene.Game.User.CanPowerAttack = p.CanUse;
                    break;
                case MagicType.Thrusting:
                    GameScene.Game.User.CanThrusting = p.CanUse;
                    break;
                case MagicType.HalfMoon:
                    GameScene.Game.User.CanHalfMoon = p.CanUse;
                    break;
                case MagicType.DestructiveSurge:
                    GameScene.Game.User.CanDestructiveBlow = p.CanUse;
                    break;
                case MagicType.FlamingSword:
                    GameScene.Game.User.CanFlamingSword = p.CanUse;
                    if (p.CanUse)
                        GameScene.Game.ReceiveChat("能量在你的武器中积聚".Lang() + "，" + "烈火剑法准备就绪".Lang(), MessageType.Combat);
                    break;
                case MagicType.DragonRise:
                    GameScene.Game.User.CanDragonRise = p.CanUse;
                    if (p.CanUse)
                        GameScene.Game.ReceiveChat("能量在你的武器中积聚".Lang() + "，" + "翔空剑法准备就绪".Lang(), MessageType.Combat);
                    break;
                case MagicType.BladeStorm:
                    GameScene.Game.User.CanBladeStorm = p.CanUse;
                    if (p.CanUse)
                        GameScene.Game.ReceiveChat("能量在你的武器中积聚".Lang() + "，" + "莲月剑法准备就绪".Lang(), MessageType.Combat);
                    break;
                case MagicType.MaelstromBlade:
                    GameScene.Game.User.CanMaelstromBlade = p.CanUse;
                    if (p.CanUse)
                        GameScene.Game.ReceiveChat("能量在你的武器中积聚".Lang() + "，" + "屠龙斩准备就绪".Lang(), MessageType.Combat);
                    break;
                case MagicType.FlameSplash:
                    GameScene.Game.User.CanFlameSplash = p.CanUse;
                    break;
                case MagicType.FullBloom:
                case MagicType.WhiteLotus:
                case MagicType.RedLotus:
                case MagicType.SweetBrier:
                case MagicType.Karma:
                    if (GameScene.Game.User.AttackMagic == p.Magic)
                        GameScene.Game.User.AttackMagic = MagicType.None;
                    break;
            }
        }

        public void Process(S.ManaChanged p)   //法力值改变
        {
            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
            {
                if (ob.ObjectID != p.ObjectID) continue;

                ob.CurrentMP += p.Change;
                //ob.DamageList.Add(new DamageInfo(p.Change));
                return;
            }
        }
        public void Process(S.LevelChanged p)   //等级改变
        {
            MapObject.User.Level = p.Level;
            MapObject.User.Experience = p.Experience;

            GameScene.Game.ReceiveChat("你升级了".Lang(), MessageType.Announcement);
        }
        public void Process(S.GainedExperience p)   //获得的经验值
        {
            MapObject.User.Experience += p.Amount;

            ClientUserItem weapon = GameScene.Game.Equipment[(int)EquipmentSlot.Weapon];

            if (p.Amount < 0)
            {
                GameScene.Game.ReceiveChat($"失去经验值".Lang() + $"　{p.Amount:#,##0} ", MessageType.Hint);
                return;
            }

            string message = "";
            if (p.WeapEx != 0M && weapon != null && weapon.Info.Effect != ItemEffect.PickAxe && (weapon.Flags & UserItemFlags.Refinable) != UserItemFlags.Refinable && (weapon.Flags & UserItemFlags.NonRefinable) != UserItemFlags.NonRefinable && weapon.Level < Globals.GameWeaponEXPInfoList.Count && !CEnvir.ClientControl.NewWeaponUpgradeCheck)
            {
                weapon.Experience += p.WeapEx;

                if (weapon.Experience >= Globals.GameWeaponEXPInfoList[weapon.Level].Exp)
                {
                    weapon.Experience = 0;
                    weapon.Level++;
                    weapon.Flags |= UserItemFlags.Refinable;

                    message = $"，你的武器准备好精炼了".Lang();
                }
                else
                    if (p.WeapEx != 0) message = $"，武器经验值" + $"　{p.WeapEx:#,##0}";
            }

            if (p.Amount != 0) message = $"得到经验值" + $"　{p.Amount:#,##0}" + message;

            if (p.BonusEx != 0) message = message + $"，得到忠诚度" + $"　{p.BonusEx:#,##0.00000}";

            if (!string.IsNullOrEmpty(message) && BigPatchConfig.ChkCloseExpTips)
                GameScene.Game.ReceiveChat(message, MessageType.Hint);
        }
        public void Process(S.ObjectLeveled p)  //升级对象
        {
            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
            {
                if (ob.ObjectID != p.ObjectID) continue;

                //升级显示的动画
                ob.Effects.Add(new MirEffect(2030, 16, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx, 50, 120, Color.DeepSkyBlue)
                {
                    Blend = true,
                    DrawColour = Color.RosyBrown,
                    Target = ob
                });

                return;
            }
        }

        public void Process(S.ObjectWeaponLeveled p)  //升级对象
        {
            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
            {
                if (ob.ObjectID != p.ObjectID) continue;

                //升级显示的动画
                ob.Effects.Add(new MirEffect(250, 20, TimeSpan.FromMilliseconds(100), LibraryFile.Interface, 50, 120, Color.DeepSkyBlue)
                {
                    Blend = true,
                    DrawColour = Color.RosyBrown,
                    Target = ob
                });

                return;
            }
        }

        public void Process(S.ObjectUseItem p)  //升级对象
        {
            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
            {
                if (ob.ObjectID != p.ObjectID) continue;

                //升级显示的动画
                ob.Effects.Add(new MirEffect(280, 10, TimeSpan.FromMilliseconds(100), LibraryFile.Interface, 50, 120, Color.White)
                {
                    Blend = true,
                    DrawColour = Color.White,
                    Target = ob
                });

                return;
            }
        }

        public void Process(S.ObjectRevive p)   //复活对象
        {
            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
            {
                if (ob.ObjectID != p.ObjectID) continue;

                ob.Dead = false;

                ob.ActionQueue.Add(new ObjectAction(MirAction.Standing, ob.Direction, p.Location));

                //复活显示的动画
                if (p.Effect)
                    ob.Effects.Add(new MirEffect(1110, 25, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx3, 50, 90, Color.White)
                    {
                        Blend = true,
                        Target = ob
                    });

                GameScene.Game.MapControl.FLayer.TextureValid = false;

                return;
            }
        }

        public void Process(S.StorageItemRefresh p)  //刷新仓库
        {
            if (!GameScene.Game.Observer)
                CEnvir.FillStorage(p.Items, false);
            else
                CEnvir.FillStorage(p.Items, true);

            for (int i = 0; i < GameScene.Game.StorageBox.Grid.Grid.Length; i++)
            {
                GameScene.Game.StorageBox.Grid.Grid[i].Item = null;
            }
            foreach (ClientUserItem item in p.Items)
            {

                GameScene.Game.StorageBox.Grid.Grid[item.Slot].Item = item;
            }
        }

        public void Process(S.CompanionGridRefresh p)  //刷新宠物包裹
        {
            if (!GameScene.Game.Observer)
                CEnvir.FillCompanionGrid(p.Items, false);
            else
                CEnvir.FillCompanionGrid(p.Items, true);

            for (int i = 0; i < GameScene.Game.CompanionBox.InventoryGrid.Grid.Length; i++)
            {
                GameScene.Game.CompanionBox.InventoryGrid.Grid[i].Item = null;
            }
            foreach (ClientUserItem item in p.Items)
            {

                GameScene.Game.CompanionBox.InventoryGrid.Grid[item.Slot].Item = item;
            }
        }

        public void Process(S.ItemsGained p)   //获得道具
        {
            foreach (ClientUserItem item in p.Items)
            {
                ItemInfo displayInfo = item.Info;

                if (item.Info.Effect == ItemEffect.ItemPart)
                {
                    var info = Globals.ItemInfoList.Binding.FirstOrDefault(x => x.Index == item.AddedStats[Stat.ItemIndex]);
                    if (info != null)
                        displayInfo = info;
                }

                item.New = true;
                string ss = displayInfo.Lang(p => p.ItemName);

                if (item.Refine) ss = "(*)" + ss;

                string text = item.Count > 1 ? $"得到".Lang() + $"{ss} " + "物品".Lang() + "（" + $"{item.Count}" + "个" + "）" : $"得到".Lang() + $"{ss}";

                if (item.Info.Effect == ItemEffect.Gold)
                    text = $"得到".Lang() + $"{item.Count}{ss}";

                if ((item.Flags & UserItemFlags.QuestItem) == UserItemFlags.QuestItem)
                    text += " (" + "任务".Lang() + ")";

                if (item.Info.Effect == ItemEffect.ItemPart)
                    text += " [" + "碎片".Lang() + "]";

                GameScene.Game.ReceiveChat(text, MessageType.Combat);
            }

            GameScene.Game.AddItems(p.Items);  //增加道具 TODO

            GameScene.Game.CharacterBox.UpdateBookmarkedCraftItem(); //更新人物页面制造需求
            GameScene.Game.CraftBox.UpdateCraftItemInfo();//更新制造页面制造需求
        }

        public void Process(S.ItemCellRefresh p)// 刷新一个现有item cell
        {
            DXItemCell cell;
            int slot = p.Item.Slot;

            if (slot >= Globals.FishingOffSet)
            {
                slot -= Globals.FishingOffSet;
            }
            else if (slot >= Globals.PatchOffSet)
            {
                slot -= Globals.PatchOffSet;
            }
            else if (slot >= Globals.EquipmentOffSet)
            {
                slot -= Globals.EquipmentOffSet;
            }

            switch (p.Grid)
            {
                case GridType.Inventory:
                    cell = GameScene.Game.InventoryBox.Grid.Grid[slot];
                    break;
                case GridType.PatchGrid:                                            //碎片包裹
                    cell = GameScene.Game.InventoryBox.PatchGrid.Grid[slot];
                    break;
                case GridType.Equipment:
                    cell = GameScene.Game.CharacterBox.Grid[slot];
                    break;
                case GridType.Storage:
                    cell = GameScene.Game.StorageBox.Grid.Grid[slot];
                    break;
                case GridType.GuildStorage:
                    cell = GameScene.Game.GuildBox.StorageGrid.Grid[slot];
                    break;
                case GridType.CompanionInventory:
                    cell = GameScene.Game.CompanionBox.InventoryGrid.Grid[slot];
                    break;
                case GridType.CompanionEquipment:
                    cell = GameScene.Game.CompanionBox.EquipmentGrid[slot];
                    break;
                case GridType.FishingEquipment:   //钓鱼装备格子
                    cell = GameScene.Game.FishingBox.FishingGrid[slot];
                    break;
                default: return;
            }
            cell.Item = p.Item;
            cell.RefreshItem();
        }

        public void Process(S.ItemMove p)  //道具移动过程
        {
            DXItemCell fromCell, toCell;

            switch (p.FromGrid) //从包裹切换
            {
                case GridType.Inventory:
                    fromCell = GameScene.Game.InventoryBox.Grid.Grid[p.FromSlot];
                    break;
                case GridType.PatchGrid:                                            //碎片包裹
                    fromCell = GameScene.Game.InventoryBox.PatchGrid.Grid[p.FromSlot];
                    break;
                case GridType.Equipment:
                    fromCell = GameScene.Game.CharacterBox.Grid[p.FromSlot];
                    break;
                case GridType.Storage:
                    fromCell = GameScene.Game.StorageBox.Grid.Grid[p.FromSlot];
                    break;
                case GridType.GuildStorage:
                    fromCell = GameScene.Game.GuildBox.StorageGrid.Grid[p.FromSlot];
                    break;
                case GridType.CompanionInventory:
                    fromCell = GameScene.Game.CompanionBox.InventoryGrid.Grid[p.FromSlot];
                    break;
                case GridType.CompanionEquipment:
                    fromCell = GameScene.Game.CompanionBox.EquipmentGrid[p.FromSlot];
                    break;
                case GridType.FishingEquipment:   //钓鱼装备格子
                    fromCell = GameScene.Game.FishingBox.FishingGrid[p.FromSlot];
                    break;
                default: return;
            }

            switch (p.ToGrid) //转换到包裹
            {
                case GridType.Inventory:
                    toCell = GameScene.Game.InventoryBox.Grid.Grid[p.ToSlot];
                    break;
                case GridType.PatchGrid:                                            //碎片包裹
                    toCell = GameScene.Game.InventoryBox.PatchGrid.Grid[p.ToSlot];
                    break;
                case GridType.Equipment:
                    toCell = GameScene.Game.CharacterBox.Grid[p.ToSlot];
                    break;
                case GridType.Storage:
                    toCell = GameScene.Game.StorageBox.Grid.Grid[p.ToSlot];
                    break;
                case GridType.GuildStorage:
                    toCell = GameScene.Game.GuildBox.StorageGrid.Grid[p.ToSlot];
                    break;
                case GridType.CompanionInventory:
                    toCell = GameScene.Game.CompanionBox.InventoryGrid.Grid[p.ToSlot];
                    break;
                case GridType.CompanionEquipment:
                    toCell = GameScene.Game.CompanionBox.EquipmentGrid[p.ToSlot];
                    break;
                case GridType.FishingEquipment: //钓鱼装备格子
                    toCell = GameScene.Game.FishingBox.FishingGrid[p.ToSlot];
                    break;
                default:
                    return;
            }

            toCell.Locked = false;
            fromCell.Locked = false;

            if (!p.Success) return;


            if (p.FromGrid != p.ToGrid)
            {
                if (p.FromGrid == GridType.Inventory) //从背包里取走
                {
                    if (!fromCell.Item.Info.ShouldLinkInfo)
                    {
                        for (int i = 0; i < GameScene.Game.BeltBox.Links.Length; i++)
                        {
                            ClientBeltLink link = GameScene.Game.BeltBox.Links[i];
                            if (link.LinkItemIndex != fromCell.Item.Index) continue;

                            link.LinkItemIndex = -1;

                            if (i < GameScene.Game.BeltBox.Grid.Grid.Length)
                                GameScene.Game.BeltBox.Grid.Grid[i].QuickItem = null; //将快捷栏设置为空

                            if (p.ToGrid == GridType.Equipment && toCell.Item != null) //用其他道具替换道具（如果是装备）
                            {
                                link.LinkItemIndex = toCell.Item.Index;

                                if (i < GameScene.Game.BeltBox.Grid.Grid.Length)
                                    GameScene.Game.BeltBox.Grid.Grid[i].QuickItem = toCell.Item; //将快捷栏设置为项目
                            }

                            if (!GameScene.Game.Observer)
                                CEnvir.Enqueue(new C.BeltLinkChanged { Slot = link.Slot, LinkIndex = link.LinkInfoIndex, LinkItemIndex = link.LinkItemIndex }); //更新服务器
                        }
                    }
                }
                else if (p.ToGrid == GridType.Inventory && toCell.Item != null) //移动到背包
                {
                    if (!toCell.Item.Info.ShouldLinkInfo)
                    {
                        for (int i = 0; i < GameScene.Game.BeltBox.Links.Length; i++)
                        {
                            ClientBeltLink link = GameScene.Game.BeltBox.Links[i];
                            if (link.LinkItemIndex != toCell.Item.Index) continue;

                            link.LinkItemIndex = -1;

                            if (i < GameScene.Game.BeltBox.Grid.Grid.Length)
                                GameScene.Game.BeltBox.Grid.Grid[i].QuickItem = null; //将快捷栏设置为空

                            if (!GameScene.Game.Observer)
                                CEnvir.Enqueue(new C.BeltLinkChanged { Slot = link.Slot, LinkIndex = link.LinkInfoIndex, LinkItemIndex = link.LinkItemIndex }); //更新服务器
                        }
                    }
                }
            }

            if (p.MergeItem)   //合并道具
            {
                if (toCell.Item.Count + fromCell.Item.Count <= toCell.Item.Info.StackSize)
                {
                    toCell.Item.Count += fromCell.Item.Count;
                    fromCell.Item = null;
                    toCell.RefreshItem();

                    return;
                }

                fromCell.Item.Count -= fromCell.Item.Info.StackSize - toCell.Item.Count;
                toCell.Item.Count = toCell.Item.Info.StackSize;
                fromCell.RefreshItem();
                toCell.RefreshItem();
                return;
            }

            ClientUserItem temp = toCell.Item;

            toCell.Item = fromCell.Item;
            fromCell.Item = temp;

            //if (p.ToGrid == GridType.GuildStorage || p.FromGrid == GridType.GuildStorage)
            //    GameScene.Game.GuildPanel.StorageControl.ItemCount = GameScene.Game.GuildStorage.Count(x => x != null);
        }
        public void Process(S.GoldChanged p)  //金币改变
        {
            if (p.Gold < GameScene.Game.User.Gold)   // && Config.ChkCloseExpTips
                GameScene.Game.ReceiveChat($"Cconnection.GoldChanged".Lang((GameScene.Game.User.Gold - p.Gold).ToString("###0")), MessageType.Hint);
            GameScene.Game.User.Gold = p.Gold;

            DXSoundManager.Play(SoundIndex.ItemGold);
        }
        public void Process(S.GameGoldChanged p)  //元宝改变
        {
            if (p.GameGold < GameScene.Game.User.GameGold)  // && Config.ChkCloseExpTips
                GameScene.Game.ReceiveChat($"Cconnection.GameGoldChanged".Lang((Convert.ToDecimal(GameScene.Game.User.GameGold - p.GameGold) / 100).ToString("###0.00")), MessageType.Hint);
            GameScene.Game.User.GameGold = p.GameGold;
        }
        public void Process(S.HuntGoldChanged p)  //赏金改变
        {
            if (p.HuntGold < GameScene.Game.User.HuntGold)  // && Config.ChkCloseExpTips
                GameScene.Game.ReceiveChat($"Cconnection.HuntGoldChanged".Lang((GameScene.Game.User.HuntGold - p.HuntGold).ToString("###0")), MessageType.Hint);
            GameScene.Game.User.HuntGold = p.HuntGold;
        }
        public void Process(S.PrestigeChanged p)  //声望改变
        {
            if (p.Prestige < GameScene.Game.User.Prestige)  // && Config.ChkCloseExpTips
                GameScene.Game.ReceiveChat($"Cconnection.PrestigeChanged".Lang((GameScene.Game.User.Prestige - p.Prestige).ToString("###0")), MessageType.Hint);
            GameScene.Game.User.Prestige = p.Prestige;
        }
        public void Process(S.ContributeChanged p)  //贡献改变
        {
            if (p.Contribute < GameScene.Game.User.Contribute)  // && Config.ChkCloseExpTips
                GameScene.Game.ReceiveChat($"Cconnection.ContributeChanged".Lang((GameScene.Game.User.Contribute - p.Contribute).ToString("###0")), MessageType.Hint);
            GameScene.Game.User.Contribute = p.Contribute;
        }
        public void Process(S.AutoTimeChanged p)  //自动挂机时间改变
        {
            if (GameScene.Game.User == null) return;
            GameScene.Game.User.AutoTime = p.AutoTime;
        }
        public void Process(S.TreasureChest p)   //打开传奇宝箱
        {
            GameScene.Game.TreasureChestBox.Visible = true;
            GameScene.Game.TreasureChestBox.NumberLabel.Text = string.Format("Cconnection.TreasureCount".Lang(p.Count));
            GameScene.Game.TreasureChestBox.Reset.Label.Text = string.Format("Cconnection.TreasureCost".Lang(p.Cost));
            for (int i = 0; i < p.Items.Count; i++)
            {
                GameScene.Game.TreasureChestBox.TreasureGrid[i].ItemGrid[0] = p.Items[i];

            }
        }
        public void Process(S.TreasureSel p)   //获得奖励事件
        {
            GameScene.Game.LuckDrawBox.GridImage[p.Slot].Visible = false;
            GameScene.Game.LuckDrawBox.TreasureGrid[p.Slot].ItemGrid[0] = p.Item;
            if (p.Count > 0)
            {
                string text = "确定选择奖励道具列表".Lang() + "。\n" +
                              "请选择".Lang() + "（" + "剩余".Lang() + "{0}" + "次".Lang() + "），\n" +
                              "（" + "本次需要".Lang() + "{1}" + "赞助币".Lang() + "）。";
                GameScene.Game.LuckDrawBox.ChoiceLabel.Text = string.Format(text, p.Count / 100, p.Cost);
            }
            else
            {
                GameScene.Game.LuckDrawBox.ChoiceLabel.Text = "抽奖机会全部用完".Lang();
            }
        }
        public void Process(S.ItemChanged p)  //道具项目已更改
        {
            DXItemCell[] grid;

            switch (p.Link.GridType)
            {
                case GridType.Inventory:
                    grid = GameScene.Game.InventoryBox.Grid.Grid;
                    break;
                case GridType.PatchGrid:                              //碎片包裹
                    grid = GameScene.Game.InventoryBox.PatchGrid.Grid;
                    break;
                case GridType.Equipment:
                    grid = GameScene.Game.CharacterBox.Grid;
                    break;
                case GridType.Storage:
                    grid = GameScene.Game.StorageBox.Grid.Grid;
                    break;
                case GridType.GuildStorage:
                    grid = GameScene.Game.GuildBox.StorageGrid.Grid;
                    break;
                case GridType.CompanionInventory:
                    grid = GameScene.Game.CompanionBox.InventoryGrid.Grid;
                    break;
                case GridType.CompanionEquipment:
                    grid = GameScene.Game.CompanionBox.EquipmentGrid;
                    break;
                case GridType.FishingEquipment:   //钓鱼装备格子
                    grid = GameScene.Game.FishingBox.FishingGrid;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            DXItemCell fromCell = grid[p.Link.Slot];

            fromCell.Locked = false;

            if (!p.Success) return;

            if (null != fromCell.Item && !fromCell.Item.Info.ShouldLinkInfo)
            {
                for (int i = 0; i < GameScene.Game.BeltBox.Links.Length; i++)
                {
                    ClientBeltLink link = GameScene.Game.BeltBox.Links[i];
                    if (link.LinkItemIndex != fromCell.Item.Index) continue;

                    link.LinkItemIndex = -1;

                    if (i < GameScene.Game.BeltBox.Grid.Grid.Length)
                        GameScene.Game.BeltBox.Grid.Grid[i].QuickItem = null; //set belt to null

                    if (!GameScene.Game.Observer)
                        CEnvir.Enqueue(new C.BeltLinkChanged { Slot = link.Slot, LinkIndex = link.LinkInfoIndex, LinkItemIndex = link.LinkItemIndex }); //Update server
                }
            }

            if (p.Link.Count == 0)
            {
                if (fromCell.Item.Info.ItemType == ItemType.Consumable)
                    GameScene.Game.ReceiveChat(string.Format("使用".Lang() + "{0}" + "物品".Lang() + "，" + "剩余".Lang() + "0" + "个".Lang(), fromCell.Item.Info.ItemName), MessageType.UseItem);
                fromCell.Item = null;
            }
            else
            {
                fromCell.Item.Count = p.Link.Count;
                if (fromCell.Item.Info.ItemType == ItemType.Consumable)
                    GameScene.Game.ReceiveChat(string.Format("使用".Lang() + "{0}" + "物品".Lang() + "，" + "剩余".Lang() + "{1}" + "个".Lang(), fromCell.Item.Info.ItemName, fromCell.Item.Count), MessageType.UseItem);
            }

            fromCell.RefreshItem();
        }
        public void Process(S.ItemsChanged p)        //道具项目已更改
        {
            foreach (CellLinkInfo cellLinkInfo in p.Links)
            {
                DXItemCell[] grid;

                switch (cellLinkInfo.GridType)
                {
                    case GridType.Inventory:
                        grid = GameScene.Game.InventoryBox.Grid.Grid;
                        break;
                    case GridType.PatchGrid:                        //碎片包裹
                        grid = GameScene.Game.InventoryBox.PatchGrid.Grid;
                        break;
                    case GridType.Equipment:
                        grid = GameScene.Game.CharacterBox.Grid;
                        break;
                    case GridType.Storage:
                        grid = GameScene.Game.StorageBox.Grid.Grid;
                        break;
                    case GridType.CompanionInventory:
                        grid = GameScene.Game.CompanionBox.InventoryGrid.Grid;
                        break;
                    case GridType.CompanionEquipment:
                        grid = GameScene.Game.CompanionBox.EquipmentGrid;
                        break;
                    case GridType.FishingEquipment: //钓鱼装备格子
                        grid = GameScene.Game.FishingBox.FishingGrid;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                DXItemCell fromCell = grid[cellLinkInfo.Slot];
                fromCell.Locked = false;

                if (!p.Success) continue;

                if (!fromCell.Item.Info.ShouldLinkInfo)
                {
                    for (int i = 0; i < GameScene.Game.BeltBox.Links.Length; i++)
                    {
                        ClientBeltLink link = GameScene.Game.BeltBox.Links[i];
                        if (link.LinkItemIndex != fromCell.Item.Index) continue;

                        link.LinkItemIndex = -1;

                        if (i < GameScene.Game.BeltBox.Grid.Grid.Length)
                            GameScene.Game.BeltBox.Grid.Grid[i].QuickItem = null; //set belt to null

                        if (!GameScene.Game.Observer)
                            CEnvir.Enqueue(new C.BeltLinkChanged { Slot = link.Slot, LinkIndex = link.LinkInfoIndex, LinkItemIndex = link.LinkItemIndex }); //Update server
                    }
                }

                if (cellLinkInfo.Count == fromCell.Item.Count)
                    fromCell.Item = null;
                else
                    fromCell.Item.Count -= cellLinkInfo.Count;

                fromCell.RefreshItem();
            }
        }
        public void Process(S.ItemRefineChange p)
        {
            DXItemCell[] grid;
            grid = GameScene.Game.CharacterBox.Grid;
            DXItemCell fromCell = grid[(int)EquipmentSlot.Weapon];
            fromCell.Item = p.Item;
            //if (p.Refine) fromCell.Item.Refine = false;
        }
        public void Process(S.ItemStatsChanged p)  //道具状态已更改
        {
            DXItemCell[] grid;

            switch (p.GridType)
            {
                case GridType.Inventory:
                    grid = GameScene.Game.InventoryBox.Grid.Grid;
                    break;
                case GridType.PatchGrid:                        //碎片包裹
                    grid = GameScene.Game.InventoryBox.PatchGrid.Grid;
                    break;
                case GridType.Equipment:
                    grid = GameScene.Game.CharacterBox.Grid;
                    break;
                case GridType.Storage:
                    grid = GameScene.Game.StorageBox.Grid.Grid;
                    break;
                case GridType.CompanionInventory:
                    grid = GameScene.Game.CompanionBox.InventoryGrid.Grid;
                    break;
                case GridType.CompanionEquipment:
                    grid = GameScene.Game.CompanionBox.EquipmentGrid;
                    break;
                case GridType.FishingEquipment: //钓鱼装备格子
                    grid = GameScene.Game.FishingBox.FishingGrid;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            DXItemCell fromCell = grid[p.Slot];

            fromCell.Item.AddedStats.Add(p.NewStats);

            if (p.NewStats.Count == 0)
            {
                GameScene.Game.ReceiveChat($"Cconnection.ItemStatNew".Lang(fromCell.Item.Info.Lang(p => p.ItemName)), MessageType.Hint);
                return;
            }

            foreach (KeyValuePair<Stat, int> pair in p.NewStats.Values)
            {
                if (pair.Key == Stat.WeaponElement)
                {
                    Type itemType = typeof(Element);
                    Element elem = (Element)fromCell.Item.AddedStats[Stat.WeaponElement];
                    MemberInfo[] infos = itemType.GetMember(elem.ToString());

                    DescriptionAttribute description = infos[0].GetCustomAttribute<DescriptionAttribute>();
                    GameScene.Game.ReceiveChat($"Cconnection.ItemStat".Lang(fromCell.Item.Info.Lang(p => p.ItemName), description?.Description ?? elem.ToString()), MessageType.Hint);
                    continue;
                }
                else if (pair.Key == Stat.IllusionDuration)
                {
                    fromCell.Item.IllusionExpireDateTime = CEnvir.Now.Add(TimeSpan.FromSeconds(pair.Value));
                    fromCell.Item.IllusionExpireTime = TimeSpan.FromSeconds(pair.Value);
                }

                string msg = p.NewStats.GetDisplay(pair.Key);

                if (string.IsNullOrEmpty(msg)) continue;

                GameScene.Game.ReceiveChat($"Cconnection.ItemNewStat".Lang(fromCell.Item.Info.Lang(p => p.ItemName), msg), MessageType.Hint);
            }

            if (p.FullItemStats != null)
            {
                fromCell.Item.FullItemStats = p.FullItemStats;
            }
            fromCell.RefreshItem();
        }
        public void Process(S.ItemStatsRefreshed p)  //道具状态刷新
        {
            DXItemCell[] grid;

            switch (p.GridType)
            {
                case GridType.Inventory:
                    grid = GameScene.Game.InventoryBox.Grid.Grid;
                    break;
                case GridType.PatchGrid:                        //碎片包裹
                    grid = GameScene.Game.InventoryBox.PatchGrid.Grid;
                    break;
                case GridType.Equipment:
                    grid = GameScene.Game.CharacterBox.Grid;
                    break;
                case GridType.Storage:
                    grid = GameScene.Game.StorageBox.Grid.Grid;
                    break;
                case GridType.CompanionInventory:
                    grid = GameScene.Game.CompanionBox.InventoryGrid.Grid;
                    break;
                case GridType.CompanionEquipment:
                    grid = GameScene.Game.CompanionBox.EquipmentGrid;
                    break;
                case GridType.FishingEquipment:  //钓鱼装备格子
                    grid = GameScene.Game.FishingBox.FishingGrid;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            DXItemCell fromCell = grid[p.Slot];

            fromCell.Item.AddedStats = p.NewStats;

            if (p.FullItemStats != null)
                fromCell.Item.FullItemStats = p.FullItemStats;

            fromCell.RefreshItem();
        }
        public void Process(S.ItemDurability p)   //道具 耐久性改变
        {
            DXItemCell[] grid;

            switch (p.GridType)
            {
                case GridType.Inventory:
                    grid = GameScene.Game.InventoryBox.Grid.Grid;
                    break;
                case GridType.Equipment:
                    grid = GameScene.Game.CharacterBox.Grid;
                    break;
                case GridType.Storage:
                    grid = GameScene.Game.StorageBox.Grid.Grid;
                    break;
                case GridType.CompanionInventory:
                    grid = GameScene.Game.CompanionBox.InventoryGrid.Grid;
                    break;
                case GridType.CompanionEquipment:
                    grid = GameScene.Game.CompanionBox.EquipmentGrid;
                    break;
                case GridType.FishingEquipment:  //钓鱼装备格子
                    grid = GameScene.Game.FishingBox.FishingGrid;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            DXItemCell fromCell = grid[p.Slot];

            fromCell.Item.CurrentDurability = p.CurrentDurability;

            if (p.CurrentDurability < 1000 && p.CurrentDurability > 0 && DurWarnDelay < CEnvir.Now && BigPatchConfig.ChkDurableWarning)
            {
                if (p.GridType == GridType.FishingEquipment) return;
                DurWarnDelay = CEnvir.Now.AddMinutes(1);
                GameScene.Game.ReceiveChat($"Cconnection.ItemCurrentDurability".Lang(fromCell.Item.Info.Lang(p => p.ItemName)), MessageType.DurabilityTips);
            }

            if (p.CurrentDurability == 0 && DurWarnDelay < CEnvir.Now)
            {
                DurWarnDelay = CEnvir.Now.AddMinutes(1);
                GameScene.Game.ReceiveChat($"Cconnection.ItemDurability".Lang(fromCell.Item.Info.Lang(p => p.ItemName)), MessageType.DurabilityTips);
            }


            fromCell.RefreshItem();
        }
        public void Process(S.StatsUpdate p)  //统计更新
        {
            if (MapObject.User == null) return;
            MapObject.User.HermitPoints = p.HermitPoints;
            MapObject.User.Stats = p.Stats;
            MapObject.User.HermitStats = p.HermitStats;
        }
        public void Process(S.ItemUseDelay p)  //道具使用延迟时间
        {
            GameScene.Game.UseItemTime = CEnvir.Now + p.Delay;
        }
        public void Process(S.ItemSplit p)  //道具分开分割
        {
            DXItemCell fromCell;

            switch (p.Grid)
            {
                case GridType.Inventory:
                    fromCell = GameScene.Game.InventoryBox.Grid.Grid[p.Slot];
                    break;
                case GridType.PatchGrid:                       //碎片包裹
                    fromCell = GameScene.Game.InventoryBox.PatchGrid.Grid[p.Slot];
                    break;
                case GridType.Storage:
                    fromCell = GameScene.Game.StorageBox.Grid.Grid[p.Slot];
                    break;
                case GridType.GuildStorage:
                    fromCell = GameScene.Game.GuildBox.StorageGrid.Grid[p.Slot];
                    break;
                case GridType.CompanionInventory:
                    fromCell = GameScene.Game.CompanionBox.InventoryGrid.Grid[p.Slot];
                    break;
                case GridType.CompanionEquipment:
                    fromCell = GameScene.Game.CompanionBox.EquipmentGrid[p.Slot];
                    break;
                case GridType.FishingEquipment:  //钓鱼装备格子
                    fromCell = GameScene.Game.FishingBox.FishingGrid[p.Slot];
                    break;
                default: return;
            }

            fromCell.Locked = false;

            if (!p.Success) return;

            DXItemCell toCell;
            switch (p.Grid)
            {
                case GridType.Inventory:
                    toCell = GameScene.Game.InventoryBox.Grid.Grid[p.NewSlot];
                    break;
                case GridType.PatchGrid:               //碎片包裹
                    toCell = GameScene.Game.InventoryBox.PatchGrid.Grid[p.NewSlot];
                    break;
                case GridType.Storage:
                    toCell = GameScene.Game.StorageBox.Grid.Grid[p.NewSlot];
                    break;
                case GridType.GuildStorage:
                    toCell = GameScene.Game.GuildBox.StorageGrid.Grid[p.NewSlot];
                    break;
                case GridType.CompanionInventory:
                    toCell = GameScene.Game.CompanionBox.InventoryGrid.Grid[p.NewSlot];
                    break;
                case GridType.CompanionEquipment:
                    toCell = GameScene.Game.CompanionBox.EquipmentGrid[p.NewSlot];
                    break;
                case GridType.FishingEquipment:  //钓鱼装备格子
                    toCell = GameScene.Game.FishingBox.FishingGrid[p.NewSlot];
                    break;
                default: return;
            }

            ClientUserItem item = new ClientUserItem(fromCell.Item, p.Count) { Slot = p.NewSlot };// { Count = p.Count, Info = cell.Item.Info, GridSlot = p.NewSlot, AddedStats = new Stats(), };

            toCell.Item = item;
            toCell.RefreshItem();

            if (p.Count == fromCell.Item.Count)
                fromCell.Item = null;
            else
                fromCell.Item.Count -= p.Count;

            fromCell.RefreshItem();
        }
        public void Process(S.ItemLock p)  //道具锁定
        {
            DXItemCell cell;

            switch (p.Grid)
            {
                case GridType.Inventory:
                    cell = GameScene.Game.InventoryBox.Grid.Grid[p.Slot];
                    break;
                case GridType.PatchGrid:           //碎片包裹
                    cell = GameScene.Game.InventoryBox.PatchGrid.Grid[p.Slot];
                    break;
                case GridType.Equipment:
                    cell = GameScene.Game.CharacterBox.Grid[p.Slot];
                    break;
                case GridType.Storage:
                    cell = GameScene.Game.StorageBox.Grid.Grid[p.Slot];
                    break;
                /*    case GridType.GuildStorage:                 //行会仓库的物品肯定不会锁定，所以注释
                      fromCell = GameScene.Game.GuildPanel.StorageControl.Grid[p.FromSlot];
                      break;*/
                case GridType.CompanionInventory:
                    cell = GameScene.Game.CompanionBox.InventoryGrid.Grid[p.Slot];
                    break;
                case GridType.CompanionEquipment:
                    cell = GameScene.Game.CompanionBox.EquipmentGrid[p.Slot];
                    break;
                case GridType.FishingEquipment: //钓鱼装备格子
                    cell = GameScene.Game.FishingBox.FishingGrid[p.Slot];
                    break;
                default: return;
            }

            if (cell.Item == null) return;

            if (p.Locked)
                cell.Item.Flags |= UserItemFlags.Locked;
            else
                cell.Item.Flags &= ~UserItemFlags.Locked;

            cell.RefreshItem();
        }
        public void Process(S.ItemExperience p)  //道具经验
        {
            DXItemCell cell;

            switch (p.Target.GridType)
            {
                case GridType.Inventory:
                    cell = GameScene.Game.InventoryBox.Grid.Grid[p.Target.Slot];
                    break;
                case GridType.PatchGrid:                //碎片包裹
                    cell = GameScene.Game.InventoryBox.PatchGrid.Grid[p.Target.Slot];
                    break;
                case GridType.Equipment:
                    cell = GameScene.Game.CharacterBox.Grid[p.Target.Slot];
                    break;
                case GridType.Storage:
                    cell = GameScene.Game.StorageBox.Grid.Grid[p.Target.Slot];
                    break;
                case GridType.CompanionInventory:
                    cell = GameScene.Game.CompanionBox.InventoryGrid.Grid[p.Target.Slot];
                    break;
                case GridType.CompanionEquipment:
                    cell = GameScene.Game.CompanionBox.EquipmentGrid[p.Target.Slot];
                    break;
                case GridType.FishingEquipment:  //钓鱼装备格子
                    cell = GameScene.Game.FishingBox.FishingGrid[p.Target.Slot];
                    break;
                default: return;
            }

            if (cell.Item == null) return;

            cell.Item.Experience = p.Experience;
            cell.Item.Level = p.Level;
            cell.Item.Flags = p.Flags;

            cell.RefreshItem();
        }

        public void Process(S.Chat p)   //聊天
        {
            if (GameScene.Game == null) return;

            if (p.NpcFace != 0)
            {
                GameScene.Game.ChatEventBox.Show(p);
                return;
            }

            GameScene.Game.ReceiveChat(p.Text, p.Type);

            if (p.Type != MessageType.Normal || p.ObjectID <= 0) return;

            //TODO Items?

            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
            {
                if (ob.ObjectID != p.ObjectID) continue;

                ob.Chat(p.Text);

                return;
            }
        }

        /// <summary>
        /// 处理聊天框服务端发送道具信息封包
        /// </summary>
        /// <param name="p"></param>
        public void Process(S.ChatItem p)
        {
            if (GameScene.Game.ChatItemList.Any(x => x.Index == p.Item.Index)) return;

            GameScene.Game.ChatItemList.Add(p.Item);
        }

        public void Process(S.NPCResponse p)  //NPC响应
        {
            GameScene.Game.NPCBox.Response(p);
        }

        public void Process(S.NPCBuyBack p)  //回购响应
        {
            GameScene.Game.NPCBox.ShowBackGoods(p);
        }
        public void Process(S.NPCBuyBackSeach p)  //回购响应
        {
            GameScene.Game.NPCBuyBackBox.UpdateSellGrid(p);
        }
        public void Process(S.DKey p)  //让客户端处理py的D键菜单
        {
            GameScene.Game.NPCDKeyBox.Visible = true;
            GameScene.Game.NPCDKeyBox.Response(p);
        }

        public void Process(S.NPCRepair p)   //NPC修理
        {
            foreach (CellLinkInfo cellLinkInfo in p.Links)
            {

                DXItemCell[] grid;

                switch (cellLinkInfo.GridType)
                {
                    case GridType.Inventory:
                        grid = GameScene.Game.InventoryBox.Grid.Grid;
                        break;
                    case GridType.Equipment:
                        grid = GameScene.Game.CharacterBox.Grid;
                        break;
                    case GridType.Storage:
                        grid = GameScene.Game.StorageBox.Grid.Grid;
                        break;
                    case GridType.GuildStorage:
                        if (GameScene.Game.Observer) continue;

                        grid = GameScene.Game.GuildBox.StorageGrid.Grid;
                        break;
                    case GridType.CompanionInventory:
                        grid = GameScene.Game.CompanionBox.InventoryGrid.Grid;
                        break;
                    case GridType.CompanionEquipment:
                        grid = GameScene.Game.CompanionBox.EquipmentGrid;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();  //参数超出范围异常
                }

                DXItemCell cell = grid[cellLinkInfo.Slot];

                cell.Locked = false;

                if (!p.Success) continue;

                cell.Link = null;

                if (p.Special)
                {
                    cell.Item.CurrentDurability = cell.Item.MaxDurability;
                    if (cell.Item.Info.ItemType != ItemType.Weapon && p.SpecialRepairDelay > TimeSpan.Zero)
                        cell.Item.NextSpecialRepair = CEnvir.Now.Add(p.SpecialRepairDelay);
                }
                else
                {
                    cell.Item.MaxDurability = Math.Max(0, cell.Item.MaxDurability - (cell.Item.MaxDurability - cell.Item.CurrentDurability) / Globals.DurabilityLossRate);
                    cell.Item.CurrentDurability = cell.Item.MaxDurability;
                }

                cell.RefreshItem();
            }
        }

        public void Process(S.NPCSpecialRepair p)   //NPC特修
        {
            foreach (CellLinkInfo cellLinkInfo in p.Links)
            {

                DXItemCell[] grid;

                switch (cellLinkInfo.GridType)
                {
                    case GridType.Inventory:
                        grid = GameScene.Game.InventoryBox.Grid.Grid;
                        break;
                    case GridType.Equipment:
                        grid = GameScene.Game.CharacterBox.Grid;
                        break;
                    case GridType.Storage:
                        grid = GameScene.Game.StorageBox.Grid.Grid;
                        break;
                    case GridType.GuildStorage:
                        if (GameScene.Game.Observer) continue;

                        grid = GameScene.Game.GuildBox.StorageGrid.Grid;
                        break;
                    case GridType.CompanionInventory:
                        grid = GameScene.Game.CompanionBox.InventoryGrid.Grid;
                        break;
                    case GridType.CompanionEquipment:
                        grid = GameScene.Game.CompanionBox.EquipmentGrid;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();  //参数超出范围异常
                }

                DXItemCell cell = grid[cellLinkInfo.Slot];

                cell.Locked = false;

                if (!p.Success) continue;

                cell.Link = null;

                if (p.Special)
                {
                    cell.Item.CurrentDurability = cell.Item.MaxDurability;
                    if (cell.Item.Info.ItemType != ItemType.Weapon && p.SpecialRepairDelay > TimeSpan.Zero)
                        cell.Item.NextSpecialRepair = CEnvir.Now.Add(p.SpecialRepairDelay);
                }
                else
                {
                    cell.Item.MaxDurability = Math.Max(0, cell.Item.MaxDurability - (cell.Item.MaxDurability - cell.Item.CurrentDurability) / Globals.DurabilityLossRate);
                    cell.Item.CurrentDurability = cell.Item.MaxDurability;
                }

                cell.RefreshItem();
            }
        }

        public void Process(S.NPCRefine p)  //NPC精炼
        {
            foreach (CellLinkInfo cellLinkInfo in p.Ores)
            {
                DXItemCell[] grid;

                switch (cellLinkInfo.GridType)
                {
                    case GridType.Inventory:
                        grid = GameScene.Game.InventoryBox.Grid.Grid;
                        break;
                    case GridType.Equipment:
                        grid = GameScene.Game.CharacterBox.Grid;
                        break;
                    case GridType.Storage:
                        grid = GameScene.Game.StorageBox.Grid.Grid;
                        break;
                    case GridType.CompanionInventory:
                        grid = GameScene.Game.CompanionBox.InventoryGrid.Grid;
                        break;
                    case GridType.CompanionEquipment:
                        grid = GameScene.Game.CompanionBox.EquipmentGrid;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(); //参数超出范围异常
                }

                DXItemCell fromCell = grid[cellLinkInfo.Slot];
                fromCell.Locked = false;

                if (!p.Success) continue;

                if (!fromCell.Item.Info.ShouldLinkInfo)
                {
                    for (int i = 0; i < GameScene.Game.BeltBox.Links.Length; i++)
                    {
                        ClientBeltLink link = GameScene.Game.BeltBox.Links[i];
                        if (link.LinkItemIndex != fromCell.Item.Index) continue;

                        link.LinkItemIndex = -1;

                        if (i < GameScene.Game.BeltBox.Grid.Grid.Length)
                            GameScene.Game.BeltBox.Grid.Grid[i].QuickItem = null; //set belt to null

                        if (!GameScene.Game.Observer)
                            CEnvir.Enqueue(new C.BeltLinkChanged { Slot = link.Slot, LinkIndex = link.LinkInfoIndex, LinkItemIndex = link.LinkItemIndex }); //Update server
                    }
                }

                if (cellLinkInfo.Count == fromCell.Item.Count)
                    fromCell.Item = null;
                else
                    fromCell.Item.Count -= cellLinkInfo.Count;

                fromCell.RefreshItem();
            }

            foreach (CellLinkInfo cellLinkInfo in p.Items)   //单元格链接信息  道具
            {
                DXItemCell[] grid;

                switch (cellLinkInfo.GridType)
                {
                    case GridType.Inventory:
                        grid = GameScene.Game.InventoryBox.Grid.Grid;
                        break;
                    case GridType.PatchGrid:               //碎片包裹
                        grid = GameScene.Game.InventoryBox.PatchGrid.Grid;
                        break;
                    case GridType.Equipment:
                        grid = GameScene.Game.CharacterBox.Grid;
                        break;
                    case GridType.Storage:
                        grid = GameScene.Game.StorageBox.Grid.Grid;
                        break;
                    case GridType.CompanionInventory:
                        grid = GameScene.Game.CompanionBox.InventoryGrid.Grid;
                        break;
                    case GridType.CompanionEquipment:
                        grid = GameScene.Game.CompanionBox.EquipmentGrid;
                        break;
                    case GridType.FishingEquipment:  //钓鱼装备格子
                        grid = GameScene.Game.FishingBox.FishingGrid;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                DXItemCell fromCell = grid[cellLinkInfo.Slot];
                fromCell.Locked = false;

                if (!p.Success) continue;

                if (!fromCell.Item.Info.ShouldLinkInfo)
                {
                    for (int i = 0; i < GameScene.Game.BeltBox.Links.Length; i++)
                    {
                        ClientBeltLink link = GameScene.Game.BeltBox.Links[i];
                        if (link.LinkItemIndex != fromCell.Item.Index) continue;

                        link.LinkItemIndex = -1;

                        if (i < GameScene.Game.BeltBox.Grid.Grid.Length)
                            GameScene.Game.BeltBox.Grid.Grid[i].QuickItem = null; //set belt to null

                        if (!GameScene.Game.Observer)
                            CEnvir.Enqueue(new C.BeltLinkChanged { Slot = link.Slot, LinkIndex = link.LinkInfoIndex, LinkItemIndex = link.LinkItemIndex }); //Update server
                    }
                }

                if (cellLinkInfo.Count == fromCell.Item.Count)
                    fromCell.Item = null;
                else
                    fromCell.Item.Count -= cellLinkInfo.Count;

                fromCell.RefreshItem();
            }

            foreach (CellLinkInfo cellLinkInfo in p.Specials) //单元格链接信息 特价商品
            {
                DXItemCell[] grid;

                switch (cellLinkInfo.GridType)
                {
                    case GridType.Inventory:
                        grid = GameScene.Game.InventoryBox.Grid.Grid;
                        break;
                    case GridType.PatchGrid:               //碎片包裹
                        grid = GameScene.Game.InventoryBox.PatchGrid.Grid;
                        break;
                    case GridType.Equipment:
                        grid = GameScene.Game.CharacterBox.Grid;
                        break;
                    case GridType.Storage:
                        grid = GameScene.Game.StorageBox.Grid.Grid;
                        break;
                    case GridType.CompanionInventory:
                        grid = GameScene.Game.CompanionBox.InventoryGrid.Grid;
                        break;
                    case GridType.CompanionEquipment:
                        grid = GameScene.Game.CompanionBox.EquipmentGrid;
                        break;
                    case GridType.FishingEquipment:  //钓鱼装备格子
                        grid = GameScene.Game.FishingBox.FishingGrid;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                DXItemCell fromCell = grid[cellLinkInfo.Slot];
                fromCell.Locked = false;

                if (!p.Success) continue;

                if (!fromCell.Item.Info.ShouldLinkInfo)
                {
                    for (int i = 0; i < GameScene.Game.BeltBox.Links.Length; i++)
                    {
                        ClientBeltLink link = GameScene.Game.BeltBox.Links[i];
                        if (link.LinkItemIndex != fromCell.Item.Index) continue;

                        link.LinkItemIndex = -1;

                        if (i < GameScene.Game.BeltBox.Grid.Grid.Length)
                            GameScene.Game.BeltBox.Grid.Grid[i].QuickItem = null; //set belt to null

                        if (!GameScene.Game.Observer)
                            CEnvir.Enqueue(new C.BeltLinkChanged { Slot = link.Slot, LinkIndex = link.LinkInfoIndex, LinkItemIndex = link.LinkItemIndex }); //Update server
                    }
                }

                if (cellLinkInfo.Count == fromCell.Item.Count)
                    fromCell.Item = null;
                else
                    fromCell.Item.Count -= cellLinkInfo.Count;

                fromCell.RefreshItem();
            }

            if (p.Success)
            {
                DXItemCell fromCell = GameScene.Game.CharacterBox.Grid[(int)EquipmentSlot.Weapon];
                fromCell.Item = null;
                fromCell.RefreshItem();

                //GameScene.Game.ReceiveChat($"Cconnection.WeaponRefine".Lang(Globals.RefineTimes[p.RefineQuality].Lang(false)), MessageType.System);
            }
        }
        public void Process(S.NPCMasterRefine p)  //NPC大师精炼
        {
            foreach (CellLinkInfo cellLinkInfo in p.Fragment1s)
            {
                DXItemCell[] grid;

                switch (cellLinkInfo.GridType)
                {
                    case GridType.Inventory:
                        grid = GameScene.Game.InventoryBox.Grid.Grid;
                        break;
                    case GridType.Equipment:
                        grid = GameScene.Game.CharacterBox.Grid;
                        break;
                    case GridType.Storage:
                        grid = GameScene.Game.StorageBox.Grid.Grid;
                        break;
                    case GridType.CompanionInventory:
                        grid = GameScene.Game.CompanionBox.InventoryGrid.Grid;
                        break;
                    case GridType.CompanionEquipment:
                        grid = GameScene.Game.CompanionBox.EquipmentGrid;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                DXItemCell fromCell = grid[cellLinkInfo.Slot];
                fromCell.Locked = false;

                if (!p.Success) continue;


                if (!fromCell.Item.Info.ShouldLinkInfo)
                {
                    for (int i = 0; i < GameScene.Game.BeltBox.Links.Length; i++)
                    {
                        ClientBeltLink link = GameScene.Game.BeltBox.Links[i];
                        if (link.LinkItemIndex != fromCell.Item.Index) continue;

                        link.LinkItemIndex = -1;

                        if (i < GameScene.Game.BeltBox.Grid.Grid.Length)
                            GameScene.Game.BeltBox.Grid.Grid[i].QuickItem = null; //set belt to null

                        if (!GameScene.Game.Observer)
                            CEnvir.Enqueue(new C.BeltLinkChanged { Slot = link.Slot, LinkIndex = link.LinkInfoIndex, LinkItemIndex = link.LinkItemIndex }); //Update server
                    }
                }

                if (cellLinkInfo.Count == fromCell.Item.Count)
                    fromCell.Item = null;
                else
                    fromCell.Item.Count -= cellLinkInfo.Count;

                fromCell.RefreshItem();
            }

            foreach (CellLinkInfo cellLinkInfo in p.Fragment2s) //单元格链接信息 大师精炼使用的碎片2
            {
                DXItemCell[] grid;

                switch (cellLinkInfo.GridType)
                {
                    case GridType.Inventory:
                        grid = GameScene.Game.InventoryBox.Grid.Grid;
                        break;
                    case GridType.Equipment:
                        grid = GameScene.Game.CharacterBox.Grid;
                        break;
                    case GridType.Storage:
                        grid = GameScene.Game.StorageBox.Grid.Grid;
                        break;
                    case GridType.CompanionInventory:
                        grid = GameScene.Game.CompanionBox.InventoryGrid.Grid;
                        break;
                    case GridType.CompanionEquipment:
                        grid = GameScene.Game.CompanionBox.EquipmentGrid;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                DXItemCell fromCell = grid[cellLinkInfo.Slot];
                fromCell.Locked = false;

                if (!p.Success) continue;

                if (!fromCell.Item.Info.ShouldLinkInfo)
                {
                    for (int i = 0; i < GameScene.Game.BeltBox.Links.Length; i++)
                    {
                        ClientBeltLink link = GameScene.Game.BeltBox.Links[i];
                        if (link.LinkItemIndex != fromCell.Item.Index) continue;

                        link.LinkItemIndex = -1;

                        if (i < GameScene.Game.BeltBox.Grid.Grid.Length)
                            GameScene.Game.BeltBox.Grid.Grid[i].QuickItem = null; //set belt to null

                        if (!GameScene.Game.Observer)
                            CEnvir.Enqueue(new C.BeltLinkChanged { Slot = link.Slot, LinkIndex = link.LinkInfoIndex, LinkItemIndex = link.LinkItemIndex }); //Update server
                    }
                }

                if (cellLinkInfo.Count == fromCell.Item.Count)
                    fromCell.Item = null;
                else
                    fromCell.Item.Count -= cellLinkInfo.Count;

                fromCell.RefreshItem();
            }

            foreach (CellLinkInfo cellLinkInfo in p.Fragment3s) //单元格链接信息 大师精炼使用的碎片3
            {
                DXItemCell[] grid;

                switch (cellLinkInfo.GridType)
                {
                    case GridType.Inventory:
                        grid = GameScene.Game.InventoryBox.Grid.Grid;
                        break;
                    case GridType.Equipment:
                        grid = GameScene.Game.CharacterBox.Grid;
                        break;
                    case GridType.Storage:
                        grid = GameScene.Game.StorageBox.Grid.Grid;
                        break;
                    case GridType.CompanionInventory:
                        grid = GameScene.Game.CompanionBox.InventoryGrid.Grid;
                        break;
                    case GridType.CompanionEquipment:
                        grid = GameScene.Game.CompanionBox.EquipmentGrid;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                DXItemCell fromCell = grid[cellLinkInfo.Slot];
                fromCell.Locked = false;

                if (!p.Success) continue;

                if (!fromCell.Item.Info.ShouldLinkInfo)
                {
                    for (int i = 0; i < GameScene.Game.BeltBox.Links.Length; i++)
                    {
                        ClientBeltLink link = GameScene.Game.BeltBox.Links[i];
                        if (link.LinkItemIndex != fromCell.Item.Index) continue;

                        link.LinkItemIndex = -1;

                        if (i < GameScene.Game.BeltBox.Grid.Grid.Length)
                            GameScene.Game.BeltBox.Grid.Grid[i].QuickItem = null; //set belt to null

                        if (!GameScene.Game.Observer)
                            CEnvir.Enqueue(new C.BeltLinkChanged { Slot = link.Slot, LinkIndex = link.LinkInfoIndex, LinkItemIndex = link.LinkItemIndex }); //Update server
                    }
                }

                if (cellLinkInfo.Count == fromCell.Item.Count)
                    fromCell.Item = null;
                else
                    fromCell.Item.Count -= cellLinkInfo.Count;

                fromCell.RefreshItem();
            }

            foreach (CellLinkInfo cellLinkInfo in p.Stones)  //石料
            {
                DXItemCell[] grid;

                switch (cellLinkInfo.GridType)
                {
                    case GridType.Inventory:
                        grid = GameScene.Game.InventoryBox.Grid.Grid;
                        break;
                    case GridType.Equipment:
                        grid = GameScene.Game.CharacterBox.Grid;
                        break;
                    case GridType.Storage:
                        grid = GameScene.Game.StorageBox.Grid.Grid;
                        break;
                    case GridType.CompanionInventory:
                        grid = GameScene.Game.CompanionBox.InventoryGrid.Grid;
                        break;
                    case GridType.CompanionEquipment:
                        grid = GameScene.Game.CompanionBox.EquipmentGrid;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                DXItemCell fromCell = grid[cellLinkInfo.Slot];
                fromCell.Locked = false;

                if (!p.Success) continue;

                if (!fromCell.Item.Info.ShouldLinkInfo)
                {
                    for (int i = 0; i < GameScene.Game.BeltBox.Links.Length; i++)
                    {
                        ClientBeltLink link = GameScene.Game.BeltBox.Links[i];
                        if (link.LinkItemIndex != fromCell.Item.Index) continue;

                        link.LinkItemIndex = -1;

                        if (i < GameScene.Game.BeltBox.Grid.Grid.Length)
                            GameScene.Game.BeltBox.Grid.Grid[i].QuickItem = null; //set belt to null

                        if (!GameScene.Game.Observer)
                            CEnvir.Enqueue(new C.BeltLinkChanged { Slot = link.Slot, LinkIndex = link.LinkInfoIndex, LinkItemIndex = link.LinkItemIndex }); //Update server
                    }
                }

                if (cellLinkInfo.Count == fromCell.Item.Count)
                    fromCell.Item = null;
                else
                    fromCell.Item.Count -= cellLinkInfo.Count;

                fromCell.RefreshItem();
            }

            foreach (CellLinkInfo cellLinkInfo in p.Specials)  //寄售商品
            {
                DXItemCell[] grid;

                switch (cellLinkInfo.GridType)
                {
                    case GridType.Inventory:
                        grid = GameScene.Game.InventoryBox.Grid.Grid;
                        break;
                    case GridType.PatchGrid:               //碎片包裹
                        grid = GameScene.Game.InventoryBox.PatchGrid.Grid;
                        break;
                    case GridType.Equipment:
                        grid = GameScene.Game.CharacterBox.Grid;
                        break;
                    case GridType.Storage:
                        grid = GameScene.Game.StorageBox.Grid.Grid;
                        break;
                    case GridType.CompanionInventory:
                        grid = GameScene.Game.CompanionBox.InventoryGrid.Grid;
                        break;
                    case GridType.CompanionEquipment:
                        grid = GameScene.Game.CompanionBox.EquipmentGrid;
                        break;
                    case GridType.FishingEquipment:  //钓鱼装备格子
                        grid = GameScene.Game.FishingBox.FishingGrid;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                DXItemCell fromCell = grid[cellLinkInfo.Slot];
                fromCell.Locked = false;

                if (!p.Success) continue;

                if (!fromCell.Item.Info.ShouldLinkInfo)
                {
                    for (int i = 0; i < GameScene.Game.BeltBox.Links.Length; i++)
                    {
                        ClientBeltLink link = GameScene.Game.BeltBox.Links[i];
                        if (link.LinkItemIndex != fromCell.Item.Index) continue;

                        link.LinkItemIndex = -1;

                        if (i < GameScene.Game.BeltBox.Grid.Grid.Length)
                            GameScene.Game.BeltBox.Grid.Grid[i].QuickItem = null; //set belt to null

                        if (!GameScene.Game.Observer)
                            CEnvir.Enqueue(new C.BeltLinkChanged { Slot = link.Slot, LinkIndex = link.LinkInfoIndex, LinkItemIndex = link.LinkItemIndex }); //Update server
                    }
                }

                if (cellLinkInfo.Count == fromCell.Item.Count)
                    fromCell.Item = null;
                else
                    fromCell.Item.Count -= cellLinkInfo.Count;

                fromCell.RefreshItem();
            }
        }
        public void Process(S.RefineList p)  //精炼列表
        {
            if (p.IsNewWeaponUpgrade)
                GameScene.Game.NPCWeaponUpgradeBox.RefineInfo = p.List.Count > 0 ? p.List[0] : null;
            else
                GameScene.Game.NPCRefineRetrieveBox.Refines.AddRange(p.List);
        }
        public void Process(S.NPCRefineRetrieve p) //NPC优化检索
        {
            if (p.IsNewWeaponUpgrade)
            {
                GameScene.Game.NPCWeaponUpgradeBox.RefineInfo = null;
                return;
            }

            foreach (ClientRefineInfo info in GameScene.Game.NPCRefineRetrieveBox.Refines)
            {
                if (info.Index != p.Index) continue;

                GameScene.Game.NPCRefineRetrieveBox.Refines.Remove(info);
                break;
            }

            GameScene.Game.NPCRefineRetrieveBox.RefreshList();
        }
        public void Process(S.NPCClose p)  //NPC关闭
        {
            GameScene.Game.NPCBox.Visible = false;
        }
        public void Process(S.NPCAccessoryLevelUp p)  //NPC附件等级升级
        {
            if (p.Target != null)
                p.Links.Add(p.Target);

            foreach (CellLinkInfo cellLinkInfo in p.Links)
            {
                DXItemCell[] grid;

                switch (cellLinkInfo.GridType)
                {
                    case GridType.Inventory:
                        grid = GameScene.Game.InventoryBox.Grid.Grid;
                        break;
                    case GridType.Equipment:
                        grid = GameScene.Game.CharacterBox.Grid;
                        break;
                    case GridType.Storage:
                        grid = GameScene.Game.StorageBox.Grid.Grid;
                        break;
                    case GridType.CompanionInventory:
                        grid = GameScene.Game.CompanionBox.InventoryGrid.Grid;
                        break;
                    case GridType.CompanionEquipment:
                        grid = GameScene.Game.CompanionBox.EquipmentGrid;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                DXItemCell fromCell = grid[cellLinkInfo.Slot];
                fromCell.Locked = false;
            }
        }

        public void Process(S.GroupSwitch p)   //组队开关
        {
            GameScene.Game.GroupBox.AllowGroup = p.Allow;
        }
        public void Process(S.GroupMember p)  //组队列表加入
        {
            GameScene.Game.GroupBox.Members.Add(new ClientPlayerInfo { ObjectID = p.ObjectID, Name = p.Name });

            GameScene.Game.GroupMemberBox.PopulateMembers(GameScene.Game.GroupBox.Members);   //填充成员到组队界面

            GameScene.Game.ReceiveChat($"Cconnection.GroupMember".Lang(p.Name), MessageType.Group);   //进组提示

            GameScene.Game.GroupBox.UpdateMembers();  //更新成员

            ClientObjectData data;
            if (!GameScene.Game.DataDictionary.TryGetValue(p.ObjectID, out data)) return;

            GameScene.Game.OnUpdateObjectData(data);
        }
        public void Process(S.GroupRemove p)  //组队离开
        {
            ClientPlayerInfo info = GameScene.Game.GroupBox.Members.FirstOrDefault(x => x.ObjectID == p.ObjectID);
            if (info == null) return;

            GameScene.Game.ReceiveChat($"Cconnection.GroupRemove".Lang(info.Name), MessageType.Group);

            HashSet<uint> checks = new HashSet<uint>();

            if (p.ObjectID == MapObject.User.ObjectID)
            {
                foreach (ClientPlayerInfo member in GameScene.Game.GroupBox.Members)
                    checks.Add(member.ObjectID);

                GameScene.Game.GroupBox.Members.Clear();
            }
            else
            {
                checks.Add(p.ObjectID);
                GameScene.Game.GroupBox.Members.Remove(info);
            }

            GameScene.Game.GroupBox.UpdateMembers();

            GameScene.Game.GroupMemberBox.PopulateMembers(GameScene.Game.GroupBox.Members);   //填充成员到组队界面

            foreach (uint objectID in checks)
            {
                ClientObjectData data;
                if (!GameScene.Game.DataDictionary.TryGetValue(objectID, out data)) return;

                GameScene.Game.OnUpdateObjectData(data);
            }
        }
        public void Process(S.GroupInvite p)  //邀请组队
        {
            DXMessageBox messageBox = new DXMessageBox($"Cconnection.GroupInvite".Lang(p.Name), "组队邀请".Lang(), DXMessageBoxButtons.YesNo);

            messageBox.YesButton.MouseClick += (o, e) => CEnvir.Enqueue(new C.GroupResponse { Accept = true });
            messageBox.NoButton.MouseClick += (o, e) => CEnvir.Enqueue(new C.GroupResponse { Accept = false });
            messageBox.CloseButton.MouseClick += (o, e) => CEnvir.Enqueue(new C.GroupResponse { Accept = false });
            messageBox.Modal = false;
            messageBox.CloseButton.Visible = false;
        }

        public void Process(S.BuffAdd p)  //增加BUFF
        {
            MapObject.User.Buffs.Add(p.Buff);
            MapObject.User.VisibleBuffs.Add(p.Buff.Type);

            GameScene.Game.BuffBox.BuffsChanged();
        }
        public void Process(S.BuffRemove p)  //BUFF移除
        {
            if (MapObject.User == null) return;
            foreach (ClientBuffInfo buff in MapObject.User.Buffs)
            {
                if (buff.Index != p.Index) continue;

                MapObject.User.Buffs.Remove(buff);
                MapObject.User.VisibleBuffs.Remove(buff.Type);
                break;
            }

            GameScene.Game.BuffBox.BuffsChanged();
        }
        public void Process(S.BuffChanged p)  //BUFF改变
        {
            ClientBuffInfo changedBuff = MapObject.User.Buffs.FirstOrDefault(x => x.Index == p.Index);
            if (changedBuff == null) return;

            changedBuff.Stats = p.Stats;
            if (p.RemainingTime > TimeSpan.Zero)
                changedBuff.RemainingTime = p.RemainingTime;
            GameScene.Game.BuffBox.BuffsChanged();
        }
        public void Process(S.BuffTime p)  //BUFF时间
        {
            var buff = MapObject.User.Buffs.FirstOrDefault(x => x.Index == p.Index);
            if (buff == null) return;
                
            buff.RemainingTime = p.Time;
            GameScene.Game.BuffBox.BuffsChanged();
        }
        public void Process(S.BuffPaused p)  //BUFF暂停
        {
            var buff = MapObject.User.Buffs.FirstOrDefault(x => x.Index == p.Index);
            if (buff == null) return;
            
            buff.Pause = p.Paused;
            GameScene.Game.BuffBox.BuffsChanged();
        }

        public void Process(S.SafeZoneChanged P)  //安全区改变
        {
            MapObject.User.InSafeZone = P.InSafeZone;

            //GameScene.Game.SafeZoneChanged(P.InSafeZone);
        }
        public void Process(S.CombatTime p)   //战斗时间
        {
            GameScene.Game.User.CombatTime = CEnvir.Now;
        }

        public void Process(S.Inspect p)  //查看其他玩家
        {
            GameScene.Game.InspectBox.NewInformation(p);
        }
        public void Process(S.Rankings p)   //排行榜
        {
            (DXControl.ActiveScene as LoginScene)?.RankingBox.Update(p);
            GameScene.Game?.RankingBox.Update(p);
        }
        public void Process(S.StartObserver p)  //启动观察者
        {
            CEnvir.FillStorage(p.Items, true);
            //CEnvir.FillPatchGrid(p.Items, true);

            int index = 0;

            if (GameScene.Game != null)
                index = GameScene.Game.RankingBox.StartIndex;

            DXControl.ActiveScene.Dispose();

            DXSoundManager.StopAllSounds();

            CEnvir.ClientControl = p.ClientControl;
            GameScene scene = new GameScene(Config.GameSize);
            scene.Loaded = true;

            scene.IsVisible = true;

            DXControl.ActiveScene = scene;
            GameScene.Game.Observer = true;

            scene.MapControl.MapInfo = Globals.MapInfoList.Binding.FirstOrDefault(x => x.Index == p.StartInformation.MapIndex);
            scene.User = new UserObject(p.StartInformation);

            GameScene.Game.QuestLog = p.StartInformation.Quests;
            GameScene.Game.QuestBox.PopulateQuests();
            GameScene.Game.QuestTrackerBox.PopulateQuests();
            GameScene.Game.AchievementLog = p.StartInformation.Achievements;  //成就日志

            GameScene.Game.NPCAdoptCompanionBox.AvailableCompanions = p.StartInformation.AvailableCompanions;
            GameScene.Game.NPCAdoptCompanionBox.RefreshUnlockButton();

            GameScene.Game.NPCCompanionStorageBox.Companions = p.StartInformation.Companions;
            GameScene.Game.NPCCompanionStorageBox.UpdateScrollBar();

            GameScene.Game.Companion = GameScene.Game.NPCCompanionStorageBox.Companions.FirstOrDefault(x => x.Index == p.StartInformation.Companion);

            GameScene.Game.PatchGridSize = p.StartInformation.PatchGridSize;  //碎片包裹

            GameScene.Game.StorageSize = p.StartInformation.StorageSize;  //仓库       

            GameScene.Game.BuffBox.BuffsChanged();

            GameScene.Game.RankingBox.StartIndex = index;
        }
        public void Process(S.ObservableSwitch p)  //观察者开关
        {
            GameScene.Game.RankingBox.Observable = p.Allow;
        }

        public void Process(S.MarketPlaceHistory p)  //商城历史记录
        {
            switch (p.Display)
            {
                case 1:
                    //GameScene.Game.MarketPlaceBox.SearchNumberSoldBox.TextBox.Text = p.SaleCount > 0 ? p.SaleCount.ToString("#,##0") : "没有记录".Lang();
                    //GameScene.Game.MarketPlaceBox.SearchAveragePriceBox.TextBox.Text = p.AveragePrice > 0 ? p.AveragePrice.ToString("#,##0") : "没有记录".Lang();
                    //GameScene.Game.MarketPlaceBox.SearchLastPriceBox.TextBox.Text = p.LastPrice > 0 ? p.LastPrice.ToString("#,##0") : "没有记录".Lang();
                    //GameScene.Game.MarketPlaceBox.SearchGameGoldAveragePriceBox.TextBox.Text = p.GameGoldAveragePrice > 0 ? p.GameGoldAveragePrice.ToString("#,##0") : "没有记录".Lang();
                    //GameScene.Game.MarketPlaceBox.SearchGameGoldLastPriceBox.TextBox.Text = p.GameGoldLastPrice > 0 ? p.GameGoldLastPrice.ToString("#,##0") : "没有记录".Lang();
                    break;
                case 2:
                    //GameScene.Game.MarketPlaceBox.NumberSoldBox.TextBox.Text = p.SaleCount > 0 ? p.SaleCount.ToString("#,##0") : "没有记录".Lang();
                    //GameScene.Game.MarketPlaceBox.AveragePriceBox.TextBox.Text = p.AveragePrice > 0 ? p.AveragePrice.ToString("#,##0") : "没有记录".Lang();
                    //GameScene.Game.MarketPlaceBox.LastPriceBox.TextBox.Text = p.LastPrice > 0 ? p.LastPrice.ToString("#,##0") : "没有记录".Lang();
                    //GameScene.Game.MarketPlaceBox.AveragePriceBox1.TextBox.Text = p.GameGoldAveragePrice > 0 ? p.GameGoldAveragePrice.ToString("#,##0") : "没有记录".Lang();
                    //GameScene.Game.MarketPlaceBox.LastPriceBox1.TextBox.Text = p.GameGoldLastPrice > 0 ? p.GameGoldLastPrice.ToString("#,##0") : "没有记录".Lang();
                    break;
            }
        }
        public void Process(S.MarketPlaceConsign p)  //商城寄售
        {
            GameScene.Game.MyMarketBox.ConsignItems.AddRange(p.Consignments);
            GameScene.Game.MyMarketBox.TotalPage = (GameScene.Game.MyMarketBox.ConsignItems.Count() - 1) / 10 + 1;
            GameScene.Game.MyMarketBox.CurrentPage = 1;
            GameScene.Game.MyMarketBox.RefreshConsignList();
            GameScene.Game.MarketConsignBox.ConsignItems.AddRange(p.Consignments);
            GameScene.Game.MarketSearchBox.Search();
        }
        public void Process(S.MarketPlaceSearch p)   //商城搜索
        {
            GameScene.Game.MarketSearchBox.SearchResults = new ClientMarketPlaceInfo[p.Count];

            for (int i = 0; i < p.Results.Count; i++)
                GameScene.Game.MarketSearchBox.SearchResults[i] = p.Results[i];
            GameScene.Game.MarketSearchBox.TotalPage = (GameScene.Game.MarketSearchBox.SearchResults.Count() - 1) / 13 + 1;
            GameScene.Game.MarketSearchBox.CurrentPage = 1;
            GameScene.Game.MarketSearchBox.RefreshList();
        }
        public void Process(S.MarketPlaceSearchCount p)  //商城搜索计数
        {
            Array.Resize(ref GameScene.Game.MarketSearchBox.SearchResults, p.Count);

            GameScene.Game.MarketSearchBox.RefreshList(GameScene.Game.MarketSearchBox.CurrentPage);
        }
        public void Process(S.MarketPlaceSearchIndex p)  //商城搜索索引
        {
            if (GameScene.Game.MarketSearchBox.SearchResults == null) return;

            GameScene.Game.MarketSearchBox.SearchResults[p.Index] = p.Result;

            GameScene.Game.MarketSearchBox.RefreshList(GameScene.Game.MarketSearchBox.CurrentPage);
        }
        public void Process(S.MarketPlaceConsignChanged p)  //商城发货改变
        {
            ClientMarketPlaceInfo info = GameScene.Game.MyMarketBox.ConsignItems.FirstOrDefault(x => x.Index == p.Index);

            if (info == null) return;

            if (p.Count > 0)
                info.Item.Count = p.Count;
            else
                GameScene.Game.MyMarketBox.ConsignItems.Remove(info);
            GameScene.Game.MyMarketBox.RefreshConsignList();
        }
        public void Process(S.MarketPlaceBuy p)  //商城买
        {
            GameScene.Game.MarketSearchBox.BuyButton.Enabled = true;

            if (!p.Success) return;

            ClientMarketPlaceInfo info = GameScene.Game.MarketSearchBox.SearchResults.FirstOrDefault(x => x != null && x.Index == p.Index);

            if (info == null) return;

            if (p.Count > 0)
                info.Item.Count = p.Count;
            else
                info.Item = null;

            GameScene.Game.MarketSearchBox.RefreshList(GameScene.Game.MarketSearchBox.CurrentPage);
        }
        public void Process(S.MarketPlaceStoreBuy p)  //商城购买
        {
            GameScene.Game.MarketPlaceBox.StoreBuyButton.Enabled = true;
        }

        public void Process(S.MailList p)   //邮件列表
        {
            GameScene.Game.CommunicationBox.MailList.AddRange(p.Mail);
            GameScene.Game.CommunicationBox.UpdateIcon();
        }
        public void Process(S.MailNew p)   //新邮件
        {
            GameScene.Game.CommunicationBox.MailList.Insert(0, p.Mail);
            GameScene.Game.CommunicationBox.RefreshList();
            GameScene.Game.CommunicationBox.UpdateIcon();
            GameScene.Game.ReceiveChat($"Cconnection.MailNew".Lang(p.Mail.Sender), MessageType.System);
        }
        public void Process(S.MailDelete p)  //邮件删除
        {
            ClientMailInfo mail = GameScene.Game.CommunicationBox.MailList.FirstOrDefault(x => x.Index == p.Index);

            if (mail == null) return;

            GameScene.Game.CommunicationBox.MailList.Remove(mail);
            GameScene.Game.CommunicationBox.RefreshList();
            GameScene.Game.CommunicationBox.UpdateIcon();

            if (mail == GameScene.Game.ReadMailBox.Mail)
                GameScene.Game.ReadMailBox.Mail = null;
        }

        public void Process(S.MailItemDelete p)  //邮件道具删除
        {
            ClientMailInfo mail = GameScene.Game.CommunicationBox.MailList.FirstOrDefault(x => x.Index == p.Index);

            if (mail == null) return;

            ClientUserItem item = mail.Items.FirstOrDefault(x => x.Slot == p.Slot);

            if (item != null)
            {
                mail.Items.Remove(item);

                foreach (MailRow row in GameScene.Game.CommunicationBox.Rows)
                {
                    if (row.Mail != mail) continue;

                    row.RefreshIcon();
                    break;
                }
            }

            GameScene.Game.CommunicationBox.UpdateIcon();

            if (mail != GameScene.Game.ReadMailBox.Mail) return;

            foreach (DXItemCell cell in GameScene.Game.ReadMailBox.Grid.Grid)
            {
                if (cell.Slot != p.Slot) continue;

                cell.Item = null;
                break;
            }
        }
        public void Process(S.MailSend p)  //邮件发送
        {
            GameScene.Game.CommunicationBox.SendAttempted = false;
        }

        #region Friend
        /*
         * 好友接口
         */

        public void Process(S.FriendSwitch p)
        {
            GameScene.Game.CommunicationBox.AllowFriend = p.Allow;
        }


        public void Process(S.FriendInvite p)
        {

            DXMessageBox messageBox = new DXMessageBox($"{p.Name} 请求添加你为好友", "添加好友请求", DXMessageBoxButtons.YesNo);

            messageBox.YesButton.MouseClick += (o, e) => CEnvir.Enqueue(new C.FriendResponse { Accept = true, Name = p.Name });
            messageBox.NoButton.MouseClick += (o, e) => CEnvir.Enqueue(new C.FriendResponse { Accept = false, Name = p.Name });
            messageBox.CloseButton.MouseClick += (o, e) => CEnvir.Enqueue(new C.FriendResponse { Accept = false, Name = p.Name });
            messageBox.Modal = false;
            messageBox.CloseButton.Visible = false;

        }

        public void Process(S.FriendList p)
        {
            GameScene.Game.CommunicationBox.FriendList.AddRange(p.Friend);
            GameScene.Game.CommunicationBox.RefreshList_Friend();
        }

        public void Process(S.FriendListRefresh p)
        {
            GameScene.Game.CommunicationBox.FriendList.RemoveRange(0, GameScene.Game.CommunicationBox.FriendList.Count);
            GameScene.Game.CommunicationBox.FriendList.AddRange(p.Friend);
            GameScene.Game.CommunicationBox.RefreshList_Friend();
        }

        public void Process(S.FriendNew p)
        {
            GameScene.Game.CommunicationBox.FriendList.Insert(0, p.Friend);
            GameScene.Game.CommunicationBox.RefreshList_Friend();
            GameScene.Game.ReceiveChat($"添加好友 {p.Friend.Name} 成功。", MessageType.System);
        }

        // 添加黑名单也需要调用这个方法
        // 双向删除
        public void Process(S.FriendDelete p)
        {
            ClientFriendInfo friend = GameScene.Game.CommunicationBox.FriendList.FirstOrDefault(x => x.Index == p.Friend.Index);
            if (friend == null)
            {
                if (p.isRequester)
                    GameScene.Game.ReceiveChat($"删除好友 {p.Friend.Name} 失败，未找到目标好友。", MessageType.System);
                return;
            }

            GameScene.Game.CommunicationBox.FriendList.Remove(friend);
            GameScene.Game.CommunicationBox.RefreshList_Friend();

            if (p.isRequester)
            {
                GameScene.Game.ReceiveChat($"删除好友 {p.Friend.Name} 成功。", MessageType.System);
            }
        }

        #endregion

        public void Process(S.ChangeAttackMode p)             //收到攻击模式封包
        {
            GameScene.Game.User.AttackMode = p.Mode;

            GameScene.Game.ReceiveChat(GameScene.Game.MainPanel.AttackModeLabel.Text, MessageType.System);
        }
        public void Process(S.ChangePetMode p)            //收到宠物模式封包
        {
            GameScene.Game.User.PetMode = p.Mode;

            GameScene.Game.ReceiveChat(GameScene.Game.MainPanel.PetModeLabel.Text, MessageType.System);
        }

        public void Process(S.WeightUpdate p)   //重量更新
        {
            if (GameScene.Game.User == null) return;
            GameScene.Game.User.BagWeight = p.BagWeight;
            GameScene.Game.User.WearWeight = p.WearWeight;
            GameScene.Game.User.HandWeight = p.HandWeight;
            GameScene.Game.WeightChanged();
        }

        public void Process(S.TradeRequest p)  //请求交易
        {
            DXMessageBox messageBox = new DXMessageBox($"Cconnection.TradeRequest".Lang(p.Name), "交易请求".Lang(), DXMessageBoxButtons.YesNo);

            messageBox.YesButton.MouseClick += (o, e) => CEnvir.Enqueue(new C.TradeRequestResponse { Accept = true });
            messageBox.NoButton.MouseClick += (o, e) => CEnvir.Enqueue(new C.TradeRequestResponse { Accept = false });
            messageBox.CloseButton.MouseClick += (o, e) => CEnvir.Enqueue(new C.TradeRequestResponse { Accept = false });
        }
        public void Process(S.TradeOpen p) //交易打开
        {
            GameScene.Game.TradeBox.Visible = true;
            GameScene.Game.TradeBox.IsTrading = true;
            GameScene.Game.TradeBox.PlayerLabel.Text = p.Name;
        }

        public void Process(S.TradeClose p)  //交易关闭
        {
            GameScene.Game.TradeBox.Visible = false;
            GameScene.Game.TradeBox.Clear();
        }
        public void Process(S.TradeAddItem p) //交易增加项目
        {
            DXItemCell fromCell;

            switch (p.Cell.GridType)
            {
                case GridType.Inventory:
                    fromCell = GameScene.Game.InventoryBox.Grid.Grid[p.Cell.Slot];
                    break;
                case GridType.PatchGrid:               //碎片包裹
                    fromCell = GameScene.Game.InventoryBox.PatchGrid.Grid[p.Cell.Slot];
                    break;
                case GridType.Equipment:
                    fromCell = GameScene.Game.CharacterBox.Grid[p.Cell.Slot];
                    break;
                case GridType.Storage:
                    fromCell = GameScene.Game.StorageBox.Grid.Grid[p.Cell.Slot];
                    break;
                /*    case GridType.GuildStorage:
                      fromCell = GameScene.Game.GuildPanel.StorageControl.Grid[p.FromSlot];
                      break;*/
                case GridType.CompanionInventory:
                    fromCell = GameScene.Game.CompanionBox.InventoryGrid.Grid[p.Cell.Slot];
                    break;
                case GridType.CompanionEquipment:
                    fromCell = GameScene.Game.CompanionBox.EquipmentGrid[p.Cell.Slot];
                    break;
                case GridType.FishingEquipment:  //钓鱼装备格子
                    fromCell = GameScene.Game.FishingBox.FishingGrid[p.Cell.Slot];
                    break;
                default: return;
            }

            if (!p.Success)
            {
                fromCell.Link = null;
                return;
            }

            if (fromCell.Link != null) return;

            foreach (DXItemCell cell in GameScene.Game.TradeBox.UserGrid.Grid)
            {
                if (cell.Item != null) continue;

                cell.LinkedCount = p.Cell.Count;
                cell.Link = fromCell;
                return;
            }
        }
        public void Process(S.TradeItemAdded p)  //增加交易道具
        {
            foreach (DXItemCell cell in GameScene.Game.TradeBox.PlayerGrid.Grid)
            {
                if (cell.Item != null) continue;

                cell.Item = p.Item;
                return;
            }
        }
        public void Process(S.TradeAddGold p)  //交易增加金币
        {
            GameScene.Game.TradeBox.UserGoldLabel.Text = p.Gold.ToString("#,##0");
        }
        public void Process(S.TradeGoldAdded p)   //交易金币增加
        {
            GameScene.Game.TradeBox.PlayerGoldLabel.Text = p.Gold.ToString("#,##0");
        }
        public void Process(S.TradeUnlock p)   //交易锁定
        {
            GameScene.Game.TradeBox.ConfirmButton.Enabled = true;
        }

        public void Process(S.sc_FixedPointList p)  //记忆传送功能
        {
            CEnvir.FixedPointTCount = p.FixedPointTCount;
            CEnvir.FixedPointList.Clear();
            CEnvir.FixedPointList = p.Info;
            if (null != GameScene.Game.FixedPointBox && true == GameScene.Game.FixedPointBox.IsVisible)
            {
                GameScene.Game.FixedPointBox.RefreshList();
            }
        }
        public void Process(S.GuildActiveCountChange p) //在这里更新自己的行会数值
        {
            GameScene.Game.GuildBox.DailyContributionLabel.Text = $"{p.DailyActiveCount.ToString("#,##0")} / {CEnvir.ClientControl.ActivationCeiling}";
        }

        public void Process(S.GuildCreate p)  //行会创建
        {
            GameScene.Game.GuildBox.CreateAttempted = false;
        }
        public void Process(S.GuildInfo p)   //行会信息
        {
            HashSet<uint> checks = new HashSet<uint>();

            if (GameScene.Game.GuildBox.GuildInfo != null)
            {
                foreach (ClientGuildMemberInfo member in GameScene.Game.GuildBox.GuildInfo.Members)
                    if (member.ObjectID > 0)
                        checks.Add(member.ObjectID);
            }

            GameScene.Game.GuildBox.GuildInfo = p.Guild;
            if (GameScene.Game.GuildBox.GuildInfo != null)
            {
                GameScene.Game.GuildBox.RecentChanges = p.Guild.FundChanges;
                GameScene.Game.GuildBox.TopRanks = p.Guild.FundRanks;
            }
            else
            {
                GameScene.Game.GuildBox.RecentChanges.Clear();
                GameScene.Game.GuildBox.TopRanks.Clear();
            }

            GameScene.Game.GuildAlliances.Clear();
            if (p.Guild?.Alliances != null)
            {
                foreach (ClientGuildAllianceInfo allianceInfo in p.Guild.Alliances)
                    GameScene.Game.GuildAlliances.Add(allianceInfo.Name);
            }

            if (GameScene.Game.GuildBox.GuildInfo != null)
            {
                foreach (ClientGuildMemberInfo member in GameScene.Game.GuildBox.GuildInfo.Members)
                    if (member.ObjectID > 0)
                        checks.Add(member.ObjectID);
            }

            GameScene.Game.GuildBox.DailyContributionLabel.Text = $"{p.Guild.OwnDailyActiveCount.ToString("#,##0")} / {CEnvir.ClientControl.ActivationCeiling}";

            foreach (uint objectID in checks)
            {
                ClientObjectData data;
                if (!GameScene.Game.DataDictionary.TryGetValue(objectID, out data)) return;

                GameScene.Game.OnUpdateObjectData(data);
            }

            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
                ob.NameChanged();
        }
        public void Process(S.GuildNoticeChanged p)   //行会公告改变
        {
            GameScene.Game.GuildBox.GuildInfo.Notice = p.Notice;

            if (!GameScene.Game.GuildBox.NoticeTextBox.Editable)
                GameScene.Game.GuildBox.NoticeTextBox.TextBox.Text = p.Notice;
        }
        public void Process(S.GuildVaultNoticeChanged p)   //行会金库公告改变
        {
            GameScene.Game.GuildBox.GuildInfo.VaultNotice = p.Notice;

            if (!GameScene.Game.GuildBox.FundNoticeTextBox.Editable)
                GameScene.Game.GuildBox.FundNoticeTextBox.TextBox.Text = p.Notice;
        }
        public void Process(S.GuildGetItem p)  //行会获得道具
        {
            DXItemCell[] grid;

            switch (p.Grid)
            {
                case GridType.Inventory:
                    grid = GameScene.Game.InventoryBox.Grid.Grid;
                    break;
                case GridType.PatchGrid:               //碎片包裹
                    grid = GameScene.Game.InventoryBox.PatchGrid.Grid;
                    break;
                case GridType.Equipment:
                    grid = GameScene.Game.CharacterBox.Grid;
                    break;
                case GridType.Storage:
                    grid = GameScene.Game.StorageBox.Grid.Grid;
                    break;
                case GridType.GuildStorage:
                    grid = GameScene.Game.GuildBox.StorageGrid.Grid;
                    break;
                case GridType.CompanionInventory:
                    grid = GameScene.Game.CompanionBox.InventoryGrid.Grid;
                    break;
                case GridType.CompanionEquipment:
                    grid = GameScene.Game.CompanionBox.EquipmentGrid;
                    break;
                case GridType.FishingEquipment:  //钓鱼装备格子
                    grid = GameScene.Game.FishingBox.FishingGrid;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            DXItemCell fromCell = grid[p.Slot];

            fromCell.Item = p.Item;
        }
        public void Process(S.GuildNewItem p)   //行会新存道具
        {
            GameScene.Game.GuildBox.StorageGrid.Grid[p.Slot].Item = p.Item;
        }
        public void Process(S.GuildUpdate p)   //行会更新
        {
            GameScene.Game.GuildBox.GuildInfo.GuildFunds = p.GuildFunds;
            GameScene.Game.GuildBox.GuildInfo.DailyGrowth = p.DailyGrowth;

            GameScene.Game.GuildBox.GuildInfo.TotalContribution = p.TotalContribution;
            GameScene.Game.GuildBox.GuildInfo.DailyContribution = p.DailyContribution;

            GameScene.Game.GuildBox.GuildInfo.ActiveCount = p.ActiveCount;
            GameScene.Game.GuildBox.GuildInfo.GuildExp = p.GuildExp;
            GameScene.Game.GuildBox.GuildInfo.GuildLevel = p.GuildLevel;

            GameScene.Game.GuildBox.GuildInfo.MemberLimit = p.MemberLimit;
            GameScene.Game.GuildBox.GuildInfo.StorageLimit = p.StorageLimit;

            GameScene.Game.GuildBox.GuildInfo.Tax = p.Tax;

            GameScene.Game.GuildBox.GuildInfo.DefaultPermission = p.DefaultPermission;
            GameScene.Game.GuildBox.GuildInfo.DefaultRank = p.DefaultRank;

            GameScene.Game.GuildBox.GuildInfo.Flag = p.Flag;

            GameScene.Game.GuildBox.GuildInfo.FlagColor = p.FlagColor;

            GameScene.Game.UpdateGuildFlag(p.Flag, p.FlagColor);

            foreach (ClientGuildMemberInfo member in p.Members)
            {
                ClientGuildMemberInfo info = GameScene.Game.GuildBox.GuildInfo.Members.FirstOrDefault(x => x.Index == member.Index);

                if (info == null)
                {
                    info = new ClientGuildMemberInfo
                    {
                        Index = member.Index,
                        Name = member.Name,
                    };
                    GameScene.Game.GuildBox.GuildInfo.Members.Add(info);
                }

                info.TotalContribution = member.TotalContribution;
                info.DailyContribution = member.DailyContribution;
                info.LastOnline = member.LastOnline;
                info.Permission = member.Permission;
                info.Rank = member.Rank;
                info.ObjectID = member.ObjectID;

                if (info.Index == GameScene.Game.GuildBox.GuildInfo.UserIndex)
                    GameScene.Game.GuildBox.PermissionChanged();

                ClientObjectData data;
                if (!GameScene.Game.DataDictionary.TryGetValue(member.ObjectID, out data)) continue;

                GameScene.Game.OnUpdateObjectData(data);
            }

            GameScene.Game.GuildBox.TopRanks = p.FundRanks;

            if (GameScene.Game.GuildBox.Visible)
                GameScene.Game.GuildBox.RefreshGuildDisplay();
        }
        public void Process(S.GuildKick p)  //行会踢人
        {
            ClientGuildMemberInfo info = GameScene.Game.GuildBox.GuildInfo.Members.FirstOrDefault(x => x.Index == p.Index);
            if (info == null) return;

            GameScene.Game.GuildBox.GuildInfo.Members.Remove(info);

            if (GameScene.Game.GuildBox.Visible)
                GameScene.Game.GuildBox.RefreshGuildDisplay();

            ClientObjectData data;
            if (!GameScene.Game.DataDictionary.TryGetValue(info.ObjectID, out data)) return;

            GameScene.Game.OnUpdateObjectData(data);
        }
        public void Process(S.GuildIncreaseMember p)  //行会增加会员容量
        {
            GameScene.Game.GuildBox.IncreaseMemberButton.Enabled = true;
        }
        public void Process(S.GuildIncreaseStorage p)  //行会增加仓库容量
        {
            GameScene.Game.GuildBox.IncreaseStorageButton.Enabled = true;
        }
        public void Process(S.GuildInviteMember p)  //行会邀请成功
        {
            GameScene.Game.GuildBox.AddMemberTextBox.Enabled = true;
            GameScene.Game.GuildBox.AddMemberButton.Enabled = true;
        }
        public void Process(S.GuildTax p)   //行会税率
        {
            GameScene.Game.GuildBox.GuildTaxBox.Enabled = true;
            GameScene.Game.GuildBox.SetTaxButton.Enabled = true;
        }

        public void Process(S.GuildMemberOffline p)  //行会成员离线
        {
            ClientGuildMemberInfo info = GameScene.Game.GuildBox.GuildInfo.Members.FirstOrDefault(x => x.Index == p.Index);
            if (info == null) return;

            info.LastOnline = CEnvir.Now;
            info.ObjectID = 0;

            if (GameScene.Game.GuildBox.Visible)
                GameScene.Game.GuildBox.RefreshGuildDisplay();
        }
        public void Process(S.GuildInvite p)  //行会邀请
        {
            DXMessageBox messageBox = new DXMessageBox($"Cconnection.GuildInvite".Lang(p.Name, p.GuildName), "行会邀请".Lang(), DXMessageBoxButtons.YesNo);

            messageBox.YesButton.MouseClick += (o, e) => CEnvir.Enqueue(new C.GuildResponse { Accept = true });
            messageBox.NoButton.MouseClick += (o, e) => CEnvir.Enqueue(new C.GuildResponse { Accept = false });
            messageBox.CloseButton.MouseClick += (o, e) => CEnvir.Enqueue(new C.GuildResponse { Accept = false });
            messageBox.Modal = false;
            messageBox.CloseButton.Visible = false;
        }
        public void Process(S.GuildMemberOnline p)  //行会成员在线
        {
            ClientGuildMemberInfo info = GameScene.Game.GuildBox.GuildInfo.Members.FirstOrDefault(x => x.Index == p.Index);
            if (info == null) return;

            info.LastOnline = DateTime.MaxValue;
            info.Name = p.Name;
            info.ObjectID = p.ObjectID;

            if (GameScene.Game.GuildBox.Visible)
                GameScene.Game.GuildBox.RefreshGuildDisplay();
        }
        public void Process(S.GuildAllyOffline p)   //行会联盟离线
        {
            ClientGuildAllianceInfo info = GameScene.Game.GuildBox.GuildInfo.Alliances.FirstOrDefault(x => x.Index == p.Index);
            if (info == null) return;

            info.OnlineCount--;

            if (GameScene.Game.GuildBox.Visible)
                GameScene.Game.GuildBox.RefreshGuildDisplay();
        }
        public void Process(S.GuildAllyOnline p)   //行会联盟在线
        {
            ClientGuildAllianceInfo info = GameScene.Game.GuildBox.GuildInfo.Alliances.FirstOrDefault(x => x.Index == p.Index);
            if (info == null) return;

            info.OnlineCount++;

            if (GameScene.Game.GuildBox.Visible)
                GameScene.Game.GuildBox.RefreshGuildDisplay();
        }
        public void Process(S.GuildMemberContribution p)  //行会成员贡献
        {
            ClientGuildMemberInfo info = GameScene.Game.GuildBox.GuildInfo.Members.FirstOrDefault(x => x.Index == p.Index);
            if (info == null) return;

            info.DailyContribution += p.Contribution;
            info.TotalContribution += p.Contribution;

            GameScene.Game.GuildBox.GuildInfo.GuildFunds += p.Contribution;
            GameScene.Game.GuildBox.GuildInfo.DailyGrowth += p.Contribution;

            GameScene.Game.GuildBox.GuildInfo.TotalContribution += p.Contribution;
            GameScene.Game.GuildBox.GuildInfo.DailyContribution += p.Contribution;

            if (GameScene.Game.GuildBox.Visible)
                GameScene.Game.GuildBox.RefreshGuildDisplay();

            if (p.IsVoluntary)
            {
                GameScene.Game.GuildBox.RecentChanges.Insert(0, new ClientGuildFundChangeInfo { Amount = p.Contribution, Name = info.Name });
            }
        }
        public void Process(S.GuildDayReset p)  //行会每日重置
        {
            foreach (ClientGuildMemberInfo member in GameScene.Game.GuildBox.GuildInfo.Members)
                member.DailyContribution = 0;

            GameScene.Game.GuildBox.GuildInfo.DailyGrowth = 0;
            GameScene.Game.GuildBox.GuildInfo.DailyContribution = 0;

            if (GameScene.Game.GuildBox.Visible)
                GameScene.Game.GuildBox.RefreshGuildDisplay();
        }
        public void Process(S.GuildFundsChanged p)  //行会资金改变
        {
            GameScene.Game.GuildBox.GuildInfo.GuildFunds += p.Change;
            GameScene.Game.GuildBox.GuildInfo.DailyGrowth += p.Change;

            if (GameScene.Game.GuildBox.Visible)
                GameScene.Game.GuildBox.RefreshGuildDisplay();
        }
        public void Process(S.GuildChanged p)   //行会改变
        {
            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
            {
                if (ob.ObjectID != p.ObjectID) continue;

                ((PlayerObject)ob).Title = p.GuildName;
                ((PlayerObject)ob).GuildRank = p.GuildRank;
                return;
            }
        }
        public void Process(S.GuildWar p)   //行会战
        {
            GameScene.Game.GuildBox.WarAttempted = false;

            if (p.Success)
                GameScene.Game.GuildBox.GuildWarTextBox.TextBox.Text = string.Empty;
        }
        public void Process(S.GuildAlliance p)   //行会联盟
        {
            GameScene.Game.GuildBox.AllyAttempted = false;

            if (p.Success)
                GameScene.Game.GuildBox.GuildAllyTextBox.TextBox.Text = string.Empty;
        }
        public void Process(S.GuildAllianceStarted p)  //行会联盟开始
        {
            GameScene.Game.GuildAlliances.Add(p.AllianceInfo.Name);
            GameScene.Game.GuildBox.GuildInfo.Alliances.Add(p.AllianceInfo);
            GameScene.Game.ReceiveChat($"Cconnection.GuildAllianceStarted".Lang(p.AllianceInfo.Name), MessageType.Hint);

            GameScene.Game.GuildBox.RefreshGuildDisplay();
        }
        public void Process(S.GuildAllianceEnded p)   //行会联盟结束
        {
            GameScene.Game.GuildAlliances.Remove(p.GuildName);
            for (int i = 0; i < GameScene.Game.GuildBox.GuildInfo.Alliances.Count; i++)
            {
                if (GameScene.Game.GuildBox.GuildInfo.Alliances[i].Name == p.GuildName)
                {
                    GameScene.Game.GuildBox.GuildInfo.Alliances.RemoveAt(i);
                    break;
                }
            }
            GameScene.Game.ReceiveChat($"Cconnection.GuildAllianceEnded".Lang(p.GuildName), MessageType.Hint);

            GameScene.Game.GuildBox.RefreshGuildDisplay();
        }

        public void Process(S.GuildWarStarted p)   //行会站进行
        {
            GameScene.Game.GuildWars.Add(p.GuildName);

            GameScene.Game.ReceiveChat($"Cconnection.GuildWarStarted".Lang(p.GuildName, p.Duration.Lang(true)), MessageType.Hint);

            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
                ob.NameChanged();
        }
        public void Process(S.GuildWarFinished p)  //行会站结束
        {
            GameScene.Game.GuildWars.Remove(p.GuildName);

            GameScene.Game.ReceiveChat($"Cconnection.GuildWarFinished".Lang(p.GuildName), MessageType.Hint);

            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
                ob.NameChanged();
        }
        public void Process(S.GuildConquestStarted p)  //行会攻城战开始
        {
            CastleInfo castle = CEnvir.CastleInfoList.Binding.FirstOrDefault(x => x.Index == p.Index);
            if (castle == null) return;

            GameScene.Game.ConquestWars.Add(castle);
            CEnvir.WarFlagsDict.Add(castle, p.FlagLocation);

            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
                ob.NameChanged();
        }
        public void Process(S.GuildConquestFinished p)  //行会攻城战结束
        {
            CastleInfo castle = CEnvir.CastleInfoList.Binding.FirstOrDefault(x => x.Index == p.Index);
            if (castle == null) return;

            GameScene.Game.ConquestWars.Remove(castle);
            CEnvir.WarFlagsDict.Remove(castle);

            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
                ob.NameChanged();
        }
        public void Process(S.GuildCastleInfo p)  //行会攻城信息
        {
            CastleInfo castle = CEnvir.CastleInfoList.Binding.FirstOrDefault(x => x.Index == p.Index);
            if (castle == null) return;

            GameScene.Game.CastleOwners[castle] = p.Owner;

            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
                ob.NameChanged();

            GameScene.Game.GuildBox.CastlePanels[castle].Update();
        }
        public void Process(S.GuildConquestDate p)  //行会攻城日期
        {
            CastleInfo castle = CEnvir.CastleInfoList.Binding.FirstOrDefault(x => x.Index == p.Index);
            if (castle == null) return;

            castle.WarDate = p.WarDate;
        }

        public void Process(S.ReviveTimers p)   //复活冷却时间
        {
            GameScene.Game.ItemReviveTime = CEnvir.Now + p.ItemReviveTime;
            GameScene.Game.ReincarnationPillTime = CEnvir.Now + p.ReincarnationPillTime;
        }

        public void Process(S.QuestChanged p)  //任务改变
        {
            foreach (ClientUserQuest quest in GameScene.Game.QuestLog)
            {
                if (quest.Quest != p.Quest.Quest) continue;

                quest.Completed = p.Quest.Completed;
                quest.Track = p.Quest.Track;
                quest.ExtraInfo = p.Quest.ExtraInfo;
                quest.SelectedReward = p.Quest.SelectedReward;
                quest.Tasks.Clear();
                quest.Tasks.AddRange(p.Quest.Tasks);

                GameScene.Game.QuestChanged(p.Quest);
                return;
            }
            GameScene.Game.QuestLog.Add(p.Quest);
            GameScene.Game.QuestChanged(p.Quest);
        }

        public void Process(S.QuestRemoved p)  //任务已删除
        {
            ClientUserQuest quest = GameScene.Game.QuestLog.FirstOrDefault(x => x.QuestIndex == p.Quest.QuestIndex);
            if (quest == null) return;

            GameScene.Game.QuestLog.Remove(quest);
            if (GameScene.Game.QuestBox.CurrentTab.Quests.Remove(quest.Quest))
            {
                GameScene.Game.QuestBox.CurrentTab.NeedUpdate = true;
            }
            if (GameScene.Game.QuestBox.CurrentTab.Quests.Remove(quest.Quest))
            {
                GameScene.Game.QuestBox.CurrentTab.NeedUpdate = true;
            }
            if (GameScene.Game.QuestBox.CurrentTab.Quests.Remove(quest.Quest))
            {
                GameScene.Game.QuestBox.CurrentTab.NeedUpdate = true;
            }

            GameScene.Game.CheckNewQuests();
            GameScene.Game.QuestBox.PopulateQuests();

            GameScene.Game.User.DailyQuestRemains = p.DailyQuestRemains;
            GameScene.Game.User.RepeatableQuestRemains = p.RepeatableQuestRemains;
        }

        public void Process(S.CompanionUnlock p)   //宠物解锁
        {
            GameScene.Game.NPCAdoptCompanionBox.UnlockButton.Enabled = true;
            if (p.Index == 0) return;

            var compan = Globals.CompanionInfoList.Binding.FirstOrDefault(x => x.Index == p.Index);
            if (compan == null) return;

            GameScene.Game.NPCAdoptCompanionBox.AvailableCompanions.Add(compan);

            GameScene.Game.NPCAdoptCompanionBox.RefreshUnlockButton();
        }

        public void Process(S.CompanionAutoFeedUnlocked p)   // 自动粮仓解锁
        {
            GameScene.Game.CompanionBox.UnlockButton.Visible = !p.AutoFeedUnlocked; //如果已解锁 就不显示图标
            ClientUserCompanion companion =
                GameScene.Game.NPCCompanionStorageBox.Companions.FirstOrDefault(x => x.Index == p.Index);
            if (companion != null)
                companion.AutoFeed = p.AutoFeedUnlocked;
        }

        public void Process(S.CompanionAdopt p)   //宠物领养
        {
            GameScene.Game.NPCAdoptCompanionBox.AdoptAttempted = false;

            if (p.UserCompanion == null) return;

            GameScene.Game.NPCCompanionStorageBox.Companions.Add(p.UserCompanion);
            GameScene.Game.NPCCompanionStorageBox.UpdateScrollBar();
            GameScene.Game.NPCAdoptCompanionBox.CompanionNameTextBox.TextBox.Text = string.Empty;
        }
        public void Process(S.CompanionStore p)   //宠物寄存
        {
            if (GameScene.Game.Companion != null)
                GameScene.Game.Companion.CharacterName = null;

            GameScene.Game.Companion = null;
        }
        public void Process(S.CompanionRetrieve p)  //宠物取回
        {
            GameScene.Game.Companion = GameScene.Game.NPCCompanionStorageBox.Companions.FirstOrDefault(x => x.Index == p.Index);
            if (GameScene.Game.Companion != null)
                GameScene.Game.CompanionBox.UnlockButton.Visible = !GameScene.Game.Companion.AutoFeed;
        }
        public void Process(S.CompanionWeightUpdate p)   //宠物重量更新
        {
            GameScene.Game.CompanionBox.BagWeight = p.BagWeight;
            GameScene.Game.CompanionBox.MaxBagWeight = p.MaxBagWeight;
            GameScene.Game.CompanionBox.InventorySize = p.InventorySize;

            GameScene.Game.CompanionBox.Refresh();
        }
        public void Process(S.CompanionShapeUpdate p)   //宠物外观更新
        {
            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
            {
                if (ob.Race != ObjectType.Monster || ob.ObjectID != p.ObjectID) continue;

                MonsterObject monster = (MonsterObject)ob;

                if (monster.CompanionObject == null) continue;

                monster.CompanionObject.HeadShape = p.HeadShape;
                monster.CompanionObject.BackShape = p.BackShape;
                return;
            }
        }
        public void Process(S.CompanionUpdate p)   //宠物更新
        {
            GameScene.Game.Companion.Hunger = p.Hunger;
            GameScene.Game.Companion.Level = p.Level;
            GameScene.Game.Companion.Experience = p.Experience;

            GameScene.Game.CompanionBox.Refresh();
        }
        public void Process(S.CompanionItemsGained p)   //宠物捡取道具
        {
            foreach (ClientUserItem item in p.Items)
            {
                ItemInfo displayInfo = item.Info;

                if (item.Info.Effect == ItemEffect.ItemPart)
                {
                    var info = Globals.ItemInfoList.Binding.FirstOrDefault(x => x.Index == item.AddedStats[Stat.ItemIndex]);
                    if (info != null)
                        displayInfo = info;
                }

                item.New = true;
                string text = item.Count > 1 ? $"Cconnection.CompanionItemsCount".Lang(displayInfo.Lang(p => p.ItemName), item.Count) : $"Cconnection.CompanionItems".Lang(displayInfo.Lang(p => p.ItemName));

                if (item.Info.Effect == ItemEffect.Gold)
                    text = $"Cconnection.CompanionItemsGold".Lang(item.Count, displayInfo.Lang(p => p.ItemName));

                if ((item.Flags & UserItemFlags.QuestItem) == UserItemFlags.QuestItem)
                    text += " (" + "任务".Lang() + ")";

                if (item.Info.Effect == ItemEffect.ItemPart)
                    text += " [" + "碎片".Lang() + "]";

                GameScene.Game.ReceiveChat(text, MessageType.Combat);
            }

            GameScene.Game.AddCompanionItems(p.Items);
        }
        public void Process(S.CompanionSkillUpdate p)   //宠物等级更新
        {
            GameScene.Game.Companion.Level3 = p.Level3;
            GameScene.Game.Companion.Level5 = p.Level5;
            GameScene.Game.Companion.Level7 = p.Level7;
            GameScene.Game.Companion.Level10 = p.Level10;
            GameScene.Game.Companion.Level11 = p.Level11;
            GameScene.Game.Companion.Level13 = p.Level13;
            GameScene.Game.Companion.Level15 = p.Level15;

            GameScene.Game.CompanionBox.Refresh();
        }

        public void Process(S.MarriageInvite p)  //结婚请求
        {
            DXMessageBox messageBox = new DXMessageBox($"Cconnection.MarriageInvite".Lang(p.Name), "婚姻请求".Lang(), DXMessageBoxButtons.YesNo);

            messageBox.YesButton.MouseClick += (o, e) => CEnvir.Enqueue(new C.MarriageResponse { Accept = true });
            messageBox.NoButton.MouseClick += (o, e) => CEnvir.Enqueue(new C.MarriageResponse { Accept = false });
            messageBox.CloseButton.MouseClick += (o, e) => CEnvir.Enqueue(new C.MarriageResponse { Accept = false });
            messageBox.Modal = false;
            messageBox.CloseButton.Visible = false;
        }
        public void Process(S.MarriageInfo p)   //结婚信息
        {
            ClientObjectData data;

            GameScene.Game.DataDictionary.TryGetValue(GameScene.Game.Partner?.ObjectID ?? p.Partner.ObjectID, out data);

            GameScene.Game.Partner = p.Partner;

            if (data == null) return;

            GameScene.Game.OnUpdateObjectData(data);
        }

        public void Process(S.MarriageRemoveRing p)   //制作婚戒
        {
            GameScene.Game.CharacterBox.Grid[(int)EquipmentSlot.RingL].Item.Flags &= ~UserItemFlags.Marriage;
        }

        public void Process(S.MarriageMakeRing p)    //移除婚戒
        {
            GameScene.Game.CharacterBox.Grid[(int)EquipmentSlot.RingL].Item.Flags |= UserItemFlags.Marriage;
        }

        public void Process(S.MarriageOnlineChanged p)  //离婚
        {
            ClientObjectData data;

            GameScene.Game.DataDictionary.TryGetValue(GameScene.Game.Partner.ObjectID > 0 ? GameScene.Game.Partner.ObjectID : p.ObjectID, out data);

            GameScene.Game.Partner.ObjectID = p.ObjectID;

            if (data == null) return;

            GameScene.Game.OnUpdateObjectData(data);
        }

        public void Process(S.DataObjectPlayer p)  //数据对象玩家
        {
            ClientObjectData data = new ClientObjectData
            {
                ObjectID = p.ObjectID,

                MapIndex = p.MapIndex,
                Location = p.CurrentLocation,

                Name = p.Name,

                Health = p.Health,
                MaxHealth = p.MaxHealth,
                Dead = p.Dead,

                Mana = p.Mana,
                MaxMana = p.MaxMana,
            };

            GameScene.Game.DataDictionary[p.ObjectID] = data;
            GameScene.Game.OnUpdateObjectData(data);
        }
        public void Process(S.DataObjectMonster p)   //数据对象怪物
        {
            ClientObjectData data = new ClientObjectData
            {
                ObjectID = p.ObjectID,

                MapIndex = p.MapIndex,
                Location = p.CurrentLocation,

                MonsterInfo = p.MonsterInfo,

                Health = p.Health,
                MaxHealth = p.Stats[Stat.Health],
                Stats = p.Stats,
                Dead = p.Dead,

                PetOwner = p.PetOwner,
            };

            GameScene.Game.DataDictionary[p.ObjectID] = data;

            GameScene.Game.OnUpdateObjectData(data);
        }
        public void Process(S.DataObjectItem p)  //数据对象道具
        {
            ClientObjectData data = new ClientObjectData
            {
                ObjectID = p.ObjectID,

                MapIndex = p.MapIndex,
                Location = p.CurrentLocation,

                ItemInfo = p.ItemInfo,
                Name = p.ItemInfo.Lang(p => p.ItemName),
            };

            GameScene.Game.DataDictionary[p.ObjectID] = data;

            GameScene.Game.OnUpdateObjectData(data);
        }
        public void Process(S.DataObjectRemove p)  //数据对象移除
        {
            ClientObjectData data;

            if (!GameScene.Game.DataDictionary.TryGetValue(p.ObjectID, out data)) return;

            GameScene.Game.DataDictionary.Remove(p.ObjectID);

            GameScene.Game.OnRemoveObjectData(data);
        }
        public void Process(S.DataObjectLocation p)  //数据对象位置
        {
            ClientObjectData data;

            if (!GameScene.Game.DataDictionary.TryGetValue(p.ObjectID, out data)) return;

            data.Location = p.CurrentLocation;
            data.MapIndex = p.MapIndex;

            GameScene.Game.OnUpdateObjectData(data);
        }

        public void Process(S.DataObjectHealthMana p)  //数据对象加血加蓝
        {
            ClientObjectData data;

            if (!GameScene.Game.DataDictionary.TryGetValue(p.ObjectID, out data)) return;

            data.Health = p.Health;
            data.Mana = p.Mana;

            if (GameScene.Game.MonsterBox.Monster != null && GameScene.Game.MonsterBox.Monster.ObjectID == p.ObjectID)
                GameScene.Game.MonsterBox.RefreshHealth();

            if (data.Dead != p.Dead)
            {
                data.Dead = p.Dead;
                GameScene.Game.OnUpdateObjectData(data);
            }
        }
        public void Process(S.DataObjectMaxHealthMana p)  //数据对象最大血蓝值
        {
            ClientObjectData data;

            if (!GameScene.Game.DataDictionary.TryGetValue(p.ObjectID, out data)) return;

            if (p.Stats != null)
            {
                data.MaxHealth = p.Stats[Stat.Health];
                data.MaxMana = p.Stats[Stat.Mana];
                data.Stats = p.Stats;
            }
            else
            {
                data.MaxHealth = p.MaxHealth;
                data.MaxMana = p.MaxMana;
            }
            if (GameScene.Game.MonsterBox.Monster != null && GameScene.Game.MonsterBox.Monster.ObjectID == p.ObjectID)
                GameScene.Game.MonsterBox.RefreshStats();
        }

        public void Process(S.BlockAdd p)  //增加锁定
        {
            CEnvir.BlockList.Add(p.Info);

            GameScene.Game.CommunicationBox.RefreshBlockList();
        }
        public void Process(S.BlockRemove p)  //移除锁定
        {
            ClientBlockInfo block = CEnvir.BlockList.FirstOrDefault(x => x.Index == p.Index);
            if (block == null) return;

            CEnvir.BlockList.Remove(block);

            GameScene.Game.CommunicationBox.RefreshBlockList();
        }

        public void Process(S.HelmetToggle p)   //头盔显示
        {
            GameScene.Game.CharacterBox.ShowHelmetBox.Checked = !p.HideHelmet;
        }

        public void Process(S.ShieldToggle p)   //盾牌显示
        {
            GameScene.Game.CharacterBox.ShowShieldBox.Checked = !p.HideShield;
        }

        public void Process(S.FashionToggle p)   //时装显示
        {
            GameScene.Game.CharacterBox.ShowFashionBox.Checked = !p.HideFashion;
        }

        public void Process(S.StorageSize p)  //仓库容量
        {
            GameScene.Game.StorageSize = p.Size;
        }
        public void Process(S.PatchGridSize p)  //碎片包裹容量
        {
            GameScene.Game.PatchGridSize = p.Size;

        }
        public void Process(S.PlayerChangeUpdate p)  //玩家变化更新
        {
            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
            {
                if (ob.Race != ObjectType.Player || ob.ObjectID != p.ObjectID) continue;

                PlayerObject player = (PlayerObject)ob;

                player.Name = p.Name;
                player.Gender = p.Gender;
                player.HairType = p.HairType;
                player.HairColour = p.HairColour;
                player.ArmourColour = p.ArmourColour;

                player.UpdateLibraries();
                return;
            }
        }

        public void Process(S.FortuneUpdate p)  //财富更新
        {
            foreach (ClientFortuneInfo fortune in p.Fortunes)
            {
                ClientFortuneInfo info;
                if (!GameScene.Game.FortuneDictionary.TryGetValue(fortune.ItemInfo, out info))
                {
                    GameScene.Game.FortuneDictionary[fortune.ItemInfo] = fortune;
                    continue;
                }

                info.DropCount = fortune.DropCount;
                info.Progress = fortune.Progress;
                info.CheckDate = fortune.CheckDate;
            }

            if (!GameScene.Game.FortuneCheckerBox.Visible) return;

            GameScene.Game.FortuneCheckerBox.RefreshList();
        }

        public void Process(S.NPCWeaponCraft p)  //NPC武器工艺
        {
            #region Template

            DXItemCell[] grid;

            switch (p.Template.GridType)
            {
                case GridType.Inventory:
                    grid = GameScene.Game.InventoryBox.Grid.Grid;
                    break;
                /*case GridType.PatchGrid:               //碎片包裹 先注释
                    grid = GameScene.Game.InventoryBox.PatchGrid.Grid;
                    break;*/
                case GridType.Equipment:
                    grid = GameScene.Game.CharacterBox.Grid;
                    break;
                case GridType.Storage:
                    grid = GameScene.Game.StorageBox.Grid.Grid;
                    break;
                case GridType.CompanionInventory:
                    grid = GameScene.Game.CompanionBox.InventoryGrid.Grid;
                    break;
                case GridType.CompanionEquipment:
                    grid = GameScene.Game.CompanionBox.EquipmentGrid;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            DXItemCell fromCell = grid[p.Template.Slot];
            fromCell.Locked = false;

            if (p.Success)
            {
                if (p.Template.Count == fromCell.Item.Count)
                    fromCell.Item = null;
                else
                    fromCell.Item.Count -= p.Template.Count;

                fromCell.RefreshItem();
            }
            #endregion

            #region Yellow

            if (p.Yellow != null)
            {
                switch (p.Yellow.GridType)
                {
                    case GridType.Inventory:
                        grid = GameScene.Game.InventoryBox.Grid.Grid;
                        break;
                    /*case GridType.PatchGrid:               //碎片包裹先注释
                        grid = GameScene.Game.InventoryBox.PatchGrid.Grid;
                        break;*/
                    case GridType.Equipment:
                        grid = GameScene.Game.CharacterBox.Grid;
                        break;
                    case GridType.Storage:
                        grid = GameScene.Game.StorageBox.Grid.Grid;
                        break;
                    case GridType.CompanionInventory:
                        grid = GameScene.Game.CompanionBox.InventoryGrid.Grid;
                        break;
                    case GridType.CompanionEquipment:
                        grid = GameScene.Game.CompanionBox.EquipmentGrid;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                fromCell = grid[p.Yellow.Slot];
                fromCell.Locked = false;

                if (p.Success)
                {
                    if (p.Yellow.Count == fromCell.Item.Count)
                        fromCell.Item = null;
                    else
                        fromCell.Item.Count -= p.Yellow.Count;

                    fromCell.RefreshItem();
                }
            }

            #endregion

            #region Blue

            if (p.Blue != null)
            {
                switch (p.Blue.GridType)
                {
                    case GridType.Inventory:
                        grid = GameScene.Game.InventoryBox.Grid.Grid;
                        break;
                    /*case GridType.PatchGrid:               //碎片包裹先注释
                        grid = GameScene.Game.InventoryBox.PatchGrid.Grid;
                        break;*/
                    case GridType.Equipment:
                        grid = GameScene.Game.CharacterBox.Grid;
                        break;
                    case GridType.Storage:
                        grid = GameScene.Game.StorageBox.Grid.Grid;
                        break;
                    case GridType.CompanionInventory:
                        grid = GameScene.Game.CompanionBox.InventoryGrid.Grid;
                        break;
                    case GridType.CompanionEquipment:
                        grid = GameScene.Game.CompanionBox.EquipmentGrid;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                fromCell = grid[p.Blue.Slot];
                fromCell.Locked = false;

                if (p.Success)
                {
                    if (p.Blue.Count == fromCell.Item.Count)
                        fromCell.Item = null;
                    else
                        fromCell.Item.Count -= p.Blue.Count;

                    fromCell.RefreshItem();
                }
            }

            #endregion

            #region Red

            if (p.Red != null)
            {
                switch (p.Red.GridType)
                {
                    case GridType.Inventory:
                        grid = GameScene.Game.InventoryBox.Grid.Grid;
                        break;
                    /*case GridType.PatchGrid:               //碎片包裹先注释
                        grid = GameScene.Game.InventoryBox.PatchGrid.Grid;
                        break;*/
                    case GridType.Equipment:
                        grid = GameScene.Game.CharacterBox.Grid;
                        break;
                    case GridType.Storage:
                        grid = GameScene.Game.StorageBox.Grid.Grid;
                        break;
                    case GridType.CompanionInventory:
                        grid = GameScene.Game.CompanionBox.InventoryGrid.Grid;
                        break;
                    case GridType.CompanionEquipment:
                        grid = GameScene.Game.CompanionBox.EquipmentGrid;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                fromCell = grid[p.Red.Slot];
                fromCell.Locked = false;

                if (p.Success)
                {
                    if (p.Red.Count == fromCell.Item.Count)
                        fromCell.Item = null;
                    else
                        fromCell.Item.Count -= p.Red.Count;

                    fromCell.RefreshItem();
                }
            }

            #endregion

            #region Purple

            if (p.Purple != null)
            {
                switch (p.Purple.GridType)
                {
                    case GridType.Inventory:
                        grid = GameScene.Game.InventoryBox.Grid.Grid;
                        break;
                    /*case GridType.PatchGrid:               //碎片包裹先注释
                        grid = GameScene.Game.InventoryBox.PatchGrid.Grid;
                        break;*/
                    case GridType.Equipment:
                        grid = GameScene.Game.CharacterBox.Grid;
                        break;
                    case GridType.Storage:
                        grid = GameScene.Game.StorageBox.Grid.Grid;
                        break;
                    case GridType.CompanionInventory:
                        grid = GameScene.Game.CompanionBox.InventoryGrid.Grid;
                        break;
                    case GridType.CompanionEquipment:
                        grid = GameScene.Game.CompanionBox.EquipmentGrid;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                fromCell = grid[p.Purple.Slot];
                fromCell.Locked = false;

                if (p.Success)
                {
                    if (p.Purple.Count == fromCell.Item.Count)
                        fromCell.Item = null;
                    else
                        fromCell.Item.Count -= p.Purple.Count;

                    fromCell.RefreshItem();
                }
            }

            #endregion

            #region Green

            if (p.Green != null)
            {
                switch (p.Green.GridType)
                {
                    case GridType.Inventory:
                        grid = GameScene.Game.InventoryBox.Grid.Grid;
                        break;
                    /*case GridType.PatchGrid:               //碎片包裹先注释
                        grid = GameScene.Game.InventoryBox.PatchGrid.Grid;
                        break;*/
                    case GridType.Equipment:
                        grid = GameScene.Game.CharacterBox.Grid;
                        break;
                    case GridType.Storage:
                        grid = GameScene.Game.StorageBox.Grid.Grid;
                        break;
                    case GridType.CompanionInventory:
                        grid = GameScene.Game.CompanionBox.InventoryGrid.Grid;
                        break;
                    case GridType.CompanionEquipment:
                        grid = GameScene.Game.CompanionBox.EquipmentGrid;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                fromCell = grid[p.Green.Slot];
                fromCell.Locked = false;

                if (p.Success)
                {
                    if (p.Green.Count == fromCell.Item.Count)
                        fromCell.Item = null;
                    else
                        fromCell.Item.Count -= p.Green.Count;

                    fromCell.RefreshItem();
                }
            }

            #endregion

            #region Grey

            if (p.Grey != null)
            {
                switch (p.Grey.GridType)
                {
                    case GridType.Inventory:
                        grid = GameScene.Game.InventoryBox.Grid.Grid;
                        break;
                    /*case GridType.PatchGrid:               //碎片包裹先注释
                        grid = GameScene.Game.InventoryBox.PatchGrid.Grid;
                        break;*/
                    case GridType.Equipment:
                        grid = GameScene.Game.CharacterBox.Grid;
                        break;
                    case GridType.Storage:
                        grid = GameScene.Game.StorageBox.Grid.Grid;
                        break;
                    case GridType.CompanionInventory:
                        grid = GameScene.Game.CompanionBox.InventoryGrid.Grid;
                        break;
                    case GridType.CompanionEquipment:
                        grid = GameScene.Game.CompanionBox.EquipmentGrid;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                fromCell = grid[p.Grey.Slot];
                fromCell.Locked = false;

                if (p.Success)
                {
                    if (p.Grey.Count == fromCell.Item.Count)
                        fromCell.Item = null;
                    else
                        fromCell.Item.Count -= p.Grey.Count;

                    fromCell.RefreshItem();
                }
            }

            #endregion
        }

        public void Process(S.CraftStartFailed p)  //制造物品失败
        {
            GameScene.Game.ReceiveChat("无法开始制造".Lang() + ": " + p.Reason, MessageType.System);
        }
        public void Process(S.CraftAcknowledged p)  //制造物品已接受
        {
            CraftItemInfo item = Globals.CraftItemInfoList.Binding.FirstOrDefault(x => x.Index == p.TargetItemIndex);
            if (item != null)
            {
                GameScene.Game.ReceiveChat("开始制造".Lang() + ": " + item.Item.Lang(p => p.ItemName), MessageType.System);
                GameScene.Game.CraftStarted(item, p.CompleteTime);
            }
            else
            {
                GameScene.Game.ReceiveChat("无法制造请更新客户端".Lang(), MessageType.System);
            }
        }

        public void Process(S.CraftResult p)  //制作
        {
            GameScene.Game.ReceiveChat(p.Reason, MessageType.System);
            GameScene.Game.User.CraftingItem = null;
            GameScene.Game.User.CraftFinishTime = DateTime.MaxValue;
        }

        public void Process(S.CraftExpChanged p)  //制作经验改变
        {
            if ((p.Exp - GameScene.Game.User.CraftExp) > 0)
            {
                GameScene.Game.ReceiveChat("获得了".Lang() + (p.Exp - GameScene.Game.User.CraftExp) + "点制造经验".Lang(), MessageType.System);
            }
            GameScene.Game.User.CraftExp = p.Exp;
            GameScene.Game.User.CraftLevel = p.Level;
        }

        public void Process(S.AchievementProgressChanged p)  //成就进度变化
        {
            //GameScene.Game.AchievementLog
            foreach (ClientUserAchievement changedAchievement in p.Achievements)
            {
                ClientUserAchievement achievement = GameScene.Game.AchievementLog.FirstOrDefault(x => x.AchievementIndex == changedAchievement.AchievementIndex);
                if (achievement == null) continue;

                achievement.Completed = changedAchievement.Completed;
                achievement.CompleteDate = changedAchievement.CompleteDate;
                achievement.Requirements.Clear();
                achievement.Requirements.AddRange(changedAchievement.Requirements);

                //GameScene.Game.AchievementBox.SelectedTabChanged(GameScene.Game.AchievementBox.ActiveTab, EventArgs.Empty);
            }
        }

        public void Process(S.AchievementTitleChanged p)   //成就称号变化
        {
            foreach (MapObject ob in GameScene.Game.MapControl.Objects.Where(ob => ob.ObjectID == p.ObjectID))
            {
                ob.AchievementTitle = p.NewTitle;

                ob.NameChanged();
                return;
            }
        }

        public void Process(S.sc_FixedPoint p)  //快捷定位传送
        {
            if (p.Opt == 0)      //新增
            {
                CEnvir.FixedPointList.Add(p.Info);
            }
            else if (p.Opt == 1) //修改
            {
                foreach (var list in CEnvir.FixedPointList)
                {
                    if (list.Uind == p.Info.Uind)
                    {
                        CEnvir.FixedPointList[CEnvir.FixedPointList.IndexOf(list)].Name = p.Info.Name;
                        CEnvir.FixedPointList[CEnvir.FixedPointList.IndexOf(list)].NameColour = p.Info.NameColour;
                        break;
                    }
                }
            }
            else if (p.Opt == 2) //删除
            {
                foreach (var list in CEnvir.FixedPointList)
                {
                    if (list.Uind == p.Info.Uind)
                    {
                        CEnvir.FixedPointList.Remove(list);
                        break;
                    }
                }
            }
            if (null != GameScene.Game.FixedPointBox)
            {
                GameScene.Game.FixedPointBox.RefreshList();
            }
        }
        public void Process(S.sc_FixedPointAdd p)  //快捷定位传送位置增加
        {
            CEnvir.FixedPointTCount = p.count;
            if (null != GameScene.Game.FixedPointBox)
            {
                GameScene.Game.FixedPointBox.RefreshList();
            }
        }

        public void Process(S.ShowConfirmationBox p) //显示二次确认窗口
        {
            new DXConfirmWindow(p.Msg, () =>
            {
                CEnvir.Enqueue(new C.NPCButton { ButtonID = p.MenuOption });
            });
        }

        public void Process(S.ShowInputBox p) //显示输入窗口
        {
            new DXInputWindow((str) =>
            {
                return !Globals.RemoveIllegalRegex.IsMatch(str);
            }, (str) =>
            {
                CEnvir.Enqueue(new C.NPCButton { ButtonID = p.MenuOption, UserInput = str });
            }, p.Msg);
        }

        public void Process(S.ShortcutsLoaded p) //通知客户端生成快捷按钮功能列表
        {
            if (p.Shortcuts.Count % 4 == 0 && GameScene.Game.ShortcutBox.Icons.Count < (p.Shortcuts.Count / 4))
            {
                for (int i = 0; i < p.Shortcuts.Count; i += 4)
                {
                    GameScene.Game.ShortcutBox.Icons.Add(new ShortcutIcon(p.Shortcuts[i], new Size(p.Shortcuts[i + 1], p.Shortcuts[i + 2]), (uint)p.Shortcuts[i + 3]));
                }
            }

            GameScene.Game.ShortcutBox.ToggleIcons(true);   //显示UI按钮图标
        }

        public void Process(S.FishingStarted p)
        {
            GameScene.Game.FishingBox.ProcessFishingStarted(p);
        }

        public void Process(S.FishingEnded p)
        {
            GameScene.Game.FishingBox.ProcessFishingEnded();
        }

        public void Process(S.UpdateNPCLook p)
        {
            if (GameScene.Game == null) return;

            CEnvir.UpdatedNPCLooks[p.ObjectID] = p;

            MapObject ob = GameScene.Game.MapControl.Objects.FirstOrDefault(x => x.ObjectID == p.ObjectID);
            NPCObject npc = (NPCObject)ob;
            if (npc == null) return;

            // 缓存更新NPC的包

            npc.UpdateNPCLook(p.NPCName, p.NameColor, p.OverlayColor, p.Library, p.ImageIndex);

            if (p.UpdateNPCIcon)
            {
                npc.UpdateNPCIcon(p.Icon);
            }
        }

        public void Process(S.ConquestWarFlagFightStarted p)
        {
            if (GameScene.Game == null) return;
            if (GameScene.Game.MapControl.MapInfo.Index == p.MapIndex)
            {
                GameScene.Game.CountdownBox.Location = new Point(GameScene.Game.Size.Width / 2 - GameScene.Game.CountdownBox.Size.Width / 2, 190);
                GameScene.Game.CountdownBox.Visible = true;
                GameScene.Game.CountdownBox.StartTime = CEnvir.Now;
                GameScene.Game.CountdownBox.EndTime = GameScene.Game.CountdownBox.StartTime + p.DelayTime;
            }
        }

        public void Process(S.ConquestWarFlagFightEnd p)
        {
            if (GameScene.Game == null) return;
            GameScene.Game.CountdownBox.Visible = false;
        }

        public void Process(S.NPCWeaponUpgrade p)  //NPC精炼
        {
            foreach (CellLinkInfo cellLinkInfo in p.items)
            {
                DXItemCell[] grid;

                switch (cellLinkInfo.GridType)
                {
                    case GridType.Inventory:
                        grid = GameScene.Game.InventoryBox.Grid.Grid;
                        break;
                    case GridType.Equipment:
                        grid = GameScene.Game.CharacterBox.Grid;
                        break;
                    case GridType.Storage:
                        grid = GameScene.Game.StorageBox.Grid.Grid;
                        break;
                    case GridType.CompanionInventory:
                        grid = GameScene.Game.CompanionBox.InventoryGrid.Grid;
                        break;
                    case GridType.CompanionEquipment:
                        grid = GameScene.Game.CompanionBox.EquipmentGrid;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(); //参数超出范围异常
                }

                DXItemCell fromCell = grid[cellLinkInfo.Slot];
                fromCell.Locked = false;

                if (!fromCell.Item.Info.ShouldLinkInfo)
                {
                    for (int i = 0; i < GameScene.Game.BeltBox.Links.Length; i++)
                    {
                        ClientBeltLink link = GameScene.Game.BeltBox.Links[i];
                        if (link.LinkItemIndex != fromCell.Item.Index) continue;

                        link.LinkItemIndex = -1;

                        if (i < GameScene.Game.BeltBox.Grid.Grid.Length)
                            GameScene.Game.BeltBox.Grid.Grid[i].QuickItem = null; //set belt to null

                        if (!GameScene.Game.Observer)
                            CEnvir.Enqueue(new C.BeltLinkChanged { Slot = link.Slot, LinkIndex = link.LinkInfoIndex, LinkItemIndex = link.LinkItemIndex }); //Update server
                    }
                }

                if (cellLinkInfo.Count == fromCell.Item.Count)
                    fromCell.Item = null;
                else
                    fromCell.Item.Count -= cellLinkInfo.Count;

                fromCell.RefreshItem();
            }
        }

        public void Process(S.PlaySound p)
        {
            DXSoundManager.Play(p.Sound);
        }

        public void Process(S.PyTextBox p)
        {
            new DXInputWindow((str) => !Globals.RemoveIllegalRegex.IsMatch(str), (str) =>
            {
                CEnvir.Enqueue(new C.PyTextBoxResponse
                {
                    ID = p.ID,
                    IsOK = true,
                    UserInput = str,
                });
            }, p.Message, () => CEnvir.Enqueue(new C.PyTextBoxResponse
            {
                ID = p.ID,
                IsOK = false,
            }));
        }

        public void Process(S.RemoveEffects p)
        {
            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
            {
                if (ob.ObjectID != p.ObjectID) continue;

                ob.RemoveEffects();
                return;
            }
        }

        public void Process(S.UpdateGuildGameTotal p)
        {
            GameScene.Game.GuildBox.GameGoldAmountLabel.Text = (p.Amount / 100).ToString();
        }

        public void Process(S.GuildWithDrawal p)
        {
            GameScene.Game.GuildBox.ReformatAppGameGoldLines(p.WithDrawals);
        }

        public void Process(S.GuildApplications p)
        {
            GameScene.Game.GuildBox.PopulateApplicationList(p.Applicants);
        }

        public void Process(S.ObjectFishing p)
        {
            foreach (MapObject ob in GameScene.Game.MapControl.Objects)
            {
                if (ob.ObjectID != p.ObjectID) continue;

                if (ob is PlayerObject player)
                {
                    player.ActionQueue.Add(new ObjectAction(p.Action, p.Direction, p.Location));
                    player.IsFishing = p.Action == MirAction.FishingCast || p.Action == MirAction.FishingWait;
                    return;
                }
            }
        }

        public void Process(S.TaishanBuffChanged p)
        {
            if (GameScene.Game?.VowBox != null)
            {
                GameScene.Game.VowBox.BuffIndex1 = p.BuffIndex1;
                GameScene.Game.VowBox.BuffIndex2 = p.BuffIndex2;
                GameScene.Game.VowBox.BuffIndex3 = p.BuffIndex3;
                GameScene.Game.VowBox.BuffIndex4 = p.BuffIndex4;
                GameScene.Game.VowBox.BuffIndex5 = p.BuffIndex5;
                GameScene.Game.VowBox.BuffIndex6 = p.BuffIndex6;
            }
        }
        public void Process(S.GoldMarketMyOrderList p)
        {
            if (GameScene.Game?.GoldTradingBusinessBox != null)
            {
                GameScene.Game.GoldTradingBusinessBox.OrderFlash(p);
            }
        }
        public void Process(S.GoldMarketList p)
        {
            if (GameScene.Game?.GoldTradingBusinessBox != null)
            {
                GameScene.Game.GoldTradingBusinessBox.TradeReFlash(p);
            }
        }
        public void Process(S.FreeCoinCountChanged p)
        {
            if (GameScene.Game?.VowBox != null)
            {
                GameScene.Game.VowBox.RemainingFreeTosses = p.Count;
            }
        }

        public void Process(S.CoinTossOnTarget p)
        {
            if (GameScene.Game?.VowBox != null)
            {
                GameScene.Game.VowBox.IsOnTarget = p.IsOnTarget;
                GameScene.Game.VowBox.Stage = VowDialog.ThrowingStage.TIntermediate;
            }
        }


        public void Process(S.NewAuctionFlash p)
        {
            if (GameScene.Game?.AuctionsBox != null)
            {
                GameScene.Game.AuctionsBox.ItemResults.Clear();
                GameScene.Game.AuctionsBox.SearchResults.Clear();
                foreach (var item in p.NewAuctionsList)
                {
                    GameScene.Game.AuctionsBox.ItemResults.Add(item);
                    GameScene.Game.AuctionsBox.SearchResults.Add(item);
                }
                GameScene.Game.AuctionsBox.RefreshList();
            }
        }

        public void Process(S.GateInformation p)
        {
            if (GameScene.Game?.TopInfoBox != null)
            {
                if (CEnvir.RespawnInfoList.Binding.Any(x => x.MonsterName == "异界之门"))
                {
                    if (p.NetherworldCloseTime <= CEnvir.Now)
                    {
                        if (p.NetherworldCloseTime != DateTime.MinValue)
                        {
                            //门已关闭
                            string lastOpenTime = (p.NetherworldCloseTime - TimeSpan.FromMinutes(20)).ToString("HH:mm");
                            GameScene.Game.TopInfoBox.NetherworldGateLabel.Text = $"异界之门已关闭, 最近开启时间 {lastOpenTime}";
                        }
                    }
                    else
                    {
                        //门开启中
                        string closeTime = p.NetherworldCloseTime.ToString("HH:mm");
                        GameScene.Game.TopInfoBox.NetherworldGateLabel.Text = $"异界之门已开启, 坐标({p.NetherworldLocation.X}:{p.NetherworldLocation.Y}), 关闭时间 {closeTime}";
                    }
                }

                if (CEnvir.RespawnInfoList.Binding.Any(x => x.MonsterName == "赤龙石门"))
                {
                    if (p.LairCloseTime <= CEnvir.Now)
                    {
                        if (p.LairCloseTime != DateTime.MinValue)
                        {
                            //门已关闭
                            string lastOpenTime = (p.LairCloseTime - TimeSpan.FromMinutes(20)).ToString("HH:mm");
                            GameScene.Game.TopInfoBox.JinamStoneGateLabel.Text = $"赤龙石门已关闭, 最近开启时间 {lastOpenTime}";
                        }
                    }
                    else
                    {
                        //门开启中
                        string closeTime = p.NetherworldCloseTime.ToString("HH:mm");
                        GameScene.Game.TopInfoBox.JinamStoneGateLabel.Text = $"赤龙石门已开启, 坐标({p.LairLocation.X}:{p.LairLocation.Y}), 关闭时间 {closeTime}";
                    }
                }
            }
        }

        public void Process(S.AccessoryCombineResult p)
        {
            GameScene.Game.NPCAccessoryCombineBox.MainJewelry.ClearLinks();
            GameScene.Game.NPCAccessoryCombineBox.AccessoryJewelry1.ClearLinks();
            GameScene.Game.NPCAccessoryCombineBox.AccessoryJewelry2.ClearLinks();
            GameScene.Game.NPCAccessoryCombineBox.Corundum.ClearLinks();
            GameScene.Game.NPCAccessoryCombineBox.Crystal.ClearLinks();
        }

        public void Process(S.MapMagicRestriction p)
        {
            if (p.ClearAll)
            {
                CEnvir.MapMagicRestrictions.Clear();
                return;
            }

            if (!CEnvir.MapMagicRestrictions.ContainsKey(p.MapName))
            {
                CEnvir.MapMagicRestrictions.Add(p.MapName, new HashSet<MagicType>());
            }

            foreach (var magic in p.Magics)
            {
                CEnvir.MapMagicRestrictions[p.MapName].Add(magic);
            }
        }

        /// <summary>
        /// 奖金池信息更新
        /// </summary>
        /// <param name="p"></param>
        public void Process(S.RewardPoolUpdate p)
        {
            if (GameScene.Game.BonusPoolBox != null)
            {
                GameScene.Game.BonusPoolBox.CurrentPoolInfo = p.RewardPoolInfo;
            }
        }
        /// <summary>
        /// 个人的奖池币发生变化
        /// </summary>
        /// <param name="p"></param>
        public void Process(S.RewardPoolCoinChanged p)
        {
            GameScene.Game.User.RewardPoolCoin = p.RewardPoolCoin;
        }
        /// <summary>
        /// 奖池币排名发生变化
        /// </summary>
        /// <param name="p"></param>
        public void Process(S.RewardPoolCoinRankChanged p)
        {
            if (GameScene.Game.BonusPoolVersionBox == null) return;

            GameScene.Game.BonusPoolVersionBox.First = p.First;
            GameScene.Game.BonusPoolVersionBox.Second = p.Second;
            GameScene.Game.BonusPoolVersionBox.Third = p.Third;
            GameScene.Game.BonusPoolVersionBox.Myself = p.Myself;

            GameScene.Game.BonusPoolVersionBox.RefreshRanks();
        }

        /// <summary>
        /// 更新单个红包的信息
        /// </summary>
        /// <param name="p"></param>
        public void Process(S.RedPacketUpdate p)
        {
            if (GameScene.Game.BonusPoolVersionBox == null) return;
            GameScene.Game.BonusPoolVersionBox.UpdateSingleRedPacket(p.RedPacket);
        }

        /// <summary>
        /// 最近24小时内的红包
        /// </summary>
        /// <param name="p"></param>
        public void Process(S.RecentRedPackets p)
        {
            if (GameScene.Game.BonusPoolVersionBox == null) return;

            GameScene.Game.BonusPoolVersionBox.UpdateRedpacketsList(p.RedPacketList);
        }
        public void Process(S.HuiShengToggle p)
        {
            GameScene.Game.ConfigBox.AllowHuiShengButton.Index = p.HuiSheng ? 1292 : 1295;
        }

        public void Process(S.ReCallToggle p)
        {
            GameScene.Game.ConfigBox.AllowRecallButton.Index = p.Recall ? 1292 : 1295;
        }

        public void Process(S.TradeToggle p)
        {
            GameScene.Game.ConfigBox.AllowTradeButton.Index = p.Trade ? 1292 : 1295;
        }

        public void Process(S.GuildToggle p)
        {
            GameScene.Game.ConfigBox.AllowGuildButton.Index = p.Guild ? 1292 : 1295;
        }

        public void Process(S.FriendToggle p)
        {
            GameScene.Game.ConfigBox.AllowFriendButton.Index = p.Friend ? 1292 : 1295;
        }

        /// <summary>
        /// 更改天气
        /// </summary>
        /// <param name="p"></param>
        public void Process(S.ChangeWeather p)
        {
            CEnvir.WeatherOverrides[p.MapIndex] = p.Weather;
            if (GameScene.Game?.MapControl?.MapInfo?.Index == p.MapIndex)
            {
                GameScene.Game.MapControl.UpdateWeather();
            }
        }
        public void Process(S.AmmAmmunition p)
        {
            if (p.Success)
            {
                GameScene.Game.WarWeaponBox.PaodanCount = p.Count;
                GameScene.Game.WarWeaponBox.SurplusAmmunition.Text = p.Count.ToString();
            }
        }

        public void Process(S.WarWeapLocation p)
        {
            if (p.ObjectID == GameScene.Game.WarWeaponID)
            {
                GameScene.Game.WarWeaponBox.Map.UpdatePetLoaction(p.Location);
            }
        }

        public void Process(S.RequestProcessHash p)
        {
            Enqueue(new C.ResponseProcessHash
            {
                HashList = AntiCheat.ComputeAllHashes(),
                DateTime = CEnvir.Now,
            });
        }

        public void Process(S.ReFlashSellCharState p)
        {
            for (int i = 0; i < (DXControl.ActiveScene as SelectScene).SelectBox.CharacterList.Count; i++)
            {
                if ((DXControl.ActiveScene as SelectScene).SelectBox.CharacterList[i].CharacterIndex == p.CharacterIndex)
                    (DXControl.ActiveScene as SelectScene).SelectBox.CharacterList[i].CharacterState = (int)CharacterState.Normal;
            }
            if ((DXControl.ActiveScene as SelectScene).SelectBox.CurrSelCharacter != null && (DXControl.ActiveScene as SelectScene).SelectBox.CurrSelCharacter.CharacterIndex == p.CharacterIndex)
            {
                (DXControl.ActiveScene as SelectScene).SelectBox.CancelButton.Visible = false;
                (DXControl.ActiveScene as SelectScene).SelectBox.ConsignmentLabel.Visible = false;
            }
        }


        public void Process(S.SellCharacterSearch p)
        {
            GameScene.Game.AccountConsignmentBox.SearchResults = new ClientAccountConsignmentInfo[p.Count];

            for (int i = 0; i < p.Results.Count; i++)
                GameScene.Game.AccountConsignmentBox.SearchResults[i] = p.Results[i];
            GameScene.Game.AccountConsignmentBox.TotalPage = (GameScene.Game.AccountConsignmentBox.SearchResults.Count() - 1) / 13 + 1;
            GameScene.Game.AccountConsignmentBox.CurrentPage = 1;
            GameScene.Game.AccountConsignmentBox.RefreshConsignList();
        }

        public void Process(S.InspectPackSack p)
        {
            GameScene.Game.InventoryJueSeBox.NewInformation(p);
        }

        public void Process(S.InspectMagery p)
        {
            GameScene.Game.MagicJueSeBox.NewInformation(p);
        }

        public void Process(S.MarketPlaceJiaoseBuy p)
        {
            GameScene.Game.AccountConsignmentBox.RefreshConsignList();
        }
    }
}

