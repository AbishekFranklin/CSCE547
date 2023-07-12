using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OnlineShopping.Model;
using OnlineShopping.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.Globalization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OnlineShopping.Services
{
    // Inheritance: ShoppingService class inherits from the IShoppingService interface
    public class ShoppingService : IShoppingService
    {
        // Encapsulation: Private fields to encapsulate internal data
        // List of predefined categories and their associated products
        private static List<Categories> ListCategories = new List<Categories>()
        {
            new Categories()
            {
                CategoryId = 1,
                Name = "Mexican",
                Type = "Non-Vegetarian",
                Products = new List<Products>()
                {
                    new Products()
                    {
                        ProductId = 1,
                        Name = "Mushroom Pasta",
                        Price = 10.99M,
                        Description = "A type of food typically made from an unleavened dough of wheat flour mixed with water or eggs, and formed into sheets or other shapes, then cooked by boiling or baking.",
                        ProductImage = "/path"
                    },
                    new Products()
                    {
                        ProductId = 2,
                        Name = "Avocado Taco",
                        Price = 5.00M,
                        Description = "A traditional Mexican food consisting of a small hand-sized corn- or wheat-based tortilla topped with a filling.",
                        ProductImage = "/path"
                    }
                }
            },
            new Categories()
            {
                CategoryId = 2,
                Name = "Italian",
                Type = "Vegetarian",
                Products = new List<Products>()
                {
                    new Products()
                    {
                        ProductId = 3,
                        Name = "Margherita Pizza",
                        Price = 9.99M,
                        Description = "A classic Italian pizza topped with tomato sauce, mozzarella cheese, and fresh basil leaves.",
                        ProductImage = "/path"
                    },
                    new Products()
                    {
                        ProductId = 4,
                        Name = "Pasta Carbonara",
                        Price = 12.50M,
                        Description = "A pasta dish from Italy made with eggs, cheese, bacon, and black pepper.",
                        ProductImage = "/path"
                    }
                }
            },
            new Categories()
            {
                CategoryId = 3,
                Name = "American",
                Type = "Non-Vegetarian",
                Products = new List<Products>()
                {
                    new Products()
                    {
                        ProductId = 5,
                        Name = "Cheeseburger",
                        Price = 8.99M,
                        Description = "A sandwich consisting of a cooked beef patty, cheese, and various toppings.",
                        ProductImage = "/path"
                    },
                    new Products()
                    {
                        ProductId = 6,
                        Name = "BBQ Ribs",
                        Price = 15.99M,
                        Description = "Slow-cooked pork ribs coated with barbecue sauce.",
                        ProductImage = "/path"
                    }
                }
            }
        };

        // ShoppingCart that stores all the cart selections from the Cart model
        private static Cart ShoppingCart = new Cart();

        // Polymorphism: Implementing methods defined in the interface with different behavior but we haven't explicitly used
        public IActionResult GetAllItems()
        {
            // Returns a list of all products across all categories
            return new OkObjectResult(ListCategories.SelectMany(category => category.Products).ToList());
        }

        public IActionResult AddItemsToCart(int productId, int quantity)
        {
            // Check if the product ID is valid
            var product = ListCategories
                .SelectMany(category => category.Products)
                .FirstOrDefault(p => p.ProductId == productId);

            if (product == null)
            {
                return new NotFoundObjectResult("Product not found or invalid product ID!");
            }

            // Check if the product already exists in the cart
            if (ShoppingCart.Items.ContainsKey(product))
            {
                // Check if the quantity is negative
                if (quantity <= 0)
                {
                    // Calculate the remaining quantity after removing from the cart
                    int remainingQuantity = ShoppingCart.Items[product] + quantity;

                    // Check if the remaining quantity is greater than zero
                    if (remainingQuantity < 0)
                    {
                        return new BadRequestObjectResult("Invalid quantity! Quantity to remove exceeds the quantity in the cart.");
                    }
                    else if (remainingQuantity == 0)
                    {
                        return new BadRequestObjectResult("Invalid quantity! Quantity selected is zero.");
                    }
                    else
                    {
                        // Remove the specified quantity from the cart
                        ShoppingCart.Items[product] = remainingQuantity;
                        return new OkObjectResult("Product removed from cart successfully.");
                    }
                }
                else
                {
                    // Add the quantity to the existing quantity in the cart
                    ShoppingCart.Items[product] += quantity;
                    return new OkObjectResult("Cart updated successfully.");
                }
            }
            else
            {
                // The product doesn't exist in the cart, so add it as a new item
                if (quantity > 0)
                {
                    ShoppingCart.Items.Add(product, quantity);
                    return new OkObjectResult("Product added to cart successfully.");
                }
                else
                {
                    return new BadRequestObjectResult("Invalid quantity! Quantity must be a positive integer.");
                }
            }
        }

        public IActionResult CartItems()
        {
            decimal totalCost = 0;

            // Retrieve the cart items and calculate the item costs and total cost
            var cartItems = ShoppingCart.Items.Select((item, index) =>
            {
                var product = item.Key;
                int quantity = item.Value;
                // Calculate the cost of the current item by multiplying the quantity with the price
                decimal itemCost = product.Price * quantity;
                // Add the item cost to the total cost
                totalCost += itemCost;

                // An object representing the cart item with relevant properties
                return new
                {
                    ProductId = product.ProductId,
                    ProductName = product.Name,
                    Quantity = quantity,
                    Price = product.Price,
                    ItemCost = itemCost
                };
            }).ToList();

            // Return the cart items and total cost
            return new OkObjectResult(new { CartItems = cartItems, TotalCost = totalCost });
        }

        // Calculates the total cost of all the items in the shopping cart
        private decimal CalculateRegularTotal()
        {
            decimal regularTotal = 0;

            // Iterate over each item in the shopping cart
            foreach (var item in ShoppingCart.Items)
            {
                var product = item.Key;
                int quantity = item.Value;
                // Calculate the item cost by multiplying the quantity with the price
                decimal itemCost = product.Price * quantity;
                // Add the item cost to the regular total
                regularTotal += itemCost;
            }

            return regularTotal;
        }

        // Calculates the bundle cost of the items in the shopping cart
        private decimal CalculateRegularOrBundleTotal()
        {
            decimal regularTotal = CalculateRegularTotal();
            bool applyDiscount = false;

            // Iterate over each item in the shopping cart to check if the quantity is a multiple of 5
            foreach (var item in ShoppingCart.Items)
            {
                int quantity = item.Value;
                // Check if the quantity is a multiple of 5
                if (quantity >= 5 || quantity % 5 == 0) // Check if the quantity is a multiple of 5
                {
                    // Setting the applyDiscount variable to true
                    applyDiscount = true;
                    break;
                }
            }

            // Calculate the discount and subtract it from the regular total
            if (applyDiscount)
            {
                decimal discount = regularTotal * 0.15m; // 15% discount
                return regularTotal - discount;
            }

            return regularTotal; // No bundle discount
        }

        // Calculates the total cost including tax
        private decimal CalculateTotalWithTaxes()
        {
            decimal totalCost = CalculateRegularOrBundleTotal();

            // Add taxes (10% tax rate)
            decimal taxRate = 0.10m;

            // Calculate the tax amount
            decimal taxes = totalCost * taxRate;

            // Calculate the total with taxes
            totalCost += taxes;

            return totalCost;
        }

        public IActionResult GetTotal()
        {
            // Calculate the totals
            decimal regularTotal = CalculateRegularTotal();
            decimal bundleTotal = CalculateRegularOrBundleTotal();
            decimal totalWithTaxes = CalculateTotalWithTaxes();

            // Return the calculated totals
            return new OkObjectResult(new
            {
                RegularTotal = regularTotal,
                BundleTotal = bundleTotal,
                TotalWithTaxes = totalWithTaxes
            });
        }

        // Validates if the card number satisfies the Luhan's Algorithm
        private bool ValidateCardNumber(string cardNumber)
        {
            // Remove any non-digit characters from the card number
            cardNumber = new string(cardNumber.Where(char.IsDigit).ToArray());

            // Check if the card number is exactly 16 digits long
            if (cardNumber.Length != 16)
            {
                return false;
            }

            // Check if all the card numbers are digits
            if (!cardNumber.All(char.IsDigit))
            {
                return false;
            }

            // Reverse the card number for processing
            char[] cardDigits = cardNumber.ToCharArray();
            Array.Reverse(cardDigits);

            int sum = 0;

            for (int i = 0; i < cardDigits.Length; i++)
            {
                int digit = int.Parse(cardDigits[i].ToString());

                // Double every second digit
                if (i % 2 == 1)
                {
                    digit *= 2;

                    // If the doubled digit is greater than 9, subtract 9 from it
                    if (digit > 9)
                    {
                        digit -= 9;
                    }
                }

                sum += digit;
            }

            // The card number is valid if the sum is divisible by 10
            return sum % 10 == 0;
        }

        // Validates if the Expiry Date is of the format "MM/yyyy"
        private bool ValidateExpiryDateFormat(string expiryDate)
        {
            DateTime parsedExpiryDate;
            // Parse the expiry date using the specified format "MM/yyyy"
            if (DateTime.TryParseExact(expiryDate, "MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedExpiryDate))
            {
                DateTime currentDate = DateTime.Now;

                // Check if the parsed expiry date is greater than the current date
                if (parsedExpiryDate > currentDate)
                {
                    return true;
                }
            }

            return false;
        }

        // Validates if the CVV is of length 3
        private bool ValidateCVV(int cvv)
        {
            string cvvString = cvv.ToString();
            int cvvLength = cvvString.Length;

            return cvvLength == 3;
        }

        // Validates if the name character length is greater than 2
        private bool ValidateCardHolderName(string CardHolderName)
        {
            string holderName = CardHolderName.ToString();
            int cardHolderName = holderName.Length;

            return cardHolderName > 2;
        }

        public IActionResult ProcessPayment([FromBody] CardInformation cardInfo)
        {
            // Validate the card number
            if (!ValidateCardNumber(cardInfo.CardNumber))
            {
                return new BadRequestObjectResult("Invalid card number!");
            }

            // Validate the cardHolderName
            if (!ValidateCardHolderName(cardInfo.CardHolderName))
            {
                return new BadRequestObjectResult("Invalid Card Holder Name. Please provide a valid Name.");
            }

            // Validate the expiry date format
            if (!ValidateExpiryDateFormat(cardInfo.ExpiryDate))
            {
                return new BadRequestObjectResult("Invalid expiry date format. Please use the format 'MM/yyyy' and a valid date.");
            }

            // Validate the CVV format
            if (!ValidateCVV(cardInfo.CVV))
            {
                return new BadRequestObjectResult("Invalid CVV. Please provide a 3-digit CVV.");
            }

            // Clear the cart
            ShoppingCart.Items.Clear();

            // Return success response
            return new OkObjectResult("Payment processed successfully!");
        }

    }
}