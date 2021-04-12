using System;
using System.Diagnostics;
using System.Xml;
namespace ResolutionHelper
{
    
    class Program
    {
        static Resolution res = new Resolution();

        static void Main(string[] args)
        {
            Process[] procs;
            Process steam= null;
            Process game=null;

            //short height=short.MinValue, width = short.MinValue, gameId = short.MinValue;
            //string launch_options = string.Empty;

            //ReadConfig(ref height, ref width, ref gameId, ref launch_options);

            try
            {
                procs = Process.GetProcessesByName("steam");
                if (procs.Length == 0)
                {//no processes
                    res.ChangeDisplaySettings(1440, 1080);
                   // res.ChangeDisplaySettings(width, height);
                    GameHandler(ref steam, procs, ref game);
                }
                else
                {                   
                    steam = procs[0];
                    
                    res.ChangeDisplaySettings(1440, 1080);
                    GameHandler(ref steam,procs,ref game);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Debug.WriteLine("Runtime error.\n" + ex.ToString());
                throw;
            }
            finally
            {
                Console.WriteLine("Press a key to exit.");
                Console.ReadLine();
                while (!game.HasExited)
                {
                    System.Threading.Thread.Sleep(10000);
                }
                GC.Collect();
            }
            
        }

        private static void GameClosed(object sender, EventArgs e)
        {
            res.RestoreDisplaySettings();
            Console.WriteLine("Restoring Display Settings");
        }

        //Launches Steam
        public static Process LaunchTaskSteam()
        {
            Process process = new Process();
            process.StartInfo.FileName = "C:\\Program Files (x86)\\Steam\\steam.exe";
            process.StartInfo.Arguments = "-applaunch 730 -noborder -windowed -w 1440 -h 1080 -novid";
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.Start();
            return process;
        }

        //Enables the Exited event in order to get the resolution back.
        public static void  GameHandler(ref Process steam,Process[] procs, ref Process game)
        {
            
            steam = LaunchTaskSteam();
            Console.WriteLine("Steam.exe Process id = {0}", steam.Id);


            while ((procs = Process.GetProcessesByName("csgo")).Length < 1) {
                procs = Process.GetProcessesByName("csgo");
                System.Threading.Thread.Sleep(100);
            }

            game = procs[0];
            game.EnableRaisingEvents = true;
            game.Exited += new EventHandler(GameClosed);
            Console.WriteLine("CounterStrike G.O. Process ID = {0}", game.Id); 
        }

        #region Config Functions
        //Function to get the configuration parameters
        public static void ReadConfig(ref short height, ref short width, ref short gameId, ref string launch_options)
        {

            XmlDocument doc = new XmlDocument();
            doc.Load("config.xml");

            XmlNode root = doc.DocumentElement;

            height = Convert.ToInt16(root.SelectSingleNode("/configuration/resolution/height").InnerText);

            width = Convert.ToInt16(root.SelectSingleNode("/configuration/resolution/width").InnerText);

            gameId = Convert.ToInt16(root.SelectSingleNode("/configuration/application/gameId").InnerText);

            foreach (XmlNode option in root.SelectSingleNode("/configuration/options"))
            {
                launch_options += option.InnerText + " ";
            }


        }
        //Function to write the configuration parameters
        public static void NewConfig()
        {
            string input;
            string buffer;
            do
            {
                Console.WriteLine("Insert the Resolution width\nExample:1440x1080");
                buffer = Console.ReadLine();
            }
            while (!buffer.Contains('x') && buffer.Length > 9 && buffer.Length < 1);
            input = buffer + ";";
            do
            {
                Console.WriteLine("Insert the game id\nExample:730 for CS:GO");
                buffer = Console.ReadLine();
            }
            while (!buffer.Contains('x') && buffer.Length > 9 && buffer.Length < 1);
            input += buffer + ";";

            Console.WriteLine("If game options needed type them\n Example: -novid -w 1440 -h 1080 -windowed -noborder");
            buffer = Console.ReadLine();
            input += buffer + ";";


            // sanitize the inputs


        }
        #endregion
    }


}
