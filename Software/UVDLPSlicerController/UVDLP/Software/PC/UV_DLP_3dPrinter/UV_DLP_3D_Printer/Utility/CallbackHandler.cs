using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace UV_DLP_3D_Printer
{
    public delegate void CallbackType(Object sender, Object vars);
    public delegate Object RetCallbackType(Object sender, Object vars);
    class CallbackItem
    {
        public String name;
        public String type;
        public String retType;
        public String description;
        public CallbackType Callback;
        public RetCallbackType RetCallback;
    }

    public class CallbackHandler
    {
        Dictionary<String, CallbackItem> CallbackDB;

        public CallbackHandler()
        {
            CallbackDB = new Dictionary<string, CallbackItem>();
        }

        public void RegisterCallback(String cmdname, CallbackType func, Type vartype, String desc)
        {
            CallbackItem cb;
            String vartypename = "null";
            String cmd = cmdname;
            if (vartype != null)
            {
                vartypename = vartype.ToString();
                cmd += "|" + vartypename;
            }
            if (CallbackDB.ContainsKey(cmdname))
            {
                cb = CallbackDB[cmd];
            }
            else
            {
                cb = new CallbackItem();
                cb.name = cmdname;
                cb.type = vartypename;
                cb.description = desc;
                CallbackDB[cmd] = cb;
            }
            cb.RetCallback = null;
            cb.retType = "void";
            cb.Callback += new CallbackType(func);
        }

        // if var type not specified, array of string args is assumed 
        public void RegisterCallback(String cmdname, CallbackType func, String desc)
        {
            RegisterCallback(cmdname, func, typeof(string[]), desc);
        }


        public void RegisterRetCallback(String cmdname, RetCallbackType func, Type vartype, Type rettype, String desc)
        {
            CallbackItem cb;
            String vartypename = "null";
            String cmd = cmdname;
            if (vartype != null)
            {
                vartypename = vartype.ToString();
                cmd += "|" + vartypename;
            }
            if (CallbackDB.ContainsKey(cmdname))
            {
                cb = CallbackDB[cmd];
            }
            else
            {
                cb = new CallbackItem();
                cb.name = cmdname;
                cb.type = vartypename;
                cb.description = desc;
                CallbackDB[cmd] = cb;
            }
            cb.RetCallback += new RetCallbackType(func);
            cb.retType = rettype.ToString();
            cb.Callback = null;
        }

        public Object Activate(String cmdname, Object sender, Object vars)
        {
            if (cmdname == null)
                return false;
            if ((vars != null) && (vars.GetType() != null))
                cmdname += "|" + vars.GetType().ToString();
            if (!CallbackDB.ContainsKey(cmdname))
                return false;
            CallbackItem cb = CallbackDB[cmdname];
            if (cb.Callback != null)
            {
                cb.Callback(sender, vars);
                return true;
            }
            if (cb.RetCallback != null)
            {
                return cb.RetCallback(sender, vars);
            }
            return false;
        }

        public Object Activate(String cmdname, Object sender)
        {
            string [] compoundCmd = cmdname.Split(new char [] {' '}, StringSplitOptions.RemoveEmptyEntries);
            if (compoundCmd.Length > 1)
            {
                List<string> varlist = compoundCmd.ToList();
                varlist.RemoveAt(0);
                return Activate(compoundCmd[0], sender, varlist.ToArray());
            }
            return Activate(cmdname, sender, null);
        }

        public Object Activate(String cmdname)
        {
            return Activate(cmdname, null);
        }

        public void DumpCommands(String filename)
        {
            StreamWriter sw = new StreamWriter(filename);
            foreach (KeyValuePair<String, CallbackItem> pair in CallbackDB)
            {
                CallbackItem ci = pair.Value;
                sw.WriteLine("{0}, \t{1}, \t{2}, \t{3}", ci.retType, ci.name, ci.type, ci.description);
            }
            sw.Close();
            sw.Dispose();
        }
    }
}
