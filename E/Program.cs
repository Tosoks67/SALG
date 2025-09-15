using System;
using System.IO;

namespace SALG
{
    internal class Program
    {
        enum Rank
        {
            None,
            Cadet,
            Private,
            Lance_Corporal,
            Corporal,
            Sergeant,
            Staff_Sergeant,
            Lieutenant,
            Major,
            Colonel,
            Assistant_Director_of_Security
        }

        enum RankShort
        {
            PVT = Rank.Private,
            LCPL = Rank.Lance_Corporal,
            CPL = Rank.Corporal,
            SGT = Rank.Sergeant,
            SSGT = Rank.Staff_Sergeant,
            LT = Rank.Lieutenant,
            MAJ = Rank.Major,
            COL = Rank.Colonel,
            ADoS = Rank.Assistant_Director_of_Security
        }

        static bool ToEnum(string value, out Rank result, Rank fallback = Rank.None)
        {
            if (Enum.TryParse(value, true, out Rank directRank))
            {
                result = directRank;
                return true;
            }

            if (Enum.TryParse(value, true, out RankShort shortRank))
            {
                result = (Rank)(int)shortRank;
                return true;
            }

            result = fallback;
            return false;
        }

        static void Setup()
        {
            Console.WriteLine("What's your Roblox username?");
            string user = Console.ReadLine() ?? "";
            Console.WriteLine("What's your current rank? (Skip the \"Security\" part, it will be added automatically)");
            string rank = Console.ReadLine() ?? "";
            rank = rank.Replace(' ', '_');
            ToEnum(rank, out Rank rankParsed, Rank.Cadet);
            Console.WriteLine("How many minutes of quota have you already done?");
            string quota = Console.ReadLine() ?? "";
            Console.WriteLine("How many minutes of total time on-site do you have?");
            string totalTime = Console.ReadLine() ?? "";
            Console.WriteLine("What is the current quota?");
            string reqQuota = Console.ReadLine() ?? "";
            Console.WriteLine("Do you wish to have a note added automatically?\n(If you skip you will be asked each time you generate a log)");
            string notes = Console.ReadLine() ?? "";
            if (notes == "") { File.Delete("notes"); } else { File.WriteAllText("notes", notes); }

            WriteData(user, rankParsed.ToString().Replace('_', ' '), quota, totalTime, reqQuota);

            Console.Clear();
        }

        static void CheckUp(string[] data)
        {
            /* syntax:
                    data[0] = Username
                    data[1] = Rank
                    data[2] = Quota Done
                    data[3] = Total Time
                    data[4] = Quota */
            ToEnum(data[1].Replace(' ', '_'), out Rank rank);
            if (data.Length > 5 || rank == Rank.None)
            {
                File.Delete("data");
                Console.Clear();
                Setup();
                Instructions(ReadData());
            }
        }

        static string[] ReadData()
        {
            string full = File.ReadAllText("data");
            string[] data = full.Split('|');
            return data;
        }

        static void WriteData(string username, string currentRank, string quotaDone, string totalTimeServed, string requiredQuota)
        {
            username = (username == "") ? "JohnDoe" : username;
            currentRank = (currentRank == "") ? "Cadet" : currentRank;
            quotaDone = (quotaDone == "") ? "0" : quotaDone;
            totalTimeServed = (totalTimeServed == "") ? "0" : totalTimeServed;
            requiredQuota = (requiredQuota == "") ? "120" : requiredQuota;
            File.WriteAllText("data", username + "|" + currentRank + "|" + quotaDone + "|" + totalTimeServed + "|" + requiredQuota);
        }

