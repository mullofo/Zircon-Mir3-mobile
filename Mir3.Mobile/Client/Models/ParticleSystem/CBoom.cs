using Client.Envir;
using Library;
using Microsoft.Xna.Framework;
using Color = System.Drawing.Color;

namespace Client.Models
{
    public class CBoom : CParticleSystem
    {
        private short m_shPartNum;

        public CBoom()
        {
            InitSystem();
        }

        ~CBoom()
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
            SetEnvFactor(-0.05f, new Vector3(0f, 200f, 0f), new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f));
            CEnvir.LibraryList.TryGetValue(LibraryFile.ProgUse, out Library);
        }

        public override void UpdateSystem(int nLoopTime, Vector3 vecGenPos)
        {
            int nSpeedRate = nLoopTime / 17;
            int nPartCnt = 0;
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
                        m_pxParticle[nCnt].m_fMass += 3f;
                        if (m_pxParticle[nCnt].m_fSize < 0f)
                        {
                            m_pxParticle[nCnt].m_fSize = 0f;
                        }
                        if (m_pxParticle[nCnt].m_nCurrDelay > m_pxParticle[nCnt].m_nDelay)
                        {
                            m_pxParticle[nCnt].m_nCurrDelay = 0;
                            m_pxParticle[nCnt].m_nCurrFrame++;
                            if (m_pxParticle[nCnt].m_nCurrFrame >= 4)
                            {
                                m_pxParticle[nCnt].m_nCurrFrame = 0;
                            }
                        }
                        if (m_pxParticle[nCnt].m_nLife == 0)
                        {
                            m_pxParticle[nCnt].m_nLife = 1;
                        }
                        m_pxParticle[nCnt].m_fSize = m_pxParticle[nCnt].m_fOriSize - m_pxParticle[nCnt].m_fOriSize * (float)((float)m_pxParticle[nCnt].m_nCurrLife / (float)m_pxParticle[nCnt].m_nLife);
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

        public bool RenderSystem()
        {
            BlendRate = 12f / 85f;
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

        public void SetBoomParticle(Vector3 vecGenPos)
        {
            int nGenCnt = 0;
            for (int nCnt = 0; nCnt < m_nNum; nCnt++)
            {
                if (m_pxParticle[nCnt].m_bIsDead)
                {
                    SetParticleDefault(nCnt, vecGenPos);
                    m_shPartNum++;
                    nGenCnt++;
                    if (nGenCnt > 1)
                    {
                        break;
                    }
                }
            }
        }

        public void SetBoomParticle2(Vector3 vecGenPos)
        {
            int nGenCnt = 0;
            for (int nCnt = 0; nCnt < m_nNum; nCnt++)
            {
                if (m_pxParticle[nCnt].m_bIsDead)
                {
                    SetParticleDefault2(nCnt, vecGenPos);
                    m_shPartNum++;
                    nGenCnt++;
                    if (nGenCnt > 20)
                    {
                        break;
                    }
                }
            }
        }

        public void SetBoomParticle3(Vector3 vecGenPos)
        {
            int nGenCnt = 0;
            for (int nCnt = 0; nCnt < m_nNum; nCnt++)
            {
                if (m_pxParticle[nCnt].m_bIsDead)
                {
                    m_pxParticle[nCnt].ZeroInit();
                    m_pxParticle[nCnt].m_vecPos = vecGenPos;
                    m_pxParticle[nCnt].m_vecVel = new Vector3(GetRandomNum(-300, 300), GetRandomNum(-300, 150), 0f);
                    m_pxParticle[nCnt].m_nLife = GetRandomNum(300, 1600);
                    m_pxParticle[nCnt].m_fMass = 10f;
                    m_pxParticle[nCnt].m_fSize = (m_pxParticle[nCnt].m_fOriSize = (float)GetRandomNum(2, 2) + GetRandomFloatNum());
                    m_pxParticle[nCnt].m_bRed = (m_pxParticle[nCnt].m_bFstRed = (byte)GetRandomNum(30, 60));
                    m_pxParticle[nCnt].m_bGreen = (m_pxParticle[nCnt].m_bFstGreen = (byte)GetRandomNum(130, 160));
                    m_pxParticle[nCnt].m_bBlue = (m_pxParticle[nCnt].m_bFstBlue = (byte)GetRandomNum(130, 160));
                    m_pxParticle[nCnt].m_nDelay = GetRandomNum(200, 300);
                    m_shPartNum++;
                    nGenCnt++;
                    if (nGenCnt > 20)
                    {
                        break;
                    }
                }
            }
        }

        public void SetBoomParticle4(Vector3 vecGenPos)
        {
            int nGenCnt = 0;
            for (int nCnt = 0; nCnt < m_nNum; nCnt++)
            {
                if (m_pxParticle[nCnt].m_bIsDead)
                {
                    SetParticleDefault3(nCnt, vecGenPos);
                    m_shPartNum++;
                    nGenCnt++;
                    if (nGenCnt > 10)
                    {
                        break;
                    }
                }
            }
        }

        public void SetBoomParticle5(Vector3 vecGenPos)
        {
            int nGenCnt = 0;
            for (int nCnt = 0; nCnt < m_nNum; nCnt++)
            {
                if (m_pxParticle[nCnt].m_bIsDead)
                {
                    SetParticleDefault2(nCnt, vecGenPos);
                    m_shPartNum++;
                    nGenCnt++;
                    if (nGenCnt > 5)
                    {
                        break;
                    }
                }
            }
        }

        public void SetBoomParticle6(Vector3 vecGenPos)
        {
            int nGenCnt = 0;
            for (int nCnt = 0; nCnt < 600; nCnt++)
            {
                if (m_pxParticle[nCnt].m_bIsDead)
                {
                    //m_pxParticle[nCnt].ZeroInit();
                    //m_pxParticle[nCnt].m_vecPos = vecGenPos;
                    //m_pxParticle[nCnt].m_vecVel = new Vector3(GetRandomNum(-80, 80), GetRandomNum(-180, 10), 0f);
                    //m_pxParticle[nCnt].m_nLife = GetRandomNum(800, 1200);
                    //m_pxParticle[nCnt].m_fMass = 5f;
                    //m_pxParticle[nCnt].m_fSize = (m_pxParticle[nCnt].m_fOriSize = (float)GetRandomNum(4, 5) + (float)CEnvir.Random.NextDouble());
                    //m_pxParticle[nCnt].m_bRed = (m_pxParticle[nCnt].m_bFstRed = (byte)GetRandomNum(120, 130));
                    //m_pxParticle[nCnt].m_bGreen = (m_pxParticle[nCnt].m_bFstGreen = (byte)GetRandomNum(200, 210));
                    //m_pxParticle[nCnt].m_bBlue = (m_pxParticle[nCnt].m_bFstBlue = (byte)GetRandomNum(230, 240));
                    //m_pxParticle[nCnt].m_nDelay = GetRandomNum(0, 0);
                    SetParticleDefault2(nCnt, vecGenPos);

                    m_shPartNum++;
                    nGenCnt++;
                    if (nGenCnt > 5)
                    {
                        break;
                    }
                }
            }
        }

        public override void SetParticleDefault(int nNum, Vector3 vecGenPos)
        {
            m_pxParticle[nNum].ZeroInit();
            m_pxParticle[nNum].m_vecPos = vecGenPos;
            m_pxParticle[nNum].m_vecVel = new Vector3(GetRandomNum(-75, 75), GetRandomNum(-180, -50), 0f);
            m_pxParticle[nNum].m_nLife = GetRandomNum(800, 1200);
            m_pxParticle[nNum].m_fMass = 1f;
            m_pxParticle[nNum].m_fSize = (m_pxParticle[nNum].m_fOriSize = (float)GetRandomNum(2, 10) + GetRandomFloatNum());
            m_pxParticle[nNum].m_bRed = (m_pxParticle[nNum].m_bFstRed = (byte)GetRandomNum(50, 100));
            m_pxParticle[nNum].m_bGreen = (m_pxParticle[nNum].m_bFstGreen = (byte)GetRandomNum(100, 200));
            m_pxParticle[nNum].m_bBlue = (m_pxParticle[nNum].m_bFstBlue = (byte)GetRandomNum(200, 255));
            m_pxParticle[nNum].m_nDelay = GetRandomNum(200, 300);
        }

        public void SetParticleDefault2(int nNum, Vector3 vecGenPos)
        {
            m_pxParticle[nNum].ZeroInit();
            m_pxParticle[nNum].m_vecPos = vecGenPos;
            m_pxParticle[nNum].m_vecVel = new Vector3(GetRandomNum(-175, 175), GetRandomNum(-180, 10), 0f);
            m_pxParticle[nNum].m_nLife = GetRandomNum(800, 2000);
            m_pxParticle[nNum].m_fMass = 1f;
            m_pxParticle[nNum].m_fSize = (m_pxParticle[nNum].m_fOriSize = (float)GetRandomNum(6, 7) + GetRandomFloatNum());
            m_pxParticle[nNum].m_bRed = (m_pxParticle[nNum].m_bFstRed = (byte)GetRandomNum(40, 50));
            m_pxParticle[nNum].m_bGreen = (m_pxParticle[nNum].m_bFstGreen = (byte)GetRandomNum(40, 50));
            m_pxParticle[nNum].m_bBlue = (m_pxParticle[nNum].m_bFstBlue = (byte)GetRandomNum(40, 50));
            m_pxParticle[nNum].m_nDelay = GetRandomNum(200, 300);
        }

        public void SetParticleDefault3(int nNum, Vector3 vecGenPos)
        {
            m_pxParticle[nNum].ZeroInit();
            m_pxParticle[nNum].m_vecPos = vecGenPos;
            m_pxParticle[nNum].m_vecVel = new Vector3(GetRandomNum(-120, 120), GetRandomNum(-180, 0), 0f);
            m_pxParticle[nNum].m_nLife = GetRandomNum(200, 1200);
            m_pxParticle[nNum].m_fMass = 1f;
            m_pxParticle[nNum].m_fSize = (m_pxParticle[nNum].m_fOriSize = (float)GetRandomNum(3, 12) + GetRandomFloatNum());
            m_pxParticle[nNum].m_bRed = (m_pxParticle[nNum].m_bFstRed = (byte)GetRandomNum(40, 50));
            m_pxParticle[nNum].m_bGreen = (m_pxParticle[nNum].m_bFstGreen = (byte)GetRandomNum(40, 50));
            m_pxParticle[nNum].m_bBlue = (m_pxParticle[nNum].m_bFstBlue = (byte)GetRandomNum(40, 50));
            m_pxParticle[nNum].m_nDelay = GetRandomNum(200, 300);
        }
    }
}
