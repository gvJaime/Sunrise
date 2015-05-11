using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UV_DLP_3D_Printer.Device_Interface;

namespace UV_DLP_3D_Printer.Drivers
{
    public class DriverFactory
    {
        public static DeviceDriver Create(eDriverType type) 
        {
            switch (type) 
            {
                case eDriverType.eNULL_DRIVER:
                    return new NULLdriver();
                case eDriverType.eGENERIC:
                    return new GenericDriver();
                case eDriverType.eRF_3DLPRINTER:
                    return new RobotFactorySRL_3DLPrinter();
                case eDriverType.eEIW_DEEPIMAGER:
                    return new DIDriver();
                case eDriverType.eUNCIA:
                    return new UnciaDriver();
                default:
                    return null;
            }
        }
    }
}
