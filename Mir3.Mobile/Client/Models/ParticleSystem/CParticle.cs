using Microsoft.Xna.Framework;

namespace Client.Models
{
    public class CParticle
    {
        public int m_nLife;

        public int m_nCurrLife;

        public Vector3 m_vecPos;

        public Vector3 m_vecOldPos;

        public Vector3 m_vecDstPos;

        public Vector3 m_vecVel;

        public Vector3 m_vecAccel;

        public Vector3 m_vecLocalForce;

        public Vector3 m_vecPrevDis;

        public float m_fMass;

        public float m_fSize;

        public bool m_bIsDead;

        public byte m_bRed;

        public byte m_bGreen;

        public byte m_bBlue;

        public byte m_bOpa;

        public float m_fOriSize;

        public byte m_bFstRed;

        public byte m_bFstGreen;

        public byte m_bFstBlue;

        public byte m_bEndRed;

        public byte m_bEndGreen;

        public byte m_bEndBlue;

        public int m_nDelay;

        public int m_nCurrDelay;

        public int m_nCurrFrame;

        public CParticle()
        {
            Init();
        }

        public void Init()
        {
            m_nLife = 0;
            m_vecPos = new Vector3(0f, 0f, 0f);
            m_vecOldPos = new Vector3(0f, 0f, 0f);
            m_vecVel = new Vector3(0f, 0f, 0f);
            m_vecAccel = new Vector3(0f, 0f, 0f);
            m_vecLocalForce = new Vector3(0f, 0f, 0f);
            m_vecDstPos = new Vector3(0f, 0f, 0f);
            m_vecPrevDis = new Vector3(10000f, 10000f, 10000f);
            m_fMass = 0f;
            m_fSize = 0f;
            m_fOriSize = 0f;
            m_bIsDead = true;
            m_bBlue = byte.MaxValue;
            m_bGreen = byte.MaxValue;
            m_bRed = byte.MaxValue;
            m_bFstBlue = byte.MaxValue;
            m_bFstGreen = byte.MaxValue;
            m_bFstRed = byte.MaxValue;
            m_bEndBlue = byte.MaxValue;
            m_bEndGreen = byte.MaxValue;
            m_bEndRed = byte.MaxValue;
            m_nDelay = 0;
            m_nCurrDelay = 0;
            m_nCurrFrame = 0;
            m_nCurrLife = 0;
            m_bOpa = byte.MaxValue;
        }

        public void ZeroInit()
        {
            m_nLife = 0;
            m_vecPos = new Vector3(0f, 0f, 0f);
            m_vecOldPos = new Vector3(0f, 0f, 0f);
            m_vecVel = new Vector3(0f, 0f, 0f);
            m_vecAccel = new Vector3(0f, 0f, 0f);
            m_vecLocalForce = new Vector3(0f, 0f, 0f);
            m_vecDstPos = new Vector3(0f, 0f, 0f);
            m_vecPrevDis = new Vector3(0f, 0f, 0f);
            m_fMass = 0f;
            m_fSize = 0f;
            m_fOriSize = 0f;
            m_bIsDead = false;
            m_bBlue = 0;
            m_bGreen = 0;
            m_bRed = 0;
            m_bFstBlue = 0;
            m_bFstGreen = 0;
            m_bFstRed = 0;
            m_bEndBlue = 0;
            m_bEndGreen = 0;
            m_bEndRed = 0;
            m_nDelay = 0;
            m_nCurrDelay = 0;
            m_nCurrFrame = 0;
            m_nCurrLife = 0;
            m_bOpa = 0;
        }
    }
}
