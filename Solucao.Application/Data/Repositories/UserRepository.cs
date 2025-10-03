using Microsoft.EntityFrameworkCore;
using Solucao.Application.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using NetDevPack.Data;
using Microsoft.Extensions.Logging;

namespace Solucao.Application.Data.Repositories
{
    public class UserRepository 
    {
        public IUnitOfWork UnitOfWork => Db;
        protected readonly SolucaoContext Db;
        protected readonly DbSet<User> DbSet;
        private readonly ILogger<UserRepository> logger;

        public UserRepository(SolucaoContext _context, ILogger<UserRepository> _logger)
        {
            Db = _context;
            DbSet = Db.Set<User>();
            logger = _logger;
        }

        public virtual async Task<IEnumerable<User>> GetAll()
        {
            return await Db.Users.ToListAsync();
        }

        public virtual async Task<User> GetById(Guid Id)
        {
            return await Db.Users.FindAsync(Id);
        }


        public virtual async Task<User> GetByName(string name)
        {
            return await Db.Users.FirstOrDefaultAsync(x => x.Name == name);
        }

        public virtual async Task<ValidationResult> Add(User user)
        {
            try
            {
                await Db.Users.AddAsync(user);
                Db.SaveChanges();
                return ValidationResult.Success;
            }
            catch (Exception e)
            {
                logger.LogError(e.StackTrace);
                return new ValidationResult(e.Message);
            }
        }

        
        public virtual async Task<ValidationResult> Update(User user)
        {
            try
            {
                DbSet.Update(user);
                await Db.SaveChangesAsync();
                return ValidationResult.Success;
            }
            catch (Exception e)
            {
                logger.LogError(e.StackTrace);
                return new ValidationResult(e.Message);
            }
        }

        public virtual async Task<User> GetByEmail(string email)
        {
            return await Db.Users.FirstOrDefaultAsync(x => x.Email == email && x.Active);

        }

        public virtual async Task<User> GetByToken(string token)
        {
            return await Db.Users.FirstOrDefaultAsync(x => x.Token == token && x.Active);

        }

    }
}
