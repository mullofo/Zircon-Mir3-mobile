using Client.Envir;
using Library;
using SharpDX;
using System;

namespace Client.Models
{
    public class CParticleSystem
    {
        protected Vector3[] m_vecBounding = new Vector3[2];

        protected int m_nNum;

        protected CParticle[] m_pxParticle;

        protected float m_fDeltaTime;

        public MirLibrary Library;

        public float Opacity = 1f;

        public float BlendRate = 0.7f;

        public Vector3 m_vecEnvironment;

        public float m_fAirFiction;

        public int GetRandomNum(int nFrom, int nTo)
        {
            //return CEnvir.Random.Next(nFrom, nTo);
            int nRand = CEnvir.Random.Next(0, Int16.MaxValue) - CEnvir.Random.Next(0, Int16.MaxValue);

            if (nRand < 0)
                nRand = -nRand;

            int nRandNum = nRand % (nTo - nFrom + 1) + nFrom;

            return nRandNum;
        }

        public float GetRandomFloatNum()
        {
            return (CEnvir.Random.Next(0, Int16.MaxValue) % 1000) / 1000.0f;
        }

        public CParticleSystem()
        {
            InitSystem();
        }

        ~CParticleSystem()
        {
            DestroySystem();
        }

        public virtual void InitSystem()
        {
            m_nNum = 0;
            m_pxParticle = null;
            m_fDeltaTime = 0.02f;
            m_fAirFiction = -0.05f;
            m_vecBounding[0] = new Vector3(0f, 0f, 0f);
            m_vecBounding[1] = new Vector3(0f, 0f, 0f);
            m_vecEnvironment = new Vector3(0f, 300f, 0f);
        }

        public virtual void SetupSystem(int wCnt = 200)
        {
            InitSystem();
            m_nNum = wCnt;
            m_pxParticle = new CParticle[m_nNum];
            for (int i = 0; i < m_nNum; i++)
            {
                m_pxParticle[i] = new CParticle();
                m_pxParticle[i].Init();
            }
        }

        public virtual void DestroySystem()
        {
            for (int i = 0; i < m_nNum; i++)
            {
                m_pxParticle[i] = null;
            }

            InitSystem();
        }

        public void UpdateAirFiction(int nNum)
        {
            if (!m_pxParticle[nNum].m_bIsDead)
            {
                m_pxParticle[nNum].m_vecLocalForce.X = (0f - m_pxParticle[nNum].m_vecVel.X) * m_fAirFiction;
                m_pxParticle[nNum].m_vecLocalForce.Y = (0f - m_pxParticle[nNum].m_vecVel.Y) * m_fAirFiction;
                m_pxParticle[nNum].m_vecLocalForce.Z = m_pxParticle[nNum].m_vecVel.Z * m_fAirFiction;
                //System.Diagnostics.Debug.WriteLine("m_vecVel.Y= " + (0f - m_pxParticle[nNum].m_vecVel.Y) + ", m_fAirFiction" + m_fAirFiction);
            }
        }

        public void UpdateMove(int nNum)
        {
            if (!m_pxParticle[nNum].m_bIsDead)
            {
                if (m_pxParticle[nNum].m_fMass == 0f)
                {
                    m_pxParticle[nNum].m_fMass = 1f;
                }
                m_pxParticle[nNum].m_vecAccel.X += (m_vecEnvironment.X + m_pxParticle[nNum].m_vecLocalForce.X) / m_pxParticle[nNum].m_fMass;
                m_pxParticle[nNum].m_vecAccel.Y += (m_vecEnvironment.Y + m_pxParticle[nNum].m_vecLocalForce.Y) / m_pxParticle[nNum].m_fMass;
                m_pxParticle[nNum].m_vecAccel.Z += (m_vecEnvironment.Z + m_pxParticle[nNum].m_vecLocalForce.Z) / m_pxParticle[nNum].m_fMass;
                m_pxParticle[nNum].m_vecVel.X += m_pxParticle[nNum].m_vecAccel.X * m_fDeltaTime;
                m_pxParticle[nNum].m_vecVel.Y += m_pxParticle[nNum].m_vecAccel.Y * m_fDeltaTime;
                m_pxParticle[nNum].m_vecVel.Z += m_pxParticle[nNum].m_vecAccel.Z * m_fDeltaTime;
                m_pxParticle[nNum].m_vecOldPos = m_pxParticle[nNum].m_vecPos;
                m_pxParticle[nNum].m_vecPos.X += m_pxParticle[nNum].m_vecVel.X * m_fDeltaTime;
                m_pxParticle[nNum].m_vecPos.Y += m_pxParticle[nNum].m_vecVel.Y * m_fDeltaTime;
                m_pxParticle[nNum].m_vecPos.Z += m_pxParticle[nNum].m_vecVel.Z * m_fDeltaTime;

                //System.Diagnostics.Debug.WriteLine("m_vecLocalForce.Y= " + m_pxParticle[nNum].m_vecLocalForce.Y + ", m_fMass" + m_pxParticle[nNum].m_fMass);
            }
        }

