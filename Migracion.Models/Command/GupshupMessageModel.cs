using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Migracion.Models.Command
{

    public class GupshupCallback
    {
        public string app { get; set; }
        public long timestamp { get; set; }
        public int? version { get; set; }
        public string type { get; set; }
        public JsonObject? payload { get; set; }
    }


    public class ResultCallback
    {
        public string? status { get; set; }
        public string? messageId { get; set; }
    }
}

