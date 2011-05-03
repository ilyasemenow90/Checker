using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Collections;


namespace Шашки
{
    public partial class Form1 : Form
    {
        Checker[] checkerArray;  //массив шашек
        bool highlight;  //подсвечивать возможные ходы?
        bool step; //чей ход  true = верхние false = нижние
        string playerMove;

        [DllImport("SiDra.dll", CharSet = CharSet.Auto)]
        static extern void EI_MakeMove(StringBuilder move);

        [DllImport("SiDra.dll", CharSet = CharSet.Auto)]
        static extern StringBuilder EI_Think();

        [DllImport("SiDra.dll", CharSet = CharSet.Auto)]
        static extern StringBuilder EI_PonderHit(StringBuilder move);

        [DllImport("SiDra.dll", CharSet = CharSet.Auto)]
        static extern void EI_Initialization(PF_SearchInfo si, int mem_lim);

        [DllImport("SiDra.dll", CharSet = CharSet.Auto)]
        static extern void EI_NewGame();

        [DllImport("SiDra.dll", CharSet = CharSet.Auto)]
        static extern void EI_Stop();

        [DllImport("SiDra.dll", CharSet = CharSet.Auto)]
        static extern void EI_SetupBoard(StringBuilder pos);

        [DllImport("SiDra.dll", CharSet = CharSet.Auto)]
        static extern void EI_SetTimeControl(int time, int inc);

        [DllImport("SiDra.dll", CharSet = CharSet.Auto)]
        static extern void EI_SetTime(int time, int otime);

        [DllImport("SiDra.dll", CharSet = CharSet.Auto)]
        static extern StringBuilder EI_GetName();

        [DllImport("SiDra.dll", CharSet = CharSet.Auto)]
        static extern void EI_OnExit();

        delegate void PF_SearchInfo(int score, int depth, int speed, StringBuilder pv, StringBuilder cm);
        static PF_SearchInfo method = new PF_SearchInfo(Form1.seacrResultDelegate);

        static Form1 realForm = null;


        public Form1()
        {
            InitializeComponent();
            highlight = false;
            step = false; //первые ходят нижние
            checkerArray = new Checker[24];
            for (int i = 0; i < checkerArray.Length; i++)
            {
                checkerArray[i] = new Checker();
            }
            this.createCheckers(0, 12, true);
            this.createCheckers(12, 24, false);
            realForm = this;
        }


        /// <summary>
        ///   Создает шашки для игры
        /// </summary>
        private void createCheckers(int start, int end, bool color)
        {
            int deltaX = 0;
            int deltaY = 0;
            bool startZero;
            if (start != 0)
            {
                startZero = true;
                deltaY = 5;
            }
            else
            {
                startZero = false;
            }

            for (int i = start; i < end; i++)
            {
                Checker asd = checkerArray[i];

                int newX = startZero ? deltaX * 2 + 1 : deltaX * 2 + 2;
                int newY = 8 - deltaY;

                asd.setPosition(newX, newY);
                pictureBox1.Controls.Add(asd);
                asd.Size = new Size(50, 50);
                int posX = startZero ? 0 : 50;
                asd.Location = new Point(posX + 100 * deltaX, 0 + 50 * deltaY);
                asd.SizeMode = PictureBoxSizeMode.StretchImage;
                asd.Image = color == true ? Properties.Resources.Шашка_1 : Properties.Resources.Шашка_2;
                asd.color = color;
                asd.BackColor = System.Drawing.Color.Transparent;
                asd.Click += new EventHandler(pictureBox_Click);
                asd.BringToFront();

                ++deltaX;
                if (deltaX == 4)
                {
                    ++deltaY;
                    deltaX = 0;
                    startZero = !startZero;
                }
            }
        }


        /// <summary>
        ///   Конвертирует позицию в строку для передачи движку
        /// </summary>
        private string convertPointToCheckerString(int x, int y)
        {
            return (Convert.ToChar(97 + x - 1).ToString() + Convert.ToChar(48 + y).ToString());
        }

