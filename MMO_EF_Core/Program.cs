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
            Console.WriteLine("[1] ReadAll");
            Console.WriteLine("[2] ShowItems");

            while (true) {
                Console.Write("> ");
                string command = Console.ReadLine();
                switch (command) {
                    case "0":
                        DbCommands.InitializeDB(forceReset: true);
                        break;
                    case "1":
                        DbCommands.ReadAll();
                        break;
                    case "2":
                        DbCommands.ShowItems();
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