using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShopping.Model;
using OnlineShopping.Models;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;

namespace OnlineShopping.Services
{
    public class ShoppingService : IShoppingService
    {
        private readonly DataContext _context;
        private readonly CardValidation _cardValidation;
        private readonly CartCalculator _cartCalculator;

        // Constructor for the ShoppingService class, which receives the required dependencies via dependency injection.
        public ShoppingService(DataContext context, CartCalculator cartCalculator, CardValidation cardValidation) 
        {
            _context = context;
            _cartCalculator = cartCalculator; // Store the injected instance of CartCalculator
            _cardValidation = cardValidation; // Store the injected instance of CardValidation
        }

        // Method to retrieve all the items from the database
        public IActionResult GetAllItems()
        {
            var products = _context.Products.ToList();
            return new OkObjectResult(products);
        }

        // Method to create a new cart ID with the given cart name.
        public IActionResult CreateNewCart(string cartName)
        {
            var existingCart = _context.Carts.FirstOrDefault(c => c.Name == cartName);

            // Check if a cart with the given name already exists in the database.
            if (existingCart != null)
            {
                return new OkObjectResult(new { cartID = existingCart.Id });
            }

            // Create a new cart with the given name and add it to the database.
            var newCart = new Cart { Name = cartName };
            _context.Carts.Add(newCart);
            _context.SaveChanges();
            int cartId = newCart.Id; // Retrieve the generated cart ID
            return new OkObjectResult(new { cartID = cartId });
        }

        // Method to add an item to the cart with the given cart ID, item ID, and quantity.
        public IActionResult AddItemToCart(int cartId, int itemId, int quantity)
        {
            // Check if the cart with the given ID exists in the database.
            var cart = _context.Carts.FirstOrDefault(c => c.Id == cartId);
            if (cart == null)
            {
                return new NotFoundObjectResult("Cart not found or invalid cart ID!");
            }

            // Check if the product with the given ID exists in the database.
            var product = _context.Products.FirstOrDefault(p => p.Id == itemId);
            if (product == null)
            {
                return new NotFoundObjectResult("Product not found or invalid product ID!");
            }

            // Check if the inventory for the product exists in the database.
            var inventory = _context.Inventory.FirstOrDefault(i => i.ProductId == itemId);
            if (inventory == null)
            {
                return new NotFoundObjectResult("Inventory not found for the product!");
            }

            // Check if the quantity is valid (positive integer).
            if (quantity <= 0)
            {
                return new BadRequestObjectResult("Invalid quantity! Quantity must be a positive integer.");
            }

            // Check if there is sufficient quantity available in the inventory.
            if (inventory.Quantity < quantity)
            {
                return new BadRequestObjectResult("Insufficient quantity available for the product.");
            }

            // Create a new CartItem instance and add it to the cart.
            var cartItem = new CartItem
            {
                ProductId = itemId,
                ProductName = product.Name,
                Quantity = quantity,
                Value = product.Price,
                CartId = cartId,
                Cart = cart
            };

            cart.Items.Add(cartItem);

            // Update the inventory quantity
            inventory.Quantity -= quantity;

            _context.SaveChanges();

            return new OkObjectResult("Item added to cart successfully.");
        }

        // Method to remove an item from the cart with the given cart ID and item ID.
        public IActionResult RemoveItem(int cartId, int itemId)
        {
            var cart = _context.Carts.Include(c => c.Items).FirstOrDefault(c => c.Id == cartId);

            // Check if the cart with the given ID exists in the database and include the cart items.
            if (cart == null)
            {
                return new NotFoundObjectResult("Cart not found or invalid cart ID!");
            }

            // Find the item to remove from the cart.
            var itemToRemove = cart.Items.FirstOrDefault(item => item.ProductId == itemId);
            if (itemToRemove == null)
            {
                return new NotFoundObjectResult("Item not found in the cart!");
            }

            // Retrieve the quantity of the item to add back to the inventory.
            var quantity = itemToRemove.Quantity;
            var inventory = _context.Inventory.FirstOrDefault(i => i.ProductId == itemId);
            if (inventory == null)
            {
                return new NotFoundObjectResult("Inventory not found for the product!");
            }

            // Update the inventory quantity by adding back the quantity of the removed item.
            inventory.Quantity += quantity;

            // Remove item from the cart
            cart.Items.Remove(itemToRemove);

            _context.SaveChanges();

            return new OkObjectResult("Item removed from cart successfully.");
        }

