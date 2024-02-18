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

        //State
        // 0) Detached (No Tracking: 추적되지 않는 상태, SaveChange를 해도 알 수 없음)
        // 1) Unchanged (DB에 있고, 수정 사항이 없음, SaveChanges를 해도 업데이트 없음)
        // 2) Deleted (DB에 있고, 삭제되어야 하는 상태, SaveChanges로 DB에 적용)
        // 3) Modified(DB에 있고, 클라이언트에서 수정된 상태, SaveChanges로 DB에 적용)
        // 4) Added(DB에 아직 없음, SaveChanges로 DB에 적용)

        // SaveChanges를 호출
        // 1) 추가된 객체들의 상태가 UnChanged로 바뀜
        // 2) SQL Identity로 PK를 관리
        // - 데이터 추가 후 ID를 받아와 객체의 ID Property를 할당
        // - Relationship 참고해서, FK 세팅 및 객체 참조 연결

        // 이미 존재하는 사용자를 연동
        // 1) Tracked Instance (추적되고 있는 객체)를 얻어와서 데이터 연결
        public static void CreateTestData(AppDbContext db) {
            var rookiss = new Player() { Name = "Rookiss" };
            var faker = new Player() { Name = "Faker" };
            var deft = new Player() { Name = "Deft" };

            Console.WriteLine("State1: " + db.Entry(rookiss).State);

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
            
            Console.WriteLine("State2: " + db.Entry(rookiss).State);
            Console.WriteLine(rookiss.PlayerID);
            db.SaveChanges();
            
            Console.WriteLine("State3: " + db.Entry(rookiss).State);
            Console.WriteLine(rookiss.PlayerID);

            {
                var owner = db.Players.Where(p => p.Name == "Rookiss").First();

                Item item = new Item()
                {
                    TemplateID = 300,
                    CreateDate = DateTime.Now,
                    Owner = owner
                };
                db.Items.Add(item);
                db.SaveChanges();
            }
        }


    }
}
