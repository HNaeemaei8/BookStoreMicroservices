using BookStore.Catalog.Application.Books.Command;
using BookStore.Catalog.Application.Books.Query;
using BookStore.Catalog.Application.Command;
using BookStore.Catalog.Application.Request;
using BookStore.Shared.API.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Catalog.API.Controllers;

[ApiController]
[Route("api/books")]
public class BooksController : ControllerBase
{
    private readonly ISender _sender;

    public BooksController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateBookRequest request,CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new CreateBookCommand(request.Title,request.Author,request.Price,request.Stock),cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id,CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetBookByIdQuery(id),cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id,UpdateBookRequest request,CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new UpdateBookCommand(id,request.Title,request.Author,request.Price,request.Stock),cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id,CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeleteBookCommand(id),cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result =await _sender.Send(new GetAllBooksQuery(),cancellationToken);
        return this.ToActionResult(result);
    }
}