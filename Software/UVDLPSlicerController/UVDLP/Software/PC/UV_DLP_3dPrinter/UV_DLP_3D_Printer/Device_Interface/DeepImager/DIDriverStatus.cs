using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UV_DLP_3D_Printer.Device_Interface.DeepImager
{
    /// <summary>
    /// this class is used to decode the various messages sent from the printer to the host controller
    /// </summary>
    public class DIDriverStatus
    {
        // parse the 13 byte respone
        public static byte MODE_PRINT = (byte)'P';
        public static byte MODE_CALIBRATE = (byte)'C';
        public static byte MODE_STANDBY = (byte)'S';
        public byte Mode;

        public static byte RESOLUTION_25 = 1;
        public static byte RESOLUTION_50 = 2;
        public static byte RESOLUTION_75 = 3;
        public static byte RESOLUTION_100 = 4;
        public static byte RESOLUTION_150 = 5;
        public static byte RESOLUTION_200 = 6;

        public byte Resolution; // x/y resolution

        public int ZPosSteps;  // 24 bytes

        public byte Printer_Status;
        public static byte Status_Door_Open = 0x01;
        public static byte Status_Proj_on   = 0x02;
        public static byte Status_Pump_on_fill = 0x04;
        public static byte Status_Pump_on_drain = 0x08;
        public static byte Status_Build_ready = 0x20; // (New slice may be projected)
        public static byte Status_Cal_Done = 0x40;
        public static byte Status_Cal_Active = 0x80;
       
        public static byte Home_Status_Proj_throw_Home = 0x01;
        public static byte Home_Status_Proj_x_home = 0x02;
        public static byte Home_Status_Proj_y_home = 0x04;
        public static byte Home_Status_Proj_z_home = 0x08;
        public static byte Home_Status_Proj_focus_home = 0x10; // (New slice may be projected)
        public static byte Home_Status_Build_home = 0x40;
        public static byte Home_Status_Build_home_limit = 0x80;

        public byte Home_Status; 
        public byte Firmware_version;
        public byte Limit_Status;
        public byte Scan_Status;
        public byte Error_Code;
        public byte Halt;
        public string ErrorCode() 
        {
            switch(Error_Code)
            {
                case 0   : return "OK";
                case 1   : return "Framing Error";
                case 2   : return "Buff Ovfl";
                case 3   : return "Type";
                case 4   : return "Function Code";
                case 5   : return "Command Code";
                case 6   : return "Xsum";
                case 7   : return "Message not permitted in Current Mode";
                case 8   : return "Comm Error";
                case 9   : return "+12v Fault";
                case 10 : return "Build Stepper Fault";
                case 11 : return "Proj Throw Stepper Fault ";
                case 12 : return "Proj X Stepper Fault";
                case 13 : return "Proj Y Stepper Fault";
                case 14 : return "Proj Z Stepper Fault";
                case 15 : return "Proj Focus Stepper Fault";
                case 16 : return "Spare Stepper Fault";
                case 17 : return "Build Stepper at limit (unexpected)";
                case 18 : return "Proj Throw Stepper at limit (unexpected)";
                case 19 : return "Proj X Stepper at limit (unexpected)";
                case 20 : return "Proj Y Stepper at limit (unexpected)";
                case 21 : return "Proj Z Stepper at limit (unexpected)";
                case 22 : return "Proj Focus Stepper at limit (unexpected)";
                case 23 : return "Spare Stepper at limit (unexpected)";
                case 24 : return "Fill Pump motor over-current";
                case 25 : return "Drain Pump motor over-current";
                case 26 : return "Tilt motor over-current";
                case 27: return "Scan motor over-current";
                case 28 : return "Build Home Not Valid";
                case 29 : return "Build Home cannot be 0";		  		  
            }
            return "Unknown";
        }

        public DIDriverStatus(byte[] data) 
        {
            // parse the response
            //checksum the message
            Mode = data[1];
            int tmp;
            ZPosSteps = data[2];
            tmp = data[3];
            tmp = tmp << 8;
            ZPosSteps |= tmp;

            tmp = data[4];
            tmp = tmp << 16;
            ZPosSteps |= tmp;

            Resolution = data[5];
            Printer_Status = data[6];
            Home_Status = data[7];
            Limit_Status = data[8];
            Scan_Status = data[9];
            Halt = (byte)(data[10] & (byte)0x80); // top bit
            Error_Code = (byte)(data[10] & (byte)0x7F); // lower 7 bits
            Firmware_version = data[11];
        }
    }

    /*
Messages to PC 		-	Issued at System Status Change

	Byte 0		'#'	to PC

	Byte 1		Current Mode
			'P'	Print	  -	Print mode,
			'C'	Cal	  -	Calibrate Mode (Activated when Cal box attached)
			'S'	Standby  -	Standby
	

	Byte 2		Build Z Position (LSB)	Steps from park (0) (@ .0005 / step, max range is 600+ feet)
	Byte 3		Build Z Position 
	Byte 4		Build Z Position (MSB)

	Byte 5		Current Resolution Code 
			‘1’    =   25 um
			‘2’    =   50 um
			‘3’    =   75 um
			‘4’    = 100 um
			‘5’    = 150 um
			‘6’    = 200 um

		
	Byte 6		Printer Status
			Bit 0 -	Door Open
			Bit 1 -	Proj On
			Bit 2 -	Pump On - Fill
			Bit 3 -	Pump On - Drain
			Bit 4 -	Build Ready 			(New slice may be projected)
			Bit 5 -	
			Bit 6 -	Cal Done for Current Resolution Code – Go to Next Code
			Bit 7 -	Cal Active			(Service Box Connected)
		
	Byte 7		Home Status
			Bit 0 -	Proj Throw Home   
			Bit 1 -	Proj X Home	 	   
			Bit 2 -	Proj Y Home	 
			Bit 3 -	Proj Z Home	 
			Bit 4 -	Proj Focus Home	 
			Bit 5 -	Spare Home
			Bit 6 -	Build Home
			Bit 7 -	Build Home Limit
		
	Byte 8		Limit Status
			Bit 0 -	Proj Throw Limit 
			Bit 1 -	Proj X Limit
			Bit 2 -	Proj Y Limit
			Bit 3 -	Proj Z Limit
			Bit 4 -	Proj Focus Limit
			Bit 5 -	Spare Limit
			Bit 6 -	Build Park
			Bit 7 -	Door Lock			
	
	Byte 9		Scan Status 
			Bit 0 -	 Power On
			Bit 1 -	Table Active
			Bit 2 -	Table Home
			Bit 3 -	
			Bit 4 -	
			Bit 5 -	
			Bit 6 -
			Bit 7 -	Scan Complete			(Read Data from Camera A & B via USB Port)
		
	Byte 10		Error code 
			Bits 0 -> 6, error code = 0 -> 128
			Bit  7 -	Halt Flag

			0   = OK
			1   = Framing Error
			2   = Buff Ovfl
			3   = Type
			4   = Function Code
			5   = Command Code
			6   = Xsum
			7   = Message not permitted in Current Mode
			8   = Comm Error
			9   = +12v Fault
			10 = Build Stepper Fault
			11 = Proj Throw Stepper Fault 
			12 = Proj X Stepper Fault
			13 = Proj Y Stepper Fault
			14 = Proj Z Stepper Fault
			15 = Proj Focus Stepper Fault
			16 = Spare Stepper Fault
			17 = Build Stepper at limit (unexpected)
			18 = Proj Throw Stepper at limit (unexpected)
			19 = Proj X Stepper at limit (unexpected)
			20 = Proj Y Stepper at limit (unexpected)
			21 = Proj Z Stepper at limit (unexpected)
			22 = Proj Focus Stepper at limit (unexpected)
			23 = Spare Stepper at limit (unexpected)
			24 = Fill Pump motor over-current
			25 = Drain Pump motor over-current
			26 = Tilt motor over-current
			27 = Scan motor over-current
			28 = Build Home Not Valid
			29 = Build Home cannot be 0		  		  
		
	Byte 11		Firmware Version

	Byte 12		XSUM
 
     
     */
}
