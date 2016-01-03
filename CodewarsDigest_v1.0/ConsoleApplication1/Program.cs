using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        public static List<string[]> ClearingUserInfo(List<string[]> listOfUsers)
        {
            List < string[]> final = new List<string[]>();
            foreach (var userArray in listOfUsers) 
            {
                var lastIndex = userArray[0].LastIndexOf('='); // If line contains =3
                if (lastIndex != -1)
                    userArray[0] = userArray[0].Substring(lastIndex + 2, userArray[0].Length - (lastIndex + 2));
                else
                    userArray[0] = userArray[0].Replace("Profile-pic", "");
            }

            return listOfUsers;
        }

        public static List<string[]> VkLinkInsertion(List<string[]> listOfNicknames)
        {
            using (FileStream fileNicks = new FileStream(@"C:\Users\Максим\Documents\Visual Studio 2015\Projects\Docs\nicknames.txt", FileMode.Open))
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

        public static List<string[]> ReadingLastWeekRating(string path)
        {
            FileStream stream = new FileStream(path, FileMode.Open);
            StreamReader info = new StreamReader(stream);
            List<string[]> listOfUsers = new List<string[]>();

            while (!info.EndOfStream) // Separating input file to fields with user info
            {
                string[] user = info.ReadLine()
                    .Split(default(string[]), StringSplitOptions.RemoveEmptyEntries);
                listOfUsers.Add(user);
            }
            return listOfUsers;
        }

        public static List<string[]> AbsoluteRating(string path)
        {
            using (
                FileStream fileIn = new FileStream(@"C:\Users\Максим\Documents\Visual Studio 2015\Projects\Docs\cw.txt", FileMode.Open))
            {
                using (StreamReader streamIn = new StreamReader(fileIn))
                {
                    List<string[]> listOfUsers = new List<string[]>();
                    List<string[]> listOfNicknames = new List<string[]>();

                    while (!streamIn.EndOfStream) // Separating input file to fields with user info
                    {
                        streamIn.ReadLine(); // Skip kyu rank
                        string[] user = streamIn.ReadLine()
                            .Split(default(string[]), StringSplitOptions.RemoveEmptyEntries);
                        listOfUsers.Add(user);
                    }
                    
                    return listOfUsers = ClearingUserInfo(listOfUsers);
                }
            }
        }

        public static void WeeklyRating()
        {
            var trialPath = @"C:\Users\Максим\Documents\Visual Studio 2015\Projects\Docs\totalRating";
            var weekNumber = 50;
            do
            {
                if (File.Exists(trialPath + weekNumber + ".txt"))
                    break;
                weekNumber--;
            } while (weekNumber >= 0);

            var lastWeekRating = ReadingLastWeekRating(trialPath + weekNumber + ".txt");
            weekNumber++;
            var thisWeekRating = AbsoluteRating(trialPath + weekNumber + ".txt");

            Console.Read();

        }

        public static void PrintingOutput(List<string[]> listOfNicknames, List<string[]> listOfUsers)
        {
            FileStream fileOut =
                new FileStream(@"C:\Users\Максим\Documents\Visual Studio 2015\Projects\Docs\out.txt",
                    FileMode.Create);

            StreamWriter streamOut = new StreamWriter(fileOut);

            var sum = 0;
            int i = 1;
            foreach (var userArray in listOfUsers) // Printing output
            {
                foreach (var nickAndLink in listOfNicknames)
                    if (userArray[0] == nickAndLink[0])
                        userArray[0] = $"@{nickAndLink[1]} ({userArray[0]})";

                Console.Write($"{i}   {userArray[0]} ({userArray[userArray.Length - 1]} points) ");
                Console.WriteLine();
                streamOut.Write($"{i} {userArray[0]} ({userArray[userArray.Length - 1]} points) ");
                streamOut.WriteLine();
                sum += int.Parse(userArray[userArray.Length - 1]);
                i++;
            }
            Console.WriteLine(sum);
            streamOut.Close();

        }

        private static void Main(string[] args)
        {

            WeeklyRating();
            Console.ReadKey();

        }
    }
}
