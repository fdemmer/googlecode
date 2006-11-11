using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace PasteToFile
{

    internal class Paster
    {
        internal Program Prog = null;
        internal Image ImageData = null;
        internal String TextData = null;

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
                Prog.doBalloon("Sorry, the clipboard seems to be empty!", Ressource.Title, ToolTipIcon.Warning);
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

            Prog.doBalloon("Sorry, there is no useable data in the clipboard!", Ressource.Title, ToolTipIcon.Warning);
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

            // create filename
            String Output;
            if (here)
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
            String Output;
            if (here)
                Output = Directory.GetCurrentDirectory() + "\\" + getFilename("txt");
            else
                Output = getPath() + getFilename("txt");

            //TODO catch exceptions
            StreamWriter stream = File.CreateText(Output);
            stream.Write(TextData);
            stream.Close();
            Prog.doBalloon("\"" + Output + "\" successfully written...", Ressource.Title, ToolTipIcon.Info);

            return true;
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

    /// <summary>
    /// Paster for uploading images to imageshack.us
    /// </summary>
    internal class ImageshackPaster : Paster
    {
        public ImageshackPaster(Program prog)
            : base(prog)
        {

        }

        /// <summary>
        /// writer for image data only
        /// </summary>
        /// <returns>boolean for success of write</returns>
        internal bool writeData()
        {
            if (ImageData != null)
                return writeImage();
            return false;
        }

        /// <summary>
        /// imagte uploader
        /// </summary>
        /// <returns>boolean for success of upload</returns>
        internal bool writeImage()
        {
            ImageData.Save("name", ImageFormat.Png);
            Prog.doBalloon("\"" + "name" + "\" successfully written...", Ressource.Title, ToolTipIcon.Info);

            return false;
        }
    }

}
