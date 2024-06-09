using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solucao.Application.Data.Entities
{
    public class CalendarSpecifications
    {
        public int Id { get; set; }
        public Guid CalendarId { get; set; }
        public Guid SpecificationId { get; set; }
        public bool Active { get; set; }
        public Calendar Calendar { get; set; }
        public Specification Specification { get; set; }
    }
}
