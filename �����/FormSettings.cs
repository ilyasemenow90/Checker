using System;
using System.Windows.Forms;

namespace Шашки
{
    public partial class FormSettings : Form
    {
        public FormSettings()
        {
            InitializeComponent();
            int hardLevel = Properties.Settings.Default.HardLevel;
            RadioButton radioButton;
            if (hardLevel == 0)
            {
                radioButton = radioButton1;
            }
            else if (hardLevel == 1)
            {
                radioButton = radioButton2;
            }
            else if (hardLevel == 2)
            {
                radioButton = radioButton3;
            }
            else
            {
                radioButton = null;
            }
            if (radioButton != null) radioButton.Checked = true;
            checkBox1.Checked = !Properties.Settings.Default.Player1_color;
            checkBox2.Checked = Properties.Settings.Default.HighlightedMove;
            checkBox3.Checked = Properties.Settings.Default.SaveGameBeforeExit;
            checkBox4.Checked = Properties.Settings.Default.PlaySound;
            checkBox5.Checked = Properties.Settings.Default.PlayBackgroundMusic;
        }

        private void exitFromForm()
        {
            Close();
        }

        private void button2Click(object sender, EventArgs e)
        {
            exitFromForm();
        }

        private void saveSettingClick(object sender, EventArgs e)
        {
            int number = 0;
            if (radioButton2.Checked)
            {
                number = 1;
            }
            if (radioButton3.Checked)
            {
                number = 2;
            }
            Properties.Settings.Default.HardLevel = number;

            Properties.Settings.Default.Player1_color = !checkBox1.Checked;
            Properties.Settings.Default.HighlightedMove = checkBox2.Checked;
            Properties.Settings.Default.SaveGameBeforeExit = checkBox3.Checked;
            Properties.Settings.Default.PlaySound = checkBox4.Checked;
            Properties.Settings.Default.PlayBackgroundMusic =checkBox5.Checked;

            Properties.Settings.Default.Save();

            Form1.realForm.refreshImage();
            exitFromForm();
        }
    }
}
