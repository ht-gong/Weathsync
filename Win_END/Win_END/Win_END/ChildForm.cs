using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Win_END
{
    public partial class ChildForm : Form
    {
        public ChildForm()
        {
            InitializeComponent();
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            if (HelpCheckBox.Checked == true)
                Properties.Settings.Default.Help_Show = false;
            Properties.Settings.Default.Save();
            this.Close();
        }
    }
}
