using Client.Envir;
using Library;
using SharpDX;
using Color = System.Drawing.Color;

namespace Client.Models
{
    public class CSmoke : CParticleSystem
    {
        private short m_shPartNum;

        public CSmoke()
        {
            InitSystem();
        }

        ~CSmoke()
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
            SetEnvFactor(-0.05f, new Vector3(0f, 1000f, 0f), new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f));
            CEnvir.LibraryList.TryGetValue(LibraryFile.ProgUse, out Library);
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
                        m_pxParticle[nCnt].m_fSize = m_pxParticle[nCnt].m_fOriSize + m_pxParticle[nCnt].m_fOriSize * 7f * (float)((float)m_pxParticle[nCnt].m_nCurrLife / (float)m_pxParticle[nCnt].m_nLife);
                        Vector3 vecAddVel = new Vector3(GetRandomNum(-1, 1), 0f, 0f);
                        m_pxParticle[nCnt].m_vecVel += vecAddVel;

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

        public bool RenderSystem()
        {
            BlendRate = 8f / 255f;
            //return base.RenderSystem(m_shPartNum, 530, BlendState._BLEND_LIGHTINV, LibraryFile.ProgUse);

            int nPartCnt = 0;
            for (int nCnt = 0; nCnt < m_nNum; nCnt++)
            {
                if (!m_pxParticle[nCnt].m_bIsDead)
                {
                    Library.DrawBlend(530, m_pxParticle[nCnt].m_vecPos.X, m_pxParticle[nCnt].m_vecPos.Y, Color.FromArgb(m_pxParticle[nCnt].m_bRed, m_pxParticle[nCnt].m_bGreen, m_pxParticle[nCnt].m_bBlue), m_pxParticle[nCnt].m_fSize, m_pxParticle[nCnt].m_fSize, BlendRate, ImageType.Image, BlendType._BLEND_LIGHTINV);
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

        public void SetSmokeParticle(Vector3 vecGenPos)
        {
            int nGenCnt = 0;
            for (int nCnt = 0; nCnt < m_nNum; nCnt++)
            {
                if (m_pxParticle[nCnt].m_bIsDead)
                {
                    SetParticleDefault(nCnt, vecGenPos);
                    m_shPartNum++;
                    nGenCnt++;
                    if (nGenCnt > 0)
                    {
                        break;
                    }
                }
            }
        }

        public void SetSmokeParticleEx(Vector3 vecGenPos)
        {
            int nGenCnt = 0;
            for (int nCnt = 0; nCnt < m_nNum; nCnt++)
            {
                if (m_pxParticle[nCnt].m_bIsDead)
                {
                    m_pxParticle[nCnt].ZeroInit();
                    m_pxParticle[nCnt].m_vecPos = vecGenPos;
                    m_pxParticle[nCnt].m_vecVel = new Vector3(GetRandomNum(-12, 12), 0f, 0f);
                    m_pxParticle[nCnt].m_nLife = GetRandomNum(200, 1200);
                    m_pxParticle[nCnt].m_fMass = 1000f;
                    m_pxParticle[nCnt].m_fSize = (m_pxParticle[nCnt].m_fOriSize = (float)GetRandomNum(20, 25) + GetRandomFloatNum());
                    m_pxParticle[nCnt].m_bRed = (m_pxParticle[nCnt].m_bFstRed = (byte)GetRandomNum(80, 120));
                    m_pxParticle[nCnt].m_bGreen = (m_pxParticle[nCnt].m_bFstGreen = (byte)GetRandomNum(80, 120));
                    m_pxParticle[nCnt].m_bBlue = (m_pxParticle[nCnt].m_bFstBlue = (byte)GetRandomNum(220, 255));
                    m_pxParticle[nCnt].m_nDelay = GetRandomNum(200, 300);
                    m_shPartNum++;
                    nGenCnt++;
                    if (nGenCnt > 0)
                    {
                        break;
                    }
                }
            }
        }

        public void SetSmokeParticleEx2(Vector3 vecGenPos)
        {
            int nGenCnt = 0;
            for (int nCnt = 0; nCnt < m_nNum; nCnt++)
            {
                if (m_pxParticle[nCnt].m_bIsDead)
                {
                    m_pxParticle[nCnt].ZeroInit();
                    m_pxParticle[nCnt].m_vecPos = vecGenPos; //生成位置
                    m_pxParticle[nCnt].m_vecVel = new Vector3(GetRandomNum(-20, 20), 0f, 0f); //3轴上的加速度？
                    m_pxParticle[nCnt].m_nLife = GetRandomNum(400, 800); //持续时间
                    m_pxParticle[nCnt].m_fMass = 1000f; //浓厚程度
                    m_pxParticle[nCnt].m_fSize = (m_pxParticle[nCnt].m_fOriSize = (float)GetRandomNum(20, 25) + GetRandomFloatNum());
                    m_pxParticle[nCnt].m_bRed = (m_pxParticle[nCnt].m_bFstRed = (byte)GetRandomNum(50, 100));
                    m_pxParticle[nCnt].m_bGreen = (m_pxParticle[nCnt].m_bFstGreen = (byte)GetRandomNum(50, 100));
                    m_pxParticle[nCnt].m_bBlue = (m_pxParticle[nCnt].m_bFstBlue = (byte)GetRandomNum(255, 255));
                    m_pxParticle[nCnt].m_nDelay = GetRandomNum(200, 300);

                    m_shPartNum++;
                    nGenCnt++;
                    if (nGenCnt > 0)
                    {
                        break;
                    }
                }
            }
        }

        public void SetSmokeParticleEx3(Vector3 vecGenPos)
        {
            int nGenCnt = 0;
            for (int nCnt = 0; nCnt < m_nNum; nCnt++)
            {
                if (m_pxParticle[nCnt].m_bIsDead)
                {
                    m_pxParticle[nCnt].ZeroInit();
                    m_pxParticle[nCnt].m_vecPos = vecGenPos;
                    m_pxParticle[nCnt].m_vecVel = new Vector3(0f, 0f, 0f);
                    m_pxParticle[nCnt].m_nLife = GetRandomNum(400, 500);
                    m_pxParticle[nCnt].m_fMass = 10000f;
                    m_pxParticle[nCnt].m_fSize = m_pxParticle[nCnt].m_fOriSize = 7.0f;
                    m_pxParticle[nCnt].m_bRed = (m_pxParticle[nCnt].m_bFstRed = (byte)GetRandomNum(100, 150));
                    m_pxParticle[nCnt].m_bGreen = (m_pxParticle[nCnt].m_bFstGreen = (byte)GetRandomNum(150, 200));
                    m_pxParticle[nCnt].m_bBlue = (m_pxParticle[nCnt].m_bFstBlue = (byte)GetRandomNum(255, 255));
                    m_pxParticle[nCnt].m_nDelay = GetRandomNum(200, 300);
                    m_shPartNum++;
                    nGenCnt++;
                    if (nGenCnt > 0)
                    {
                        break;
                    }
                }
            }
        }

        //有一个向上的弧线
        public void SetSmokeParticleEx4(Vector3 vecGenPos)
        {
            int nGenCnt = 0;
            for (int nCnt = 0; nCnt < m_nNum; nCnt++)
            {
                if (m_pxParticle[nCnt].m_bIsDead)
                {
                    m_pxParticle[nCnt].ZeroInit();
                    m_pxParticle[nCnt].m_vecPos = vecGenPos;
                    m_pxParticle[nCnt].m_vecVel = new Vector3(GetRandomNum(-12, 12), GetRandomNum(-120, -80), 0f);
                    m_pxParticle[nCnt].m_nLife = GetRandomNum(300, 600);
                    m_pxParticle[nCnt].m_fMass = 1000f;
                    m_pxParticle[nCnt].m_fSize = (m_pxParticle[nCnt].m_fOriSize = GetRandomNum(10, 30));
                    m_pxParticle[nCnt].m_bRed = (m_pxParticle[nCnt].m_bFstRed = (byte)GetRandomNum(100, 150));
                    m_pxParticle[nCnt].m_bGreen = (m_pxParticle[nCnt].m_bFstGreen = (byte)GetRandomNum(150, 200));
                    m_pxParticle[nCnt].m_bBlue = (m_pxParticle[nCnt].m_bFstBlue = (byte)GetRandomNum(255, 255));
                    m_pxParticle[nCnt].m_nDelay = GetRandomNum(200, 300);
                    m_shPartNum++;
                    nGenCnt++;
                    if (nGenCnt > 0)
                    {
                        break;
                    }
                }
            }
        }

        public void SetSmokeParticleEx5(Vector3 vecGenPos)
        {
            int nGenCnt = 0;
            for (int nCnt = 0; nCnt < m_nNum; nCnt++)
            {
                if (m_pxParticle[nCnt].m_bIsDead)
                {
                    m_pxParticle[nCnt].ZeroInit();
                    m_pxParticle[nCnt].m_vecPos = vecGenPos;
                    m_pxParticle[nCnt].m_vecVel = new Vector3(GetRandomNum(-2, 2), 0f, 0f);
                    m_pxParticle[nCnt].m_nLife = GetRandomNum(200, 400);
                    m_pxParticle[nCnt].m_fMass = 1000f;
                    m_pxParticle[nCnt].m_fSize = (m_pxParticle[nCnt].m_fOriSize = (float)GetRandomNum(8, 12) + GetRandomFloatNum());
                    m_pxParticle[nCnt].m_bRed = (m_pxParticle[nCnt].m_bFstRed = (byte)GetRandomNum(220, 255));
                    m_pxParticle[nCnt].m_bGreen = (m_pxParticle[nCnt].m_bFstGreen = (byte)GetRandomNum(140, 160));
                    m_pxParticle[nCnt].m_bBlue = (m_pxParticle[nCnt].m_bFstBlue = (byte)GetRandomNum(60, 80));
                    m_pxParticle[nCnt].m_nDelay = GetRandomNum(200, 300);
                    m_shPartNum++;
                    nGenCnt++;
                    if (nGenCnt > 0)
                    {
                        break;
                    }
                }
            }
        }

        //尾流有向上的喷射
        public void SetSmokeParticleEx6(Vector3 vecGenPos)
        {
            int nGenCnt = 0;
            for (int nCnt = 0; nCnt < m_nNum; nCnt++)
            {
                if (m_pxParticle[nCnt].m_bIsDead)
                {
                    m_pxParticle[nCnt].ZeroInit();
                    m_pxParticle[nCnt].m_vecPos = vecGenPos;
                    m_pxParticle[nCnt].m_vecVel = new Vector3(GetRandomNum(-30, 0), GetRandomNum(-60, -60), 0f);
                    m_pxParticle[nCnt].m_nLife = GetRandomNum(800, 1200);
                    m_pxParticle[nCnt].m_fMass = 2000f;
                    m_pxParticle[nCnt].m_fSize = (m_pxParticle[nCnt].m_fOriSize = GetRandomNum(20, 40));
                    m_pxParticle[nCnt].m_bRed = (m_pxParticle[nCnt].m_bFstRed = (byte)GetRandomNum(50, 50));
                    m_pxParticle[nCnt].m_bGreen = (m_pxParticle[nCnt].m_bFstGreen = (byte)GetRandomNum(50, 50));
                    m_pxParticle[nCnt].m_bBlue = (m_pxParticle[nCnt].m_bFstBlue = (byte)GetRandomNum(50, 50));
                    m_pxParticle[nCnt].m_nDelay = GetRandomNum(200, 300);
                    m_shPartNum++;
                    nGenCnt++;
                    if (nGenCnt > 0)
                    {
                        break;
                    }
                }
            }
        }

        public void SetSmokeParticleEx7(Vector3 vecGenPos)
        {
            int nGenCnt = 0;
            for (int nCnt = 0; nCnt < m_nNum; nCnt++)
            {
                if (m_pxParticle[nCnt].m_bIsDead)
                {
                    m_pxParticle[nCnt].ZeroInit();
                    m_pxParticle[nCnt].m_vecPos = vecGenPos;
                    m_pxParticle[nCnt].m_vecVel = new Vector3(-20f, -20f, 0f);
                    m_pxParticle[nCnt].m_nLife = 1000;
                    m_pxParticle[nCnt].m_fMass = 10000f;
                    m_pxParticle[nCnt].m_fSize = m_pxParticle[nCnt].m_fOriSize = 18.0f;
                    m_pxParticle[nCnt].m_bRed = (m_pxParticle[nCnt].m_bFstRed = (byte)GetRandomNum(60, 70));
                    m_pxParticle[nCnt].m_bGreen = (m_pxParticle[nCnt].m_bFstGreen = (byte)GetRandomNum(60, 70));
                    m_pxParticle[nCnt].m_bBlue = (m_pxParticle[nCnt].m_bFstBlue = (byte)GetRandomNum(70, 80));
                    m_pxParticle[nCnt].m_nDelay = GetRandomNum(200, 300);
                    m_shPartNum++;
                    nGenCnt++;
                    if (nGenCnt > 0)
                    {
                        break;
                    }
                }
            }
        }

        public void SetSmokeParticleEx8(Vector3 vecGenPos)
        {
            int nGenCnt = 0;
            for (int nCnt = 0; nCnt < m_nNum; nCnt++)
            {
                if (m_pxParticle[nCnt].m_bIsDead)
                {
                    m_pxParticle[nCnt].ZeroInit();
                    m_pxParticle[nCnt].m_vecPos = vecGenPos;
                    m_pxParticle[nCnt].m_vecVel = new Vector3(GetRandomNum(-12, 12), 0f, 0f);
                    m_pxParticle[nCnt].m_nLife = GetRandomNum(200, 400);
                    m_pxParticle[nCnt].m_fMass = 1000f;
                    m_pxParticle[nCnt].m_fSize = (m_pxParticle[nCnt].m_fOriSize = (float)GetRandomNum(20, 20) + GetRandomFloatNum());
                    m_pxParticle[nCnt].m_bRed = (m_pxParticle[nCnt].m_bFstRed = (byte)GetRandomNum(60, 70));
                    m_pxParticle[nCnt].m_bGreen = (m_pxParticle[nCnt].m_bFstGreen = (byte)GetRandomNum(220, 230));
                    m_pxParticle[nCnt].m_bBlue = (m_pxParticle[nCnt].m_bFstBlue = (byte)GetRandomNum(210, 220));
                    m_pxParticle[nCnt].m_nDelay = GetRandomNum(200, 300);
                    m_shPartNum++;
                    nGenCnt++;
                    if (nGenCnt > 0)
                    {
                        break;
                    }
                }
            }
        }

        //尾流快速消散
        public void SetSmokeParticleEx9(Vector3 vecGenPos)
        {
            int nGenCnt = 0;
            for (int nCnt = 0; nCnt < m_nNum; nCnt++)
            {
                if (m_pxParticle[nCnt].m_bIsDead)
                {
                    m_pxParticle[nCnt].ZeroInit();
                    m_pxParticle[nCnt].m_vecPos = vecGenPos;
                    m_pxParticle[nCnt].m_vecVel = new Vector3(GetRandomNum(-20, 0), GetRandomNum(-90, -150), 0f);
                    m_pxParticle[nCnt].m_nLife = GetRandomNum(1000, 3000);
                    m_pxParticle[nCnt].m_fMass = 2000f;
                    m_pxParticle[nCnt].m_fSize = (m_pxParticle[nCnt].m_fOriSize = (float)GetRandomNum(8, 10) + GetRandomFloatNum());
                    m_pxParticle[nCnt].m_bRed = (m_pxParticle[nCnt].m_bFstRed = (m_pxParticle[nCnt].m_bGreen = (m_pxParticle[nCnt].m_bFstGreen = (m_pxParticle[nCnt].m_bBlue = (m_pxParticle[nCnt].m_bFstBlue = (byte)GetRandomNum(120, 150))))));
                    m_pxParticle[nCnt].m_nDelay = GetRandomNum(200, 300);
                    m_shPartNum++;
                    nGenCnt++;
                    if (nGenCnt > 1)
                    {
                        break;
                    }
                }
            }
        }

        //硕大的扇状尾流
        public void SetSmokeParticleEx10(Vector3 vecGenPos)
        {
            int nGenCnt = 0;
            for (int nCnt = 0; nCnt < m_nNum; nCnt++)
            {
                if (m_pxParticle[nCnt].m_bIsDead)
                {
                    m_pxParticle[nCnt].ZeroInit();
                    m_pxParticle[nCnt].m_vecPos = vecGenPos;
                    m_pxParticle[nCnt].m_vecVel = new Vector3(GetRandomNum(-10, 10), GetRandomNum(-80, 0), 0f);
                    m_pxParticle[nCnt].m_nLife = GetRandomNum(800, 1200);
                    m_pxParticle[nCnt].m_fMass = 2000f;
                    m_pxParticle[nCnt].m_fSize = (m_pxParticle[nCnt].m_fOriSize = GetRandomNum(20, 40));
                    m_pxParticle[nCnt].m_bRed = (m_pxParticle[nCnt].m_bFstRed = (byte)GetRandomNum(50, 50));
                    m_pxParticle[nCnt].m_bGreen = (m_pxParticle[nCnt].m_bFstGreen = (byte)GetRandomNum(50, 50));
                    m_pxParticle[nCnt].m_bBlue = (m_pxParticle[nCnt].m_bFstBlue = (byte)GetRandomNum(50, 50));
                    m_pxParticle[nCnt].m_nDelay = GetRandomNum(200, 300);
                    m_shPartNum++;
                    nGenCnt++;
                    if (nGenCnt > 0)
                    {
                        break;
                    }
                }
            }
        }

        //尾流有扇状向上的喷射
        public void SetSmokeParticleEx11(Vector3 vecGenPos)
        {
            int nGenCnt = 0;
            for (int nCnt = 0; nCnt < m_nNum; nCnt++)
            {
                if (m_pxParticle[nCnt].m_bIsDead)
                {
                    m_pxParticle[nCnt].ZeroInit();
                    m_pxParticle[nCnt].m_vecPos = vecGenPos;
                    m_pxParticle[nCnt].m_vecVel = new Vector3(GetRandomNum(-55, 5), GetRandomNum(-100, -50), 0f);
                    m_pxParticle[nCnt].m_nLife = GetRandomNum(900, 1000);
                    m_pxParticle[nCnt].m_fMass = 2000f;
                    m_pxParticle[nCnt].m_fSize = (m_pxParticle[nCnt].m_fOriSize = GetRandomNum(20, 40));
                    m_pxParticle[nCnt].m_bRed = (m_pxParticle[nCnt].m_bFstRed = (byte)GetRandomNum(50, 50));
                    m_pxParticle[nCnt].m_bGreen = (m_pxParticle[nCnt].m_bFstGreen = (byte)GetRandomNum(50, 50));
                    m_pxParticle[nCnt].m_bBlue = (m_pxParticle[nCnt].m_bFstBlue = (byte)GetRandomNum(50, 50));
                    m_pxParticle[nCnt].m_nDelay = GetRandomNum(200, 300);
                    m_shPartNum++;
                    nGenCnt++;
                    if (nGenCnt > 0)
                    {
                        break;
                    }
                }
            }
        }

        public void SetSmokeParticleEx12(Vector3 vecGenPos)
        {
            int nGenCnt = 0;
            for (int nCnt = 0; nCnt < m_nNum; nCnt++)
            {
                if (m_pxParticle[nCnt].m_bIsDead)
                {
                    m_pxParticle[nCnt].ZeroInit();
                    m_pxParticle[nCnt].m_vecPos = vecGenPos;
                    m_pxParticle[nCnt].m_vecVel = new Vector3(GetRandomNum(-10, 10), 0f, 0f);
                    m_pxParticle[nCnt].m_nLife = GetRandomNum(500, 500);
                    m_pxParticle[nCnt].m_fMass = 1000f;
                    m_pxParticle[nCnt].m_fSize = (m_pxParticle[nCnt].m_fOriSize = (float)GetRandomNum(10, 30) + GetRandomFloatNum());
                    m_pxParticle[nCnt].m_bRed = (m_pxParticle[nCnt].m_bFstRed = (m_pxParticle[nCnt].m_bGreen = (m_pxParticle[nCnt].m_bFstGreen = (m_pxParticle[nCnt].m_bBlue = (m_pxParticle[nCnt].m_bFstBlue = (byte)GetRandomNum(100, 150))))));
                    m_pxParticle[nCnt].m_nDelay = GetRandomNum(200, 300);
                    m_shPartNum++;
                    nGenCnt++;
                    if (nGenCnt > 0)
                    {
                        break;
                    }
                }
            }
        }

        public void SetSmokeParticleEx13(Vector3 vecGenPos)
        {
            int nGenCnt = 0;
            for (int nCnt = 0; nCnt < m_nNum; nCnt++)
            {
                if (m_pxParticle[nCnt].m_bIsDead)
                {
                    m_pxParticle[nCnt].ZeroInit();
                    m_pxParticle[nCnt].m_vecPos = vecGenPos;
                    m_pxParticle[nCnt].m_vecVel = new Vector3(GetRandomNum(-30, 0), GetRandomNum(-130, -100), 0f);
                    m_pxParticle[nCnt].m_nLife = GetRandomNum(800, 900);
                    m_pxParticle[nCnt].m_fMass = 1000f;
                    m_pxParticle[nCnt].m_fSize = (m_pxParticle[nCnt].m_fOriSize = GetRandomNum(20, 30));
                    m_pxParticle[nCnt].m_bRed = (m_pxParticle[nCnt].m_bFstRed = (byte)GetRandomNum(0, 10));
                    m_pxParticle[nCnt].m_bGreen = (m_pxParticle[nCnt].m_bFstGreen = (byte)GetRandomNum(65, 75));
                    m_pxParticle[nCnt].m_bBlue = (m_pxParticle[nCnt].m_bFstBlue = (byte)GetRandomNum(140, 150));
                    m_pxParticle[nCnt].m_nDelay = GetRandomNum(1000, 1500);
                    m_shPartNum++;
                    nGenCnt++;
                    if (nGenCnt > 0)
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
            m_pxParticle[nNum].m_vecVel = new Vector3(GetRandomNum(-8, 8), 0f, 0f);
            m_pxParticle[nNum].m_nLife = GetRandomNum(150, 400);
            m_pxParticle[nNum].m_fMass = 1000f;
            m_pxParticle[nNum].m_fSize = (m_pxParticle[nNum].m_fOriSize = (float)GetRandomNum(5, 10) + GetRandomFloatNum());
            m_pxParticle[nNum].m_bRed = (m_pxParticle[nNum].m_bFstRed = (m_pxParticle[nNum].m_bGreen = (m_pxParticle[nNum].m_bFstGreen = (m_pxParticle[nNum].m_bBlue = (m_pxParticle[nNum].m_bFstBlue = (byte)GetRandomNum(100, 150))))));
            m_pxParticle[nNum].m_nDelay = GetRandomNum(200, 300);
        }
    }
}
