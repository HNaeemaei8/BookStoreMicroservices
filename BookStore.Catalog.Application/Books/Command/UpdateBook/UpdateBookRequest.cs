
namespace BookStore.Catalog.Application.Books.Command
{
    public class UpdateBookRequest
    {
        public string Title { get; set; } = string.Empty;

        public string Author { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int Stock { get; set; }
    }
}
