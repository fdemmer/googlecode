using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PasteToFile
{
    internal partial class Options : Form
    {
        internal RegistryHandler Reg;

        internal Options(RegistryHandler reg)
        {
            Reg = reg;

            //TODO move text to ressource or configuration or something
            String sDate = "d :08/17/2000 \nD :Thursday, August 17, 2000\nf :Thursday, August 17, 2000 16:32\nF :Thursday, August 17, 2000 16:32:32\ng :08/17/2000 16:32\nG :08/17/2000 16:32:32\nm :August 17\nr :Thu, 17 Aug 2000 23:32:32 GMT\ns :2000-08-17T16:32:32\nt :16:32\nT :16:32:32\nu :2000-08-17 23:32:32Z\nU :Thursday, August 17, 2000 23:32:32\ny :August, 2000\ndddd, MMMM dd yyyy :Thursday, August 17 2000\nddd, MMM d \"'\"yy :Thu, Aug 17 '00\ndddd, MMMM dd :Thursday, August 17\nM/yy :8/00\ndd-MM-yy :17-08-00";
            String sTime = sDate; // TODO

            InitializeComponent();
            ToolTip ToolTips = new ToolTip();
            ToolTips.SetToolTip(label2, "Set the filename for new images. Use the following tags to create a name-mask: <date> <time> <ext>");
            ToolTips.SetToolTip(label3, sDate);
            ToolTips.SetToolTip(label4, sTime);
            ToolTips.SetToolTip(label5, "Set the path where to write files. Leave empty to paste into execution directory or when using context menu!");
            ToolTips.SetToolTip(label6, "Set the time in milliseconds how long the balloon notification in the tray will be visible. Set to 0 to disable.");
            ToolTips.SetToolTip(checkBox2, "Adds a \"PasteToFile here...\" item to the context menu in explorer.");
            ToolTips.AutomaticDelay = 100; // time until popup
            ToolTips.AutoPopDelay = 10000; // time popup remains visible

            loadRegistrySettings();
        }

        private void loadDefaultSettings()
        {
            textBox1.Text = Ressource.Default_Mask_File;
            textBox2.Text = Ressource.Default_Mask_Date;
            textBox3.Text = Ressource.Default_Mask_Time;
            textBox4.Text = Ressource.Default_OutputPath;
            textBox5.Text = Ressource.Default_BalloonTimeout.ToString();
            comboBox1.SelectedIndex = Ressource.Default_ImageFormat;
            checkBox1.Checked = Ressource.Default_UpperCaseExt;
            checkBox2.Checked = Ressource.Default_ContextItem;
        }

        private void loadRegistrySettings()
        {
            textBox1.Text = (String)Reg.Config.GetValue(Ressource.RegKey_Mask_File, Ressource.Default_Mask_File);
            textBox2.Text = (String)Reg.Config.GetValue(Ressource.RegKey_Mask_Date, Ressource.Default_Mask_Date);
            textBox3.Text = (String)Reg.Config.GetValue(Ressource.RegKey_Mask_Time, Ressource.Default_Mask_Time);
            textBox4.Text = (String)Reg.Config.GetValue(Ressource.RegKey_OutputPath, Ressource.Default_OutputPath);
            textBox5.Text = (String)Reg.Config.GetValue(Ressource.RegKey_BalloonTimeout, Ressource.Default_BalloonTimeout).ToString();
            comboBox1.SelectedIndex = (int)Reg.Config.GetValue(Ressource.RegKey_ImageFormat, Ressource.Default_ImageFormat);
            checkBox1.Checked = bool.Parse((String)Reg.Config.GetValue(Ressource.RegKey_UpperCaseExt, Ressource.Default_UpperCaseExt));
            checkBox2.Checked = Reg.existingContextItem();
        }

        private void saveRegistrySettings()
        {
            Reg.Config.SetValue(Ressource.RegKey_Mask_File, textBox1.Text);
            Reg.Config.SetValue(Ressource.RegKey_Mask_Date, textBox2.Text);
            Reg.Config.SetValue(Ressource.RegKey_Mask_Time, textBox3.Text);
            Reg.Config.SetValue(Ressource.RegKey_OutputPath, textBox4.Text);
            Reg.Config.SetValue(Ressource.RegKey_BalloonTimeout, int.Parse(textBox5.Text));
            Reg.Config.SetValue(Ressource.RegKey_ImageFormat, comboBox1.SelectedIndex);
            Reg.Config.SetValue(Ressource.RegKey_UpperCaseExt, checkBox1.Checked);

            if (checkBox2.Checked == true)
                Reg.registerContextItem("PasteToFile here...", Application.ExecutablePath);
            else
                Reg.deleteContextItem();
        }

        // "OK" Button
        private void button1_Click(object sender, EventArgs e)
        {
            this.saveRegistrySettings();
            this.Close();
        }

        // "Cancel" Button
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // "Reset" Button
        private void button3_Click(object sender, EventArgs e)
        {
            this.loadDefaultSettings();
        }

        // "Browse" Button
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