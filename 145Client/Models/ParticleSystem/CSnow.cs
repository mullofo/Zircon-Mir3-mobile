using Client.Envir;
using Library;
using SharpDX;
using Color = System.Drawing.Color;

namespace Client.Models
{
    public class CSnow : CParticleSystem
    {
        private float m_fWidth;

        private float m_fGround;

        private short m_shPartNum;

        public CSnow()
        {
            InitSystem();
        }

        ~CSnow()
        {
            DestroySystem();
        }

        public override void InitSystem()
        {
            base.InitSystem();
            m_fWidth = m_fGround = 0.0f;
            m_shPartNum = 0;
        }

        public override void DestroySystem()
        {
            base.DestroySystem();
            InitSystem();
        }

        public void SetupSystem(int wCnt = 500, float fWidth = 1024f, float fGround = 768f)
        {
            InitSystem();
            base.SetupSystem(wCnt);
            m_fWidth = fWidth;
            m_fGround = fGround;
            SetEnvFactor(-0.05f, new Vector3(10f, 100f, 0f), new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f));
            CEnvir.LibraryList.TryGetValue(LibraryFile.ProgUse, out Library);
            BlendRate = 1f;
        }

        public override void UpdateSystem(int nLoopTime, Vector3 vecGenPos)
        {
            //System.Diagnostics.Debug.WriteLine("looptime = " + nLoopTime);
            int nGenCnt = 0;
            int nPartCnt = 0;
            int nSpeedRate = nLoopTime / 17;
            if (nSpeedRate < 1)
            {
                nSpeedRate = 1;
            }
            m_fDeltaTime = 0.02f * nSpeedRate;
            for (int nCnt = 0; nCnt < m_nNum; nCnt++)
            {
                if (m_pxParticle[nCnt].m_bIsDead && nGenCnt < 5)
                {
                    SetParticleDefault(nCnt, new Vector3(0f, 0f, 0f));
                    m_shPartNum++;
                    nGenCnt++;
                }
                if (!m_pxParticle[nCnt].m_bIsDead)
                {
                    m_pxParticle[nCnt].m_nCurrLife += nLoopTime;
                    if (m_pxParticle[nCnt].m_nCurrLife > m_pxParticle[nCnt].m_nLife || m_pxParticle[nCnt].m_vecPos.Y >= m_fGround)
                    {
                        if (m_pxParticle[nCnt].m_nCurrLife - m_pxParticle[nCnt].m_nLife > 255)
                        {
                            m_pxParticle[nCnt].Init();
                            m_shPartNum--;
                            nPartCnt--;
                        }
                        else
                        {
                            byte bRate = (byte)(255 - (m_pxParticle[nCnt].m_nCurrLife - m_pxParticle[nCnt].m_nLife));
                            m_pxParticle[nCnt].m_bOpa = bRate;
                            if (bRate < m_pxParticle[nCnt].m_bRed)
                            {
                                m_pxParticle[nCnt].m_bRed = bRate;
                            }
                            if (bRate < m_pxParticle[nCnt].m_bGreen)
                            {
                                m_pxParticle[nCnt].m_bGreen = bRate;
                            }
                            if (bRate < m_pxParticle[nCnt].m_bBlue)
                            {
                                m_pxParticle[nCnt].m_bBlue = bRate;
                            }
                            nPartCnt++;
                        }
                        continue;
                    }
                    UpdateAirFiction(nCnt);
                    UpdateMove(nCnt);
                    m_pxParticle[nCnt].m_nCurrDelay += nLoopTime;
                    if (m_pxParticle[nCnt].m_nCurrDelay > m_pxParticle[nCnt].m_nDelay)
                    {
                        m_pxParticle[nCnt].m_nCurrDelay = 0;
                        m_pxParticle[nCnt].m_nCurrFrame++;
                        if (m_pxParticle[nCnt].m_nCurrFrame >= 1)
                        {
                            m_pxParticle[nCnt].m_nCurrFrame = 0;
                        }
                    }
                    nPartCnt++;
                }
                if (nPartCnt >= m_shPartNum)
                {
                    break;
                }
            }
        }

