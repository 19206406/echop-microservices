namespace Catalog.API.Products.CreateProduct
{
    // Datos que necesitamos para crear el producto 
    public record CreateProductCommand(string Name, List<string> Category, string Description, string ImageFile, decimal Price)
        : ICommand<CreateProductResult>; 
    
    // lo que devuelve la creación 
    public record CreateProductResult(Guid id);

    internal class CreateProductCommandHandler(IDocumentSession session) :
        ICommandHandler<CreateProductCommand, CreateProductResult>
    {
        public async Task<CreateProductResult> Handle(CreateProductCommand command, CancellationToken cancellationToken)
        {
            // create Product entity from command object 
            // save to database 
            // return CreateProductResult result 

            var product = new Product
            {
                Name = command.Name,
                Category = command.Category,
                Description = command.Description,
                ImageFile = command.ImageFile,
                Price = command.Price,
            };

            // save to database 
            session.Store(product);
            await session.SaveChangesAsync(cancellationToken); 

            // return result 

            return new CreateProductResult(product.Id); 
        }
    }
}
