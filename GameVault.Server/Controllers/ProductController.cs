using Microsoft.AspNetCore.Mvc;
using GameVault.Server.Services;
using GameVault.Shared.Models;
using GameVault.Shared.DTOs;
using OneOf.Types;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Square;
using System.Security.Cryptography.X509Certificates;
using System.Reflection.Metadata;
using FirebaseAdmin.Messaging;

namespace GameVault.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IFirestoreService _firestore;

    public ProductController(IFirestoreService firestore)
    {
        _firestore = firestore;
    }

    [HttpGet]
    public async Task<ActionResult<ListResponse<FullListingDTO>>> GetAllProducts()
    {
        try
        {
            var products = await _firestore.GetCollectionAsyncWithId<Models.Firestore.Listing>("listings");
            List<FullListingDTO> dTOs = [];

            List<CachedVendor> vendorCache = [];
            foreach (Models.Firestore.Listing product in products)
            {
                if (product.Status == ListingStatus.Published)
                {
                    // Lookup vendor
                    string vendorName;

                    // TODO: Make sure this is working (hasn't been tested)
                    List<CachedVendor> thisVendor = vendorCache.Where(p =>
                        p.Id == product.OwnerID).ToList();

                    if (thisVendor.Count > 0)
                    {
                        vendorName = thisVendor[0].Name;
                    }
                    else
                    {
                        // Look up vendor in database
                        var vendor = await _firestore.GetDocumentAsync<Models.User>("users", product.OwnerID);
                        vendorName = vendor?.DisplayName ?? "Unknown Vendor";
                        vendorCache.Add(new()
                        {
                            Id = product.OwnerID,
                            Name = vendorName
                        });
                    }

                    dTOs.Add(new FullListingDTO()
                    {
                        Name = product.Name,
                        Price = product.Price,
                        Description = product.Description,
                        Stock = product.Stock,
                        Status = product.Status,
                        Image = product.Image,
                        Id = product.Id,
                        OwnerID = product.OwnerID,
                        LastModified = product.LastModified,
                        VendorName = vendorName
                    });
                }
            }

            return Ok(new ListResponse<FullListingDTO>()
            {
                Success = true,
                List = dTOs,
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ListResponse<FullListingDTO>()
            {
                Success = false,
                Message = ex.Message
            });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DataResponse<FullListingDTO>>> GetProductById(string id)
    {
        try
        {
            Console.WriteLine($"Fetching product with ID: {id}");
            var product = await _firestore.GetDocumentAsync<Models.Firestore.Listing>("listings", id);

            if (product == null)
            {
                Console.WriteLine($"Product not found: {id}");
                return NotFound(new DataResponse<FullListingDTO>()
                {
                    Success = false,
                    Message = "Product not found"
                });
            }

            // Check for if published or not

            // Look up vendor in database
            Models.User? vendor = await _firestore.GetDocumentAsync<Models.User>("users", product.OwnerID);
            string vendorName = vendor?.DisplayName ?? "Unknown Vendor";

            Console.WriteLine($"Product found: {product.Name}");
            return Ok(new DataResponse<FullListingDTO>()
            {
                Success = true,
                Data = new FullListingDTO()
                {
                    Name = product.Name,
                    Price = product.Price,
                    Description = product.Description,
                    Stock = product.Stock,
                    Status = product.Status,
                    Image = product.Image,
                    Id = product.Id,
                    OwnerID = product.OwnerID,
                    LastModified = product.LastModified,
                    VendorName = vendorName
                }
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return StatusCode(500, new DataResponse<FullListingDTO>()
            {
                Success = false,
                Message = ex.Message
            });
        }
    }

    [HttpGet("category/{category}")]
    public async Task<ActionResult<ListResponse<FullListingDTO>>> GetProductsByCategory(string category)
    {
        try
        {
            var products = await _firestore.QueryDocumentsAsyncWithId<Models.Firestore.Listing>("listings", "category", category);
            List<FullListingDTO> dTOs = [];

            List<CachedVendor> vendorCache = [];
            foreach (Models.Firestore.Listing product in products)
            {
                if (product.Status == ListingStatus.Published)
                {
                    // Lookup vendor
                    string vendorName;

                    // TODO: Make sure this is working (hasn't been tested)
                    List<CachedVendor> thisVendor = vendorCache.Where(p =>
                        p.Id == product.OwnerID).ToList();

                    if (thisVendor.Count > 0)
                    {
                        vendorName = thisVendor[0].Name;
                    }
                    else
                    {
                        // Look up vendor in database
                        var vendor = await _firestore.GetDocumentAsync<Models.User>("users", product.OwnerID);
                        vendorName = vendor?.DisplayName ?? "Unknown Vendor";
                        vendorCache.Add(new()
                        {
                            Id = product.OwnerID,
                            Name = vendorName
                        });
                    }

                    dTOs.Add(new FullListingDTO()
                    {
                        Name = product.Name,
                        Price = product.Price,
                        Description = product.Description,
                        Stock = product.Stock,
                        Status = product.Status,
                        Image = product.Image,
                        Id = product.Id,
                        OwnerID = product.OwnerID,
                        LastModified = product.LastModified,
                        VendorName = vendorName
                    });
                }
            }

            return Ok(new ListResponse<FullListingDTO>()
            {
                Success = true,
                List = dTOs,
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ListResponse<FullListingDTO>()
            {
                Success = false,
                Message = ex.Message
            });
        }
    }

    [HttpPost("search")]
    public async Task<ActionResult<ListResponse<FullListingDTO>>> SearchProducts([FromBody] SearchRequest request)
    {
        try
        {
            var allProducts = await _firestore.GetCollectionAsyncWithId<Models.Firestore.Listing>("listings");

            if (string.IsNullOrWhiteSpace(request.Query))
            {
                return await GetAllProducts();
            }

            var query = request.Query.ToLower();
            var filteredProducts = allProducts
                .Where(p =>
                    p.Name.ToLower().Contains(query) ||
                    p.Description.ToLower().Contains(query)// ||
                                                           // p.Category.ToString().ToLower().Contains(query))
                )
                .ToList();

            List<FullListingDTO> dTOs = [];

            List<CachedVendor> vendorCache = [];
            foreach (Models.Firestore.Listing product in filteredProducts)
            {
                if (product.Status == ListingStatus.Published)
                {
                    // Lookup vendor
                    string vendorName;

                    // TODO: Make sure this is working (hasn't been tested)
                    List<CachedVendor> thisVendor = vendorCache.Where(p =>
                        p.Id == product.OwnerID).ToList();

                    if (thisVendor.Count > 0)
                    {
                        vendorName = thisVendor[0].Name;
                    }
                    else
                    {
                        // Look up vendor in database
                        var vendor = await _firestore.GetDocumentAsync<Models.User>("users", product.OwnerID);
                        vendorName = vendor?.DisplayName ?? "Unknown Vendor";
                        vendorCache.Add(new()
                        {
                            Id = product.OwnerID,
                            Name = vendorName
                        });
                    }

                    dTOs.Add(new FullListingDTO()
                    {
                        Name = product.Name,
                        Price = product.Price,
                        Description = product.Description,
                        Stock = product.Stock,
                        Status = product.Status,
                        Image = product.Image,
                        Id = product.Id,
                        OwnerID = product.OwnerID,
                        LastModified = product.LastModified,
                        VendorName = vendorName
                    });
                }
            }

            return Ok(new ListResponse<FullListingDTO>()
            {
                Success = true,
                List = dTOs,
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ListResponse<FullListingDTO>()
            {
                Success = false,
                Message = ex.Message
            });
        }
    }
}

public class CachedVendor
{
    public required string Id;
    public required string Name;
};
