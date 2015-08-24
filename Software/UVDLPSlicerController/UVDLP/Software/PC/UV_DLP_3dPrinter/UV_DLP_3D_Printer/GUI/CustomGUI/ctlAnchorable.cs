using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UV_DLP_3D_Printer.GUI.CustomGUI
{
    public class ctlAnchorable : ctlUserPanel
    {
        public enum AnchorTypes
        {
            None = -1,
            Top = 0,
            Left,
            Center,
            Right,
            Bottom
        }

        protected enum CtlState
        {
            Normal = 0,
            Hover = 1,
            Pressed = 2
        }

        protected CtlState mCtlState;
        AnchorTypes mAnchorHoriz;
        AnchorTypes mAnchorVert;
        bool mChecked;
        protected int mAutorepeatInitial;
        protected int mAutorepeatPeriod;
        protected Timer mAuturepTimer = null;
        protected MouseEventArgs mLastMouseArgs;

       //Control mCtlRefPos;

        // will apear in properties panel

        [Description("Horizontal anchor type"), Category("Data")]
        public AnchorTypes HorizontalAnchor
        {
            get { return mAnchorHoriz; }
            set { mAnchorHoriz = value; }
        }

        [Description("Vertical anchor type"), Category("Data")]
        public AnchorTypes VerticalAnchor
        {
            get { return mAnchorVert; }
            set { mAnchorVert = value; }
        }

        [Description("Check state"), Category("Data")]
        public bool Checked
        {
            get { return mChecked; }
            set { mChecked = value; Invalidate();  }
        }

        [DefaultValue(0)]
        [Description("Autorepeat initial wait (ms) 0 = No repeat"), Category("Data")]
        public virtual int AutorepeatInitial
        {
            get { return mAutorepeatInitial; }
            set { mAutorepeatInitial = value; SetupAutorepeat();  }
        }

        [DefaultValue(100)]
        [Description("Autorepeat period (ms)"), Category("Data")]
        public virtual int AutorepeatPeriod
        {
            get { return mAutorepeatPeriod; }
            set { mAutorepeatPeriod = value; }
        }
        
        public ctlAnchorable()
        {
            mCtlState = CtlState.Normal;
            InitializeComponent();
            mGapx = 5;
            mGapy = 5;
            mAnchorHoriz = AnchorTypes.None;
            mAnchorVert = AnchorTypes.None;
            mAutorepeatInitial = 0;
            mAutorepeatPeriod = 100;
            
        }

        protected void SetupAutorepeat()
        {
            if (mAutorepeatInitial > 0)
            {
                if (mAuturepTimer == null)
                {
                    mAuturepTimer = new Timer();
                    mAuturepTimer.Tick += new EventHandler(mAuturepTimer_Tick);
                }
            }
            else
            {
                if (mAuturepTimer != null)
                {
                    mAuturepTimer.Stop();
                    mAuturepTimer.Dispose();
                    mAuturepTimer = null;
                }
            }
        }

        private void StartAutorepeat(MouseEventArgs eargs)
        {
            if (mAutorepeatInitial == 0)
                return;
            //mCurX = xpos;
            mLastMouseArgs = eargs;
            mAuturepTimer.Interval = mAutorepeatInitial;
            mAuturepTimer.Start();
        }


        void mAuturepTimer_Tick(object sender, EventArgs e)
        {
            mAuturepTimer.Interval = mAutorepeatPeriod;
            OnClick(mLastMouseArgs);
        }

        protected void StopAutorepeat()
        {
            if (mAuturepTimer != null)
                mAuturepTimer.Stop();
        }

        public void SetPositioning(AnchorTypes horiz, AnchorTypes vert, int gapx, int gapy)
        {
            mAnchorHoriz = horiz;
            mAnchorVert = vert;
            mGapx = gapx;
            mGapy = gapy;
            UpdatePosition();
        }

        protected int GetPosition(int refpos, int refwidth, int width, int gap, AnchorTypes anchor)
        {
            int retval = 0;
            switch (anchor)
            {
                case AnchorTypes.Top:
                case AnchorTypes.Left:
                    retval = refpos + gap;
                    break;

                case AnchorTypes.Center:
                    retval = refpos + (refwidth - width) / 2 + gap;
                    break;

                case AnchorTypes.Right:
                case AnchorTypes.Bottom:
                    retval = refpos + refwidth - width - gap;
                    break;
            }
            return retval;
        }


        public void UpdatePosition()
        {
            if ((mAnchorHoriz == AnchorTypes.None) || (mAnchorVert == AnchorTypes.None))
                return;
            if (Parent == null)
                return;
            int x = GetPosition(0, Parent.Width, Width, mGapx, mAnchorHoriz);
            int y = GetPosition(0, Parent.Height, Height, mGapy, mAnchorVert);
            Location = new System.Drawing.Point(x,y);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            mCtlState = CtlState.Hover;
            Invalidate();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            mCtlState = CtlState.Normal;
            StopAutorepeat();
            Invalidate();
            base.OnMouseLeave(e);
        }

        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            if (mevent.Button != System.Windows.Forms.MouseButtons.Left)
                return;
            mCtlState = CtlState.Pressed;
            if ((mAuturepTimer != null) && (!mAuturepTimer.Enabled))
                StartAutorepeat(mevent);
            Invalidate();
            base.OnMouseDown(mevent);
        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            mCtlState = CtlState.Hover;
            StopAutorepeat();
            Invalidate();
            base.OnMouseUp(mevent);
        }

        protected override void OnResize(EventArgs e)
        {
            UpdatePosition();
            base.OnResize(e);
        }

        protected override void OnClick(EventArgs e)
        {
            Checked = !Checked;
            base.OnClick(e);
        }

        private void InitializeComponent()
        {
        }
   }
}
