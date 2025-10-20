using Google.Cloud.Firestore;

namespace GameVault.Shared.Models
{
    [FirestoreData]
    public class Listing
    {
        [FirestoreDocumentId]
        public string Id { get; set; } = string.Empty;

        [FirestoreProperty("name")]
        public string Name { get; set; } = string.Empty;

        [FirestoreProperty("description")]
        public string Description { get; set; } = string.Empty;

        [FirestoreProperty("price")]
        public int Price { get; set; }

        [FirestoreProperty("stock")]
        public int Stock { get; set; }

        [FirestoreProperty("category")]
        public string Category { get; set; } = string.Empty;

        [FirestoreProperty("ownerID")]
        public string OwnerID { get; set; } = string.Empty;

        [FirestoreProperty("image")]
        public string Image { get; set; } = string.Empty;

        [FirestoreProperty("status")]
        public int Status { get; set; }
    }
}
