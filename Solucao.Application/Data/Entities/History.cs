using System;
namespace Solucao.Application.Data.Entities
{
    public class History : BaseEntity
    {
        public string TableName { get; set; }
        public Guid RecordId { get; set; }
        public string Operation { get; set; }
        public DateTime OperationDate { get; set; }
        public string Message { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}

