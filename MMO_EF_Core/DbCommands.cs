using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace MMO_EF_Core
{
    public class DbCommands
    {
        //초기화 시간이 걸림
        public static void InitializeDB(bool forceReset = false)
        {
            using (AppDbContext db = new AppDbContext())
            {
                if (forceReset && !(db.GetService<IDatabaseCreator>() as RelationalDatabaseCreator).Exists()) return;

                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                CreateTestData(db);
                Console.WriteLine("DB Initialized");
            }
        }

        //장점: DB 접근 한 번에 필요한 모든 정보를 로딩
        //단점: 불필요할 수도 있는 정보들까지 로딩
        public static void CreateTestData(AppDbContext db) {
            var rookis = new Player() { Name = "Rookis" };
            var faker = new Player() { Name = "Faker" };
            var deft = new Player() { Name = "Deft" };


            var items = new List<Item>() {
                new Item(){
                    TemplateID = 101,
                    CreateDate = DateTime.Now,
                    Owner = rookis
                },
                new Item(){
                    TemplateID = 102,
                    CreateDate = DateTime.Now,
                    Owner = faker
                },
                new Item(){
                    TemplateID = 103,
                    CreateDate = DateTime.Now,
                    Owner = deft
                }
            };

            Guild guild = new Guild()
            {
                GuildName = "T1",
                Members = new List<Player>() { rookis, faker, deft }
            };

            db.Items.AddRange(items);
            db.Guilds.Add(guild);
            db.SaveChanges();
        }

        //(1+2) 특정 길드에 있는 길드원들이 소지한 모든 아이템들을 보기
        public static void EagerLoading() {
            Console.WriteLine("길드 이름을 입력하시오");
            Console.Write("> ");
            string name = Console.ReadLine();
            
            using (var db = new AppDbContext()) {
                Guild guild = db.Guilds.AsNoTracking()
                    .Where(g => g.GuildName == name)
                    .Include(g => g.Members)
                        .ThenInclude(p => p.Item)
                    .First();

                foreach (Player player in guild.Members) {
                    Console.WriteLine($"TemplateID({player.Item.TemplateID}) Owner({player.Name})");
                }
            }
        }

        //장점: 필요한 시점에 필요한 데이터만 로딩
        //단점: DB 접근 비용이 많음
        public static void ExplicitLoading()
        {
            Console.WriteLine("길드 이름을 입력하시오");
            Console.Write("> ");
            string name = Console.ReadLine();

            using (var db = new AppDbContext())
            {
                Guild guild = db.Guilds
                    .Where(g => g.GuildName == name)
                    .First();

                //명시적
                db.Entry(guild).Collection(g => g.Members).Load();

                foreach (Player player in guild.Members) {
                    db.Entry(player).Reference(p => p.Item).Load();
                }

                foreach (Player player in guild.Members)
                {
                    Console.WriteLine($"TemplateID({player.Item.TemplateID}) Owner({player.Name})");
                }
            }
        }

        // 3) 특정 길드에 있는 길드원들의 수
        //장점: 필요한 정보만 추출
        //단점: 일일히 select 안에 만들어줘야 함
        public static void SelectLoading()
        {
            Console.WriteLine("길드 이름을 입력하시오");
            Console.Write("> ");
            string name = Console.ReadLine();

            using (var db = new AppDbContext())
            {
                var info = db.Guilds.
                    Where(g => g.GuildName == name)
                    .Select(g => new
                    {
                        Name = g.GuildName,
                        MemeberCount = g.Members.Count
                    })
                    .First();

                Console.WriteLine($"GuildName({info.Name}) MemeberCount({info.MemeberCount})");
            }
        }


    }
}
