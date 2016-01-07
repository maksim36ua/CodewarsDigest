using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using System.IO;
using UserInfoClass;
using CodewarsDigest_v1._2;


namespace CodewarsDigest_v1._2
{
    class Program
    {
        
        static void Main(string[] args)
        {
            List<UserInfo> userList = new List<UserInfo>();
            int thisWeekNumber = 0;

            for (int i = 0; i < 100; i++) // Checking what is the number of current week
            {
                if (!File.Exists(@"Data\WeeklyRating\totalRating" + i + ".txt"))
                {
                    thisWeekNumber = i;
                    break;
                }
                else continue;
            }

            Read.AllInfo(userList, thisWeekNumber - 1);
            Print.InConsoleHtmlTxt(userList, thisWeekNumber);

            Console.Read();
        }

    }
}
