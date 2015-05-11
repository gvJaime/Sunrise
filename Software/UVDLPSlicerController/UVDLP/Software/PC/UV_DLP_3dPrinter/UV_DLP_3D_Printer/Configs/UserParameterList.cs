using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using UV_DLP_3D_Printer.GUI.CustomGUI;

namespace UV_DLP_3D_Printer.Configs
{
    public class UserParameterList
    {
        public Dictionary<string, CWParameter> paramDict;
        public bool ConfigChanged;
        public UserParameterList()
        {
            paramDict = new Dictionary<string, CWParameter>();
            ConfigChanged = false;
        }

        public CWParameter GetParameter(string name, object defaultVal)
        {
            if (paramDict.ContainsKey(name))
                return paramDict[name];
            CWParameter par = null;
            if (defaultVal is string)
                par = CWParameter.CreateStringParam((string)defaultVal, null);
            else if (defaultVal is int)
                par = CWParameter.CreateIntParam(((int)defaultVal).ToString(), null);
            else if (defaultVal is bool)
                par = CWParameter.CreateBoolParam(((bool)defaultVal).ToString(), null);
            else if (defaultVal is Color)
                par = CWParameter.CreateColorParam(GuiConfigDB.ColorToString((Color)defaultVal), null);
            if (par != null)
            {
                par.paramName = name;
                paramDict[name] = par;
                ConfigChanged = true;
            }
            return par;
        }
    }
}
