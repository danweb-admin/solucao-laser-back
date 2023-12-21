﻿using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Solucao.Application.Contracts;
using Solucao.Application.Data.Entities;
using Solucao.Application.Data.Interfaces;
using Solucao.Application.Data.Repositories;
using Solucao.Application.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Solucao.Application.Service.Implementations
{
    public class CalendarService : ICalendarService
    {
        private CalendarRepository calendarRepository;
        private IEquipamentRepository equipamentRepository;
        private SpecificationRepository specificationRepository;
        private readonly IMapper mapper;
        public CalendarService(CalendarRepository _calendarRepository, IMapper _mapper, SpecificationRepository _specificationRepository, IEquipamentRepository _equipamentRepository)
        {
            calendarRepository = _calendarRepository;
            mapper = _mapper;
            specificationRepository = _specificationRepository;
            equipamentRepository = _equipamentRepository;
        }

        public async Task<IEnumerable<CalendarViewModel>> GetAll(DateTime date)
        {
            return mapper.Map<IEnumerable<CalendarViewModel>>(await calendarRepository.GetAll(date));
        }
        public async Task<CalendarViewModel> GetById(Guid id)
        {
            return mapper.Map<CalendarViewModel>(await calendarRepository.GetById(id));
        }

        public Task<ValidationResult> Add(CalendarViewModel calendar, Guid user)
        {
            calendar.Id = Guid.NewGuid();
            calendar.Client = null;
            calendar.UserId = user;
            calendar.CreatedAt = DateTime.Now;

            if (!string.IsNullOrEmpty(calendar.Value))
                calendar.Value = calendar.Value.Replace(",", ".");

            if (!string.IsNullOrEmpty(calendar.StartTime1))
            {
                var start = calendar.Date.ToString("yyyy-MM-dd") + " " + calendar.StartTime1.Insert(2, ":");
                calendar.StartTime = DateTime.Parse(start);
            }

            if (!string.IsNullOrEmpty(calendar.EndTime1))
            {
                var end = calendar.Date.ToString("yyyy-MM-dd") + " " + calendar.EndTime1.Insert(2, ":");
                calendar.EndTime = DateTime.Parse(end);
            }

            if (string.IsNullOrEmpty(calendar.Status))
                calendar.Status = "pending";
            
            var _calendar = mapper.Map<Calendar>(calendar);

            return calendarRepository.Add(_calendar);
        }

        public async Task<ValidationResult> Update(CalendarViewModel calendar, Guid user)
        {
            
            ValidationResult result;
            Guid parentId;
            if (!string.IsNullOrEmpty(calendar.Value))
                calendar.Value = calendar.Value.Replace(",", ".");

            // Atualiza o registro e inativa a locação
            if (calendar.ParentId != null)
            {
                var temp = await GetById(calendar.Id.Value);
                temp.Active = false;
                temp.UpdatedAt = DateTime.Now;
                temp.Client = null;
                parentId = temp.ParentId.Value;
                calendar.Client = null;
                var temp_ = mapper.Map<Calendar>(temp);

                result = await calendarRepository.Update(temp_);
            }
            else
            {
                calendar.Client = null;
                calendar.UpdatedAt = DateTime.Now;
                calendar.Active = false;
                parentId = calendar.Id.Value;
                var _calendar = mapper.Map<Calendar>(calendar);

                result = await calendarRepository.Update(_calendar);
            }

            if (result == null)
            {
                calendar.ParentId = parentId;
                calendar.Id = null;
                calendar.CreatedAt = DateTime.Now;
                calendar.UpdatedAt = null;
                calendar.Active = true;
                calendar.StartTime = null;
                calendar.EndTime = null;
                calendar.UserId = user;

                if (!string.IsNullOrEmpty(calendar.StartTime1))
                {
                    var start = calendar.Date.ToString("yyyy-MM-dd") + " " + calendar.StartTime1.Replace(":", "").Insert(2, ":");
                    calendar.StartTime = DateTime.Parse(start);
                }

                if (!string.IsNullOrEmpty(calendar.EndTime1))
                {
                    var end = calendar.Date.ToString("yyyy-MM-dd") + " " + calendar.EndTime1.Replace(":", "").Insert(2, ":");
                    calendar.EndTime = DateTime.Parse(end);
                }

                if (string.IsNullOrEmpty(calendar.Status))
                    calendar.Status = "pending";

                var _calendarAdd = mapper.Map<Calendar>(calendar);

                return await calendarRepository.Add(_calendarAdd);

            }

            return ValidationResult.Success;
            
            
        }

        public async Task<ValidationResult> ValidateLease(DateTime date, Guid clientId, Guid equipamentId, IList<CalendarSpecifications> specifications, string startTime, string endTime)
        {
            try
            {
                var startTime_ = DateTime.Parse(date.ToString("yyyy-MM-dd ") + startTime.Replace(":", "").Insert(2, ":"));
                var endTime_ = DateTime.Parse(date.ToString("yyyy-MM-dd ") + endTime.Replace(":", "").Insert(2, ":"));

                // obtem todas as locacoes do dia
                var result = await calendarRepository.GetCalendarsByDate(date);

                if (ValidEquipamentInUse(result, equipamentId, clientId, startTime_, endTime_))
                    return new ValidationResult("Para data e hora informada, equipamento já está em uso.");

                // Valida se a ponteira ja esta em uso com outro equipamento
                // Os equipamentos podem compartilhar a mesma ponteira
                var temp = specifications.Where(x => x.Active).ToList();

                foreach (var item in temp)
                {
                    var spec = await specificationRepository.GetById(item.SpecificationId);
                    var counter = await calendarRepository.SpecCounterBySpec(item.SpecificationId, date, startTime_, clientId);

                    if (counter >= spec.Amount )
                        return new ValidationResult($"Para data e hora informada, ponteira já está em uso. ({spec.Name})");
                }
                

                // Valida se a ponteira/especificação é unica
                if (specifications.Any())
                {
                    var valid = await ValidIfSpecInUse(temp, date);
                    if (!valid)
                        return new ValidationResult($"Para data e hora informada, dispositivo ÚNICO está em uso.");

                }

                foreach (var item in result.Where(x => x.EquipamentId == equipamentId))
                {
                    
                    if (item.EndTime.HasValue)
                    {
                        var time = startTime_ - item.EndTime.Value;
                        double minutes = 0;
                        if (time.TotalMinutes < 0)
                            minutes = time.TotalMinutes * -1;
                        else
                            minutes = time.TotalMinutes;

                        if (minutes < 60)
                            return new ValidationResult("Diferença da locação do equipamento menor que 60 minutos.");

                    }
                }

                return ValidationResult.Success;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IEnumerable<CalendarViewModel>> Schedules(DateTime startDate, DateTime endDate, Guid? clientId, List<Guid> equipamentId, List<Guid> driverId, Guid? techniqueId, string status)
        {
            return mapper.Map<IEnumerable<CalendarViewModel>>(await calendarRepository.Schedules(startDate, endDate, clientId, equipamentId, driverId,techniqueId, status));
        }

        public async Task<string> Availability(List<Guid> equipamentIds, int month, int year)
        {
            var unavailables = await calendarRepository.Availability(equipamentIds, month, year);
            var monthDays = DateTime.DaysInMonth(year, month);
            dynamic ret = new JObject();
            var objectList = new List<object>();

            var equipament = await equipamentRepository.GetListById(equipamentIds);
            var list = new List<object>();
            foreach (var item in equipament)
            {
                var dayList = new List<object>();
                dynamic availableEquipament = new JObject();

                for (int day = 1; day <= monthDays; day++)
                {
                    dynamic availableDay = new JObject();
                    availableDay.Available = !unavailables.Any(x => x.Date.Date.Day == day && x.EquipamentId == item.Id);
                    availableDay.Day = day;
                    dayList.Add(availableDay);
                }
                objectList.Add(dayList);
                availableEquipament.Equipament = item.Name;
                availableEquipament.DayList = JArray.FromObject(dayList);
                list.Add(availableEquipament);
            }
            ret.List = JArray.FromObject(list);
            return JsonConvert.SerializeObject(ret);
        }

        public async Task<IEnumerable<EquipamentList>> GetAllByDate(DateTime date)
        {
            var list = new List<EquipamentList>();
            var equipament = await equipamentRepository.GetAll(true);
            var calendars = await calendarRepository.GetAll(date);

            foreach (var item in equipament)
            {
                var item_ = new EquipamentList();
                item_.Equipament = item;
                item_.Equipament.EquipamentSpecifications = null;
                var dayCalendars = calendars.Where(x => x.EquipamentId == item.Id).OrderBy(x => x.StartTime);
                if (dayCalendars.Any())
                {
                    item_.Calendars = dayCalendars;
                }

                list.Add(item_);
            }
            return list;
        }

        public async Task<ValidationResult> UpdateDriverOrTechniqueCalendar(Guid id, Guid personId, bool isDriver, bool isCollect)
        {
            try
            {
                var calendar = await calendarRepository.GetById(id);

                if (isDriver)
                {
                    if (isCollect)
                        calendar.DriverCollectsId = personId;
                    else
                        calendar.DriverId = personId;
                }
                else
                {
                    calendar.TechniqueId = personId;
                }

                await calendarRepository.Update(calendar);

                return ValidationResult.Success;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ValidationResult> UpdateStatusOrTravelOnCalendar(Guid id, string status, string travelOn, bool isTravelOn)
        {
            try
            {
                var calendar = await calendarRepository.GetById(id);

                if (isTravelOn)
                    calendar.TravelOn = int.Parse(travelOn);
                else
                    calendar.Status = status;

                await calendarRepository.Update(calendar);

                return ValidationResult.Success;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ValidationResult> UpdateContractMade(Guid id)
        {
            try
            {
                var calendar = await calendarRepository.GetById(id);
                calendar.ContractMade = !calendar.ContractMade;

                await calendarRepository.Update(calendar);

                return ValidationResult.Success;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<bool> ValidIfSpecInUse(IList<CalendarSpecifications> specifications, DateTime date)
        {
            var singleSpec = await specificationRepository.GetSingleSpec();

            if (singleSpec != null)
            {
                if (specifications.Any(x => x.SpecificationId == singleSpec.Id))
                {
                    var resp = await calendarRepository.SingleSpecCounter(singleSpec.Id, date);

                    if (resp > 0)
                        return false;
                }
            }
            return true;
        }

        

        private bool ValidEquipamentInUse(IEnumerable<Calendar> calendars, Guid equipamentId, Guid clientId, DateTime start, DateTime end)
        {
            var inUse = false;

            foreach (var calendar in calendars.Where(x => x.EquipamentId == equipamentId && x.ClientId != clientId))
            {
                if ((start >= calendar.StartTime && start <= calendar.EndTime) ||
                    (end >= calendar.StartTime && end <= calendar.EndTime) ||
                    (start <= calendar.StartTime && end >= calendar.EndTime))
                    return true;
            }

            return inUse;
        }

        private async Task<IEnumerable<Equipament>> MountSpecificationByEquipament(List<Guid> equipamentIds, List<Guid> specificationIds)
        {
            
                if (specificationIds.Any())
                {
                    var ret = new List<Equipament>();
                    var specification = await equipamentRepository.GetListById(equipamentIds);

                    foreach (var item in specification)
                    {
                        var specs = item.EquipamentSpecifications.Where(x => x.Active && specificationIds.Contains(x.SpecificationId)).ToList();
                        var equip = new Equipament();
                        equip = item;
                        equip.EquipamentSpecifications = specs;
                        ret.Add(equip);
                    }

                    return ret;
                }
            
            

            return null;
        }

        
    }
}
