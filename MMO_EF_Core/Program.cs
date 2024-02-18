using System;

namespace MMO_EF_Core
{
    class Program
    {
        
    
        static void Main(string[] args)
        {
            

            //CRUD (Create, Read, Update, Delete)
            Console.WriteLine("명령어 입력");
            Console.WriteLine("[0] Force Reset");
            Console.WriteLine("[1] Update (Reload)");
            Console.WriteLine("[2] Update (Full)");
            

            while (true) {
                Console.Write("> ");
                string command = Console.ReadLine();
                switch (command) {
                    case "0":
                        DbCommands.InitializeDB(forceReset: true);
                        break;
                    case "1":
                        DbCommands.UpdateByReload();
                        break;
                    case "2":
                        DbCommands.UpdateByFull();
                        break;
                    case "3":
                        
                        break;
                    default:
                        break;
                }
            }
        }
    }
}