        public void UpdateCrash(int nNum)
        {
            if (m_pxParticle[nNum].m_bIsDead)
            {
                return;
            }
            if (m_pxParticle[nNum].m_vecPos.X <= m_vecBounding[0].X || m_pxParticle[nNum].m_vecPos.X >= m_vecBounding[1].X)
            {
                m_pxParticle[nNum].m_vecVel.X = (0f - m_pxParticle[nNum].m_vecVel.X) * 0.7f;
            }
            if (m_pxParticle[nNum].m_vecPos.Y <= m_vecBounding[0].Y || m_pxParticle[nNum].m_vecPos.Y >= m_vecBounding[1].Y)
            {
                float fOldX = m_pxParticle[nNum].m_vecPos.X - m_pxParticle[nNum].m_vecVel.X * (float)m_pxParticle[nNum].m_nDelay;
                float fOldY = m_pxParticle[nNum].m_vecPos.Y - m_pxParticle[nNum].m_vecVel.Y * (float)m_pxParticle[nNum].m_nDelay;
                float fBefore = 0f;
                float fAfter = 0f;
                if (m_pxParticle[nNum].m_vecPos.Y - fOldY != 0f)
                {
                    fBefore = (float)m_pxParticle[nNum].m_nDelay * (m_vecBounding[1].Y - fOldY) / (m_pxParticle[nNum].m_vecPos.Y - fOldY);
                }
                if (m_pxParticle[nNum].m_vecPos.Y - fOldY != 0f)
                {
                    fAfter = (float)m_pxParticle[nNum].m_nDelay * (m_pxParticle[nNum].m_vecPos.Y - m_vecBounding[1].Y) / (m_pxParticle[nNum].m_vecPos.Y - fOldY);
                }
                m_pxParticle[nNum].m_vecPos.X = fOldX + m_pxParticle[nNum].m_vecVel.X * fBefore;
                m_pxParticle[nNum].m_vecPos.Y = fOldY + m_pxParticle[nNum].m_vecVel.Y * fBefore;
                m_pxParticle[nNum].m_vecVel.Y = (0f - m_pxParticle[nNum].m_vecVel.Y) * 0.6f;
                m_pxParticle[nNum].m_vecPos.X += m_pxParticle[nNum].m_vecVel.X * fAfter;
                m_pxParticle[nNum].m_vecPos.Y += m_pxParticle[nNum].m_vecVel.Y * fAfter;
            }
            if (m_pxParticle[nNum].m_vecPos.Z <= m_vecBounding[0].Z || m_pxParticle[nNum].m_vecPos.Z >= m_vecBounding[1].Y)
            {
                m_pxParticle[nNum].m_vecVel.Z = (0f - m_pxParticle[nNum].m_vecVel.Z) * 0.6f;
            }
        }

        public void SetEnvFactor(float fAirFriction, Vector3 vecEnvironment, Vector3 vecMinBound, Vector3 vecMaxBound)
        {
            m_fAirFiction = fAirFriction;
            m_vecEnvironment = vecEnvironment;
            m_vecBounding[0] = vecMinBound;
            m_vecBounding[1] = vecMaxBound;
        }

        public virtual bool RenderSystem(int m_shPartNum, int FramIdx, BlendType RendState = BlendType.NONE, LibraryFile libraryFile = LibraryFile.ProgUse)
        {
            //int nPartCnt = 0;
            //CEnvir.LibraryList.TryGetValue(libraryFile, out Library);
            //for (int nCnt = 0; nCnt < m_nNum; nCnt++)
            //{
            //	if (!m_pxParticle[nCnt].m_bIsDead)
            //	{
            //		Library.DrawBlend(FramIdx+ m_pxParticle[nCnt].m_nCurrFrame, m_pxParticle[nCnt].m_vecPos.X, m_pxParticle[nCnt].m_vecPos.Y, Color.FromArgb(m_pxParticle[nCnt].m_bRed, m_pxParticle[nCnt].m_bGreen, m_pxParticle[nCnt].m_bBlue), m_pxParticle[nCnt].m_fSize, m_pxParticle[nCnt].m_fSize, BlendRate, ImageType.Image, RendState);
            //      //System.Diagnostics.Debug.WriteLine("X = " + m_pxParticle[i].m_vecPos.X + ", Y = " + m_pxParticle[i].m_vecPos.Y);
            //		nPartCnt++;
            //	}
            //	if (nPartCnt >= m_shPartNum)
            //	{
            //		return true;
            //	}
            //}
            return true;
        }

        public virtual void UpdateSystem(int nLoopTime, Vector3 vecGenPos)
        {
        }

        public virtual void SetParticleDefault(int nNum, Vector3 vecGenPos)
        {
        }

    }
}
