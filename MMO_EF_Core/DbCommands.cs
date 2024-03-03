using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;

namespace MMO_EF_Core
{
    public class DbCommands
    {
        // State
        // 0) Detached (No Tracking: 추적되지 않는 상태, SaveChange를 해도 알 수 없음)
        // 1) Unchanged (DB에 있고, 수정 사항이 없음, SaveChanges를 해도 업데이트 없음)
        // 2) Deleted (DB에 있고, 삭제되어야 하는 상태, SaveChanges로 DB에 적용)
        // 3) Modified(DB에 있고, 클라이언트에서 수정된 상태, SaveChanges로 DB에 적용)
        // 4) Added(DB에 아직 없음, SaveChanges로 DB에 적용)

        public static void InitializeDB(bool forceReset = false)
        {
            using (AppDbContext db = new AppDbContext())
            {
                if (forceReset && !(db.GetService<IDatabaseCreator>() as RelationalDatabaseCreator).Exists()) return;

                //db.Database.Migrate();
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                string command =
                    @"  CREATE FUNCTION GetAverageReviewScore (@ItemID INT) RETURNs FLOAT
                        AS
                        BEGIN

                        DECLARE @result AS FLOAT

                        SELECT @result = AVG(CAST([Score] AS FLOAT))
                        FROM ItemReview AS r
                        WHERE @ItemID = r.ItemID

                        RETURN @result
                        END";
                db.Database.ExecuteSqlRaw(command);

                CreateTestData(db);
                Console.WriteLine("DB Initialized");
            }
        }

        
        public static void CreateTestData(AppDbContext db) {
            var rookiss = new Player() { Name = "Rookiss" };
            var faker = new Player() { Name = "Faker" };
            var deft = new Player() { Name = "Deft" };

            var items = new List<Item>() {
                new Item(){
                    TemplateID = 101,
                    Owner = rookiss
                },
                /*new Item(){
                    TemplateID = 102,
                    Owner = faker,
                },
                new Item(){
                    TemplateID = 103,
                    Owner = deft
                }*/
            };

            Guild guild = new Guild()
            {
                GuildName = "T1",
                Members = new List<Player>() { rookiss, faker, deft }
            };

            db.Items.AddRange(items);
            db.Guilds.Add(guild);

            Console.WriteLine("1) " + db.Entry(rookiss).State);

            db.SaveChanges();
        }


        public static void ShowItems() {
            using (AppDbContext db = new AppDbContext()) {
                foreach (var item in db.Items.Include(i => i.Owner).ToList())
                {
                    if (item.SoftDeleted)
                    {
                        Console.WriteLine($"DELETED - ItemID({item.ItemID}) TemplateID({item.TemplateID})");
                    }
                    else
                    {
                        if (item.Owner == null)
                        {
                            Console.WriteLine($"ItemID({item.ItemID}) TemplateID({item.TemplateID}) Owner(0)");
                        }
                        else
                        {
                            Console.WriteLine($"ItemID({item.ItemID}) TemplateID({item.TemplateID}) OwnerID({item.Owner.PlayerID}) Owner({item.Owner.Name})");
                        }
                    }
                }
            }
        }

        public static void ShowGuild()
        {
            using (AppDbContext db = new AppDbContext())
            {
                foreach (var guild in db.Guilds.Include(g => g.Members).ToList())
                {
                    
                    Console.WriteLine($"GuildID({guild.GuildID}) GuildName({guild.GuildName}) MemberCount({guild.Members.Count})");
                    
                }
            }
        }


        public static void Test() {
            using (AppDbContext db = new AppDbContext())
            {
                // FromSql
                {
                    string name = "Rookiss";
                    var list = db.Players
                        .FromSqlRaw("SELECT * FROM dbo.Player WHERE Name = {0}", name)
                        .Include(p => p.OwnedItem)
                        .ToList();

                    foreach (var p in list) {
                        Console.WriteLine($"{p.Name} {p.PlayerID}");
                    }

                    // String Interplation C#6.0
                    var list2 = db.Players
                        .FromSqlInterpolated($"SELECT * FROM dbo.Player WHERE Name = {name}")
                        .ToList();

                    foreach (var p in list2) {
                        Console.WriteLine($"{p.Name} {p.PlayerID}");
                    }
                }

                // ExcuteSqlCommand (Non_Query SQL)
                {
                    Player p = db.Players.Single(p => p.Name == "Faker");

                    string prevName = "Faker";
                    string afterName = "Faker_New";
                    db.Database.ExecuteSqlInterpolated($"Update dbo.Player SET Name={afterName} WHERE Name = {prevName}");

                    db.Entry(p).Reload();
                }
            }
        }

    }
}
