using System;
using System.ComponentModel.DataAnnotations;

namespace Services.Model
{
    [Serializable]
    public class PersonModel
    {
        public int Id { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public string Mobile { get; set; }
        public string NationalCode { get; set; }
        public string OrganizationName { get; set; }
        public int? OrganizationId { get; set; }

    }
}
