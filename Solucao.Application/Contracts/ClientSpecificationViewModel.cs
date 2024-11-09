using System;
namespace Solucao.Application.Contracts
{
    public class ClientSpecificationViewModel
    {
        public Guid? Id { get; set; }
        public Guid ClientId { get; set; }
        public Guid SpecificationId { get; set; }
        public int Hours { get; set; }
        public decimal Value { get; set; }
        public ClientViewModel Client { get; set; }
        public SpecificationViewModel Specification { get; set; }
    }
}

