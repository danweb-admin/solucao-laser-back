using System;
namespace Solucao.Application.Data.Entities
{
	public class EquipamentConsumable : BaseEntity
	{
		public Guid ConsumableId { get; set; }
		public Guid EquipamentId { get; set; }
		public decimal Value { get; set; }
		public Consumable Consumable { get; set; }
		public Equipament Equipament { get; set; }
	}
}