        static void Instructions(string[] data)
        {
            ToEnum(data[1].Replace(' ', '_'), out Rank rank);
            string extra = (rank == Rank.Assistant_Director_of_Security || rank == Rank.None) ? "" : "Security ";
            Console.WriteLine("You username is: " + data[0] + ".\nYour rank is: " + extra + rank.ToString().Replace('_',' ') + ".\nYou have done " + data[2] + " minutes of quota out of the required " + data[4] + ".\nYour total time served is: " + data[3] + " minutes.\n");
            Console.WriteLine("What do you wish to do?:\n    1. Run Setup again\n    2. Reset the quota\n    3. Clear the console\n    4. View permament note(s)\n    5. Generate an activity log");
        }

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
                string method = Console.ReadLine() ?? "";
                if (!File.Exists("data"))
                {
                    Setup();
                    Instructions(ReadData());
                }
                data = ReadData();
                switch (method)
                {
                    case "1":
                        Setup();
                        data = ReadData();
                        Instructions(data);
                        break;
                    case "2":
                        WriteData(data[0], data[1], "0", data[3], data[4]);
                        Console.Clear();
                        data = ReadData();
                        Instructions(data);
                        break;
                    case "3":
                        Console.Clear();
                        Instructions(data);
                        break;
                    case "4":
                        Console.Clear();
                        Console.WriteLine("Your permament note(s):\n");
                        if (File.Exists("notes"))
                        {
                            Console.WriteLine(File.ReadAllText("notes"));
                        }
                        else
                        {
                            Console.WriteLine("You have no permament notes.");
                        }
                        Console.WriteLine("\nPress any key to continue");
                        Console.ReadLine();
                        Console.Clear();
                        Instructions(data);
                        break;
                    case "5":
                        Console.WriteLine("Start time? (Add 1 hour if affected by Daylight Savings):");
                        string start = Console.ReadLine() ?? "0:00";
                        start = (start == "") ? "0:00" : start;
                        Console.WriteLine("End time?:");
                        string end = Console.ReadLine() ?? "0:01";
                        end = (end == "") ? "0:01" : end;
                        string note = "";
                        if (!File.Exists("notes"))
                        {
                            Console.WriteLine("Note? (Skip if none):");
                            note = Console.ReadLine() ?? "";
                        }
                        else
                        {
                            note = File.ReadAllText("notes");
                            Console.WriteLine("Add Note to Permament Note? (Skip if none):");
                            note = ((Console.ReadLine() ?? "") != "") ? note + " " + (Console.ReadLine() ?? "") : note;
                        }
                        note = (note != "") ? "\r\n**Note(s): **" + note : "";

                        string[] startSplitted = start.Split(':');
                        int startHour = Convert.ToInt32(startSplitted[0]);
                        int startMinute = Convert.ToInt32(startSplitted[1]);

                        if (startHour > 24 || startHour < 0 || startMinute > 60 || startMinute < 0) { startHour = 0; startMinute = 0; start = "0:00"; Console.WriteLine("I'm setting your start time to " + start + " because you're too stupid to write a proper start time."); }

                        string[] endSplitted = end.Split(':');
                        int endHour = Convert.ToInt32(endSplitted[0]);
                        int endMinute = Convert.ToInt32(endSplitted[1]);

                        if (endHour > 24 || endHour < 0 || endMinute > 60 || endMinute < 0) { endHour = 0; endMinute = 1; end = "0:01"; Console.WriteLine("I'm setting your end time to " + end + " because you're too stupid to write a proper end time."); }


                        TimeSpan startTime = new TimeSpan(startHour, startMinute, 0);
                        TimeSpan endTime = new TimeSpan(endHour, endMinute, 0);

                        if(endTime < startTime)
                        {
                            endTime = endTime.Add(TimeSpan.FromDays(1));
                        }

                        int difference = (int)(endTime - startTime).TotalMinutes;

                        Console.WriteLine("Copy the following text:\n");

                        int qDone = Convert.ToInt32(data[2]) + difference;
                        int tTime = Convert.ToInt32(data[3]) + difference;
                        Console.WriteLine("**Username: **" + data[0] + "\r\n**Rank: **Security " + data[1] + "\r\n**Start Time: **" + start + "\r\n**End time: **" + end + "\r\n**Total time on-site: **" + difference + " minutes\r\n**Total time: **" + tTime + " minutes\r\n-# Quota: " + qDone + " / " + data[4] + "\r\n__**Evidence: **__" + note);
                        WriteData(data[0], data[1], Convert.ToString(qDone), Convert.ToString(tTime), data[4]);
                        break;
                    default:
                        random = new Random();
                        int randomAnswer = random.Next(3);
                        switch (randomAnswer)
                        {
                            case 0:
                                Console.WriteLine("Is it that hard to choose a number between 1 and 5?");
                                break;
                            case 1:
                                Console.WriteLine("Are you fricking serious right now.");
                                break;
                            case 2:
                                Console.WriteLine("Do better, please.");
                                break;
                        }
                        break;
                }
            }
        }
    }
}
