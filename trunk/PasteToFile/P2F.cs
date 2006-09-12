/*

PasteToFile

Version     0.1
Constact    <florian@demmer.org>
WWW         http://florian.demmer.org
License     GPL 2.0

requires .net Framework 1.1

*/


using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace p2f
{
    class GUI
    {
        internal static void showAbout()
        {
            Application.Run(new About());
        }

        internal static void showOptions()
        {
            Application.Run(new Options());
        }
    }

	class simpleP2F
	{
        [STAThread]
        static void Main(string[] args)
        {
            String filename;

            // parse commandline arguments
            CommandLine.Utility.Arguments Switches = new CommandLine.Utility.Arguments(args);

            if (Switches["about"] != null)
                GUI.showAbout();
            else if (Switches["options"] != null)
                GUI.showOptions();
            else
            {
                Microsoft.Win32.RegistryKey hklm = Microsoft.Win32.Registry.CurrentUser;
                Microsoft.Win32.RegistryKey regKey = hklm.CreateSubKey("Software\\PasteToFile");
                
                String prefix = (String)regKey.GetValue("prefix", "clip_"); // filename prefix

                // generate timestamp and set filename
                DateTime d = DateTime.Now;

                String timestamp = d.ToString((String)regKey.GetValue("timestamp", "yyyyMMddHHmmss"));

                // check if an image is available
                if (Clipboard.GetDataObject() != null)
                {
                    IDataObject obj = Clipboard.GetDataObject();
                    if (obj.GetDataPresent(DataFormats.Bitmap))
                    {
                        // fetch image data as bitmap
                        Image img = (Image)obj.GetData(DataFormats.Bitmap);
                        try
                        {
                            // store in the current workdirectory as png
                            //  "png", "jpeg", "gif", "bmp", "tiff" 
                            switch ((int)regKey.GetValue("imageformat", 0))
                            {
                                case 0:
                                    filename = String.Format("{0}{1}{2}", prefix, timestamp, ".png");
                                    img.Save(filename, ImageFormat.Png);
                                    break;
                                case 1:
                                    filename = String.Format("{0}{1}{2}", prefix, timestamp, ".jpg");
                                    img.Save(filename, ImageFormat.Jpeg);
                                    break;
                                case 2:
                                    filename = String.Format("{0}{1}{2}", prefix, timestamp, ".gif");
                                    img.Save(filename, ImageFormat.Gif);
                                    break;
                                case 3:
                                    filename = String.Format("{0}{1}{2}", prefix, timestamp, ".bmp");
                                    img.Save(filename, ImageFormat.Bmp);
                                    break;
                                case 4:
                                    filename = String.Format("{0}{1}{2}", prefix, timestamp, ".tif");
                                    img.Save(filename, ImageFormat.Tiff);
                                    break;
                            }
                        }
                        catch (System.Security.SecurityException)
                        {
                            System.Windows.Forms.MessageBox.Show("Sorry, this cannot be run from a network drive!",
                                "SecurityException", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                    }
                    else
                    {
                        System.Windows.Forms.MessageBox.Show("Sorry, there is no useable bitmap data in the clipboard!",
                            "No picture found!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
            }

        }

	}


}
