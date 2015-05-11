using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UV_DLP_3D_Printer.GUI.CustomGUI
{
    public partial class ctlCollapse : ctlUserPanel
    {
        int openedSizeW, openedSiseH;
        Control pnlInner;
        bool adjustingSize;
        int vscrollWidth;
        public ctlCollapse()
        {
            InitializeComponent();
            buttCollapse.Checked = false;
            pnlInner = null;
            adjustingSize = false;
            vscrollWidth = 20;
            CalcSizes();
        }

        public Control InnerControl
        {
            get { return pnlInner; }
            set
            {
                if (pnlInner != null)
                    Controls.Remove(pnlInner);
                pnlInner = value;
                if (pnlInner != null)
                {
                    pnlInner.Dock = DockStyle.Fill;
                    Controls.Add(pnlInner);
                    pnlInner.BringToFront();
                    pnlInner.ClientSizeChanged += new EventHandler(pnlInner_ClientSizeChanged);
                }
                CalcSizes();
                AdjustSize();
            }
        }

        void pnlInner_ClientSizeChanged(object sender, EventArgs e)
        {
            AdjustSize();
        }

        void CalcSizes()
        {
            openedSizeW = buttCollapse.Width;
            openedSiseH = buttCollapse.Height;
            if (pnlInner == null)
                return;
            foreach (Control ctl in pnlInner.Controls)
            {
                if (openedSizeW < ctl.Width)
                    openedSizeW = ctl.Width;
                if (openedSiseH < ctl.Height)
                    openedSiseH = ctl.Height;
            }
        }

        public override System.Windows.Forms.DockStyle Dock
        {
            get
            {
                return base.Dock;
            }
            set
            {
                base.Dock = value;
                if ((value == DockStyle.Left) || (value == DockStyle.Right))
                {
                    pnlTitle.Height = buttCollapse.Height;
                    pnlTitle.Dock = DockStyle.Top;
                }
                else if ((value == DockStyle.Top) || (value == DockStyle.Bottom))
                {
                    pnlTitle.Width = buttCollapse.Width;
                    pnlTitle.Dock = DockStyle.Left;
                }
                
            }
        }

        public bool Collapsed
        {
            get
            {
                return !buttCollapse.Checked;
            }
            set
            {
                buttCollapse.Checked = !value;
                AdjustSize();
            }
        }

        void AdjustSize()
        {
            if (adjustingSize) // eliminate recursion
                return;
            if ((Dock == DockStyle.Left) || (Dock == DockStyle.Right))
            {
                int w = openedSizeW + 6;
                if (Collapsed)
                    w = buttCollapse.Width;
                if (InnerControl is ScrollableControl)
                {
                    if (InnerControl.Width > InnerControl.ClientSize.Width)
                        vscrollWidth = InnerControl.Width - InnerControl.ClientSize.Width;
                    if (((ScrollableControl)InnerControl).VerticalScroll.Visible)
                        w += vscrollWidth;
                }
                adjustingSize = true;
                if (w != Width)
                    Width = w;
                adjustingSize = false;
            }
            else if ((Dock == DockStyle.Top) || (Dock == DockStyle.Bottom))
            {
                if (Collapsed)
                    Height = buttCollapse.Height;
                else
                    Height = openedSiseH;
            }
            if (pnlInner != null)
                pnlInner.Visible = ! Collapsed;
        }

        public override void ApplyStyle(GuiControlStyle ct)
        {
            base.ApplyStyle(ct);
            if (ct.BackColor.IsValid())
                this.BackColor = ct.BackColor;
            if (pnlInner != null)
                pnlInner.BackColor = this.BackColor;
            pnlTitle.BackColor = this.BackColor;
        }

        public void AddControl(Control ctl)
        {
            pnlInner.Controls.Add(ctl);
            CalcSizes();
            AdjustSize();
        }

        private void buttCollapse_Click(object sender, EventArgs e)
        {
            AdjustSize();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            Invalidate();
        }

    }
}
