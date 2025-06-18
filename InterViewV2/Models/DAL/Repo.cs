using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace InterViewV2.Models.DAL
{
    public class Repo
    {
        private bool track = false;

        public Repo(DB_Context db, IConfiguration configuration)
        {
            DB = db;
            ConnectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public string ConnectionString { get; set; }
        public DB_Context DB { get; }

        public void EnableTracking(bool enable = true)
        {
            track = enable;
            DB.ChangeTracker.AutoDetectChangesEnabled = enable;
            DB.ChangeTracker.QueryTrackingBehavior = (enable) ? QueryTrackingBehavior.TrackAll : QueryTrackingBehavior.NoTracking;
        }

        public async Task ExecuteNonQueryAsync(string procedure, params (string name, object value)[] parameters)
        {
            using var connection = new SqlConnection(ConnectionString);
            using var command = new SqlCommand(procedure, connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure,
                CommandTimeout = 300
            };

            foreach (var (name, value) in parameters)
            {
                command.Parameters.Add(new SqlParameter(name, value));
            }

            connection.Open();

            await command.ExecuteNonQueryAsync();
        }
        public IQueryable<TEntity> Get<TEntity>() where TEntity : class
        {
            if (track)
            {
                return DB.Set<TEntity>();
            }

            return DB.Set<TEntity>().AsNoTracking();
        }
        public IQueryable<TEntity> Get<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            return Get<TEntity>().Where(predicate);
        }
        public async Task<TEntity> Add<TEntity>(TEntity entity) where TEntity : class
        {
            return await Save(DB.Add(entity));
        }
        public async Task<TEntity> Update<TEntity>(TEntity entity) where TEntity : class
        {
            return await Save(DB.Update(entity));
        }
        public async Task<TEntity> Remove<TEntity>(TEntity entity) where TEntity : class
        {
            return await Save(DB.Remove(entity));
        }
        private async Task<TEntity> Save<TEntity>(EntityEntry<TEntity> entity) where TEntity : class
        {
            await DB.SaveChangesAsync();

            if (!track)
            {
                entity.State = EntityState.Detached;
            }

            return entity.Entity;
        }
    }
}
