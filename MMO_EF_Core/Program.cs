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
            Console.WriteLine("[1] Eager Loading"); //즉시 로딩
            Console.WriteLine("[2] Explicit Loading"); // 명시적
            Console.WriteLine("[3] Select Loading"); // 선택

            while (true) {
                Console.Write("> ");
                string command = Console.ReadLine();
                switch (command) {
                    case "0":
                        DbCommands.InitializeDB(forceReset: true);
                        break;
                    case "1":
                        DbCommands.EagerLoading();
                        break;
                    case "2":
                        DbCommands.ExplicitLoading();
                        break;
                    case "3":
                        DbCommands.SelectLoading();
                        break;
                    default:
                        break;
                }
            }
        }
    }
}