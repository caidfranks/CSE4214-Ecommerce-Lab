using GameVault.Server.Tests.Stubs;
using GameVault.Server.Controllers;
using Microsoft.AspNetCore.Mvc;
using GameVault.Shared.Models;
using GameVault.Server.Models.Firestore;
using GameVault.Shared.DTOs;
using System.Collections;
namespace GameVault.Server.Tests;

class AssertException : Exception
{

}

class Tester
{
  FirestoreServiceStub firestoreStub;
  static public int Total = 0;
  static public int Fails = 0;

  public Tester()
  {
    firestoreStub = new();
  }
  public async Task<bool> Test()
  {
    ProductTests productTests = new(firestoreStub);
    await productTests.Test();

    Console.WriteLine();
    if (Fails == 0)
    {
      Console.WriteLine($"All {Total} tests passed!");
    }
    else
    {
      Console.WriteLine($"{Fails} of {Total} tests failed!");
    }
    Console.WriteLine();

    return Fails == 0;
  }

  static public bool AssertSuccess<T>(ActionResult<T> response, string success, string failure) where T : Shared.Models.BaseResponse
  {
    Total++;
    if (response.Value is null || !response.Value.Success)
    {
      Console.WriteLine($"Assertion failed: {failure ?? "No message provided"}");
      Fails++;
      return false;
      // throw new Exception();
    }
    else
    {
      Console.WriteLine($"Assertion passed: {success}");
      return true;
    }
  }

  static public bool AssertFailure<T>(ActionResult<T> response, string success, string failure) where T : Shared.Models.BaseResponse
  {
    Total++;
    if (response.Value?.Success ?? false)
    {
      Console.WriteLine($"Assertion failed: {failure}");
      Fails++;
      return false;
      // throw new Exception();
    }
    else
    {
      Console.WriteLine($"Assertion passed: {success}");
      return true;
    }
  }

  static public bool AssertEqual<T>(T actual, T expected, string success, string failure)
  {
    // if (actual == expected)
    Total++;
    if (!EqualityComparer<T>.Default.Equals(actual, expected))
    {
      Console.WriteLine($"Assertion failed: {failure ?? "No message provided"}");
      Fails++;
      return false;
      // throw new Exception();
    }
    else
    {
      Console.WriteLine($"Assertion passed: {success}");
      return true;
    }
  }
}