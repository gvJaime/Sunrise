using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Ionic.Zip;
using System.Globalization;
namespace UV_DLP_3D_Printer
{
    public static class Utility
    {
        public static DateTime RetrieveLinkerTimestamp()
        {
            string filePath = System.Reflection.Assembly.GetCallingAssembly().Location;
            const int c_PeHeaderOffset = 60;
            const int c_LinkerTimestampOffset = 8;
            byte[] b = new byte[2048];
            System.IO.Stream s = null;

            try
            {
                s = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                s.Read(b, 0, 2048);
            }
            finally
            {
                if (s != null)
                {
                    s.Close();
                }
            }

            int i = System.BitConverter.ToInt32(b, c_PeHeaderOffset);
            int secondsSince1970 = System.BitConverter.ToInt32(b, i + c_LinkerTimestampOffset);
            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0);
            dt = dt.AddSeconds(secondsSince1970);
            dt = dt.AddHours(TimeZone.CurrentTimeZone.GetUtcOffset(dt).Hours);
            return dt;
        }
        public static bool CreateDirectory(String path) 
        {
            try
            {
                Directory.CreateDirectory(path);
                return true;
            }
            catch (Exception ) 
            {
                return false;
            }
        }
        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        public static Stream ReadFromZip(string zipname, string zipentryname) 
        {
            try
            {
                using (ZipFile zip = ZipFile.Read(zipname))
                {
                    ZipEntry ze = zip[zipentryname];
                    Stream stream = new MemoryStream();
                    ze.Extract(stream);
                    stream.Seek(0, SeekOrigin.Begin); // rewind
                    return stream;
                }
                
            }
            catch (Exception ex)
            {
                DebugLogger.Instance().LogError(ex.Message);
                return null;
            }
        
        }
        public static string CleanMonitorString(string str)
        {
            string tmp = str.Replace("\\", string.Empty);
            tmp = tmp.Replace(".", string.Empty);
            tmp = tmp.Trim();
            return tmp;
        }
        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        public static byte[] HexStringToByteArray(string hexString)
        {
            if (hexString.Length % 2 != 0)
            {
                //throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, "The binary key cannot have an odd number of digits: {0}", hexString));
                DebugLogger.Instance().LogError("The hex string cannot have an odd number of digits");
                return null;
            }

            byte[] HexAsBytes = new byte[hexString.Length / 2];
            for (int index = 0; index < HexAsBytes.Length; index++)
            {
                string byteValue = hexString.Substring(index * 2, 2);
                HexAsBytes[index] = byte.Parse(byteValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }

            return HexAsBytes;
        }

        public static string RelativePath(string absPath, string relTo) 
        {
          string[] absDirs = absPath.Split('\\');
          string[] relDirs = relTo.Split('\\');
  
          // Get the shortest of the two paths
          int len = absDirs.Length < relDirs.Length ? absDirs.Length : 
          relDirs.Length;

          // Use to determine where in the loop we exited
          int lastCommonRoot = -1;
          int index;

          // Find common root
          for (index = 0; index < len; index++) {
            if (absDirs[index] == relDirs[index]) lastCommonRoot = index;
            else break;
          }

          // If we didn't find a common prefix then throw
          if (lastCommonRoot == -1) {
            throw new ArgumentException("Paths do not have a common base");
          }

          // Build up the relative path
          StringBuilder relativePath = new StringBuilder();

          // Add on the ..
          for (index = lastCommonRoot + 1; index < absDirs.Length; index++) {
            if (absDirs[index].Length > 0) relativePath.Append("..\\");
          }
  
          // Add on the folders
          for (index = lastCommonRoot + 1; index < relDirs.Length - 1; index++) {
            relativePath.Append(relDirs[index] + "\\");
          }
          relativePath.Append(relDirs[relDirs.Length - 1]);
  
        return relativePath.ToString();
        }
        public static bool StoreInZip(string zipname, string zipentryname, Stream stream) 
        {            
            try
            {
                ZipFile zip = ZipFile.Read(zipname);                
                zip.AddEntry(zipentryname, stream);
                zip.Save();
                return true;
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex.Message);
                return false;
            }
        }

        public static bool IsAlphaNum(string str)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(str, @"^[_a-zA-Z0-9]+$");
        }
    }
}