        public bool RenderSystem()
        {
            int nPartCnt = 0;
            for (int nCnt = 0; nCnt < m_nNum; nCnt++)
            {
                if (!m_pxParticle[nCnt].m_bIsDead)
                {
                    //Library.DrawBlend(500, m_pxParticle[nCnt].m_vecPos.X, m_pxParticle[nCnt].m_vecPos.Y, Color.FromArgb(m_pxParticle[nCnt].m_bRed, m_pxParticle[nCnt].m_bGreen, m_pxParticle[nCnt].m_bBlue), m_pxParticle[nCnt].m_fSize, m_pxParticle[nCnt].m_fSize, BlendRate, ImageType.Image, BlendState._BLEND_LIGHT);
                    //System.Diagnostics.Debug.WriteLine("X = " + m_pxParticle[i].m_vecPos.X + ", Y = " + m_pxParticle[i].m_vecPos.Y);
                    Library.DrawBlend(500, m_pxParticle[nCnt].m_vecPos.X + m_pxParticle[nCnt].m_fSize / 3f, m_pxParticle[nCnt].m_vecPos.Y + m_pxParticle[nCnt].m_fSize / 3f, Color.FromArgb(m_pxParticle[nCnt].m_bRed, m_pxParticle[nCnt].m_bGreen, m_pxParticle[nCnt].m_bBlue), m_pxParticle[nCnt].m_fSize, m_pxParticle[nCnt].m_fSize, BlendRate, ImageType.Image, BlendType._BLEND_LIGHT);
                    nPartCnt++;
                }
                if (nPartCnt >= m_shPartNum)
                {
                    return true;
                }
            }
            return true;
        }

        public override void SetParticleDefault(int nNum, Vector3 vecGenPos)
        {
            //m_pxParticle[nNum].m_vecPos = new Vector3(GetRandomNum(0, (int)m_fWidth), GetRandomNum(-300, 0), 0f);
            m_pxParticle[nNum].m_vecPos = new Vector3(GetRandomNum(0, (int)m_fWidth), GetRandomNum(-(int)(m_fGround / 2), 0), 0f); //雪的绘制区域（必须跟分辨率挂钩，否则分辨率变了，粒子的区域不会变）
            m_pxParticle[nNum].m_vecVel = new Vector3(GetRandomNum(-30, 30), GetRandomNum(70, 100), 0f);
            m_pxParticle[nNum].m_vecAccel = new Vector3(0f, 0f, 0f);
            m_pxParticle[nNum].m_vecOldPos = new Vector3(0f, 0f, 0f);
            m_pxParticle[nNum].m_vecLocalForce = new Vector3(0f, 0f, 0f);
            m_pxParticle[nNum].m_nLife = GetRandomNum(2500, 7000);
            m_pxParticle[nNum].m_fMass = 1000f + GetRandomFloatNum();
            m_pxParticle[nNum].m_fSize = (float)GetRandomNum(2, 6) + GetRandomFloatNum();
            m_pxParticle[nNum].m_bIsDead = false;
            m_pxParticle[nNum].m_bRed = (m_pxParticle[nNum].m_bGreen = (m_pxParticle[nNum].m_bBlue = (byte)GetRandomNum(120, 150)));
            m_pxParticle[nNum].m_bBlue += 100;
            m_pxParticle[nNum].m_nDelay = 300;
            m_pxParticle[nNum].m_nCurrLife = 0;
            m_pxParticle[nNum].m_nCurrDelay = 0;
            m_pxParticle[nNum].m_nCurrFrame = 0;
            m_pxParticle[nNum].m_bOpa = byte.MaxValue;
        }
    }
}
