namespace CustomerDesk.Api;

public interface ICustomerService
{
    Task<List<Customer>> SearchAsync(
    string? name,
    string? email);
    Task<Customer?> GetByIdAsync(int id);
    Task<Customer?> GetByEmailAsync(string email);
    Task<Customer?> GetByPhoneAsync(string phone);
    Task<Customer> CreateAsync(CustomerRequest request);
    Task<Customer?> UpdateAsync(int id, CustomerRequest request);
    Task<Customer?> PatchAsync(int id, CustomerPatchRequest request);
    Task<bool> DeleteAsync(int id);
}
