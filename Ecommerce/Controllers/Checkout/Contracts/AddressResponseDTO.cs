﻿using Ecommerce.Models;

namespace Ecommerce.Controllers.Checkout.Contracts
{
    public class AddressResponseDTO
    {
        public int AddressId { get; set; }
        public string? Street { get; set; }
        public AddressType AddressType { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
    }
}