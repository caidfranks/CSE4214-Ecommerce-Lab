using GameVaultWeb.Models;

namespace GameVaultWeb.Services
{
    public class ProductService
    {
        // TODO: Replace with actual Firebase database calls
        public async Task<List<Product>> GetAllProductsAsync()
        {
            await Task.Delay(500); // Simulate network delay

            return new List<Product>
            {
                // Action Games
                new Product
                {
                    Id = 1,
                    Name = "Dark Souls III",
                    Price = 39.99m,
                    VendorName = "FromSoftware Store",
                    ThumbnailUrl = "https://via.placeholder.com/200x300/1a1a1a/ffffff?text=Dark+Souls+III",
                    Category = "Action",
                    Stock = 10,
                    Description = "Dark Souls III is an action RPG developed by FromSoftware. Experience the epic conclusion to the Dark Souls trilogy with challenging combat, intricate level design, and a dark fantasy world."
                },
                new Product
                {
                    Id = 2,
                    Name = "Elden Ring",
                    Price = 59.99m,
                    VendorName = "FromSoftware Store",
                    ThumbnailUrl = "https://via.placeholder.com/200x300/2d5016/ffffff?text=Elden+Ring",
                    Category = "Action",
                    Stock = 20,
                    Description = "Elden Ring is an action RPG set in a vast open world. Explore the Lands Between, face challenging foes, and uncover the mysteries of the Elden Ring."
                },
                new Product
                {
                    Id = 3,
                    Name = "Devil May Cry 5",
                    Price = 29.99m,
                    VendorName = "Capcom Games",
                    ThumbnailUrl = "https://via.placeholder.com/200x300/8b0000/ffffff?text=DMC+5",
                    Category = "Action",
                    Stock = 8,
                    Description = "The ultimate Devil Hunter is back in style with Devil May Cry 5. Experience stylish action with multiple playable characters and over-the-top combat."
                },
                new Product
                {
                    Id = 4,
                    Name = "Doom Eternal",
                    Price = 39.99m,
                    VendorName = "Bethesda Shop",
                    ThumbnailUrl = "https://via.placeholder.com/200x300/ff4500/ffffff?text=Doom+Eternal",
                    Category = "Action",
                    Stock = 15,
                    Description = "Hell's armies have invaded Earth. Become the Slayer in an epic single-player campaign to conquer demons across dimensions and stop the final destruction of humanity."
                },

                // RPG Games
                new Product
                {
                    Id = 5,
                    Name = "The Witcher 3: Wild Hunt",
                    Price = 29.99m,
                    VendorName = "CD Projekt Store",
                    ThumbnailUrl = "https://via.placeholder.com/200x300/4a4a4a/ffffff?text=Witcher+3",
                    Category = "RPG",
                    Stock = 5,
                    Description = "As Geralt of Rivia, a monster hunter for hire, embark on an epic journey in a visually stunning fantasy open world filled with meaningful choices and impactful consequences."
                },
                new Product
                {
                    Id = 6,
                    Name = "Cyberpunk 2077",
                    Price = 49.99m,
                    VendorName = "CD Projekt Store",
                    ThumbnailUrl = "https://via.placeholder.com/200x300/ffff00/000000?text=Cyberpunk",
                    Category = "RPG",
                    Stock = 12,
                    Description = "Cyberpunk 2077 is an open-world action-adventure RPG set in Night City, a megalopolis obsessed with power, glamour and body modification."
                },
                new Product
                {
                    Id = 7,
                    Name = "Final Fantasy XVI",
                    Price = 69.99m,
                    VendorName = "Square Enix",
                    ThumbnailUrl = "https://via.placeholder.com/200x300/4169e1/ffffff?text=FF+XVI",
                    Category = "RPG",
                    Stock = 7,
                    Description = "An epic dark fantasy world where the fate of the land is decided by the mighty Eikons and the Dominants who wield them."
                },
                new Product
                {
                    Id = 8,
                    Name = "Baldur's Gate 3",
                    Price = 59.99m,
                    VendorName = "Larian Studios",
                    ThumbnailUrl = "https://via.placeholder.com/200x300/800020/ffffff?text=BG3",
                    Category = "RPG",
                    Stock = 18,
                    Description = "Gather your party and return to the Forgotten Realms in a tale of fellowship and betrayal, sacrifice and survival, and the lure of absolute power."
                },

                // Strategy Games
                new Product
                {
                    Id = 9,
                    Name = "Civilization VI",
                    Price = 49.99m,
                    VendorName = "2K Games",
                    ThumbnailUrl = "https://via.placeholder.com/200x300/006994/ffffff?text=Civ+VI",
                    Category = "Strategy",
                    Stock = 8,
                    Description = "Build an empire to stand the test of time in Civilization VI. Advance your civilization from the Stone Age to the Information Age."
                },
                new Product
                {
                    Id = 10,
                    Name = "Total War: Warhammer III",
                    Price = 59.99m,
                    VendorName = "Sega Shop",
                    ThumbnailUrl = "https://via.placeholder.com/200x300/8b0000/ffffff?text=Warhammer",
                    Category = "Strategy",
                    Stock = 6,
                    Description = "The cataclysmic conclusion to the Total War: Warhammer trilogy. Rally your forces and step into the Realm of Chaos."
                },
                new Product
                {
                    Id = 11,
                    Name = "XCOM 2",
                    Price = 34.99m,
                    VendorName = "2K Games",
                    ThumbnailUrl = "https://via.placeholder.com/200x300/0f4c81/ffffff?text=XCOM+2",
                    Category = "Strategy",
                    Stock = 9,
                    Description = "Earth has changed. Twenty years have passed since world leaders offered an unconditional surrender to alien forces. XCOM, the planet's last line of defense, was left decimated."
                },

                // Sports Games
                new Product
                {
                    Id = 12,
                    Name = "FIFA 24",
                    Price = 59.99m,
                    VendorName = "EA Sports",
                    ThumbnailUrl = "https://via.placeholder.com/200x300/00529f/ffffff?text=FIFA+24",
                    Category = "Sports",
                    Stock = 0,
                    Description = "Experience the most authentic football simulation with FIFA 24. Play with your favorite teams and players in stunning detail."
                },
                new Product
                {
                    Id = 13,
                    Name = "NBA 2K24",
                    Price = 59.99m,
                    VendorName = "2K Sports",
                    ThumbnailUrl = "https://via.placeholder.com/200x300/ff6347/ffffff?text=NBA+2K24",
                    Category = "Sports",
                    Stock = 11,
                    Description = "Step into the court with NBA 2K24. Experience basketball like never before with enhanced gameplay and realistic graphics."
                },

                // Puzzle Games
                new Product
                {
                    Id = 14,
                    Name = "Portal 2",
                    Price = 19.99m,
                    VendorName = "Valve Store",
                    ThumbnailUrl = "https://via.placeholder.com/200x300/ff8c00/ffffff?text=Portal+2",
                    Category = "Puzzle",
                    Stock = 15,
                    Description = "The sequel to the award-winning Portal featuring mind-bending puzzles, dark humor, and an unforgettable story."
                },
                new Product
                {
                    Id = 15,
                    Name = "The Witness",
                    Price = 24.99m,
                    VendorName = "Thekla Inc",
                    ThumbnailUrl = "https://via.placeholder.com/200x300/87ceeb/000000?text=The+Witness",
                    Category = "Puzzle",
                    Stock = 4,
                    Description = "Explore a beautiful open-world island filled with intricate puzzles that will challenge your perception and understanding."
                },

                // Adventure Games
                new Product
                {
                    Id = 16,
                    Name = "Red Dead Redemption 2",
                    Price = 49.99m,
                    VendorName = "Rockstar Games",
                    ThumbnailUrl = "https://via.placeholder.com/200x300/8b4513/ffffff?text=RDR2",
                    Category = "Adventure",
                    Stock = 13,
                    Description = "Experience the epic tale of outlaw Arthur Morgan and the Van der Linde gang as they rob, fight and steal their way across America."
                },
                new Product
                {
                    Id = 17,
                    Name = "The Last of Us Part II",
                    Price = 39.99m,
                    VendorName = "Naughty Dog",
                    ThumbnailUrl = "https://via.placeholder.com/200x300/2f4f4f/ffffff?text=TLOU+2",
                    Category = "Adventure",
                    Stock = 10,
                    Description = "Five years after their dangerous journey across the post-pandemic United States, Ellie and Joel have settled down in Jackson, Wyoming."
                },
                new Product
                {
                    Id = 18,
                    Name = "Uncharted 4",
                    Price = 29.99m,
                    VendorName = "Naughty Dog",
                    ThumbnailUrl = "https://via.placeholder.com/200x300/4682b4/ffffff?text=Uncharted+4",
                    Category = "Adventure",
                    Stock = 6,
                    Description = "Several years after his last adventure, retired fortune hunter Nathan Drake is forced back into the world of thieves."
                },

                // Horror Games
                new Product
                {
                    Id = 19,
                    Name = "Resident Evil 4 Remake",
                    Price = 59.99m,
                    VendorName = "Capcom Games",
                    ThumbnailUrl = "https://via.placeholder.com/200x300/8b0000/ffffff?text=RE4",
                    Category = "Horror",
                    Stock = 14,
                    Description = "Survival is just the beginning. Six years have passed since the biological disaster in Raccoon City."
                },
                new Product
                {
                    Id = 20,
                    Name = "Silent Hill 2 Remake",
                    Price = 69.99m,
                    VendorName = "Konami",
                    ThumbnailUrl = "https://via.placeholder.com/200x300/2f4f2f/ffffff?text=Silent+Hill",
                    Category = "Horror",
                    Stock = 8,
                    Description = "Experience the psychological horror masterpiece rebuilt from the ground up with stunning visuals and immersive atmosphere."
                },
            };
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            var allProducts = await GetAllProductsAsync();
            return allProducts.FirstOrDefault(p => p.Id == id);
        }

        public async Task<List<Product>> GetProductsByCategoryAsync(string category)
        {
            var allProducts = await GetAllProductsAsync();
            return allProducts.Where(p => p.Category == category).ToList();
        }
    }
}
