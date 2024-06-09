using System;
using Solucao.Application.Data.Entities;

namespace Solucao.Application.Contracts
{
	public class CalendarEquipamentConsumablesViewModel
	{
        public Guid? Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool Active { get; set; }
        public int Amount { get; set; }
        public Guid? CalendarId { get; set; }
        public Guid ConsumableId { get; set; }
        public Guid EquipamentId { get; set; }
        public decimal Value { get; set; }
        public decimal TotalValue { get; set; }
        public Consumable Consumable { get; set; }
    }
}

