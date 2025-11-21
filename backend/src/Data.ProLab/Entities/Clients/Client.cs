using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProLab.Data.Entities.Orders;
using ProLab.Data.Shared;

namespace ProLab.Data.Entities.Clients
{
    public class Client : BaseEntity<long>
    {
        public string FullName { get; set; }
    }
}