using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMO_EF_Core
{
    //DB 관계 모델링
    //1:1
    //1:다
    //다:다

    [Table("Item")]
    public class Item { 
        public bool SoftDeleted { get; set; }
        public int ItemID { get; set; }
        public int TemplateID { get; set; }
        public DateTime CreateDate { get; set; }

        //다른 클래스를 참조(FK) (Navigational Property)
        //public int OwnerID { get; set; }
        public int? OwnerID { get; set; }
        public Player Owner { get; set; } //데이터 베이스에서는 없는 참조 값
    }

    [Table("Player")]
    public class Player { 
        public int PlayerID { get; set; } // => PK
        public string Name { get; set; }

        public Item Item { get; set; } 
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
        public string Name { get; set; }
        public int MemberCount { get; set; }

    }

}
