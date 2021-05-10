using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.EF.Repositories
{
    public abstract class BaseRepository<T> : IRepository where T : DbContext
    {
        protected readonly T _context;

        public BaseRepository(T context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void ExecuteSqlScript(string sql)
        {
            _context.Database.ExecuteSqlRaw(sql);
        }
    }
}
