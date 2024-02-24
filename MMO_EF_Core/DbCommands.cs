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
        //State
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

                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

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
                    CreateDate = DateTime.Now,
                    Owner = rookiss
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
                Members = new List<Player>() { rookiss, faker, deft }
            };

            db.Items.AddRange(items);
            db.Guilds.Add(guild);

            db.SaveChanges();
        }


        public static void ShowItems() {
            using (AppDbContext db = new AppDbContext()) {
                /*foreach (var item in db.Items.Include(i => i.Owner).ToList()) {
                    if (item.Owner == null)
                    {
                        Console.WriteLine($"ItemID({item.ItemID}) TemplateID({item.TemplateID}) Owner(0)");
                    }
                    else {
                        Console.WriteLine($"ItemID({item.ItemID}) TemplateID({item.TemplateID}) OwnerID({item.Owner.PlayerID}) Owner({item.Owner.Name})");
                    }
                }*/

                foreach (var item in db.Items.Include(i => i.Owner).IgnoreQueryFilters().ToList())
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

        // 1) Tracking Entity 할당
        // 2) Remove 호출
        // 3) SaveChanges 호출
        public static void TestDelete() {
            ShowItems();
            Console.WriteLine("Select Delete ItemID");
            Console.Write("> ");
            int id = int.Parse(Console.ReadLine());

            using (AppDbContext db = new AppDbContext()) {
                Item item = db.Items.Find(id);
                //db.Items.Remove(item);
                item.SoftDeleted = true;
                db.SaveChanges();
            }


            Console.WriteLine("--- Test Delete Complete ---");
            ShowItems();
        }

    }
}
