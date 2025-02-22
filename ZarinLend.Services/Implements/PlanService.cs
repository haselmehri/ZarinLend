using Common;
using Common.Exceptions;
using Common.Utilities;
using Core.Data.Repositories;
using Core.Entities;
using Core.Entities.Business.RequestFacility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using Services.Dto;
using Services.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Common.Enums;

namespace Services
{
    public class PlanService : IPlanService, IScopedDependency
    {
        private readonly ILogger<PlanService> logger;
        private readonly IBaseRepository<Plan> planRepository;
        private readonly IBaseRepository<PlanMember> planMemberRepository;
        private readonly IOrganizationRepository organizationRepository;
        private readonly ILocationService locationService;
        private readonly IBaseRepository<FacilityType> facilityTypeRepository;
        private readonly IUserRepository userRepository;
        private readonly IRequestFacilityService requestFacilityService;
        private readonly IWorkFlowStepRepository workFlowStepRepository;
        private readonly UserManager<User> userManager;
        private readonly IRequestFacilityRepository requestFacilityRepository;
        private readonly IGlobalSettingService globalSettingService;
        private readonly IWebHostEnvironment webHostEnvironment;

        public PlanService(ILogger<PlanService> logger, IBaseRepository<Plan> planRepository, IBaseRepository<PlanMember> planMemberRepository,
            IOrganizationRepository organizationRepository, ILocationService locationService, IBaseRepository<FacilityType> facilityTypeRepository,
            IUserRepository userRepository, IRequestFacilityService requestFacilityService, IWorkFlowStepRepository workFlowStepRepository,
            UserManager<User> userManager, IRequestFacilityRepository requestFacilityRepository, IGlobalSettingService globalSettingService,
            IWebHostEnvironment webHostEnvironment)
        {
            this.logger = logger;
            this.planRepository = planRepository;
            this.planMemberRepository = planMemberRepository;
            this.organizationRepository = organizationRepository;
            this.locationService = locationService;
            this.facilityTypeRepository = facilityTypeRepository;
            this.userRepository = userRepository;
            this.requestFacilityService = requestFacilityService;
            this.workFlowStepRepository = workFlowStepRepository;
            this.userManager = userManager;
            this.requestFacilityRepository = requestFacilityRepository;
            this.globalSettingService = globalSettingService;
            this.webHostEnvironment = webHostEnvironment;
        }

        //public async Task<OrganizationModel> GetOrganization(int organizationId, CancellationToken cancellationToken)
        //{
        //    return (await organizationRepository.SelectByAsync(p => p.Id.Equals(organizationId),
        //        p => new OrganizationModel()
        //        {
        //            Id = p.Id,
        //            Address = p.Address,
        //            Name = p.Name,
        //            NationalId = p.NationalId,
        //            OrganizationTypeId = p.OrganizationTypeId,
        //            OrganizationTypeName = p.OrganizationType.Name,
        //            SiteUrl = p.SiteUrl,
        //            Tel = p.Tel
        //        }, cancellationToken))
        //        .FirstOrDefault();
        //}

        public virtual async Task<PagingDto<PlanListModel>> SelectPlans(PagingFilterDto filter, CancellationToken cancellationToken = default)
        {
            var result = await planRepository.SelectByAsync(p => new PlanListModel()
            {
                Id = p.Id,
                Name = p.Name,
                AmountWaranty = p.AmountWaranty,
                FacilityAmount = p.FacilityAmount,
                FacilityTypeTitle = p.FacilityType.Organization.Name + " " + p.FacilityType.MonthCountTitle,
                ImportDone = p.ImportDone,
                OrganizationName = p.Organization.Name,
                PlanFileUrlList = p.PlanFiles.Select(p => p.FilePath).ToList(),
                IsActive = p.IsActive
            },
            cancellationToken, pageNumber: filter.Page, pageSize: filter.PageSize);

            var totalRowCount = await planRepository.TableNoTracking.CountAsync();

            return new PagingDto<PlanListModel>()
            {
                CurrentPage = filter.Page,
                Data = result,
                TotalRowCount = totalRowCount,
                TotalPages = (int)Math.Ceiling(Convert.ToDouble(totalRowCount) / filter.PageSize)
            };
        }

        public async Task<List<PlanMemberModel>> GetMembers(int planId, CancellationToken cancellationToken)
        {
            return await planMemberRepository.SelectByAsync(p => p.PlanId == planId,
                p => new PlanMemberModel
                {
                    FName = p.FName,
                    LName = p.LName,
                    FatherName = p.FatherName,
                    NationalCode = p.NationalCode,
                    Mobile = p.Mobile,
                    SSID = p.SSID,
                    HasError = p.HasError,
                    ErrorMessage = p.ErrorMessage,
                    ImportSuccess = p.ImportSuccess,
                    ProvinceName = p.AddressProvince,
                    CityName = p.AddressCity,
                    ProvinceOfBirthName = p.BirthLocationProvince,
                    CityOfBirthName = p.BirthLocationCity,
                    ProvinceOfIssueName = p.ProvinceOfIssue,
                    CityOfIssueName = p.CityOfIssueName,
                    FacilityAmount = p.FacilityAmount
                }, cancellationToken);
        }

