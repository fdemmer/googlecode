/*

PasteToFile

Version     0.2
Constact    <florian@demmer.org>
WWW         http://florian.demmer.org
License     GPL 2.0

requires .net Framework 2.0

*/


using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace PasteToFile
{

    class Registry
    {
        private Microsoft.Win32.RegistryKey hkcu;
        internal Microsoft.Win32.RegistryKey Key;
        
        public Registry()
        {
            hkcu = Microsoft.Win32.Registry.CurrentUser;
            // open existing settings, writeable
            if ((Key = open(Resource.Title, true)) == null)
                // key was not found, create a new one
                Key = create(Resource.Title);
        }

        private Microsoft.Win32.RegistryKey open(String name, bool writeable)
        {
            try
            {
                // open subkey
                Key = hkcu.OpenSubKey(Resource.RegistryPath + name, writeable);
            }
            catch (System.ObjectDisposedException)
            {
                // registry key is closed and connot be opened
            }
            catch (System.Security.SecurityException)
            {
                // insufficient rights to access key
            }
            return Key;
        }

        // create the registry key
        private Microsoft.Win32.RegistryKey create(String name)
        {
            try
            {
                // open/create subkey read/write
                Key = hkcu.CreateSubKey(Resource.RegistryPath + name);
                /*Key.SetValue(Resource.RegKey_ImageFormat, Resource.Default_ImageFormat);
                Key.SetValue(Resource.RegKey_OutputPath,  Resource.Default_OutputPath);
                Key.SetValue(Resource.RegKey_Mask_File,   Resource.Default_Mask_File);
                Key.SetValue(Resource.RegKey_Mask_Date,   Resource.Default_Mask_Date);
                Key.SetValue(Resource.RegKey_Mask_Time,   Resource.Default_Mask_Time);
                Key.SetValue(Resource.RegKey_Version,     Resource.Version);*/
            }
            catch (System.ObjectDisposedException)
            {
                // registry key is closed and connot be opened
            }
            catch (System.Security.SecurityException)
            {
                // insufficient rights to access key
            }
            catch (System.IO.IOException)
            {
                // systemerror, boxing > 510, wrong tree (local machine)
            }
            catch (System.UnauthorizedAccessException)
            {
                // insufficient rights to access key
            }

            return Key;
        }

        // remove all values from registry
        private void remove(String name)
        {
            hkcu.DeleteSubKeyTree(Resource.RegistryPath+name);
        }

    } // end of class

    class Resource
    {
        internal static String Title        = "PasteToFile";
        internal static String Version      = "0.2";
        internal static String Author       = "Florian Demmer";
        internal static String Website      = "http://florian.demmer.org";
        internal static String Email        = "florian@demmer.org";
        internal static String License      = "GPL 2.0";
    
        internal static String RegistryPath         = "Software\\";
        internal static String RegKey_Version       = "Version";
        internal static String RegKey_Mask_File     = "FileMask";
        internal static String RegKey_Mask_Date     = "DateMask";
        internal static String RegKey_Mask_Time     = "TimeMask";
        internal static String RegKey_OutputPath    = "OutputPath";
        internal static String RegKey_ImageFormat   = "ImageFormat";
        internal static String Mask_Date            = "<date>";
        internal static String Mask_Time            = "<time>";
        internal static String Mask_Extension       = "<ext>";
        internal static String Default_Mask_File    = "sshot-<date>-<time>.<ext>";
        internal static String Default_Mask_Date    = "yyyyMMdd";
        internal static String Default_Mask_Time    = "HHmmss";
        internal static String Default_OutputPath   = "";
        internal static int    Default_ImageFormat  = 0;

    } // end of class

    class Program
    {
        internal Registry Reg;

        public Program()
        {
            // access registry
            Reg = new Registry();
        }

        internal void showAbout()
        {
            Application.Run(new About());
        }

        internal void showOptions()
        {
            Application.Run(new Options(Reg));
        }
        
        private String getFilename(String extension)
        {
            // retrieve filename mask setting
            String filename = (String)Reg.Key.GetValue(Resource.RegKey_Mask_File, Resource.Default_Mask_File);

            // generate timestamps
            DateTime d = DateTime.Now;
            String date = d.ToString((String)Reg.Key.GetValue(Resource.RegKey_Mask_Date, Resource.Default_Mask_Date));
            String time = d.ToString((String)Reg.Key.GetValue(Resource.RegKey_Mask_Time, Resource.Default_Mask_Time));

            // replace tags in filemask
            if (filename.Contains(Resource.Mask_Date))
                filename = filename.Replace(Resource.Mask_Date, date);
            if (filename.Contains(Resource.Mask_Time))
                filename = filename.Replace(Resource.Mask_Time, time);
            if (filename.Contains(Resource.Mask_Extension))
                filename = filename.Replace(Resource.Mask_Extension, extension);

            String path = (string)Reg.Key.GetValue(Resource.RegKey_OutputPath, Resource.Default_OutputPath);
            /*
             *    .    does not work
             *    /    root of the directory p2f runs on
             *    ../  one directoy up of place where p2f is run
             *    c:\  well... c:! :)
             * 
             */
            return path+filename;
        }

        internal void doPaste(IDataObject obj)
        {
            // get the image data from the clipboard
            Image image = (Image)obj.GetData(DataFormats.Bitmap);

            // set output format and file extension
            ImageFormat format;
            String extension;
            switch ((int)Reg.Key.GetValue(Resource.RegKey_ImageFormat, Resource.Default_ImageFormat))
            {
                case 0:
                    format = ImageFormat.Png; extension = "png"; break;
                case 1:
                    format = ImageFormat.Jpeg; extension = "jpg"; break;
                case 2:
                    format = ImageFormat.Gif; extension = "gif"; break;
                case 3:
                    format = ImageFormat.Bmp; extension = "bmp"; break;
                case 4:
                    format = ImageFormat.Tiff; extension = "tif"; break;
                default:
                    format = ImageFormat.Png; extension = "png"; break;
            }

            // create a filename
            String filename = getFilename(extension);

            try
            {
                // write output file
                image.Save(filename, format);

                //TODO: show baloon when successfully saved (if that is set that way in the optionns)
            }
            catch (System.Security.SecurityException)
            {
                System.Windows.Forms.MessageBox.Show("Sorry, this cannot be run on a network drive!",
                    "SecurityException", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            catch (System.ArgumentException)
            {
                System.Windows.Forms.MessageBox.Show("Check your filename for invalid characters! (" + filename + ")",
                    "ArgumentException", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            catch (System.NotSupportedException)
            {
                System.Windows.Forms.MessageBox.Show("Check your filename for invalid characters! (" + filename + ")",
                    "ArgumentException", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

        }


    }

    class PasteToFile
    {
        [STAThread]
        static void Main(string[] args)
        {
            // parse commandline arguments
            CommandLine.Utility.Arguments Switches = new CommandLine.Utility.Arguments(args);
            Program prog = new Program();

            // decide next steps...
            if (Switches["about"] != null)
                prog.showAbout();
            else if (Switches["help"] != null)
                prog.showAbout();
            else if (Switches["options"] != null)
                prog.showOptions();
            else if (Switches["setup"] != null)
                prog.showOptions();
            else
            {
                // do not continue if there is nothing in the clipboard
                if (Clipboard.GetDataObject() == null)
                {
                     System.Windows.Forms.MessageBox.Show(
                        "Sorry, the clipboard seems to be empty!",
                        "No picture found!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                   // also do not continue if there is not an image in the clipboard
                    IDataObject obj = Clipboard.GetDataObject();
                    if (!obj.GetDataPresent(DataFormats.Bitmap))
                    {
                        System.Windows.Forms.MessageBox.Show(
                            "Sorry, there is no useable image data in the clipboard!",
                            "No picture found!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        // ONLY if there really is something useful available continue...
                        prog.doPaste(obj);
                    }            
                }            

            } // end of else

        } // end of Main

    } // end of class


} // end of namespace
