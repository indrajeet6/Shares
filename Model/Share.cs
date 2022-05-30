using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shares.Model
{
    public partial class Share
    {
        public string Symbol { get; set; } = null!;
        public string Name { get; set; } = null!;
        [Column("Previous Working Day Market Value")]
        public decimal MktValue { get; set; }
        public string Count { get; set; } = null!;
        public decimal Total { get; set; }
    }
}
