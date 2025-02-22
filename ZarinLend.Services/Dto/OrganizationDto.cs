using AutoMapper;
using Core.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Services.Dto
{
    public class OrganizationDto : BaseDto<OrganizationDto, Organization>, IValidatableObject
    {
        [Required(ErrorMessage = "شماره پاسپورت اجباری است!", AllowEmptyStrings = false)]
        [Display(Name = "شماره پاسپورت")]
        public string PassportId { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "توضیحات")]
        public string Description { get; set; }

        [Required(ErrorMessage = "کاربر را انتخاب کنید!")]
        [Display(Name = "کاربر")]
        public Guid AuthorId { get; set; }


        [Required(ErrorMessage = "دسته بندی را انتخاب کنید!")]
        [Display(Name = "دسته بندی")]
        public int CategoryId { get; set; }

        public int Capacity { get; set; }

        public decimal Price { get; set; }
        public bool  IsActive { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; }

        public IEnumerable<SelectListItem> Authors { get; set; }

        public IEnumerable<ValidationResult> Validate(System.ComponentModel.DataAnnotations.ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty( PassportId))
                yield return new ValidationResult("شماره پاسپورت اجباری است!");
        }

    }

    public class OrganizationSelectDto : BaseDto<OrganizationSelectDto, Organization>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string CategoryName { get; set; } //=> Category.Name
        public string AuthorFullName { get; set; } //=> Author.FullName
        public string FullTitle { get; set; } // => mapped from "Title + Category.Name"        

        public override void CustomMappings(IMappingExpression<Organization, OrganizationSelectDto> mappingExpression)
        {
            //mappingExpression.ForMember(
            //        dest => dest.FullTitle,
            //        config => config.MapFrom(src => $"{src.Title} ({src.Category.Name})"));
        }
    }
}
