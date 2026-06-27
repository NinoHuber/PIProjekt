namespace PIProjekt.Dtos {
    public class WeddingSubmissionDto {
        public DateTime WeddingDate { get; set; }
        public int? RestaurantId { get; set; }
        public int? PastryId { get; set; }
        public int? FloristId { get; set; }
        public int? WeddingHallId { get; set; }
        public List<int> BandIds { get; set; } = new();
    }

    public class PagedResultDto<T> {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
    }

    public class PartnerResponseDto {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal CommissionPercentage { get; set; }
        public bool? HasHall { get; set; }
        public string? StartTime { get; set; }
        public string? EndTime { get; set; }
    }
}