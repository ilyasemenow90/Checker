using System;
using System.Windows.Forms;

namespace Шашки
{
    public partial class WhomToPlay : Form
    {
        public WhomToPlay()
        {
            InitializeComponent();
        }

        private void button1Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Game_type = 0;
            Properties.Settings.Default.Save();
            Close();
        }

        private void button2Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Game_type = 1;
            Properties.Settings.Default.Save();
            Close();
        }
    }
}
