using GameVault.Server.Controllers;
using GameVault.Server.Tests.Stubs;
using Microsoft.AspNetCore.Mvc;

namespace GameVault.Server.Tests;

class ProductTests
{

  FirestoreServiceStub firestoreStub;

  ProductController productController;

  public ProductTests(FirestoreServiceStub _firestoreStub)
  {
    firestoreStub = _firestoreStub;
    productController = new(firestoreStub);
  }

  public async Task Test()
  {
    await GetAllProducts();
    await GetCategories();
  }

  private async Task GetAllProducts()
  {
    // TODO: Doesn't work because can't extract data from ActionResult

    Console.WriteLine();
    Console.WriteLine("Testing route GET /api/product");
    Console.WriteLine();

    // TODO: Create example product data for testing
    firestoreStub.SetFail(false);
    firestoreStub.SetEmpty(false);
    var response = await productController.GetAllProducts();
    // response.Result.ExecuteResult(new ActionContext());
    Console.WriteLine($"{response.Result}");

    Console.Write("Test 1: ");
    if (Tester.AssertSuccess(response, "Success", "Failure"))
    {
      // Test with example data
      Console.WriteLine("Test 2: ");
    }
    else
    {
      Console.WriteLine("Skipping Test 2");
    }

    firestoreStub.SetEmpty(true);
    response = await productController.GetAllProducts();

    Console.Write("Test 3: ");
    if (Tester.AssertSuccess(response, "Success", "Failure"))
    {
      Console.Write("Test 4: ");
      Tester.AssertEqual(response.Value!.List!.Count, 0, "Empty", "Not empty");
    }
    else
    {
      Console.WriteLine("Skipping Test 4");
    }

  }

  private async Task GetCategories()
  {
    Console.WriteLine();
    Console.WriteLine("Testing route GET /api/product/categories");
    Console.WriteLine();

    firestoreStub.SetFail(false);
    firestoreStub.SetEmpty(false);
    var response = await productController.GetCategories();

    Console.Write("Test 1: ");
    if (Tester.AssertSuccess(response, "Success!", "Failure!"))
    {
      Console.Write("Test 2: ");
      if (Tester.AssertEqual(response.Value!.List!.Count, 2, "2 categories", "Not enough categories"))
      {
        Console.Write("Test 3: ");
        Tester.AssertEqual(response.Value!.List![0].Name, "Test1", "Right first category", "Wrong first category");
      }
      else
      {
        Console.WriteLine("Skipping Test 3");
      }
    }
    else
    {
      Console.WriteLine("Skipping Tests 2 and 3");
    }

    firestoreStub.SetFail(true);
    response = await productController.GetCategories();

    Console.Write("Test 4: ");
    Tester.AssertFailure(response, "Failure Success!", "Failure Failure!");

    firestoreStub.SetFail(false);
    firestoreStub.SetEmpty(true);
    response = await productController.GetCategories();

    Console.Write("Test 5: ");
    if (Tester.AssertSuccess(response, "Success", "Failure"))
    {
      Console.Write("Test 6: ");
      Tester.AssertEqual(response.Value!.List!.Count, 0, "Empty", "Not empty");
    }
    else
    {
      Console.WriteLine("Skipping Test 6");
    }
  }
}