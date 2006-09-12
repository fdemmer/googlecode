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

namespace P2F
{
	class simpleP2F
	{
        [STAThread]
        static void Main(string[] args)
        {
            String prefix = "clip_"; // filename prefix

            // generate timestamp and set filename
            DateTime d = DateTime.Now;
            String timestamp = d.ToString("yyyyMMddHHmmss");
            String filename = String.Format("{0}{1}.png",prefix,timestamp);
        	
            // check if an image is available
            if(Clipboard.GetDataObject() != null)
            {
	            IDataObject obj = Clipboard.GetDataObject();
                if (obj.GetDataPresent(DataFormats.Bitmap))
                {
                    // fetch image data as bitmap
                    Image img = (Image)obj.GetData(DataFormats.Bitmap);
                    try
                    {
                        // store in the current workdirectory as png
                        img.Save(filename, ImageFormat.Png);
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
