using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Collections;


namespace Шашки
{
    public partial class Form1 : Form
    {
        private const string helpFilePath = "help.chm";
        /// <summary>
        /// Подсвечивать возможные ходы?
        /// </summary>
        bool _highlight;

        /// <summary>
        /// Ход игрока в строке
        /// </summary>
        string _playerMove;


        /// <summary>
        /// 0 - с компьютером
        /// </summary>
        int _withHuman;


        /// <summary>
        /// 0-2 сложность компьютера
        /// </summary>
        static int _computerHard;


        /// <summary>
        /// Время игры в секундах
        /// </summary>
        private int _timeGame;


        /// <summary>
        /// False - нижние не столе
        /// </summary>
        private bool _playerColor;

        /// <summary>
        /// Означает началась ли игра
        /// </summary>
        private bool _gameStarted;

        [DllImport("SiDra.dll", CharSet = CharSet.Ansi)]
        static extern void EI_MakeMove(string move);

        [DllImport("SiDra.dll", CharSet = CharSet.Auto)]
        static extern StringBuilder EI_Think();

        //[DllImport("SiDra.dll", CharSet = CharSet.Auto)]
        //static extern StringBuilder EI_PonderHit(StringBuilder move);

        [DllImport("SiDra.dll", CharSet = CharSet.Auto)]
        static extern void EI_Initialization(PfSearchInfo si, int memLim);

        [DllImport("SiDra.dll", CharSet = CharSet.Auto)]
        static extern void EI_NewGame();

        //[DllImport("SiDra.dll", CharSet = CharSet.Auto)]
        //static extern void EI_Stop();

        //[DllImport("SiDra.dll", CharSet = CharSet.Auto)]
        //static extern void EI_SetupBoard(StringBuilder pos);

        [DllImport("SiDra.dll", CharSet = CharSet.Auto)]
        static extern void EI_SetTimeControl(int time, int inc);

        [DllImport("SiDra.dll", CharSet = CharSet.Auto)]
        static extern void EI_SetTime(int time, int otime);

        //[DllImport("SiDra.dll", CharSet = CharSet.Auto)]
       // static extern StringBuilder EI_GetName();

        //[DllImport("SiDra.dll", CharSet = CharSet.Auto)]
        //static extern void EI_OnExit();

        delegate void PfSearchInfo(int score, int depth, int speed, StringBuilder pv, StringBuilder cm);

        public static Form1 realForm;


