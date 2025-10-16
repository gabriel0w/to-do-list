using Microsoft.EntityFrameworkCore;
using Todo.Application.Abstractions.Persistence;

namespace Todo.Infrastructure.Persistence;

public interface IUnitOfWork<TDb> : IUnitOfWork where TDb : DbContext
{
}
