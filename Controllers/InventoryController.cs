using Microsoft.AspNetCore.Mvc;
using OnlineShopping.Model;
using OnlineShopping.Models;
using System.Linq;

namespace OnlineShopping.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly DataContext _context;

        public InventoryController(DataContext context)
        {
            _context = context;
        }

        [HttpPost("UpdateStock")]
        public IActionResult UpdateStock(int itemId, int quantity)
        {
            var inventory = _context.Inventory.FirstOrDefault(i => i.ProductId == itemId);
            if (inventory == null)
            {
                return new NotFoundObjectResult("Inventory not found for the product!");
            }

            inventory.Quantity = quantity;

            _context.SaveChanges();

            return new OkObjectResult("Inventory updated successfully.");
        }

        [HttpPost("ChangePrice")]
        public IActionResult ChangePrice(int itemId, decimal price)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == itemId);
            if (product == null)
            {
                return new NotFoundObjectResult("Product not found or invalid product ID!");
            }

            if (price <= 0)
            {
                return new BadRequestObjectResult("The price is not valid!");
            }

            product.Price = price;

            _context.SaveChanges();

            return new OkObjectResult("Price changed successfully.");
        }

        [HttpPost("AddNewItem")]
        public IActionResult AddNewItem([FromBody] ProductInputModel inputModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid input data.");
            }

            // Check if the price is greater than zero
            if (inputModel.Price <= 0)
            {
                return BadRequest("Price must be greater than zero.");
            }

            // Check if the quantity is greater than zero
            if (inputModel.Quantity <= 0)
            {
                return BadRequest("Quantity must be greater than zero.");
            }

            var product = new Products
            {
                Name = inputModel.Name,
                Price = inputModel.Price,
                Image = inputModel.Image,
            };

            _context.Products.Add(product);
            _context.SaveChanges();

            // Create a new entry in the Inventory table with the initial quantity
            var inventoryItem = new Inventory
            {
                ProductId = product.Id,
                Quantity = inputModel.Quantity // Set the initial quantity here (you can change it to any desired value)
            };
            _context.Inventory.Add(inventoryItem);
            _context.SaveChanges();

            return new OkObjectResult("New item added successfully.");
        }
    }
}
