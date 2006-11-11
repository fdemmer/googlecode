using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using Microsoft.Win32;

namespace PasteToFile
{
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

            // write file
            else if (Switches["here"] != null)
                prog.doPasteHere();

            // image upload
            else if (Switches["imageshack"] != null)
                prog.doPasteToImageshack();
            else if (Switches["flickr"] != null)
                prog.doPasteToFlickr();

            // text upload
            else if (Switches["pastebin"] != null)
                prog.doPasteToPastebin();

            else
                prog.doPaste();

        } // end of Main

    } // end of class

    class Ressource
    {
        internal static String Title        = "PasteToFile";
        internal static String Version      = "0.3";
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

        internal static String Default_Mask_File        = "clip-<date>-<time>.<ext>";
        internal static String Default_Mask_Date        = "yyyyMMdd";
        internal static String Default_Mask_Time        = "HHmmss";

        internal static String Default_OutputPath       = "";
        internal static String Default_UpperCaseExt     = "False";
        internal static String Default_ContextItem      = "True";
        internal static int    Default_ImageFormat      = 0;
        internal static int    Default_BalloonTimeout   = 3000;

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

        internal void doPasteToImageshack()
        {
            ImageshackPaster Pst = new ImageshackPaster(this);
            if (Pst.retriveData())
                Pst.writeData();
        }

    } // end of class

} // end of namespace
