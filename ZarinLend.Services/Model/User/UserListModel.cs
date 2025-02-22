using System;
using System.Collections.Generic;

namespace Services.Model;

[Serializable]
public class UserListModel
{
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public string FName { get; set; }
    public string LName { get; set; }
    public string OrganizationName { get; set; }
    public string OrganizationTypeName { get; set; }
    public string Mobile { get; set; }
    public string RoleName { get; set; }
    public bool IsActive { get; set; }
    public long Balance { get; set; }
    public DateTime CreatedDate { get; set; }
    public string CreatedDatePersian { get; set; }
    public int? IranCardScore { get; set; }
}
