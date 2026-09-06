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

    public Task<User?> GetByIdAsync(string userId, CancellationToken cancellationToken)
    {
        return _db.Users.AsNoTracking().SingleOrDefaultAsync(user => user.Id == userId, cancellationToken);
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

    public async Task UpdateAsync(User user, CancellationToken cancellationToken)
    {
        _db.Users.Update(user);
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

    public async Task DeleteAsync(string userId, CancellationToken cancellationToken)
    {
        await _db.Users.Where(user => user.Id == userId).ExecuteDeleteAsync(cancellationToken);
    }

    private static bool IsUniqueConstraint(DbUpdateException exception)
    {
        return exception.InnerException is SqliteException sqlite && sqlite.SqliteErrorCode == 19;
    }
}
