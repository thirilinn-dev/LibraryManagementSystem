using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LibraryManagementSystem.Domain.Features.Borrower;
using LibraryManagementSystem.Domain.Models.Borrower;
using LibraryManagementSystem.Domain.Models.Borrowing;

namespace LibraryManagementSystem.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BorrowerController : ControllerBase
{
    private readonly IBorrowerService _borrowerService;

    public BorrowerController(IBorrowerService borrowerService)
    {
        _borrowerService = borrowerService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BorrowerDto>>> GetBorrowers()
    {
        var borrowers = await _borrowerService.GetBorrowersAsync();
        return Ok(borrowers);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BorrowerDto>> GetBorrower(int id)
    {
        var borrower = await _borrowerService.GetBorrowerByIdAsync(id);
        if (borrower == null)
        {
            return NotFound(new { message = $"Borrower with ID {id} not found." });
        }
        return Ok(borrower);
    }

    [HttpPost]
    public async Task<ActionResult<BorrowerDto>> RegisterBorrower([FromBody] CreateBorrowerDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var createdBorrower = await _borrowerService.RegisterBorrowerAsync(dto);
            return CreatedAtAction(nameof(GetBorrower), new { id = createdBorrower.BorrowerId }, createdBorrower);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPatch("{id}")]
    public async Task<ActionResult<BorrowerDto>> UpdateBorrower(int id, [FromBody] UpdateBorrowerDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var updatedBorrower = await _borrowerService.UpdateBorrowerAsync(id, dto);
            if (updatedBorrower == null)
            {
                return NotFound(new { message = $"Borrower with ID {id} not found." });
            }
            return Ok(updatedBorrower);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBorrower(int id)
    {
        try
        {
            var result = await _borrowerService.DeleteBorrowerAsync(id);
            if (!result)
            {
                return NotFound(new { message = $"Borrower with ID {id} not found." });
            }
            return NoContent();
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

    [HttpGet("{id}/history")]
    public async Task<ActionResult<IEnumerable<BorrowingDto>>> GetBorrowingHistory(int id)
    {
        var history = await _borrowerService.GetBorrowingHistoryAsync(id);
        return Ok(history);
    }
}
