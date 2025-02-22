using Core.Entities;
using System;

namespace ZarinLend.Services.Model.NeginHub;

public class UpdatePersonInfoInputModel
{
    public string NationalCode { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string FatherName { get; set; }
    public string PlaceOfBirth { get; set; }
    public string SSID { get; set; }
    public GenderEnum Gender { get; set; }
}