        public Form1()
        {
            InitializeComponent();

            _timeGame = 0;
            logMove = false;

            _highlight = false;
            step = false; //первые ходят нижние
            checkerArray = new Checker[24];
            for (int i = 0; i < checkerArray.Length; i++)
            {
                checkerArray[i] = new Checker();
            }
            createCheckers(0, 12, true);
            createCheckers(12, 24, false);
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
                asd.Image = color ? Properties.Resources.Шашка_1 : Properties.Resources.Шашка_2;
                asd.color = color;
                asd.BackColor = Color.Transparent;
                asd.Click += pictureBoxClick;
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
        ///   Проверяет может ли шашка забрать другую шашку
        /// </summary>
        bool fightChecker(Checker ch)
        {
            if (ch.position.X < 1 || ch.position.Y < 1 || ch.position.X > 8 || ch.position.Y > 8)
            {
                return false;
            }
            bool canFight = false;
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
                            Checker chnew = checkerFromPosition(new Point(activePoint.X + deltaX * i, activePoint.Y + deltaY * i)); //берем все шашки во всех направлениях
                            if (chnew != null)
                            {
                                if (chnew.color != ch.color)
                                {
                                    int xMove = activePoint.X + deltaX * (i + 1);
                                    int yMove = activePoint.Y + deltaY * (i + 1);
                                    var killMove = new Point(xMove, yMove);
                                    if (checkerFromPosition(killMove) == null && killMove.X > 0 && killMove.X < 9
                                        && killMove.Y > 0 && killMove.Y < 9)  //значит можно бить шашку
                                    {
                                        canFight = true;
                                        break;
                                    }
                                    break;
                                }
                                 //наткнулись на шашки нашего цвета, значит дальше в этом направлении проверять смысла нет
                                 break;
                            }
                        }
                    }
                }
            }
            else
            {
                Point activePoint = ch.position;
                //сначала делаем проверку на то, а можем ли мы побить какую-нибуь шашку рядом
                var chArray = new Checker[4];
                chArray[0] = checkerFromPosition(new Point(activePoint.X - 1, activePoint.Y + 1)); //верхняя левая
                chArray[1] = checkerFromPosition(new Point(activePoint.X + 1, activePoint.Y + 1)); //верхняя правая
                chArray[2] = checkerFromPosition(new Point(activePoint.X - 1, activePoint.Y - 1)); //нижняя левая
                chArray[3] = checkerFromPosition(new Point(activePoint.X + 1, activePoint.Y - 1)); //нижняя правая
                foreach (Checker chFind in chArray)
                {
                    if (chFind != null)
                    {
                        if (ch.color != chFind.color)//шашки разные
                        {
                            int xMove = chFind.position.X - ch.position.X;
                            int yMove = chFind.position.Y - ch.position.Y;
                            var killMove = new Point(chFind.position.X + xMove, chFind.position.Y + yMove);
                            if (checkerFromPosition(killMove) == null && killMove.X > 0 && killMove.X < 9 && killMove.Y > 0 && killMove.Y < 9)  //значит можно бить шашку
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
        ///   Проверяет входит ли точка в "стол"
        /// </summary>
        public bool pointIsGood(Point checkPoint)
        {
            if (checkPoint.X < 1 || checkPoint.Y < 1 || checkPoint.X > 8 || checkPoint.Y > 8)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        ///   Проверяет может ли шашка сделать ход
        /// </summary>
        private bool checkerCanMove(Checker ch)
        {
            if (ch.position.X < 1 || ch.position.Y < 1 || ch.position.X > 8 || ch.position.Y > 8)
            {
                return false;
            }
            bool down = ch.color;

            Point activePoint = ch.position;
            var chArray = new Point[4];
            if (ch.king)
            {
                chArray[0] = new Point(activePoint.X - 1, activePoint.Y - 1); //нижняя левая
                chArray[1] = new Point(activePoint.X + 1, activePoint.Y - 1); //нижняя правая
                chArray[2] = new Point(activePoint.X - 1, activePoint.Y + 1); //верхняя левая
                chArray[3] = new Point(activePoint.X + 1, activePoint.Y + 1); //верхняя правая
            }
            else
            {
                if (down)
                {
                    chArray[0] = new Point(activePoint.X - 1, activePoint.Y - 1); //нижняя левая
                    chArray[1] = new Point(activePoint.X + 1, activePoint.Y - 1); //нижняя правая
                }
                else
                {
                    chArray[0] = new Point(activePoint.X - 1, activePoint.Y + 1); //верхняя левая
                    chArray[1] = new Point(activePoint.X + 1, activePoint.Y + 1); //верхняя правая
                }
            }


            return (from findPoint in chArray where pointIsGood(findPoint) select checkerFromPosition(findPoint)).Any(checkCh => checkCh == null);
        }

        /// <summary>
        ///   Возвращает все шашки, которые могут ходить
        /// </summary>
        private Checker[] solveCheckers()
        {
            var solveArray = new ArrayList();
            //Проверка на шашки, которые должны бить другие шашки
            foreach (Checker obj in checkerArray.Where(obj => obj.color == step & fightChecker(obj)))
            {
                solveArray.Add(obj);
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
            var retCh = new Checker[solveArray.Count];
            for (var i = 0; i < solveArray.Count; i++ )
            {
                retCh[i] = (Checker)solveArray[i];
            }
            return retCh;
        }


        /// <summary>
        ///   Обработчик нажатий шашек
        /// </summary>
        private void pictureBoxClick(object sender, EventArgs e)
        {
            if (!_gameStarted)
            {
                return;
            }
            if (!timer1.Enabled)
            {
                timer1.Enabled = true;
                сдатьсяToolStripMenuItem.Enabled = true;
            }
            if (_withHuman == 0)
            {
                if (_playerColor && !step) //игрок ходит черными!!!
                {
                    return;
                }
            }
            var active = (Checker)sender;
            foreach (var obj in checkerArray)
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

            var canMoveCheckers = solveCheckers();

            foreach (var obj in canMoveCheckers)
            {
                if (obj == sender)
                {
                    obj.click = true;
                    _highlight = true;
                    pictureBox1.Invalidate();
                    //a - 97   
                    //b - 98   
                    //c - 99   7 - 55
                    _playerMove = convertPointToCheckerString(obj.position.X, obj.position.Y);
                    if (logMove)
                    {
                        Text = _playerMove;
                    }
                    
                }
                else
                {
                    obj.click = false;
                }
            }
            pictureBox1.Invalidate();
        }

        private void form1MouseUp(object sender, MouseEventArgs e)
        {

        }

        /// <summary>
        ///   Возвращает активную шашку
        /// </summary>
        private Checker getActiveChecker()
        {
            return checkerArray.FirstOrDefault(obj => obj.click); 
        }

        /// <summary>
        ///   Возвращает шашку по номеру позиции на игровой доске
        /// </summary>
        private Checker checkerFromPosition(Point pos)
        {
            if (pos.X < 1 || pos.Y < 1 || pos.X > 8 || pos.Y > 8)
            {
                return null;
            }
            return checkerArray.FirstOrDefault(obj => obj.position.X == pos.X && obj.position.Y == pos.Y);
        }


        /// <summary>
        ///   Вызывается, когда отпускается мышка на столе
        /// </summary>
        private void pictureBox1MouseUp(object sender, MouseEventArgs e)
        {
            if (!_gameStarted)
            {
                return;
            }
            if (_withHuman == 0)
            {
                if (_playerColor && !step) //игрок ходит черными!!!
                {
                    return;
                }
            }

            Checker active = getActiveChecker();
            

            if (null != active)
            {
                var newPoint = getPointOnPosition(e.X, e.Y);
                var newPointForCh = new Point(newPoint.X + 1, 8 - newPoint.Y);

                var moveIsGood = false;
                var fightMove = false;
                var movesPoint = getMovesOnOneMove();
                if (movesPoint.Any(a => a == newPointForCh))
                {
                    moveIsGood = true;
                    if (active.king) 
                    {
                        int deltaForX = (newPointForCh.X - active.position.X) / Math.Abs(newPointForCh.X - active.position.X);
                        int deltaForY = (newPointForCh.Y - active.position.Y) / Math.Abs(newPointForCh.Y - active.position.Y);
                        for (int i = 1; i < Math.Abs(newPointForCh.X - active.position.X) + 1; i++)
                        {
                            var searchPoint = new Point(active.position.X + deltaForX * i, active.position.Y + deltaForY * i);
                            var findCh = checkerFromPosition(searchPoint);
                            if (findCh != null && findCh.color != active.color)
                            {
                                fightMove = true;
                                active.fight = true;
                                _playerMove += ":" + convertPointToCheckerString(searchPoint.X, searchPoint.Y);
                                if (logMove)
                                {
                                    Text = _playerMove;
                                }
                                deleteChecker(searchPoint);
                                break;
                            }
                        }
                    }
                    else
                    {
                        var deltaX = active.position.X - newPointForCh.X;
                        var deltaY = active.position.Y - newPointForCh.Y;
                        if (Math.Abs(deltaX) > 1 && Math.Abs(deltaY) > 1)
                        {
                            fightMove = true;
                            active.fight = true;
                            var deleteCh = new Point(active.position.X, active.position.Y);
                            deleteCh.X -= deltaX < 0 ? -1 : 1;
                            deleteCh.Y -= deltaY < 0 ? -1 : 1;
                            _playerMove += ":" + convertPointToCheckerString(deleteCh.X, deleteCh.Y);
                            if (logMove)
                            {
                                Text = _playerMove;
                            }
                            deleteChecker(deleteCh);
                        }
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
            _highlight = false;
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
            _playerMove += addString;
            activeCh.fight = false;
            if (logMove)
            {
                Text = _playerMove;
            }


            if (_withHuman == 0)
            {
                //Если играем с комьютером передаем ему строку движения шашки
                userMove(_playerMove);
            }

            var find = solveCheckers();
            if (find.Length == 0)
            {
                gameEnded();
            }
            else
            {
                if (_withHuman == 0)
                {
                    _timerForComputerStep = new Timer {Interval = 10};
                    _timerForComputerStep.Tick += computerTimerElapsed;
                    _timerForComputerStep.Enabled = true;
                }
            }
        }

        private Timer _timerForComputerStep;
        private void computerTimerElapsed(Object sender, EventArgs e)
        {
            computerStep();
            _timerForComputerStep.Enabled = false;
        }
        /// <summary>
        ///   Восстанавливает все шашки на столе в default положение
        /// </summary>
        private void resetAllCheckersOnBoard()
        {
            foreach (var ch in checkerArray)
            {
                ch.setDefaultValue();
            }
            createCheckers(0, 12, true);
            createCheckers(12, 24, false);
        }

        /// <summary>
        ///   Игра окончена
        /// </summary>
        private void gameEnded()
        {
            _gameStarted = false;
            string newString;
            timer1.Enabled = false;
            сдатьсяToolStripMenuItem.Enabled = false;
            bool firstPlayerWin = false;
            if (_withHuman == 0) //играем с компьютером
            {
                if (!_playerColor)
                {
                    if (!step)
                    {
                        newString = "Вы проиграли";
                    }
                    else
                    {
                        firstPlayerWin = true;
                        newString = "Вы победили";
                    }
                }
                else
                {
                    if (step)
                    {
                        newString = "Вы проиграли";
                    }
                    else
                    {
                        firstPlayerWin = true;
                        newString = "Вы победили";
                    }
                }
                
            }
            else
            {
                if (!_playerColor)
                {
                    if (!step)
                    {
                        newString = "Игрок 2 победил";
                    }
                    else
                    {
                        firstPlayerWin = true;
                        newString = "Игрок 1 победил";
                    }
                } else
                {
                    if (step)
                    {
                        newString = "Игрок 2 победил";
                    }
                    else
                    {
                        firstPlayerWin = true;
                        newString = "Игрок 1 победил";
                    }
                }
                
            }
            
            //this.
            if (_withHuman == 0) //с компьютером
            {
                Database.Instance.saveGameData(_withHuman, @"Игрок1", @"Computer", _timeGame, firstPlayerWin,
                                               _computerHard);
            }
            else
            {
                string playerOneName = Properties.Settings.Default.Player1;
                string playerTwoName = Properties.Settings.Default.Player2;
                Database.Instance.saveGameData(_withHuman, playerOneName, playerTwoName, _timeGame, firstPlayerWin,
                                               -1);
            }
            newString = string.Format("{0}\nЖелаете начать новую игру?",newString);
            var result1 = MessageBox.Show(newString, @"Игра окончена", MessageBoxButtons.YesNo);
            if (result1 == DialogResult.Yes)
            {
                startNewGame(true);
            }
            else
            {
                resetAllGameValueToDefaullt(true);
                resetAllCheckersOnBoard();
                _gameStarted = false;
            }
            
        }

        private void resetAllGameValueToDefaullt(bool loadProperties)
        {
            _highlight = false;
            step = false;
            _timeGame = 0;
            timer1.Enabled = false;
            сдатьсяToolStripMenuItem.Enabled = false;
            timerLabel.Text = @"00:00";
            _gameStarted = true;
            if (loadProperties)
            {
                _withHuman = Properties.Settings.Default.Game_type;
                _computerHard = Properties.Settings.Default.HardLevel;
                _playerColor = Properties.Settings.Default.Player1_color;
            } 
        }

        /// <summary>
        ///   Начать новую игру
        /// </summary>
        private void startNewGame(bool changeHuman)
        {
            resetAllGameValueToDefaullt(changeHuman);
            resetAllCheckersOnBoard();
            if (_withHuman == 0) //с компьютером
            {
                newGame();
                if (_playerColor) //игрок ходит черными!!!
                {
                    //делаем ход компьютера
                    timer1.Enabled = true;
                    сдатьсяToolStripMenuItem.Enabled = true;
                    _gameStarted = true;
                    computerStep();
                }
            }
            else
            {
                var newFormFirst = new InsertPlayerNameForm(1);
                var newFormSecond = new InsertPlayerNameForm(2);
                newFormFirst.ShowDialog();
                newFormSecond.ShowDialog();
            }
        }


        /// <summary>
        ///   Возвращает все возможные ходы для активной шашки на один ход
        /// </summary>
        private IEnumerable<Point> getMovesOnOneMove()
        {
            var active = getActiveChecker();
            if (active == null)
            {
                return null;
            }
            var down = false;
            if (active.color)
            {
                down = true;
            }
            var moveArray = new ArrayList(4);
            var activePoint = active.position;

            if (active.king)
            {
                for (var j = 0; j < 2; j++)
                {
                    var deltaX = j == 0 ? -1 : 1; //сначала влево, потом вправо проверяем
                    for (var k = 0; k < 2; k++)
                    {
                        var deltaY = k == 0 ? -1 : 1; //вверх, потом вниз
                        for (var i = 1; i < 9; i++)
                        {
                            var canKill = true;
                            var ch = checkerFromPosition(new Point(activePoint.X + deltaX * i, activePoint.Y + deltaY * i)); //берем все шашки во всех направлениях
                            if (ch != null)
                            {
                                if (ch.color != active.color)
                                {
                                    for (int l = 1; l < 8; l++) //проверяем все предполагаемые клетки за той, которую мы хотим "забрать"
                                    {
                                        var xMove = activePoint.X + deltaX * (i + l);
                                        var yMove = activePoint.Y + deltaY * (i + l);
                                        var killMove = new Point(xMove, yMove);
                                        if (checkerFromPosition(killMove) == null && killMove.X > 0 && killMove.X < 9
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
                var chArray = new Checker[4];
                chArray[0] = checkerFromPosition(new Point(activePoint.X - 1, activePoint.Y + 1)); //верхняя левая
                chArray[1] = checkerFromPosition(new Point(activePoint.X + 1, activePoint.Y + 1)); //верхняя правая
                chArray[2] = checkerFromPosition(new Point(activePoint.X - 1, activePoint.Y - 1)); //нижняя левая
                chArray[3] = checkerFromPosition(new Point(activePoint.X + 1, activePoint.Y - 1)); //нижняя правая
                foreach (Checker ch in chArray)
                {
                    if (ch != null)
                    {
                        if (ch.color != active.color)//шашки разные
                        {
                            var xMove = ch.position.X - active.position.X;
                            var yMove = ch.position.Y - active.position.Y;
                            var killMove = new Point(ch.position.X + xMove, ch.position.Y + yMove);
                            if (checkerFromPosition(killMove) == null && killMove.X > 0 && killMove.X < 9
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
                                var checkMove = new Point(activePoint.X + deltaX * i, activePoint.Y + deltaY * i);
                                if (checkMove.X < 1 || checkMove.X > 8 || checkMove.Y < 1 || checkMove.Y > 8)
                                {
                                     checkMove.X = -1;
                                }
                                if (checkMove.X != -1)
                                {
                                    var ch = checkerFromPosition(checkMove); //ищем пустые клетки без шашек
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
                    int deltaY;
                    if (down)
                    {
                        deltaY = -1;
                    }
                    else
                    {
                        deltaY = 1;
                    }
                    for (var i = 0; i < 2; i++)
                    {
                        var x = i == 0 ? active.position.X - 1 : active.position.X + 1;
                        var checkMove = new Point(x, active.position.Y + deltaY);
                        if (checkMove.X < 1 || checkMove.X > 8 || checkMove.Y < 1 || checkMove.Y > 8)
                        {
                            checkMove.X = -1;
                        }
                        if (checkMove.X != -1)
                        {
                            var findCh = checkerFromPosition(checkMove);
                            if (findCh == null)
                            {
                                moveArray.Add(checkMove);
                            }
                        }
                    }
                }
            }

            var retPoint = new Point[moveArray.Count];
            for (var i = 0; i < moveArray.Count; i++)
            {
                var addPoint = moveArray[i];
                retPoint[i] = (Point) addPoint;
            }
            return retPoint;
        }


        /// <summary>
        ///   Вызывается при перерисовки стола
        /// </summary>
        private void pictureBox1Paint(object sender, PaintEventArgs e)
        {
            if (_highlight && Properties.Settings.Default.HighlightedMove)
            {  
                Checker active = getActiveChecker();

                if (null != active)
                {
                    var moveArray = getMovesOnOneMove();
                    foreach (var drawRectPoint in moveArray)
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
            var x = (startPosition.X - 1) * 50;
            var y = (8 - startPosition.Y) * 50;
            const int wight = 50;
            const int height = 50;

            Graphics g = pictureBox1.CreateGraphics();

            var drawRectangle = new Rectangle(x,y,wight,height);

            g.FillRectangle(Brushes.Green, drawRectangle);
        }

        private static Point getPointOnPosition(int x, int y)
        {
            var newX = x / 50;
            var newY = y / 50;
            return new Point(newX, newY);
        }

        private void файлToolStripMenuItemClick(object sender, EventArgs e)
        {

        }

        /// <summary>
        ///   Метод, который выполняет ход компьютера
        /// </summary>
        private static void computerStep()
        {
            var timeToComputer = (_computerHard + 1)*1000;
            EI_SetTime(timeToComputer, 1000); //5 * 1000 миллисекунд свое время   1000 противника
            EI_Think();
        }

        /// <summary>
        ///   Создание новой игры в движке
        /// </summary>
        partial void newGame()
        {
            EI_NewGame();
            EI_Initialization(method, 16384); //2^14
            EI_SetTimeControl(24 * 60, 0); //24 * 60 минут на партию   0 - бонус за ход
        }


        /// <summary>
        ///   Меняет позицию шашки
        /// </summary>
        public void moveChecker(Point fromPoint, Point toPoint)
        {
            if (fromPoint.X == toPoint.X & fromPoint.Y == toPoint.Y)
            {
                return;
            }
            foreach (var ch in checkerArray)
            {
                if (ch.position == fromPoint)
                {
                    ch.position = toPoint;

                    if (!ch.king)
                    {
                        if (ch.color)
                        {
                            if (toPoint.Y == 1)
                            {
                                ch.king = true;
                                ch.Image = Properties.Resources.Шашка_1_дамка;
                            }
                        }
                        else
                        {
                            if (toPoint.Y == 8)
                            {
                                ch.king = true;
                                ch.Image = Properties.Resources.Шашка_2_дамка;
                            }
                        }
                    }
                    ch.Location = new Point((ch.position.X - 1)*50, (8 - ch.position.Y)*50);
                    ch.click = false;
                    _highlight = false;
                    pictureBox1.Invalidate();

                    break;
                }
            }
        }


        /// <summary>
        ///   Убирает шашку со стола
        /// </summary>
        public void deleteChecker(Point deletePoint)
        {
            foreach (var ch in checkerArray)
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

                    int numberFromVertical = moveFromLetter - 96;
                    int numberFromHorizontal = moveFromNumber - 48;

                    var fromPoint = new Point(numberFromVertical, numberFromHorizontal);

                    for (int j = 1; j < words.Length - 1; j++)
                    {
                        string deleteChStr = words[j];
                        //delete
                        char deleteLetter = deleteChStr[0];
                        char deleteNumber = deleteChStr[1];

                        int numberDeleteVertical = deleteLetter - 96;
                        int numberDeleteHorizontal = deleteNumber - 48;

                        var deletePoint = new Point(numberDeleteVertical, numberDeleteHorizontal);
                        var newToPoint = new Point(deletePoint.X + (deletePoint.X - fromPoint.X), deletePoint.Y + (deletePoint.Y - fromPoint.Y));
                       // realForm.moveChecker(fromPoint, toPoint);
                        realForm.deleteChecker(deletePoint);
                        realForm.moveChecker(fromPoint, newToPoint);
                        fromPoint = newToPoint;
                    }

                   
                    
                    int numberToVertical = moveToLetter - 96;
                    int numberToHorizontal = moveToNumber - 48;
                    var toPoint = new Point(numberToVertical, numberToHorizontal);
                    realForm.moveChecker(fromPoint, toPoint);
                    
                    i = move.Length;
                    
                }
                else  //ход игрока без взятия
                {
                    char moveToLetter = move[i + 2];
                    char moveToNumber = move[i + 3];

                    int numberFromVertical = moveFromLetter - 96;
                    int numberFromHorizontal = moveFromNumber - 48;

                    int numberToVertical = moveToLetter - 96 ;
                    int numberToHorizontal = moveToNumber - 48;

                    var fromPoint = new Point(numberFromVertical, numberFromHorizontal);
                    var toPoint = new Point(numberToVertical, numberToHorizontal);

                    realForm.moveChecker(fromPoint, toPoint);
                    i += 4;
                }
            }


            realForm.step = !realForm.step;
            Checker[] find = realForm.solveCheckers();
            if (find.Length == 0)
            {
                realForm.gameEnded();
            }
            

        }


        /// <summary>
        ///   Передача строки движения движку
        /// </summary>
        //Дописать!!!
        private static void userMove(string move)
        {
            EI_MakeMove(move);
        }


        /// <summary>
        ///   Метод получающий данные из движка
        /// </summary>
        static void seacrResultDelegate(int score, int depth, int speed, StringBuilder pv, StringBuilder cm)
        {
            if (score == depth && depth == speed && speed == 123) //ход делает компьютер
            {
                string moveString = pv.ToString();
                moveFromString(moveString);
                if (realForm.logMove)
                {
                    realForm.Text = moveString;
                } 
            }
            else
            if (score == depth && depth == speed && speed == 111) //Ошибка!
            {
                string moveString = pv.ToString();
                MessageBox.Show(moveString);
                if (realForm.logMove)
                {
                    realForm.Text = moveString;
                }
            }
            else if (realForm.logMove)
            {
                if (ActiveForm != null)
                    ActiveForm.Text = string.Format("score = {0} depth = {1} speed = {2} pv = {3}", score, depth, speed, pv);
            }
            
        }

        private void новаяИграToolStripMenuItemClick(object sender, EventArgs e)
        {

        }

        private void сКомпьютеромToolStripMenuItemClick(object sender, EventArgs e)
        {
            _withHuman = 0;
            Properties.Settings.Default.Game_type = _withHuman;
            Properties.Settings.Default.Save();
            startNewGame(true);
        }

        private void сЧеловекомToolStripMenuItemClick(object sender, EventArgs e)
        {
            _withHuman = 1;
            Properties.Settings.Default.Game_type = _withHuman;
            Properties.Settings.Default.Save();
            startNewGame(true);
        }

        /// <summary>
        ///   Конвертирует позицию в строку для передачи движку
        /// </summary>
        private static string convertPointToCheckerString(int x, int y)
        {
            return (Convert.ToChar(97 + x - 1) + Convert.ToChar(48 + y).ToString());
        }

        private void настройкиToolStripMenuItemClick(object sender, EventArgs e)
        {
            var newForm = new FormSettings();
            newForm.ShowDialog(this);
        }

        private void form1Load(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.FirstStart)
            {
                var addForm = new WhomToPlay();
                addForm.ShowDialog(this);
                Properties.Settings.Default.FirstStart = false;
                Properties.Settings.Default.Save();
            }
            startNewGame(true);
        }

        public void refreshImage()
        {
            pictureBox1.Invalidate();
        }

        private void оПрограммеToolStripMenuItemClick(object sender, EventArgs e)
        {
            var aboutForm = new AboutBox1();
            aboutForm.ShowDialog(this);
        }

        private void выходToolStripMenuItemClick(object sender, EventArgs e)
        {
            Close();
        }

        private void статистикаToolStripMenuItemClick(object sender, EventArgs e)
        {
            var addForm = new Statistic();
            addForm.ShowDialog(this);
        }

        private void timer1Tick(object sender, EventArgs e)
        {
            _timeGame += 1;
            setTimeGameLabelValue();
        }

        private void setTimeGameLabelValue()
        {
            int day = _timeGame > 86399 ? _timeGame / 86400 : 0;
            int hour = _timeGame > 3599 ? (_timeGame - (day * 86400)) / 3600 : 0;
            int minute = _timeGame > 59 ? (_timeGame - (hour * 3600) - (day * 86400)) / 60 : 0;
            int second = _timeGame -(day * 86400) - (hour * 3600) - (minute * 60);

            string dayStr = day.ToString();
            dayStr = dayStr.Length == 1 ? "0" + dayStr : dayStr;

            string hourStr = hour.ToString();
            hourStr = hourStr.Length == 1 ? "0" + hourStr : hourStr;

            string minuteStr = minute.ToString();
            minuteStr = minuteStr.Length == 1 ? "0" + minuteStr : minuteStr;

            string secondStr = second.ToString();
            secondStr = secondStr.Length == 1 ? "0" + secondStr : secondStr;

            if (day > 0)
            {
                timerLabel.Text = string.Format("{0}:{1}:{2}:{3}", dayStr, hourStr, minuteStr, secondStr);
            }
            else if (hour > 0)
            {
                timerLabel.Text = string.Format("{0}:{1}:{2}", hourStr, minuteStr, secondStr);
            } else
            {
                timerLabel.Text = string.Format("{0}:{1}", minuteStr, secondStr);
            }
        }

        /// <summary>
        /// Вызывается когда нажимается на стол (для запуска таймера)
        /// </summary>
        private void pictureBox1MouseDown(object sender, MouseEventArgs e)
        {
            if (_gameStarted && !timer1.Enabled)
            {
                timer1.Enabled = true;
                сдатьсяToolStripMenuItem.Enabled = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (_withHuman == 0) //с компьютером
            {
                Database.Instance.saveGameData(_withHuman, @"Игрок1", @"Computer", _timeGame, false,
                                               _computerHard);
            }
            else
            {
                string playerOneName = Properties.Settings.Default.Player1;
                string playerTwoName = Properties.Settings.Default.Player2;
                Database.Instance.saveGameData(_withHuman, playerOneName, playerTwoName, _timeGame, false,
                                               -1);
            }
        }

        private void какИгратьToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (File.Exists(helpFilePath))
            {
                Help.ShowHelp(this, helpFilePath);
            }
            
        }

        private void сдатьсяToolStripMenuItemClick(object sender, EventArgs e)
        {
            const string addStr = @"Вы уверены, что хотите сдаться? В текущей партии вам будет зачислено поражение";
            var result1 = MessageBox.Show(addStr, @"Сдаться", MessageBoxButtons.YesNo);
            if (result1 == DialogResult.Yes)
            {
                _gameStarted = false;
                string newString;
                timer1.Enabled = false;
                сдатьсяToolStripMenuItem.Enabled = false;
                bool firstPlayerWin = false;
                if (_withHuman == 0) //играем с компьютером
                {
                        newString = "Вы проиграли";
                }
                else
                {
                    if (!_playerColor)
                    {
                        if (!step)
                        {
                            newString = "Игрок 2 победил";
                        }
                        else
                        {
                            firstPlayerWin = true;
                            newString = "Игрок 1 победил";
                        }
                    }
                    else
                    {
                        if (step)
                        {
                            newString = "Игрок 2 победил";
                        }
                        else
                        {
                            firstPlayerWin = true;
                            newString = "Игрок 1 победил";
                        }
                    }

                }

                if (_withHuman == 0) //с компьютером
                {
                    Database.Instance.saveGameData(_withHuman, @"Игрок1", @"Computer", _timeGame, firstPlayerWin,
                                                   _computerHard);
                }
                else
                {
                    string playerOneName = Properties.Settings.Default.Player1;
                    string playerTwoName = Properties.Settings.Default.Player2;
                    Database.Instance.saveGameData(_withHuman, playerOneName, playerTwoName, _timeGame, firstPlayerWin,
                                                   -1);
                }
                newString = string.Format("{0}\nЖелаете начать новую игру?", newString);
                var result2 = MessageBox.Show(newString, @"Игра окончена", MessageBoxButtons.YesNo);
                if (result2 == DialogResult.Yes)
                {
                    startNewGame(true);
                }
                else
                {
                    resetAllGameValueToDefaullt(true);
                    resetAllCheckersOnBoard();
                    _gameStarted = false;
                }
            }
        }
    }
}
