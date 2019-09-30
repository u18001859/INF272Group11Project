//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace INF272Group11Project.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Voter
    {
        public int VoterID { get; set; }
        public string VoterIDNumber { get; set; }
        public string VoterPassword { get; set; }
        public string VoterFirstNames { get; set; }
        public string VoterLastName { get; set; }
        public string VoterEmail { get; set; }
        public string VoterPhoneNumber { get; set; }
        public string VoterStreetAddress { get; set; }
        public bool VotingStatus { get; set; }
        public string SecurityQuestionAnswer { get; set; }
        public Nullable<int> SecurityQuestionID { get; set; }
        public Nullable<int> SuburbID { get; set; }
    
        public virtual SecurityQuestion SecurityQuestion { get; set; }
        public virtual Suburb Suburb { get; set; }
    }
}