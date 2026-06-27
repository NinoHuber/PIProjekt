using Bogus;
using PIProjekt.Data;
using PIProjekt.Models;

namespace PIProjekt.Utilities {
    public static class DatabaseSeeder {
        public static void Seed(AppDbContext context) {
            if (context.PartnerCategories.Any() || context.Partners.Any()) {
                return;
            }

            var categories = new List<PartnerCategory>
            {
                new() { Name = "Restaurant" },
                new() { Name = "Pastry" },
                new() { Name = "Florist" },
                new() { Name = "Wedding Hall" },
                new() { Name = "Band" }
            };

            context.PartnerCategories.AddRange(categories);
            context.SaveChanges();

            var partnerIdCounter = 1;
            var partnerFaker = new Faker<Partner>()
                .RuleFor(p => p.Id, f => partnerIdCounter++)
                .RuleFor(p => p.Name, f => f.Company.CompanyName())
                .RuleFor(p => p.Phone, f => f.Phone.PhoneNumber("###-###-####"))
                .RuleFor(p => p.Email, (f, p) => f.Internet.Email(p.Name.Replace(" ", "")))
                .RuleFor(p => p.Address, f => $"{f.Address.StreetAddress()}, {f.Address.City()}")
                .RuleFor(p => p.Price, f => Math.Round(f.Random.Decimal(400, 7000), 2))
                .RuleFor(p => p.CommissionPercentage, f => Math.Round(f.Random.Decimal(5, 15), 2));

            var allPartners = new List<Partner>();

            foreach (var category in categories) {
                for (int i = 0; i < 30; i++) {
                    var partner = partnerFaker.Generate();
                    partner.PartnerCategoryId = category.Id;

                    if (category.Name == "Restaurant") {
                        partner.HasHall = Faker.GlobalUniqueIndex % 2 == 0;
                    } else if (category.Name == "Band") {
                        // Generate random but logical band slots (e.g., between 16:00 and 23:00)
                        var startHour = new Random().Next(16, 21);
                        partner.StartTime = new TimeSpan(startHour, 0, 0);
                        partner.EndTime = partner.StartTime.Value.Add(new TimeSpan(2, 30, 0));
                    }

                    allPartners.Add(partner);
                }
            }

            context.Partners.AddRange(allPartners);
            context.SaveChanges();

            var weddingIdCounter = 1;
            var weddingFaker = new Faker<Wedding>()
                .RuleFor(w => w.Id, f => weddingIdCounter++)
                .RuleFor(w => w.WeddingDate, f => f.Date.Between(DateTime.Today.AddDays(30), DateTime.Today.AddDays(365)));

            var sampleWeddings = weddingFaker.Generate(5);
            context.Weddings.AddRange(sampleWeddings);
            context.SaveChanges();

            var restaurants = allPartners.Where(p => p.PartnerCategoryId == categories[0].Id).ToList();
            var bands = allPartners.Where(p => p.PartnerCategoryId == categories[4].Id).ToList();

            if (restaurants.Any() && bands.Any()) {
                context.WeddingPartners.AddRange(
                    new WeddingPartner { WeddingId = sampleWeddings[0].Id, PartnerId = restaurants[0].Id },
                    new WeddingPartner { WeddingId = sampleWeddings[0].Id, PartnerId = bands[0].Id }
                );
                context.SaveChanges();
            }
        }
    }
}