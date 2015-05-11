using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Engine3D;

namespace UV_DLP_3D_Printer.GUI.CustomGUI
{
    public partial class ctlScene : ctlAnchorable
    {
        bool ignorerefresh;
        public ctlScene()
        {
            InitializeComponent();
            UVDLPApp.Instance().AppEvent += new AppEventDelegate(AppEventDel);
            UpdateSceneTree();
            ignorerefresh = false;
        }
        /// <summary>
        /// this is for a 2-toned foregrtound color
        /// </summary>
        /// <param name="clr"></param>
        public void FixForeColor(Color clr)
        {
            treeScene.ForeColor = clr;
        }
        private void AppEventDel(eAppEvent ev, String Message) 
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(delegate() { AppEventDel(ev, Message); }));
            }
            else
            {
                
                switch (ev) 
                {
                    case eAppEvent.eObjectSelected:
                    case eAppEvent.eModelRemoved: 
                    case eAppEvent.eModelAdded:
                        if (ignorerefresh) return;
                        UpdateSceneTree();
                        break;
                }
                Refresh();
            }
            
        }

        public override void ApplyStyle(GuiControlStyle ct)
        {
            base.ApplyStyle(ct);
            if (ct.ForeColor.IsValid())
            {
                ctlTitle1.ForeColor = ct.ForeColor;
                treeScene.ForeColor = ct.ForeColor;
                treeScene.LineColor = ct.ForeColor;
                cmdCopy.ForeColor = ct.ForeColor;
                cmdDelete.ForeColor = ct.ForeColor;
            }
            if (ct.BackColor.IsValid())
            {
                BackColor = ct.BackColor;
                manipObject.BackColor = ct.BackColor;
                cmdCopy.BackColor = ct.BackColor;
                cmdDelete.BackColor = ct.BackColor;

            }
            if (ct.FrameColor.IsValid())
            {
                treeScene.BackColor = ct.FrameColor;
                //flowLayoutPanel1.BackColor = ct.FrameColor;
                panel1.BackColor = ct.FrameColor;
            }

        }

        #region Scene tree

        public void UpdateSceneTree()
        {
            SetupSceneTree();
        }

	    private TreeNode FindObjectNode(TreeNodeCollection nodes, Object3d sel) 
        {
	        TreeNode found = null;
	        foreach (TreeNode node in nodes) {
	            if (node.Tag == sel) { return node; }
                found = FindObjectNode(node.Nodes, sel);
	            if (found!=null) { break; }
	        }
	        return found;
        }

        private void SetupSceneTree()
        {
            treeScene.Nodes.Clear();//clear the old

            TreeNode scenenode = new TreeNode("Scene");
            treeScene.Nodes.Add(scenenode);
            TreeNode selNode = null;

            foreach (Object3d obj in UVDLPApp.Instance().Engine3D.m_objects)
            {
                
                if(obj.tag == Object3d.OBJ_NORMAL)
                {
                    TreeNode objnode = new TreeNode(obj.Name);
                    objnode.Tag = obj;
                    scenenode.Nodes.Add(objnode);
                    if (obj == UVDLPApp.Instance().SelectedObject)  // expand this node
                    {
                        //objnode.BackColor = Color.LightBlue;
                        //treeScene.SelectedNode = objnode;
                        selNode = objnode;
                    }
                    if (obj.m_supports.Count > 0) //if object has supports, create a node for them
                    {
                        TreeNode supnode = new TreeNode("Supports");
                        objnode.Nodes.Add(supnode);
                        supnode.Collapse();
                        foreach (Object3d sup in obj.m_supports) 
                        {
                            TreeNode tn = new TreeNode(sup.Name);
                            tn.Tag = sup;
                            supnode.Nodes.Add(tn);
                        }
                    }
                    
                }
            }
            if (selNode != null)
                selNode.BackColor = Color.Green;
            scenenode.Expand();
            treeScene.SelectedNode = selNode;
        }

        private void cmdRemoveObject_Click(object sender, EventArgs e)
        {
            // delete the current selected object
            if (UVDLPApp.Instance().SelectedObject != null)
            {
                //if this is a support, remove it from the parent as to not create an orphan
                if (UVDLPApp.Instance().SelectedObject.tag == Object3d.OBJ_SUPPORT || UVDLPApp.Instance().SelectedObject.tag == Object3d.OBJ_SUPPORT_BASE)
                {
                    if (UVDLPApp.Instance().SelectedObject.m_parent != null)
                        UVDLPApp.Instance().SelectedObject.m_parent.RemoveSupport(UVDLPApp.Instance().SelectedObject);
                }
                
                UVDLPApp.Instance().RemoveCurrentModel();

                SetupSceneTree();
            }
        }

        private void cmdRemoveAllSupports_Click(object sender, EventArgs e)
        {
            // remove all supports
            //iterate through objects, remove all supports
            UVDLPApp.Instance().RemoveAllSupports();
            SetupSceneTree();
        }

        /*
          This function does 2 things,
          * when a node is click that is an object node, it sets
          * the App current object to be the clicked object
          * when an obj in the tree view is right clicked, it shows the context menu
          */
        private void treeScene_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            //if (e.Node.Tag != null)            
            if (e.Button == System.Windows.Forms.MouseButtons.Left && e.Node.Tag != null)
            {
                ignorerefresh = true;
                TreeNode tn = FindObjectNode(treeScene.Nodes, UVDLPApp.Instance().SelectedObject); // find the node that's currently selected
                if (tn != null) //set it back to the original color
                    tn.BackColor = treeScene.BackColor;

                UVDLPApp.Instance().SelectedObject = (Object3d)e.Node.Tag;
                e.Node.BackColor = Color.Green; // now set the selected color

               // SetupSceneTree();                
                UVDLPApp.Instance().RaiseAppEvent(eAppEvent.eUpdateSelectedObject, "updateobject");
                ignorerefresh = false;
            }

            if (e.Button == System.Windows.Forms.MouseButtons.Right)  // we right clicked a menu item, check and see if it has a tag
            {
                
                if (e.Node.Tag != null)
                {
                    contextMenuObject.Show(treeScene, e.Node.Bounds.Left, e.Node.Bounds.Top);
                }
                
            }
        }
            #endregion Scene tree

        private void ctlTitle1_Click(object sender, EventArgs e)
        {
            if (ctlTitle1.Checked)
            {
                this.Height = manipObject.Height;
            }
            else
            {
                this.Height = ctlTitle1.Height + 5;
            }
        }

        private void ctlScene_Resize(object sender, EventArgs e)
        {
            manipObject.Width = manipObject.Parent.Width;
            ctlTitle1.Width = ctlTitle1.Parent.Width - 6;
            //flowLayoutPanel1.Width = flowLayoutPanel1.Parent.Width - 6;
            panel1.Width = panel1.Parent.Width - 6;
            treeScene.Width = treeScene.Parent.Width - 6;
        }

        private void cmdDelete_Click(object sender, EventArgs e)
        {
            cmdRemoveObject_Click(null, null);
        }

        private void cmdCopy_Click(object sender, EventArgs e)
        {
            //copy the object
            if (UVDLPApp.Instance().SelectedObject != null)
            {
                try
                {
                    Object3d obj = UVDLPApp.Instance().SelectedObject.Copy();
                    foreach (Object3d sup in obj.m_supports) 
                    {
                        UVDLPApp.Instance().m_engine3d.AddObject(sup);
                    }
                    UVDLPApp.Instance().m_engine3d.AddObject(obj);
                    UVDLPApp.Instance().m_undoer.SaveAddition(obj);
                    UVDLPApp.Instance().RaiseAppEvent(eAppEvent.eModelAdded, "Model Created");
                    SetupSceneTree();
                }
                catch (Exception ex) 
                {
                    DebugLogger.Instance().LogError(ex);
                }
            }
        }

        private void cmdDelete_Load(object sender, EventArgs e)
        {

        }

        private void cmdNewScene_Click(object sender, EventArgs e)
        {
            UVDLPApp.Instance().Engine3D.RemoveAllObjects();
            SetupSceneTree();
            //refresh the 3d view
            UVDLPApp.Instance().RaiseAppEvent(eAppEvent.eModelRemoved, "");
        }

        private void panel1_Resize(object sender, EventArgs e)
        {
            cmdNewScene.SetPositioning(AnchorTypes.Right, AnchorTypes.Center, 4, 0);
        }

        public override void RegisterSubControls(string parentName)
        {
            UVDLPApp.Instance().m_gui_config.AddButton(parentName + ".title", ctlTitle1);
        }
       

    }
}
