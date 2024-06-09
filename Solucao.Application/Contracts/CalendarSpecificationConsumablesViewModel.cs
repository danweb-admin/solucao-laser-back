using System;
using Solucao.Application.Data.Entities;

namespace Solucao.Application.Contracts
{
	public class CalendarSpecificationConsumablesViewModel
	{

        public Guid? Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool Active { get; set; }
        public Guid? CalendarId { get; set; }
        public Guid SpecificationId { get; set; }
        public int Initial { get; set; }
        public int Final { get; set; }
        public decimal Value { get; set; }
        public decimal TotalValue { get; set; }
        public Calendar Calendar { get; set; }
        public Specification Specification { get; set; }

    }
}

