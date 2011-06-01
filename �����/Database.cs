using System;
using System.Text;
using System.Xml;
using System.IO;
using System.Collections;

namespace Шашки
{
    class Database
    {
        const string filePath = @"Resources//Games.xml";
        static Database instance = null;
        static readonly object padlock = new object();

        public static Database Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new Database();
                    }
                    return instance;
                }
            }
        }



        public ArrayList loadAllDataFromFile()
        {
            if (!File.Exists(filePath))
            {
                return null;
            }

            // Объявляем и забиваем файл в документ  
            var xd = new XmlDocument();
            var fs = new FileStream(filePath, FileMode.Open);
            xd.Load(fs);

            var retList = new ArrayList();
            var list = xd.GetElementsByTagName("game"); // Создаем и заполняем лист по тегу "user"  
            for (int i = 0; i < list.Count; i++)
            {

                /*
                     <Player1>Игрок1</Player1>
                    <Player2>Computer</Player2>
                    <GameTime>0</GameTime>
                    <PlayerOneWin>False</PlayerOneWin>
                    <HardComputer>1</HardComputer>
                    <GameDate>09.05.2011 18:35:08</GameDate>
                */

                var player1 = (XmlElement)xd.GetElementsByTagName("Player1")[i];         // Забиваем id в переменную  
                var player2 = (XmlElement)xd.GetElementsByTagName("Player2")[i];      // Забиваем login в переменную  
                var gameTime = (XmlElement)xd.GetElementsByTagName("GameTime")[i];   // Забиваем password в переменную  
                var playerWin = (XmlElement)xd.GetElementsByTagName("PlayerOneWin")[i];
                var hardComputer = (XmlElement)xd.GetElementsByTagName("HardComputer")[i];
                var date = (XmlElement)xd.GetElementsByTagName("GameDate")[i];
                var gameType = (XmlElement)xd.GetElementsByTagName("TypeGame")[i];
                

                var newObj = new GameSettings
                                 {
                                     playerOneName = player1.InnerText,
                                     playerTwoName = player2.InnerText,
                                     hardComputer = Convert.ToInt32(hardComputer.InnerText),//hardComputer.InnerText,
                                     playerOneWin = Convert.ToBoolean(playerWin.InnerText),
                                     timeGame = Convert.ToInt32(gameTime.InnerText),
                                     dateGame = Convert.ToDateTime(date.InnerText),
                                     typeGame = Convert.ToInt32(gameType.InnerText)
                                 };
                retList.Add(newObj);
            }
            // Закрываем поток  
            fs.Close();
            return retList;
        }

        private void createAndAddToElementElemnt(XmlDocument xd, XmlElement gameElement, string elementName, string elementValue)
        {
            var player1 = xd.CreateElement(elementName);
            var player1Name = xd.CreateTextNode(elementValue);
            player1.AppendChild(player1Name);
            gameElement.AppendChild(player1);
        }

        public void saveGameData(int gameType, string playerOneName, string playerTwoName, int gameTime, bool playerOneWin, int hardComputer)
        {
            if (!File.Exists(filePath))
                createXmlDocument(filePath);
            
            var xd = new XmlDocument();  
            var fs = new FileStream(filePath, FileMode.Open);  
            xd.Load(fs);  
    
            var user = xd.CreateElement("game");
            user.SetAttribute(@"gameType", gameType.ToString());

            createAndAddToElementElemnt(xd, user, @"TypeGame", gameType.ToString());
            createAndAddToElementElemnt(xd, user, @"Player1", playerOneName);
            createAndAddToElementElemnt(xd, user, @"Player2", playerTwoName);
            createAndAddToElementElemnt(xd, user, @"GameTime", gameTime.ToString());
            createAndAddToElementElemnt(xd, user, @"PlayerOneWin", playerOneWin.ToString());
            createAndAddToElementElemnt(xd, user, @"HardComputer", hardComputer.ToString());
            createAndAddToElementElemnt(xd, user, @"GameDate", Convert.ToString(DateTime.Now));

            if (xd.DocumentElement != null) xd.DocumentElement.AppendChild(user);
            fs.Close();         // Закрываем поток  
            xd.Save(filePath); // Сохраняем файл 
        }

        private void createXmlDocument(string filepath)
        {
            var xtw = new XmlTextWriter(filepath, Encoding.UTF32);
            xtw.WriteStartDocument();
            xtw.WriteStartElement("games");
            xtw.WriteEndDocument();
            xtw.Close();
        } 

        public void deleteFile()
        {
            File.Delete(filePath);
        }
    }
}