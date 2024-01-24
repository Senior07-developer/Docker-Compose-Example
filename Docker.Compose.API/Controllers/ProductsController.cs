using Docker.Compose.API.Cache;
using Docker.Compose.API.Contracts;
using Docker.Compose.API.Extensions;
using Docker.Compose.API.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace Docker.Compose.API.Controllers;

[Route("api/products")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IDistributedCache _cache;

    public ProductsController(ApplicationDbContext dbContext, IDistributedCache cache)
    {
        _dbContext = dbContext;
        _cache = cache;
    }

    [HttpGet("{id:guid}", Name = "GetProduct")]
    public async Task<IActionResult> Get(Guid id)
    {
        var product = await _cache.GetOrCreateAsync($"products-{id}",
            async () =>
            {
                return await _dbContext.Products.FirstOrDefaultAsync(x => x.Id == id);
            }, CacheOptions.DefaultExpiration);

        return product is null ? NotFound() : Ok(product);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products = await _dbContext.Products.AsNoTracking().ToListAsync();

        return Ok(products);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateProductRequest request)
    {
        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price
        };

        _dbContext.Products.Add(product);

        await _dbContext.SaveChangesAsync();

        return CreatedAtRoute("GetProduct", new { product?.Id }, product);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateProductRequest request)
    {
        if (request?.Id != id)
        {
            return BadRequest();
        }

        var product = await _dbContext.Products.FirstOrDefaultAsync(x => x.Id == id);

        if (product is null) 
        {
            return NotFound();
        }

        product.Name = request.Name;
        product.Description = request.Description;
        product.Price = request.Price;

        await _dbContext.SaveChangesAsync();

        await _cache.RemoveAsync($"products-{id}");

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var product = await _dbContext.Products.FirstOrDefaultAsync(x => x.Id == id);

        if (product is null)
        {
            return NotFound();
        }

        _dbContext.Products.Remove(product);

        await _dbContext.SaveChangesAsync();
        
        await _cache.RemoveAsync($"products-{id}");

        return NoContent();
    }
}
