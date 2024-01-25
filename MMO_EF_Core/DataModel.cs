using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMO_EF_Core
{
    [Table("Item")]
    public class Item { 
        public int ItemID { get; set; }
        public int TemplateID { get; set; }
        public DateTime CreateDate { get; set; }

        //다른 클래스를 참조(FK) (Navigational Property)
        public int OwnerID { get; set; }
        public Player Owner { get; set; }
    }

    //클래스 이름 => 테이블 이름
    public class Player { 
        public int PlayerID { get; set; } // => PK
        public string Name { get; set; }
    }

}
