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
    // Backing Field -> private field를 DB에 Mapping
    // Navigation Property에서도 사용 가능

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

        public double? AverageScore { get; set; }
        private readonly List<ItemReview> _reviews = new List<ItemReview>();
        public IEnumerable<ItemReview> Reviews { get { return _reviews.ToList(); } }

        public void AddReview(ItemReview review) {
            _reviews.Add(review);
            AverageScore = _reviews.Average(r => r.Score);
        }

        public void RemoveReview(ItemReview review) {
            _reviews.Remove(review);
            AverageScore = _reviews.Any() ? _reviews.Average(r => r.Score) : (double?)null;
        }
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
