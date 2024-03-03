using System;

namespace MMO_EF_Core
{
    class Program
    {    
        static void Main(string[] args)
        {
            Console.WriteLine("명령어 입력");
            Console.WriteLine("[0] Force Reset");
            Console.WriteLine("[1] Show Items");
            Console.WriteLine("[2] Test Update Attach");


            while (true) {
                Console.Write("> ");
                string command = Console.ReadLine();
                switch (command) {
                    case "0":
                        DbCommands.InitializeDB(forceReset: true);
                        break;
                    case "1":
                        DbCommands.ShowItems();
                        break;
                    case "2":
                        DbCommands.UpdateAttach();
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