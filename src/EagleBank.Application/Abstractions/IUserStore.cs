using EagleBank.Domain;

namespace EagleBank.Application.Abstractions;

public interface IUserStore
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);

    Task AddAsync(User user, CancellationToken cancellationToken);
}
