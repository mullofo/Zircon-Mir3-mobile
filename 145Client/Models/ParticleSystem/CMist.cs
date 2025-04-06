using Client.Envir;
using Library;
using SharpDX;
using Color = System.Drawing.Color;

namespace Client.Models
{
    public class CMist
    {
        public bool m_bActive;

        public int m_bMistState;

        public Vector3 m_vTrans;

        public Vector3 m_vScale;

        public float BlendRate;

        public MirLibrary Library;

        public float m_fWidth;

        public float m_fGround;

        public float nCnt = 0;

        public CMist()
        {
            m_vTrans.X = 0f;
            m_vTrans.Y = 0f;
            m_vTrans.Z = 0f;
            BlendRate = 0.7f;
            CEnvir.LibraryList.TryGetValue(LibraryFile.ProgUse, out Library);
        }

        ~CMist()
        {
        }

        public bool Create()
        {
            return true;
        }

        public void Init(float fWidth, float fGround)
        {
            m_vTrans.X = 0f;
            m_vTrans.Y = 0f;
            m_vTrans.Z = 0f;
            BlendRate = 0.7f;
            m_fWidth = fWidth;
            m_fGround = fGround;
        }

        public void Destory()
        {
        }

        public void DrawMist()
        {
            m_bMistState++;
            //m_vTrans.X += 1f;
            //m_vTrans.Y += 0.2f;
            //m_vTrans.Z = 0f;
            //m_vTrans.X %= fWidth;
            //m_vTrans.Y %= fGround;

            nCnt += 1.9f * 2f;

            if (nCnt < m_fWidth)
            {
                m_vTrans.X -= 1.9f;
                m_vTrans.Y -= 0.2f;
                m_vTrans.Z = 0;
            }
            else if (nCnt >= m_fWidth && nCnt < m_fWidth * 2)
            {
                m_vTrans.X += 1.9f;
                m_vTrans.Y += 0.2f;
                m_vTrans.Z = 0;
            }
            else
            {
                nCnt = 0;
            }

            Library.DrawBlend(550, (m_vTrans.X - m_fWidth / 2) + m_fWidth / 3f, (m_vTrans.Y + m_fGround / 2) + m_fGround / 3f, Color.FromArgb(100, 200, 200), m_fWidth, m_fGround, BlendRate, ImageType.Image, BlendType._BLEND_LIGHTINV);
            Library.DrawBlend(550, (m_vTrans.X - m_fWidth / 2) + m_fWidth / 3f, (m_vTrans.Y - m_fGround / 2) + m_fGround / 3f, Color.FromArgb(100, 200, 200), m_fWidth, m_fGround, BlendRate, ImageType.Image, BlendType._BLEND_LIGHTINV);
            Library.DrawBlend(550, (m_vTrans.X + m_fWidth / 2) + m_fWidth / 3f, (m_vTrans.Y + m_fGround / 2) + m_fGround / 3f, Color.FromArgb(100, 200, 200), m_fWidth, m_fGround, BlendRate, ImageType.Image, BlendType._BLEND_LIGHTINV);
            Library.DrawBlend(550, (m_vTrans.X + m_fWidth / 2) + m_fWidth / 3f, (m_vTrans.Y - m_fGround / 2) + m_fGround / 3f, Color.FromArgb(100, 200, 200), m_fWidth, m_fGround, BlendRate, ImageType.Image, BlendType._BLEND_LIGHTINV);

        }

        public void ProgressMist()
        {
            DrawMist();
        }
    }
}
