using System;
using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.Models
{
    public class CardInformation
    {
        // Encapsulation - Property with private setter

        [Required(ErrorMessage = "Card number is required.")]
        public string CardNumber { get; private set; }

        [Required(ErrorMessage = "Expiry date is required.")]
        public string ExpiryDate { get; private set; }

        [Required(ErrorMessage = "Card holder name is required.")]
        public string CardHolderName { get; private set; }

        [Required(ErrorMessage = "CVV is required.")]
        public int CVV { get; private set; }

        public CardInformation(string cardNumber, string expiryDate, string cardHolderName, int cvv)
        {
            // Encapsulation - Assigning value through constructor

            CardNumber = cardNumber;
            ExpiryDate = expiryDate;
            CardHolderName = cardHolderName;
            CVV = cvv;
        }
    }
}


