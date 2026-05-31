using BookStore.Shared.Common.Results;

namespace BookStore.Catalog.Application.Errors
{
   
    public static class CatalogErrors
    {
        public static Error BookNotFound(Guid id)
            => new(
                "Book.NotFound",
                $"Book '{id}' was not found.",
                ErrorType.NotFound);
    }
}
