using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MMO_EF_Core
{
    // User Defined Function(UDF)
    // 직접 만든 SQL을 호출하는 기능
    // - 연산을 DB쪽에서 담당
    // - EF Core 쿼리가 비효율적인 경우

    // 1) Configuration
    // - static 함수를 만들고 EF COre 등록
    // 2) Database Setup

    public class ItemReview { 
        public int ItemReviewID { get; set; }
        public int Score { get; set; } // 0~5점
    }

    [Table("Item")]
    public class Item 
    {
        public bool SoftDeleted { get; set; }
        public int ItemID { get; set; }
        public int TemplateID { get; set; }
        public DateTime CreateDate { get; set; }

        public int OwnerID { get; set; }
        public Player Owner { get; set; }

        public ICollection<ItemReview> Reviews { get; set; }
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
