using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace MMO_EF_Core
{
    // DbContext 심화 (최적화 등)
    // 1) ChangeTracker
    // - Tracking State 관련
    // 2) Database
    // - Transaction
    // - DB Creation/Migration
    // - Raw SQL
    // 3) Model
    // - DB 모델링 관련

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
