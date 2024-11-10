using System;
using Solucao.Application.Data.Entities;

namespace Solucao.Application.Contracts
{
    public class ClientSpecificationViewModel
    {
        public Guid? Id { get; set; }
        public Guid ClientId { get; set; }
        public Guid SpecificationId { get; set; }
        public double Hours { get; set; }
        public decimal Value { get; set; }
        public string Condition { get; set; }
        public Client Client { get; set; }
        public Specification Specification { get; set; }
    }
}

