using System;
using System.IO;
using static SALG.Functions;

namespace SALG
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Security Activity Log Generator";
            Console.ForegroundColor = ConsoleColor.Yellow;
            if (!File.Exists("data"))
            {
                Setup();
            }
            Random random;
            string[] data = ReadData();
            Instructions(data);
            while (true)
            {
                CheckUp(data);
                string method = CWrite("", false, "MainMenu", true);
                if (!File.Exists("data"))
                {
                    Setup();
                    Instructions(ReadData());
                }
                data = ReadData();
                switch (method)
                {
                    case "1":
                        Setup(false);
                        data = ReadData();
                        Instructions(data);
                        break;
                    case "2":
                        WriteData(data[0], data[1], "0", data[3], data[4], data[5]);
                        data = ReadData();
                        Instructions(data);
                        break;
                    case "3":
                        Instructions(data);
                        break;
                    case "4":
                        Console.Clear();
                        CWrite("Your permament note(s):\n", false, "MainMenu");
                        if (File.Exists("notes"))
                        {
                            CWrite(File.ReadAllText("notes"), true);
                        }
                        else
                        {
                            CWrite("You have no permament notes.", true, "MainMenu");
                        }
                        CWrite("\nPress any key to continue", true, "MainMenu", true);
                        Instructions(data);
                        break;
                    case "5":
                        string start = CWrite("Start time? (Add 1 hour if affected by Daylight Savings):", false, "System", true);
                        start = (start == "") ? "0:00" : start;
                        string end = CWrite("End time?:", false, "System", true);
                        end = (end == "") ? "0:01" : end;
                        string note = "";
                        if (!File.Exists("notes"))
                        {
                            note = CWrite("Note? (Skip if none):", false, "System", true);
                        }
                        else
                        {
                            note = File.ReadAllText("notes");
                            note = (CWrite("Add Note to Permament Note? (Skip if none):", false, "System", true) != "") ? note + " " + CWrite("Add Note to Permament Note? (Skip if none):", false, "System", true) : note;
                        }
                        note = (note != "") ? "\r\n**Note(s): **" + note : "";

                        string[] startSplitted = start.Split(':');
                        int startHour = Convert.ToInt32(startSplitted[0]);
                        int startMinute = Convert.ToInt32(startSplitted[1]);

                        if (startHour > 24 || startHour < 0 || startMinute > 60 || startMinute < 0) { startHour = 0; startMinute = 0; start = "0:00"; Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine("I'm setting your start time to " + start + " because you're too stupid to write a proper one."); }

                        string[] endSplitted = end.Split(':');
                        int endHour = Convert.ToInt32(endSplitted[0]);
                        int endMinute = Convert.ToInt32(endSplitted[1]);

                        if (endHour > 24 || endHour < 0 || endMinute > 60 || endMinute < 0) { endHour = 0; endMinute = 1; end = "0:01"; Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine("I'm setting your end time to " + end + " because you're too stupid to write a proper one."); }


                        TimeSpan startTime = new TimeSpan(startHour, startMinute, 0);
                        TimeSpan endTime = new TimeSpan(endHour, endMinute, 0);

                        if (endTime < startTime)
                        {
                            endTime = endTime.Add(TimeSpan.FromDays(1));
                        }

                        int difference = (int)(endTime - startTime).TotalMinutes;

                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("\nCopy the following text:\n");

                        int qDone = Convert.ToInt32(data[2]) + difference;
                        int tTime = Convert.ToInt32(data[3]) + difference;
                        string qShown = (data[5].ToUpper() == "Y") ? "\r\n-# Quota: " + qDone + " / " + data[4] : "";
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.WriteLine("**Username: **" + data[0] + "\r\n**Rank: **Security " + data[1] + "\r\n**Start Time: **" + start + "\r\n**End time: **" + end + "\r\n**Total time on-site: **" + difference + " minutes\r\n**Total time: **" + tTime + " minutes" + qShown + "\r\n__**Evidence: **__" + note);
                        WriteData(data[0], data[1], Convert.ToString(qDone), Convert.ToString(tTime), data[4], data[5]);
                        break;
                    default:
                        random = new Random();
                        int randomAnswer = random.Next(3);
                        switch (randomAnswer)
                        {
                            case 0:
                                CWrite("Is it that hard to choose a number between 1 and 5?", false, " ");
                                break;
                            case 1:
                                CWrite("Are you fricking serious right now.", false, " ");
                                break;
                            case 2:
                                CWrite("Do better, please.", false, " ");
                                break;
                        }
                        break;
                }
            }
        }
    }
}
