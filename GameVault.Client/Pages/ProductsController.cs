using Microsoft.AspNetCore.Mvc;
using GameVault.Server.Services;
using GameVault.Shared.Models;

namespace GameVault.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IFirestoreService _firestore;

    public ProductsController(IFirestoreService firestore)
    {
        _firestore = firestore;
    }

    [HttpGet]
    public async Task<ActionResult<List<Listing>>> GetAllProducts()
    {
        try
        {
            var products = await _firestore.GetCollectionAsync<Listing>("listings");
            return Ok(products);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Listing>> GetProductById(string id)
    {
        try
        {
            Console.WriteLine($"Fetching product with ID: {id}");
            var product = await _firestore.GetDocumentAsync<Listing>("listings", id);

            if (product == null)
            {
                Console.WriteLine($"Product not found: {id}");
                return NotFound(new { error = "Product not found" });
            }

            Console.WriteLine($"Product found: {product.Name}");
            return Ok(product);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("category/{category}")]
    public async Task<ActionResult<List<Listing>>> GetProductsByCategory(string category)
    {
        try
        {
            var products = await _firestore.QueryDocumentsAsync<Listing>("listings", "category", category);
            return Ok(products);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPost("search")]
    public async Task<ActionResult<List<Listing>>> SearchProducts([FromBody] SearchRequest request)
    {
        try
        {
            var allProducts = await _firestore.GetCollectionAsync<Listing>("listings");

            if (string.IsNullOrWhiteSpace(request.Query))
            {
                return Ok(allProducts);
            }

            var query = request.Query.ToLower();
            var filteredProducts = allProducts
                .Where(p =>
                    p.Name.ToLower().Contains(query) ||
                    p.Description.ToLower().Contains(query) ||
                    p.Category.ToString().ToLower().Contains(query))
                .ToList();

            return Ok(filteredProducts);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
}

public class SearchRequest
{
    public string Query { get; set; } = string.Empty;
}