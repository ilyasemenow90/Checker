using System;
using System.Windows.Forms;

namespace Шашки
{
    public partial class InsertPlayerNameForm : Form
    {
        private int _num;
        public InsertPlayerNameForm(int number)
        {
            InitializeComponent();
            _num = number;
        }

        private void button1Click(object sender, EventArgs e)
        {
            if (_num == 1)
            {
                Properties.Settings.Default.Player1 = textBox1.Text;
                Properties.Settings.Default.Save();
            }
            if (_num == 2)
            {
                Properties.Settings.Default.Player2 = textBox1.Text;
                Properties.Settings.Default.Save();
            }
            Close();
        }

        private void insertPlayerNameFormShown(object sender, EventArgs e)
        {
            Text = string.Format(@"Введите имя игрока {0}", _num);
            label1.Text = string.Format(@"Введите имя игрока {0}", _num);
            textBox1.Text = _num == 1 ? Properties.Settings.Default.Player1 : Properties.Settings.Default.Player2;
            textBox1.Focus();
        }

        private void textBox1KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                button1Click(null, null);
            }
        }
    }
}
