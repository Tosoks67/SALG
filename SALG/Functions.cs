using System;
using System.IO;

namespace SALG
{
    internal static class Functions
    {
        public static bool RankStringToEnum(string value, out Rank result, Rank fallback = Rank.None)
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

        public static string CWrite(string input, bool newline = false, string mType = "System", bool read = false)
        {
            Enum.TryParse(mType, true, out CWriteMessageType mTypeEnum);
            Console.ForegroundColor = mTypeEnum == CWriteMessageType.System ? ConsoleColor.Green : mTypeEnum == CWriteMessageType.MainMenu ? ConsoleColor.Cyan : ConsoleColor.Red;
            input = newline ? "\n" + input : input;
            Console.Write(input);
            if (read)
            {
                Console.Write('\n');
                Console.ForegroundColor = mTypeEnum == CWriteMessageType.System ? ConsoleColor.Magenta : ConsoleColor.Yellow;
                return Console.ReadLine() ?? "";
            }
            else
            {
                return "";
            }
        }

        public static void CheckUp(string[] data)
        {
            /* syntax:
                    data[0] = Username
                    data[1] = Rank
                    data[2] = Quota Done
                    data[3] = Total Time
                    data[4] = Quota
                    data[5] = Show Quota Done */
            RankStringToEnum(data[1].Replace(' ', '_'), out Rank rank);
            if (data.Length != 6 || rank == Rank.None || !int.TryParse(data[2], out int _) || !int.TryParse(data[3], out int _) || !int.TryParse(data[4], out int _) || data[5].ToUpper() != "Y" && data[5].ToUpper() != "N")
            {
                File.Delete("data");
                Console.Clear();
                Setup();
                Instructions(ReadData());
            }
        }

        public static string[] ReadData()
        {
            string full = File.ReadAllText("data");
            string[] data = full.Split('|');
            return data;
        }

        public static void WriteData(string username, string currentRank, string quotaDone, string totalTimeServed, string requiredQuota, string showQuotaDone)
        {
            username = username == "" ? "John Doe" : username;
            currentRank = currentRank == "" ? "Cadet" : currentRank;
            quotaDone = quotaDone == "" ? "0" : quotaDone;
            totalTimeServed = totalTimeServed == "" ? "0" : totalTimeServed;
            requiredQuota = requiredQuota == "" ? "120" : requiredQuota;
            showQuotaDone = showQuotaDone == "" ? "Y" : showQuotaDone;
            File.WriteAllText("data", username + "|" + currentRank + "|" + quotaDone + "|" + totalTimeServed + "|" + requiredQuota + "|" + showQuotaDone);
        }

        public static void Instructions(string[] data)
        {
            Console.Clear();
            RankStringToEnum(data[1].Replace(' ', '_'), out Rank rank);
            string extra = rank == Rank.Assistant_Director_of_Security || rank == Rank.None ? "" : "Security ";
            CWrite("You username is: " + data[0] + ".\nYour rank is: " + extra + rank.ToString().Replace('_', ' ') +
                ".\nYou have done " + data[2] + " minutes of quota out of the required " + data[4] +
                ".\nYour total time served is: " + data[3] + " minutes.\nQuota shown?: " + data[5] +
                "\n", false, "MainMenu");
            CWrite("What do you wish to do?:" +
                "\n    1. Run Setup again" +
                "\n    2. Reset the quota" +
                "\n    3. Clear the console" +
                "\n    4. View permament note(s)" +
                "\n    5. Generate an activity log", true, "MainMenu");
        }
        public static void Setup(bool first = true)
        {
            Console.Clear();
            string[] current;
            current = !first ? ReadData() : Array.Empty<string>();
            if (first)
            {
                CWrite("Data missing/corrupted, skip-to-keep not supported.\n\n", false, "MainMenu");
            }
            else
            {
                CWrite("Data found, skip the question if you want to keep it as is.\n\n", false, "MainMenu");
            }
            string user = first ? CWrite("What's your Roblox username?", true, "System", true) : CWrite("What's your Roblox username? (Current: '" + current[0] + "')", true, "System", true);
            user = user == "" && !first ? current[0] : user;
            string rank = first ? CWrite("What's your current rank?\n(You can use shortcuts e.g. \"PVT\" or \"ADoS\")", true, "System", true) : CWrite("What's your current rank? (Current: '" + current[1].Replace('_', ' ') + "')\n(You can use shortcuts e.g. \"PVT\" or \"ADoS\")", true, "System", true);
            rank = rank == "" && !first ? current[1] : rank;
            rank = rank.Replace(' ', '_');
            RankStringToEnum(rank, out Rank rankParsed, Rank.Cadet);
            string quota = first ? CWrite("How many minutes of quota have you already done?", true, "System", true) : CWrite("How many minutes of quota have you already done? (Current: '" + current[2] + "')", true, "System", true);
            quota = quota == "" && !first ? current[2] : quota;
            string totalTime = first ? CWrite("How many minutes of total time do you have?", true, "System", true) : CWrite("How many minutes of total time do you have? (Current: '" + current[3] + "')", true, "System", true);
            totalTime = totalTime == "" && !first ? current[3] : totalTime;
            string reqQuota = first ? CWrite("What is the current quota?", true, "System", true) : CWrite("What is the current quota? (Current: '" + current[4] + "')", true, "System", true);
            reqQuota = reqQuota == "" && !first ? current[4] : reqQuota;
            string notes = first ? CWrite("Do you wish to have a note added automatically?\n(Skip if not)", true, "System", true) : File.Exists("notes") ? CWrite("Do you wish to have a note added automatically? (You currently have one)\n(Type '~' to clear)", true, "System", true) : CWrite("Do you wish to have a note added automatically? (You currently don't have one)", true, "System", true);
            if (!first && File.Exists("notes")) { notes = notes == "~" ? "" : notes == "" ? File.ReadAllText("notes") : notes; }
            string showQDone = first ? CWrite("Do you wish to have your Quota Done shown on the log? (Y/N)", true, "System", true) : CWrite("Do you wish to have your Quota Done shown on the log? (Y/N) (Current: '" + current[5] + "')", true, "System", true);
            showQDone = showQDone == "" && !first ? current[5] : showQDone;
            if (notes == "") { File.Delete("notes"); } else { File.WriteAllText("notes", notes); }

            WriteData(user, rankParsed.ToString().Replace('_', ' '), quota, totalTime, reqQuota, showQDone);

            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
        }
    }
}
