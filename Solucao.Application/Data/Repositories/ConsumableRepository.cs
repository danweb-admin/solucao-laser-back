using System;
using Microsoft.EntityFrameworkCore;
using NetDevPack.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Solucao.Application.Data.Entities;

namespace Solucao.Application.Data.Repositories
{
	public class ConsumableRepository
	{
        public IUnitOfWork UnitOfWork => Db;
        protected readonly SolucaoContext Db;
        protected readonly DbSet<Consumable> DbSet;

        public ConsumableRepository(SolucaoContext _context)
        {
            Db = _context;
            DbSet = Db.Set<Consumable>();
        }

        public async Task<IEnumerable<Consumable>> GetAll()
        {
            return await DbSet.OrderBy(x => x.Name).ToListAsync();
        }

        public async Task<Consumable> Add(Consumable consumable)
        {
            try
            {

                Db.Consumables.Add(consumable);
                await Db.SaveChangesAsync();
                return consumable;
            }
            catch (Exception e)
            {
                throw new Exception(e.InnerException.Message);
            }
        }

        public async Task<Consumable> Update(Consumable consumable)
        {
            try
            {
                Db.Consumables.Update(consumable);
                await Db.SaveChangesAsync();

                return consumable;
            }
            catch (Exception e)
            {
                throw new Exception(e.InnerException.Message);
            }

        }
    }
}

