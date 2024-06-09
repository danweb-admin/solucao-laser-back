using System;
using System.Collections.Generic;

namespace Solucao.Application.Data.Entities
{
	public class Consumable :BaseEntity
	{
		public string Name { get; set; }
		public List<EquipamentConsumable>  EquipamentConsumables { get; set; }

	}
}

