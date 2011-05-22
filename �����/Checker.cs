using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Шашки
{
    class Checker : PictureBox
    {
        public bool click; //Если по шашке нажали
        public bool color; //false = верхние (черные)  true = нижние
        public bool king;  //дамка или нет?
        public Point position;  //X (a,b,c,d...f)  Y(1,2,3,4...8)
        public bool fight; //Когда шашка забирает шашку противника значение становится true до начала следующего хода
        public bool knock; //Шашка сбита (состояние, когда шашка сбита со стола, но ещё не убрана  с него)
        public Checker() 
        { 
            click = false;
            king = false;
            fight = false;
            knock = false;
            position = new Point(-1, -1);
        }

        public void setDefaultValue()
        {
            click = false;
            king = false;
            fight = false;
            knock = false;
            position = new Point(-1, -1);
            BackColor = Color.Transparent;
        }

        public void setPosition(int x, int y) 
        {
            position.X = x;
            position.Y = y;
        }
    }
}
