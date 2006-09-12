using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace p2f
{
    public partial class Options : Form
    {
        public Options()
        {
            String sDate = "d :08/17/2000 \nD :Thursday, August 17, 2000\nf :Thursday, August 17, 2000 16:32\nF :Thursday, August 17, 2000 16:32:32\ng :08/17/2000 16:32\nG :08/17/2000 16:32:32\nm :August 17\nr :Thu, 17 Aug 2000 23:32:32 GMT\ns :2000-08-17T16:32:32\nt :16:32\nT :16:32:32\nu :2000-08-17 23:32:32Z\nU :Thursday, August 17, 2000 23:32:32\ny :August, 2000\ndddd, MMMM dd yyyy :Thursday, August 17 2000\nddd, MMM d \"'\"yy :Thu, Aug 17 '00\ndddd, MMMM dd :Thursday, August 17\nM/yy :8/00\ndd-MM-yy :17-08-00";

            InitializeComponent();
            ToolTip ToolTips = new ToolTip();
            ToolTips.SetToolTip(textBox2, sDate);
            ToolTips.SetToolTip(label3, sDate);
            ToolTips.AutomaticDelay = 200;

            loadRegistrySettings();
        }

        private void loadDefaultSettings()
        {
            textBox2.Text = "yyyyMMddHHmmss";
            textBox1.Text = "clip_";
            comboBox1.SelectedIndex = 0;
        }

        private void loadRegistrySettings()
        {
            Microsoft.Win32.RegistryKey hklm = Microsoft.Win32.Registry.CurrentUser;
            Microsoft.Win32.RegistryKey regKey = hklm.CreateSubKey("Software\\PasteToFile");

            textBox1.Text = (String)regKey.GetValue("prefix", "clip_");
            textBox2.Text = (String)regKey.GetValue("timestamp", "yyyyMMddHHmmss");
            comboBox1.SelectedIndex = (int)regKey.GetValue("imageformat", 0);
        }

        private void saveRegistrySettings()
        {
            Microsoft.Win32.RegistryKey hklm = Microsoft.Win32.Registry.CurrentUser;
            Microsoft.Win32.RegistryKey regKey = hklm.CreateSubKey("Software\\PasteToFile");

            regKey.SetValue("prefix", textBox1.Text);
            regKey.SetValue("timestamp", textBox2.Text);
            regKey.SetValue("imageformat", comboBox1.SelectedIndex);
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


    }
}