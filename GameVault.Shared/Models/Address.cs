using Google.Cloud.Firestore;

namespace GameVault.Shared.Models;

[FirestoreData]
public class Address
{
    [FirestoreProperty] public string Street { get; set; } = string.Empty;

    [FirestoreProperty] public string City { get; set; } = string.Empty;

    [FirestoreProperty] public string State { get; set; } = string.Empty;

    [FirestoreProperty] public string PostalCode { get; set; } = string.Empty;
    [FirestoreProperty] public string Country { get; set; } = "US";
}