using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codewars_Parser
{
    public interface IRating
    {
        void ReadLastWeekRating();
        void ReadThisWeekRating();
        List<string[]> VkLinkInsertion(List<string[]> listOfNickNames);
        List<string[]> ClearThisWeekRating(List<string[]> userInfo);
        void CompareRatings();
        void PrintRatings();
    }

    class RatingClass : IRating
    {
        private List<userInfoClass> listOfUserObjects = new List<userInfoClass>();

        private string path = @"C:\Users\Максим\Documents\Visual Studio 2015\Projects\Docs\";
        private int weekNumber;



        public void ReadLastWeekRating ()
        {
            int previousWeekNumber = weekNumber--;
            FileStream stream = new FileStream(path + "totalRating" + previousWeekNumber + ".txt", FileMode.Open);
            StreamReader info = new StreamReader(stream);

            List<string[]> listOfUsers = new List<string[]>();

            while (!info.EndOfStream) // Separating input file to fields with user info
            {
                string[] userInfo = info.ReadLine().Split(default(string[]), StringSplitOptions.RemoveEmptyEntries);
                listOfUsers.Add(userInfo);
            }

            foreach (var userInfo in listOfUsers)
            {
                listOfUserObjects.Add(new userInfoClass(userInfo[0], lastWeekPoints: int.Parse(userInfo[userInfo.Length-1])));
            }
        }

        public void ReadThisWeekRating()
        {
            FileStream fileIn = new FileStream(path + "cw.txt", FileMode.Open);
            StreamReader streamIn = new StreamReader(fileIn);

            List<string[]> listOfUsers = new List<string[]>();

            while (!streamIn.EndOfStream) // Separating input file to fields with user info
            {
                streamIn.ReadLine(); // Skip kyu rank
                string[] user = streamIn.ReadLine().Split(default(string[]), StringSplitOptions.RemoveEmptyEntries);
                listOfUsers.Add(user);
            }

            listOfUsers = ClearThisWeekRating(listOfUsers);

            bool isPresent = false;

            foreach (var userInfo in listOfUsers)
            {
                for (int i = 0; i < listOfUserObjects.Count; i++)
                { 
                    if (userInfo[0] == listOfUserObjects[i].Name)
                    {
                        listOfUserObjects[i].CurrentPoints= int.Parse(userInfo[userInfo.Length-1]);
                            // Updating current week points if user object already exists
                        isPresent = true;
                        break;
                    }
                }
                if (!isPresent)
                    listOfUserObjects.Add(new userInfoClass(userInfo[0], currentPoints: int.Parse(userInfo[userInfo.Length - 1]))); 
                        // Adding object to main list if user is new
            }

        }

        public List<string[]> ClearThisWeekRating (List<string[]> listOfUsers)
        {
            for (int i = 0; i < listOfUsers.Count; i++)//foreach (var userArray in listOfUsers)
            {
                var lastIndex = listOfUsers[i][0].LastIndexOf('='); // If line contains =3
                if (lastIndex != -1)
                    listOfUsers[i][0] = listOfUsers[i][0].Substring(lastIndex + 2, listOfUsers[i][0].Length - (lastIndex + 2));
                else
                    listOfUsers[i][0] = listOfUsers[i][0].Replace("Profile-pic", "");
            }
            return listOfUsers;
        }

        public List<string[]> VkLinkInsertion(List<string[]> listOfNicknames)
        {
            using (FileStream fileNicks = new FileStream(path + "nicknames.txt", FileMode.Open))
            {
                using (StreamReader streamNicks = new StreamReader(fileNicks))
                {
                    while (!streamNicks.EndOfStream) // Separating nickname file to user field
                    {
                        string[] nickAndLink = streamNicks.ReadLine()
                            .Split(default(string[]), StringSplitOptions.RemoveEmptyEntries);
                        listOfNicknames.Add(nickAndLink);
                    }

                    foreach (var nickAndLink in listOfNicknames) // remove "vk.com"
                    {
                        var lastIndex = nickAndLink[1].LastIndexOf('/');
                        if (lastIndex != -1)
                        {
                            nickAndLink[1] = nickAndLink[1].Substring(lastIndex + 1,
                                nickAndLink[1].Length - (lastIndex + 1));
                        }
                    }

                    return listOfNicknames;
                }
            }
        }

        public void CompareRatings ()
        {
            for (int i = 0; i < listOfUserObjects.Count; i++)
                listOfUserObjects[i].PointsForThisWeek = listOfUserObjects[i].CurrentPoints -
                                                         listOfUserObjects[i].LastWeekPoints;
            // Sort users by rating 

        }

        public void PrintRatings ()
        {
            CompareRatings();
            foreach (var userInstance in listOfUserObjects)
            {
                Console.WriteLine(userInstance.Name + ": " + userInstance.LastWeekPoints + ", " + userInstance.CurrentPoints);
                Console.WriteLine(userInstance.Name + ": " + userInstance.PointsForThisWeek);
            }

            using (FileStream fileOut =
                new FileStream(@"C:\Users\Максим\Documents\Visual Studio 2015\Projects\Docs\out.txt",
                    FileMode.Create))
            {
                using (StreamWriter streamOut = new StreamWriter(fileOut))
                {
                   
                }

            }

        }

    }
}
