using Client.Envir;
using Library;
using SharpDX;
using Color = System.Drawing.Color;

namespace Client.Models
{
    public class CFlyingTail : CParticleSystem
    {
        private int m_shPartNum;

        public CFlyingTail()
        {
            InitSystem();
        }

        ~CFlyingTail()
        {
            DestroySystem();
        }

        public override void InitSystem()
        {
            base.InitSystem();
            m_shPartNum = 0;
        }

        public override void DestroySystem()
        {
            base.DestroySystem();
            InitSystem();
        }

        public override void SetupSystem(int wCnt = 1000)
        {
            InitSystem();
            base.SetupSystem(wCnt);
            SetEnvFactor(-0.05f, new Vector3(100f, 1000f, 0f), new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f));
            //SetEnvFactor(-0.05f, new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f));
            CEnvir.LibraryList.TryGetValue(LibraryFile.ProgUse, out Library);
        }

        public bool RenderSystem()
        {
            BlendRate = 8f / 255f;
            //return base.RenderSystem(m_shPartNum, 520, BlendState._BLEND_CSnow);

            int nPartCnt = 0;
            for (int nCnt = 0; nCnt < m_nNum; nCnt++)
            {
                if (!m_pxParticle[nCnt].m_bIsDead)
                {
                    Library.DrawBlend(520 + m_pxParticle[nCnt].m_nCurrFrame, m_pxParticle[nCnt].m_vecPos.X, m_pxParticle[nCnt].m_vecPos.Y, Color.FromArgb(m_pxParticle[nCnt].m_bRed, m_pxParticle[nCnt].m_bGreen, m_pxParticle[nCnt].m_bBlue), m_pxParticle[nCnt].m_fSize, m_pxParticle[nCnt].m_fSize, BlendRate, ImageType.Image, BlendType._BLEND_LIGHT);
                    //System.Diagnostics.Debug.WriteLine("X = " + m_pxParticle[i].m_vecPos.X + ", Y = " + m_pxParticle[i].m_vecPos.Y);
                    nPartCnt++;
                }
                if (nPartCnt >= m_shPartNum)
                {
                    return true;
                }
            }
            return true;
        }

        public void SetFlyTailParticle(Vector3 vecGenPos)
        {
            int nGenCnt = 0;
            for (int nCnt = 0; nCnt < m_nNum; nCnt++)
            {
                if (m_pxParticle[nCnt].m_bIsDead)
                {
                    SetParticleDefault(nCnt, vecGenPos);
                    m_shPartNum++;
                    nGenCnt++;
                    if (nGenCnt > 2)
                    {
                        break;
                    }
                }
            }
        }

        public void SetFlyTailParticleEx(Vector3 vecGenPos)
        {
            int nnGenCntum = 0;
            for (int nCnt = 0; nCnt < m_nNum; nCnt++)
            {
                if (m_pxParticle[nCnt].m_bIsDead)
                {
                    m_pxParticle[nCnt].ZeroInit();
                    m_pxParticle[nCnt].m_vecPos = new Vector3(vecGenPos.X + (float)GetRandomNum(-10, 10), vecGenPos.Y + (float)GetRandomNum(-10, 10), 0f);
                    m_pxParticle[nCnt].m_vecVel = new Vector3(GetRandomNum(-150, 150), GetRandomNum(-80, 150), 0f);
                    m_pxParticle[nCnt].m_nLife = GetRandomNum(200, 1200);
                    m_pxParticle[nCnt].m_fMass = 1000f;
                    m_pxParticle[nCnt].m_fSize = (m_pxParticle[nCnt].m_fOriSize = (float)GetRandomNum(5, 10) + GetRandomFloatNum());
                    m_pxParticle[nCnt].m_bRed = (m_pxParticle[nCnt].m_bFstRed = (byte)GetRandomNum(100, 110));
                    m_pxParticle[nCnt].m_bGreen = (m_pxParticle[nCnt].m_bFstGreen = (byte)GetRandomNum(100, 110));
                    m_pxParticle[nCnt].m_bBlue = (m_pxParticle[nCnt].m_bFstBlue = (byte)GetRandomNum(245, 255));
                    m_pxParticle[nCnt].m_nDelay = GetRandomNum(200, 300);
                    m_shPartNum++;
                    nnGenCntum++;
                    if (nnGenCntum > 1)
                    {
                        break;
                    }
                }
            }
        }

