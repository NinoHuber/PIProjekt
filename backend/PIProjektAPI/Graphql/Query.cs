using HotChocolate;
using HotChocolate.Data;
using Microsoft.EntityFrameworkCore;
using PIProjekt.Data;
using PIProjekt.Models;

namespace PIProjekt.GraphQL {
    public class Query {
        [UseFiltering]
        [UseSorting]  
        public IQueryable<Partner> GetPartners([Service] AppDbContext context) {
            return context.Partners.Include(p => p.Category);
        }

        public async Task<List<PartnerCategory>> GetCategories([Service] AppDbContext context) {
            return await context.PartnerCategories.ToListAsync();
        }

        public async Task<Wedding?> GetWeddingById(int id, [Service] AppDbContext context) {
            return await context.Weddings
                .Include(w => w.WeddingPartners)
                .ThenInclude(wp => wp.Partner)
                .FirstOrDefaultAsync(w => w.Id == id);
        }
    }
}