        public virtual async Task Add(PlanAddModel model, IFormFile membersExcelFile, CancellationToken cancellationToken = default)
        {
            if (await planRepository.TableNoTracking.AnyAsync(p => p.Name == model.Name))
                throw new AppException("نام طرح تکراری می باشد");

            var activeGlobaSetting = await globalSettingService.GetActiveGlobalSetting(cancellationToken);
            if (activeGlobaSetting == null)
                throw new AppException("فاکتورهای مربوط به تسهیلات یافت نشد!");

            var planExcelPath = @"UploadFiles\Plan";
            string uploadFolder = Path.Combine(webHostEnvironment.WebRootPath, planExcelPath);
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(membersExcelFile.FileName)}";
            var relativePath = $"/UploadFiles/Plan/{fileName}";
            string filePath = Path.Combine(uploadFolder, fileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await membersExcelFile.CopyToAsync(fileStream);
            }

            var plan = new Plan()
            {
                GlobalSettingId = activeGlobaSetting.Id,
                Name = model.Name,
                Description = model.Description,
                AmountWaranty = model.AmountWaranty,
                FacilityAmount = model.FacilityAmount,
                FacilityTypeId = model.FacilityTypeId,
                OrganizationId = model.OrganizationId,
                IsActive = true,
                PlanFiles = new List<PlanFile>()
                {
                    new PlanFile()
                    {
                        FilePath = relativePath,
                        Status = DocumentStatus.Active,
                        FileType = FileType.Image
                    }
                },
                PlanMembers = model.Members.Select(p => new PlanMember()
                {
                    AccountNumber = p.AccountNumber,
                    Address = p.Address,
                    BankName = p.BankName,
                    BirthCertificateSerial = p.BirthCertificateSerial,
                    BirthDate = p.BirthDate,
                    BirthLocationProvince = p.ProvinceOfBirthName,
                    BirthLocationId = p.CityOfBirthId,
                    BirthLocationCity = p.CityOfBirthName,
                    CityOfIssueId = p.CityOfIssueId,
                    CityOfIssueName = p.CityOfIssueName,
                    ProvinceOfIssue = p.ProvinceOfIssueName,
                    AddressCity = p.CityName,
                    AddressProvince = p.ProvinceName,
                    CityId = p.CityId,
                    CardNumber = !string.IsNullOrEmpty(p.CardNumber) ? p.CardNumber.Replace("-", string.Empty) : null,
                    CustomerNumber = p.CustomerNumber,
                    DepositOwners = $"{p.FName} {p.LName}",
                    DepositStatus = "فعال",
                    FacilityAmount = p.FacilityAmount,
                    LName = p.LName,
                    FName = p.FName,
                    FatherName = p.FatherName,
                    GenderText = p.GenderText,
                    Gender = p.Gender,
                    IBAN = p.IBAN,
                    Mobile = p.Mobile,
                    NationalCode = p.NationalCode,
                    PostalCode = p.PostalCode,
                    SSID = p.SSID,
                    PhoneNumber = p.PhoneNumber,
                    Email = p.Email,
                    UserId = p.UserId,
                    HasError = p.HasError,
                    ErrorMessage = p.ErrorMessage,
                })
                .ToList()
            };
            await planRepository.AddAsync(plan, cancellationToken, true);
            var planMembers = planMemberRepository.TableNoTracking.Where(p => !p.HasError).ToList();
            var firstStep = await workFlowStepRepository.GetFirstStep(WorkFlowEnum.RequestFacility, new List<RoleEnum> { RoleEnum.Buyer }, cancellationToken);
            if (firstStep == null)
                throw new AppException("First Step to RequestFacility WorkFlow Not Found!");
            foreach (var member in planMembers)
            {
                if (!member.UserId.HasValue)
                {
                    #region first create user & person
                    var user = new User()
                    {
                        UserName = member.NationalCode,
                        Email = member.Email,
                        EmailConfirmed = false,
                        IsActive = true,
                        PhoneNumber = member.PhoneNumber,
                        Person = new Person()
                        {
                            Address = member.Address,
                            FName = member.FName,
                            LName = member.LName,
                            FatherName = member.FatherName,
                            SSID = member.SSID,
                            Gender = member.Gender.Value,
                            Mobile = member.Mobile,
                            MobileConfirmed = false,
                            MobileShahkarConfirmed = false,
                            NationalCode = member.NationalCode,
                            PostalCode = member.PostalCode,
                            CityId = member.CityId.Value,
                            BirthDate = member.BirthDate.Value,
                            CountryId = 1,//Iran
                            CustomerNumber = member.CustomerNumber,
                            //AccountNumber = member.AccountNumber,
                            //CardNumber = member.CardNumber,
                            //IBAN = member.IBAN,
                            //BankName = member.BankName,
                            //DepositOwners = member.DepositOwners,
                            //DepositStatus = member.DepositStatus
                        }
                    };
                    var result = await userManager.CreateAsync(user, member.NationalCode);
                    if (result.Succeeded)
                    {
                        #region Add User to Role(s)
                        if ((await userManager.AddToRolesAsync(user, new[] { RoleEnum.Buyer.ToString() })).Succeeded)
                        {
                            member.UserId = user.Id;
                            member.ImportSuccess = true;
                            await planMemberRepository.UpdateCustomPropertiesAsync(member, cancellationToken, true,
                                nameof(PlanMember.ImportSuccess),
                                nameof(PlanMember.UserId));
                        }
                        #endregion
                        logger.LogInformation("Buyer User Created ");
                    }
                    #endregion first create user & person
                }

                #region Add Facility base Plan
                var requestFacility = new RequestFacility()
                {
                    GlobalSettingId = activeGlobaSetting.Id,
                    Amount = member.FacilityAmount.Value,
                    BuyerId = member.UserId.Value,
                    FacilityTypeId = plan.FacilityTypeId,
                    PlanMemberId = member.Id,
                    RequestFacilityWorkFlowSteps = new List<RequestFacilityWorkFlowStep>()
                    {
                        new RequestFacilityWorkFlowStep()
                        {
                            OpratorId = model.CreatorId,
                            StatusId = (short)StatusEnum.Approved,
                            StatusDescription =$"ایجاد درخواست براساس طرح '{plan.Name}'،بصورت اتوماتیک تایید شد",
                            WorkFlowStepId = firstStep.Id,
                        },
                         new RequestFacilityWorkFlowStep()
                        {
                            OpratorId = model.CreatorId,
                            StatusDescription =$"بعد از ثبت درخواست تسهیلات براساس طرح '{plan.Name}'،بصورت اتوماتیک توسط سیستم ایجاد شد",
                            WorkFlowStepId = firstStep.ApproveNextStepId.Value,
                        }
                    }
                };
                await requestFacilityRepository.AddAsync(requestFacility, cancellationToken, true);
                #endregion Add Facility base Plan
            }
        }

