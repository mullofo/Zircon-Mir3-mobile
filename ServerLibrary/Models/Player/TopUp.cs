using IronPython.Runtime;
using IronPython.Runtime.Exceptions;
using IronPython.Runtime.Operations;
using Library;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using Server.Envir;
using System;


namespace Server.Models
{
    public partial class PlayerObject : MapObject //玩家充值
    {
        private DateTime NextCheckTopUpTime = SEnvir.Now;
        public void TopUp() //点击“领取元宝”后 处理充值
        {
            if (SEnvir.Now >= NextCheckTopUpTime)
            {
                NextCheckTopUpTime = NextCheckTopUpTime.AddSeconds(10);

                #region 充值py事件

                try
                {
                    dynamic trig_play;
                    if (SEnvir.PythonEvent.TryGetValue("PlayerEvent_trig_player", out trig_play))
                    {
                        PythonTuple args = PythonOps.MakeTuple(new object[] { this, });
                        bool? res = SEnvir.ExecutePyWithTimer(trig_play, this, "OnCollectGameGoldClicked", args);
                        // bool? res = trig_play(this, "OnCollectGameGoldClicked", args);
                    }

                }
                catch (SyntaxErrorException e)
                {

                    string msg = "延迟调用语法错误 : \"{0}\"";
                    ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                    string errorMsg = eo.FormatException(e);
                    SEnvir.Log(string.Format(msg, errorMsg));
                }
                catch (SystemExitException e)
                {

                    string msg = "延迟系统退出 : \"{0}\"";
                    ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                    string errorMsg = eo.FormatException(e);
                    SEnvir.Log(string.Format(msg, errorMsg));
                }
                catch (Exception ex)
                {

                    string msg = "加载插件时出现延迟调用错误 : \"{0}\"";
                    ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                    string errorMsg = eo.FormatException(ex);
                    SEnvir.Log(string.Format(msg, errorMsg));
                }

                #endregion

                Connection.ReceiveChat("如果您有充值，已经领取完毕了".Lang(Connection.Language), MessageType.System);
            }
            else
            {
                Connection.ReceiveChat("请稍后再点击".Lang(Connection.Language), MessageType.System);
            }
        }
    }
}
