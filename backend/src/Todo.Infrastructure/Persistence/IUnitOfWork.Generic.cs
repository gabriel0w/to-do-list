using Microsoft.EntityFrameworkCore;
using Todo.Application.Common.Persistence;

namespace Todo.Infrastructure.Persistence;

public interface IUnitOfWork<TDb> : IUnitOfWork where TDb : DbContext
{
}

