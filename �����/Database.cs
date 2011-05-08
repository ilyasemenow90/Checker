using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Шашки
{
    class Database
    {
        static Database instance = null;
        static readonly object padlock = new object();

        Database()
        {
            
        }

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



        public void loadAllDataFromFile()
        {
            
        }

        public void saveGameData()
        {
            
        }

    }
}