using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace PeerReviewApp.Data
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected ApplicationDbContext context { get; set; }
        private DbSet<T> DbSet { get; set; }

        public Repository(ApplicationDbContext ctx)
        {
            context = ctx;
            DbSet = context.Set<T>();
        }

        public async Task AddAsync(T entity)
        {
            await DbSet.AddAsync(entity);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(T entity)
        {
            DbSet.Remove(entity);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            DbSet.Update(entity);
            await context.SaveChangesAsync();
        }
    }
}
