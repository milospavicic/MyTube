//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MyTube.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Subscriber
    {
        public long SubID { get; set; }
        public string ChannelSubscribed { get; set; }
        public string Subscriber1 { get; set; }
    
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
    }
}