        //public virtual async Task Edit(OrganizationModel organizationModel, CancellationToken cancellationToken = default)
        //{
        //    if (await organizationRepository.TableNoTracking.AnyAsync(p => p.Name == organizationModel.Name && !p.Id.Equals(organizationModel.Id)))
        //        throw new AppException("نام سامان/لیزینگ/فروشگاه تکراری می باشد");

        //    if (await organizationRepository.TableNoTracking.AnyAsync(p => p.NationalId == organizationModel.NationalId && !p.Id.Equals(organizationModel.Id)))
        //        throw new AppException("شناسه ملی تکراری می باشد");

        //    //var user = await userManager.FindByIdAsync(userModel.Id.ToString());
        //    var organization = await organizationRepository.GetByIdAsync(cancellationToken, organizationModel.Id);
        //    if (organization == default(Organization))
        //        throw new NotFoundException();

        //    organization.Name = organizationModel.Name;
        //    organization.Address = organizationModel.Address;
        //    organization.SiteUrl = organizationModel.SiteUrl;
        //    organization.IsActive = organizationModel.IsActive;
        //    organization.Tel = organizationModel.Tel;
        //    organization.OrganizationTypeId = organizationModel.OrganizationTypeId;

        //    await organizationRepository.UpdateAsync(organization, cancellationToken);
        //}

        //public async Task<string> GetOrganizationName(int organizationId, CancellationToken cancellationToken)
        //{
        //    return await organizationRepository.GetColumnValueAsync<string>(p => p.Id.Equals(organizationId), cancellationToken, "Name");
        //}

