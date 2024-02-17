using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MMO_EF_Core
{
    //EF Core 작동 스텝
    //1) DbContext 만들 때
    //2) DbSet<T>을 찾는다.
    //3) 모델링 class를 분석해서, 칼럼을 찾는다.
    //4) 모델링 class에서 참조하는 다른 class가 있으면, 그것도 분석한다.
    //5) OnModelCreating 함수 호출(추가 설정 override)
    //6) 데이터베이스의 전체 모델링 구조를 내부 메모리에 들고 있음
    public class AppDbContext : DbContext
    {
        //Item이라는 DB 테이블이 있는데, 세부적인 칼럼/키 정보는 Items 클래스를 참고
        public DbSet<Item> Items { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Guild> Guilds { get; set; }
        //어떤 DB를 어떻게 연결하는 등의 각종 설정
        public const string ConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=EFCoreDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        protected override void OnConfiguring(DbContextOptionsBuilder options) {
            options.UseSqlServer(ConnectionString);
        }

    }
}
