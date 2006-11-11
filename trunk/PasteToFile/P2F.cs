/*

PasteToFile

Version     0.2.1
Constact    <florian@demmer.org>
WWW         http://florian.demmer.org
License     GPL 2.0

*/


using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using Microsoft.Win32;

namespace PasteToFile
{

    class RegistryHandler
    {
        private Program Prog;

        private RegistryKey hkcu;       // HKEY_CURRENT_USER
        private RegistryKey hkcr;       // HKEY_CLASSES_ROOT

        private String PathText;        // path of registry key for contextmenu text
        private String PathCommand;     // path of registry key for contextmenu command 
        private String PathSoftware;    // path of registry key for configuration

        internal RegistryKey Config;    // registry key for configuration key/values
        private RegistryKey RegMenu;    // registry key for contextmenu text
        private RegistryKey RegCommand; // registry key for contextmenu command

        public RegistryHandler(Program prog)
        {
            Prog = prog;

            hkcu = Registry.CurrentUser;
            hkcr = Registry.ClassesRoot;

            PathText = "Folder\\shell\\" + Ressource.Title;
            PathCommand = "Folder\\shell\\" + Ressource.Title + "\\command";
            PathSoftware = "Software\\";

            // open or create configuration key, writeable
            Config = openOrCreateKey(PathSoftware + Ressource.Title, hkcu);
        }

        // open read only or writeable
        private Microsoft.Win32.RegistryKey openKey(String name, RegistryKey hierachy, bool writeable)
        {
            RegistryKey key = null;

            try
            {
                // open subkey
                key = hierachy.OpenSubKey(name, writeable);
            }
            catch (System.ObjectDisposedException)
            {
                // registry key is closed and connot be opened
            }
            catch (System.Security.SecurityException)
            {
                // insufficient rights to access key
            }

            return key;
        }

        // create or open writeable
        private RegistryKey openOrCreateKey(String name, RegistryKey hierachy)
        {
            RegistryKey key = null;

            try
            {
                // open/create *key* with read/write in *hierachy*
                key = hierachy.CreateSubKey(name);
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

            return key;
        }

        internal void registerContextItem(String text, String command)
        {
            try
            {
                RegMenu = Registry.ClassesRoot.CreateSubKey(PathText);
                if (RegMenu != null)
                    RegMenu.SetValue("", text);
                RegCommand = Registry.ClassesRoot.CreateSubKey(PathCommand);
                if (RegCommand != null)
                    RegCommand.SetValue("", command);
            }
            catch (Exception ex)
            {
                Prog.doBalloon(ex.ToString(), Ressource.Title, ToolTipIcon.Error);
            }
            finally
            {
                if (RegMenu != null)
                    RegMenu.Close();
                if (RegCommand != null)
                    RegCommand.Close();
            }
        }

        internal void deleteContextItem()
        {
            RegistryKey RegKey = null;
            try
            {
                RegKey = hkcr.OpenSubKey(PathCommand);
                if (RegKey != null)
                {
                    RegKey.Close();
                    hkcr.DeleteSubKey(PathCommand);
                }
                RegKey = Registry.ClassesRoot.OpenSubKey(PathText);
                if (RegKey != null)
                {
                    RegKey.Close();
                    hkcr.DeleteSubKey(PathText);
                }
            }
            catch (Exception ex)
            {
                Prog.doBalloon(ex.ToString(), Ressource.Title, ToolTipIcon.Error);
            }
        }

        internal bool existingContextItem()
        {
            bool retval = false;

            try
            {
                RegCommand = hkcr.OpenSubKey(PathCommand);
                RegMenu = hkcr.OpenSubKey(PathText);
                if (RegCommand != null && RegMenu != null)
                    retval = true;
            }
            catch (Exception ex)
            {
                Prog.doBalloon(ex.ToString(), Ressource.Title, ToolTipIcon.Error);
            }

            return retval;
        }

    } // end of class

    class Ressource
    {
        internal static String Title        = "PasteToFile";
        internal static String Version      = "0.2.1";
        internal static String Author       = "Florian Demmer";
        internal static String Website      = "http://florian.demmer.org";
        internal static String Email        = "florian@demmer.org";
        internal static String License      = "GPL 2.0";
    