        public async Task<List<PlanMemberModel>> VerifyPlanMembesExcel(IFormFile membersExcelFile, CancellationToken cancellationToken)
        {
            using (var ms = new MemoryStream())
            {
                membersExcelFile.CopyTo(ms);
                var extractMemberList = await ExtractMemberDataFromExcel(ms, cancellationToken);
                if (extractMemberList == null || !extractMemberList.Any())
                    throw new AppException("براساس فایل بارگذاری شده هیچ اطلاعاتی یافت نشد");

                foreach (var member in extractMemberList.Where(p => !p.HasError))
                {
                    var user = await userRepository.TableNoTracking
                        .Include(p => p.Person)
                        .FirstOrDefaultAsync(p => p.UserName == member.NationalCode);

                    if (user != null)
                    {
                        if (user.Person.SSID.Trim() != member.SSID || user.Person.FName.CleanString() != member.FName ||
                            user.Person.LName.CleanString() != member.LName || user.Person.FatherName != member.FatherName)
                        {
                            member.HasError = true;
                            member.ErrorMessage = !string.IsNullOrEmpty(member.ErrorMessage)
                                ? "<br/>کد ملی فوق تکراری است،اما اطلاعات هویتی متفاوت است"
                                : "کد ملی فوق تکراری است،اما اطلاعات هویتی متفاوت است";
                            continue;
                        }
                        else
                        {
                            member.ErrorMessage = "اطلاعات هویتی در دیتابیس وجود دارد";
                            if (await requestFacilityService.UserHasOpenRequest(user.Id, cancellationToken))
                            {
                                member.HasError = true;
                                member.ErrorMessage = !string.IsNullOrEmpty(member.ErrorMessage)
                                    ? "<br/>درخواست تسهیلات فعال وجود دارد"
                                    : "درخواست تسهیلات فعال وجود دارد";
                                continue;
                            }
                            var sumAmountApprovedFacilitiesNotPaid = await requestFacilityService.GetUserSumAmountApprovedFacilities(user.Id, cancellationToken);
                            if (member.FacilityAmount.Value > (500000000 - sumAmountApprovedFacilitiesNotPaid))
                            {
                                member.HasError = true;
                                member.ErrorMessage = !string.IsNullOrEmpty(member.ErrorMessage)
                                    ? $"<br/>مجموع تسهیلات فعال : {sumAmountApprovedFacilitiesNotPaid:N0} ريال | حداکثر میزان تسهیلات فعال : {500000000:N0} ريال | مبلغ درخواستی : {member.FacilityAmount.Value:N0}"
                                    : $"مجموع تسهیلات فعال : {sumAmountApprovedFacilitiesNotPaid:N0} ريال | حداکثر میزان تسهیلات فعال : {500000000:N0} ريال | مبلغ درخواستی : {member.FacilityAmount.Value:N0}";
                                continue;
                            }
                            member.UserId = user.Id;
                        }
                    }
                }

                return extractMemberList;
            }
        }

        public async Task<PlanModel> PrepareModelForAdd(CancellationToken cancellationToken)
        {
            return new PlanModel()
            {
                Organizations = (await organizationRepository.SelectByAsync(p => p.IsActive.Equals(true) && p.OrganizationTypeId == (int)OrganizationTypeEnum.Company,
                     p => new SelectListItem
                     {
                         Text = p.Name,
                         Value = p.Id.ToString()
                     }, cancellationToken))
                     .ToList(),
                FacilityTypes = (await facilityTypeRepository.SelectByAsync(p => p.IsActive,
                p => new SelectListItem
                {
                    Text = p.Organization.Name + " - " + p.MonthCountTitle,
                    Value = p.Id.ToString()
                }, cancellationToken))
                .ToList()
            };
        }

