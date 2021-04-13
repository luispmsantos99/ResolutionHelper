using System;
using System.Diagnostics;
using System.IO;
using System.Timers;
using System.Xml;

namespace ResolutionHelper
{
    
    class Program
    {
        static void Main(string[] args)
        {
            Process steam= null;
            Process game=null;
            Resolution res = new Resolution();

            short height=short.MinValue, width = short.MinValue, gameId = short.MinValue;
            string launch_options = string.Empty;
            #region HARDCODED VALUES
            /*gameId = 730;
            width = 1440;
            height = 1080;
            launch_options = "-noborder -windowed -w 1440 -h 1080 -novid";
            */
            #endregion
            
            try
            {
                ReadConfig(ref height, ref width, ref gameId, ref launch_options);
                Console.WriteLine("Resolution: {0}x{1}\nGame {2}\nLaunch options: {3}",width,height,gameId,launch_options);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Runtime error.\n" + ex.ToString());
                Console.ReadLine();
                gameId = 730;
                width = 1440;
                height = 1080;
                launch_options = "-noborder -windowed -w 1440 -h 1080 -novid";
            }            

            try
            {
                res.ChangeDisplaySettings(width,height);
                GameHandler(ref steam,ref game,gameId,launch_options);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Runtime error.\n" + ex.ToString());
                Console.ReadLine();
                throw;
            }
            finally
            {
                Console.WriteLine("Game is running ...");
                game.WaitForExit();
                
                res.RestoreDisplaySettings();                

                Console.Write("Press <q> to exit... ");
                
                
                    
                while (Console.ReadKey(true).Key != ConsoleKey.Q);
                game.Dispose();
                

                GC.Collect();
            }

        }

        private static void GameClosed(object sender, EventArgs e)
        {          
            Console.WriteLine("Restoring Display Settings");
        }

        //Launches Steam
        public static Process LaunchTaskSteam(short gameId,string launchOptions)
        {
            Process process = new Process();
            process.StartInfo.FileName = "C:\\Program Files (x86)\\Steam\\steam.exe";
            //process.StartInfo.Arguments = "-applaunch 730 -noborder -windowed -w 1440 -h 1080 -novid";
            process.StartInfo.Arguments = "-applaunch "+gameId.ToString()+" "+launchOptions;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.Start();
            return process;
        }

        //Enables the Exited event in order to get the resolution back.
        public static void  GameHandler(ref Process steam, ref Process game, short gameId, string launchOptions)
        {
            Process[] procs;
            steam = LaunchTaskSteam(gameId,launchOptions);
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



        #region Clean Function
        public static void Clean(ref Process [] list)
        {
            foreach (Process proc in list)
            {
                proc.Dispose();
            }

            
        }
        #endregion

        #region Config Functions
        //Function to get the configuration parameters
        public static void ReadConfig(ref short height, ref short width, ref short gameId, ref string launch_options)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(Directory.GetCurrentDirectory() + "\\config.cfg");

                XmlNode root = doc.DocumentElement;

                height = Convert.ToInt16(root.SelectSingleNode("/configuration/resolution/height").InnerText);

                width = Convert.ToInt16(root.SelectSingleNode("/configuration/resolution/width").InnerText);

                gameId = Convert.ToInt16(root.SelectSingleNode("/configuration/application/gameId").InnerText);

                foreach (XmlNode option in root.SelectSingleNode("/configuration/options"))
                {
                    launch_options += option.InnerText + " ";
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Runtime error.\n" + ex.ToString());
                Console.ReadLine();
                throw;
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
