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

        enum CWriteMessageType
        {
            Stupid,
            System,
            MainMenu
        }

        static bool RankStringToEnum(string value, out Rank result, Rank fallback = Rank.None)
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

        static string CWrite(string input, bool newline = false, string mType = "System", bool read = false)
        {
            Enum.TryParse(mType, true, out CWriteMessageType mTypeEnum);
            Console.ForegroundColor = (mTypeEnum == CWriteMessageType.System) ? ConsoleColor.Green : (mTypeEnum == CWriteMessageType.MainMenu) ? ConsoleColor.Cyan : ConsoleColor.Red;
            input = (newline) ? "\n" + input : input;
            Console.Write(input);
            if (read)
            {
                Console.Write('\n');
                Console.ForegroundColor = (mTypeEnum == CWriteMessageType.System) ? ConsoleColor.Magenta : ConsoleColor.Yellow;
                return Console.ReadLine() ?? "";
            }
            else
            {
                return "";
            }
        }

        static void Setup()
        {
            Console.Clear();
            string user = CWrite("What's your Roblox username?", false, "System", true);
            string rank = CWrite("What's your current rank? (Skip the \"Security\" part, it will be added automatically)", true, "System", true);
            rank = rank.Replace(' ', '_');
            RankStringToEnum(rank, out Rank rankParsed, Rank.Cadet);
            string quota = CWrite("How many minutes of quota have you already done?", true, "System", true);
            string totalTime = CWrite("How many minutes of total time do you have?", true, "System", true);
            string reqQuota = CWrite("What is the current quota?", true, "System", true);
            string notes = CWrite("Do you wish to have a note added automatically?\n(If you skip you will be asked each time you generate a log)", true, "System", true);
            string showQDone = CWrite("Do you wish to have your Quota Done shown on the log? (Y/N)", true, "System", true);
            if (notes == "") { File.Delete("notes"); } else { File.WriteAllText("notes", notes); }

            WriteData(user, rankParsed.ToString().Replace('_', ' '), quota, totalTime, reqQuota, showQDone);

            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
        }

        static void CheckUp(string[] data)
        {
            /* syntax:
                    data[0] = Username
                    data[1] = Rank
                    data[2] = Quota Done
                    data[3] = Total Time
                    data[4] = Quota
                    data[5] = Show Quota Done */
            RankStringToEnum(data[1].Replace(' ', '_'), out Rank rank);
            if (data.Length < 6 || rank == Rank.None || !int.TryParse(data[2], out int _) || !int.TryParse(data[3], out int _) || !int.TryParse(data[4], out int _) || (data[5].ToUpper() != "Y" && data[5].ToUpper() != "N") || data.Length > 6)
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

        static void WriteData(string username, string currentRank, string quotaDone, string totalTimeServed, string requiredQuota, string showQuotaDone)
        {
            username = (username == "") ? "John Doe" : username;
            currentRank = (currentRank == "") ? "Cadet" : currentRank;
            quotaDone = (quotaDone == "") ? "0" : quotaDone;
            totalTimeServed = (totalTimeServed == "") ? "0" : totalTimeServed;
            requiredQuota = (requiredQuota == "") ? "120" : requiredQuota;
            showQuotaDone = (showQuotaDone == "" || (showQuotaDone.ToUpper() != "Y" && showQuotaDone.ToUpper() != "N")) ? "Y" : showQuotaDone;
            File.WriteAllText("data", username + "|" + currentRank + "|" + quotaDone + "|" + totalTimeServed + "|" + requiredQuota + "|" + showQuotaDone);
        }

        static void Instructions(string[] data)
        {
            Console.Clear();
            RankStringToEnum(data[1].Replace(' ', '_'), out Rank rank);
            string extra = (rank == Rank.Assistant_Director_of_Security || rank == Rank.None) ? "" : "Security ";
            CWrite("You username is: " + data[0] + ".\nYour rank is: " + extra + rank.ToString().Replace('_', ' ') + ".\nYou have done " + data[2] + " minutes of quota out of the required " + data[4] + ".\nYour total time served is: " + data[3] + " minutes.\nQuota shown?: " + data[5] + "\n", false, "MainMenu");
            CWrite("What do you wish to do?:\n    1. Run Setup again\n    2. Reset the quota\n    3. Clear the console\n    4. View permament note(s)\n    5. Generate an activity log", true, "MainMenu");
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
                        Setup();
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
