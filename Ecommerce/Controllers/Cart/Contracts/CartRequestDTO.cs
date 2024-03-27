namespace Ecommerce.Controllers.Cart.Contracts
{
    public class AddToCartRequest 
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class AddMultipleItemsToCartRequest
    {
        public List<AddToCartRequest> Items { get; set; }
    }

    public class RemoveFromCartRequest
    {
        public int ProductId { get; set; }
    }


    public class UpdateCartItemQuantityRequest
    {
        public int ProductId { get; set; }
        public int NewQuantity { get; set; }
    }




    public class DeleteCartItemsRequest
    {
        public List<int>? CartItemIds { get; set; }
    }

}
