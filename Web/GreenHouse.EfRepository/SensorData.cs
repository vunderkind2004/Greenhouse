//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GreenHouse.EfRepository
{
    using System;
    using System.Collections.Generic;
    
    public partial class SensorData
    {
        public int Id { get; set; }
        public System.DateTime EventDateTime { get; set; }
        public int SensorId { get; set; }
        public double Value { get; set; }
    
        public virtual Sensor Sensor { get; set; }
    }
}
