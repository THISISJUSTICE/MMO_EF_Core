﻿using System;
using Microsoft.EntityFrameworkCore;

namespace MMO_EF_Core
{
    class Program
    {
        // Annotation (Attribute)
        [DbFunction()]
        public static double? GetAverageReviewScore(int itemID) {
            throw new NotImplementedException("SQL 함수");


        }
    
        static void Main(string[] args)
        {
            

            //CRUD (Create, Read, Update, Delete)
            Console.WriteLine("명령어 입력");
            Console.WriteLine("[0] Force Reset");
            Console.WriteLine("[1] Show Items");


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