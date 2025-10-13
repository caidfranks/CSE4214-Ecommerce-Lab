using System;
using System.Diagnostics.CodeAnalysis;

namespace GameVault.Client.Models;

public class EditableListing
{
  public required string Name { get; set; }
  public required string Description { get; set; }
  public required decimal Price { get; set; }
  public required int Stock { get; set; }
}