        private string CleanValue(string value)
        {
            return value.CleanString();
        }
        private async Task<List<PlanMemberModel>> ExtractMemberDataFromExcel(Stream stream, CancellationToken cancellationToken)
        {
            using (var excelPackage = new ExcelPackage(stream))
            {
                var result = new List<PlanMemberModel>();
                PlanMemberModel item = null;
                if (excelPackage.Workbook.Worksheets.Count > 0)
                {
                    var workSheet = excelPackage.Workbook.Worksheets[0];

                    #region find column in excel sheet

                    var nationalCodeColumn = workSheet.Cells["1:1"].FirstOrDefault(c => c.Value != null && c.Value.ToString().CleanString().Replace(" ", string.Empty) == "کدملی");
                    if (nationalCodeColumn == null)
                        throw new AppException(ApiResultStatusCode.BadRequest, "ستون 'کد ملی' در فایل اکسل یافت نشد");

                    var fnameColumn = workSheet.Cells["1:1"]
                          .FirstOrDefault(c => c.Value != null && c.Value.ToString().Replace(" ", string.Empty).CleanString() == "نام".CleanString().Replace(" ", string.Empty));
                    if (fnameColumn == null)
                        throw new AppException(ApiResultStatusCode.BadRequest, "ستون 'نام' در فایل اکسل یافت نشد");

                    var lnameColumn = workSheet.Cells["1:1"]
                           .FirstOrDefault(c => c.Value != null && c.Value.ToString().Replace(" ", string.Empty).CleanString() == "نام خانوادگی".CleanString().Replace(" ", string.Empty));
                    if (lnameColumn == null)
                        throw new AppException(ApiResultStatusCode.BadRequest, "ستون 'نام خانوادگی' در فایل اکسل یافت نشد");

                    var fatherNameColumn = workSheet.Cells["1:1"]
                        .FirstOrDefault(c => c.Value != null && c.Value.ToString().Replace(" ", string.Empty).CleanString() == "نام پدر".CleanString().Replace(" ", string.Empty));
                    if (fatherNameColumn == null)
                        throw new AppException(ApiResultStatusCode.BadRequest, "ستون 'نام پدر' در فایل اکسل یافت نشد");

                    var mobileColumn = workSheet.Cells["1:1"]
                            .FirstOrDefault(c => c.Value != null && c.Value.ToString().Replace(" ", string.Empty).CleanString() == "شماره موبایل".CleanString().Replace(" ", string.Empty));
                    if (mobileColumn == null)
                        throw new AppException(ApiResultStatusCode.BadRequest, "ستون 'شماره موبایل' در فایل اکسل یافت نشد");

                    var genderColumn = workSheet.Cells["1:1"]
                        .FirstOrDefault(c => c.Value != null && c.Value.ToString().Replace(" ", string.Empty).CleanString() == "جنسیت".CleanString().Replace(" ", string.Empty));
                    if (genderColumn == null)
                        throw new AppException(ApiResultStatusCode.BadRequest, "ستون 'جنسیت' در فایل اکسل یافت نشد");

                    var ssidColumn = workSheet.Cells["1:1"]
                          .FirstOrDefault(c => c.Value != null && c.Value.ToString().Replace(" ", string.Empty).CleanString() == "شماره شناسنامه".CleanString().Replace(" ", string.Empty));
                    if (ssidColumn == null)
                        throw new AppException(ApiResultStatusCode.BadRequest, "ستون 'شماره شناسنامه' در فایل اکسل یافت نشد");

                    var birthCertificateSerialColumn = workSheet.Cells["1:1"]
                        .FirstOrDefault(c => c.Value != null && c.Value.ToString().Replace(" ", string.Empty).CleanString() == "سريال شناسنامه".CleanString().Replace(" ", string.Empty));
                    if (birthCertificateSerialColumn == null)
                        throw new AppException(ApiResultStatusCode.BadRequest, "ستون 'سريال شناسنامه' در فایل اکسل یافت نشد");

                    var phoneNumberColumn = workSheet.Cells["1:1"]
                          .FirstOrDefault(c => c.Value != null && c.Value.ToString().Replace(" ", string.Empty).CleanString() == "تلفن ثابت".CleanString().Replace(" ", string.Empty));
                    if (phoneNumberColumn == null)
                        throw new AppException(ApiResultStatusCode.BadRequest, "ستون 'تلفن ثابت' در فایل اکسل یافت نشد");

                    var emailColumn = workSheet.Cells["1:1"]
                        .FirstOrDefault(c => c.Value != null && c.Value.ToString().Replace(" ", string.Empty).CleanString() == "ایمیل".CleanString().Replace(" ", string.Empty));
                    if (emailColumn == null)
                        throw new AppException(ApiResultStatusCode.BadRequest, "ستون 'ایمیل' در فایل اکسل یافت نشد");

                    var postalCodeColumn = workSheet.Cells["1:1"]
                        .FirstOrDefault(c => c.Value != null && c.Value.ToString().Replace(" ", string.Empty).CleanString() == "کد پستی".CleanString().Replace(" ", string.Empty));
                    if (postalCodeColumn == null)
                        throw new AppException(ApiResultStatusCode.BadRequest, "ستون 'کد پستی' در فایل اکسل یافت نشد");

                    var shamsiBirthDateColumn = workSheet.Cells["1:1"]
                        .FirstOrDefault(c => c.Value != null && c.Value.ToString().Replace(" ", string.Empty).CleanString() == "تاریخ تولد".CleanString().Replace(" ", string.Empty));
                    if (shamsiBirthDateColumn == null)
                        throw new AppException(ApiResultStatusCode.BadRequest, "ستون 'تاریخ تولد' در فایل اکسل یافت نشد");

                    var addressColumn = workSheet.Cells["1:1"]
                   .FirstOrDefault(c => c.Value != null && c.Value.ToString().Replace(" ", string.Empty).CleanString() == "آدرس".CleanString().Replace(" ", string.Empty));
                    if (addressColumn == null)
                        throw new AppException(ApiResultStatusCode.BadRequest, "ستون 'آدرس' در فایل اکسل یافت نشد");

                    var birthProvinceCityColumn = workSheet.Cells["1:1"]
                   .FirstOrDefault(c => c.Value != null && c.Value.ToString().Replace(" ", string.Empty).CleanString() == "استان-شهر محل تولد".CleanString().Replace(" ", string.Empty));
                    if (birthProvinceCityColumn == null)
                        throw new AppException(ApiResultStatusCode.BadRequest, "ستون 'استان-شهر محل تولد' در فایل اکسل یافت نشد");

                    var issueProvinceCityColumn = workSheet.Cells["1:1"]
                        .FirstOrDefault(c => c.Value != null && c.Value.ToString().Replace(" ", string.Empty).CleanString() == "استان-شهر محل صدور".CleanString().Replace(" ", string.Empty));
                    if (issueProvinceCityColumn == null)
                        throw new AppException(ApiResultStatusCode.BadRequest, "ستون 'استان-شهر محل صدور' در فایل اکسل یافت نشد");

                    var addressProvinceCityColumn = workSheet.Cells["1:1"]
                        .FirstOrDefault(c => c.Value != null && c.Value.ToString().Replace(" ", string.Empty).CleanString() == "استان-شهر محل سکونت".CleanString().Replace(" ", string.Empty));
                    if (addressProvinceCityColumn == null)
                        throw new AppException(ApiResultStatusCode.BadRequest, "ستون 'استان-شهر محل سکونت' در فایل اکسل یافت نشد");

                    var customerNumberColumn = workSheet.Cells["1:1"]
                        .FirstOrDefault(c => c.Value != null && c.Value.ToString().Replace(" ", string.Empty).CleanString() == "شماره مشتری".CleanString().Replace(" ", string.Empty));
                    if (customerNumberColumn == null)
                        throw new AppException(ApiResultStatusCode.BadRequest, "ستون 'شماره مشتری' در فایل اکسل یافت نشد");

                    var accountNumberColumn = workSheet.Cells["1:1"]
                      .FirstOrDefault(c => c.Value != null && c.Value.ToString().Replace(" ", string.Empty).CleanString() == "شماره حساب".CleanString().Replace(" ", string.Empty));
                    if (accountNumberColumn == null)
                        throw new AppException(ApiResultStatusCode.BadRequest, "ستون 'شماره حساب' در فایل اکسل یافت نشد");

                    var cardNumberColumn = workSheet.Cells["1:1"]
                      .FirstOrDefault(c => c.Value != null && c.Value.ToString().Replace(" ", string.Empty).CleanString() == "شماره کارت".CleanString().Replace(" ", string.Empty));
                    if (cardNumberColumn == null)
                        throw new AppException(ApiResultStatusCode.BadRequest, "ستون 'شماره کارت' در فایل اکسل یافت نشد");

                    var ibanColumn = workSheet.Cells["1:1"]
                     .FirstOrDefault(c => c.Value != null && c.Value.ToString().Replace(" ", string.Empty).CleanString() == "شماره شبا".CleanString().Replace(" ", string.Empty));
                    if (ibanColumn == null)
                        throw new AppException(ApiResultStatusCode.BadRequest, "ستون 'شماره شبا' در فایل اکسل یافت نشد");

                    var bankNameColumn = workSheet.Cells["1:1"]
                     .FirstOrDefault(c => c.Value != null && c.Value.ToString().Replace(" ", string.Empty).CleanString() == "نام بانک".CleanString().Replace(" ", string.Empty));
                    if (bankNameColumn == null)
                        throw new AppException(ApiResultStatusCode.BadRequest, "ستون 'نام بانک' در فایل اکسل یافت نشد");

                    var facilityAmountColumn = workSheet.Cells["1:1"]
                     .FirstOrDefault(c => c.Value != null && c.Value.ToString().Replace(" ", string.Empty).CleanString() == "مبلغ وام(ريال)".CleanString().Replace(" ", string.Empty));
                    if (facilityAmountColumn == null)
                        throw new AppException(ApiResultStatusCode.BadRequest, "ستون 'مبلغ وام(ريال)' در فایل اکسل یافت نشد");


                    #endregion

                    var dataRowIndex = 2;
                    while (workSheet.Dimension.End.Row >= dataRowIndex)
                    {
                        item = new PlanMemberModel();
                        List<string> errorList = new List<string>();
                        //if (workSheet.Cells[dataRowIndex, nationalCodeColumn.Start.Column].Value == null ||
                        //    string.IsNullOrEmpty(workSheet.Cells[dataRowIndex, nationalCodeColumn.Start.Column].Value.ToString().CleanString().Replace(" ", string.Empty)))
                        //{
                        //    errorList.Add($"کد ملی وارد شده در سطر {dataRowIndex}ام با فرمت کد ملی مطابقت ندارد<br/>");
                        //    item.HasError = true;
                        //    continue;
                        //}
                        //if (!regexNationalCode.IsMatch(workSheet.Cells[dataRowIndex, nationalCodeColumn.Start.Column].Value.ToString()))
                        //{
                        //    item.NationalCode = Convert.ToString(workSheet.Cells[dataRowIndex, nationalCodeColumn.Start.Column].Value);
                        //    item.HasError = true;
                        //    errorList.Add($"کد ملی وارد شده در سطر {dataRowIndex}ام با فرمت کد ملی مطابقت ندارد<br/>");
                        //    continue;
                        //}
                        item.NationalCode = workSheet.Cells[dataRowIndex, nationalCodeColumn.Start.Column].Value.ToString().CleanString();
                        item.FName = CleanValue(Convert.ToString(workSheet.Cells[dataRowIndex, fnameColumn.Start.Column].Value));
                        item.LName = CleanValue(Convert.ToString(workSheet.Cells[dataRowIndex, lnameColumn.Start.Column].Value));
                        item.FatherName = CleanValue(Convert.ToString(workSheet.Cells[dataRowIndex, fatherNameColumn.Start.Column].Value));
                        item.Mobile = CleanValue(Convert.ToString(workSheet.Cells[dataRowIndex, mobileColumn.Start.Column].Value));
                        item.GenderText = CleanValue(Convert.ToString(workSheet.Cells[dataRowIndex, genderColumn.Start.Column].Value));
                        if (new string[] { "مرد", "زن" }.Contains(CleanValue(Convert.ToString(workSheet.Cells[dataRowIndex, genderColumn.Start.Column].Value))))
                            item.Gender = CleanValue(Convert.ToString(workSheet.Cells[dataRowIndex, genderColumn.Start.Column].Value)) == "مرد" ? GenderEnum.Male : GenderEnum.Female;

                        item.SSID = CleanValue(Convert.ToString(workSheet.Cells[dataRowIndex, ssidColumn.Start.Column].Value));
                        item.BirthCertificateSerial = CleanValue(Convert.ToString(workSheet.Cells[dataRowIndex, birthCertificateSerialColumn.Start.Column].Value));
                        item.PhoneNumber = CleanValue(Convert.ToString(workSheet.Cells[dataRowIndex, phoneNumberColumn.Start.Column].Value));
                        item.Address = CleanValue(Convert.ToString(workSheet.Cells[dataRowIndex, addressColumn.Start.Column].Value));
                        item.Email = CleanValue(Convert.ToString(workSheet.Cells[dataRowIndex, emailColumn.Start.Column].Value));
                        item.PostalCode = CleanValue(Convert.ToString(workSheet.Cells[dataRowIndex, postalCodeColumn.Start.Column].Value));
                        item.ShamsiBirthDate = CleanValue(Convert.ToString(workSheet.Cells[dataRowIndex, shamsiBirthDateColumn.Start.Column].Value));
                        item.AccountNumber = CleanValue(Convert.ToString(workSheet.Cells[dataRowIndex, accountNumberColumn.Start.Column].Value));
                        item.CardNumber = CleanValue(Convert.ToString(workSheet.Cells[dataRowIndex, cardNumberColumn.Start.Column].Value));
                        item.CustomerNumber = CleanValue(Convert.ToString(workSheet.Cells[dataRowIndex, customerNumberColumn.Start.Column].Value));
                        item.IBAN = CleanValue(Convert.ToString(workSheet.Cells[dataRowIndex, ibanColumn.Start.Column].Value));
                        item.BankName = CleanValue(Convert.ToString(workSheet.Cells[dataRowIndex, bankNameColumn.Start.Column].Value));

                        long facilityAmount;
                        if (CleanValue(Convert.ToString(workSheet.Cells[dataRowIndex, facilityAmountColumn.Start.Column].Value)) != null &&
                            long.TryParse(CleanValue(Convert.ToString(workSheet.Cells[dataRowIndex, facilityAmountColumn.Start.Column].Value)), out facilityAmount))
                            item.FacilityAmount = facilityAmount;

                        var locations = await locationService.GetAll(cancellationToken);
                        #region Extract Birth City and Province
                        var birthCityAndProvince = CleanValue(Convert.ToString(workSheet.Cells[dataRowIndex, birthProvinceCityColumn.Start.Column].Value));
                        if (birthCityAndProvince != null && birthCityAndProvince.Split('-').Length == 2)
                        {
                            item.ProvinceOfBirthName = birthCityAndProvince.Split('-')[0];
                            item.ProvinceOfBirthId = locations.Any(p => p.Name.CleanString().Replace(" ", string.Empty) == birthCityAndProvince.Split('-')[0].Replace(" ", string.Empty) && p.LocationType == LocationTypeEnum.Province)
                                ? locations.FirstOrDefault(p => p.Name.CleanString().Replace(" ", string.Empty) == birthCityAndProvince.Split('-')[0].Replace(" ", string.Empty) && p.LocationType == LocationTypeEnum.Province).Id
                                : null;

                            item.CityOfBirthName = birthCityAndProvince.Split('-')[1];
                            item.CityOfBirthId = locations.Any(p => p.Name.CleanString().Replace(" ", string.Empty) == birthCityAndProvince.Split('-')[1].Replace(" ", string.Empty) && p.LocationType == LocationTypeEnum.City)
                                ? locations.FirstOrDefault(p => p.Name.CleanString().Replace(" ", string.Empty) == birthCityAndProvince.Split('-')[1].Replace(" ", string.Empty) && p.LocationType == LocationTypeEnum.City).Id
                                : null;
                        }
                        #endregion Extract Birth City and Province

                        #region Extract Issue City and Province
                        var issueCityAndProvince = CleanValue(Convert.ToString(workSheet.Cells[dataRowIndex, issueProvinceCityColumn.Start.Column].Value));
                        if (issueCityAndProvince != null && issueCityAndProvince.Split('-').Length == 2)
                        {
                            item.ProvinceOfIssueName = issueCityAndProvince.Split('-')[0];
                            item.ProvinceOfIssueId = locations.Any(p => p.Name.CleanString().Replace(" ", string.Empty) == issueCityAndProvince.Split('-')[0].Replace(" ", string.Empty) && p.LocationType == LocationTypeEnum.Province)
                                ? locations.FirstOrDefault(p => p.Name.CleanString().Replace(" ", string.Empty) == issueCityAndProvince.Split('-')[0].Replace(" ", string.Empty) && p.LocationType == LocationTypeEnum.Province).Id
                                : null;

                            item.CityOfIssueName = issueCityAndProvince.Split('-')[1];
                            item.CityOfIssueId = locations.Any(p => p.Name.CleanString().Replace(" ", string.Empty) == issueCityAndProvince.Split('-')[1].Replace(" ", string.Empty) && p.LocationType == LocationTypeEnum.City)
                                ? locations.FirstOrDefault(p => p.Name.CleanString().Replace(" ", string.Empty) == issueCityAndProvince.Split('-')[1].Replace(" ", string.Empty) && p.LocationType == LocationTypeEnum.City).Id
                                : null;
                        }
                        #endregion Extract Issue City and Province

                        #region Extract Address City and Province
                        var addressCityAndProvince = CleanValue(Convert.ToString(workSheet.Cells[dataRowIndex, addressProvinceCityColumn.Start.Column].Value));
                        if (addressCityAndProvince != null && addressCityAndProvince.Split('-').Length == 2)
                        {
                            item.ProvinceName = addressCityAndProvince.Split('-')[0];
                            item.ProvinceId = locations.Any(p => p.Name.CleanString().Replace(" ", string.Empty) == addressCityAndProvince.Split('-')[0].Replace(" ", string.Empty) && p.LocationType == LocationTypeEnum.Province)
                                ? locations.FirstOrDefault(p => p.Name.CleanString().Replace(" ", string.Empty) == addressCityAndProvince.Split('-')[0].Replace(" ", string.Empty) && p.LocationType == LocationTypeEnum.Province).Id
                                : null;

                            item.CityName = addressCityAndProvince.Split('-')[1];
                            item.CityId = locations.Any(p => p.Name.CleanString().Replace(" ", string.Empty) == addressCityAndProvince.Split('-')[1].Replace(" ", string.Empty) && p.LocationType == LocationTypeEnum.City)
                                ? locations.FirstOrDefault(p => p.Name.CleanString().Replace(" ", string.Empty) == addressCityAndProvince.Split('-')[1].Replace(" ", string.Empty) && p.LocationType == LocationTypeEnum.City).Id
                                : null;
                        }
                        #endregion Extract Extract City and Province

                        try
                        {
                            if (item.BirthDate.HasValue)
                            {
                                DateTime zeroTime = new DateTime(1, 1, 1);
                                if ((zeroTime + (DateTime.Now - item.BirthDate.Value)).Year - 1 < 18)
                                    errorList.Add("حداقل سن برای درخواست تسهیلات 18 سال می باشد!");
                            }
                            else
                            {
                                errorList.Add("تاریخ تولد نامشخص است!");
                            }
                        }
                        catch (Exception exp)
                        {
                            errorList.Add(exp.Message);
                        }

                        var context = new ValidationContext(item, serviceProvider: null, items: null);
                        var validationResults = new List<ValidationResult>();
                        // var validateResult = item.Validate(context).Where(p => p != null).ToList();
                        if (!Validator.TryValidateObject(item, context, validationResults, true))
                        {
                            foreach (var error in validationResults)
                            {
                                if (!string.IsNullOrEmpty(error.ErrorMessage))
                                    errorList.Add($"{error.ErrorMessage}");
                            }
                        }

                        //validationResults = item.Validate(context).Where(p => p != null).ToList();
                        //if (validationResults.Any())
                        //{
                        //    foreach (var error in validationResults)
                        //    {
                        //        if (!string.IsNullOrEmpty(error.ErrorMessage))
                        //            errorList.Add($"{error.ErrorMessage}");
                        //    }
                        //}

                        if (errorList.Any())
                        {
                            item.ErrorMessage = string.Join("<br/>", errorList);
                            item.HasError = true;
                            //throw new AppException(item.ErrorMessage);
                        }
                        else
                        {
                            item.ErrorMessage = string.Empty;
                            item.HasError = false;
                        }

                        result.Add(item);
                        dataRowIndex++;
                    }
                }
                return result;
            }
        }

        //public async Task<OrganizationModel> PrepareModelForEdit(int Id, CancellationToken cancellationToken)
        //{
        //    var organization = await organizationRepository.GetByIdAsync(cancellationToken, Id);

        //    if (organization == default(Organization))
        //        return null;

        //    return new OrganizationModel()
        //    {
        //        Id = organization.Id,
        //        Address = organization.Address,
        //        Name = organization.Name,
        //        Tel = organization.Tel,
        //        SiteUrl = organization.SiteUrl,
        //        NationalId = organization.NationalId,
        //        IsActive = organization.IsActive,
        //        OrganizationTypeId = organization.OrganizationTypeId,
        //        OrganizationTypes = (await organizationTypeRepository.SelectByAsync(p => p.IsActive.Equals(true),
        //            p => new SelectListItem
        //            {
        //                Text = p.Name,
        //                Value = p.Id.ToString(),
        //                Selected = p.Id == organization.OrganizationTypeId
        //            }, cancellationToken))
        //            .OrderBy(p => p.Text)
        //            .ToList(),
        //        IsEditMode = true
        //    };
        //}
    }
}
