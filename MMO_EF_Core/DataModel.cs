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
    //Configuration

    // A) Convention (관례)
    // - 각종 형식과 이름 등을 정해진 규칙에 맞게 만들면 EF Core에서 알아서 처리 (변수 이름 형식 등)
    // - 쉽고 빠르지만, 모든 경우를 처리할 수 없음
    // B) Data Annotation (데이터 주석)
    // - class/property 등에 attribute를 붙여 추가 정보 ([Table("")] 등)
    // C) Fluent Api (직접 정의)
    // - OnModeCreating에서 직접 설정을 정의
    // - 활용 범위가 넓음 (DbContext 상속 클래스에 속성 추가)

    // Shadow Property
    // Class에는 있지만, DB에는 없음 -> [NotMapped] .Ignore()
    // DB에는 있지만 Class에는 없음 -> Shadow Property
    // 생성 -> .Property<DateTime>("UpdateOn")
    // Read/Write -> .Property("RecoveredDate").CurrentValue

    // Backing Field (EF Core)
    // private field DB에 매핑하고, public getter로 가공해서 사용
    // Fluent Api

    public struct ItemOption {
        public int str;
        public int dex;
        public int hp;
    }

    [Table("Item")]
    public class Item {
        private string _jsonData;
        public string JsonData { 
            get { return _jsonData; } 
        }

        public void SetOPtion(ItemOption option) {
            _jsonData = JsonConvert.SerializeObject(option);
        }

        
        public ItemOption GetOption() {
            return JsonConvert.DeserializeObject<ItemOption>(_jsonData);
        }

        [NotMapped]
        public int Test { get; set; }
        public bool SoftDeleted { get; set; }
        public int ItemID { get; set; }
        public int TemplateID { get; set; }
        public DateTime CreateDate { get; set; }

        //[ForeignKey("Owner")]
        public int OwnerID { get; set; }
        //[InverseProperty("OwnedItem")] //[ForeignKey("OwnerID")]
        public Player Owner { get; set; }
        
        public int? CreatorID { get; set; }
        public Player Creator { get; set; }
    }

    [Table("Player")]
    public class Player { 
        public int PlayerID { get; set; } // => PK
        [Required]
        [MaxLength(20)]
        public string Name { get; set; }

        [InverseProperty("Owner")]
        public Item OwnedItem { get; set; }
        [InverseProperty("Creator")]
        public ICollection<Item> CreatedItems { get; set; }
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
