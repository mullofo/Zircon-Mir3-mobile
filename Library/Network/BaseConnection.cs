using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Reflection;
using G = Library.Network.GeneralPackets;


namespace Library.Network
{
    /// <summary>
    /// 关联
    /// </summary>
    public abstract class BaseConnection
    {
        /// <summary>
        /// 诊断值
        /// </summary>
        public static Dictionary<string, DiagnosticValue> Diagnostics = new Dictionary<string, DiagnosticValue>();
        /// <summary>
        /// 数据包方法
        /// </summary>
        public static Dictionary<Type, MethodInfo> PacketMethods = new Dictionary<Type, MethodInfo>();
        /// <summary>
        /// 效验检查
        /// </summary>
        public static bool Monitor;
        /// <summary>
        /// 有联系的
        /// </summary>
        public bool Connected { get; set; }
        /// <summary>
        /// 发送
        /// </summary>
        protected bool Sending { get; set; }
        /// <summary>
        /// 总字节数
        /// </summary>
        public int TotalBytesSent { get; set; }
        /// <summary>
        /// 接受的总字节数
        /// </summary>
        public int TotalBytesReceived { get; set; }
        /// <summary>
        /// 附加日志记录
        /// </summary>
        public bool AdditionalLogging;
        /// <summary>
        /// 客户端TCP协议
        /// </summary>
        protected TcpClient Client;
        /// <summary>
        /// 连接时间
        /// </summary>
        public DateTime TimeConnected { get; set; }
        /// <summary>
        /// 持续时间=系统时间-连接时间
        /// </summary>
        public TimeSpan Duration => Time.Now - TimeConnected;
        /// <summary>
        /// 超时延迟
        /// </summary>
        protected abstract TimeSpan TimeOutDelay { get; }
        /// <summary>
        /// 超时时间
        /// </summary>
        public DateTime TimeOutTime { get; set; }
        /// <summary>
        /// 断开
        /// </summary>
        private bool _disconnecting;
        /// <summary>
        /// 断开延迟
        /// </summary>
        public bool Disconnecting
        {
            get { return _disconnecting; }
            set
            {
                if (_disconnecting == value) return;
                _disconnecting = value;
                TimeOutTime = Time.Now.AddSeconds(5);
            }
        }
        /// <summary>
        /// 接受数据包列表
        /// </summary>
        public ConcurrentQueue<Packet> ReceiveList = new ConcurrentQueue<Packet>();
        /// <summary>
        /// 发送数据包列表
        /// </summary>
        public ConcurrentQueue<Packet> SendList = new ConcurrentQueue<Packet>();
        /// <summary>
        /// 原始数据
        /// </summary>
        public byte[] _rawData = new byte[0];
        /// <summary>
        /// 异常时的事件处理
        /// </summary>
        public EventHandler<Exception> OnException;
        /// <summary>
        /// 异常时的事件处理输出
        /// </summary>
        public EventHandler<string> Output;
        protected BaseConnection(TcpClient client)
        {
            Client = client;
            Client.NoDelay = true;


            Connected = true;
            TimeConnected = Time.Now;
        }
        /// <summary>
        /// 获取接收列表队列长度
        /// </summary>
        /// <returns></returns>
        public int GetReceiveListQueueLength()
        {
            if (ReceiveList != null)
                return ReceiveList.Count;
            return 0;
        }
        /// <summary>
        /// 获取发送列表队列长度
        /// </summary>
        /// <returns></returns>
        public int GetSendListQueueLength()
        {
            if (SendList != null)
                return SendList.Count;
            return 0;
        }
        /// <summary>
        /// 开始接收
        /// </summary>
        protected void BeginReceive()
        {
            try
            {
                if (Client == null || !Client.Connected) return;

                byte[] rawBytes = new byte[8 * 1024];

                Client.Client.BeginReceive(rawBytes, 0, rawBytes.Length, SocketFlags.None, ReceiveData, rawBytes);
            }
            catch (Exception ex)
            {
                if (AdditionalLogging)
                    OnException(this, ex);
                Disconnecting = true;
            }
        }
        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="result"></param>
        private void ReceiveData(IAsyncResult result)
        {
            try
            {
                if (!Connected) return;

                int dataRead = Client.Client.EndReceive(result);

                if (dataRead == 0)
                {
                    Disconnecting = true;
                    return;
                }
                TotalBytesReceived += dataRead;

                UpdateTimeOut();

                byte[] rawBytes = result.AsyncState as byte[];

                byte[] temp = _rawData;
                _rawData = new byte[dataRead + temp.Length];
                Buffer.BlockCopy(temp, 0, _rawData, 0, temp.Length);
                Buffer.BlockCopy(rawBytes, 0, _rawData, temp.Length, dataRead);

                //封包处理单独拆出来
                ReceivePacket();

                BeginReceive();
            }
            catch (Exception ex)
            {
                if (AdditionalLogging)
                    OnException(this, ex);
                Disconnecting = true;
            }
        }