        internal static String RegKey_Version           = "Version";
        internal static String RegKey_Mask_File         = "FileMask";
        internal static String RegKey_Mask_Date         = "DateMask";
        internal static String RegKey_Mask_Time         = "TimeMask";
        internal static String RegKey_OutputPath        = "OutputPath";
        internal static String RegKey_ImageFormat       = "ImageFormat";
        internal static String RegKey_UpperCaseExt      = "UppercaseExtension";
        internal static String RegKey_BalloonTimeout    = "BalloonTimeout";

        internal static String Mask_Date                = "<date>";
        internal static String Mask_Time                = "<time>";
        internal static String Mask_Extension           = "<ext>";

        internal static String Default_Mask_File        = "sshot-<date>-<time>.<ext>";
        internal static String Default_Mask_Date        = "yyyyMMdd";
        internal static String Default_Mask_Time        = "HHmmss";

        internal static String Default_OutputPath       = "";
        internal static String Default_UpperCaseExt     = "False";
        internal static String Default_ContextItem      = "True";
        internal static int    Default_ImageFormat      = 0;
        internal static int    Default_BalloonTimeout   = 3000;

    } // end of class

    class Paster
    {
        Program Prog = null;
        Image ImageData = null;
        String TextData = null;

        public Paster(Program prog)
        {
            Prog = prog;
        }

        internal bool retriveData()
        {
            IDataObject RawData = null;

            try
            {
                RawData = Clipboard.GetDataObject();
            }
            catch (System.Runtime.InteropServices.ExternalException ex)
            {
                // clipboard is probably used by another process
                Prog.doBalloon(ex.ToString(), Ressource.Title, ToolTipIcon.Error);
                return false;
            }

            if (RawData == null)
            {
                //doBalloon("Sorry, the clipboard seems to be empty!", Ressource.Title, ToolTipIcon.Warning);
                return false;
            }

            // get image data from the clipboard
            if (RawData.GetDataPresent(DataFormats.Bitmap))
            {
                ImageData = (Image)RawData.GetData(DataFormats.Bitmap);
                return true;
            }

            // get text data from the clipboard
            if (RawData.GetDataPresent(DataFormats.Text))
            {
                TextData = (String)RawData.GetData(DataFormats.Text);
                return true;
            }

            //doBalloon("Sorry, there is no useable image data in the clipboard!", Ressource.Title, ToolTipIcon.Warning);
            return false;
        }

        internal bool writeData(bool here)
        {
            if (ImageData != null)
                return writeImage(here);
            if (TextData != null)
                return writeText(here);
            return false;
        }

        internal bool writeImage(bool here)
        {
            ImageFormat Format; 
            switch ((int)Prog.Reg.Config.GetValue(Ressource.RegKey_ImageFormat, Ressource.Default_ImageFormat))
            {
                case 0:
                    Format = ImageFormat.Png; 
                    break;
                case 1:
                    Format = ImageFormat.Jpeg; 
                    break;
                case 2:
                    Format = ImageFormat.Gif; 
                    break;
                case 3:
                    Format = ImageFormat.Bmp; 
                    break;
                case 4:
                    Format = ImageFormat.Tiff;
                    break;
                default:
                    Format = ImageFormat.Png; 
                    break;
            }

            String Extension = Format.ToString().ToLower();

            // make extension uppercase
            //TODO this seems to be b0rked.
            if (bool.Parse((String)Prog.Reg.Config.GetValue(Ressource.RegKey_UpperCaseExt, Ressource.Default_UpperCaseExt)))
                Extension.ToUpper();

            // create a filename
            String Output; 
            if(here)
                Output = Directory.GetCurrentDirectory() + "\\" + getFilename(Extension);
            else
                Output = getPath() + getFilename(Extension);

            try
            {
                ImageData.Save(Output, Format);
                Prog.doBalloon("\"" + Output + "\" successfully written...", Ressource.Title, ToolTipIcon.Info);
                return true;
            }
            catch (System.Runtime.InteropServices.ExternalException)
            {
                Prog.doBalloon("System.Runtime.InteropServices.ExternalException", Ressource.Title, ToolTipIcon.Warning);
            }
            catch (System.Security.SecurityException)
            {
                Prog.doBalloon("Sorry, this cannot be run on a network drive!", Ressource.Title, ToolTipIcon.Warning);
            }
            catch (System.ArgumentException)
            {
                Prog.doBalloon("Check your filename for invalid characters! (" + Output + ")", Ressource.Title, ToolTipIcon.Warning);
            }
            catch (System.NotSupportedException)
            {
                Prog.doBalloon("Check your filename for invalid characters! (" + Output + ")", Ressource.Title, ToolTipIcon.Warning);
            }
            return false;
        }

