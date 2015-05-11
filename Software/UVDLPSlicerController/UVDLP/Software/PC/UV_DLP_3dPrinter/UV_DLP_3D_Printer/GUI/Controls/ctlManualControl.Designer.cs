namespace UV_DLP_3D_Printer.GUI.Controls
{
    partial class ctlManualControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.flowTop = new System.Windows.Forms.FlowLayoutPanel();
            this.cMCXY = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlMCXY();
            this.cMCZ = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlMCZ();
            this.cMCExtruder = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlMCZ();
            this.cMCTilt = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlMCZ();
            this.cMCTempExtruder = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlMCTemp();
            this.cMCTempPlatform = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlMCTemp();
            this.flowData1 = new System.Windows.Forms.FlowLayoutPanel();
            this.cOnOffMotors = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlOnOff();
            this.cOnOffManGcode = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlOnOff();
            this.cOnOffHeater = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlOnOff();
            this.cOnOffPlatform = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlOnOff();
            this.cOnOffMonitorTemp = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlOnOff();
            this.cShutter = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlMCShutter();
            this.flowData2 = new System.Windows.Forms.FlowLayoutPanel();
            this.ctlParamZrate = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlParameter();
            this.ctlParamXYrate = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlParameter();
            this.ctlParamExtrudeRate = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlParameter();
            this.flowBot = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLeft = new System.Windows.Forms.FlowLayoutPanel();
            this.flowMain = new System.Windows.Forms.FlowLayoutPanel();
            this.flowRight = new System.Windows.Forms.FlowLayoutPanel();
            this.cGCodeManual = new UV_DLP_3D_Printer.GUI.Controls.ManualControls.ctlGCodeManual();
            this.cProjectorControl = new UV_DLP_3D_Printer.GUI.Controls.ctlProjectorControl();
            this.flowTop.SuspendLayout();
            this.flowData1.SuspendLayout();
            this.flowData2.SuspendLayout();
            this.flowBot.SuspendLayout();
            this.flowLeft.SuspendLayout();
            this.flowMain.SuspendLayout();
            this.flowRight.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowTop
            // 
            this.flowTop.Controls.Add(this.cMCXY);
            this.flowTop.Controls.Add(this.cMCZ);
            this.flowTop.Controls.Add(this.cMCExtruder);
            this.flowTop.Controls.Add(this.cMCTilt);
            this.flowTop.Controls.Add(this.cMCTempExtruder);
            this.flowTop.Controls.Add(this.cMCTempPlatform);
            this.flowTop.Location = new System.Drawing.Point(2, 2);
            this.flowTop.Margin = new System.Windows.Forms.Padding(2);
            this.flowTop.Name = "flowTop";
            this.flowTop.Size = new System.Drawing.Size(645, 264);
            this.flowTop.TabIndex = 0;
            // 
            // cMCXY
            // 
            this.cMCXY.Font = new System.Drawing.Font("Arial", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.cMCXY.FrameColor = System.Drawing.Color.RoyalBlue;
            this.cMCXY.Gapx = 0;
            this.cMCXY.Gapy = 0;
            this.cMCXY.GLBackgroundImage = null;
            this.cMCXY.GLVisible = false;
            this.cMCXY.GuiAnchor = null;
            this.cMCXY.Location = new System.Drawing.Point(3, 3);
            this.cMCXY.Name = "cMCXY";
            this.cMCXY.ReturnValues = new float[] {
        0.1F,
        1F,
        10F,
        100F};
            this.cMCXY.Size = new System.Drawing.Size(256, 256);
            this.cMCXY.StyleName = null;
            this.cMCXY.TabIndex = 0;
            this.cMCXY.Title = "XY Axis";
            this.cMCXY.Unit = "mm";
            this.cMCXY.MotorMove += new UV_DLP_3D_Printer.GUI.CustomGUI.ctlMCBase.MotorMoveDelegate(this.cMCXY_MotorMove);
            this.cMCXY.MotorHome += new UV_DLP_3D_Printer.GUI.CustomGUI.ctlMCBase.MotorHomeDelegate(this.cMCXY_MotorHome);
            // 
            // cMCZ
            // 
            this.cMCZ.Font = new System.Drawing.Font("Arial", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.cMCZ.FrameColor = System.Drawing.Color.RoyalBlue;
            this.cMCZ.Gapx = 0;
            this.cMCZ.Gapy = 0;
            this.cMCZ.GLBackgroundImage = null;
            this.cMCZ.GLVisible = false;
            this.cMCZ.GuiAnchor = null;
            this.cMCZ.Location = new System.Drawing.Point(265, 3);
            this.cMCZ.Name = "cMCZ";
            this.cMCZ.ReturnValues = new float[] {
        0.1F,
        1F,
        10F,
        50F};
            this.cMCZ.Size = new System.Drawing.Size(70, 256);
            this.cMCZ.StyleName = null;
            this.cMCZ.TabIndex = 1;
            this.cMCZ.Title = "Z Axis";
            this.cMCZ.Unit = "mm";
            this.cMCZ.MotorMove += new UV_DLP_3D_Printer.GUI.CustomGUI.ctlMCBase.MotorMoveDelegate(this.cMCXY_MotorMove);
            this.cMCZ.MotorHome += new UV_DLP_3D_Printer.GUI.CustomGUI.ctlMCBase.MotorHomeDelegate(this.cMCXY_MotorHome);
            // 
            // cMCExtruder
            // 
            this.cMCExtruder.AxisSign = "E";
            this.cMCExtruder.AxisValue = UV_DLP_3D_Printer.GUI.CustomGUI.MachineControlAxis.Extruder;
            this.cMCExtruder.Font = new System.Drawing.Font("Arial", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.cMCExtruder.FrameColor = System.Drawing.Color.RoyalBlue;
            this.cMCExtruder.Gapx = 0;
            this.cMCExtruder.Gapy = 0;
            this.cMCExtruder.GLBackgroundImage = null;
            this.cMCExtruder.GLVisible = false;
            this.cMCExtruder.GuiAnchor = null;
            this.cMCExtruder.HasHome = false;
            this.cMCExtruder.Location = new System.Drawing.Point(341, 3);
            this.cMCExtruder.Name = "cMCExtruder";
            this.cMCExtruder.ReturnValues = new float[] {
        0.1F,
        1F,
        10F,
        100F};
            this.cMCExtruder.Size = new System.Drawing.Size(70, 256);
            this.cMCExtruder.StyleName = null;
            this.cMCExtruder.TabIndex = 2;
            this.cMCExtruder.Title = "Extrude";
            this.cMCExtruder.Unit = "mm";
            this.cMCExtruder.MotorMove += new UV_DLP_3D_Printer.GUI.CustomGUI.ctlMCBase.MotorMoveDelegate(this.cMCXY_MotorMove);
            // 
            // cMCTilt
            // 
            this.cMCTilt.AxisSign = "T";
            this.cMCTilt.AxisValue = UV_DLP_3D_Printer.GUI.CustomGUI.MachineControlAxis.Tilt;
            this.cMCTilt.Font = new System.Drawing.Font("Arial", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.cMCTilt.FrameColor = System.Drawing.Color.RoyalBlue;
            this.cMCTilt.Gapx = 0;
            this.cMCTilt.Gapy = 0;
            this.cMCTilt.GLBackgroundImage = null;
            this.cMCTilt.GLVisible = false;
            this.cMCTilt.GuiAnchor = null;
            this.cMCTilt.Location = new System.Drawing.Point(417, 3);
            this.cMCTilt.Name = "cMCTilt";
            this.cMCTilt.ReturnValues = new float[] {
        0.1F,
        1F,
        10F,
        100F};
            this.cMCTilt.Size = new System.Drawing.Size(70, 256);
            this.cMCTilt.StyleName = null;
            this.cMCTilt.TabIndex = 3;
            this.cMCTilt.Title = "Tilt";
            this.cMCTilt.Unit = "mm";
            this.cMCTilt.MotorMove += new UV_DLP_3D_Printer.GUI.CustomGUI.ctlMCBase.MotorMoveDelegate(this.cMCXY_MotorMove);
            this.cMCTilt.MotorHome += new UV_DLP_3D_Printer.GUI.CustomGUI.ctlMCBase.MotorHomeDelegate(this.cMCXY_MotorHome);
            // 
            // cMCTempExtruder
            // 
            this.cMCTempExtruder.Font = new System.Drawing.Font("Arial", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.cMCTempExtruder.FrameColor = System.Drawing.Color.RoyalBlue;
            this.cMCTempExtruder.Gapx = 0;
            this.cMCTempExtruder.Gapy = 0;
            this.cMCTempExtruder.GLBackgroundImage = null;
            this.cMCTempExtruder.GLVisible = false;
            this.cMCTempExtruder.GuiAnchor = null;
            this.cMCTempExtruder.Location = new System.Drawing.Point(493, 3);
            this.cMCTempExtruder.Name = "cMCTempExtruder";
            this.cMCTempExtruder.ReturnValues = new float[] {
        0F,
        0F,
        0F,
        0F};
            this.cMCTempExtruder.Size = new System.Drawing.Size(70, 256);
            this.cMCTempExtruder.StyleName = null;
            this.cMCTempExtruder.TabIndex = 4;
            this.cMCTempExtruder.Title = "Heater";
            this.cMCTempExtruder.Unit = "C";
            // 
            // cMCTempPlatform
            // 
            this.cMCTempPlatform.Font = new System.Drawing.Font("Arial", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.cMCTempPlatform.FrameColor = System.Drawing.Color.RoyalBlue;
            this.cMCTempPlatform.Gapx = 0;
            this.cMCTempPlatform.Gapy = 0;
            this.cMCTempPlatform.GLBackgroundImage = null;
            this.cMCTempPlatform.GLVisible = false;
            this.cMCTempPlatform.GuiAnchor = null;
            this.cMCTempPlatform.Location = new System.Drawing.Point(569, 3);
            this.cMCTempPlatform.Name = "cMCTempPlatform";
            this.cMCTempPlatform.ReturnValues = new float[] {
        0F,
        0F,
        0F,
        0F};
            this.cMCTempPlatform.Size = new System.Drawing.Size(70, 256);
            this.cMCTempPlatform.StyleName = null;
            this.cMCTempPlatform.TabIndex = 5;
            this.cMCTempPlatform.Title = "Platform";
            this.cMCTempPlatform.Unit = "C";
            // 
            // flowData1
            // 
            this.flowData1.Controls.Add(this.cOnOffMotors);
            this.flowData1.Controls.Add(this.cOnOffManGcode);
            this.flowData1.Controls.Add(this.cOnOffHeater);
            this.flowData1.Controls.Add(this.cOnOffPlatform);
            this.flowData1.Controls.Add(this.cOnOffMonitorTemp);
            this.flowData1.Controls.Add(this.cShutter);
            this.flowData1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowData1.Location = new System.Drawing.Point(0, 0);
            this.flowData1.Margin = new System.Windows.Forms.Padding(0);
            this.flowData1.Name = "flowData1";
            this.flowData1.Size = new System.Drawing.Size(209, 283);
            this.flowData1.TabIndex = 1;
            // 
            // cOnOffMotors
            // 
            this.cOnOffMotors.Font = new System.Drawing.Font("Arial", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.cOnOffMotors.FrameColor = System.Drawing.Color.RoyalBlue;
            this.cOnOffMotors.Gapx = 0;
            this.cOnOffMotors.Gapy = 0;
            this.cOnOffMotors.GLBackgroundImage = null;
            this.cOnOffMotors.GLVisible = false;
            this.cOnOffMotors.GuiAnchor = null;
            this.cOnOffMotors.Location = new System.Drawing.Point(3, 3);
            this.cOnOffMotors.Name = "cOnOffMotors";
            this.cOnOffMotors.ReturnValues = new float[] {
        0F,
        0F,
        0F,
        0F};
            this.cOnOffMotors.Size = new System.Drawing.Size(202, 30);
            this.cOnOffMotors.StyleName = null;
            this.cOnOffMotors.TabIndex = 0;
            this.cOnOffMotors.Title = "Motors:";
            this.cOnOffMotors.Visible = false;
            this.cOnOffMotors.StateChange += new UV_DLP_3D_Printer.GUI.CustomGUI.ctlOnOff.StateChangeDelegate(this.ctlOnOffMotors_StateChange);
            // 
            // cOnOffManGcode
            // 
            this.cOnOffManGcode.Font = new System.Drawing.Font("Arial", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.cOnOffManGcode.FrameColor = System.Drawing.Color.RoyalBlue;
            this.cOnOffManGcode.Gapx = 0;
            this.cOnOffManGcode.Gapy = 0;
            this.cOnOffManGcode.GLBackgroundImage = null;
            this.cOnOffManGcode.GLVisible = false;
            this.cOnOffManGcode.GuiAnchor = null;
            this.cOnOffManGcode.Location = new System.Drawing.Point(3, 39);
            this.cOnOffManGcode.Name = "cOnOffManGcode";
            this.cOnOffManGcode.ReturnValues = new float[] {
        0F,
        0F,
        0F,
        0F};
            this.cOnOffManGcode.Size = new System.Drawing.Size(202, 30);
            this.cOnOffManGcode.StyleName = null;
            this.cOnOffManGcode.TabIndex = 4;
            this.cOnOffManGcode.Title = "Manual Gcode:";
            this.cOnOffManGcode.StateChange += new UV_DLP_3D_Printer.GUI.CustomGUI.ctlOnOff.StateChangeDelegate(this.ctlManGcode_StateChange);
            // 
            // cOnOffHeater
            // 
            this.cOnOffHeater.Font = new System.Drawing.Font("Arial", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.cOnOffHeater.FrameColor = System.Drawing.Color.RoyalBlue;
            this.cOnOffHeater.Gapx = 0;
            this.cOnOffHeater.Gapy = 0;
            this.cOnOffHeater.GLBackgroundImage = null;
            this.cOnOffHeater.GLVisible = false;
            this.cOnOffHeater.GuiAnchor = null;
            this.cOnOffHeater.Location = new System.Drawing.Point(3, 75);
            this.cOnOffHeater.Name = "cOnOffHeater";
            this.cOnOffHeater.ReturnValues = new float[] {
        0F,
        0F,
        0F,
        0F};
            this.cOnOffHeater.Size = new System.Drawing.Size(202, 30);
            this.cOnOffHeater.StyleName = null;
            this.cOnOffHeater.TabIndex = 1;
            this.cOnOffHeater.Title = "Heater:";
            this.cOnOffHeater.StateChange += new UV_DLP_3D_Printer.GUI.CustomGUI.ctlOnOff.StateChangeDelegate(this.ctlOnOffHeater_StateChange);
            // 
            // cOnOffPlatform
            // 
            this.cOnOffPlatform.Font = new System.Drawing.Font("Arial", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.cOnOffPlatform.FrameColor = System.Drawing.Color.RoyalBlue;
            this.cOnOffPlatform.Gapx = 0;
            this.cOnOffPlatform.Gapy = 0;
            this.cOnOffPlatform.GLBackgroundImage = null;
            this.cOnOffPlatform.GLVisible = false;
            this.cOnOffPlatform.GuiAnchor = null;
            this.cOnOffPlatform.Location = new System.Drawing.Point(3, 111);
            this.cOnOffPlatform.Name = "cOnOffPlatform";
            this.cOnOffPlatform.ReturnValues = new float[] {
        0F,
        0F,
        0F,
        0F};
            this.cOnOffPlatform.Size = new System.Drawing.Size(202, 30);
            this.cOnOffPlatform.StyleName = null;
            this.cOnOffPlatform.TabIndex = 2;
            this.cOnOffPlatform.Title = "Platform:";
            this.cOnOffPlatform.StateChange += new UV_DLP_3D_Printer.GUI.CustomGUI.ctlOnOff.StateChangeDelegate(this.ctlOnOffPlatform_StateChange);
            // 
            // cOnOffMonitorTemp
            // 
            this.cOnOffMonitorTemp.Font = new System.Drawing.Font("Arial", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.cOnOffMonitorTemp.FrameColor = System.Drawing.Color.RoyalBlue;
            this.cOnOffMonitorTemp.Gapx = 0;
            this.cOnOffMonitorTemp.Gapy = 0;
            this.cOnOffMonitorTemp.GLBackgroundImage = null;
            this.cOnOffMonitorTemp.GLVisible = false;
            this.cOnOffMonitorTemp.GuiAnchor = null;
            this.cOnOffMonitorTemp.Location = new System.Drawing.Point(3, 147);
            this.cOnOffMonitorTemp.Name = "cOnOffMonitorTemp";
            this.cOnOffMonitorTemp.ReturnValues = new float[] {
        0F,
        0F,
        0F,
        0F};
            this.cOnOffMonitorTemp.Size = new System.Drawing.Size(202, 30);
            this.cOnOffMonitorTemp.StyleName = null;
            this.cOnOffMonitorTemp.TabIndex = 3;
            this.cOnOffMonitorTemp.Title = "Monitor Temp:";
            // 
            // cShutter
            // 
            this.cShutter.Font = new System.Drawing.Font("Arial", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.cShutter.FrameColor = System.Drawing.Color.RoyalBlue;
            this.cShutter.Gapx = 0;
            this.cShutter.Gapy = 0;
            this.cShutter.GLBackgroundImage = null;
            this.cShutter.GLVisible = false;
            this.cShutter.GuiAnchor = null;
            this.cShutter.Location = new System.Drawing.Point(3, 183);
            this.cShutter.Name = "cShutter";
            this.cShutter.ReturnValues = new float[] {
        0F,
        0F,
        0F,
        0F};
            this.cShutter.Size = new System.Drawing.Size(202, 62);
            this.cShutter.StyleName = null;
            this.cShutter.TabIndex = 5;
            this.cShutter.Title = "Shutter:";
            this.cShutter.StateChange += new UV_DLP_3D_Printer.GUI.CustomGUI.ctlMCShutter.StateChangeDelegate(this.cShutter_StateChange);
            // 
            // flowData2
            // 
            this.flowData2.Controls.Add(this.ctlParamZrate);
            this.flowData2.Controls.Add(this.ctlParamXYrate);
            this.flowData2.Controls.Add(this.ctlParamExtrudeRate);
            this.flowData2.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowData2.Location = new System.Drawing.Point(209, 0);
            this.flowData2.Margin = new System.Windows.Forms.Padding(0);
            this.flowData2.Name = "flowData2";
            this.flowData2.Size = new System.Drawing.Size(260, 283);
            this.flowData2.TabIndex = 2;
            // 
            // ctlParamZrate
            // 
            this.ctlParamZrate.Font = new System.Drawing.Font("Arial", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.ctlParamZrate.FrameColor = System.Drawing.Color.RoyalBlue;
            this.ctlParamZrate.Gapx = 0;
            this.ctlParamZrate.Gapy = 0;
            this.ctlParamZrate.GLBackgroundImage = null;
            this.ctlParamZrate.GLVisible = false;
            this.ctlParamZrate.GuiAnchor = null;
            this.ctlParamZrate.Location = new System.Drawing.Point(5, 3);
            this.ctlParamZrate.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            this.ctlParamZrate.Name = "ctlParamZrate";
            this.ctlParamZrate.ReturnValues = new float[] {
        0F,
        0F,
        0F,
        0F};
            this.ctlParamZrate.Size = new System.Drawing.Size(250, 30);
            this.ctlParamZrate.StyleName = null;
            this.ctlParamZrate.TabIndex = 0;
            this.ctlParamZrate.Title = "Z Rate (mm/m):";
            this.ctlParamZrate.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.ctlParamZrate.ValueChanged += new UV_DLP_3D_Printer.GUI.CustomGUI.ctlParameter.ValueChangedDelegate(this.ctlParamZrate_ValueChanged);
            // 
            // ctlParamXYrate
            // 
            this.ctlParamXYrate.Font = new System.Drawing.Font("Arial", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.ctlParamXYrate.FrameColor = System.Drawing.Color.RoyalBlue;
            this.ctlParamXYrate.Gapx = 0;
            this.ctlParamXYrate.Gapy = 0;
            this.ctlParamXYrate.GLBackgroundImage = null;
            this.ctlParamXYrate.GLVisible = false;
            this.ctlParamXYrate.GuiAnchor = null;
            this.ctlParamXYrate.Location = new System.Drawing.Point(5, 39);
            this.ctlParamXYrate.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            this.ctlParamXYrate.Name = "ctlParamXYrate";
            this.ctlParamXYrate.ReturnValues = new float[] {
        0F,
        0F,
        0F,
        0F};
            this.ctlParamXYrate.Size = new System.Drawing.Size(250, 30);
            this.ctlParamXYrate.StyleName = null;
            this.ctlParamXYrate.TabIndex = 1;
            this.ctlParamXYrate.Title = "XY Rate (mm/m):";
            this.ctlParamXYrate.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.ctlParamXYrate.ValueChanged += new UV_DLP_3D_Printer.GUI.CustomGUI.ctlParameter.ValueChangedDelegate(this.ctlParamXYrate_ValueChanged);
            // 
            // ctlParamExtrudeRate
            // 
            this.ctlParamExtrudeRate.Font = new System.Drawing.Font("Arial", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.ctlParamExtrudeRate.FrameColor = System.Drawing.Color.RoyalBlue;
            this.ctlParamExtrudeRate.Gapx = 0;
            this.ctlParamExtrudeRate.Gapy = 0;
            this.ctlParamExtrudeRate.GLBackgroundImage = null;
            this.ctlParamExtrudeRate.GLVisible = false;
            this.ctlParamExtrudeRate.GuiAnchor = null;
            this.ctlParamExtrudeRate.Location = new System.Drawing.Point(5, 75);
            this.ctlParamExtrudeRate.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            this.ctlParamExtrudeRate.Name = "ctlParamExtrudeRate";
            this.ctlParamExtrudeRate.ReturnValues = new float[] {
        0F,
        0F,
        0F,
        0F};
            this.ctlParamExtrudeRate.Size = new System.Drawing.Size(250, 30);
            this.ctlParamExtrudeRate.StyleName = null;
            this.ctlParamExtrudeRate.TabIndex = 2;
            this.ctlParamExtrudeRate.Title = "Extrude Rate (mm/m):";
            this.ctlParamExtrudeRate.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            // 
            // flowBot
            // 
            this.flowBot.Controls.Add(this.flowData1);
            this.flowBot.Controls.Add(this.flowData2);
            this.flowBot.Location = new System.Drawing.Point(3, 268);
            this.flowBot.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.flowBot.Name = "flowBot";
            this.flowBot.Size = new System.Drawing.Size(645, 283);
            this.flowBot.TabIndex = 3;
            // 
            // flowLeft
            // 
            this.flowLeft.Controls.Add(this.flowTop);
            this.flowLeft.Controls.Add(this.flowBot);
            this.flowLeft.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLeft.Location = new System.Drawing.Point(0, 3);
            this.flowLeft.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.flowLeft.Name = "flowLeft";
            this.flowLeft.Size = new System.Drawing.Size(653, 551);
            this.flowLeft.TabIndex = 4;
            // 
            // flowMain
            // 
            this.flowMain.Controls.Add(this.flowLeft);
            this.flowMain.Controls.Add(this.flowRight);
            this.flowMain.Location = new System.Drawing.Point(0, 0);
            this.flowMain.Name = "flowMain";
            this.flowMain.Size = new System.Drawing.Size(987, 557);
            this.flowMain.TabIndex = 5;
            // 
            // flowRight
            // 
            this.flowRight.Controls.Add(this.cGCodeManual);
            this.flowRight.Controls.Add(this.cProjectorControl);
            this.flowRight.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowRight.Location = new System.Drawing.Point(656, 3);
            this.flowRight.Name = "flowRight";
            this.flowRight.Size = new System.Drawing.Size(294, 551);
            this.flowRight.TabIndex = 5;
            // 
            // cGCodeManual
            // 
            this.cGCodeManual.Location = new System.Drawing.Point(3, 3);
            this.cGCodeManual.Name = "cGCodeManual";
            this.cGCodeManual.Padding = new System.Windows.Forms.Padding(5);
            this.cGCodeManual.Size = new System.Drawing.Size(280, 345);
            this.cGCodeManual.TabIndex = 0;
            this.cGCodeManual.Load += new System.EventHandler(this.cGCodeManual_Load);
            // 
            // cProjectorControl
            // 
            this.cProjectorControl.Location = new System.Drawing.Point(3, 353);
            this.cProjectorControl.Margin = new System.Windows.Forms.Padding(3, 2, 2, 0);
            this.cProjectorControl.Name = "cProjectorControl";
            this.cProjectorControl.Size = new System.Drawing.Size(280, 196);
            this.cProjectorControl.TabIndex = 1;
            // 
            // ctlManualControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.flowMain);
            this.Name = "ctlManualControl";
            this.Size = new System.Drawing.Size(987, 557);
            this.flowTop.ResumeLayout(false);
            this.flowData1.ResumeLayout(false);
            this.flowData2.ResumeLayout(false);
            this.flowBot.ResumeLayout(false);
            this.flowLeft.ResumeLayout(false);
            this.flowMain.ResumeLayout(false);
            this.flowRight.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowTop;
        private CustomGUI.ctlMCXY cMCXY;
        private CustomGUI.ctlMCZ cMCZ;
        private CustomGUI.ctlMCZ cMCExtruder;
        private CustomGUI.ctlMCZ cMCTilt;
        private CustomGUI.ctlMCTemp cMCTempExtruder;
        private CustomGUI.ctlMCTemp cMCTempPlatform;
        private System.Windows.Forms.FlowLayoutPanel flowData1;
        private CustomGUI.ctlOnOff cOnOffMotors;
        private CustomGUI.ctlOnOff cOnOffHeater;
        private CustomGUI.ctlOnOff cOnOffPlatform;
        private CustomGUI.ctlOnOff cOnOffMonitorTemp;
        private System.Windows.Forms.FlowLayoutPanel flowData2;
        private CustomGUI.ctlParameter ctlParamZrate;
        private CustomGUI.ctlOnOff cOnOffManGcode;
        private CustomGUI.ctlParameter ctlParamXYrate;
        private System.Windows.Forms.FlowLayoutPanel flowBot;
        private System.Windows.Forms.FlowLayoutPanel flowLeft;
        private System.Windows.Forms.FlowLayoutPanel flowMain;
        private System.Windows.Forms.FlowLayoutPanel flowRight;
        private ManualControls.ctlGCodeManual cGCodeManual;
        private ctlProjectorControl cProjectorControl;
        private CustomGUI.ctlParameter ctlParamExtrudeRate;
        private CustomGUI.ctlMCShutter cShutter;
    }
}
