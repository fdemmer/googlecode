using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PasteToFile
{
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // show License
        private void button3_Click(object sender, EventArgs e)
        {
            String file = System.IO.Path.Combine(Application.StartupPath, "LICENSE.txt");
            try
            {
                System.Diagnostics.Process.Start(file);
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                MessageBox.Show(this, ex.ToString());
            }
        }

        // show help
        private void button2_Click(object sender, EventArgs e)
        {
            String file = System.IO.Path.Combine(Application.StartupPath, "README.txt");
            try
            {
                System.Diagnostics.Process.Start(file);
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                MessageBox.Show(this, ex.ToString());
            }
        }



    }
}
