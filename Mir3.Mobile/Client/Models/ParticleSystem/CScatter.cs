using Client.Envir;
using Library;
using Microsoft.Xna.Framework;
using System;
using Color = System.Drawing.Color;

namespace Client.Models
{
    public class CScatter : CParticleSystem
    {
        private short m_shPartNum;

        public CScatter()
        {
            InitSystem();
        }

        ~CScatter()
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

        public override void SetupSystem(int wCnt = 2000)
        {
            InitSystem();
            base.SetupSystem(wCnt);
            CEnvir.LibraryList.TryGetValue(LibraryFile.ProgUse, out Library);
        }

        public override void UpdateSystem(int nLoopTime, Vector3 vecGenPos)
        {
            int nPartCnt = 0;
            for (int nCnt = 0; nCnt < m_nNum; nCnt++)
            {
                if (!m_pxParticle[nCnt].m_bIsDead)
                {
                    if (!m_pxParticle[nCnt].m_bIsDead)
                    {
                        m_pxParticle[nCnt].m_nCurrLife += nLoopTime;
                    }
                    int nScrnGapX, nScrnGapY;
                    if (m_pxParticle[nCnt].m_vecPos.X != m_pxParticle[nCnt].m_vecDstPos.X)
                    {
                        nScrnGapX = (int)Math.Abs(m_pxParticle[nCnt].m_vecDstPos.X - m_pxParticle[nCnt].m_vecPos.X);
                    }
                    else
                    {
                        nScrnGapX = 1;
                    }

                    if (m_pxParticle[nCnt].m_vecPos.Y != m_pxParticle[nCnt].m_vecDstPos.Y)
                    {
                        nScrnGapY = (int)Math.Abs(m_pxParticle[nCnt].m_vecDstPos.Y - m_pxParticle[nCnt].m_vecPos.Y);
                    }
                    else
                    {
                        nScrnGapY = 1;
                    }

                    if (nScrnGapX == 0)
                    {
                        nScrnGapX = 1;
                    }
                    if (nScrnGapY == 0)
                    {
                        nScrnGapY = 1;
                    }

                    float fGapRateX = (float)(500 / (float)nScrnGapX);
                    float fGapRateY = (float)(500 / (float)nScrnGapY);
                    int nDisX, nDisY;

                    if (nScrnGapX > nScrnGapY)
                    {
                        nDisX = (int)((m_pxParticle[nCnt].m_vecDstPos.X - m_pxParticle[nCnt].m_vecPos.X) * fGapRateX);
                        nDisY = (int)((m_pxParticle[nCnt].m_vecDstPos.Y - m_pxParticle[nCnt].m_vecPos.Y) * fGapRateX);
                    }
                    else
                    {
                        nDisX = (int)((m_pxParticle[nCnt].m_vecDstPos.X - m_pxParticle[nCnt].m_vecPos.X) * fGapRateY);
                        nDisY = (int)((m_pxParticle[nCnt].m_vecDstPos.Y - m_pxParticle[nCnt].m_vecPos.Y) * fGapRateY);
                    }
                    float fDisX = (float)((float)nDisX / 1000);
                    float fDisY = (float)((float)nDisY / 1000);
                    m_pxParticle[nCnt].m_vecOldPos.X = m_pxParticle[nCnt].m_vecPos.X;
                    m_pxParticle[nCnt].m_vecOldPos.Y = m_pxParticle[nCnt].m_vecPos.Y;
                    m_pxParticle[nCnt].m_vecPos.X += fDisX * (float)nLoopTime;
                    m_pxParticle[nCnt].m_vecPos.Y += fDisY * (float)nLoopTime;

                    int nabsX, nabsY;
                    nabsX = (int)Math.Abs(m_pxParticle[nCnt].m_vecDstPos.X - m_pxParticle[nCnt].m_vecPos.X);
                    nabsY = (int)Math.Abs(m_pxParticle[nCnt].m_vecDstPos.Y - m_pxParticle[nCnt].m_vecPos.Y);
                    if ((nabsX <= 10 && nabsY <= 10) || (nabsX >= m_pxParticle[nCnt].m_vecPrevDis.X && nabsY >= m_pxParticle[nCnt].m_vecPrevDis.Y))
                    {
                        //if ( m_pxParticle[nCnt].m_nCurrLife > m_pxParticle[nCnt].m_nLife /*|| nCnt%5*/ )
                        {
                            m_pxParticle[nCnt].Init();
                            m_shPartNum--;
                        }
                        //else
                        {
                            //m_pxParticle[nCnt].m_fSize = (FLOAT)GetRandomNum(10, 50);
                        }
                    }
                    else
                    {
                        m_pxParticle[nCnt].m_vecPrevDis.X = nabsX;
                        m_pxParticle[nCnt].m_vecPrevDis.Y = nabsY;
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
            BlendRate = 12f / 85f;
            //return base.RenderSystem(m_shPartNum, 520, BlendState._BLEND_INVLIGHTINV, LibraryFile.ProgUse);

            int nPartCnt = 0;
            for (int nCnt = 0; nCnt < m_nNum; nCnt++)
            {
                if (!m_pxParticle[nCnt].m_bIsDead)
                {
                    Library.DrawBlend(520 + m_pxParticle[nCnt].m_nCurrFrame, m_pxParticle[nCnt].m_vecPos.X, m_pxParticle[nCnt].m_vecPos.Y, Color.FromArgb(m_pxParticle[nCnt].m_bRed, m_pxParticle[nCnt].m_bGreen, m_pxParticle[nCnt].m_bBlue), m_pxParticle[nCnt].m_fSize, m_pxParticle[nCnt].m_fSize, BlendRate, ImageType.Image, BlendType._BLEND_INVLIGHTINV);
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

        public void SetParticles(Vector3 vecDstPos)
        {
            int nGenCnt = 0;
            for (int nCnt = 0; nCnt < m_nNum; nCnt++)
            {
                if (m_pxParticle[nCnt].m_bIsDead)
                {
                    m_pxParticle[nCnt].m_vecPos = new Vector3(vecDstPos.X + (float)GetRandomNum(-400, 400), vecDstPos.Y + (float)GetRandomNum(-400, 400), 0f);
                    m_pxParticle[nCnt].m_vecDstPos = vecDstPos;
                    m_pxParticle[nCnt].m_nLife = GetRandomNum(2000, 2200);
                    m_pxParticle[nCnt].m_fMass = 1000f + GetRandomFloatNum();
                    m_pxParticle[nCnt].m_fSize = (float)GetRandomNum(30, 100) + GetRandomFloatNum();
                    m_pxParticle[nCnt].m_bIsDead = false;
                    m_pxParticle[nCnt].m_bRed = (m_pxParticle[nCnt].m_bFstRed = (byte)GetRandomNum(100, 125));
                    m_pxParticle[nCnt].m_bGreen = (m_pxParticle[nCnt].m_bFstGreen = (byte)(m_pxParticle[nCnt].m_bFstRed / 2));
                    m_pxParticle[nCnt].m_bBlue = (m_pxParticle[nCnt].m_bFstBlue = (byte)GetRandomNum(0, 15));
                    m_pxParticle[nCnt].m_nDelay = 300;
                    m_pxParticle[nCnt].m_nCurrDelay = 0;
                    m_pxParticle[nCnt].m_nCurrFrame = 0;
                    m_pxParticle[nCnt].m_bOpa = byte.MaxValue;
                    m_shPartNum++;
                    nGenCnt++;
                    if (nGenCnt >= 150)
                    {
                        break;
                    }
                }
            }
        }

    }
}
