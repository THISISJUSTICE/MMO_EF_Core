using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace MMO_EF_Core
{
    // SQL 직접 호출
    // ex) LINQ로 처리할 수 없는 것 -> Stored Procedure 호출
    // ex) 성능 최적화

    // 1) FromSql -> FromSqlRaw / FromSqlInterpolated
    // - EF Core 쿼리에 Raw SQL 추가

    // 2) ExcuteSqlCommand -> ExecuteSqlRaw / ExecuteSqlInterplated
    // - Non-Query (SELECT가 아님) SQL

    // 3) Reload
    // - 1) Tracked Entity가 이미 있는 상태
    // - 2) ExcuteSqlCommand에 의해 DB 정보가 변경됐을 시
    // - 최신 상태로 맞춤

    [Table("Item")]
    public class Item 
    {
        public bool SoftDeleted { get; set; }
        public int ItemID { get; set; }
        public int TemplateID { get; set; }
        public DateTime CreateDate { get; private set; }
        
        public int ItemGrade { get; set; }

        public int OwnerID { get; set; }
        public Player Owner { get; set; }
    }

    public interface ILogEntity { 
        DateTime CreateTime { get; }
        void SetCreateTime();
    }

    [Table("Player")]
    public class Player : ILogEntity
    { 
        public int PlayerID { get; set; } // => PK
        [Required]
        [MaxLength(20)]
        public string Name { get; set; }

        [InverseProperty("Owner")]
        public Item OwnedItem { get; set; }
        public Guild Guild { get; set; }

        public DateTime CreateTime { get; private set; }
        public void SetCreateTime() {
            CreateTime = DateTime.Now;
        }
    }

    [Table("Guild")]
    public class Guild { 
        public int GuildID { get; set; }
        public string GuildName { get; set; }
        public ICollection<Player> Members { get; set; }
    }

    // DTO(Data Transfer Object)
    public class GuildDto {
        public int GuildID { get; set; }
        //Alternate Key
        public string Name { get; set; }
        public int MemberCount { get; set; }

    }

}
