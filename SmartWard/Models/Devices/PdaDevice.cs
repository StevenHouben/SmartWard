﻿using NooSphere.Model.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWard.Models.Devices
{
    public class PdaDevice : Device
    {
        public PdaDevice()
        {
            Type = typeof(PdaDevice).Name;
        }
    }
}