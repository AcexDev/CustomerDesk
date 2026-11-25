using Microsoft.AspNetCore.Mvc;

namespace CustomerDesk.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _service;

    public CustomersController(ICustomerService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult<Customer>> Register(CustomerRequest request)
    {
        Customer? existing = await _service.GetByEmailAsync(request.Email);
        if (existing != null){
            return Problem(statusCode: StatusCodes.Status409Conflict,
                title: "Conflict", detail: "A customer with this email already exists.");
        }

        Customer? existingPhone = await _service.GetByPhoneAsync(request.Phone);
        if (existingPhone != null){
            return Problem(statusCode: StatusCodes.Status409Conflict,
                title: "Conflict", detail: "A customer with this phone number already exists.");
        }

        Customer created = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpGet]
    public async Task<ActionResult<List<Customer>>> Search(
        [FromQuery] string? name,
        [FromQuery] string? email)
    {
        List<Customer> customers = await _service.SearchAsync(name, email);

        return Ok(customers);
    }
    [HttpGet("{id}")]
    public async Task<ActionResult<Customer>> GetById(int id)
    {
        Customer? customer = await _service.GetByIdAsync(id);
        if (customer == null)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound,
                title: "Customer not found", detail: $"No active customer has ID {id}.");
        }
        return Ok(customer);
    }

    [HttpGet("availability")]
public async Task<IActionResult> CheckAvailability(
    [FromQuery] string? email,
    [FromQuery] string? phone)
{
    if (string.IsNullOrWhiteSpace(email) &&
        string.IsNullOrWhiteSpace(phone))
    {
        return Problem(statusCode: StatusCodes.Status400BadRequest,
            title: "Invalid request", detail: "Provide an email, a phone number, or both.");
    }

    bool emailAvailable = true;
    bool phoneAvailable = true;

    if (!string.IsNullOrWhiteSpace(email))
    {
        Customer? existingEmail = await _service.GetByEmailAsync(email);
        emailAvailable = existingEmail == null;
    }

    if (!string.IsNullOrWhiteSpace(phone))
    {
        Customer? existingPhone = await _service.GetByPhoneAsync(phone);
        phoneAvailable = existingPhone == null;
    }

    return Ok(new
    {
        emailAvailable,
        phoneAvailable
    });
}

    [HttpPut("{id}")]
    public async Task<ActionResult<Customer>> Update(int id, CustomerRequest request)
    {
        Customer? sameEmail = await _service.GetByEmailAsync(request.Email);
        if (sameEmail != null && sameEmail.Id != id)
        {
            return Problem(statusCode: StatusCodes.Status409Conflict,
                title: "Conflict", detail: "Another customer already uses this email.");
        }

        Customer? samePhone = await _service.GetByPhoneAsync(request.Phone);
        if (samePhone != null && samePhone.Id != id)
        {
            return Problem(statusCode: StatusCodes.Status409Conflict,
                title: "Conflict", detail: "Another customer already uses this phone number.");
        }

        Customer? updated = await _service.UpdateAsync(id, request);
        if (updated == null)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound,
                title: "Customer not found", detail: $"No active customer has ID {id}.");
        }
        return Ok(updated);
    }

    [HttpPatch("{id}")]
    public async Task<ActionResult<Customer>> Patch(
        int id,
        CustomerPatchRequest request)
    {
        if (request.FullName != null && string.IsNullOrWhiteSpace(request.FullName))
        {
            return BadRequest(new ValidationProblemDetails(new Dictionary<string, string[]>
            {
                [nameof(request.FullName)] = ["Full name cannot be empty."]
            }));
        }

        if (request.Email != null && string.IsNullOrWhiteSpace(request.Email))
        {
            return BadRequest(new ValidationProblemDetails(new Dictionary<string, string[]>
            {
                [nameof(request.Email)] = ["Email cannot be empty."]
            }));
        }

        if (request.Phone != null && string.IsNullOrWhiteSpace(request.Phone))
        {
            return BadRequest(new ValidationProblemDetails(new Dictionary<string, string[]>
            {
                [nameof(request.Phone)] = ["Phone number cannot be empty."]
            }));
        }

        Customer? current = await _service.GetByIdAsync(id);
        if (current == null)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound,
                title: "Customer not found", detail: $"No active customer has ID {id}.");
        }

        if (request.Email != null)
        {
            Customer? sameEmail = await _service.GetByEmailAsync(request.Email);
            if (sameEmail != null && sameEmail.Id != id)
            {
                return Problem(statusCode: StatusCodes.Status409Conflict,
                    title: "Conflict", detail: "Another customer already uses this email.");
            }
        }

        if (request.Phone != null)
        {
            Customer? samePhone = await _service.GetByPhoneAsync(request.Phone);
            if (samePhone != null && samePhone.Id != id)
            {
                return Problem(statusCode: StatusCodes.Status409Conflict,
                    title: "Conflict", detail: "Another customer already uses this phone number.");
            }
        }

        Customer? updated = await _service.PatchAsync(id, request);
        if (updated == null)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound,
                title: "Customer not found", detail: $"No active customer has ID {id}.");
        }

        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        bool deleted = await _service.DeleteAsync(id);
        if (!deleted)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound,
                title: "Customer not found", detail: $"No active customer has ID {id}.");
        }
        return NoContent();
    }
}
