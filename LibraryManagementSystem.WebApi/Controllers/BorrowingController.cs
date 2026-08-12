using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LibraryManagementSystem.Domain.Features.Borrowing;
using LibraryManagementSystem.Domain.Models.Borrowing;

namespace LibraryManagementSystem.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BorrowingController : ControllerBase
{
    private readonly IBorrowingService _borrowingService;

    public BorrowingController(IBorrowingService borrowingService)
    {
        _borrowingService = borrowingService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BorrowingDto>>> GetBorrowings()
    {
        var borrowings = await _borrowingService.GetBorrowingsAsync();
        return Ok(borrowings);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BorrowingDto>> GetBorrowing(int id)
    {
        var borrowing = await _borrowingService.GetBorrowingByIdAsync(id);
        if (borrowing == null)
        {
            return NotFound(new { message = $"Borrowing record with ID {id} not found." });
        }
        return Ok(borrowing);
    }

    [HttpPost]
    public async Task<ActionResult<BorrowingDto>> LendBook([FromBody] CreateBorrowingDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var created = await _borrowingService.LendBookAsync(dto);
            return CreatedAtAction(nameof(GetBorrowing), new { id = created.BorrowId }, created);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{id}/return")]
    public async Task<ActionResult<BorrowingDto>> ReturnBook(int id)
    {
        try
        {
            var updated = await _borrowingService.ReturnBookAsync(id);
            if (updated == null)
            {
                return NotFound(new { message = $"Borrowing record with ID {id} not found." });
            }
            return Ok(updated);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("overdue")]
    public async Task<ActionResult<IEnumerable<BorrowingDto>>> GetOverdueBorrowings()
    {
        var overdue = await _borrowingService.GetOverdueBorrowingsAsync();
        return Ok(overdue);
    }
}
