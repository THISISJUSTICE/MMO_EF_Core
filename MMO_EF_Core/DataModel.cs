using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Newtonsoft.Json;

namespace MMO_EF_Core
{
    // 초기값(Default Value)

    // 기본값 설정 방법
    // 1) Auto-Property Initializer (C# 6.0)
    // - Entity 차원이ㅡ 초기값 -> SaveChanges로 적용
    // 2) Fluent Api
    // - DB Table DEFAULT를 적용 (고정값 적용)
    // 3) SQL Fragment (새로운 값이 추가되는 시점에 DB에서 실행되는 코드)
    // - .HasDefaultValueSql
    // 4) Value Generator (EF Core에서 실행)
    // - 일종의 Generator 규칙

    // 1) Entity Class 자체의 초기값인지
    // 2) DB Table 차원에서 초기값인지
    // - EF <-> DB외에 다른 경로로 DB를 사용하면 차이가 날 수 있음

    [Table("Item")]
    public class Item 
    {
        public bool SoftDeleted { get; set; }
        public int ItemID { get; set; }
        public int TemplateID { get; set; }
        public DateTime CreateDate { get; private set; }

        public int OwnerID { get; set; }
        public Player Owner { get; set; }
    }

    public class PlayerNameGenerator : ValueGenerator<string>
    {
        public override bool GeneratesTemporaryValues => false;

        public override string Next(EntityEntry entry)
        {
            string name = $"Player_{DateTime.Now.ToString("yyyyMMdd")}";
            return name;
        }
    }


    [Table("Player")]
    public class Player { 
        public int PlayerID { get; set; } // => PK
        [Required]
        [MaxLength(20)]
        public string Name { get; set; }

        [InverseProperty("Owner")]
        public Item OwnedItem { get; set; }
        public Guild Guild { get; set; }
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
