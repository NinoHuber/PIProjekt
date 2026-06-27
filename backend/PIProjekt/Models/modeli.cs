using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PIProjekt.Models {
    public class PartnerCategory {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty; // e.g., 

        // Navigation property
        public ICollection<Partner> Partners { get; set; } = new List<Partner>();
    }

    public class Partner {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PartnerCategoryId { get; set; }

        [Required]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string Address { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Required]
        [Column(TypeName = "decimal(5,2)")]
        public decimal CommissionPercentage { get; set; }

        
        public bool? HasHall { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }

        // Navigation properties
        [ForeignKey(nameof(PartnerCategoryId))]
        public PartnerCategory? Category { get; set; }

        public ICollection<WeddingPartner> WeddingPartners { get; set; } = new List<WeddingPartner>();
    }

    public class Wedding {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime WeddingDate { get; set; }

        // Navigation property for the join table
        public ICollection<WeddingPartner> WeddingPartners { get; set; } = new List<WeddingPartner>();
    }

    public class WeddingPartner {
        public int WeddingId { get; set; }
        public Wedding? Wedding { get; set; }

        public int PartnerId { get; set; }
        public Partner? Partner { get; set; }
    }
}