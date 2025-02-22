using System;
using System.Collections.Generic;

namespace Services.Model
{
    [Serializable]
    public class UserDetailModel
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public string Address { get; set; }
        public string Mobile { get; set; }
        public string PhoneNumber { get; set; }
        public string NationalCode { get; set; }
        public string Email { get; set; }
        public string PostalCode { get; set; }
        public long Balance { get; set; }
        public int ProvinceId { get; set; }
        public int CityId { get; set; }

        public List<DocumentModel> UserIdentityDocuments { get; set; }
    }
}
