using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace MMO_EF_Core
{
    // Migration

    // EF Core DbContext <-> DB 상태에 대해 동의가 있어야 함

    // 1) Code-First
    // - Entity Class / DbContext 기준
    // - 항상 최신 상태로 DB를 업데이트 하는 것이 아님

    // *** Migration step ***
    // A) Migration 생성
    // B) Migration 적용

    // A) Add-Migration [Name]
    // - 1) DbContext 탐색 후 분석 -> DB 모델링(최신)
    // - 2) ModleSnapshot.cs를 이용해서 가장 마지막 Migration 상태의 DB 모델링 (가장 마지막 상태)
    // - 3) 1-2 비교 결과 도출
    // -- a) ModeSnapshpt -> 최신 DB 모델링
    // -- b) Migrate Designer.cs와 Migration.cs -> Migration 관련된 세부 정보
    // 수동으로 Up/Down 추가 및 수정 가능

    // B) Migration 적용
    // - 1) SQL change script
    // -- Script-Migration [From] [To] [Options]
    // - 2) Database.Migrate 호출
    // - 3) Command Line 방식
    // - Update_Database [options]

    // 특정 Migration으로 Sync (Update-Database [Name])
    // 마지막 Migration 삭제 (Remove-Migration)

    // 2) Database-First
    // 3) SQL-First

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
