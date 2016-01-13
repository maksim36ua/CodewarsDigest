using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using UserInfoClass;
using System.IO;

namespace CodewarsDigest_v1._2
{
    class Print
    {

        private static List<UserInfo> SortActiveUsersOfThisWeek(List<UserInfo> userList)
        {
            userList.RemoveAll(user => user.PointsForThisWeek == 0);
            List<UserInfo> sortedList = userList.OrderByDescending(user => user.PointsForThisWeek).ToList();
            return sortedList;
        }

        private static void InTXT(List<UserInfo> userList, int thisWeekNumber)
        {
            using (FileStream file = new FileStream(@"Data\WeeklyRating\totalRating" + thisWeekNumber + ".txt", FileMode.Append))
            {
                using (StreamWriter stream = new StreamWriter(file))
                {
                    foreach (var user in userList)
                        stream.WriteLine(user.Name + " " + user.CurrentPoints);
                }
            }

            using (FileStream file = new FileStream(@"Data\ForSite.txt", FileMode.Open))
            {
                using (StreamWriter stream = new StreamWriter(file))
                {
                    for (int i = 0; i < userList.Count; i++)
                        stream.WriteLine($"{i + 1}.  {userList[i].Name}  ({userList[i].CurrentPoints}, { userList[i].PointsForThisWeek})");

                }
            }
        }

        private static void InHTML(List<UserInfo> activeUserList)
        {
            HtmlDocument page = new HtmlDocument();
            page.Load(@"Data\HTMLRating\Rating.html");

            for (int i = 0; i < 10; i++)
            {
                HtmlTextNode hName = null;
                hName = page.DocumentNode
                    .SelectSingleNode($"//div[@id='name{i}']//text()") as HtmlTextNode;
                hName.Text = activeUserList[i].Name;

                HtmlTextNode hPoints = null;
                hPoints = page.DocumentNode
                    .SelectSingleNode($"//div[@id='points{i}']//text()") as HtmlTextNode;
                hPoints.Text = activeUserList[i].PointsForThisWeek.ToString();
            }
            page.Save(@"Data\HTMLRating\Rating.html");
        } // TODO

        private static void InConsole(List<UserInfo> activeUserList, List<string[]> listOfNicknamesAndVKLinks)
        {
            for (int i = 0; i < activeUserList.Count; i++)
                foreach (var nick in listOfNicknamesAndVKLinks)
                {
                    if (nick.Length == 2) // If username consists of 1 word
                    {
                        if (nick[0] == activeUserList[i].Name)
                            activeUserList[i].Name = $"@{nick[1]}({ activeUserList[i].Name})";
                    }

                    else if (nick.Length == 3) // If username consists of 2 words
                    {
                        if ((nick[0] + " " + nick[1]) == activeUserList[i].Name)
                            activeUserList[i].Name = $"@{nick[2]}({ activeUserList[i].Name})";
                    }
                }

            for (int i = 0; i < activeUserList.Count; i++)
                Console.WriteLine($"{i + 1}.  {activeUserList[i].Name}  ({activeUserList[i].PointsForThisWeek})");

        }

        public static void InConsoleHtmlTxt(List<UserInfo> userList, int thisWeekNumber)
        {
            List<string[]> listOfNicknamesAndVKLinks = Read.ExtractVkLinksFromTXT();

            InTXT(userList, thisWeekNumber);

            List<UserInfo> activeUserList = SortActiveUsersOfThisWeek(userList);
            
            InHTML(activeUserList);
            InConsole(activeUserList, listOfNicknamesAndVKLinks);
            

        }
    }
}
