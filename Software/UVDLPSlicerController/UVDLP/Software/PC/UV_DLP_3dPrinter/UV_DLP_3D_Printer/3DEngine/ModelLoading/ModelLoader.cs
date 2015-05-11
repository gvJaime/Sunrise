using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Engine3D
{
    class ModelLoader
    {
        protected List<ModelLoaderType> m_loaders = null;

        public ModelLoader()
        {
            m_loaders = new List<ModelLoaderType>();
            m_loaders.Add(new ModelLoaderAmf());
        }

        public List<Object3d> Load(string filename)
        {
            string ext = Path.GetExtension(filename).ToLower();
            // special loaders
            foreach (ModelLoaderType loader in m_loaders)
            {
                if (ext == loader.FileExtension)
                    return loader.LoadModel(filename);
            }

            // built in loaders
            Object3d obj = new Object3d();
            bool ret = false;
            if (ext.Equals(".dxf"))
            {
                ret = obj.LoadDXF(filename);
            }
            if (ext.Equals(".stl"))
            {
                ret = obj.LoadSTL(filename);
            }
            if (ext.Equals(".obj"))
            {
                ret = obj.LoadObjFile(filename);
            }
            if (ext.Equals(".3ds"))
            {
                ret = obj.Load3ds(filename);
            }
            if (ret == true)
            {
                List<Object3d> objlist = new List<Object3d>();
                objlist.Add(obj);
                return objlist;
            }

            return null;
        }
    }
}
