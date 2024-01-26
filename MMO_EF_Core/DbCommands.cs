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

        public static void CreateTestData(AppDbContext db) {
            var player = new Player()
            {
                Name = "Rookis"
            };

            var items = new List<Item>() {
                new Item(){
                    TemplateID = 101,
                    CreateDate = DateTime.Now,
                    Owner = player
                },
                new Item(){
                    TemplateID = 102,
                    CreateDate = DateTime.Now,
                    Owner = player
                },
                new Item(){
                    TemplateID = 103,
                    CreateDate = DateTime.Now,
                    Owner = new Player() {Name = "Faker"}
                }
            };

            db.Items.AddRange(items);
            db.SaveChanges();
        }

        public static void ReadAll() {
            using (var db = new AppDbContext()) {
                // AsNoTracking : ReadOnly << Tracking Snapshot이라는 데이터 변경 탐지 기능 때문
                // Include : Eager Loading (즉시 로딩) << 나중에 알아볼 것
                foreach (Item item in db.Items.AsNoTracking().Include(i => i.Owner)) {
                    Console.WriteLine($"TemplateID({item.TemplateID}), Owner({item.Owner.Name}), Created({item.CreateDate})");
                }
            }
        }

        //특정 플레이어가 소지한 아이템들의 CreateDate를 수정
        public static void UpdateDate() {
            Console.WriteLine("Input Player Name");
            Console.Write("> ");
            string name = Console.ReadLine();

            using (var db = new AppDbContext()) {
                var items = db.Items.Include(i => i.Owner)
                    .Where(i => i.Owner.Name == name);

                foreach (Item item in items) {
                    item.CreateDate = DateTime.Now;
                }
                db.SaveChanges();
            }

            ReadAll();
        }

        public static void DeleteItem() {
            Console.WriteLine("Input Player Name");
            Console.Write("> ");
            string name = Console.ReadLine();

            using (var db = new AppDbContext())
            {
                var items = db.Items.Include(i => i.Owner)
                    .Where(i => i.Owner.Name == name);

                db.Items.RemoveRange(items);
                db.SaveChanges();
            }

            ReadAll();
        }

    }
}
