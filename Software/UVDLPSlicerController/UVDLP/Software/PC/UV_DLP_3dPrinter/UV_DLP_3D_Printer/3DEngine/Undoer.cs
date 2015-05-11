using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UV_DLP_3D_Printer;
using UV_DLP_3D_Printer.GUI.CustomGUI;

namespace Engine3D
{
    public class Undoer
    {
        protected enum eOperationType
        {
            Translate = 0,
            Scale,
            Rotate,
            Add,
            Del
        }

        protected class UndoItem
        {
            public Object3d obj;
            public eOperationType opType;
            public double x, y, z;
            public bool linkedToPrev;
        }

        List<UndoItem> m_undoItemList;
        ctlImageButton m_undoButt = null;
        ctlImageButton m_redoButt = null;
        int m_undopointer;
        public Control parentControl;

        public Undoer()
        {
            m_undoItemList = new List<UndoItem>();
            m_undopointer = 0;
            //RegisterCallback();
        }
        public void RegisterCallback() 
        {
            //undoButt_Click
            //UVDLPApp.Instance().m_callbackhandler.RegisterCallback("undo", undoButt_Click, "undo");
            UVDLPApp.Instance().m_callbackhandler.RegisterCallback("undo", undoButt_Click, null, "Undo");
            UVDLPApp.Instance().m_callbackhandler.RegisterCallback("redo", redoButt_Click, null,"Redo");
        }


        protected void AddItem(eOperationType type, Object3d obj, double x, double y, double z)
        {
            if (obj == null)
                return;
            while (m_undopointer < m_undoItemList.Count)
            {
                m_undoItemList.RemoveAt(m_undopointer);
            }
            UndoItem item = new UndoItem();
            item.opType = type;
            item.obj = obj;
            item.x = x;
            item.y = y;
            item.z = z;
            item.linkedToPrev = false;
            m_undoItemList.Add(item);
            m_undopointer++;
            UpdateButtons();
        }

        public void SaveTranslation(Object3d obj, double x, double y, double z)
        {
            if ((x == 0) && (y == 0) && (z == 0))
                return;
            AddItem(eOperationType.Translate, obj, x, y, z);
        }

        public void SaveRotation(Object3d obj, double x, double y, double z)
        {
            if ((x == 0) && (y == 0) && (z == 0))
                return;
            AddItem(eOperationType.Rotate, obj, x, y, z);
        }

        public void SaveScale(Object3d obj, double x, double y, double z)
        {
            if ((x == 1) && (y == 1) && (z == 1))
                return;
            if ((x == 0) || (y == 0) || (z == 0))
                return;

            AddItem(eOperationType.Scale, obj, x, y, z);
        }

        public void SaveAddition(Object3d obj)
        {
            AddItem(eOperationType.Add, obj, 0, 0, 0);
        }

        public void SaveDelition(Object3d obj)
        {
            AddItem(eOperationType.Del, obj, 0, 0, 0);
        }

        // link last action to the one before. all linked actions will undo together
        public void LinkToPrev()
        {
            if (m_undopointer > 0)
                m_undoItemList[m_undopointer - 1].linkedToPrev = true;
        }

        public void Undo()
        {
            while (m_undopointer > 0)
            {
                UndoItem item = m_undoItemList[m_undopointer - 1];
                m_undopointer--;
                switch (item.opType)
                {
                    case eOperationType.Translate:
                        item.obj.Translate(-(float)item.x, -(float)item.y, -(float)item.z, false);
                        break;

                    case eOperationType.Rotate:
                        item.obj.Rotate(-(float)item.x, -(float)item.y, -(float)item.z);
                        break;

                    case eOperationType.Scale:
                        item.obj.Scale((float)(1.0 / item.x), (float)(1.0 / item.y), (float)(1.0 / item.z));
                        break;

                    case eOperationType.Add:
                        UVDLPApp.Instance().m_engine3d.RemoveObject(item.obj);
                        break;

                    case eOperationType.Del:
                        UVDLPApp.Instance().m_engine3d.AddObject(item.obj);
                        break;

                }
                item.obj.Update();
                if (item.linkedToPrev == false)
                    break;
            }
            UVDLPApp.Instance().RaiseAppEvent(eAppEvent.eUpdateSelectedObject, "updateobject");
            UpdateButtons();
        }

        public void Redo()
        {
            while (m_undopointer < m_undoItemList.Count)
            {
                UndoItem item = m_undoItemList[m_undopointer];
                m_undopointer++;
                switch (item.opType)
                {
                    case eOperationType.Translate:
                        item.obj.Translate((float)item.x, (float)item.y, (float)item.z);
                        break;

                    case eOperationType.Rotate:
                        item.obj.Rotate((float)item.x, (float)item.y, (float)item.z);
                        break;

                    case eOperationType.Scale:
                        item.obj.Scale((float)item.x, (float)item.y, (float)item.z);
                        break;

                    case eOperationType.Add:
                        UVDLPApp.Instance().m_engine3d.AddObject(item.obj);
                        break;

                    case eOperationType.Del:
                        UVDLPApp.Instance().m_engine3d.RemoveObject(item.obj);
                        break;

                }
                item.obj.Update();
                if ((m_undopointer < m_undoItemList.Count)
                    && (m_undoItemList[m_undopointer].linkedToPrev == false))
                    break;
            }
            UVDLPApp.Instance().RaiseAppEvent(eAppEvent.eUpdateSelectedObject, "updateobject");
            UpdateButtons();
        }

        public void AsociateUndoButton(ctlImageButton butt)
        {
            m_undoButt = butt;
            m_undoButt.Click += new EventHandler(m_undoButt_Click);
            UpdateButtons();
        }

        public void AsociateRedoButton(ctlImageButton butt)
        {
            m_redoButt = butt;
            m_redoButt.Click += new EventHandler(m_redoButt_Click);
            UpdateButtons();
        }

        protected void UpdateButtons()
        {
            if (parentControl.InvokeRequired)
            {
                parentControl.BeginInvoke(new MethodInvoker(delegate() { UpdateButtons(); }));
            }
            else
            {
                if (m_undoButt != null)
                {
                    m_undoButt.Enabled = m_undopointer != 0;
                }
                if (m_redoButt != null)
                {
                    m_redoButt.Enabled = m_undopointer < m_undoItemList.Count;
                }
            }
        }

        void undoButt_Click(object sender, object e)
        {
            Undo();
        }
        void redoButt_Click(object sender, object e)
        {
            Redo();
        }
        void m_undoButt_Click(object sender, EventArgs e)
        {
            Undo();
        }

        void m_redoButt_Click(object sender, EventArgs e)
        {
            Redo();
        }

        public bool isEmpty()
        {
            return m_undoItemList.Count == 0;
        }


    }
}
