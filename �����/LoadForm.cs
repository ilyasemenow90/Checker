using System;
using System.Windows.Forms;

namespace Шашки
{
    public partial class LoadForm : Form
    {
        public LoadForm()
        {
            InitializeComponent();
        }

        private void loadFormLoad(object sender, EventArgs e)
        {
            timer1.Start();
        }

        private void timer1Tick(object sender, EventArgs e)
        {
            progressBar1.Value += progressBar1.Step;
            if (progressBar1.Value >= progressBar1.Maximum - 3)
            {
                Close();
            }
        }
    }
}
