using Microsoft.EntityFrameworkCore;
using NetDevPack.Data;
using Solucao.Application.Data.Entities;
using Solucao.Application.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solucao.Application.Data.Repositories
{
    public class ClientRepository :  IClientRepository
    {
        public IUnitOfWork UnitOfWork => Db;
        protected readonly SolucaoContext Db;
        protected readonly DbSet<Client> DbSet;

        public ClientRepository(SolucaoContext _context)
        {
            Db = _context;
            DbSet = Db.Set<Client>();
        }

        public async Task<IEnumerable<Client>> GetAll(bool ativo, string search)
        {
            if (string.IsNullOrEmpty(search))
                return await Db.Clients.Include(x => x.City).Include(x => x.State).Where(x => x.Active == ativo).OrderBy(x => x.Name).ToListAsync();

            return await Db.Clients.Include(x => x.City).Include(x => x.State).Where(x => x.Active == ativo && (x.Address.Contains(search) || x.Email.Contains(search) || x.Name.Contains(search) || x.Phone.Contains(search) || x.CellPhone.Contains(search))).OrderBy(x => x.Name).ToListAsync();
        }

        public async Task<Client> GetById(Guid Id)
        {
            return await Db.Clients.FindAsync(Id);
        }

        public async Task<ValidationResult> Add(Client client)
        {
            try
            {

                Db.Clients.Add(client);
                await Db.SaveChangesAsync();
                return ValidationResult.Success;
            }
            catch (Exception e)
            {
                throw new Exception(e.InnerException.Message);
            }
        }

        public async Task<ValidationResult> Update(Client client)
        {
            try
            {
                DbSet.Update(client);
                await Db.SaveChangesAsync();
                return ValidationResult.Success;
            }
            catch (Exception e)
            {
                throw new Exception(e.InnerException.Message);
            }

        }
    }
}
