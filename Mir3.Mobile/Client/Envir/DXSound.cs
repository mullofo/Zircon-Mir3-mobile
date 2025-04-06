using Client.Helpers;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Client.Envir
{
    /// <summary>
    /// 声音控件
    /// </summary>
    public sealed class DXSound
    {
        private SoundEffect SoundEffect;
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 声音缓冲区
        /// </summary>
        public List<SoundEffectInstance> BufferList = new List<SoundEffectInstance>();
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
        public float Volume { get; set; }

        private byte SoundFileStat { get; set; }
        private bool Stoped { get; set; }

        public DXSound(string fileName, SoundType type)
        {
            FileName = fileName;
            SoundType = type;

            Volume = DXSoundManager.GetVolume(SoundType) / 100f;
        }
        /// <summary>
        /// 播放
        /// </summary>
        public void Play()
        {
            Stoped = false;

            if (SoundEffect == null)
            {
                if (SoundFileStat == 1 || SoundFileStat == 2) return;

                if (!File.Exists(CEnvir.MobileClientPath + FileName) && SoundFileStat == 0 && LibraryHelper.MicroServerActive)
                {
                    //限制并发量
                    if (LibraryHelper.Semaphore.CurrentCount > 0)
                    {
                        LibraryHelper.Semaphore.WaitAsync();
                        SoundFileStat = 1;
                        Task.Run(async () =>
                        {
                            try
                            {
                                if (SoundFileStat != 1) return;

                                SoundFileStat = 2;
                                bool returned = await LibraryHelper.GetSound(FileName);
                                if (returned)
                                {
                                    SoundFileStat = 3;
                                    if (Loop && !Stoped) Play();
                                }
                                else
                                    SoundFileStat = 0;
                            }
                            finally
                            {
                                LibraryHelper.Semaphore.Release();
                            }
                        });
                    }
                    return;
                }

                if (!File.Exists(CEnvir.MobileClientPath + FileName)) return;
                SoundEffect = SoundEffect.FromFile(CEnvir.MobileClientPath + FileName);
                DXManager.SoundList.Add(this);
            }

            if (BufferList.Count == 0)
                CreateBuffer();

            if (Loop)
            {
                if (BufferList[0].State != SoundState.Playing)
                {
                    try
                    {
                        BufferList[0].Play();
                    }
                    catch
                    {
                        BufferList[0].Stop();
                        SoundEffectInstance soundEffectInstance = CreateBuffer();
                        try
                        {
                            soundEffectInstance.Play();
                        }
                        catch
                        {
                            soundEffectInstance.Stop();
                        }
                    }
                }
                BufferList[0].IsLooped = true;
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

                if (BufferList[i].State == SoundState.Playing) continue;

                try
                {
                    BufferList[i].Play();
                    return;
                }
                catch
                {
                    BufferList[i].Stop();
                    SoundEffectInstance soundEffectInstance2 = CreateBuffer();
                    try
                    {
                        soundEffectInstance2.Play();
                        return;
                    }
                    catch
                    {
                        soundEffectInstance2.Stop();
                        return;
                    }
                }
            }

            if (BufferList.Count >= Config.SoundOverLap) return;

            SoundEffectInstance soundEffectInstance3 = CreateBuffer();
            try
            {
                soundEffectInstance3.Play();
            }
            catch
            {
                soundEffectInstance3.Stop();
            }
        }
        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            Stoped = true;

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
                BufferList[i].Stop();
            }
        }
        /// <summary>
        /// 创建缓冲区
        /// </summary>
        /// <returns></returns>
        private SoundEffectInstance CreateBuffer()
        {
            SoundEffectInstance soundEffectInstance = SoundEffect.CreateInstance();
            soundEffectInstance.Volume = Volume;
            BufferList.Add(soundEffectInstance);
            return soundEffectInstance;
        }
        /// <summary>
        /// 处理声音缓冲
        /// </summary>
        public void DisposeSoundBuffer()
        {
            SoundEffect.Dispose();
            SoundEffect = null;

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
            Volume = DXSoundManager.GetVolume(SoundType) / 100f;

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
    }
}
