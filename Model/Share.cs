using System;
using System.Collections.Generic;

namespace Shares.Model
{
    public partial class Share
    {
        public string Symbol { get; set; } = null!;
        public string Name { get; set; } = null!;
        public decimal MktValue { get; set; }
        public string Count { get; set; } = null!;
        public decimal Total { get; set; }
        public string? LastUpdateTime { get; set; }
    }
}
