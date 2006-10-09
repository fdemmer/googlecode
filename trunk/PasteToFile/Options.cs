using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PasteToFile
{
    public partial class Options : Form
    {
        internal Registry Reg;

        internal Options(Registry Reg)
        {
            this.Reg = Reg;

            //TODO
            String sDate = "d :08/17/2000 \nD :Thursday, August 17, 2000\nf :Thursday, August 17, 2000 16:32\nF :Thursday, August 17, 2000 16:32:32\ng :08/17/2000 16:32\nG :08/17/2000 16:32:32\nm :August 17\nr :Thu, 17 Aug 2000 23:32:32 GMT\ns :2000-08-17T16:32:32\nt :16:32\nT :16:32:32\nu :2000-08-17 23:32:32Z\nU :Thursday, August 17, 2000 23:32:32\ny :August, 2000\ndddd, MMMM dd yyyy :Thursday, August 17 2000\nddd, MMM d \"'\"yy :Thu, Aug 17 '00\ndddd, MMMM dd :Thursday, August 17\nM/yy :8/00\ndd-MM-yy :17-08-00";
            String sTime = "d :08/17/2000 \nD :Thursday, August 17, 2000\nf :Thursday, August 17, 2000 16:32\nF :Thursday, August 17, 2000 16:32:32\ng :08/17/2000 16:32\nG :08/17/2000 16:32:32\nm :August 17\nr :Thu, 17 Aug 2000 23:32:32 GMT\ns :2000-08-17T16:32:32\nt :16:32\nT :16:32:32\nu :2000-08-17 23:32:32Z\nU :Thursday, August 17, 2000 23:32:32\ny :August, 2000\ndddd, MMMM dd yyyy :Thursday, August 17 2000\nddd, MMM d \"'\"yy :Thu, Aug 17 '00\ndddd, MMMM dd :Thursday, August 17\nM/yy :8/00\ndd-MM-yy :17-08-00";

            InitializeComponent();
            ToolTip ToolTips = new ToolTip();
            ToolTips.SetToolTip(label3, sDate);
            ToolTips.SetToolTip(label4, sTime);
            ToolTips.SetToolTip(label6, "Set the time in milliseconds how long the balloon notification in the tray will be visible. Set to 0 to disable.");
            ToolTips.SetToolTip(label2, "Set the filename for new images. Use the following tags to create a name-mask: <date> <time> <ext>");
            ToolTips.AutomaticDelay = 100;

            loadRegistrySettings();
        }

        private void loadDefaultSettings()
        {
            textBox1.Text = Resource.Default_Mask_File;
            textBox2.Text = Resource.Default_Mask_Date;
            textBox3.Text = Resource.Default_Mask_Time;
            textBox4.Text = Resource.Default_OutputPath;
            textBox5.Text = Resource.Default_BalloonTimeout.ToString();
            comboBox1.SelectedIndex = Resource.Default_ImageFormat;
        }

        private void loadRegistrySettings()
        {
            textBox1.Text = (String)Reg.Key.GetValue(Resource.RegKey_Mask_File, Resource.Default_Mask_File);
            textBox2.Text = (String)Reg.Key.GetValue(Resource.RegKey_Mask_Date, Resource.Default_Mask_Date);
            textBox3.Text = (String)Reg.Key.GetValue(Resource.RegKey_Mask_Time, Resource.Default_Mask_Time);
            textBox4.Text = (String)Reg.Key.GetValue(Resource.RegKey_OutputPath, Resource.Default_OutputPath);
            textBox5.Text = (String)Reg.Key.GetValue(Resource.RegKey_BalloonTimeout, Resource.Default_BalloonTimeout).ToString();
            comboBox1.SelectedIndex = (int)Reg.Key.GetValue(Resource.RegKey_ImageFormat, Resource.Default_ImageFormat);
        }

        private void saveRegistrySettings()
        {
            Reg.Key.SetValue(Resource.RegKey_Mask_File, textBox1.Text);
            Reg.Key.SetValue(Resource.RegKey_Mask_Date, textBox2.Text);
            Reg.Key.SetValue(Resource.RegKey_Mask_Time, textBox3.Text);
            Reg.Key.SetValue(Resource.RegKey_OutputPath, textBox4.Text);
            Reg.Key.SetValue(Resource.RegKey_BalloonTimeout, int.Parse(textBox5.Text));
            Reg.Key.SetValue(Resource.RegKey_ImageFormat, comboBox1.SelectedIndex);
        }

        // OK
        private void button1_Click(object sender, EventArgs e)
        {
            this.saveRegistrySettings();
            this.Close();
        }

        // Cancel
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Reset
        private void button3_Click(object sender, EventArgs e)
        {
            this.loadDefaultSettings();
        }

        // Browse
        private void button4_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "Select the output folder for your images.";
            DialogResult result = dialog.ShowDialog();
            if(result == DialogResult.OK)
                textBox4.Text = dialog.SelectedPath;
        }


    }
}