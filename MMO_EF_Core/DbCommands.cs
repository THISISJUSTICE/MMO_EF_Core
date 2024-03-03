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

        // State 확인 방법
        // - Entry().State
        // - Entry().Property().IsModified
        // - Entry().Navigation().IsModified

        // - 1) Add/AddRange 사용할 때의 상태 변화
        // -- NotTracking 상태라면 Added
        // -- Tracking 상태라면, FK 설정이 필요한지에 따라 Modified / 기존 상태 유지
        // - 2) Remove/RemoveRange 사용할 때의 상태 변화
        // -- (DB에 의해 생성된 Key) && (C# 기본값 아님) -> 필요에 따라 Unchanged / Modified / Deleted
        // -- (DB에 의해 생성된 Key 없음) || (C# 기본값) -> Added

        // - 3) Update/UpdateRange
        // -- Tracking Entity 호출 -> Property 수정 -> SaveChanges
        // -- Untracked Entity 전체 업데이트 (Disconnected State)

        // EF Core Update
        // 1) Update 호출
        // 2) Entity State = Modified로 변경
        // 3) 모든 Non-Relational Property의 IsModified = true로 변경
        // -- (DB에 의해 생성된 Key) && (C# 기본값 아님) -> 필요에 따라 Unchanged / Modified / Deleted
        // -- (DB에 의해 생성된 Key 없음) || (C# 기본값) -> Added

        // - 4) Attach
        // -- UnTracked Entity를 Tracked Entity로 변경
        // -- (DB에 의해 생성된 Key) && (C# 기본값 아님) -> Unchanged
        // -- (DB에 의해 생성된 Key 없음) || (C# 기본값) -> Added

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

            // Add Test
            {
                Item item = new Item()
                {
                    TemplateID = 500,
                    Owner = rookiss
                };
                db.Items.Add(item);
                // 아이템 추가 -> 간접적으로 Player도 영향
                // Player는 Tracking 상태이고, FK 설정은 필요 없음
                Console.WriteLine("2) " + db.Entry(rookiss).State); //Unchanged
            }

            // Delete Teast
            {
                Player p = db.Players.First();
                // 아직 DB 키 없음
                p.Guild = new Guild() { GuildName = "곧 삭제될 길드" };

                // DB 키 있음
                p.OwnedItem = items[0];

                db.Players.Remove(p);

                Console.WriteLine("3) " + db.Entry(p).State); // Deleted
                Console.WriteLine("4) " + db.Entry(p.Guild).State); // Added
                Console.WriteLine("5) " + db.Entry(p.OwnedItem).State); // Deleted (Nullable이 아니기 때문)
            }

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


        public static void UpdateAttach() {
            using (AppDbContext db = new AppDbContext())
            {
                // Update Test
                {
                    Player p = new Player();
                    p.PlayerID = 2;
                    p.Name = "FakerSenpai";
                    // DB 키 없음, Non-trakced
                    p.Guild = new Guild() { GuildName = "Update Guild" };
                    Console.WriteLine("6) " + db.Entry(p.Guild).State); // Detached
                    db.Players.Update(p);
                    Console.WriteLine("7) " + db.Entry(p.Guild).State); // Added
                }

                // Attach Test
                {
                    Player p = new Player();

                    // temp
                    p.PlayerID = 3;
                    /*p.Name = "Drift";*/

                    p.Guild = new Guild() { GuildName = "Attach Guild" };

                    Console.WriteLine("8) " + db.Entry(p.Guild).State); // Detached
                    db.Players.Update(p);
                    p.Name = "Drift";
                    Console.WriteLine("9) " + db.Entry(p.Guild).State); // Added
                }

                db.SaveChanges();
            }
        }

    }
}
