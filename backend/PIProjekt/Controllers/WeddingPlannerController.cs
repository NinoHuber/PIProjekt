using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PIProjekt.Data;
using PIProjekt.Models;
using PIProjekt.Dtos;

namespace PIProjekt.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class WeddingPlannerController : ControllerBase {
        private readonly AppDbContext _context;

        public WeddingPlannerController(AppDbContext context) {
            _context = context;
        }

        [HttpGet("vendors")]
        public async Task<ActionResult<PagedResultDto<PartnerResponseDto>>> GetVendors(
            [FromQuery] string categoryName,
            [FromQuery] string searchTerm = "",
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 3) {
            if (string.IsNullOrWhiteSpace(categoryName)) {
                return BadRequest("Category name parameter is required.");
            }

            var query = _context.Partners
                .Include(p => p.Category)
                .Where(p => p.Category!.Name.ToLower() == categoryName.ToLower());

            if (!string.IsNullOrWhiteSpace(searchTerm)) {
                query = query.Where(p => p.Name.ToLower().Contains(searchTerm.ToLower()));
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var partners = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new PartnerResponseDto {
                    Id = p.Id,
                    Name = p.Name,
                    Phone = p.Phone,
                    Email = p.Email,
                    Address = p.Address,
                    Price = p.Price,
                    CommissionPercentage = p.CommissionPercentage,
                    HasHall = p.HasHall,
                    StartTime = p.StartTime.HasValue ? p.StartTime.Value.ToString(@"hh\:mm") : null,
                    EndTime = p.EndTime.HasValue ? p.EndTime.Value.ToString(@"hh\:mm") : null
                })
                .ToListAsync();

            return Ok(new PagedResultDto<PartnerResponseDto> {
                Items = partners,
                TotalCount = totalCount,
                TotalPages = totalPages == 0 ? 1 : totalPages,
                CurrentPage = page
            });
        }

        [HttpPost("weddings")]
        public async Task<IActionResult> CreateWedding([FromBody] WeddingSubmissionDto submission) {
            if (submission.WeddingDate < DateTime.Today) {
                return BadRequest("The wedding date cannot be set in the past.");
            }

            var targetPartnerIds = new List<int>();
            var utcDate = DateTime.SpecifyKind(submission.WeddingDate, DateTimeKind.Utc);

            if (submission.RestaurantId.HasValue) targetPartnerIds.Add(submission.RestaurantId.Value);
            if (submission.PastryId.HasValue) targetPartnerIds.Add(submission.PastryId.Value);
            if (submission.FloristId.HasValue) targetPartnerIds.Add(submission.FloristId.Value);
            if (submission.WeddingHallId.HasValue) targetPartnerIds.Add(submission.WeddingHallId.Value);
            if (submission.BandIds.Any()) targetPartnerIds.AddRange(submission.BandIds);

            var verifiedPartners = await _context.Partners
                .Where(p => targetPartnerIds.Contains(p.Id))
                .ToListAsync();

            if (verifiedPartners.Count != targetPartnerIds.Distinct().Count()) {
                return BadRequest("One or more of the selected vendor identifiers are invalid.");
            }

            var selectedBands = verifiedPartners.Where(p => submission.BandIds.Contains(p.Id)).ToList();
            for (int i = 0; i < selectedBands.Count; i++) {
                for (int j = i + 1; j < selectedBands.Count; j++) {
                    var bandA = selectedBands[i];
                    var bandB = selectedBands[j];

                    if (bandA.StartTime.HasValue && bandA.EndTime.HasValue &&
                        bandB.StartTime.HasValue && bandB.EndTime.HasValue) {
                        if (bandA.StartTime < bandB.EndTime && bandB.StartTime < bandA.EndTime) {
                            return BadRequest($"Schedule conflict detected between selected bands: '{bandA.Name}' and '{bandB.Name}'.");
                        }
                    }
                }
            }

            var newWedding = new Wedding {
                WeddingDate = utcDate
            };

            _context.Weddings.Add(newWedding);
            await _context.SaveChangesAsync();

            var links = verifiedPartners.Select(partner => new WeddingPartner {
                WeddingId = newWedding.Id,
                PartnerId = partner.Id
            }).ToList();

            _context.WeddingPartners.AddRange(links);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(CreateWedding), new { id = newWedding.Id }, new { message = "Wedding setup stored completely." });
        }
    }
}