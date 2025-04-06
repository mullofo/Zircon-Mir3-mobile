using System.Collections.Generic;
using System;
using System.Drawing;
using Client.Envir;
using SharpDX;
using Color = System.Drawing.Color;
using Point = System.Drawing.Point;

namespace Client.Models.ParticleEngine
{
    /// <summary>
    /// 天气类型
    /// </summary>
    public enum ParticleType
    {
        /// <summary>
        /// 没有天气效果
        /// </summary>
        None = 0,
        /// <summary>
        /// 雾
        /// </summary>
        Fog,
        /// <summary>
        /// 燃烧的雾
        /// </summary>
        BurningFog,
        /// <summary>
        /// 燃烧雾的灰烬
        /// </summary>
        BurningFogEmber,
        /// <summary>
        /// 雪
        /// </summary>
        Snow,
        /// <summary>
        /// 花瓣雨
        /// </summary>
        Everfall,
        /// <summary>
        /// 雨
        /// </summary>
        Rain,
    }

    public class ParticleEngine
    {    
        public Vector2 EmitterLocation { get; set; }
        protected List<Particle> particles;
        protected List<ParticleImageInfo> Textures;
        public Vector2 ForceVelocity = Vector2.Zero;
        public bool GenerateParticles;
        public DateTime NextParticleTime;

        public ParticleEngine(List<ParticleImageInfo> textures, Vector2 location)
        {
            EmitterLocation = location;
            Textures = textures;
            particles = new List<Particle>();
            GenerateParticles = true;
        }

        public virtual Particle GenerateNewParticle(ParticleType type)  //生成粒子
        {
            Particle particle = null;
            switch (type)
            {
                case ParticleType.None:                    
                    particle = new Particle()
                    {
                        Engine = this,
                        ImageInfo = Textures[CEnvir.Random.Next(Textures.Count)],
                        Color = Color.FromArgb(CEnvir.Random.Next(255), CEnvir.Random.Next(255), CEnvir.Random.Next(255)),
                        Size = (float)CEnvir.Random.NextDouble(),
                        AliveTime = CEnvir.Now.AddSeconds(1 + CEnvir.Random.Next(2)),
                        Blend = true,
                        BlendRate = 1.0F,
                    };                    

                    particles.Add(particle);
                    break;

                case ParticleType.Fog:   //雾
                    particle = new FogParticle(this, Textures[CEnvir.Random.Next(Textures.Count)])
                    {
                        Color = Color.White,
                        Size = 1F,
                        AliveTime = DateTime.MaxValue,
                        BlendRate = 0.4F,
                    };

                    particles.Add(particle);
                    break;

                case ParticleType.BurningFog:  //燃烧的雾
                    particle = new FogParticle(this, Textures[CEnvir.Random.Next(Textures.Count)])
                    {
                        Color = Color.FromArgb(255, 100, 0, 0),
                        Size = 1F,
                        AliveTime = DateTime.MaxValue,
                        BlendRate = 0.5F,
                    };

                    particles.Add(particle);
                    break;

                case ParticleType.BurningFogEmber:  //燃烧的雾的灰烬
                    particle = new Particle()
                    {
                        Engine = this,
                        ImageInfo = Textures[CEnvir.Random.Next(Textures.Count)],
                        Color = Color.DarkRed,
                        Size = (float)CEnvir.Random.NextDouble(),
                        AliveTime = CEnvir.Now.AddSeconds(1 + CEnvir.Random.Next(2)),
                        Blend = true,
                        BlendRate = 0.35F,
                        Position = new Vector2(CEnvir.Random.Next(Config.GameSize.Width), CEnvir.Random.Next(Config.GameSize.Height / 2, Config.GameSize.Height)),
                        Velocity = new Vector2(0, -2F * CEnvir.Random.Next(3)),
                    };

                    particles.Add(particle);
                    break;

                case ParticleType.Snow:   //雪
                    particle = new FogParticle(this, Textures[CEnvir.Random.Next(Textures.Count)])
                    {
                        Color = Color.White,   //颜色
                        Size = 1F,         //尺寸
                        AliveTime = DateTime.MaxValue,  //持续时间
                        BlendRate = 0.9F,   //混合率
                    };

                    particles.Add(particle);
                    break;

                case ParticleType.Everfall:  //花瓣雨
                    particle = new FogParticle(this, Textures[CEnvir.Random.Next(Textures.Count)])
                    {
                        Color = Color.White,
                        Size = 1F,
                        AliveTime = DateTime.MaxValue,
                        BlendRate = 0.9F,
                    };

                    particles.Add(particle);
                    break;

                case ParticleType.Rain:  //雨
                    particle = new FogParticle(this, Textures[CEnvir.Random.Next(Textures.Count)])
                    {
                        Color = Color.Ivory,
                        Size = 1F,
                        AliveTime = DateTime.MaxValue,
                        BlendRate = 0.8F,
                    };

                    particles.Add(particle);
                    break;
            }

            return particle;
        }

        protected Particle FindParticleFromLocation(Vector2 positon)
        {
            foreach (Particle particle in particles)
            {
                if (particle.Position == positon)
                    return particle;
            }

            return null;
        }

        public virtual void Update()
        {
            if (GenerateParticles && CEnvir.Now > NextParticleTime)
            {
                NextParticleTime = CEnvir.Now.AddMilliseconds(20);
                GenerateNewParticle(ParticleType.BurningFogEmber);
            }

            for (int particle = 0; particle < particles.Count; particle++)
            {                
                particles[particle].Update();
                if (CEnvir.Now > particles[particle].AliveTime)
                {
                    particles[particle].OnParticleEnd();
                    particles.RemoveAt(particle);
                    particle--;
                }
            }
        }

        public virtual void Draw()
        {
            for (int index = 0; index < particles.Count; index++)
                particles[index].Draw();
        }

        public void ParticlesOffSet(Point offset)
        {
            for (int particle = 0; particle < particles.Count; particle++)
                particles[particle].Position += new Vector2(offset.X, offset.Y);
        }

        public void Dispose()
        {
            for (int i = particles.Count - 1; i > 0; i--)
                particles.RemoveAt(i);
            particles = null;

            for (int i = Textures.Count - 1; i > 0; i--)
                Textures.RemoveAt(i);
            Textures = null;

            EmitterLocation = Vector2.Zero;
            ForceVelocity = Vector2.Zero;
        }


    }
}
