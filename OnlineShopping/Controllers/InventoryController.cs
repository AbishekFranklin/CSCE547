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

        // Constructor for the InventoryController class
        public InventoryController(DataContext context)
        {
            _context = context;
        }

        // Action method to update the stock quantity for a specific item in the inventory.
        [HttpPost("UpdateStock")]
        public IActionResult UpdateStock(int itemId, int quantity)
        {
            // Find the inventory item with the given product ID in the database.
            var inventory = _context.Inventory.FirstOrDefault(i => i.ProductId == itemId);
            if (inventory == null)
            {
                return new NotFoundObjectResult("Inventory not found for the product!");
            }

            // Update the quantity of the inventory item with the provided value.
            inventory.Quantity = quantity;

            // Save the changes to the database.
            _context.SaveChanges();

            return new OkObjectResult("Inventory updated successfully.");
        }

        // Action method to change the price of a specific item in the product list.
        [HttpPost("ChangePrice")]
        public IActionResult ChangePrice(int itemId, decimal price)
        {
            // Find the product with the given item ID in the database.
            var product = _context.Products.FirstOrDefault(p => p.Id == itemId);
            if (product == null)
            {
                return new NotFoundObjectResult("Product not found or invalid product ID!");
            }

            // Check if the provided price is valid (greater than zero).
            if (price <= 0)
            {
                return new BadRequestObjectResult("The price is not valid!");
            }

            // Update the price of the product with the provided value.
            product.Price = price;

            // Save the changes to the database.
            _context.SaveChanges();

            return new OkObjectResult("Price changed successfully.");
        }

        // Action method to add a new item to the product list and corresponding inventory.
        [HttpPost("AddNewItem")]
        public IActionResult AddNewItem([FromBody] ProductInputModel inputModel)
        {
            // Check if the provided input model is valid.
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid input data.");
            }

            // Check if the price is greater than zero.
            if (inputModel.Price <= 0)
            {
                return BadRequest("Price must be greater than zero.");
            }

            // Check if the quantity is greater than zero.
            if (inputModel.Quantity <= 0)
            {
                return BadRequest("Quantity must be greater than zero.");
            }

            // Create a new product with the properties from the input model.
            var product = new Products
            {
                Name = inputModel.Name,
                Price = inputModel.Price,
                Image = inputModel.Image,
            };

            // Add the new product to the product list.
            _context.Products.Add(product);
            _context.SaveChanges();

            // Create a new entry in the Inventory table with the initial quantity for the new product.
            var inventoryItem = new Inventory
            {
                ProductId = product.Id,
                Quantity = inputModel.Quantity // Set the initial quantity here (you can change it to any desired value)
            };

            // Add the new inventory item to the inventory list.
            _context.Inventory.Add(inventoryItem);
            _context.SaveChanges();

            return new OkObjectResult("New item added successfully.");
        }
    }
}