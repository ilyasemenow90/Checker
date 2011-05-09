using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace Шашки
{
    public partial class Statistic : Form
    {
        private ArrayList _gameData;
        private ArrayList _computerGame;
        private List<int> listWin;
        private List<int> listLost;
        private List<int> listTimeGame;
        public Statistic()
        {
            InitializeComponent();
            _gameData = Database.Instance.loadAllDataFromFile();
            _computerGame = new ArrayList(3);
            listWin = new List<int>(3);
            listWin.Add(0);
            listWin.Add(0);
            listWin.Add(0);

            listLost = new List<int>(3);
            listLost.Add(0);
            listLost.Add(0);
            listLost.Add(0);

            listTimeGame = new List<int>(3);
            listTimeGame.Add(0);
            listTimeGame.Add(0);
            listTimeGame.Add(0);

            
            if (_gameData != null && _gameData.Count > 0)
            {
                int rowNumber = 0;
                foreach (GameSettings obj in _gameData)
                {
                    if (obj.typeGame == 0) //Игра с компьютером
                    {
                        bool win = false;
                        if (obj.playerOneWin)
                        {
                            win = true;
                            listTimeGame[obj.hardComputer] = obj.timeGame < listTimeGame[obj.hardComputer] ? obj.timeGame : listTimeGame[obj.hardComputer];
                        }

                        if (win)
                        {
                            ++listWin[obj.hardComputer];
                        }
                        else
                        {
                            ++listLost[obj.hardComputer];
                        }
                    }
                    else
                    {
                        dataGridView1.Rows.Add();
                        dataGridView1.Rows[rowNumber].Cells[0].Value = obj.dateGame.ToString();
                        dataGridView1.Rows[rowNumber].Cells[1].Value = obj.playerOneName;
                        dataGridView1.Rows[rowNumber].Cells[2].Value = obj.playerTwoName;
                        dataGridView1.Rows[rowNumber].Cells[3].Value = timerFromInt(obj.timeGame);
                        if (obj.playerOneWin)
                        {
                            dataGridView1.Rows[rowNumber].Cells[1].Style.BackColor = Color.Green;
                        }
                        else
                        {
                            dataGridView1.Rows[rowNumber].Cells[2].Style.BackColor = Color.Green;
                        }
                        rowNumber++;
                    }
                } 
            }
        }

        private string timerFromInt(int timeGame)
        {
            int day = timeGame > 86399 ? timeGame / 86400 : 0;
            int hour = timeGame > 3599 ? (timeGame - (day * 86400)) / 3600 : 0;
            int minute = timeGame > 59 ? (timeGame - (hour * 3600) - (day * 86400)) / 60 : 0;
            int second = timeGame - (day * 86400) - (hour * 3600) - (minute * 60);

            string dayStr = day.ToString();
            dayStr = dayStr.Length == 1 ? "0" + dayStr : dayStr;

            string hourStr = hour.ToString();
            hourStr = hourStr.Length == 1 ? "0" + hourStr : hourStr;

            string minuteStr = minute.ToString();
            minuteStr = minuteStr.Length == 1 ? "0" + minuteStr : minuteStr;

            string secondStr = second.ToString();
            secondStr = secondStr.Length == 1 ? "0" + secondStr : secondStr;

            string retStr;
            if (day > 0)
            {
                retStr = string.Format("{0}:{1}:{2}:{3}", dayStr, hourStr, minuteStr, secondStr);
            }
            else if (hour > 0)
            {
                retStr = string.Format("{0}:{1}:{2}", hourStr, minuteStr, secondStr);
            }
            else
            {
                retStr = string.Format("{0}:{1}", minuteStr, secondStr);
            }
            return retStr;
        }

        private void radioButton2CheckedChanged(object sender, EventArgs e)
        {
            if (sender == radioButton1)
            {
                winCount.Text = listWin[0].ToString();
                loseCount.Text = listLost[0].ToString();
                goodTime.Text = listTimeGame[0].ToString();
            }
            if (sender == radioButton2)
            {
                winCount.Text = listWin[1].ToString();
                loseCount.Text = listLost[1].ToString();
                goodTime.Text = listTimeGame[1].ToString();
            }
            if (sender == radioButton3)
            {
                winCount.Text = listWin[2].ToString();
                loseCount.Text = listLost[2].ToString();
                goodTime.Text = listTimeGame[2].ToString();
            }
        }

        private void statisticShown(object sender, EventArgs e)
        {
            radioButton2CheckedChanged(radioButton1, null);
        }

        private void resetStatistics(object sender, EventArgs e)
        {
           
            var result1 = MessageBox.Show(@"Вы уверены, что хотите сбросить статистику?", @"Сброс статистики", MessageBoxButtons.YesNo);
            if (result1 == DialogResult.Yes)
            {
                while (dataGridView1.RowCount > 0)
                {
                    dataGridView1.Rows.RemoveAt(0);
                }

                for (int i = 0; i < 3; i++)
                {
                    listWin[i] = 0;
                    listLost[i] = 0;
                    listTimeGame[i] = 0;
                }
                if (radioButton1.Checked)
                {
                    radioButton2CheckedChanged(radioButton1, null);
                }
                if (radioButton2.Checked)
                {
                    radioButton2CheckedChanged(radioButton2, null);
                }
                if (radioButton3.Checked)
                {
                    radioButton2CheckedChanged(radioButton3, null);
                }
            }
            Database.Instance.deleteFile();

        }



    }
}
