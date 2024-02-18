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

        // Update 3단계
        // 1) Tracked Entity를 얻어온다
        // 2) Entity 클래스의 property를 변경(set)
        // 3) SaveChanges 호출

        // (Connected VS Disconnected) Update
        // Disconnected: Update 단계가 한 번에 일어나지 않고 끊기는 경우
        // (REST API 등)
        // 처리법
        // 1) Reload 방식
        // 2) Full Update 방식: 모든 정보를 다 송수신하여 Entity를 다시 만들고 Update

        public static void ShowGuilds() {
            using (AppDbContext db = new AppDbContext()) {
                foreach (var guild in db.Guilds.MapGuildToDto()) {
                    Console.WriteLine($"GuildID({guild.GuildID}) GuildName({guild.Name}) MemberCount({guild.MemberCount})");
                }
            }
        }

        public static void UpdateByReload() {
            ShowGuilds();
            
            Console.WriteLine("Input Guild ID");
            Console.Write("> ");
            int id = int.Parse(Console.ReadLine());
            Console.WriteLine("Input Guild Name");
            Console.Write("> ");
            string name = Console.ReadLine();

            using (AppDbContext db = new AppDbContext()) {
                Guild guild = db.Find<Guild>(id);
                guild.GuildName = name;
                db.SaveChanges();
            }

            Console.WriteLine("--- Update Complete ---");
            ShowGuilds();
        }

        public static string MakeUpdateJsonStr() {
            var jsonStr = "{\"GuildID\":1, \"GuildName\": \"Hello\", \"Memebers\":null}";
            return jsonStr;
        }

        // 장점: DB에 다시 Road할 필요 없이 바로 Update
        // 단점: 모든 정보 필요, 보안 문제
        public static void UpdateByFull()
        {
            ShowGuilds();

            string jsonStr = MakeUpdateJsonStr();
            Guild guild = JsonConvert.DeserializeObject<Guild>(jsonStr);

            using (AppDbContext db = new AppDbContext())
            {
                db.Guilds.Update(guild);
                db.SaveChanges();
            }

            Console.WriteLine("--- Update Complete ---");
            ShowGuilds();
        }

    }
}