        public virtual void ReceivePacket()
        {
            Packet p;

            while ((p = Packet.ReceivePacket(_rawData, out _rawData)) != null)
                ReceiveList.Enqueue(p);
        }
        /// <summary>
        /// 开始发送
        /// </summary>
        /// <param name="data"></param>
        private void BeginSend(List<byte> data)
        {
            if (!Connected || data.Count == 0) return;

            try
            {
                Sending = true;
                TotalBytesSent += data.Count;
                Client.Client.BeginSend(data.ToArray(), 0, data.Count, SocketFlags.None, SendData, null);
                UpdateTimeOut();
            }
            catch (Exception ex)
            {
                if (AdditionalLogging)
                    OnException(this, ex);
                Disconnecting = true;
                Sending = false;
            }
        }
        /// <summary>
        /// 发送日期
        /// </summary>
        /// <param name="result"></param>
        private void SendData(IAsyncResult result)
        {
            try
            {
                Sending = false;
                Client.Client.EndSend(result);
                UpdateTimeOut();
            }
            catch (Exception ex)
            {
                if (AdditionalLogging)
                    OnException(this, ex);
                Disconnecting = true;
            }
        }
        /// <summary>
        /// 队列
        /// </summary>
        /// <param name="p"></param>
        public virtual void Enqueue(Packet p)
        {
            if (!Connected || p == null) return;

            SendList.Enqueue(p);
        }
        /// <summary>
        /// 尝试断开连接
        /// </summary>
        public abstract void TryDisconnect();
        /// <summary>
        /// 断开
        /// </summary>
        public virtual void Disconnect()
        {
            if (!Connected) return;

            Connected = false;

            SendList = null;
            ReceiveList = null;
            _rawData = null;

            Client.Client.Dispose();
            Client = null;
        }
        /// <summary>
        /// 尝试发送断开连接
        /// </summary>
        /// <param name="p"></param>
        public abstract void TrySendDisconnect(Packet p);
        /// <summary>
        /// 发送断开链接
        /// </summary>
        /// <param name="p"></param>
        public virtual void SendDisconnect(Packet p)
        {
            if (!Connected || Disconnecting)
            {
                Disconnecting = true;
                return;
            }

            List<byte> data = new List<byte>();

            data.AddRange(p.GetPacketBytes());

            BeginSendDisconnect(data);
        }
        /// <summary>
        /// 开始断开链接
        /// </summary>
        /// <param name="data"></param>
        private void BeginSendDisconnect(List<byte> data)
        {
            if (!Connected || data.Count == 0) return;

            if (Disconnecting) return;

            try
            {
                Disconnecting = true;

                TotalBytesSent += data.Count;
                Client.Client.BeginSend(data.ToArray(), 0, data.Count, SocketFlags.None, SendDataDisconnect, null);
            }
            catch (Exception ex)
            {
                if (AdditionalLogging)
                    OnException(this, ex);
            }
        }
        /// <summary>
        /// 发送数据断开连接
        /// </summary>
        /// <param name="result"></param>
        private void SendDataDisconnect(IAsyncResult result)
        {
            try
            {
                Client.Client.EndSend(result);
            }
            catch (Exception ex)
            {
                if (AdditionalLogging)
                    OnException(this, ex);
            }
        }
        /// <summary>
        /// 过程
        /// </summary>
        public virtual void Process()
        {
            if (Client == null || !Client.Connected)
            {
                TryDisconnect();
                return;
            }

            while (ReceiveList != null && !ReceiveList.IsEmpty && !Disconnecting)
            {
                Packet p = null;
                try
                {
                    if (!ReceiveList.TryDequeue(out p)) continue;

                    ProcessPacket(p);
                }
                catch (NotImplementedException ex)
                {
                    Output?.Invoke(p, $"Process->ProcessPacket {p?.GetType().Name} catch NotImplementedException");
                    OnException(this, ex);
                }
                catch (Exception ex)
                {
                    Output?.Invoke(p, $"Process->ProcessPacket {p?.GetType().Name} catch Exception");
                    OnException(this, ex);
                    //throw ex;
                }
            }

            if (Time.Now >= TimeOutTime)
            {
                if (!Disconnecting)
                    TrySendDisconnect(new G.Disconnect { Reason = DisconnectReason.TimedOut });
                else
                    TryDisconnect();

                return;
            }

            if (!Disconnecting && Sending)
                UpdateTimeOut();

            if (SendList == null || SendList.IsEmpty || Sending) return;

            List<byte> data = new List<byte>();
            while (!SendList.IsEmpty)
            {
                Packet p = null;

                if (!SendList.TryDequeue(out p)) continue;

                if (p == null) continue;

                try
                {
                    byte[] bytes = p.GetPacketBytes();

                    data.AddRange(bytes);
                }
                catch (Exception ex)
                {
                    Output?.Invoke(p, $"Process->GetPacketBytes ->AddRange  {p?.GetType().Name} catch Exception");
                    OnException?.Invoke(this, ex);
                    Disconnecting = true;
                    return;
                }

                if (!Monitor) continue;

                DiagnosticValue value;
                Type type = p.GetType();

                if (!Diagnostics.TryGetValue(type.FullName, out value))
                    Diagnostics[type.FullName] = value = new DiagnosticValue { Name = type.FullName };

                value.Count++;
                value.TotalSize += p.Length;

                if (p.Length > value.LargestSize)
                    value.LargestSize = p.Length;
            }

            BeginSend(data);
        }
        /// <summary>
        /// 处理封包
        /// </summary>
        /// <param name="p"></param>
        private void ProcessPacket(Packet p)
        {
            if (p == null) return;

            DateTime start = Time.Now;

            MethodInfo info;
            if (!PacketMethods.TryGetValue(p.PacketType, out info))
                PacketMethods[p.PacketType] = info = GetType().GetMethod("Process", new[] { p.PacketType });

            if (info == null)
            {
                Output?.Invoke(p, $"未执行异常: 方式过程({p.PacketType}).");
                return;
            }
            try
            {
                info.Invoke(this, new object[] { p });
            }
            catch
            {
                //todo 保存错误日志
                Output?.Invoke(p, $"封包发生错误： {p?.GetType().Name}");
            }
            Monitor = true;
            if (!Monitor) return;

            TimeSpan execution = Time.Now - start;
            DiagnosticValue value;

            if (!Diagnostics.TryGetValue(p.PacketType.FullName, out value))
                Diagnostics[p.PacketType.FullName] = value = new DiagnosticValue { Name = p.PacketType.FullName };

            value.Count++;
            value.TotalTime += execution;
            value.TotalSize += p.Length;

            if (execution > value.LargestTime)
                value.LargestTime = execution;

            if (p.Length > value.LargestSize)
                value.LargestSize = p.Length;
        }
        /// <summary>
        /// 更新超时
        /// </summary>
        public void UpdateTimeOut()
        {
            if (Disconnecting) return;

            TimeOutTime = Time.Now + TimeOutDelay;
        }
    }

    /// <summary>
    /// 诊断值
    /// </summary>
    public class DiagnosticValue
    {
        /// <summary>
        /// 名字
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 总时间
        /// </summary>
        public TimeSpan TotalTime { get; set; }
        /// <summary>
        /// 最终时间
        /// </summary>
        public TimeSpan LargestTime { get; set; }
        /// <summary>
        /// 计数
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// 总大小
        /// </summary>
        public long TotalSize { get; set; }
        /// <summary>
        /// 最后大小
        /// </summary>
        public long LargestSize { get; set; }
        /// <summary>
        /// 总滴答数
        /// </summary>
        public long TotalTicks => TotalTime.Ticks;
        /// <summary>
        /// 总计毫秒
        /// </summary>
        public long TotalMilliseconds => TotalTicks / TimeSpan.TicksPerMillisecond;
        /// <summary>
        /// 最后地大叔
        /// </summary>
        public long LargestTicks => LargestTime.Ticks;
        /// <summary>
        /// 最后计数毫秒
        /// </summary>
        public long LargestMilliseconds => LargestTicks / TimeSpan.TicksPerMillisecond;
    }
}