        // Method to get the cart details with the given cart ID.
        public IActionResult GetCart(int cartId)
        {
            // Check if the cart with the given ID exists in the database and include the cart items.
            var cart = _context.Carts.Include(c => c.Items).FirstOrDefault(c => c.Id == cartId);
            if (cart == null)
            {
                return new NotFoundObjectResult("Cart not found or invalid cart ID!");
            }

            // Calculate the cost of each cart item and store them in a list.
            var cartItems = cart.Items.Select(item => new
            {
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                Quantity = item.Quantity,
                Price = item.Value,
                ItemCost = item.Value * item.Quantity
            }).ToList();

            // Calculate the total cost of the cart by summing the costs of all cart items.
            decimal totalCost = cartItems.Sum(item => item.ItemCost);

            return new OkObjectResult(new { CartItems = cartItems, TotalCost = totalCost });
        }

        // Method to calculate different types of totals for the cart with the given cart ID.
        public IActionResult GetTotal(int cartId)
        {
            // Check if the cart with the given ID exists in the database and include the cart items.
            var cart = _context.Carts.Include(c => c.Items).FirstOrDefault(c => c.Id == cartId);
            if (cart == null)
            {
                return new NotFoundObjectResult("Cart not found or invalid cart ID!");
            }

            // Calculate regular total, bundle total, and total with taxes using the CartCalculator.
            decimal regularTotal = _cartCalculator.CalculateRegularTotal(cart);
            decimal bundleTotal = _cartCalculator.CalculateRegularOrBundleTotal(cart);
            decimal totalWithTaxes = _cartCalculator.CalculateTotalWithTaxes(cart);

            return new OkObjectResult(new
            {
                RegularTotal = regularTotal,
                BundleTotal = bundleTotal,
                TotalWithTaxes = totalWithTaxes
            });
        }

        // Method to process payment for the cart with the given cart ID and card information.
        public IActionResult ProcessPayment(int cartId, [FromBody] CardInformation cardInfo)
        {
            // Validate the card information using the CardValidation class.
            if (!_cardValidation.ValidateCardNumber(cardInfo.CardNumber))
            {
                return new BadRequestObjectResult("Invalid card number!");
            }

            if (!_cardValidation.ValidateCardHolderName(cardInfo.CardHolderName))
            {
                return new BadRequestObjectResult("Invalid Card Holder Name. Please provide a valid Name.");
            }

            if (!_cardValidation.ValidateExpiryDateFormat(cardInfo.ExpiryDate))
            {
                return new BadRequestObjectResult("Invalid expiry date format. Please use the format 'MM/yyyy' and a valid date.");
            }

            if (!_cardValidation.ValidateCVV(cardInfo.CVV))
            {
                return new BadRequestObjectResult("Invalid CVV. Please provide a 3-digit CVV.");
            }

            // Check if the payment for this cart has already been processed.
            var processedCartExists = _context.ProcessedCarts.Any(pc => pc.CartId == cartId);
            if (processedCartExists)
            {
                return new BadRequestObjectResult("Payment for this cart has already been processed!");
            }

            // Retrieve the cart with the given cart ID and include the cart items.
            var cart = _context.Carts.Include(c => c.Items).FirstOrDefault(c => c.Id == cartId);
            if (cart == null)
            {
                return new NotFoundObjectResult("Cart not found or invalid cart ID!");
            }

            // Calculate the cost of each cart item and store them in a list as a string representation.
            var cartItems = cart.Items.Select(item => new
            {
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                Quantity = item.Quantity,
                Price = item.Value,
                ItemCost = item.Value * item.Quantity
            }).ToList();

            var cartItemsString = string.Join("; ", cartItems.Select(item =>
                $"ProductId: {item.ProductId}, ProductName: {item.ProductName}, Quantity: {item.Quantity}, Price: {item.Price}, ItemCost: {item.ItemCost}"));

            // Calculate the total amount of the cart by summing the costs of all cart items.
            var totalAmount = cartItems.Sum(item => item.ItemCost);

            // Create a new ProcessedCart instance to store the payment information.
            var processedCart = new ProcessedCart
            {
                CartId = cartId,
                TotalAmount = totalAmount,
                PaymentDate = DateTime.Now,
                Items = cartItemsString
            };

            // Add the processedCart to the database and save changes.
            _context.ProcessedCarts.Add(processedCart);
            _context.SaveChanges();

            // Clear the cart items for the processed cart
            cart.Items.Clear();
            _context.SaveChanges();

            return new OkObjectResult("Payment processed successfully!");

        }
    }
}