        public void SetFlyTailParticleEx2(Vector3 vecGenPos)
        {
            int nGenCnt = 0;
            for (int nCnt = 0; nCnt < m_nNum; nCnt++)
            {
                if (m_pxParticle[nCnt].m_bIsDead)
                {
                    m_pxParticle[nCnt].ZeroInit();
                    m_pxParticle[nCnt].m_vecPos = new Vector3(vecGenPos.X + (float)GetRandomNum(-10, 10), vecGenPos.Y + (float)GetRandomNum(-10, 10), 0f);
                    m_pxParticle[nCnt].m_vecVel = new Vector3(GetRandomNum(-30, 30), GetRandomNum(-30, 30), 0f);
                    m_pxParticle[nCnt].m_nLife = GetRandomNum(200, 1200);
                    m_pxParticle[nCnt].m_fMass = 1000f;
                    m_pxParticle[nCnt].m_fSize = (m_pxParticle[nCnt].m_fOriSize = (float)GetRandomNum(3, 7) + GetRandomFloatNum());
                    m_pxParticle[nCnt].m_bRed = (m_pxParticle[nCnt].m_bFstRed = (byte)GetRandomNum(100, 150));
                    m_pxParticle[nCnt].m_bGreen = (m_pxParticle[nCnt].m_bFstGreen = (byte)GetRandomNum(150, 255));
                    m_pxParticle[nCnt].m_bBlue = (m_pxParticle[nCnt].m_bFstBlue = (byte)GetRandomNum(100, 150));
                    m_pxParticle[nCnt].m_nDelay = GetRandomNum(200, 300);
                    m_shPartNum++;
                    nGenCnt++;
                    if (nGenCnt > 2)
                    {
                        break;
                    }
                }
            }
        }

        public void SetFlyTailParticleEx3(Vector3 vecGenPos)
        {
            int nGenCnt = 0;
            for (int nCnt = 0; nCnt < m_nNum; nCnt++)
            {
                if (m_pxParticle[nCnt].m_bIsDead)
                {
                    m_pxParticle[nCnt].ZeroInit();
                    m_pxParticle[nCnt].m_vecPos = vecGenPos;
                    m_pxParticle[nCnt].m_vecVel = new Vector3(GetRandomNum(-50, 50), GetRandomNum(-30, 60), 0f);
                    m_pxParticle[nCnt].m_nLife = GetRandomNum(500, 900);
                    m_pxParticle[nCnt].m_fMass = 1000f;
                    m_pxParticle[nCnt].m_fSize = (m_pxParticle[nCnt].m_fOriSize = (float)GetRandomNum(3, 5) + GetRandomFloatNum());
                    m_pxParticle[nCnt].m_bRed = (m_pxParticle[nCnt].m_bFstRed = (byte)GetRandomNum(200, 255));
                    m_pxParticle[nCnt].m_bGreen = (m_pxParticle[nCnt].m_bFstGreen = (byte)((int)m_pxParticle[nCnt].m_bFstRed / 2));
                    m_pxParticle[nCnt].m_bBlue = (m_pxParticle[nCnt].m_bFstBlue = (byte)GetRandomNum(0, 30));
                    m_pxParticle[nCnt].m_nDelay = GetRandomNum(200, 300);
                    m_shPartNum++;
                    nGenCnt++;
                    if (nGenCnt > 4)
                    {
                        break;
                    }
                }
            }
        }

        public void SetFlyTailParticleEx4(Vector3 vecGenPos)
        {
            int nGenCnt = 0;
            for (int nCnt = 0; nCnt < m_nNum; nCnt++)
            {
                if (m_pxParticle[nCnt].m_bIsDead)
                {
                    m_pxParticle[nCnt].ZeroInit();
                    m_pxParticle[nCnt].m_vecPos = vecGenPos;
                    m_pxParticle[nCnt].m_vecVel = new Vector3(GetRandomNum(-50, 50), GetRandomNum(-30, 60), 0f);
                    m_pxParticle[nCnt].m_nLife = GetRandomNum(20, 500);
                    m_pxParticle[nCnt].m_fMass = 10000f;
                    m_pxParticle[nCnt].m_fSize = (float)GetRandomNum(5, 15) + GetRandomFloatNum();
                    m_pxParticle[nCnt].m_bRed = (m_pxParticle[nCnt].m_bFstRed = (byte)GetRandomNum(70, 90));
                    m_pxParticle[nCnt].m_bGreen = (m_pxParticle[nCnt].m_bFstGreen = (byte)GetRandomNum(70, 90));
                    m_pxParticle[nCnt].m_bBlue = (m_pxParticle[nCnt].m_bFstBlue = (byte)GetRandomNum(100, 150));
                    m_pxParticle[nCnt].m_nDelay = GetRandomNum(0, 10);
                    m_shPartNum++;
                    nGenCnt++;
                    if (nGenCnt > 2)
                    {
                        break;
                    }
                }
            }
        }

