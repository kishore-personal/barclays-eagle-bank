using EagleBank.Application.Abstractions;
using EagleBank.Domain;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace EagleBank.Infrastructure.Persistence;

public sealed class UserStore : IUserStore
{
    private readonly EagleBankDbContext _db;

    public UserStore(EagleBankDbContext db)
    {
        _db = db;
    }

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return _db.Users.AsNoTracking().SingleOrDefaultAsync(user => user.Email == email, cancellationToken);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken)
    {
        _db.Users.Add(user);
        try
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (IsUniqueConstraint(exception))
        {
            throw new ValidationException([
                new ValidationFailure("email", "Email is already registered.") { ErrorCode = "Duplicate" }
            ]);
        }
    }

    private static bool IsUniqueConstraint(DbUpdateException exception)
    {
        return exception.InnerException is SqliteException sqlite && sqlite.SqliteErrorCode == 19;
    }
}
