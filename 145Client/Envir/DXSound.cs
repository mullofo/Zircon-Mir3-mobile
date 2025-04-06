using SharpDX.DirectSound;
using SharpDX.Multimedia;
using System;
using System.Collections.Generic;
using System.IO;

namespace Client.Envir
{
    /// <summary>
    /// 声音控件
    /// </summary>
    public sealed class DXSound
    {
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 声音缓冲区
        /// </summary>
        public List<SecondarySoundBuffer> BufferList = new List<SecondarySoundBuffer>();
        /// <summary>
        /// 波形格式
        /// </summary>
        private WaveFormat Format;
        /// <summary>
        /// 原始数据
        /// </summary>
        private byte[] RawData;
        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime ExpireTime { get; set; }
        /// <summary>
        /// 是否循环
        /// </summary>
        public bool Loop { get; set; }
        /// <summary>
        /// 声音类型
        /// </summary>
        public SoundType SoundType { get; set; }
        /// <summary>
        /// 音量
        /// </summary>
        public int Volume { get; set; }

        public DXSound(string fileName, SoundType type)
        {
            FileName = fileName;
            SoundType = type;

            Volume = DXSoundManager.GetVolume(SoundType);
        }
        /// <summary>
        /// 播放
        /// </summary>
        public void Play()
        {
            if (RawData == null)
            {
                if (!File.Exists(FileName)) return;
                var stream = File.OpenRead(FileName);
                using (SoundStream wStream = new SoundStream(stream))  //音流
                {
                    Format = wStream.Format;
                    RawData = new byte[wStream.Length];

                    wStream.Position = 44;
                    wStream.Read(RawData, 0, RawData.Length);
                }
                DXManager.SoundList.Add(this);
                stream.Dispose();
            }

            if (BufferList.Count == 0)
                CreateBuffer();

            if (Loop)
            {
                if (((BufferStatus)BufferList[0].Status & BufferStatus.Playing) != BufferStatus.Playing) BufferList[0].Play(0, PlayFlags.Looping);
                ExpireTime = DateTime.MaxValue;
                return;
            }
            ExpireTime = CEnvir.Now + Config.CacheDuration;

            for (int i = BufferList.Count - 1; i >= 0; i--)
            {
                if (BufferList[i].IsDisposed)
                {
                    BufferList.RemoveAt(i);
                    continue;
                }

                if ((BufferStatus)BufferList[i].Status == BufferStatus.Playing) continue;

                BufferList[i].Play(0, PlayFlags.None);
                return;
            }

            if (BufferList.Count >= Config.SoundOverLap) return;

            SecondarySoundBuffer buff = CreateBuffer();
            buff.Play(0, PlayFlags.None);
        }
        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            if (BufferList == null) return;

            if (Loop)
                ExpireTime = CEnvir.Now + Config.CacheDuration;

            for (int i = BufferList.Count - 1; i >= 0; i--)
            {
                if (BufferList[i].IsDisposed)
                {
                    BufferList.RemoveAt(i);
                    continue;
                }
                BufferList[i].CurrentPosition = 0;
                BufferList[i].Stop();
            }
        }
        /// <summary>
        /// 创建缓冲区
        /// </summary>
        /// <returns></returns>
        private SecondarySoundBuffer CreateBuffer()
        {
            SecondarySoundBuffer buff;
            BufferFlags flags = BufferFlags.ControlVolume;

            if (Config.SoundInBackground)
                flags |= BufferFlags.GlobalFocus;

            BufferList.Add(buff = new SecondarySoundBuffer(DXSoundManager.Device, new SoundBufferDescription { Format = Format, BufferBytes = RawData.Length, Flags = flags })
            {
                Volume = Volume
            });

            buff.Write(RawData, 0, LockFlags.EntireBuffer);

            return buff;
        }
        /// <summary>
        /// 处理声音缓冲
        /// </summary>
        public void DisposeSoundBuffer()
        {
            RawData = null;

            for (int i = BufferList.Count - 1; i >= 0; i--)
            {
                if (!BufferList[i].IsDisposed)
                    BufferList[i].Dispose();

                BufferList.RemoveAt(i);
            }

            DXManager.SoundList.Remove(this);
            ExpireTime = DateTime.MinValue;
        }
        /// <summary>
        /// 设置音量
        /// </summary>
        public void SetVolume()
        {
            Volume = DXSoundManager.GetVolume(SoundType);

            for (int i = BufferList.Count - 1; i >= 0; i--)
            {
                if (BufferList[i].IsDisposed)
                {
                    BufferList.RemoveAt(i);
                    continue;
                }

                BufferList[i].Volume = Volume;
            }
        }
        /// <summary>
        /// 更新标志
        /// </summary>
        public void UpdateFlags()
        {
            for (int i = BufferList.Count - 1; i >= 0; i--)
            {
                SecondarySoundBuffer buffer = CreateBuffer();

                BufferList[0].GetCurrentPosition(out var play, out var write);
                buffer.CurrentPosition = play;

                if (((BufferStatus)BufferList[0].Status & BufferStatus.Playing) == BufferStatus.Playing)
                    buffer.Play(0, Loop ? PlayFlags.Looping : PlayFlags.None);

                if (!BufferList[0].IsDisposed)
                    BufferList[0].Dispose();

                BufferList.RemoveAt(0);
            }
        }
    }
}
