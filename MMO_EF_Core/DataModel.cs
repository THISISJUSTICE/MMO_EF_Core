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
    // Entity Class All Read/Write -> 부담(Select Loading, DTO)

    // 1) Owned Type
    // - 일반 class를 Entity class에 추가
    // a) 동일한 테이블 추가
    // - .OwnsOne()
    // - Relationship이 아닌 Onwership의 개념이기 때문에 .Include()
    // b) 다른 테이블에 추가
    // - .OwnsOne().ToTable()

    // 2) Table Per Hierachy(TPH)
    // - 상속 관계의 여러 class <-> 하나의 테이블에 Mapping
    // a) Convention
    // - class 상속 -> DbSet 추가
    // -- Discriminator
    // b) Fluent Api
    // - .HasDiscriminator().HasValue()

    // 3) Table Splitting
    // - 다수의 Entity class <-> 하나의 테이블에 Mapping

    public enum ItemType { 
        NormalItem,
        EventItem
    }

    public class ItemOption { 
        public int Str { get; set; }
        public int Dex { get; set; }
        public int Hp { get; set; }
    }

    public class ItemDetail { 
        public int ItemDetailID { get; set; }
        public string Description { get; set; }
    }

    [Table("Item")]
    public class Item 
    {
        public ItemType Type { get; set; }
        public bool SoftDeleted { get; set; }
        public ItemOption Option { get; set; }
        public ItemDetail Detail { get; set; }
        public int ItemID { get; set; }
        public int TemplateID { get; set; }
        public DateTime CreateDate { get; set; }

        //[ForeignKey("Owner")]
        public int OwnerID { get; set; }
        //[InverseProperty("OwnedItem")] //[ForeignKey("OwnerID")]
        public Player Owner { get; set; }
    }

    public class EventItem : Item { 
        public DateTime DestroyDate { get; set; }
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