        public void SetFlyTailParticleEx5(Vector3 vecGenPos)
        {
            int nGenCnt = 0;
            for (int nCnt = 0; nCnt < m_nNum; nCnt++)
            {
                if (m_pxParticle[nCnt].m_bIsDead)
                {
                    m_pxParticle[nCnt].ZeroInit();
                    m_pxParticle[nCnt].m_vecPos = vecGenPos;
                    m_pxParticle[nCnt].m_vecVel = new Vector3(GetRandomNum(-45, 45), GetRandomNum(-150, -250), 0f);
                    m_pxParticle[nCnt].m_nLife = GetRandomNum(1000, 1600);
                    m_pxParticle[nCnt].m_fMass = 100f;
                    m_pxParticle[nCnt].m_fSize = 4f;
                    m_pxParticle[nCnt].m_bRed = m_pxParticle[nCnt].m_bFstRed = 255;
                    m_pxParticle[nCnt].m_bGreen = m_pxParticle[nCnt].m_bFstGreen = 125; ;
                    m_pxParticle[nCnt].m_bBlue = m_pxParticle[nCnt].m_bFstBlue = 0;
                    m_pxParticle[nCnt].m_nDelay = GetRandomNum(200, 300);
                    m_shPartNum++;
                    nGenCnt++;
                    if (nGenCnt > 2)
                    {
                        break;
                    }
                }
            }
        }

        public override void UpdateSystem(int nLoopTime, Vector3 vecGenPos)
        {
            int nPartCnt = 0;
            int nSpeedRate = nLoopTime / 17;
            if (nSpeedRate < 1)
            {
                nSpeedRate = 1;
            }
            m_fDeltaTime = 0.02f * nSpeedRate;
            for (int nCnt = 0; nCnt < m_nNum; nCnt++)
            {
                if (!m_pxParticle[nCnt].m_bIsDead)
                {
                    m_pxParticle[nCnt].m_nCurrLife += nLoopTime;
                    if (m_pxParticle[nCnt].m_nCurrLife > m_pxParticle[nCnt].m_nLife)
                    {
                        m_pxParticle[nCnt].Init();
                        m_shPartNum--;
                        nPartCnt--;
                    }
                    else
                    {
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
                        if (m_pxParticle[nCnt].m_nLife == 0)
                        {
                            m_pxParticle[nCnt].m_nLife = 1;
                        }
                        byte bRate = (byte)(m_pxParticle[nCnt].m_bFstRed - m_pxParticle[nCnt].m_bFstRed * (float)((float)m_pxParticle[nCnt].m_nCurrLife / (float)m_pxParticle[nCnt].m_nLife));
                        m_pxParticle[nCnt].m_bRed = bRate;
                        bRate = (byte)(m_pxParticle[nCnt].m_bFstGreen - m_pxParticle[nCnt].m_bFstGreen * (float)((float)m_pxParticle[nCnt].m_nCurrLife / (float)m_pxParticle[nCnt].m_nLife));
                        m_pxParticle[nCnt].m_bGreen = bRate;
                        bRate = (byte)(m_pxParticle[nCnt].m_bFstBlue - m_pxParticle[nCnt].m_bFstBlue * (float)((float)m_pxParticle[nCnt].m_nCurrLife / (float)m_pxParticle[nCnt].m_nLife));
                        m_pxParticle[nCnt].m_bBlue = bRate;
                        UpdateAirFiction(nCnt);
                        UpdateMove(nCnt);
                        nPartCnt++;
                    }
                }
                if (nPartCnt >= m_shPartNum)
                {
                    break;
                }
            }
        }

        public override void SetParticleDefault(int nNum, Vector3 vecGenPos)
        {
            m_pxParticle[nNum].ZeroInit();
            m_pxParticle[nNum].m_vecPos = vecGenPos;
            m_pxParticle[nNum].m_vecVel = new Vector3(GetRandomNum(-50, 50), GetRandomNum(-30, 60), 0f);
            m_pxParticle[nNum].m_nLife = GetRandomNum(150, 400);
            m_pxParticle[nNum].m_fMass = 1000f;
            m_pxParticle[nNum].m_fSize = (float)GetRandomNum(5, 15) + GetRandomFloatNum();
            m_pxParticle[nNum].m_bRed = (m_pxParticle[nNum].m_bFstRed = (byte)GetRandomNum(200, 255));
            m_pxParticle[nNum].m_bGreen = (m_pxParticle[nNum].m_bFstGreen = (byte)((int)m_pxParticle[nNum].m_bFstRed / 2));
            m_pxParticle[nNum].m_bBlue = (m_pxParticle[nNum].m_bFstBlue = (byte)GetRandomNum(0, 30));
            m_pxParticle[nNum].m_nDelay = GetRandomNum(200, 300);
        }
    }
}
