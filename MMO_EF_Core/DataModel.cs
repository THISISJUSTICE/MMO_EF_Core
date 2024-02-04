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
        public int ItemID { get; set; }
        public int TemplateID { get; set; }
        public DateTime CreateDate { get; set; }

        //다른 클래스를 참조(FK) (Navigational Property)
        public Player Owner { get; set; } //데이터 베이스에서는 없는 참조 값
    }

    //클래스 이름 => 테이블 이름
    public class Player { 
        public int PlayerID { get; set; } // => PK
        public string Name { get; set; }

        //list보다 속도가 빠름
        public ICollection<Item> Items { get; set; } 
    }

}
