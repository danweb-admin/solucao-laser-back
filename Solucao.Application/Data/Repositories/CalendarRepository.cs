﻿using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using NetDevPack.Data;
using Solucao.Application.Contracts.Response;
using Solucao.Application.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solucao.Application.Data.Repositories
{
    public class CalendarRepository
    {
        public IUnitOfWork UnitOfWork => Db;
        protected readonly SolucaoContext Db;
        protected readonly DbSet<Calendar> DbSet;
        private List<string> notIn = new List<string> { "4" };

        public CalendarRepository(SolucaoContext _context)
        {
            Db = _context;
            DbSet = Db.Set<Calendar>();
        }

        public async Task<IEnumerable<Calendar>> GetAll(DateTime date, User user)
        {
            if (user.Role == "driver")
            {
                var result = await Db.Calendars
                .Include(x => x.Equipament)
                .Include(x => x.Client.City)
                .Include(x => x.Client)
                .Include(x => x.Driver)
                .Include(x => x.DriverCollects)
                .Include(x => x.Technique)
                .Include(x => x.User)
                .Include(x => x.CalendarSpecifications)
                .Where(x => x.Date.Date == date && x.Active && !notIn.Contains(x.Status))
                .Join(Db.People,
                      c => c.DriverId,
                      p => p.Id,
                      (c, p) => new { Calendar = c, Person = p })
                .Join(Db.Users,
                      cp => cp.Person.UserId,
                      u => u.Id,
                      (cp, u) => new { cp.Calendar, cp.Person, User = u })
                .Where(cp => cp.User.Id == user.Id)
                .Select(cp => cp.Calendar)
                .ToListAsync();

                return result;
            }


            return await Db.Calendars.Include(x => x.Equipament)
                                         .Include(x => x.Client.City)
                                         .Include(x => x.Client)
                                         .Include(x => x.Driver)
                                         .Include(x => x.DriverCollects)
                                         .Include(x => x.Technique)
                                         .Include(x => x.User)
                                         .Include(x => x.CalendarSpecifications)
                                         .Where(x => x.Date.Date == date && x.Active && !notIn.Contains(x.Status))
                                         .OrderBy(x => x.Status).ToListAsync();

        }

        public async Task<IEnumerable<Calendar>> GetAllByDayAndConfirmed(DateTime date)
        {
            var confirmed = "1";

            return await Db.Calendars
                        .Include(x => x.Equipament)
                        .Include(x => x.Client)
                        .Where(x => x.Date.Date == date && x.Active && x.Status == confirmed)
                        .OrderBy(x => x.Equipament.Name)
                        .ToListAsync();

        }

        public async Task<Calendar> GetById(Guid id)
        {
            return await Db.Calendars
                        .Include(x => x.Equipament)
                        .Include(x => x.Client)
                        .Include(x => x.CalendarSpecifications)
                        .Include("CalendarSpecifications.Specification")
                        .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<ValidationResult> Add(Calendar calendar)
        {
            try
            {

                Db.Calendars.Add(calendar);
                await Db.SaveChangesAsync();
                return ValidationResult.Success;
            }
            catch (Exception e)
            {
                throw new Exception(e.InnerException.Message);
            }
        }

        public async Task<ValidationResult> Update(Calendar calendar)
        {
            try
            {
                DbSet.Update(calendar);
                await Db.SaveChangesAsync();
                return ValidationResult.Success;
            }
            catch (Exception e)
            {
                throw new Exception(e.InnerException.Message);
            }

        }

        public async Task<IEnumerable<Calendar>> GetCalendarsByDate(DateTime date)
        {

            return await Db.Calendars
                .Include(x => x.CalendarSpecifications)
                .Where(x => x.Date.Date == date.Date && !notIn.Contains(x.Status) && x.Active)
                .OrderBy(x => x.Status)
                .OrderBy(x => x.Client.Name).ToListAsync();

        }

        public async Task<List<Calendar>> GetSpecificationsByDate(DateTime date, List<CalendarSpecifications> list)
        {
            var in_ = list.Select(x => x.SpecificationId);

            var result = await (from calendar in Db.Calendars
                    join specs in Db.CalendarSpecifications on calendar.Id equals specs.CalendarId
                    where in_.Contains(specs.SpecificationId) && calendar.Date.Date == date.Date &&
                    calendar.Active
                    select new Calendar
                    {
                        ClientId = calendar.ClientId,
                        EquipamentId = calendar.EquipamentId,
                        StartTime = calendar.StartTime,
                        EndTime = calendar.EndTime,
                        
                        
                    }).ToListAsync();

            return result;

        }

        public async Task<IEnumerable<Calendar>> ValidateEquipament(DateTime date, Guid clientId, Guid equipamentId)
        {
            
            var sql = $"select * from Calendars where date = '{date.ToString("yyyy-MM-dd")}' and equipamentId = '{equipamentId}' and ClientId != '{clientId}' and status not in ('3','4')";
            return await Db.Calendars.FromSqlRaw(sql).ToListAsync();

        }

        public async Task<IEnumerable<Calendar>> GetCalendarBySpecificationsAndDate(List<CalendarSpecifications> list, DateTime date, DateTime startTime)
        {
            var _in = In(list.Select(x => x.SpecificationId).ToList());

            var sql = $"select distinct c.* from Calendars as c left join CalendarSpecifications as cs on " +
                                "c.Id = cs.CalendarId " +
                                $"where CONVERT(varchar, c.date, 112) = '{date.ToString("yyyyMMdd")}' and ";
            if (list.Any())
                sql += $"cs.SpecificationId in ({_in}) and ";

            sql += $"'{startTime.ToString("HH:mm:ss")}' >= CONVERT(varchar, c.StartTime, 108) and " +
                                $"'{startTime.ToString("HH:mm:ss")}' <= CONVERT(varchar, c.EndTime, 108) ";

            return await Db.Calendars.FromSqlRaw(sql).ToListAsync();
        }

        public async Task<int> SpecCounterBySpec(Guid specificationId, DateTime date, DateTime startTime, Guid clientId)
        {
            var _notIn = new List<string> { "3", "4" };
            
            return await (from calendar in Db.Calendars
                           join specs in Db.CalendarSpecifications on calendar.Id equals specs.CalendarId
                           where specs.SpecificationId == specificationId
                           && calendar.Date.Date == date.Date
                           && startTime.TimeOfDay >= calendar.StartTime.Value.TimeOfDay
                           && startTime.TimeOfDay <= calendar.EndTime.Value.TimeOfDay
                           && specs.Active
                           && calendar.Active
                           && !_notIn.Contains(calendar.Status)
                           && calendar.ClientId != clientId
                           select specs).CountAsync();
        }


        public async Task<int> SingleSpecCounter(Guid specificationId, DateTime date)
        {
            var sql = $"select count(cs.Id) as amount from Calendars as c left join CalendarSpecifications as cs on " +
                                "c.Id = cs.CalendarId " +
                                $"where CONVERT(varchar, c.date, 112) = '{date.ToString("yyyyMMdd")}' and " +
                                $"cs.SpecificationId = '{specificationId}'";

            return await Db.Calendars.FromSqlRaw(sql).CountAsync();
        }


        public async Task<IEnumerable<Calendar>> Schedules(DateTime startDate, DateTime endDate,  Guid? clientId, List<Guid> equipamentId, List<Guid> driverId, Guid? techniqueId, string status)
        {
            try
            {
                var sql = await Db.Calendars.Include(x => x.Equipament)
                                  .Include(x => x.Client)
                                  .Include(x => x.Client.City)
                                  .Include(x => x.Client.State)
                                  .Include(x => x.Driver)
                                  .Include(x => x.Technique)
                                  .Include(x => x.User)
                                  .Include(x => x.CalendarSpecifications)
                                  .Where(x => x.Date.Date >= startDate
                                  && x.Date.Date <= endDate
                                  && x.Active).ToListAsync();

                if (clientId.HasValue)
                    sql = sql.Where(x => x.ClientId == clientId.Value).ToList();


                if (equipamentId.Any())
                    sql = sql.Where(x => equipamentId.Contains(x.EquipamentId)).ToList();

                if (driverId.Any())
                    sql = sql.Where(x => x.DriverId != null).Where(x => driverId.Contains(x.DriverId.Value)).ToList();

                if (techniqueId.HasValue)
                    sql = sql.Where(x => x.TechniqueId == techniqueId.Value).ToList();

                if (!string.IsNullOrEmpty(status))
                {
                    var _status = status.Split(",");
                    sql = sql.Where(x => _status.Contains(x.Status)).ToList();
                }
                    

                return sql.OrderBy(x => x.StartTime);
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public async Task<IEnumerable<Calendar>> Availability(List<Guid> equipamentIds, int month, int year)
        {
            var _notIn = new List<string> { "3", "4" };

            try
            {
                var sql = await Db.Calendars
                                  .Include(x => x.CalendarSpecifications)
                                  .Where(x => x.Date.Month == month && x.Date.Year == year
                                  && !_notIn.Contains(x.Status)
                                  && x.Active && equipamentIds.Contains(x.EquipamentId)).ToListAsync();

                
                return sql;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private string In(List<Guid> list)
        {
            var join = new List<string>();
            foreach (var item in list)
            {
                join.Add("'" + item + "'");
            }
            return string.Join(",",join);
        }

    }
}
