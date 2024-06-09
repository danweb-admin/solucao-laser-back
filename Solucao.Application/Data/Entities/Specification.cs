using System.Collections.Generic;

namespace Solucao.Application.Data.Entities
{
    public class Specification : BaseEntity
    {
        public string Name { get; set; }
        public int Amount { get; set; }
        public bool Single { get; set; }
        public bool HasConsumable { get; set; }
        public decimal Value { get; set; }
        public List<EquipamentSpecifications> EquipamentSpecifications { get; set; }
        public IList<CalendarSpecifications> CalendarSpecifications { get; set; }
    }
}
