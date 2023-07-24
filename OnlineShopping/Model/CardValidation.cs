using System;
using System.Globalization;
using System.Linq;

namespace OnlineShopping.Models
{
    public class CardValidation
    {
        // Validates if the card number satisfies the Luhan's Algorithm
        public bool ValidateCardNumber(string cardNumber)
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
        public bool ValidateExpiryDateFormat(string expiryDate)
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
        public bool ValidateCVV(int cvv)
        {
            string cvvString = cvv.ToString();
            int cvvLength = cvvString.Length;

            return cvvLength == 3;
        }

        // Validates if the name character length is greater than 2
        public bool ValidateCardHolderName(string cardHolderName)
        {
            return !string.IsNullOrEmpty(cardHolderName) && cardHolderName.Length > 2;
        }
    }
}
