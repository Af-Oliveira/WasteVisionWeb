using System;
using System.Text.Json.Serialization;

namespace DDDSample1.Domain.Application
{
    public class ProfileDtoForUpdate
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }

        public ProfileDtoForUpdate()
        {
        }

        [JsonConstructor]
        public ProfileDtoForUpdate(string FirstName, string LastName, string FullName, string Phone)
        {
            this.FirstName = FirstName;
            this.LastName = LastName;
            this.FullName = FullName;
            this.Phone = Phone;
        }
    }
}
