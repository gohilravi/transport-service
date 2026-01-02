using Microsoft.AspNetCore.Mvc;
using TransportService.Core.DTOs;
using TransportService.Core.Interfaces;
using FluentValidation;

namespace TransportService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransportController : ControllerBase
{
    private readonly ITransportService _transportService;
    private readonly IValidator<CreateTransportRequest> _createValidator;
    private readonly IValidator<UpdateTransportStatusRequest> _updateValidator;

    public TransportController(
        ITransportService transportService,
        IValidator<CreateTransportRequest> createValidator,
        IValidator<UpdateTransportStatusRequest> updateValidator)
    {
        _transportService = transportService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    /// <summary>
    /// Creates a new transport
    /// </summary>
    /// <param name="request">Transport creation request</param>
    /// <returns>Transport ID</returns>
    [HttpPost]
    [ProducesResponseType(typeof(CreateTransportResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CreateTransportResponse>> CreateTransport([FromBody] CreateTransportRequest request)
    {
        try
        {
            var validationResult = await _createValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => new { Field = e.PropertyName, Error = e.ErrorMessage }));
            }

            var transport = await _transportService.CreateTransportAsync(request, request.ElasticSearchId);
            return CreatedAtRoute(null, new { id = transport.Id }, transport);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { Error = "An error occurred while creating the transport.", Details = ex.Message });
        }
    }

    /// <summary>
    /// Updates transport status
    /// </summary>
    /// <param name="id">Transport ID</param>
    /// <param name="request">Status update request</param>
    /// <returns>Success confirmation</returns>
    [HttpPatch("{id}/status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> UpdateTransportStatus(int id, [FromBody] UpdateTransportStatusRequest request)
    {
        try
        {
            var validationResult = await _updateValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => new { Field = e.PropertyName, Error = e.ErrorMessage }));
            }

            await _transportService.UpdateTransportStatusAsync(id, request.Status, request.ElasticSearchId);
            return Ok();
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { Error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { Error = "An error occurred while updating the transport status.", Details = ex.Message });
        }
    }
}