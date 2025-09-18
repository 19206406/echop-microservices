namespace Basket.API.Basket.StoreBasket
{
    public record StoreBasketCommmand(ShoppingCart Cart) : ICommand<StoreBasketResult>; 
    public record StoreBasketResult(string UserName); 
    
    public class StoreBasketCommandValidator : AbstractValidator<StoreBasketCommmand>
    {
        public StoreBasketCommandValidator()
        {
            RuleFor(x => x.Cart).NotNull().WithMessage("Cart can not be null");
            RuleFor(x => x.Cart.UserName).NotEmpty().WithMessage("UserName is required"); 
        }
    }

    public class StoreBasketCommandHandler(IBasketRepository repository)
        : ICommandHandler<StoreBasketCommmand, StoreBasketResult>
    {
        public async Task<StoreBasketResult> Handle(StoreBasketCommmand command, CancellationToken cancellationToken)
        {
            ShoppingCart cart = command.Cart;

            //TODO: store basket in database (use Marten upsert - if exist = update, if not = insert) 
            //TODO: update cache
            await repository.StoreBasket(command.Cart, cancellationToken); 

            return new StoreBasketResult(command.Cart.UserName); 
        }
    }
}