        internal bool writeText(bool here)
        {
            return false;
        }

        private String getFilename(String extension)
        {
            // retrieve filename mask setting
            String name = (String)Prog.Reg.Config.GetValue(Ressource.RegKey_Mask_File, Ressource.Default_Mask_File);

            // generate timestamps
            DateTime d = DateTime.Now;
            String date = d.ToString((String)Prog.Reg.Config.GetValue(Ressource.RegKey_Mask_Date, Ressource.Default_Mask_Date));
            String time = d.ToString((String)Prog.Reg.Config.GetValue(Ressource.RegKey_Mask_Time, Ressource.Default_Mask_Time));

            // replace tags in filemask
            if (name.Contains(Ressource.Mask_Date))
                name = name.Replace(Ressource.Mask_Date, date);
            if (name.Contains(Ressource.Mask_Time))
                name = name.Replace(Ressource.Mask_Time, time);
            if (name.Contains(Ressource.Mask_Extension))
                name = name.Replace(Ressource.Mask_Extension, extension);

            return name;
        }

        private String getPath()
        {
            String path = (string)Prog.Reg.Config.GetValue(Ressource.RegKey_OutputPath, Ressource.Default_OutputPath);

            if (path == "")
            {
                return Directory.GetCurrentDirectory() + "\\";
            }

            if (!System.IO.Directory.Exists(path))
            {
                Prog.doBalloon("Directory \"" + path + "\" does not exist!\n Writing file to current directory.", Ressource.Title, ToolTipIcon.Error);
                return Directory.GetCurrentDirectory() + "\\";
            }

            /*
             *    .    does not work, use just ""
             *    ""   empty... pastes to current directory
             *    /    root of the drive p2f runs on
             *    ../  one directoy up of place where p2f is run
             *    c:\  well... c: :)
             * 
             */

            return path + "\\";
        }

    } // end of class

    class Program
    {
        internal RegistryHandler Reg;
        internal NotifyIcon TrayIcon;

        public Program()
        {
            // access registry
            Reg = new RegistryHandler(this);

            // prepare notification area
            TrayIcon = new System.Windows.Forms.NotifyIcon();
            TrayIcon.Icon = Resources.IconApp;
        }

        internal void showAbout()
        {
            Application.Run(new About());
        }

        internal void showOptions()
        {
            Application.Run(new Options(Reg));
        }
        
        internal void doBalloon(String Message, String Title, ToolTipIcon Icon)
        {
            int timeout = (int)Reg.Config.GetValue(Ressource.RegKey_BalloonTimeout, Ressource.Default_BalloonTimeout);
            TrayIcon.Visible = true;
            TrayIcon.ShowBalloonTip(0, Title, Message, Icon);
            System.Threading.Thread.Sleep(timeout); //TODO: that's ugly, but until we have a persistent tray icon...
            TrayIcon.Visible = false;
        }

        // paste image to current directory
        internal void doPasteHere()
        {
            Paster Pst = new Paster(this);
            if (Pst.retriveData())
                Pst.writeData(true);
        }

        // paste image to default directory
        internal void doPaste()
        {
            Paster Pst = new Paster(this);
            if (Pst.retriveData())
                Pst.writeData(false);
        }

    } // end of class

    class PasteToFile
    {
        [STAThread]
        static void Main(string[] args)
        {
            // create program instance
            Program prog = new Program();

            // parse commandline arguments
            CommandLine.Utility.Arguments Switches = new CommandLine.Utility.Arguments(args);

            // evaluate commandline arguments
            if (Switches["about"] != null)
                prog.showAbout();
            else if (Switches["help"] != null)
                prog.showAbout();
            else if (Switches["options"] != null)
                prog.showOptions();
            else if (Switches["setup"] != null)
                prog.showOptions();
            else if (Switches["here"] != null)
                prog.doPasteHere();
            else
                prog.doPaste();

        } // end of Main

    } // end of class


} // end of namespace
