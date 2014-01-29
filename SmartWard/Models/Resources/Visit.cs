using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWard.Models.Resources
{
    public class Visit : Resource
    {
        #region Properties
        private DateTime Visited { get; set; }
        #endregion  

        public Visit(string patientId) : base(patientId)
        {
            Type = typeof(Visit).Name;
        }
    }
}
