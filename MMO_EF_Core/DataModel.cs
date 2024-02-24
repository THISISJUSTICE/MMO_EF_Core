using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    #region Convention
    // Convention
    // 1) Entity Class 관련
    // - public 접근 한정자 + non-static
    // - property 중에서 public getter를 찾으면서 분석
    // - property 이름 = table column 이름
    // 2) 이름, 형식, 크기 관련
    // - .NET 형식 <-> SQL 형식 (int, bool)
    // - .NET 형식의 Nullable 여부를 따라감 (string은 nullable, int non-null, int?는 nullable)
    // 3) PK 관련
    // - id 혹은 <클래스 이름>id 정의된 property는 PK로 인정 (후자 권장)
    // - 복합키(Composite Key) Convention으로 처리 불가
    #endregion

    // Q1) DB column type, size, nullable
    // Nullbale     [Required]      .isRequired()
    // 문자열 길이 [MaxLength(20)]  .HasMaxLength(20)
    // 문자 형식                    .isUnicode(true)
    // - varchar nvarchar

    // Q2) PK
    // [Key][Column(Order=0)] [Key][Column(Order=1)]
    // .HasKey(x => new {x.Prop1, x.Prop2})

    // Q3) Index
    // 인덱스 추가                   .HasIndex(p => p.Prop1)
    // 복합 인덱스 추가              .HasIndex(p => new { p.Prop1, p.Prop2})
    // 인덱스 이름을 정해서 추가     .HasIndex(p => p.Prop1).HasName("Index_MyProp")
    // 유니크 인덱스 추가            .HasIndex(p => p.Prop1).IsUnique()

    // Q4) 테이블 이름
    // DbSet<T> property 이름 or class 이름
    // [Table("MyTable")]       .ToTable("MyTable")

    // Q5) 칼럼 이름
    // property 이름
    // [Column("MyCol")]        .HasColumnName("MyCol")

    // Q6) 코드 모델링에서는 사용하되, DB 모델링에선 제외 (property / class 모두 가능)
    // [NotMapped]      .Ignore()

    // Q7) Soft Delete
    // HasQueryFilter()

    // 활용
    // 1) Convention
    // 2) Validation과 관련된 부분들은 Data Annotation이 직관적, SaveChanges 호출
    // 3) 그 외에는 Fluent Api


    [Table("Item")]
    public class Item { 
        [NotMapped]
        public int Test { get; set; }
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
        [Required]
        [MaxLength(20)]
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
