using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform.Windows;

namespace Engine3D
{
    /// <summary>
    /// This class isn't used yet, but it will be...
    /// </summary>
    public struct TexCoord
    {
        public float U;
        public float V;

        public TexCoord(float u, float v)
        {
            U = u;
            V = v;
        }
    }
    public class Material
    {
        // Set Default values
        public float[] Ambient = new float[] { 0.5f, 0.5f, 0.5f };
        public float[] Diffuse = new float[] { 0.5f, 0.5f, 0.5f };
        public float[] Specular = new float[] { 0.5f, 0.5f, 0.5f };
        public int Shininess = 50;

        int textureid = -1;
        public int TextureId
        {
            get
            {
                return textureid;
            }
        }

    }
}
