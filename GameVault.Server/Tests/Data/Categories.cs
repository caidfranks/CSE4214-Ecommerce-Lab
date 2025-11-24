using GameVault.Server.Models.Firestore;

namespace GameVault.Server.Tests.Data;

// Example data for the "categories" firestore collection
public class CategoryExample
{
  public FirestoreCategory firestoreCategory;
  public Category category;
  public List<FirestoreCategory> firestoreCategories;
  public List<Category> categories;

  public CategoryExample()
  {
    firestoreCategory = new()
    {
      Name = "Test1"
    };
    category = new()
    {
      Id = "abc123",
      Name = firestoreCategory.Name
    };
    firestoreCategories = [
      firestoreCategory,
      new () {
        Name = "Test2"
      }
    ];
    categories = [
      category,
      new () {
        Id = "def456",
        Name = "Test2"
      }
    ];
  }
}