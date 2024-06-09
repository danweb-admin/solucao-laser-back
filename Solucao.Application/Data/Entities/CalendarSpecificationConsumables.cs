using System;
namespace Solucao.Application.Data.Entities
{
	public class CalendarSpecificationConsumables : BaseEntity
	{
		public Guid CalendarId { get; set; }
		public Guid SpecificationId { get; set; }
		public int Initial { get; set; }
		public int Final { get; set; }
		public decimal Value { get; set; }
		public decimal TotalValue { get; set; }
        public Calendar Calendar { get; set; }
        public Specification Specification { get; set; }
    }
}

