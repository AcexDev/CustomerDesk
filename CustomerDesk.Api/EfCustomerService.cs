using Microsoft.EntityFrameworkCore;

namespace CustomerDesk.Api;

using CustomerDesk.Api.Models;

public class EfCustomerService : ICustomerService
{
    private readonly AppDbContext _db;

    public EfCustomerService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Customer>> SearchAsync(
        string? name,
        string? email)
    {
        IQueryable<Customer> query = _db.Customers;

        if (!string.IsNullOrWhiteSpace(name))
        {
            string nameFilter = name.Trim();
            query = query.Where(c => c.FullName.Contains(nameFilter));
        }

        if (!string.IsNullOrWhiteSpace(email))
        {
            string emailFilter = email.Trim().ToLowerInvariant();
            query = query.Where(c => c.Email.Contains(emailFilter));
        }

        return await query
            .OrderBy(c => c.FullName)
            .ThenBy(c => c.Id)
            .ToListAsync();
    }
    public async Task<Customer?> GetByIdAsync(int id)
    {
        return await _db.Customers.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Customer?> GetByEmailAsync(string email)
    {
        string normalized = email.Trim().ToLowerInvariant();
        return await _db.Customers.FirstOrDefaultAsync(c => c.Email == normalized);
    }

    public async Task<Customer?> GetByPhoneAsync(string phone)
    {
        string normalized = phone.Trim();
        return await _db.Customers.FirstOrDefaultAsync(c => c.Phone == normalized);
    }

    public async Task<Customer> CreateAsync(CustomerRequest request)
    {
        Customer customer = new Customer();
        customer.FullName = request.FullName;
        customer.Email = request.Email.Trim().ToLowerInvariant();
        customer.Phone = request.Phone.Trim();
        customer.CreatedAt = DateTime.UtcNow;

        _db.Customers.Add(customer);
        await _db.SaveChangesAsync();
        return customer;
    }

    public async Task<Customer?> UpdateAsync(int id, CustomerRequest request)
    {
        Customer? customer = await _db.Customers.FirstOrDefaultAsync(c => c.Id == id);
        if (customer == null)
        {
            return null;
        }

        customer.FullName = request.FullName;
        customer.Email = request.Email.Trim().ToLowerInvariant();
        customer.Phone = request.Phone.Trim();
        await _db.SaveChangesAsync();
        return customer;
    }

    public async Task<Customer?> PatchAsync(int id, CustomerPatchRequest request)
    {
        Customer? customer = await _db.Customers.FirstOrDefaultAsync(c => c.Id == id);
        if (customer == null)
        {
            return null;
        }

        if (request.FullName != null)
        {
            customer.FullName = request.FullName;
        }

        if (request.Email != null)
        {
            customer.Email = request.Email.Trim().ToLowerInvariant();
        }

        if (request.Phone != null)
        {
            customer.Phone = request.Phone.Trim();
        }

        await _db.SaveChangesAsync();
        return customer;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        Customer? customer = await _db.Customers.FirstOrDefaultAsync(c => c.Id == id);
        if (customer == null)
        {
            return false;
        }

        customer.IsDeleted = true;
        await _db.SaveChangesAsync();
        return true;
    }
}
