﻿using AutoMapper;
using Ecommerce.Controllers.Checkout.Contracts;
using Ecommerce.Data;
using Ecommerce.Models;
using Ecommerce.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Services.Checkout
{

    public class CheckoutService : ICheckoutService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;


        public CheckoutService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<AddressResponseDTO>> GetAddressesAsync(string userId)
        {
            try
            {
                var Addresses = await _context.Addresses
                .Where(o => o.UserId == userId)
                .ToListAsync();
                if (Addresses.Count == 0)
                {
                    throw new ArgumentException("No addresses found for this user");
                }

                return _mapper.Map<List<AddressResponseDTO>>(Addresses);
            }
            catch(ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting addresses", ex);
            }
            
        }
        public async Task<AddressResponseDTO> GetAddressByIdAsync(string userId,int addressId)
        {
            try
            {
                var address = await _context.Addresses
                .FirstOrDefaultAsync(o => o.AddressId == addressId) ?? throw new ArgumentException("Address not found");
                if (address.UserId != userId)
                {
                    throw new UnauthorizedAccessException("Address does not belong to user");
                }
                return _mapper.Map<AddressResponseDTO>(address);
            }
            catch(UnauthorizedAccessException)
            {
                throw;
            }
            catch(ArgumentException)
            {
                throw; 
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting address", ex);
            }
            
        }


        public async Task<AddressResponseDTO> AddAddressAsync(string userId, AddressRequestDTO address, string addressType)
        {
            try
            {
                var newAddress = new Address
                {
                    UserId = userId,
                    AddressType = addressType == "shipping" ? AddressType.Shipping : AddressType.Billing,
                    Street = address.Street,
                    City = address.City,
                    State = address.State,
                    Country = address.Country,
                    PostalCode = address.PostalCode
                };
                _context.Addresses.Add(newAddress);
                await _context.SaveChangesAsync();
                return _mapper.Map<AddressResponseDTO>(newAddress);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while entering shipping address", ex);
            }
        }
        public async Task<AddressResponseDTO> UpdateAddressAsync(string userId, UpdateAddressRequestDTO address)
        {
            try
            {
                var existingAddress = await _context.Addresses
                .FirstOrDefaultAsync(o => o.AddressId == address.AddressId) ?? throw new ArgumentException("Address not found");
                if (existingAddress.UserId != userId)
                {
                    throw new UnauthorizedAccessException("Address does not belong to user");
                }
                existingAddress.Street = address.Street;
                existingAddress.City = address.City;
                existingAddress.State = address.State;
                existingAddress.Country = address.Country;
                existingAddress.PostalCode = address.PostalCode;
                await _context.SaveChangesAsync();
                return _mapper.Map<AddressResponseDTO>(existingAddress);
            }
            catch(UnauthorizedAccessException)
            {
                throw;
            }
            catch(ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating address", ex);
            }
        }

        public async Task RemoveAddressAsync(string userId, int addressId)
        {
            try
            {
                var address = await _context.Addresses
                .FirstOrDefaultAsync(o => o.AddressId == addressId) ?? throw new ArgumentException("Address not found");
                if (address.UserId != userId)
                {
                    throw new UnauthorizedAccessException("Address does not belong to user");
                }
                _context.Addresses.Remove(address);
                await _context.SaveChangesAsync();
            }
            catch(UnauthorizedAccessException)
            {
                throw;
            }
            catch(ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while removing address", ex);
            }
        }

        public async Task RemoveMultipleAddresses(string userId, List<int> addressIds)
        {
            try
            {
                foreach (var addressId in addressIds)
                {
                    await RemoveAddressAsync(userId, addressId);
                }
            }
            catch(UnauthorizedAccessException)
            {
                throw;
            }
            catch(ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while removing addresses", ex);
            }
        }

        public async Task ClearAddressesAsync(string userId)
        {
            try
            {
                var addresses = await _context.Addresses
                .Where(o => o.UserId == userId)
                .ToListAsync();
                if (addresses.Count == 0)
                {
                    throw new ArgumentException("No addresses found for this user");
                }
                _context.Addresses.RemoveRange(addresses);
                await _context.SaveChangesAsync();
            }
            catch(ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while clearing addresses", ex);
            }
        }
        
   
    }

}
