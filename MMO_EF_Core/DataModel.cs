using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace MMO_EF_Core
{
    // State 조작

    // ex) Entry().State = EntityState.Added
    // ex) Entity().Property("").IsModified = true

    // - Track Graph
    // Relationship이 있는 Untracked Entity의 State 조작

    // - Change Tracker
    // 상태 정보의 변화를 감지할 때 유용
    // ex) Player의 Name이 바뀔 때 로그
    // - 1) SaveChanges를 override
    // - 2) ChangeTracker.Entries를 이용해서 정보 추출 및 사용

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
