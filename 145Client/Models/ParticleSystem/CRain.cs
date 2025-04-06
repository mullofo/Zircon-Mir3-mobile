using Client.Envir;
using Library;
using SharpDX;
using Color = System.Drawing.Color;

namespace Client.Models
{
    public class CRain : CParticleSystem
    {
        private float m_fWidth;

        private float m_fGround;

        private int m_shPartNum;

        public byte m_bGenCnt;

        public CRain()
        {
            InitSystem();
        }

        ~CRain()
        {
            DestroySystem();
        }

        public override void InitSystem()
        {
            base.InitSystem();
            m_fWidth = m_fGround = 0.0f;
            m_bGenCnt = 10;
        }

        public override void DestroySystem()
        {
            base.DestroySystem();
            InitSystem();
        }

        public void SetupSystem(int wCnt = 400, float fWidth = 1024f, float fGround = 768f)
        {
            InitSystem();
            base.SetupSystem(wCnt);
            m_fWidth = fWidth;
            m_fGround = fGround;
            SetEnvFactor(-0.05f, new Vector3(10f, 100f, 0f), new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f));
            CEnvir.LibraryList.TryGetValue(LibraryFile.ProgUse, out Library);
            BlendRate = 0.6f;
        }

        public override void UpdateSystem(int nLoopTime, Vector3 vecGenPos)
        {
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
                if (m_pxParticle[nCnt].m_bIsDead && nGenCnt < 11)
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
                        if (m_pxParticle[nCnt].m_nCurrLife - m_pxParticle[nCnt].m_nLife > 250)
                        {
                            m_pxParticle[nCnt].Init();
                            m_shPartNum--;
                            nPartCnt--;
                        }
                        else
                        {
                            byte bRate = (byte)(250 - (m_pxParticle[nCnt].m_nCurrLife - m_pxParticle[nCnt].m_nLife) / 4);
                            m_pxParticle[nCnt].m_bOpa = bRate;
                            m_pxParticle[nCnt].m_nDelay = 50;
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
                            m_pxParticle[nCnt].m_nCurrDelay += nLoopTime;
                            if (m_pxParticle[nCnt].m_nCurrDelay > m_pxParticle[nCnt].m_nDelay)
                            {
                                m_pxParticle[nCnt].m_nCurrDelay = 0;
                                m_pxParticle[nCnt].m_nCurrFrame++;
                                if (m_pxParticle[nCnt].m_nCurrFrame >= 5)
                                {
                                    m_pxParticle[nCnt].m_nCurrFrame = 0;
                                }
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
                    if (m_pxParticle[nCnt].m_nCurrLife - m_pxParticle[nCnt].m_nLife > 0 && m_pxParticle[nCnt].m_nCurrLife - m_pxParticle[nCnt].m_nLife < 510)
                    {
                        //Library.DrawBlend(510 + m_pxParticle[nCnt].m_nCurrFrame, m_pxParticle[nCnt].m_vecPos.X, m_pxParticle[nCnt].m_vecPos.Y, Color.White, 32f, 32f, BlendRate, ImageType.Image, BlendState._BLEND_LIGHTINV);
                        Library.DrawBlend(510 + m_pxParticle[nCnt].m_nCurrFrame, m_pxParticle[nCnt].m_vecPos.X + 32f / 3f, m_pxParticle[nCnt].m_vecPos.Y + 32f / 3f, Color.White, 32f, 32f, BlendRate, ImageType.Image, BlendType._BLEND_LIGHTINV);
                    }
                    else
                    {
                        //Library.DrawBlend(500 + m_pxParticle[nCnt].m_nCurrFrame, m_pxParticle[nCnt].m_vecPos.X, m_pxParticle[nCnt].m_vecPos.Y, Color.White, 1.5f, m_pxParticle[nCnt].m_fSize, BlendRate, ImageType.Image, BlendState._BLEND_LIGHTINV);
                        Library.DrawBlend(500 + m_pxParticle[nCnt].m_nCurrFrame, m_pxParticle[nCnt].m_vecPos.X + 1.5f / 3f, m_pxParticle[nCnt].m_vecPos.Y + m_pxParticle[nCnt].m_fSize / 3f, Color.White, 1.5f, m_pxParticle[nCnt].m_fSize, BlendRate, ImageType.Image, BlendType._BLEND_LIGHTINV);
                    }
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
            m_pxParticle[nNum].ZeroInit();
            //m_pxParticle[nNum].m_vecPos = new Vector3(GetRandomNum(-100, (int)(m_fWidth + 100f)), GetRandomNum(-500, 0), 0f);
            m_pxParticle[nNum].m_vecPos = new Vector3(GetRandomNum(-100, (int)(m_fWidth + 100f)), GetRandomNum(-(int)(m_fGround - 100f), 0), 0f); //雨点的绘制区域（必须跟分辨率挂钩，否则分辨率变了，粒子的区域不会变）
            m_pxParticle[nNum].m_vecVel = new Vector3(0f, 500f, 0f);
            //m_pxParticle[nNum].m_nLife = GetRandomNum(800, 1400);
            m_pxParticle[nNum].m_nLife = GetRandomNum(800, (int)(m_fGround) * 2); //雨点的坠落生命周期（决定着雨点的坠落长度。随着分辨率变化，随机值区域也要变大，否则雨点无法落到下面去）
            m_pxParticle[nNum].m_fMass = 100f;
            m_pxParticle[nNum].m_fSize = (float)GetRandomNum(3, 30) + GetRandomFloatNum();
            m_pxParticle[nNum].m_bRed = (m_pxParticle[nNum].m_bGreen = (m_pxParticle[nNum].m_bBlue = (byte)GetRandomNum(120, 180)));
            m_pxParticle[nNum].m_bBlue = 125;
            m_pxParticle[nNum].m_nDelay = GetRandomNum(50, 150);
        }
    }
}
