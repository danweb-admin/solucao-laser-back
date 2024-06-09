using System;
namespace Solucao.Application.Data.Entities
{
	public class CalendarEquipamentConsumable : BaseEntity
	{
		public int Amount { get; set; }
		public Guid CalendarId { get; set; }
		public Guid ConsumableId { get; set; }
		public Guid EquipamentId { get; set; }
		public decimal Value { get; set; }
        public decimal TotalValue { get; set; }
        public Calendar Calendar { get; set; }
		public Consumable Consumable { get; set; }
		public Equipament Equipament { get; set; }
	}
}

