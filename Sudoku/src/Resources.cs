using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Resources
{
    public struct ResourcePack
    {
        public Dictionary<string, Texture2D> Textures = new();
        public Dictionary<string, Sound> Audio = new();
        public Dictionary<string, string> YAMLs = new();
        public Dictionary<string, string> Fonts = new();

        public ResourcePack()
        {

        }
    }

    public class Resources
    {
        private bool m_RecentlyLoaded = false;
        private ResourcePack m_Resources = new();

        public ResourcePack Get => m_Resources;

        public void Reset()
        {
            foreach(var slice in m_Resources.Textures)
            {
                Raylib.UnloadTexture(m_Resources.Textures[slice.Key]);
            }
            m_Resources.Textures.Clear();
            foreach (var slice in m_Resources.Audio)
            {
                Raylib.UnloadSound(m_Resources.Audio[slice.Key]);
            }
            m_Resources.Audio.Clear();
            m_Resources.YAMLs.Clear();
            m_Resources.Fonts.Clear();
        }
        public void Load(string resourcePack)
        {
            Reset();
            foreach (var file in Directory.EnumerateFiles($"./Resources/{resourcePack}", "*.*", SearchOption.AllDirectories))
            {
                string fileExtension = Path.GetExtension(file).ToLower();
                string fileName = Path.GetFileName(file).ToLower();
                switch (fileExtension.ToLower())
                {
                    case ".png":
                        m_Resources.Textures.Add(fileName, Raylib.LoadTexture(file));
                        break;
                    case ".ogg":
                        m_Resources.Audio.Add(fileName, Raylib.LoadSound(file));
                        break;
                    case ".ttf":
                        m_Resources.Fonts.Add(fileName, file);
                        break;
                    case ".yml" or ".yaml":
                        m_Resources.YAMLs.Add(fileName, file);
                        break;
                }
            }
            m_RecentlyLoaded = true;
        }

        //public ResourcePack Get => ResourcePack;
        public bool ShouldReload => m_RecentlyLoaded;


        public void Set_bRecentlyLoaded_to_False()
        {
            m_RecentlyLoaded = false;
        }
    }
}
