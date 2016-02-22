using System;
using System.Collections.Generic;

namespace ConsoleAppServiceBus.Example.Common
{
    [Serializable]
    public class Applicant : IMessage
    {
        public int Id { get; set; }
        public Guid ApplicantionId { get; set; }
        public Salutation Salutation { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public IList<Address> Addresses { get; set; }
        public DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public DateTime Modified { get; set; }
        public string ModifiedBy { get; set; }

        
        public Applicant()
        {
            Created = DateTime.Now;
            Modified = DateTime.Now;
        }

        public override string ToString()
        {
            return $"{Salutation} {FirstName} {LastName} (Id:{Id})";
        }
    }
    
}