        /// <summary>
        ///   Проверяет может ли шашка забрать другую шашку
        /// </summary>
        private bool fightChecker(Checker ch)
        {
            if (ch.position.X < 1 || ch.position.Y < 1 || ch.position.X > 8 || ch.position.Y > 8)
            {
                return false;
            }
            bool canFight = false;
            bool down = ch.color;
            if (ch.king)
            {
                Point activePoint = ch.position;
                for (int j = 0; j < 2; j++)
                {
                    int deltaX = j == 0 ? -1 : 1; //сначала влево, потом вправо проверяем
                    if (canFight)
                    {
                        break;
                    }
                    for (int k = 0; k < 2; k++)
                    {
                        int deltaY = k == 0 ? -1 : 1; //вверх, потом вниз
                        if (canFight)
                        {
                            break;
                        }
                        for (int i = 1; i < 9; i++)
                        {
                            
                            if (canFight)
                            {
                                break;
                            }
                            Checker chnew = this.checkerFromPosition(new Point(activePoint.X + deltaX * i, activePoint.Y + deltaY * i)); //берем все шашки во всех направлениях
                            if (chnew != null)
                            {
                                if (chnew.color != ch.color)
                                {
                                    int xMove = activePoint.X + deltaX * (i + 1);
                                    int yMove = activePoint.Y + deltaY * (i + 1);
                                    Point killMove = new Point(xMove, yMove);
                                    if (this.checkerFromPosition(killMove) == null && killMove.X > 0 && killMove.X < 9
                                        && killMove.Y > 0 && killMove.Y < 9)  //значит можно бить шашку
                                    {
                                        canFight = true;
                                        break;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                else
                                {
                                    //наткнулись на шашки нашего цвета, значит дальше в этом направлении проверять смысла нет
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                Point activePoint = ch.position;
                //сначала делаем проверку на то, а можем ли мы побить какую-нибуь шашку рядом
                Checker[] chArray = new Checker[4];
                chArray[0] = this.checkerFromPosition(new Point(activePoint.X - 1, activePoint.Y + 1)); //верхняя левая
                chArray[1] = this.checkerFromPosition(new Point(activePoint.X + 1, activePoint.Y + 1)); //верхняя правая
                chArray[2] = this.checkerFromPosition(new Point(activePoint.X - 1, activePoint.Y - 1)); //нижняя левая
                chArray[3] = this.checkerFromPosition(new Point(activePoint.X + 1, activePoint.Y - 1)); //нижняя правая
                for (int i = 0; i < chArray.Length; i++)
                {
                    Checker chFind = chArray[i];
                    if (chFind != null)
                    {
                        if (ch.color != chFind.color)//шашки разные
                        {
                            int xMove = chFind.position.X - ch.position.X;
                            int yMove = chFind.position.Y - ch.position.Y;
                            Point killMove = new Point(chFind.position.X + xMove, chFind.position.Y + yMove);
                            if (this.checkerFromPosition(killMove) == null && killMove.X > 0 && killMove.X < 9 && killMove.Y > 0 && killMove.Y < 9)  //значит можно бить шашку
                            {
                                canFight = true;
                            }
                        }
                    }
                }
            }
            return canFight;
        }

        /// <summary>
        ///   Проверяет может ли шашка сделать ход
        /// </summary>
        //переписать проверку для дамок и переписать проверку, а если мы около конца стола стоим???
        private bool checkerCanMove(Checker ch)
        {
            if (ch.position.X < 1 || ch.position.Y < 1 || ch.position.X > 8 || ch.position.Y > 8)
            {
                return false;
            }
            bool canMove = false;
            bool down = ch.color;

            Point activePoint = ch.position;
            //сначала делаем проверку на то, а можем ли мы пойти
            Checker[] chArray = new Checker[2];
            if (down)
            {
                chArray[0] = this.checkerFromPosition(new Point(activePoint.X - 1, activePoint.Y - 1)); //нижняя левая
                chArray[1] = this.checkerFromPosition(new Point(activePoint.X + 1, activePoint.Y - 1)); //нижняя правая
            }
            else
            {
                chArray[0] = this.checkerFromPosition(new Point(activePoint.X - 1, activePoint.Y + 1)); //верхняя левая
                chArray[1] = this.checkerFromPosition(new Point(activePoint.X + 1, activePoint.Y + 1)); //верхняя правая
            }

            for (int i = 0; i < chArray.Length; i++)
            {
                if (chArray[i] == null)
                {
                    canMove = true;
                    break;
                }
            }
            
            return canMove;
        }

        /// <summary>
        ///   Возвращает все шашки, которые могут ходить
        /// </summary>
        private Checker[] solveCheckers()
        {
            ArrayList solveArray = new ArrayList();
            //Проверка на шашки, которые должны бить другие шашки
            foreach (Checker obj in checkerArray)
            {
                if (obj.color == step & fightChecker(obj))
                {
                    solveArray.Add(obj);
                }
            }
            if (solveArray.Count == 0) //Если ни одна из шашек не бьет, значит надо проверить на простые ходы
            {
                foreach (Checker obj in checkerArray)
                {
                    if (obj.color == step & checkerCanMove(obj))
                    {
                        solveArray.Add(obj);
                    }
                }
            }
            Checker[] retCh = new Checker[solveArray.Count];
            for (int i = 0; i < solveArray.Count; i++ )
            {
                retCh[i] = (Checker)solveArray[i];
            }
            return retCh;
        }


        /// <summary>
        ///   Обработчик нажатий шашек
        /// </summary>
        private void pictureBox_Click(object sender, EventArgs e)
        {
            Checker active = (Checker)sender;
            foreach (Checker obj in checkerArray)
            {
                if (obj.click & obj.color != active.color)
                {
                    return;
                }
                if (obj.fight)
                {
                    obj.click = true;
                    pictureBox1.Invalidate();
                    return;
                }
            }

            Checker[] canMoveCheckers = solveCheckers();

            foreach (Checker obj in canMoveCheckers)
            {
                if (obj == sender)
                {
                    obj.click = true;
                    highlight = true;
                    pictureBox1.Invalidate();
                    //a - 97   
                    //b - 98   
                    //c - 99   7 - 55
                    playerMove = this.convertPointToCheckerString(obj.position.X, obj.position.Y);
                    this.Text = playerMove;
                }
                else
                {
                    obj.click = false;
                }
            }
            pictureBox1.Invalidate();
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {

        }

        /// <summary>
        ///   Возвращает активную шашку
        /// </summary>
        private Checker getActiveChecker()
        {
            Checker retCh = null;
            foreach (Checker obj in checkerArray)
            {
                if (obj.click == true)
                {
                    retCh = obj;
                    break;

                }
            }
            return retCh; 
        }

        /// <summary>
        ///   Возвращает шашку по номеру позиции на игровой доске
        /// </summary>
        private Checker checkerFromPosition(Point pos)
        {
            Checker objCh = null;
            if (pos.X < 1 || pos.Y < 1 || pos.X > 8 || pos.Y > 8)
            {
                return null;
            }
            foreach (Checker obj in checkerArray)
            {
                if (obj.position.X == pos.X && obj.position.Y == pos.Y)
                {
                    objCh = obj;
                    break;
                }
            }
            return objCh;
        }


        /// <summary>
        ///   Вызывается, когда отпускается мышка на столе
        /// </summary>
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            Checker active = this.getActiveChecker();
            
            //
            if (null != active)
            {
                Point newPoint = getPointOnPosition(e.X, e.Y);
                Point newPointForCh = new Point(newPoint.X + 1, 8 - newPoint.Y);

                bool moveIsGood = false;
                bool fightMove = false;
                Point[] movesPoint = getMovesOnOneMove();
                foreach (Point a in movesPoint)
                {
                    if (a == newPointForCh)
                    {
                        moveIsGood = true;
                        if (active.king) 
                        {
                            int deltaForX = (newPointForCh.X - active.position.X) / Math.Abs(newPointForCh.X - active.position.X);
                            int deltaForY = (newPointForCh.Y - active.position.Y) / Math.Abs(newPointForCh.Y - active.position.Y);
                            for (int i = 1; i < 8; i++ )
                            {
                                Point searchPoint = new Point(active.position.X + deltaForX * i, active.position.Y + deltaForY * i);
                                Checker findCh = checkerFromPosition(searchPoint);
                                if (findCh != null && findCh.color != active.color)
                                {
                                    fightMove = true;
                                    active.fight = true;
                                    playerMove += ":" + convertPointToCheckerString(searchPoint.X, searchPoint.Y);
                                    this.Text = playerMove;
                                    deleteChecker(searchPoint);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            int deltaX = active.position.X - newPointForCh.X;
                            int deltaY = active.position.Y - newPointForCh.Y;
                            if (Math.Abs(deltaX) > 1 && Math.Abs(deltaY) > 1)
                            {
                                fightMove = true;
                                active.fight = true;
                                Point deleteCh = new Point(active.position.X, active.position.Y);
                                deleteCh.X -= deltaX < 0 ? -1 : 1;
                                deleteCh.Y -= deltaY < 0 ? -1 : 1;
                                playerMove += ":" + convertPointToCheckerString(deleteCh.X, deleteCh.Y);
                                this.Text = playerMove;
                                deleteChecker(deleteCh);
                            }
                        }
                        break;
                    }
                }
                if (moveIsGood)
                {
                    active.setPosition(newPointForCh.X, newPointForCh.Y);
                    if (!active.king)
                    {
                        if (active.color)
                        {
                            if (newPointForCh.Y == 1)
                            {
                                active.king = true;
                                active.Image = Properties.Resources.Шашка_1_дамка;
                            }
                        }
                        else
                        {
                            if (newPointForCh.Y == 8)
                            {
                                active.king = true;
                                active.Image = Properties.Resources.Шашка_2_дамка;
                            }
                        }
                    }
                    
                    
                    active.Location = new Point(newPoint.X * 50, newPoint.Y * 50);
                    pictureBox1.Invalidate();
                    if (!fightMove)
                    {
                        endMove(active);
                    } else if (!fightChecker(active))
                    {
                        endMove(active);
                    }
                    else
                    {
                        pictureBox1.Invalidate();
                    }
                }
               
            }


        }


        /// <summary>
        ///   Конец хода
        /// </summary>
        private void endMove(Checker activeCh)
        {
            activeCh.click = false;
            highlight = false;
            step = !step;
            String addString;
            if (activeCh.fight)
            {
                addString = ":" + convertPointToCheckerString(activeCh.position.X, activeCh.position.Y);
            }
            else
            {
                addString = convertPointToCheckerString(activeCh.position.X, activeCh.position.Y);
            }
            playerMove += addString;
            activeCh.fight = false;
            this.Text = playerMove;

            Checker[] find = solveCheckers();
            if (find.Length == 0)
            {
                gameEnded();
            }
        }


        /// <summary>
        ///   Игра окончена
        /// </summary>
        private void gameEnded()
        {
            DialogResult result1 = MessageBox.Show("Вы проиграли.\nЖелаете начать новую игру?", "Игра окончена", MessageBoxButtons.YesNo);
            if (result1 == DialogResult.Yes)
            {
                startNewGame();
            }
            else
            {

            }
        }

        /// <summary>
        ///   Начать новую игру
        /// </summary>
        private void startNewGame()
        {
            highlight = false;
            step = false;
        }


        /// <summary>
        ///   Возвращает все возможные ходы для активной шашки на один ход
        /// </summary>
        private Point[] getMovesOnOneMove()
        {
            Checker active = this.getActiveChecker();
            if (active == null)
            {
                return null;
            }
            bool down = false;
            if (active.color == true)
            {
                down = true;
            }
            ArrayList moveArray = new ArrayList(4);
            Point activePoint = active.position;

            if (active.king)
            {
                for (int j = 0; j < 2; j++)
                {
                    int deltaX = j == 0 ? -1 : 1; //сначала влево, потом вправо проверяем
                    for (int k = 0; k < 2; k++)
                    {
                        int deltaY = k == 0 ? -1 : 1; //вверх, потом вниз
                        for (int i = 1; i < 9; i++)
                        {
                            bool canKill = true;
                            Checker ch = this.checkerFromPosition(new Point(activePoint.X + deltaX * i, activePoint.Y + deltaY * i)); //берем все шашки во всех направлениях
                            if (ch != null)
                            {
                                if (ch.color != active.color)
                                {
                                    for (int l = 1; l < 8; l++) //проверяем все предполагаемые клетки за той, которую мы хотим "забрать"
                                    {
                                        int xMove = activePoint.X + deltaX * (i + l);
                                        int yMove = activePoint.Y + deltaY * (i + l);
                                        Point killMove = new Point(xMove, yMove);
                                        if (this.checkerFromPosition(killMove) == null && killMove.X > 0 && killMove.X < 9
                                            && killMove.Y > 0 && killMove.Y < 9)  //значит можно бить шашку
                                        {
                                            moveArray.Add(killMove);
                                        }
                                        else
                                        {
                                            canKill = false;
                                            break;
                                        }
                                    }
                                    if (!canKill)
                                    {
                                        break;
                                    }
                                }
                                else
                                {
                                    //наткнулись на шашки нашего цвета, значит дальше в этом направлении проверять смысла нет
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                //сначала делаем проверку на то, а можем ли мы побить какую-нибуь шашку рядом
                Checker[] chArray = new Checker[4];
                chArray[0] = this.checkerFromPosition(new Point(activePoint.X - 1, activePoint.Y + 1)); //верхняя левая
                chArray[1] = this.checkerFromPosition(new Point(activePoint.X + 1, activePoint.Y + 1)); //верхняя правая
                chArray[2] = this.checkerFromPosition(new Point(activePoint.X - 1, activePoint.Y - 1)); //нижняя левая
                chArray[3] = this.checkerFromPosition(new Point(activePoint.X + 1, activePoint.Y - 1)); //нижняя правая
                for (int i = 0; i < chArray.Length; i++)
                {
                    Checker ch = chArray[i];
                    if (ch != null)
                    {
                        if (ch.color != active.color)//шашки разные
                        {
                            int xMove = ch.position.X - active.position.X;
                            int yMove = ch.position.Y - active.position.Y;
                            Point killMove = new Point(ch.position.X + xMove, ch.position.Y + yMove);
                            if (this.checkerFromPosition(killMove) == null && killMove.X > 0 && killMove.X < 9
                                && killMove.Y > 0 && killMove.Y < 9)  //значит можно бить шашку
                            {
                                moveArray.Add(killMove);
                            }
                        }
                    }
                }
            }
            

            
           

            if (moveArray.Count == 0) //рядом шашек, которые можно побить нет, значит делаем обычный ход.
            {
                if (active.king)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        int deltaX = j == 0 ? -1 : 1; //сначала влево, потом вправо проверяем
                        for (int k = 0; k < 2; k++)
                        {
                            int deltaY = k == 0 ? -1 : 1; //вверх, потом вниз
                            for (int i = 1; i < 9; i++)
                            {
                                Point checkMove = new Point(activePoint.X + deltaX * i, activePoint.Y + deltaY * i);
                                if (checkMove.X < 1 || checkMove.X > 8 || checkMove.Y < 1 || checkMove.Y > 8)
                                {
                                     checkMove.X = -1;
                                }
                                if (checkMove.X != -1)
                                {
                                    Checker ch = this.checkerFromPosition(checkMove); //ищем пустые клетки без шашек
                                    if (ch == null)
                                    {
                                        moveArray.Add(checkMove);
                                    }
                                    else
                                    {
                                        break; //если нарвались на шашку, значит продолжать поиск в этом направлении сымсла нет
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    int deltaY = 0;
                    if (down)
                    {
                        deltaY = -1;
                    }
                    else
                    {
                        deltaY = 1;
                    }
                    for (int i = 0; i < 2; i++)
                    {
                        int x = i == 0 ? active.position.X - 1 : active.position.X + 1;
                        Point checkMove = new Point(x, active.position.Y + deltaY);
                        if (checkMove.X < 1 || checkMove.X > 8 || checkMove.Y < 1 || checkMove.Y > 8)
                        {
                            checkMove.X = -1;
                        }
                        if (checkMove.X != -1)
                        {
                            Checker findCh = checkerFromPosition(checkMove);
                            if (findCh == null)
                            {
                                moveArray.Add(checkMove);
                            }
                        }
                    }
                }
            }

            Point[] retPoint = new Point[moveArray.Count];
            for (int i = 0; i < moveArray.Count; i++)
            {
                Point addPoint = (Point)moveArray[i];
                retPoint[i] = addPoint;
            }
            return retPoint;
        }


        /// <summary>
        ///   Вызывается при перерисовки стола
        /// </summary>
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (highlight && Шашки.Properties.Settings.Default.HighlightedMove)
            {  
                Checker active = this.getActiveChecker();

                if (null != active)
                {
                    //int startX = active.Location.X;
                    //int startY = active.Location.Y;
                    Point[] moveArray = this.getMovesOnOneMove();
                    foreach (Point drawRectPoint in moveArray)
                    {
                        drawRect(drawRectPoint);
                    }                       
                }
            }

        }

        /// <summary>
        ///   Рисует зеленый квадрат по заданной точке
        /// </summary>
        private void drawRect(Point startPosition)
        {
            int x = (startPosition.X - 1) * 50;
            int y = (8 - startPosition.Y) * 50;
            int wight = 50;
            int height = 50;

            Graphics g = pictureBox1.CreateGraphics();

            Rectangle drawRectangle = new Rectangle(x,y,wight,height);
            Pen drawPen = new Pen(Color.Green);

            g.FillRectangle(Brushes.Green, drawRectangle);
        }

        private Point getPointOnPosition(int x, int y)
        {
            int newX = x / 50;
            int newY = y / 50;
            return new Point(newX, newY);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.newGame();
        }

        private void button2_Click(object sender, EventArgs e)
        {
           this.computerStep();

        }

        private void файлToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        ///   Метод, который выполняет ход компьютера
        /// </summary>
        private void computerStep()
        {
           // EI_SetTimeControl()
            EI_SetTime(5 * 1000, 1000); //5 * 1000 миллисекунд свое время   99 противника
            EI_Think();
           // EI_Think();
        }

        /// <summary>
        ///   Создание новой игры в движке
        /// </summary>
        private void newGame()
        {
            EI_NewGame();
            EI_Initialization(method, (int)16384); //2^14
            EI_SetTimeControl((int)24 * 60, 0); //24 * 60 минут на партию   0 - бонус за ход
            step = false;
        }


        /// <summary>
        ///   Меняет позицию шашки
        /// </summary>
        public void moveChecker(Point fromPoint, Point toPoint)
        {
            foreach (Checker ch in checkerArray)
            {
                if (ch.position == fromPoint)
                {
                    ch.position = toPoint;

                    ch.Location = new Point((ch.position.X - 1) * 50,  (8 - ch.position.Y) * 50);
                    ch.click = false;
                    highlight = false;
                    pictureBox1.Invalidate();
                    step = !step;

                    break;
                }
            }
        }


        /// <summary>
        ///   Убирает шашку со стола
        /// </summary>
        public void deleteChecker(Point deletePoint)
        {
            foreach (Checker ch in checkerArray)
            {
                if (ch.position == deletePoint)
                {
                    ch.setPosition(-1, -1);
                    ch.Location = new Point(400, 400);
                    ch.click = false;
                    pictureBox1.Invalidate();
                    break;
                }
            }
        }

        /// <summary>
        ///   Делает действия на доске по строке хода (используется при ходе компьютера)
        /// </summary>
        public static void moveFromString(string move)
        {
            //a - 97   
            //b - 98   
            //c - 99   7 - 55
            //h - 104  8 - 56

            bool hit = false;
            for (int i = 0; i < move.Length-2; )
            {  
                char moveFromLetter = move[i];
                char moveFromNumber = move[i + 1];
                if (move[i + 2] == ':')
                {
                    hit = true;
                }

                if (hit) //Игрок забирает шашку(и)
                {
                    string[] words = move.Split(':');

                    //a1:b2:c3:g4
                    //a1 - фишка, которая ходит
                    //b2 и c3 - эти фишки он бьет
                    //g4 - конечное положение фишки
                    
                    //move[i + 5] == ':'
                    char moveToLetter = words[words.Length - 1][0];
                    char moveToNumber = words[words.Length - 1][1];

                    for (int j = 1; j < words.Length; j++)
                    {
                        string deleteChStr = words[j];
                        //delete
                        char deleteLetter = deleteChStr[0];
                        char deleteNumber = deleteChStr[1];

                        int numberDeleteVertical = (int)deleteLetter - 96;
                        int numberDeleteHorizontal = (int)deleteNumber - 48;

                        Point deletePoint = new Point(numberDeleteVertical, numberDeleteHorizontal);

                        realForm.deleteChecker(deletePoint);
                    }

                    int numberFromVertical = (int)moveFromLetter - 96;
                    int numberFromHorizontal = (int)moveFromNumber - 48;

                    int numberToVertical = (int)moveToLetter - 96;
                    int numberToHorizontal = (int)moveToNumber - 48;

                   
                    Point fromPoint = new Point(numberFromVertical, numberFromHorizontal);
                    Point toPoint = new Point(numberToVertical, numberToHorizontal);


                    
                    realForm.moveChecker(fromPoint, toPoint);

                    i = move.Length;
                    
                }
                else  //ход игрока без взятия
                {
                    char moveToLetter = move[i + 2];
                    char moveToNumber = move[i + 3];

                    int numberFromVertical = (int)moveFromLetter - 96;
                    int numberFromHorizontal = (int)moveFromNumber - 48;

                    int numberToVertical = (int)moveToLetter - 96 ;
                    int numberToHorizontal = (int)moveToNumber - 48;

                    Point fromPoint = new Point(numberFromVertical, numberFromHorizontal);
                    Point toPoint = new Point(numberToVertical, numberToHorizontal);

                    realForm.moveChecker(fromPoint, toPoint);
                    i += 4;
                }
                
            }
        }


        /// <summary>
        ///   Передача строки движения движку
        /// </summary>
        //Дописать!!!
        private void userMove(string move)
        {
            StringBuilder moveBuilder = new StringBuilder(move);
            EI_MakeMove(moveBuilder);
        }


        /// <summary>
        ///   Метод получающий данные из движка
        /// </summary>
        static void seacrResultDelegate(int score, int depth, int speed, StringBuilder pv, StringBuilder cm)
        {
            if (score == depth && depth == speed && speed == 123) //ход делает компьютер
            {
                string moveString = pv.ToString();
                Form1.moveFromString(moveString);
                realForm.Text = moveString;
            }
            else
            {
                Form1.ActiveForm.Text = "score = " + score.ToString() + " depth = " + depth.ToString() + " speed = " + speed.ToString()
                + " pv = " + pv.ToString();
            }
            
        }

        private void новаяИграToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            gameEnded();
        }

    }
}
