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

        public ShoppingService(DataContext context)
        {
            _context = context;
        }

        public IActionResult GetAllItems()
        {
            var products = _context.Products.ToList();
            return new OkObjectResult(products);
        }

        public IActionResult CreateNewCart(string cartName)
        {
            var existingCart = _context.Carts.FirstOrDefault(c => c.Name == cartName);
            if (existingCart != null)
            {
                return new OkObjectResult($"Cart already exists. Cart ID: {existingCart.Id}");
            }

            var newCart = new Cart { Name = cartName };
            _context.Carts.Add(newCart);
            _context.SaveChanges();
            int cartId = newCart.Id; // Retrieve the generated cart ID
            return new OkObjectResult($"New cart created successfully. Cart ID: {cartId}");
        }

        public IActionResult AddItemToCart(int cartId, int itemId, int quantity)
        {
            var cart = _context.Carts.FirstOrDefault(c => c.Id == cartId);
            if (cart == null)
            {
                return new NotFoundObjectResult("Cart not found or invalid cart ID!");
            }

            var product = _context.Products.FirstOrDefault(p => p.Id == itemId);
            if (product == null)
            {
                return new NotFoundObjectResult("Product not found or invalid product ID!");
            }

            var inventory = _context.Inventory.FirstOrDefault(i => i.ProductId == itemId);
            if (inventory == null)
            {
                return new NotFoundObjectResult("Inventory not found for the product!");
            }

            if (quantity <= 0)
            {
                return new BadRequestObjectResult("Invalid quantity! Quantity must be a positive integer.");
            }

            if (inventory.Quantity < quantity)
            {
                return new BadRequestObjectResult("Insufficient quantity available for the product.");
            }

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

        public IActionResult RemoveItem(int cartId, int itemId)
        {
            var cart = _context.Carts.Include(c => c.Items).FirstOrDefault(c => c.Id == cartId);
            if (cart == null)
            {
                return new NotFoundObjectResult("Cart not found or invalid cart ID!");
            }

            var itemToRemove = cart.Items.FirstOrDefault(item => item.ProductId == itemId);
            if (itemToRemove == null)
            {
                return new NotFoundObjectResult("Item not found in the cart!");
            }

            var quantity = itemToRemove.Quantity;
            var inventory = _context.Inventory.FirstOrDefault(i => i.ProductId == itemId);
            if (inventory == null)
            {
                return new NotFoundObjectResult("Inventory not found for the product!");
            }

            // Update the inventory quantity
            inventory.Quantity += quantity;

            // Remove item from the cart
            cart.Items.Remove(itemToRemove);

            _context.SaveChanges();

            return new OkObjectResult("Item removed from cart successfully.");
        }

        public IActionResult GetCart(int cartId)
        {
            var cart = _context.Carts.Include(c => c.Items).FirstOrDefault(c => c.Id == cartId);
            if (cart == null)
            {
                return new NotFoundObjectResult("Cart not found or invalid cart ID!");
            }

            var cartItems = cart.Items.Select(item => new
            {
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                Quantity = item.Quantity,
                Price = item.Value,
                ItemCost = item.Value * item.Quantity
            }).ToList();

            decimal totalCost = cartItems.Sum(item => item.ItemCost);

            return new OkObjectResult(new { CartItems = cartItems, TotalCost = totalCost });
        }

        private decimal CalculateRegularTotal(Cart cart)
        {
            decimal regularTotal = 0;

            foreach (var item in cart.Items)
            {
                decimal itemCost = item.Value * item.Quantity;
                regularTotal += itemCost;
            }

            return regularTotal;
        }

        private decimal CalculateRegularOrBundleTotal(Cart cart)
        {
            decimal regularTotal = 0m;
            decimal discountedTotal = 0m;

            foreach (var item in cart.Items)
            {
                decimal itemCost = item.Value * item.Quantity;
                regularTotal += itemCost;

                if (item.Quantity >= 5 || item.Quantity % 5 == 0)
                {
                    decimal discount = itemCost * 0.15m;
                    discountedTotal += itemCost - discount;
                }
                else
                {
                    discountedTotal += itemCost;
                }
            }

            return discountedTotal;
        }

        private decimal CalculateTotalWithTaxes(Cart cart)
        {
            decimal totalCost = CalculateRegularOrBundleTotal(cart);
            decimal taxRate = 0.10m;
            decimal taxes = totalCost * taxRate;
            totalCost += taxes;

            return totalCost;
        }

        public IActionResult GetTotal(int cartId)
        {
            var cart = _context.Carts.Include(c => c.Items).FirstOrDefault(c => c.Id == cartId);
            if (cart == null)
            {
                return new NotFoundObjectResult("Cart not found or invalid cart ID!");
            }

            decimal regularTotal = CalculateRegularTotal(cart);
            decimal bundleTotal = CalculateRegularOrBundleTotal(cart);
            decimal totalWithTaxes = CalculateTotalWithTaxes(cart);

            return new OkObjectResult(new
            {
                RegularTotal = regularTotal,
                BundleTotal = bundleTotal,
                TotalWithTaxes = totalWithTaxes
            });
        }

        private bool ValidateCardNumber(string cardNumber)
        {
            cardNumber = new string(cardNumber.Where(char.IsDigit).ToArray());

            if (cardNumber.Length != 16)
            {
                return false;
            }

            if (!cardNumber.All(char.IsDigit))
            {
                return false;
            }

            char[] cardDigits = cardNumber.ToCharArray();
            Array.Reverse(cardDigits);

            int sum = 0;

            for (int i = 0; i < cardDigits.Length; i++)
            {
                int digit = int.Parse(cardDigits[i].ToString());

                if (i % 2 == 1)
                {
                    digit *= 2;

                    if (digit > 9)
                    {
                        digit -= 9;
                    }
                }

                sum += digit;
            }

            return sum % 10 == 0;
        }

        private bool ValidateExpiryDateFormat(string expiryDate)
        {
            DateTime parsedExpiryDate;

            if (DateTime.TryParseExact(expiryDate, "MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedExpiryDate))
            {
                DateTime currentDate = DateTime.Now;

                if (parsedExpiryDate > currentDate)
                {
                    return true;
                }
            }

            return false;
        }

        private bool ValidateCVV(int cvv)
        {
            string cvvString = cvv.ToString();
            int cvvLength = cvvString.Length;

            return cvvLength == 3;
        }

        private bool ValidateCardHolderName(string cardHolderName)
        {
            return !string.IsNullOrEmpty(cardHolderName) && cardHolderName.Length > 2;
        }

        public IActionResult ProcessPayment(int cartId, [FromBody] CardInformation cardInfo)
        {
            if (!ValidateCardNumber(cardInfo.CardNumber))
            {
                return new BadRequestObjectResult("Invalid card number!");
            }

            if (!ValidateCardHolderName(cardInfo.CardHolderName))
            {
                return new BadRequestObjectResult("Invalid Card Holder Name. Please provide a valid Name.");
            }

            if (!ValidateExpiryDateFormat(cardInfo.ExpiryDate))
            {
                return new BadRequestObjectResult("Invalid expiry date format. Please use the format 'MM/yyyy' and a valid date.");
            }

            if (!ValidateCVV(cardInfo.CVV))
            {
                return new BadRequestObjectResult("Invalid CVV. Please provide a 3-digit CVV.");
            }

            var processedCartExists = _context.ProcessedCarts.Any(pc => pc.CartId == cartId);
            if (processedCartExists)
            {
                return new BadRequestObjectResult("Payment for this cart has already been processed!");
            }

            var cart = _context.Carts.Include(c => c.Items).FirstOrDefault(c => c.Id == cartId);
            if (cart == null)
            {
                return new NotFoundObjectResult("Cart not found or invalid cart ID!");
            }

            var cartItems = cart.Items.Select(item => new
            {
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                Quantity = item.Quantity,
                Price = item.Value,
                ItemCost = item.Value * item.Quantity
            }).ToList();

            var cartItemsString = string.Join(", ", cartItems.Select(item =>
                $"ProductId: {item.ProductId}, ProductName: {item.ProductName}, Quantity: {item.Quantity}, Price: {item.Price}, ItemCost: {item.ItemCost}"));

            var totalAmount = cartItems.Sum(item => item.ItemCost);

            var processedCart = new ProcessedCart
            {
                CartId = cartId,
                TotalAmount = totalAmount,
                PaymentDate = DateTime.Now,
                Items = cartItemsString
            };

            _context.ProcessedCarts.Add(processedCart);
            _context.SaveChanges();

            // Clear the cart items for the processed cart
            cart.Items.Clear();
            _context.SaveChanges();

            return new OkObjectResult("Payment processed successfully!");

        }
    }
}
