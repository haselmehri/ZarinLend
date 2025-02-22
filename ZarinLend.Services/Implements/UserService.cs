using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common;
using Common.Exceptions;
using Common.Utilities;
using Core.Data.Repositories;
using Core.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Services.Dto;
using Services.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ZarinLend.Common.LocalizationResource;
using ZarinLend.Services.Model.Ayandeh.BankAccount;
using static Common.Enums;
using UserDto = Services.Dto.UserDto;

namespace Services
{
    public class UserService : IUserService, IScopedDependency
    {
        private readonly IMapper mapper;
        private readonly string domain;
        private readonly IUserRepository userRepository;
        private readonly IBaseRepository<UserIdentityDocument> userIdentityDocumentRepository;
        private readonly IRequestFacilityWorkFlowStepRepository requestFacilityWorkFlowStepRepository;
        private readonly IPersonRepository personRepository;
        private readonly UserManager<User> userManager;
        private readonly IJwtService jwtService;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IBaseRepository<Bank> bankRepository;
        private readonly IBaseRepository<Location> locationRepository;
        private readonly IBaseRepository<OrganizationType> organizationTypeRepository;
        private readonly IOrganizationRepository organizationRepository;
        private readonly ISmsService smsService;
        private readonly IWalletTransactionService walletTransactionService;
        private readonly IRequestFacilityGuarantorWorkFlowStepRepository requestFacilityGuarantorWorkFlowStepRepository;
        private readonly IBaseRepository<UserVAPID> userVapidRepository;
        private readonly IBaseRepository<JobTitle> jobTitleRepository;
        private readonly IBaseRepository<SalaryRange> salaryRangeRepository;
        private readonly INeginHubService neginHubService;
        private readonly IBaseRepository<UserBankAccount> userBankAccountRepository;
        private readonly IFinnotechService finnotechService;
        private readonly RoleManager<Role> roleManager;
        private readonly ILogger<UserService> logger;

        public UserService(IMapper mapper,
                           IUserRepository userRepository,
                           IBaseRepository<UserIdentityDocument> userIdentityDocumentRepository,
                           UserManager<User> userManager,
                           IRequestFacilityWorkFlowStepRepository requestFacilityWorkFlowStepRepository,
                           IPersonRepository personRepository,
                           IJwtService jwtService,
                           IWebHostEnvironment webHostEnvironment,
                           IBaseRepository<Bank> bankRepository,
                           IBaseRepository<Location> locationRepository,
                           IBaseRepository<OrganizationType> organizationTypeRepository,
                           IOrganizationRepository organizationRepository,
                           ISmsService smsService,
                           IWalletTransactionService walletTransactionService,
                           IRequestFacilityGuarantorWorkFlowStepRepository requestFacilityGuarantorWorkFlowStepRepository,
                           IBaseRepository<UserVAPID> userVapidRepository,
                           IBaseRepository<JobTitle> jobTitleRepository,
                           IBaseRepository<SalaryRange> salaryRangeRepository,
                           INeginHubService neginHubService,
                           IBaseRepository<UserBankAccount> userBankAccountRepository,
                           IFinnotechService finnotechService,
                           RoleManager<Role> roleManager,
                           IOptions<SiteSettings> siteSettings,
                           ILogger<UserService> logger)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            this.mapper = mapper;
            this.userRepository = userRepository;
            this.userIdentityDocumentRepository = userIdentityDocumentRepository;
            this.requestFacilityWorkFlowStepRepository = requestFacilityWorkFlowStepRepository;
            this.personRepository = personRepository;
            this.userManager = userManager;
            this.jwtService = jwtService;
            this.webHostEnvironment = webHostEnvironment;
            this.bankRepository = bankRepository;
            this.locationRepository = locationRepository;
            this.organizationTypeRepository = organizationTypeRepository;
            this.organizationRepository = organizationRepository;
            this.smsService = smsService;
            this.walletTransactionService = walletTransactionService;
            this.requestFacilityGuarantorWorkFlowStepRepository = requestFacilityGuarantorWorkFlowStepRepository;
            this.userVapidRepository = userVapidRepository;
            this.jobTitleRepository = jobTitleRepository;
            this.salaryRangeRepository = salaryRangeRepository;
            this.neginHubService = neginHubService;
            this.userBankAccountRepository = userBankAccountRepository;
            this.finnotechService = finnotechService;
            this.roleManager = roleManager;
            this.logger = logger;
            domain = siteSettings.Value.Domain;
        }

        public virtual async Task<TokenModel> Token(LoginWithUserPassModel model, CancellationToken cancellationToken = default)
        {
            var user = await userManager.Users
                .Include(p => p.Person)
                .Include(p => p.Person.Organization)
                .FirstOrDefaultAsync(u => u.UserName == model.Username, cancellationToken);
            //var user = (await userRepository.SelectByAsync(p => p.UserName == model.NationalCode, cancellationToken, true, null, null, p => p.Person, p => p.Person.Organization))
            //    .FirstOrDefault();

            if (user == default(User))
                throw new LogicException(ResourceFile.InvalidUsernameOrPassword);

            if (!user.IsActive)
                throw new AppException(ResourceFile.AccountIsDisable);

            var isPasswordValid = await userManager.CheckPasswordAsync(user, model.Password);
            if (!isPasswordValid)
                throw new LogicException(ResourceFile.InvalidUsernameOrPassword);

            return await jwtService.GenerateAsync(user);
        }

        public virtual async Task<TokenModel> Token(string userName, CancellationToken cancellationToken = default)
        {
            //var user = await userManager.Users
            //    .Where(u => u.UserName == userName)
            //    .Include(p => p.Person)
            //    .Include(p => p.Person.Organization)
            //    .Select(p => p)
            //    .FirstOrDefaultAsync( cancellationToken);

            //var user1 = await userManager.Users
            //    .Include(p => p.Person)
            //    .Include(p => p.Person.Organization)
            //    .Where(u => u.UserName == userName)
            //    .Select(p => p)
            //    .FirstOrDefaultAsync(cancellationToken);

            var user = await userRepository.TableNoTracking
               .Include(p => p.Person)
               .Include(p => p.Person.Organization)
               .Where(p => p.UserName == userName)
               .Select(p => p)
               .FirstOrDefaultAsync(cancellationToken);

            if (user == default(User))
                throw new LogicException(ResourceFile.InvalidUsernameOrPassword);

            if (!user.IsActive)
                throw new AppException(ResourceFile.AccountIsDisable);

            return await jwtService.GenerateAsync(user);
        }
        public async Task SaveVAPID(UserVAPIDModel model, CancellationToken cancellationToken)
        {
            if (!await userVapidRepository.TableNoTracking.AnyAsync(p => p.Endpoint == model.Endpoint, cancellationToken))
            {
                await userVapidRepository.AddAsync(new UserVAPID()
                {
                    Auth = model.Auth,
                    Endpoint = model.Endpoint,
                    P256dh = model.P256dh,
                    UserId = model.UserId,
                    OsName = model.OsName,
                    IsMobile = model.IsMobile,
                    Platform = model.Platform,
                    AppName = model.AppName,
                    AppVersion = model.AppVersion,
                    UserAgent = model.UserAgent,
                    AppCodeName = model.AppCodeName,
                    IsActive = true
                }, cancellationToken);
            }
        }
        public async Task SendOtpForLogin(OtpModel model, CancellationToken cancellationToken = default)
        {
            var user = await userRepository.TableNoTracking.FirstOrDefaultAsync(p => p.Person.Mobile == model.Mobile, cancellationToken);

            if (user == default(User))
                throw new LogicException("براساس شماره موبایل فوق کاربری یافت نشد!");

            if (!user.IsActive)
                throw new AppException(ResourceFile.AccountIsDisable);

            Random rnd = new Random();
            var otp = string.Empty;
            for (int j = 0; j < 5; j++)
            {
                otp = $"{otp}{rnd.Next(0, 9)}";
            }

            user.Otp = otp;
            user.OtpStartTime = DateTime.Now;
            user.OtpExpireTime = DateTime.Now.AddSeconds(125);

            await userRepository.UpdateCustomPropertiesAsync(user, cancellationToken, true,
                nameof(User.Otp),
                nameof(User.OtpStartTime),
                nameof(User.OtpExpireTime));

            //var smsContent = $"کد تایید : {otp}{Environment.NewLine} @{domain} #{otp}";
            var smsContent = $"کد تایید : {otp}";
            var messageId = await smsService.Send(model.Mobile, smsContent, cancellationToken);
        }

        public async Task<string> SendOtpForRegister(OtpModel model, CancellationToken cancellationToken = default)
        {
            var user = await userRepository.TableNoTracking.FirstOrDefaultAsync(p => p.Person.Mobile == model.Mobile, cancellationToken);

            if (user != default(User))
                throw new LogicException("شماره موبایل فوق قبلا در سیستم ثبت شده است!");

            Random rnd = new Random();
            var otp = string.Empty;
            for (int j = 0; j < 5; j++)
            {
                otp = $"{otp}{rnd.Next(0, 9)}";
            }

            //var smsContent = $"کد تایید : {otp}{Environment.NewLine} @{domain} #{otp}";
            var smsContent = $"کد تایید : {otp}";
            var messageId = await smsService.Send(model.Mobile, smsContent, cancellationToken);
            return otp;
        }

        public async Task GenerateResetPasswordOtp(ResetPasswordModel model, CancellationToken cancellationToken = default)
        {
            var user = await userRepository.TableNoTracking
                .FirstOrDefaultAsync(p => p.Person.Mobile == model.Mobile && p.Person.NationalCode == model.NationalCode, cancellationToken);

            if (user == default(User))
                throw new LogicException("براساس کد ملی و شماره موبایل فوق کاربری یافت نشد!");

            if (!user.IsActive)
                throw new AppException(ResourceFile.AccountIsDisable);

            Random rnd = new Random();
            var otp = string.Empty;
            for (int j = 0; j < 5; j++)
            {
                otp = $"{otp}{rnd.Next(0, 9)}";
            }

            user.Otp = otp;
            user.OtpStartTime = DateTime.Now;
            user.OtpExpireTime = DateTime.Now.AddSeconds(185);

            await userRepository.UpdateCustomPropertiesAsync(user, cancellationToken, true,
                nameof(User.Otp),
                nameof(User.OtpStartTime),
            nameof(User.OtpExpireTime));

            //var smsContent =   $"کد تایید برای بازیابی کلمه عبور : {otp}{Environment.NewLine} @{domain} #{otp}";
            var smsContent = $"کد تایید برای بازیابی کلمه عبور : {otp}";
            var messageId = await smsService.Send(model.Mobile, smsContent, cancellationToken);
        }
        public async Task<TokenModeWithUser> GenerateTokenWithMobile(LoginWithOtpModel model, CancellationToken cancellationToken = default)
        {
            var user = await userRepository.TableNoTracking
                .FirstOrDefaultAsync(p => p.Person.Mobile == model.Mobile && p.Otp == model.Otp && p.OtpStartTime < DateTime.Now && p.OtpExpireTime > DateTime.Now, cancellationToken);

            if (user == default(User))
                throw new LogicException("براساس شماره موبایل و رمز یکبار مصرف وارد شده، اطلاعاتی یافت نشد!");

            if (!user.IsActive)
                throw new AppException(ResourceFile.AccountIsDisable);

            return new TokenModeWithUser { Token = await jwtService.GenerateAsync(user), User = user };
        }

        public virtual async Task<IdentityResult> ResetPassword(ForgotPasswordModel model, CancellationToken cancellationToken = default)
        {
            var user = await userRepository.Table
                .FirstOrDefaultAsync(p => p.Person.Mobile == model.Mobile &&
                                          p.Person.NationalCode == model.NationalCode &&
                                          p.Otp == model.Otp &&
                                          p.OtpStartTime < DateTime.Now &&
                                          p.OtpExpireTime > DateTime.Now,
                                          cancellationToken);
            if (user == null)
                throw new BadRequestException("براساس کد ملی و شماره موبایل فوق کاربری یافت نشد!");

            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            return await userManager.ResetPasswordAsync(user, token, model.Password);
        }

        public async Task<List<SelectListItem>> GetOrganizationUsers(int organizationId, CancellationToken cancellationToken)
        {
            var organizationUser = (await userRepository.SelectByAsync(p => p.Person.OrganizationId.Equals(organizationId) && p.Person.IsActive && p.IsActive,
                    p => new SelectListItem
                    {
                        Text = $"{p.Person.FName} {p.Person.LName}({p.UserName})",
                        Value = p.Id.ToString()
                    }, cancellationToken))
                    .OrderBy(p => p.Text)
                    .ToList();

            return organizationUser;
        }

        public virtual async Task<string> GetMobile(Guid userId, CancellationToken cancellationToken = default)
        {
            return await userRepository.TableNoTracking.Where(p => p.Id.Equals(userId)).Select(p => p.Person.Mobile).FirstOrDefaultAsync();
        }
        public virtual async Task<PagingDto<UserListModel>> SelectUsers(PagingFilterDto filter, CancellationToken cancellationToken = default)
        {
            var users = userRepository.TableNoTracking;

            if (filter != null && filter.FilterList != null)
            {
                foreach (var item in filter.FilterList)
                {
                    switch (item.PropertyName)
                    {
                        case "FName":
                            {
                                string propertyValue = item.PropertyValue;
                                propertyValue = propertyValue.CleanString().Replace(" ", string.Empty);
                                if (!string.IsNullOrEmpty(propertyValue))
                                    users = users.Where(p => p.Person.FName.Replace(" ", string.Empty).Contains(propertyValue));
                                break;
                            }
                        case "LName":
                            {
                                string propertyValue = item.PropertyValue;
                                propertyValue = propertyValue.CleanString().Replace(" ", string.Empty);
                                if (!string.IsNullOrEmpty(propertyValue))
                                    users = users.Where(p => p.Person.LName.Replace(" ", string.Empty).Contains(propertyValue));
                                break;
                            }
                        case "NationalCode":
                            {
                                string propertyValue = item.PropertyValue;
                                users = users.Where(p => p.Person.NationalCode.Contains(propertyValue));
                                break;
                            }
                        case "RoleId":
                            {
                                Guid roleId = new Guid(item.PropertyValue);
                                users = users.Where(p => p.UserRoles.Any(x => x.RoleId == roleId));
                                break;
                            }
                        case "UseHasActiveRequestFacility":
                            {
                                users = users.Where(p => p.RequestFacilityBuyers.Any(x => !x.CancelByUser &&
                                                                                          x.RequestFacilityWorkFlowSteps.Any(c => !c.StatusId.HasValue)));
                                break;
                            }
                        case "UserInstallApp":
                            {
                                users = users.Where(p => p.UserVAPIDs.Any());
                                break;
                            }
                        //case "StartDate":
                        //    {
                        //        DateTime propertyValue = Convert.ToDateTime(item.PropertyValue);
                        //        requestFacilities = requestFacilities.Where(p => p.CreatedDate.Date >= propertyValue.Date);
                        //        break;
                        //    }
                        //case "EndDate":
                        //    {
                        //        DateTime propertyValue = Convert.ToDateTime(item.PropertyValue);
                        //        requestFacilities = requestFacilities.Where(p => p.CreatedDate.Date <= propertyValue.Date);
                        //        break;
                        //    }
                        default:
                            break;
                    }
                }
            }

            var result = await users
                .OrderBy(p => p.Id)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize != 0 ? filter.PageSize : int.MaxValue)
                .Include(i => i.IranCreditScorings)
                .Select(p => new UserListModel()
                {
                    FName = p.Person.FName,
                    LName = p.Person.LName,
                    Id = p.Id,
                    Mobile = p.Person.Mobile,
                    OrganizationName = p.Person.Organization != null ? p.Person.Organization.Name : null,
                    OrganizationTypeName = p.Person.Organization != null ? p.Person.Organization.OrganizationType.Name : null,
                    UserName = p.UserName,
                    RoleName = p.UserRoles.Any() ? p.UserRoles.First().Role.Name : null,
                    CreatedDate = p.CreatedDate,
                    IsActive = p.IsActive,
                    IranCardScore = p.IranCreditScorings.FirstOrDefault() == null 
                                    ? 0 
                                    : p.IranCreditScorings.FirstOrDefault().Score.HasValue 
                                        ? p.IranCreditScorings.FirstOrDefault().Score.Value 
                                        : 0
                })
                .ToListAsync(cancellationToken);
            result = result.Select(s => { s.CreatedDatePersian = DateTimeHelper.GregorianToShamsi(s.CreatedDate); return s; }).ToList();

            return new PagingDto<UserListModel>()
            {
                CurrentPage = filter.Page,
                Data = result,
                TotalRowCount = users.Count(),
                TotalPages = (int)Math.Ceiling(Convert.ToDouble(users.Count()) / filter.PageSize)
            };
        }

        public virtual async Task<bool> AddUser(UserAddEditModelByAdmin userModel, CancellationToken cancellationToken = default)
        {
            if (await userRepository.TableNoTracking.AnyAsync(p => p.UserName == userModel.UserName))
                throw new AppException(ApiResultStatusCode.BadRequest, ResourceFile.MessageErrorExistUsername, System.Net.HttpStatusCode.BadRequest);

            if (await userRepository.TableNoTracking.AnyAsync(p => p.Person.NationalCode == userModel.NationalCode))
                throw new AppException(ApiResultStatusCode.BadRequest, ResourceFile.MessageErrorExistUsername, System.Net.HttpStatusCode.BadRequest);

            if (await userRepository.TableNoTracking.AnyAsync(p => p.Person.Mobile == userModel.Mobile))
                throw new AppException(ApiResultStatusCode.BadRequest, ResourceFile.MessageErrorExistMobile, System.Net.HttpStatusCode.BadRequest);

            if (!string.IsNullOrEmpty(userModel.Email) && await userRepository.TableNoTracking.AnyAsync(p => p.Email == userModel.Email))
                throw new AppException(ApiResultStatusCode.BadRequest, ResourceFile.MessageErrorExistEmail, System.Net.HttpStatusCode.BadRequest);

            var user = new User()
            {
                UserName = userModel.UserName,
                Email = userModel.Email,
                IsActive = userModel.IsActive,
                PhoneNumber = userModel.PhoneNumber,
                Person = new Person()
                {
                    Address = userModel.Address,
                    FName = userModel.FName,
                    LName = userModel.LName,
                    FatherName = userModel.FatherName,
                    SSID = userModel.SSID,
                    Gender = userModel.Gender,
                    Mobile = userModel.Mobile,
                    //MobileConfirmed = false,
                    //MobileShahkarConfirmed = false,
                    NationalCode = userModel.NationalCode,
                    PostalCode = userModel.PostalCode,
                    CityId = userModel.CityId,
                    BirthDate = userModel.BirthDate,
                    CountryId = 1,//Iran
                    OrganizationId = userModel.OrganizationId,
                    IsActive = userModel.IsActive,
                }
            };
            var result = await userManager.CreateAsync(user, userModel.Password);
            if (result.Succeeded)
            {
                #region Add User to Role(s)
                var role = await roleManager.FindByIdAsync(userModel.RoleId.ToString());

                await userManager.AddToRoleAsync(user, role.Name);
                #endregion
                logger.LogInformation("User Created By Admin");
                return true;
            }
            else
                return false;

            throw new AppException(string.Join("<br/>", result.Errors.Select(p => p.Description)));
        }

        public virtual async Task<bool> EditUser(UserAddEditModelByAdmin userModel, CancellationToken cancellationToken = default)
        {
            if (await userRepository.TableNoTracking.AnyAsync(p => p.UserName == userModel.UserName && !p.Id.Equals(userModel.Id)))
                throw new AppException(ResourceFile.MessageErrorExistUsername);

            if (await userRepository.TableNoTracking.AnyAsync(p => p.Person.NationalCode == userModel.NationalCode && !p.Id.Equals(userModel.Id)))
                throw new AppException(ResourceFile.MessageErrorExistNationalCode);

            if (await userRepository.TableNoTracking.AnyAsync(p => p.Person.Mobile == userModel.Mobile && !p.Id.Equals(userModel.Id)))
                throw new AppException(ResourceFile.MessageErrorExistMobile);

            if (!string.IsNullOrEmpty(userModel.Email) && await userRepository.TableNoTracking.AnyAsync(p => p.Email == userModel.Email && !p.Id.Equals(userModel.Id)))
                throw new AppException(ResourceFile.MessageErrorExistEmail);

            //var user = await userManager.FindByIdAsync(userModel.Id.ToString());
            var user = (await userRepository.SelectByAsync(p => p.Id.Equals(userModel.Id), cancellationToken, false, null, null,
                p => p.Person,
                p => p.Person.Organization))
                .FirstOrDefault();

            if (user == default(User))
                throw new NotFoundException();

            user.UserName = userModel.UserName;
            user.IsActive = userModel.IsActive;
            user.PhoneNumber = userModel.PhoneNumber;
            user.Email = userModel.Email;

            user.Person.NationalCode = userModel.UserName;
            user.Person.Mobile = userModel.Mobile;
            user.Person.FName = userModel.FName;
            user.Person.LName = userModel.LName;
            user.Person.FatherName = userModel.FatherName;
            user.Person.SSID = userModel.SSID;
            user.Person.BirthDate = userModel.BirthDate;
            user.Person.Gender = userModel.Gender;
            user.Person.Address = userModel.Address;
            user.PhoneNumber = userModel.PhoneNumber;
            user.Person.PostalCode = userModel.PostalCode;
            user.Person.CityId = userModel.CityId;
            user.Person.OrganizationId = userModel.OrganizationId;            

            var result = await userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                //TODO : working on performance
                var role = await roleManager.FindByIdAsync(userModel.RoleId.ToString());
                var currentRoles = await userManager.GetRolesAsync(user);
                if (!currentRoles.Any())
                {
                    await userManager.AddToRolesAsync(user, new[] { role.Name });
                }
                else if (!currentRoles.Any(p => p == role.Name))
                {
                    await userManager.RemoveFromRolesAsync(user, currentRoles);
                    await userManager.AddToRolesAsync(user, new[] { role.Name });
                }
                else
                {
                    //current role and update role is equal
                }

                logger.LogInformation("User Update By Admin");
                return true;
            }
            else
                return false;

            throw new AppException(string.Join("<br/>", result.Errors.Select(p => p.Description)));
        }

        public virtual async Task<UserSearchResultModel> SearchUser(UserFilterModel filter, CancellationToken cancellationToken = default)
        {
            var user = (await userRepository.SelectByAsync(u => u.Person.NationalCode == filter.NationalCode &&
                    u.UserRoles.Any(p => p.Role.Name == RoleEnum.Buyer.ToString()) &&
                    u.RequestFacilityBuyers.Any(x => !x.CancelByUser &&
                        x.RequestFacilityWorkFlowSteps.Any(c => c.WorkFlowStep.IsApproveFinalStep &&
                                                                c.WorkFlowStep.IsLastStep &&
                                                                c.StatusId.HasValue &&
                                                                c.StatusId.Value == (short)StatusEnum.Approved)),
                    p => new
                    {
                        p.Id,
                        p.Person.FName,
                        p.Person.LName,
                        p.Person.NationalCode,
                        p.IsActive,
                        p.Person.Mobile,
                        //UserIdentityDocuments = p.UserIdentityDocuments.Any() ? p.UserIdentityDocuments.ToList() : null,
                    }, cancellationToken))
                    .FirstOrDefault();

            if (user != null)
            {
                //throw new LogicException("با کد ملی فوق کاربری یافت نشد");

                if (!user.IsActive)
                    throw new AppException("حساب کاربری خریدار غیرفعال می باشد!");

                return new UserSearchResultModel()
                {
                    Id = user.Id,
                    FName = user.FName,
                    LName = user.LName,
                    NationalCode = user.NationalCode,
                    Mobile = user.Mobile,
                    //UserIdentityDocuments = user.UserIdentityDocuments != null
                    //    ? user.UserIdentityDocuments.Select(x => new DocumentModel()
                    //    {
                    //        FilePath = x.FilePath,
                    //        IsDeleted = x.IsDeleted,
                    //        IsPrivate = x.IsPrivate,
                    //        Status = x.Status,
                    //        FileType = x.FileType,
                    //        DocumentType = x.DocumentType
                    //    }).ToList()
                    //    : null,
                    WalletBalanceBaseCardNumber = await walletTransactionService.GetWalletBalanceBaseCards(user.Id, cancellationToken)
                };
            }

            return null;
        }

        //public virtual async Task<string> GeneratePasswordResetToken(ForgotPasswordModel model, CancellationToken cancellationToken = default)
        //{
        //    var user = await userManager.FindByEmailAsync(model.Email);
        //    if (user == null)
        //        throw new BadRequestException("حساب کاربری با ایمیل وارد شده یافت نشد!");

        //    //if (!await userManager.IsEmailConfirmedAsync(user))
        //    //    throw new BadRequestException("ایمیل تایید نشده است!!");

        //    // For more information on how to enable account confirmation and password reset please 
        //    // visit https://go.microsoft.com/fwlink/?LinkID=532713
        //    var code = await userManager.GeneratePasswordResetTokenAsync(user);
        //    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        //    return code;
        //}

        public virtual async Task<List<UserSelectDto>> Get(CancellationToken cancellationToken = default)
        {
            var users = await userRepository.TableNoTracking
                .Include(p => p.UserRoles)
                    .ThenInclude(p => p.Role)
                .ProjectTo<UserSelectDto>(mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return users;
        }
        public virtual async Task<UserEditModel> Get(Guid id, CancellationToken cancellationToken)
        {
            #region old code to mapper
            //var user = await userRepository.TableNoTracking
            //    .Include(p => p.UserRoles)
            //        .ThenInclude(p => p.Role)
            //    .SingleOrDefaultAsync(p => p.Id == id, cancellationToken);

            //if (user == null)
            //    return null;

            //var userDto = mapper.Map<UserSelectDto>(user);

            //return userDto;
            #endregion

            var user = await userManager.FindByIdAsync(id.ToString());
            if (user == default(User))
                return null;

            return new UserEditModel()
            {
                Address = user.Person.Address,
                Email = user.Email,
                FName = user.Person.FName,
                LName = user.Person.LName,
                FatherName = user.Person.FatherName,
                Mobile = user.Person.Mobile,
                NationalCode = user.Person.NationalCode,
                PhoneNumber = user.PhoneNumber,
                UserName = user.UserName,
                PostalCode = user.Person.PostalCode,
                CityId = user.Person.CityId,
                ProvinceId = user.Person.CityId.HasValue ? user.Person.City.Parent.Id : null,
                IsEditMode = true
            };
        }

        public virtual async Task<UserRegisterByZarinpalResultModel> RegisterByZarinpal(UserRegisterByZarinpalModel userModel, CancellationToken cancellationToken = default)
        {
            var currentUser = await userRepository.TableNoTracking.
                Include(p => p.Person).
                Where(p => p.Person.NationalCode == userModel.NationalCode)
                .Select(p => p)
                .FirstOrDefaultAsync(cancellationToken);
            if (currentUser != null)
            {
                var result = new UserRegisterByZarinpalResultModel()
                {
                    User = currentUser,
                    ExistUser = true
                };
                if (!currentUser.Person.ZP_Id.HasValue)
                {
                    await personRepository.UpdateCustomPropertiesAsync(new Person() { Id = currentUser.PersonId, ZP_Id = userModel.ZP_Id }, cancellationToken,
                        true, nameof(Person.ZP_Id));
                    result.ZP_Id = userModel.ZP_Id;
                }
                return result;
            }
            else
            {
                if (await userRepository.TableNoTracking.AnyAsync(p => p.UserName == userModel.UserName))
                    throw new AppException(ApiResultStatusCode.BadRequest, ResourceFile.MessageErrorExistUsername, System.Net.HttpStatusCode.BadRequest);

                //if (await userRepository.TableNoTracking.AnyAsync(p => p.Person.NationalCode == userModel.NationalCode))
                //    throw new AppException(ApiResultStatusCode.BadRequest, sharedCultureLocalizer["MessageErrorExistNationalCode"], System.Net.HttpStatusCode.BadRequest);

                if (await userRepository.TableNoTracking.AnyAsync(p => p.Person.Mobile == userModel.Mobile))
                    throw new AppException(ApiResultStatusCode.BadRequest, ResourceFile.MessageErrorExistMobile, System.Net.HttpStatusCode.BadRequest);

                if (!string.IsNullOrEmpty(userModel.Email) && await userRepository.TableNoTracking.AnyAsync(p => p.Email == userModel.Email))
                    throw new AppException(ApiResultStatusCode.BadRequest, ResourceFile.MessageErrorExistEmail, System.Net.HttpStatusCode.BadRequest);

                var newUser = new User()
                {
                    UserName = userModel.UserName,
                    Email = userModel.Email,
                    EmailConfirmed = false,
                    IsActive = true,
                    PhoneNumber = userModel.PhoneNumber,
                    Person = new Person()
                    {
                        ZP_Id = userModel.ZP_Id,
                        Address = userModel.Address,
                        FName = userModel.FName,
                        LName = userModel.LName,
                        Mobile = userModel.Mobile,
                        NationalCode = userModel.NationalCode,
                        PostalCode = userModel.PostalCode,
                        VerifiedPostalCode = userModel.VerifiedPostCode,
                        BirthDate = userModel.BirthDate,

                        CountryId = 1,//Iran
                        MobileConfirmed = false,
                        MobileShahkarConfirmed = false,
                        CityId = null,
                        Gender = null,
                        CustomerNumber = string.Empty,
                        HashCardNumber = string.Empty,
                        FatherName = string.Empty,
                        SSID = string.Empty,
                    }
                };
                var result = await userManager.CreateAsync(newUser, userModel.Password);
                if (result.Succeeded)
                {
                    #region Add User to Role(s)

                    if ((await userManager.AddToRolesAsync(newUser, new[] { RoleEnum.Buyer.ToString() })).Succeeded)
                        return new UserRegisterByZarinpalResultModel()
                        {
                            User = newUser
                        };

                    #endregion
                    logger.LogInformation("Register New User from Zarinpal ");
                }

                throw new AppException(string.Join("<br/>", result.Errors.Select(p => p.Description)));
            }
        }

        public virtual async Task<User> QuickRegister(UserQuickRegisterModel userModel, CancellationToken cancellationToken = default)
        {
            if (await userRepository.TableNoTracking.AnyAsync(p => p.UserName == userModel.UserName))
                throw new AppException(ApiResultStatusCode.BadRequest, ResourceFile.MessageErrorExistUsername, System.Net.HttpStatusCode.BadRequest);

            if (await userRepository.TableNoTracking.AnyAsync(p => p.Person.NationalCode == userModel.NationalCode))
                throw new AppException(ApiResultStatusCode.BadRequest, ResourceFile.MessageErrorExistNationalCode, System.Net.HttpStatusCode.BadRequest);

            if (await userRepository.TableNoTracking.AnyAsync(p => p.Person.Mobile == userModel.Mobile))
                throw new AppException(ApiResultStatusCode.BadRequest, ResourceFile.MessageErrorExistMobile, System.Net.HttpStatusCode.BadRequest);

            var user = new User()
            {
                UserName = userModel.UserName,
                IsActive = true,
                Person = new Person()
                {
                    Address = null,
                    Mobile = userModel.Mobile,
                    MobileConfirmed = true,
                    MobileShahkarConfirmed = false,
                    NationalCode = userModel.NationalCode,
                    BirthDate = userModel.BirthDate,
                    CountryId = 1,//Iran,
                    FName = string.Empty,
                    LName = string.Empty,
                    FatherName = string.Empty,
                    SSID = string.Empty
                }
            };
            var result = await userManager.CreateAsync(user, userModel.Password);
            if (result.Succeeded)
            {
                #region Add User to Role(s)
                if ((await userManager.AddToRolesAsync(user, new[] { RoleEnum.Buyer.ToString() })).Succeeded)
                    return user;
                #endregion
                logger.LogInformation("Buyer User Created ");
            }

            throw new AppException(string.Join("<br/>", result.Errors.Select(p => p.Description)));
        }
        public virtual async Task<User> RegisterBuyer(UserAddModel userModel, CancellationToken cancellationToken = default)
        {
            if (await userRepository.TableNoTracking.AnyAsync(p => p.UserName == userModel.UserName))
                throw new AppException(ApiResultStatusCode.BadRequest, ResourceFile.MessageErrorExistUsername, System.Net.HttpStatusCode.BadRequest);

            if (await userRepository.TableNoTracking.AnyAsync(p => p.Person.NationalCode == userModel.NationalCode))
                throw new AppException(ApiResultStatusCode.BadRequest, ResourceFile.MessageErrorExistNationalCode, System.Net.HttpStatusCode.BadRequest);

            if (await userRepository.TableNoTracking.AnyAsync(p => p.Person.Mobile == userModel.Mobile))
                throw new AppException(ApiResultStatusCode.BadRequest, ResourceFile.MessageErrorExistMobile, System.Net.HttpStatusCode.BadRequest);

            if (!string.IsNullOrEmpty(userModel.Email) && await userRepository.TableNoTracking.AnyAsync(p => p.Email == userModel.Email))
                throw new AppException(ApiResultStatusCode.BadRequest, ResourceFile.MessageErrorExistEmail, System.Net.HttpStatusCode.BadRequest);

            var user = new User()
            {
                UserName = userModel.UserName,
                Email = userModel.Email,
                EmailConfirmed = false,
                IsActive = true,
                PhoneNumber = userModel.PhoneNumber,
                Person = new Person()
                {
                    Address = userModel.Address,
                    FName = userModel.FName,
                    LName = userModel.LName,
                    FatherName = userModel.FatherName,
                    SSID = userModel.SSID,
                    Gender = userModel.Gender,
                    Mobile = userModel.Mobile,
                    MobileConfirmed = false,
                    MobileShahkarConfirmed = false,
                    NationalCode = userModel.NationalCode,
                    PostalCode = userModel.PostalCode,
                    CityId = userModel.CityId,
                    //BankId = userModel.BankId,
                    BirthDate = userModel.BirthDate,
                    CountryId = 1,//Iran
                    CustomerNumber = userModel.CustomerNumber,
                    //AccountNumber = userModel.AccountNumber,
                    //CardNumber = userModel.CardNumber,
                    HashCardNumber = SecurityHelper.GetSha256Hash(userModel.CardNumber),
                    //IBAN = userModel.IBAN,
                    //BankName = userModel.BankName,
                    //DepositOwners = userModel.DepositOwners,
                    //DepositStatus = userModel.DepositStatus
                }
            };
            var result = await userManager.CreateAsync(user, userModel.Password);
            if (result.Succeeded)
            {
                #region Add User to Role(s)
                //if (!await roleManager.RoleExistsAsync(RoleEnum.Buyer.ToString()))
                //{
                //    result = await roleManager.CreateAsync(new Role() { Name = "User", Description = "User" });
                //    if (result.Succeeded)
                //    {
                //        if ((await userManager.AddToRolesAsync(user, new[] { "User" })).Succeeded)
                //            return user;
                //    }
                //}
                //else
                //{
                if ((await userManager.AddToRolesAsync(user, new[] { RoleEnum.Buyer.ToString() })).Succeeded)
                    return user;
                //}
                #endregion
                logger.LogInformation("Buyer User Created ");
            }

            throw new AppException(string.Join("<br/>", result.Errors.Select(p => p.Description)));
        }
        public virtual async Task<UserEditResult> Update(UserEditModel userModel, CancellationToken cancellationToken = default)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            if (await userRepository.TableNoTracking.AnyAsync(p => p.UserName == userModel.UserName && !p.Id.Equals(userModel.Id)))
                throw new AppException(ResourceFile.MessageErrorExistUsername);

            if (await userRepository.TableNoTracking.AnyAsync(p => p.Person.NationalCode == userModel.NationalCode && !p.Id.Equals(userModel.Id)))
                throw new AppException(ResourceFile.MessageErrorExistNationalCode);

            var user = (await userRepository.SelectByAsync(p => p.Id == userModel.Id, cancellationToken,
                navigationPropertyPaths: p => p.Person))
                .FirstOrDefault();

            if (user == default(User))
                throw new NotFoundException();

            user.Person.FName = userModel.FName;
            user.Person.LName = userModel.LName;
            user.Person.FatherName = userModel.FatherName;
            user.Person.SSID = userModel.SSID;
            user.Person.BirthDate = userModel.BirthDate;
            user.Person.Gender = userModel.Gender;
            user.Person.Address = userModel.Address;
            user.PhoneNumber = userModel.PhoneNumber;
            user.Person.PostalCode = userModel.PostalCode;
            //user.Person.AccountNumber = userModel.AccountNumber;
            //user.Person.CardNumber = userModel.CardNumber;
            user.Person.HashCardNumber = SecurityHelper.GetSha256Hash(userModel.CardNumber);
            //user.Person.IBAN = userModel.IBAN;
            //user.Person.BankName = userModel.BankName;
            //user.Person.DepositStatus = userModel.DepositStatus;
            //user.Person.DepositOwners = userModel.DepositOwners;
            user.Person.CityId = userModel.CityId;
            //user.Person.BirthLocationId = userModel.CityOfBirthId;

            var nationalCodeChanged = user.Person.NationalCode != userModel.NationalCode;
            if (nationalCodeChanged)
                user.Person.NationalCode = userModel.NationalCode;

            var usernameChanged = user.UserName != userModel.UserName;
            if (usernameChanged)
                user.UserName = userModel.UserName;

            var emailChanged = user.Email != userModel.Email;
            if (emailChanged)
            {
                user.Email = userModel.Email;
                user.EmailConfirmed = false;
            }

            var mobileChanged = user.Person.Mobile != userModel.Mobile;
            if (mobileChanged)
            {
                user.Person.Mobile = userModel.Mobile;
                user.Person.MobileConfirmed = false;
                user.Person.MobileShahkarConfirmed = false;
            }

            await userRepository.UpdateAsync(user, cancellationToken, false);
            if (userModel.RequestFacilityId.HasValue)
            {
                await requestFacilityWorkFlowStepRepository.ChangeCurrentStepAndGoToNextStep(userModel.RequestFacilityId.Value,
                    WorkFlowFormEnum.RegisterIdentityInfo,
                    StatusEnum.Approved,
                    buyerId: userModel.Id,
                    opratorId: userModel.Id,
                    "تایید اتوماتیک بعد از بروزرسانی هویتی",
                    cancellationToken);
            }
            else
                await userRepository.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Buyer User Updated ");
            return new UserEditResult()
            {
                EmailChanged = emailChanged,
                MobileChanged = mobileChanged,
                NationalCodeChanged = nationalCodeChanged,
                UsernameChanged = usernameChanged,
                EnsureSignOut = usernameChanged,
                Token = (await jwtService.GenerateAsync(user)).Token
            };
        }

        public virtual async Task<bool> GoToNextStepFromProfileToUploadDocuments(Guid userId, int requestFacilityId, CancellationToken cancellationToken = default)
        {
            var person = await userRepository.TableNoTracking.Where(p => p.Id == userId).Select(p => p.Person).FirstOrDefaultAsync(cancellationToken);
            if (person != null)
            {
                if (!person.CityId.HasValue ||
                    string.IsNullOrEmpty(person.PostalCode) ||
                    string.IsNullOrEmpty(person.Address))
                    throw new AppException($"لطفا فیلدهای اجباری در بخش <b>اطلاعات تماس و محل اقامت</b> را وارد کنید<br/>-استان و شهر محل سکونت<br/>-کد پستی<br/>-آدرس محل سکونت");


                if (string.IsNullOrEmpty(person.CustomerNumber) ||
                    !await userBankAccountRepository.TableNoTracking.AnyAsync(p => p.UserId == userId, cancellationToken))
                    throw new AppException($"لطفا فیلدهای اجباری در بخش <b>اطلاعات حساب بانکی</b> را وارد کنید<br/>-با انتخاب گزینه ویرایش و انتخاب کلید دریافت حساب ها شماره حساب مورد نظر را انتخاب کنید");
            }
            await requestFacilityWorkFlowStepRepository.ChangeCurrentStepAndGoToNextStep(requestFacilityId,
                WorkFlowFormEnum.RegisterIdentityInfo,
                StatusEnum.Approved,
                buyerId: userId,
                opratorId: userId,
                "رفتن به مرحله بعد بدون تغییر اطلاعات هویتی",
                cancellationToken);

            return true;
        }

        public virtual async Task<bool> GoToNextStepFromProfileToUploadDocumentsInGuarantor(Guid userId, int requestFacilityId, CancellationToken cancellationToken = default)
        {
            var person = await userRepository.TableNoTracking.Where(p => p.Id == userId).Select(p => p.Person).FirstOrDefaultAsync(cancellationToken);
            if (person != null)
            {
                if (string.IsNullOrEmpty(person.FatherName) ||
                    string.IsNullOrEmpty(person.SSID) ||
                    !person.Gender.HasValue)
                    throw new AppException($"لطفا فیلدهای اجباری در بخش <b>اطلاعات هویتی</b> را وارد کنید<br/>-نام پدر<br/>-جنسیت<br/>-شماره شناسامه");

                if (!person.CityId.HasValue)
                    throw new AppException($"لطفا فیلدهای اجباری در بخش <b>اطلاعات تماس و محل اقامت</b> را وارد کنید<br/>-استان و شهر محل سکونت");
            }
            await requestFacilityGuarantorWorkFlowStepRepository.ChangeCurrentStepAndGoToNextStep(requestFacilityId,
                WorkFlowFormEnum.EditGuarantorInfo,
                StatusEnum.Approved,
                guarantorUserId: userId,
                opratorId: userId,
                "رفتن به مرحله بعد از مرحله اطلاعات هویتی",
                cancellationToken);

            return true;
        }
        public virtual async Task<bool> GoToNextStepFromUploadDocumentsToVerifyZarinLendIfPossible(Guid userId, int requestFacilityId, CancellationToken cancellationToken = default)
        {
            //if (await userIdentityDocumentRepository.TableNoTracking.AnyAsync(p => p.UserId == userId && p.DocumentType == DocumentType.BirthCertificatePage1 && !p.IsDeleted, cancellationToken) &&
            //    await userIdentityDocumentRepository.TableNoTracking.AnyAsync(p => p.UserId == userId && p.DocumentType == DocumentType.BirthCertificateDescription && !p.IsDeleted, cancellationToken) &&
            //    await userIdentityDocumentRepository.TableNoTracking.AnyAsync(p => p.UserId == userId && p.DocumentType == DocumentType.NationalCardFront && !p.IsDeleted, cancellationToken) &&
            //    await userIdentityDocumentRepository.TableNoTracking.AnyAsync(p => p.UserId == userId && p.DocumentType == DocumentType.NationalCardBack && !p.IsDeleted, cancellationToken) &&
            //    await userIdentityDocumentRepository.TableNoTracking.AnyAsync(p => p.UserId == userId && p.DocumentType == DocumentType.JobDocument && !p.IsDeleted, cancellationToken) &&
            //    await userIdentityDocumentRepository.TableNoTracking.AnyAsync(p => p.UserId == userId && p.DocumentType == DocumentType.AddressDocument && !p.IsDeleted, cancellationToken))
            //{
            //    await requestFacilityWorkFlowStepRepository.ChangeCurrentStepAndGoToNextStep(requestFacilityId,
            //        WorkFlowFormEnum.UploadIdentityDocuments,
            //        StatusEnum.Approved,
            //        buyerId: userId,
            //        opratorId: userId,
            //        "رفتن به مرحله بعد بدون تغییر مدارک ",
            //        cancellationToken);

            //    return true;
            //}
            //return false;
            if (await userIdentityDocumentRepository.TableNoTracking.AnyAsync(p => p.UserId == userId && p.DocumentType == DocumentType.NationalCardFront && !p.IsDeleted, cancellationToken) &&
                await userIdentityDocumentRepository.TableNoTracking.AnyAsync(p => p.UserId == userId && p.DocumentType == DocumentType.NationalCardBack && !p.IsDeleted, cancellationToken))
            {
                await requestFacilityWorkFlowStepRepository.ChangeCurrentStepAndGoToNextStep(requestFacilityId,
                    WorkFlowFormEnum.UploadIdentityDocuments,
                    StatusEnum.Approved,
                    buyerId: userId,
                    opratorId: userId,
                    "رفتن به مرحله بعد بدون تغییر مدارک ",
                    cancellationToken);

                return true;
            }
            return false;
        }
        public virtual async Task<bool> GoToNextStepFromUploadDocumentsToVerifyZarinLend(Guid userId, int requestFacilityId, CancellationToken cancellationToken = default)
        {
            //if (!await userIdentityDocumentRepository.TableNoTracking.AnyAsync(p => p.UserId == userId && p.DocumentType == DocumentType.BirthCertificatePage1 && !p.IsDeleted, cancellationToken))
            //    throw new AppException("تصویر صفحه اول شناسامه بارگذاری نشده است،لطفا قبل از رفتن به مرحله بعد مدارک خود را بارگذاری کنید");

            //if (!await userIdentityDocumentRepository.TableNoTracking.AnyAsync(p => p.UserId == userId && p.DocumentType == DocumentType.BirthCertificateDescription && !p.IsDeleted, cancellationToken))
            //    throw new AppException("تصویر صفحه توضیحات شناسامه بارگذاری نشده است،لطفا قبل از رفتن به مرحله بعد مدارک خود را بارگذاری کنید");

            if (!await userIdentityDocumentRepository.TableNoTracking.AnyAsync(p => p.UserId == userId && p.DocumentType == DocumentType.NationalCardFront && !p.IsDeleted, cancellationToken))
                throw new AppException("تصویر روی کارت ملی بارگذاری نشده است،لطفا قبل از رفتن به مرحله بعد مدارک خود را بارگذاری کنید");

            if (!await userIdentityDocumentRepository.TableNoTracking.AnyAsync(p => p.UserId == userId && p.DocumentType == DocumentType.NationalCardBack && !p.IsDeleted, cancellationToken))
                throw new AppException("تصویر پشت کارت ملی بارگذاری نشده است،لطفا قبل از رفتن به مرحله بعد مدارک خود را بارگذاری کنید");

            //if (!await userIdentityDocumentRepository.TableNoTracking.AnyAsync(p => p.UserId == userId && p.DocumentType == DocumentType.JobDocument && !p.IsDeleted, cancellationToken))
            //    throw new AppException("تصویر مدارک شغلی بارگذاری نشده است،لطفا قبل از رفتن به مرحله بعد مدارک خود را بارگذاری کنید");

            //if (!await userIdentityDocumentRepository.TableNoTracking.AnyAsync(p => p.UserId == userId && p.DocumentType == DocumentType.AddressDocument && !p.IsDeleted, cancellationToken))
            //    throw new AppException("تصویر سند مالکیت یا اجاره نامه محل سکونت بارگذاری نشده است،لطفا قبل از رفتن به مرحله بعد مدارک خود را بارگذاری کنید");

            await requestFacilityWorkFlowStepRepository.ChangeCurrentStepAndGoToNextStep(requestFacilityId,
                WorkFlowFormEnum.UploadIdentityDocuments,
                StatusEnum.Approved,
                buyerId: userId,
                opratorId: userId,
                "رفتن به مرحله بعد بدون تغییر مدارک ",
                cancellationToken);

            return true;
        }

        public virtual async Task<bool> GoToNextStepFromUploadDocumentsToVerifyZarinLendInGuarantor(Guid userId, int requestFacilityGuarantorId, CancellationToken cancellationToken = default)
        {
            //if (!await userIdentityDocumentRepository.TableNoTracking.AnyAsync(p => p.UserId == userId && p.DocumentType == DocumentType.BirthCertificatePage1 && !p.IsDeleted, cancellationToken))
            //    throw new AppException("تصویر صفحه اول شناسامه بارگذاری نشده است،لطفا قبل از رفتن به مرحله بعد مدارک خود را بارگذاری کنید");

            //if (!await userIdentityDocumentRepository.TableNoTracking.AnyAsync(p => p.UserId == userId && p.DocumentType == DocumentType.BirthCertificateDescription && !p.IsDeleted, cancellationToken))
            //    throw new AppException("تصویر صفحه توضیحات شناسامه بارگذاری نشده است،لطفا قبل از رفتن به مرحله بعد مدارک خود را بارگذاری کنید");

            if (!await userIdentityDocumentRepository.TableNoTracking.AnyAsync(p => p.UserId == userId && p.DocumentType == DocumentType.NationalCardFront && !p.IsDeleted, cancellationToken))
                throw new AppException("تصویر روی کارت ملی بارگذاری نشده است،لطفا قبل از رفتن به مرحله بعد مدارک خود را بارگذاری کنید");

            if (!await userIdentityDocumentRepository.TableNoTracking.AnyAsync(p => p.UserId == userId && p.DocumentType == DocumentType.NationalCardBack && !p.IsDeleted, cancellationToken))
                throw new AppException("تصویر پشت کارت ملی بارگذاری نشده است،لطفا قبل از رفتن به مرحله بعد مدارک خود را بارگذاری کنید");

            //if (!await userIdentityDocumentRepository.TableNoTracking.AnyAsync(p => p.UserId == userId && p.DocumentType == DocumentType.JobDocument && !p.IsDeleted, cancellationToken))
            //    throw new AppException("تصویر مدارک شغلی بارگذاری نشده است،لطفا قبل از رفتن به مرحله بعد مدارک خود را بارگذاری کنید");

            //if (!await userIdentityDocumentRepository.TableNoTracking.AnyAsync(p => p.UserId == userId && p.DocumentType == DocumentType.AddressDocument && !p.IsDeleted, cancellationToken))
            //    throw new AppException("تصویر سند مالکیت یا اجاره نامه محل سکونت بارگذاری نشده است،لطفا قبل از رفتن به مرحله بعد مدارک خود را بارگذاری کنید");

            await requestFacilityGuarantorWorkFlowStepRepository.ChangeCurrentStepAndGoToNextStep(requestFacilityGuarantorId,
                WorkFlowFormEnum.UploadIdentityDocumentsGuarantor,
                StatusEnum.Approved,
                guarantorUserId: userId,
                opratorId: userId,
                "رفتن به مرحله بعد بدون تغییر مدارک ",
                cancellationToken);

            return true;
        }

        public virtual async Task<UserEditResult> UpdateUserInfo(UserInfoEditModel userModel, CancellationToken cancellationToken = default)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            if (await userRepository.TableNoTracking.AnyAsync(p => p.UserName == userModel.UserName && !p.Id.Equals(userModel.Id)))
                throw new AppException(ResourceFile.MessageErrorExistUsername);

            if (await userRepository.TableNoTracking.AnyAsync(p => p.Person.NationalCode == userModel.NationalCode && !p.Id.Equals(userModel.Id)))
                throw new AppException(ResourceFile.MessageErrorExistNationalCode);

            var user = (await userRepository.SelectByAsync(p => p.Id == userModel.Id, cancellationToken,
                navigationPropertyPaths: p => p.Person))
                .FirstOrDefault();

            if (user == default(User))
                throw new NotFoundException();

            user.Person.FName = userModel.FName;
            user.Person.LName = userModel.LName;
            user.Person.FatherName = userModel.FatherName;
            user.Person.SSID = userModel.SSID;
            //user.Person.SanaNumber = userModel.SanaNumber;
            //user.Person.BirthCertificateSerial = userModel.BirthCertificateSerial;
            user.Person.BirthDate = userModel.BirthDate;
            user.Person.Gender = userModel.Gender;
            //user.Person.CityOfIssueId = userModel.CityOfIssueId;
            //user.Person.BirthLocationId = userModel.CityOfBirthId;

            var nationalCodeChanged = false;// user.Person.NationalCode != userModel.NationalCode;
            //if (nationalCodeChanged)
            //    user.Person.NationalCode = userModel.NationalCode;

            var usernameChanged = false;// user.UserName != userModel.UserName;
            //if (usernameChanged)
            //    user.UserName = userModel.UserName;

            await userRepository.UpdateAsync(user, cancellationToken, true);

            logger.LogInformation("Buyer User Updated ");
            return new UserEditResult()
            {
                EmailChanged = false,
                MobileChanged = false,
                NationalCodeChanged = nationalCodeChanged,
                UsernameChanged = usernameChanged,
                EnsureSignOut = usernameChanged,
                //Token = (await jwtService.GenerateAsync(user)).Token
            };
        }

        public virtual async Task<UserEditResult> UpdateUserLocation(UserLocationEditModel userModel, CancellationToken cancellationToken = default)
        {
            var user = (await userRepository.SelectByAsync(p => p.Id == userModel.Id, cancellationToken,
                navigationPropertyPaths: p => p.Person))
                .FirstOrDefault();

            if (await userRepository.TableNoTracking.AnyAsync(p => p.Person.Mobile == userModel.Mobile && !p.Id.Equals(userModel.Id)))
                throw new AppException(ResourceFile.MessageErrorExistMobile);

            if (!string.IsNullOrEmpty(userModel.Email) && await userRepository.TableNoTracking.AnyAsync(p => p.Email == userModel.Email && !p.Id.Equals(userModel.Id)))
                throw new AppException(ResourceFile.MessageErrorExistEmail);

            if (user == default(User))
                throw new NotFoundException();

            if (userModel.Address == userModel.VerifiedAddress)
                user.Person.IsAddressValidated = true;
            user.Person.VerifiedAddress = userModel.VerifiedAddress;
            user.Person.Address = userModel.Address;
            user.PhoneNumber = userModel.PhoneNumber;
            user.Person.PostalCode = userModel.PostalCode;
            user.Person.CityId = userModel.CityId;

            var emailChanged = user.Email != userModel.Email;
            if (emailChanged)
            {
                user.Email = userModel.Email;
                user.EmailConfirmed = false;
            }

            var mobileChanged = user.Person.Mobile != userModel.Mobile;
            string token = null;
            if (mobileChanged)
            {
                user.Person.Mobile = userModel.Mobile;
                user.Person.MobileConfirmed = false;
                user.Person.MobileShahkarConfirmed = false;
                //token = (await jwtService.GenerateAsync(user)).Token;
            }

            await userRepository.UpdateAsync(user, cancellationToken, true);

            logger.LogInformation("Buyer User Updated ");
            return new UserEditResult()
            {
                EmailChanged = emailChanged,
                MobileChanged = mobileChanged,
                NationalCodeChanged = false,
                UsernameChanged = false,
                EnsureSignOut = false,
                //Token = token
            };
        }

        //public virtual async Task<bool> UpdateBankAccount(UserBankAccountEditModel userModel, CancellationToken cancellationToken = default)
        //{
        //    var user = (await userRepository.SelectByAsync(p => p.Id == userModel.Id, cancellationToken,
        //        navigationPropertyPaths: p => p.Person))
        //        .FirstOrDefault();

        //    if (user == default(User))
        //        throw new NotFoundException();

        //    user.Person.AccountNumber = userModel.AccountNumber;
        //    user.Person.CustomerNumber = userModel.CustomerNumber;
        //    user.Person.CardNumber = userModel.CardNumber;
        //    user.Person.HashCardNumber = SecurityHelper.GetSha256Hash(userModel.CardNumber);
        //    user.Person.IBAN = userModel.IBAN;
        //    user.Person.BankName = userModel.BankName;
        //    user.Person.DepositStatus = userModel.DepositStatus;
        //    user.Person.DepositOwners = userModel.DepositOwners;

        //    await userRepository.UpdateAsync(user, cancellationToken, true);

        //    logger.LogInformation("Buyer User Updated ");
        //    return true;
        //}

        public virtual async Task<string> GetCustomerNumber(Guid userId, CancellationToken cancellationToken = default)
        {
            var result = await userManager.Users.AsNoTracking().Where(p => p.Id == userId).Select(p => new
            {
                p.PersonId,
                p.Person.CustomerNumber,
                p.Person.NationalCode
            })
                .FirstOrDefaultAsync(cancellationToken);
            if (!string.IsNullOrEmpty(result!.CustomerNumber))
                return result.CustomerNumber;

            var customerNumber = await finnotechService.CifInquiry(result.NationalCode, userId, cancellationToken);
            await personRepository.UpdateCustomPropertiesAsync(new Person()
            {
                Id = result.PersonId,
                CustomerNumber = customerNumber
            },
            cancellationToken,
            saveNow: true,
            nameof(Person.CustomerNumber));

            return customerNumber;
        }
        public virtual async Task<BankAccountModel?> AddCardNumber(Guid userId, [NotNull] BankAccountAddModel model, CancellationToken cancellationToken = default)
        {
            if (await userBankAccountRepository.TableNoTracking.AnyAsync(p => p.CardNumber == model.CardNumber, cancellationToken))
                throw new AppException("شماره کارت تکراری است!");

            var result = await neginHubService.CardToIBAN(model.CardNumber, userId, cancellationToken);
            if (result != null)
            {
                if (result.IsSuccess)
                {
                    var userInfo = await userManager.Users.AsNoTracking().Where(p => p.Id == userId).Select(p => new { p.Person.NationalCode, p.Person.BirthDate }).FirstOrDefaultAsync(cancellationToken);
                    var cardAndNationalCodeIsMatch = await neginHubService.NationalCodeAndCardVerification(model.CardNumber,
                                                                                                           userInfo!.NationalCode!,
                                                                                                           userInfo.BirthDate.GregorianToShamsi(_separator: string.Empty),
                                                                                                           userId,
                                                                                                           cancellationToken);
                    if (cardAndNationalCodeIsMatch == null)
                        throw new AppException("خطا در استعلام کد ملی و شماره کارت!");
                    if (cardAndNationalCodeIsMatch == false)
                        throw new AppException("کد ملی و شماره کارت با یکدیگر مطابقت ندارد!");

                    var fullName = string.Empty;
                    if (result.DepositOwners.Any())
                    {
                        foreach (var depositOwner in result.DepositOwners)
                        {
                            if (fullName != string.Empty)
                                fullName += " | ";
                            if (!string.IsNullOrEmpty(depositOwner.FirstName))
                                fullName = depositOwner.FirstName.Trim();
                            if (!string.IsNullOrEmpty(depositOwner.LastName))
                                fullName += $" {depositOwner.FirstName.Trim()}";
                        }
                    }
                    await userBankAccountRepository.AddAsync(new UserBankAccount()
                    {
                        BankName = result.BankName,
                        CardNumber = result.Card,
                        Deposit = result.Deposit,
                        DepositOwner = fullName,
                        DepositStatus = result.DepositStatus,
                        IBAN = result.IBAN,
                        IsConfirm = true,
                        //NeginHubLogId = result.NeginHubLogId,
                        UserId = userId,
                    }, cancellationToken);

                    return new BankAccountModel
                    {
                        IBAN = result.IBAN,
                        DepositOwners = fullName,
                        DepositStatus = result.DepositStatus,
                        Deposit = result.Deposit,
                        CardNumber = result.Card,
                        CreateDate = DateTime.UtcNow,
                        BankName = result.BankName,
                        IsConfirm = true
                    };
                }
                else
                    throw new AppException(result.ErrorMessage);
            }
            throw new AppException("خطا در استعلام شماره کارت!");
        }

        public async Task<UserIdentityDocumentModel?> GetUserIdentityDocument(Guid userId, CancellationToken cancellationToken)
        {
            var userIdentityDocument = new UserIdentityDocumentModel();
            var identityDocuments = await userIdentityDocumentRepository.SelectByAsync(p => p.UserId.Equals(userId) && !p.IsDeleted, cancellationToken);

            userIdentityDocument.BirthCertificatePage1 = identityDocuments.Any(p => p.DocumentType == DocumentType.BirthCertificatePage1)
                ? identityDocuments.Where(p => p.DocumentType == DocumentType.BirthCertificatePage1).Select(p => new DocumentModel
                {
                    CreatedDate = p.CreatedDate,
                    DocumentType = DocumentType.BirthCertificatePage1,
                    FilePath = p.FilePath,
                    FileType = FileType.Image,
                    Id = p.Id,
                    Status = p.Status,
                    Version = p.Version
                }).First()
                : null;

            userIdentityDocument.BirthCertificateDescription = identityDocuments.Any(p => p.DocumentType == DocumentType.BirthCertificateDescription)
                ? identityDocuments.Where(p => p.DocumentType == DocumentType.BirthCertificateDescription).Select(p => new DocumentModel
                {
                    CreatedDate = p.CreatedDate,
                    DocumentType = DocumentType.BirthCertificateDescription,
                    FilePath = p.FilePath,
                    FileType = FileType.Image,
                    Id = p.Id,
                    Status = p.Status,
                    Version = p.Version
                }).First()
                : null;

            userIdentityDocument.NationalCardFront = identityDocuments.Any(p => p.DocumentType == DocumentType.NationalCardFront)
                ? identityDocuments.Where(p => p.DocumentType == DocumentType.NationalCardFront).Select(p => new DocumentModel
                {
                    CreatedDate = p.CreatedDate,
                    DocumentType = DocumentType.NationalCardFront,
                    FilePath = p.FilePath,
                    FileType = FileType.Image,
                    Id = p.Id,
                    Status = p.Status,
                    Version = p.Version
                }).First()
                : null;

            userIdentityDocument.NationalCardBack = identityDocuments.Any(p => p.DocumentType == DocumentType.NationalCardBack)
                ? identityDocuments.Where(p => p.DocumentType == DocumentType.NationalCardBack).Select(p => new DocumentModel
                {
                    CreatedDate = p.CreatedDate,
                    DocumentType = DocumentType.NationalCardBack,
                    FilePath = p.FilePath,
                    FileType = FileType.Image,
                    Id = p.Id,
                    Status = p.Status,
                    Version = p.Version
                }).First()
                : null;

            userIdentityDocument.JobDocuments = identityDocuments.Any(p => p.DocumentType == DocumentType.JobDocument)
                ? identityDocuments.Where(p => p.DocumentType == DocumentType.JobDocument).Select(p => new DocumentModel
                {
                    CreatedDate = p.CreatedDate,
                    DocumentType = DocumentType.JobDocument,
                    FilePath = p.FilePath,
                    FileType = FileType.Image,
                    Id = p.Id,
                    Status = p.Status,
                    Version = p.Version
                })
                .ToList()
                : null;

            userIdentityDocument.AddressDocuments = identityDocuments.Any(p => p.DocumentType == DocumentType.AddressDocument)
              ? identityDocuments.Where(p => p.DocumentType == DocumentType.AddressDocument).Select(p => new DocumentModel
              {
                  CreatedDate = p.CreatedDate,
                  DocumentType = DocumentType.AddressDocument,
                  FilePath = p.FilePath,
                  FileType = FileType.Image,
                  Id = p.Id,
                  Status = p.Status,
                  Version = p.Version
              })
              .ToList()
              : null;

            userIdentityDocument.AccountStatement = identityDocuments.Any(p => p.DocumentType == DocumentType.AccountStatement)
             ? identityDocuments.Where(p => p.DocumentType == DocumentType.AccountStatement).Select(p => new DocumentModel
             {
                 CreatedDate = p.CreatedDate,
                 DocumentType = DocumentType.AccountStatement,
                 FilePath = p.FilePath,
                 FileType = FileType.Image,
                 Id = p.Id,
                 Status = p.Status,
                 Version = p.Version
             })
             .First()
             : null;

            userIdentityDocument.Banks = (await bankRepository.SelectByAsync(p => p.IsActive.Equals(true), cancellationToken))
                .Select(p => new SelectListItem
                {
                    Text = p.Name,
                    Value = p.Id.ToString(),
                    Selected = identityDocuments.Any(p => p.DocumentType == DocumentType.AccountStatement) &&
                                       identityDocuments.First(p => p.DocumentType == DocumentType.AccountStatement).BankId.HasValue &&
                                       identityDocuments.First(p => p.DocumentType == DocumentType.AccountStatement).BankId == p.Id
                                ? true
                                : false,
                })
                .OrderBy(p => p.Text)
                .ToList();

            return userIdentityDocument;
        }

        public async Task UploadIdentityDocuments(Guid userId, int? bankId, IFormFile BirthCertificatePage1, IFormFile BirthCertificateDescription,
            IFormFile NationalCardFront, IFormFile NationalCardBack, IFormFile AccountStatement, List<IFormFile> JobDocument, List<IFormFile> AddressDocument, List<int> deleteFileIDs,
            CancellationToken cancellationToken = default)
        {
            var documentTypeFilePairs = new Dictionary<DocumentType, List<IFormFile>>()
            {
               {DocumentType.BirthCertificatePage1,BirthCertificatePage1 != null ? new List<IFormFile>{ BirthCertificatePage1 } : null},
               {DocumentType.BirthCertificateDescription,BirthCertificateDescription != null ?  new List<IFormFile>{BirthCertificateDescription }:null },
               {DocumentType.NationalCardFront,NationalCardFront != null ?  new List<IFormFile>{NationalCardFront }:null },
               {DocumentType.NationalCardBack,NationalCardBack != null ?  new List<IFormFile>{NationalCardBack } : null },
               {DocumentType.AccountStatement,AccountStatement != null ?  new List<IFormFile>{AccountStatement } : null },
               {DocumentType.JobDocument, JobDocument},
               {DocumentType.AddressDocument,  AddressDocument}
            };

            var userIdentityDocumentPath = @"UploadFiles\UserIdentityDocument";
            string uploadFolder = Path.Combine(webHostEnvironment.WebRootPath, userIdentityDocumentPath);
            foreach (var documentType in documentTypeFilePairs.Keys)
            {
                #region Save File On Disk & Insert/Update Record In Database
                var files = documentTypeFilePairs[documentType];
                if (files == null) continue;

                var userIdentityDocuments = await userIdentityDocumentRepository
                    .SelectByAsync(p => p.UserId == userId && p.DocumentType == documentType && !p.IsDeleted, cancellationToken);
                if (userIdentityDocuments != null && userIdentityDocuments.Any()) //Edit Mode
                {
                    if (userIdentityDocuments.Count == files.Count)
                    {
                        for (int i = 0; i < userIdentityDocuments.Count; i++)
                        {
                            if (File.Exists($@"{uploadFolder}\{Path.GetFileName(userIdentityDocuments[i].FilePath)}"))
                                File.Delete($@"{uploadFolder}\{Path.GetFileName(userIdentityDocuments[i].FilePath)}");

                            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(files[i].FileName)}";
                            var relativePath = $"/UploadFiles/UserIdentityDocument/{fileName}";
                            string filePath = Path.Combine(uploadFolder, fileName);
                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await files[i].CopyToAsync(fileStream);

                                userIdentityDocuments[i].Version += 1;
                                userIdentityDocuments[i].FilePath = relativePath;
                                userIdentityDocuments[i].FileType = FileExtensions.GetFileType(relativePath);
                                userIdentityDocuments[i].BankId = documentType == DocumentType.AccountStatement ? bankId : null;
                                await userIdentityDocumentRepository
                                    .UpdateCustomPropertiesAsync(userIdentityDocuments[i], cancellationToken, true,
                                                                 nameof(UserIdentityDocument.Version),
                                                                 nameof(UserIdentityDocument.FileType),
                                                                 nameof(UserIdentityDocument.FilePath),
                                                                 nameof(UserIdentityDocument.BankId));
                            }
                            if (deleteFileIDs != null && deleteFileIDs.Any(p => p == userIdentityDocuments[i].Id))
                                deleteFileIDs.Remove(userIdentityDocuments[i].Id);
                        }
                    }
                    else if (userIdentityDocuments.Count < files.Count)
                    {
                        for (int i = 0; i < userIdentityDocuments.Count; i++)
                        {
                            if (File.Exists($@"{uploadFolder}\{Path.GetFileName(userIdentityDocuments[i].FilePath)}"))
                                File.Delete($@"{uploadFolder}\{Path.GetFileName(userIdentityDocuments[i].FilePath)}");

                            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(files[i].FileName)}";
                            var relativePath = $"/UploadFiles/UserIdentityDocument/{fileName}";
                            string filePath = Path.Combine(uploadFolder, fileName);
                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await files[i].CopyToAsync(fileStream);

                                userIdentityDocuments[i].Version += 1;
                                userIdentityDocuments[i].FilePath = relativePath;
                                userIdentityDocuments[i].FileType = FileExtensions.GetFileType(relativePath);
                                userIdentityDocuments[i].BankId = documentType == DocumentType.AccountStatement ? bankId : null;
                                await userIdentityDocumentRepository
                                    .UpdateCustomPropertiesAsync(userIdentityDocuments[i], cancellationToken, true,
                                            nameof(UserIdentityDocument.Version),
                                            nameof(UserIdentityDocument.FileType),
                                            nameof(UserIdentityDocument.FilePath),
                                            nameof(UserIdentityDocument.BankId));
                            }
                            if (deleteFileIDs != null && deleteFileIDs.Any(p => p == userIdentityDocuments[i].Id))
                                deleteFileIDs.Remove(userIdentityDocuments[i].Id);
                        }
                        for (int i = userIdentityDocuments.Count; i < files.Count; i++)
                        {
                            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(files[i].FileName)}";
                            var relativePath = $"/UploadFiles/UserIdentityDocument/{fileName}";
                            string filePath = Path.Combine(uploadFolder, fileName);
                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await files[i].CopyToAsync(fileStream);
                                #region Add File Info To DB
                                await userIdentityDocumentRepository.AddAsync(new UserIdentityDocument()
                                {
                                    UserId = userId,
                                    FilePath = relativePath,
                                    Status = DocumentStatus.Active,
                                    FileType = FileExtensions.GetFileType(relativePath),
                                    DocumentType = documentType,
                                    BankId = documentType == DocumentType.AccountStatement ? bankId : null
                                }, cancellationToken);
                                #endregion
                            }
                        }
                    }
                    else if (userIdentityDocuments.Count > files.Count)
                    {
                        for (int i = 0; i < files.Count; i++)
                        {
                            if (File.Exists($@"{uploadFolder}\{Path.GetFileName(userIdentityDocuments[i].FilePath)}"))
                                File.Delete($@"{uploadFolder}\{Path.GetFileName(userIdentityDocuments[i].FilePath)}");

                            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(files[i].FileName)}";
                            var relativePath = $"/UploadFiles/UserIdentityDocument/{fileName}";
                            string filePath = Path.Combine(uploadFolder, fileName);
                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await files[i].CopyToAsync(fileStream);

                                userIdentityDocuments[i].Version += 1;
                                userIdentityDocuments[i].FilePath = relativePath;
                                userIdentityDocuments[i].FileType = FileExtensions.GetFileType(relativePath);
                                await userIdentityDocumentRepository
                                    .UpdateCustomPropertiesAsync(userIdentityDocuments[i], cancellationToken, true,
                                            nameof(UserIdentityDocument.Version),
                                            nameof(UserIdentityDocument.FileType),
                                            nameof(UserIdentityDocument.FilePath),
                                            nameof(UserIdentityDocument.BankId));
                            }
                            if (deleteFileIDs != null && deleteFileIDs.Any(p => p == userIdentityDocuments[i].Id))
                                deleteFileIDs.Remove(userIdentityDocuments[i].Id);
                        }
                        for (int i = files.Count; i < userIdentityDocuments.Count; i++)
                        {
                            var iw = i;
                            if (File.Exists($@"{uploadFolder}\{Path.GetFileName(userIdentityDocuments[i].FilePath)}"))
                                File.Delete($@"{uploadFolder}\{Path.GetFileName(userIdentityDocuments[i].FilePath)}");

                            await userIdentityDocumentRepository.DeleteByIdAsync(userIdentityDocuments[i].Id, cancellationToken);
                            if (deleteFileIDs != null && deleteFileIDs.Any(p => p == userIdentityDocuments[i].Id))
                                deleteFileIDs.Remove(userIdentityDocuments[i].Id);
                        }
                    }
                }
                else
                {
                    foreach (var file in files)
                    {
                        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                        var relativePath = $"/UploadFiles/UserIdentityDocument/{fileName}";
                        string filePath = Path.Combine(uploadFolder, fileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                            #region Add File Info To DB
                            await userIdentityDocumentRepository.AddAsync(new UserIdentityDocument()
                            {
                                UserId = userId,
                                FilePath = relativePath,
                                Status = DocumentStatus.Active,
                                FileType = FileExtensions.GetFileType(relativePath),
                                DocumentType = documentType,
                                BankId = documentType == DocumentType.AccountStatement ? bankId : null
                            }, cancellationToken);
                            #endregion
                        }
                    }
                }
                #endregion
            }

            if (deleteFileIDs != null && deleteFileIDs.Any())
            {
                foreach (var id in deleteFileIDs)
                {
                    var document = await userIdentityDocumentRepository.GetByIdAsync(cancellationToken, id);
                    if (File.Exists($@"{uploadFolder}\{Path.GetFileName(document.FilePath)}"))
                        File.Delete($@"{uploadFolder}\{Path.GetFileName(document.FilePath)}");

                    await userIdentityDocumentRepository.DeleteAsync(document, cancellationToken);
                }
            }

            //if (waitingRequestFacilityId.HasValue)
            //{
            //    await requestFacilityWorkFlowStepRepository.ChangeCurrentStepAndGoToNextStep(waitingRequestFacilityId.Value,
            //      WorkFlowFormEnum.UploadIdentityDocuments,
            //      StatusEnum.Approved,
            //      buyerId: userId,
            //      opratorId: userId,
            //      "تایید اتوماتیک بعد از بارگذاری مدارک هویتی",
            //      cancellationToken);
            //}
        }

        public async Task UploadIdentityDocuments(Guid userId, IFormFile NationalCardFront, IFormFile NationalCardBack, CancellationToken cancellationToken = default)
        {
            var documentTypeFilePairs = new Dictionary<DocumentType, List<IFormFile>?>()
            {
               {DocumentType.NationalCardFront,NationalCardFront != null ?  new List<IFormFile>{NationalCardFront }:null },
               {DocumentType.NationalCardBack,NationalCardBack != null ?  new List<IFormFile>{NationalCardBack } : null }
            };

            var userIdentityDocumentPath = @"UploadFiles\UserIdentityDocument";
            string uploadFolder = Path.Combine(webHostEnvironment.WebRootPath, userIdentityDocumentPath);
            foreach (var documentType in documentTypeFilePairs.Keys)
            {
                #region Save File On Disk & Insert/Update Record In Database
                var files = documentTypeFilePairs[documentType];
                if (files == null) continue;

                var userIdentityDocuments = await userIdentityDocumentRepository
                    .SelectByAsync(p => p.UserId == userId && p.DocumentType == documentType && !p.IsDeleted, cancellationToken);
                if (userIdentityDocuments != null && userIdentityDocuments.Any()) //Edit Mode
                {
                    if (userIdentityDocuments.Count == files.Count)
                    {
                        for (int i = 0; i < userIdentityDocuments.Count; i++)
                        {
                            if (File.Exists($@"{uploadFolder}\{Path.GetFileName(userIdentityDocuments[i].FilePath)}"))
                                File.Delete($@"{uploadFolder}\{Path.GetFileName(userIdentityDocuments[i].FilePath)}");

                            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(files[i].FileName)}";
                            var relativePath = $"/UploadFiles/UserIdentityDocument/{fileName}";
                            string filePath = Path.Combine(uploadFolder, fileName);
                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await files[i].CopyToAsync(fileStream);

                                userIdentityDocuments[i].Version += 1;
                                userIdentityDocuments[i].FilePath = relativePath;
                                userIdentityDocuments[i].FileType = FileExtensions.GetFileType(relativePath);
                                await userIdentityDocumentRepository
                                    .UpdateCustomPropertiesAsync(userIdentityDocuments[i], cancellationToken, true,
                                                                 nameof(UserIdentityDocument.Version),
                                                                 nameof(UserIdentityDocument.FileType),
                                                                 nameof(UserIdentityDocument.FilePath));
                            }
                        }
                    }
                    else if (userIdentityDocuments.Count < files.Count)
                    {
                        for (int i = 0; i < userIdentityDocuments.Count; i++)
                        {
                            if (File.Exists($@"{uploadFolder}\{Path.GetFileName(userIdentityDocuments[i].FilePath)}"))
                                File.Delete($@"{uploadFolder}\{Path.GetFileName(userIdentityDocuments[i].FilePath)}");

                            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(files[i].FileName)}";
                            var relativePath = $"/UploadFiles/UserIdentityDocument/{fileName}";
                            string filePath = Path.Combine(uploadFolder, fileName);
                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await files[i].CopyToAsync(fileStream);

                                userIdentityDocuments[i].Version += 1;
                                userIdentityDocuments[i].FilePath = relativePath;
                                userIdentityDocuments[i].FileType = FileExtensions.GetFileType(relativePath);
                                //userIdentityDocuments[i].BankId = documentType == DocumentType.AccountStatement ? bankId : null;
                                await userIdentityDocumentRepository
                                    .UpdateCustomPropertiesAsync(userIdentityDocuments[i], cancellationToken, true,
                                            nameof(UserIdentityDocument.Version),
                                            nameof(UserIdentityDocument.FileType),
                                            nameof(UserIdentityDocument.FilePath));
                            }
                        }
                        for (int i = userIdentityDocuments.Count; i < files.Count; i++)
                        {
                            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(files[i].FileName)}";
                            var relativePath = $"/UploadFiles/UserIdentityDocument/{fileName}";
                            string filePath = Path.Combine(uploadFolder, fileName);
                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await files[i].CopyToAsync(fileStream);
                                #region Add File Info To DB
                                await userIdentityDocumentRepository.AddAsync(new UserIdentityDocument()
                                {
                                    UserId = userId,
                                    FilePath = relativePath,
                                    Status = DocumentStatus.Active,
                                    FileType = FileExtensions.GetFileType(relativePath),
                                    DocumentType = documentType,
                                }, cancellationToken);
                                #endregion
                            }
                        }
                    }
                    else if (userIdentityDocuments.Count > files.Count)
                    {
                        for (int i = 0; i < files.Count; i++)
                        {
                            if (File.Exists($@"{uploadFolder}\{Path.GetFileName(userIdentityDocuments[i].FilePath)}"))
                                File.Delete($@"{uploadFolder}\{Path.GetFileName(userIdentityDocuments[i].FilePath)}");

                            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(files[i].FileName)}";
                            var relativePath = $"/UploadFiles/UserIdentityDocument/{fileName}";
                            string filePath = Path.Combine(uploadFolder, fileName);
                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await files[i].CopyToAsync(fileStream);

                                userIdentityDocuments[i].Version += 1;
                                userIdentityDocuments[i].FilePath = relativePath;
                                userIdentityDocuments[i].FileType = FileExtensions.GetFileType(relativePath);
                                await userIdentityDocumentRepository
                                    .UpdateCustomPropertiesAsync(userIdentityDocuments[i], cancellationToken, true,
                                            nameof(UserIdentityDocument.Version),
                                            nameof(UserIdentityDocument.FileType),
                                            nameof(UserIdentityDocument.FilePath),
                                            nameof(UserIdentityDocument.BankId));
                            }
                        }
                        for (int i = files.Count; i < userIdentityDocuments.Count; i++)
                        {
                            var iw = i;
                            if (File.Exists($@"{uploadFolder}\{Path.GetFileName(userIdentityDocuments[i].FilePath)}"))
                                File.Delete($@"{uploadFolder}\{Path.GetFileName(userIdentityDocuments[i].FilePath)}");

                            await userIdentityDocumentRepository.DeleteByIdAsync(userIdentityDocuments[i].Id, cancellationToken);
                        }
                    }
                }
                else
                {
                    foreach (var file in files)
                    {
                        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                        var relativePath = $"/UploadFiles/UserIdentityDocument/{fileName}";
                        string filePath = Path.Combine(uploadFolder, fileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                            #region Add File Info To DB
                            await userIdentityDocumentRepository.AddAsync(new UserIdentityDocument()
                            {
                                UserId = userId,
                                FilePath = relativePath,
                                Status = DocumentStatus.Active,
                                FileType = FileExtensions.GetFileType(relativePath),
                                DocumentType = documentType,
                                //BankId = documentType == DocumentType.AccountStatement ? bankId : null
                            }, cancellationToken);
                            #endregion
                        }
                    }
                }
                #endregion
            }
        }

        public async Task UploadIdentityDocumentsGuarantor(Guid userId, int? waitingRequestFacilityGuarantorId, IFormFile BirthCertificatePage1, IFormFile BirthCertificateDescription,
      IFormFile NationalCardFront, IFormFile NationalCardBack, List<IFormFile> JobDocument, List<IFormFile> AddressDocument, List<int> deleteFileIDs,
      CancellationToken cancellationToken = default)
        {
            var documentTypeFilePairs = new Dictionary<DocumentType, List<IFormFile>?>()
            {
               {DocumentType.BirthCertificatePage1,BirthCertificatePage1 != null ? new List<IFormFile>{ BirthCertificatePage1 } : null},
               {DocumentType.BirthCertificateDescription,BirthCertificateDescription != null ?  new List<IFormFile>{BirthCertificateDescription }:null },
               {DocumentType.NationalCardFront,NationalCardFront != null ?  new List<IFormFile>{NationalCardFront }:null },
               {DocumentType.NationalCardBack,NationalCardBack != null ?  new List<IFormFile>{NationalCardBack } : null },
               {DocumentType.JobDocument, JobDocument},
               {DocumentType.AddressDocument,  AddressDocument}
            };
            var userIdentityDocumentPath = @"UploadFiles\UserIdentityDocument";
            string uploadFolder = Path.Combine(webHostEnvironment.WebRootPath, userIdentityDocumentPath);
            foreach (var documentType in documentTypeFilePairs.Keys)
            {
                #region Save File On Disk & Insert/Update Record In Database
                var files = documentTypeFilePairs[documentType];
                if (files == null) continue;

                var userIdentityDocuments = await userIdentityDocumentRepository
                    .SelectByAsync(p => p.UserId == userId && p.DocumentType == documentType && !p.IsDeleted, cancellationToken);
                if (userIdentityDocuments != null && userIdentityDocuments.Any()) //Edit Mode
                {
                    if (userIdentityDocuments.Count == files.Count)
                    {
                        for (int i = 0; i < userIdentityDocuments.Count; i++)
                        {
                            var fileName = Path.GetFileName(userIdentityDocuments[i].FilePath);
                            string filePath = Path.Combine(uploadFolder, fileName);
                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await files[i].CopyToAsync(fileStream);

                                userIdentityDocuments[i].Version += 1;
                                await userIdentityDocumentRepository
                                    .UpdateCustomPropertiesAsync(userIdentityDocuments[i], cancellationToken, true, nameof(UserIdentityDocument.Version));
                            }
                            if (deleteFileIDs != null && deleteFileIDs.Any(p => p == userIdentityDocuments[i].Id))
                                deleteFileIDs.Remove(userIdentityDocuments[i].Id);
                        }
                    }
                    else if (userIdentityDocuments.Count < files.Count)
                    {
                        for (int i = 0; i < userIdentityDocuments.Count; i++)
                        {
                            var fileName = Path.GetFileName(userIdentityDocuments[i].FilePath);
                            string filePath = Path.Combine(uploadFolder, fileName);
                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await files[i].CopyToAsync(fileStream);

                                userIdentityDocuments[i].Version += 1;
                                await userIdentityDocumentRepository
                                    .UpdateCustomPropertiesAsync(userIdentityDocuments[i], cancellationToken, true, nameof(UserIdentityDocument.Version));
                            }
                            if (deleteFileIDs != null && deleteFileIDs.Any(p => p == userIdentityDocuments[i].Id))
                                deleteFileIDs.Remove(userIdentityDocuments[i].Id);
                        }
                        for (int i = userIdentityDocuments.Count; i < files.Count; i++)
                        {
                            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(files[i].FileName)}";
                            var relativePath = $"/UploadFiles/UserIdentityDocument/{fileName}";
                            string filePath = Path.Combine(uploadFolder, fileName);
                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await files[i].CopyToAsync(fileStream);
                                #region Add File Info To DB
                                await userIdentityDocumentRepository.AddAsync(new UserIdentityDocument()
                                {
                                    UserId = userId,
                                    FilePath = relativePath,
                                    Status = DocumentStatus.Active,
                                    FileType = FileType.Image,
                                    DocumentType = documentType
                                }, cancellationToken);
                                #endregion
                            }
                        }
                    }
                    else if (userIdentityDocuments.Count > files.Count)
                    {
                        for (int i = 0; i < files.Count; i++)
                        {
                            var fileName = Path.GetFileName(userIdentityDocuments[i].FilePath);
                            string filePath = Path.Combine(uploadFolder, fileName);
                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await files[i].CopyToAsync(fileStream);

                                userIdentityDocuments[i].Version += 1;
                                await userIdentityDocumentRepository
                                    .UpdateCustomPropertiesAsync(userIdentityDocuments[i], cancellationToken, true, nameof(UserIdentityDocument.Version));
                            }
                            if (deleteFileIDs != null && deleteFileIDs.Any(p => p == userIdentityDocuments[i].Id))
                                deleteFileIDs.Remove(userIdentityDocuments[i].Id);
                        }
                        for (int i = files.Count; i < userIdentityDocuments.Count; i++)
                        {
                            var iw = i;
                            if (File.Exists($@"{uploadFolder}\{Path.GetFileName(userIdentityDocuments[i].FilePath)}"))
                                File.Delete($@"{uploadFolder}\{Path.GetFileName(userIdentityDocuments[i].FilePath)}");

                            await userIdentityDocumentRepository.DeleteByIdAsync(userIdentityDocuments[i].Id, cancellationToken);
                            if (deleteFileIDs != null && deleteFileIDs.Any(p => p == userIdentityDocuments[i].Id))
                                deleteFileIDs.Remove(userIdentityDocuments[i].Id);
                        }
                    }
                }
                else
                {
                    foreach (var file in files)
                    {
                        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                        var relativePath = $"/UploadFiles/UserIdentityDocument/{fileName}";
                        string filePath = Path.Combine(uploadFolder, fileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                            #region Add File Info To DB
                            await userIdentityDocumentRepository.AddAsync(new UserIdentityDocument()
                            {
                                UserId = userId,
                                FilePath = relativePath,
                                Status = DocumentStatus.Active,
                                FileType = FileType.Image,
                                DocumentType = documentType
                            }, cancellationToken);
                            #endregion
                        }
                    }
                }
                #endregion
            }

            if (deleteFileIDs != null && deleteFileIDs.Any())
            {
                foreach (var id in deleteFileIDs)
                {
                    var document = await userIdentityDocumentRepository.GetByIdAsync(cancellationToken, id);
                    if (File.Exists($@"{uploadFolder}\{Path.GetFileName(document.FilePath)}"))
                        File.Delete($@"{uploadFolder}\{Path.GetFileName(document.FilePath)}");

                    await userIdentityDocumentRepository.DeleteAsync(document, cancellationToken);
                }
            }

            if (waitingRequestFacilityGuarantorId.HasValue)
            {
                await requestFacilityGuarantorWorkFlowStepRepository.ChangeCurrentStepAndGoToNextStep(waitingRequestFacilityGuarantorId.Value,
                  WorkFlowFormEnum.UploadIdentityDocumentsGuarantor,
                  StatusEnum.Approved,
                  guarantorUserId: userId,
                  opratorId: userId,
                  "تایید اتوماتیک بعد از بارگذاری مدارک هویتی",
                  cancellationToken);
            }
        }

        public async Task UploadIdentityDocumentsGuarantor(Guid userId, int? waitingRequestFacilityGuarantorId, IFormFile NationalCardFront, IFormFile NationalCardBack, CancellationToken cancellationToken = default)
        {
            var documentTypeFilePairs = new Dictionary<DocumentType, List<IFormFile>?>()
            {
               {DocumentType.NationalCardFront,NationalCardFront != null ?  new List<IFormFile>{NationalCardFront }:null },
               {DocumentType.NationalCardBack,NationalCardBack != null ?  new List<IFormFile>{NationalCardBack } : null }
            };
            var userIdentityDocumentPath = @"UploadFiles\UserIdentityDocument";
            string uploadFolder = Path.Combine(webHostEnvironment.WebRootPath, userIdentityDocumentPath);
            foreach (var documentType in documentTypeFilePairs.Keys)
            {
                #region Save File On Disk & Insert/Update Record In Database
                var files = documentTypeFilePairs[documentType];
                if (files == null) continue;

                var userIdentityDocuments = await userIdentityDocumentRepository
                    .SelectByAsync(p => p.UserId == userId && p.DocumentType == documentType && !p.IsDeleted, cancellationToken);
                if (userIdentityDocuments != null && userIdentityDocuments.Any()) //Edit Mode
                {
                    if (userIdentityDocuments.Count == files.Count)
                    {
                        for (int i = 0; i < userIdentityDocuments.Count; i++)
                        {
                            var fileName = Path.GetFileName(userIdentityDocuments[i].FilePath);
                            string filePath = Path.Combine(uploadFolder, fileName);
                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await files[i].CopyToAsync(fileStream);

                                userIdentityDocuments[i].Version += 1;
                                await userIdentityDocumentRepository
                                    .UpdateCustomPropertiesAsync(userIdentityDocuments[i], cancellationToken, true, nameof(UserIdentityDocument.Version));
                            }
                        }
                    }
                    else if (userIdentityDocuments.Count < files.Count)
                    {
                        for (int i = 0; i < userIdentityDocuments.Count; i++)
                        {
                            var fileName = Path.GetFileName(userIdentityDocuments[i].FilePath);
                            string filePath = Path.Combine(uploadFolder, fileName);
                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await files[i].CopyToAsync(fileStream);

                                userIdentityDocuments[i].Version += 1;
                                await userIdentityDocumentRepository
                                    .UpdateCustomPropertiesAsync(userIdentityDocuments[i], cancellationToken, true, nameof(UserIdentityDocument.Version));
                            }
                        }
                        for (int i = userIdentityDocuments.Count; i < files.Count; i++)
                        {
                            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(files[i].FileName)}";
                            var relativePath = $"/UploadFiles/UserIdentityDocument/{fileName}";
                            string filePath = Path.Combine(uploadFolder, fileName);
                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await files[i].CopyToAsync(fileStream);
                                #region Add File Info To DB
                                await userIdentityDocumentRepository.AddAsync(new UserIdentityDocument()
                                {
                                    UserId = userId,
                                    FilePath = relativePath,
                                    Status = DocumentStatus.Active,
                                    FileType = FileType.Image,
                                    DocumentType = documentType
                                }, cancellationToken);
                                #endregion
                            }
                        }
                    }
                    else if (userIdentityDocuments.Count > files.Count)
                    {
                        for (int i = 0; i < files.Count; i++)
                        {
                            var fileName = Path.GetFileName(userIdentityDocuments[i].FilePath);
                            string filePath = Path.Combine(uploadFolder, fileName);
                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await files[i].CopyToAsync(fileStream);

                                userIdentityDocuments[i].Version += 1;
                                await userIdentityDocumentRepository
                                    .UpdateCustomPropertiesAsync(userIdentityDocuments[i], cancellationToken, true, nameof(UserIdentityDocument.Version));
                            }
                        }
                        for (int i = files.Count; i < userIdentityDocuments.Count; i++)
                        {
                            var iw = i;
                            if (File.Exists($@"{uploadFolder}\{Path.GetFileName(userIdentityDocuments[i].FilePath)}"))
                                File.Delete($@"{uploadFolder}\{Path.GetFileName(userIdentityDocuments[i].FilePath)}");

                            await userIdentityDocumentRepository.DeleteByIdAsync(userIdentityDocuments[i].Id, cancellationToken);
                        }
                    }
                }
                else
                {
                    foreach (var file in files)
                    {
                        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                        var relativePath = $"/UploadFiles/UserIdentityDocument/{fileName}";
                        string filePath = Path.Combine(uploadFolder, fileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                            #region Add File Info To DB
                            await userIdentityDocumentRepository.AddAsync(new UserIdentityDocument()
                            {
                                UserId = userId,
                                FilePath = relativePath,
                                Status = DocumentStatus.Active,
                                FileType = FileType.Image,
                                DocumentType = documentType
                            }, cancellationToken);
                            #endregion
                        }
                    }
                }
                #endregion
            }

            if (waitingRequestFacilityGuarantorId.HasValue)
            {
                await requestFacilityGuarantorWorkFlowStepRepository.ChangeCurrentStepAndGoToNextStep(waitingRequestFacilityGuarantorId.Value,
                  WorkFlowFormEnum.UploadIdentityDocumentsGuarantor,
                  StatusEnum.Approved,
                  guarantorUserId: userId,
                  opratorId: userId,
                  "تایید اتوماتیک بعد از بارگذاری مدارک هویتی",
                  cancellationToken);
            }
        }
        public async Task Update(UserDto userDto, CancellationToken cancellationToken = default)
        {
            if (userDto == null)
                throw new NotFoundException("'userDto' can't be NULL");

            var user = await userRepository.GetByIdAsync(cancellationToken, userDto.Id);
            if (user == null)
                throw new NotFoundException($"not found User entity to PKey(Id) : '{userDto.Id}'");

            var updateUser = mapper.Map(userDto, user);

            await userRepository.UpdateAsync(updateUser, cancellationToken);
        }

        public virtual async Task Delete(Guid id, CancellationToken cancellationToken = default)
        {
            await userRepository.DeleteByIdAsync(id, cancellationToken);
        }

        public async Task UpdateFullname(UserDto userDto, CancellationToken cancellationToken = default)
        {
            if (userDto == null)
                throw new NotFoundException("'userDto' can not NULL");

            var user = await userRepository.GetByIdAsync(cancellationToken, userDto.Id);
            if (user == null)
                throw new NotFoundException($"not found User entity to PKey(Id) : '{userDto.Id}'");

            await userRepository.UpdateCustomPropertiesAsync(user, cancellationToken, true, new string[] { nameof(User.Person.FName), nameof(User.Person.LName) });
        }

        public async Task<UserDto> UpdateProperty(Guid id, PropertyValueDto dto, CancellationToken cancellationToken = default)
        {
            var entityType = typeof(User);
            var property = entityType.GetProperty(dto.PropertyName);
            if (property == null)
                throw new NotFoundException($"property {dto.PropertyName} not found in entity : {nameof(User)}");

            var model = await userRepository.GetByIdAsync(cancellationToken, id);

            if (model == null)
                throw new NotFoundException($"not found {nameof(User)} entity to PKey(Id) : '{id}'");

            if (dto.PropertyValue != null)
            {
                if (entityType.GetProperty(dto.PropertyName).PropertyType == typeof(Guid))
                {
                    entityType.GetProperty(dto.PropertyName).SetValue(model, new Guid(dto.PropertyValue.ToString()));
                }
                else if (entityType.GetProperty(dto.PropertyName).PropertyType == typeof(Guid?))
                {
                    entityType.GetProperty(dto.PropertyName).SetValue(model, new Guid(dto.PropertyValue.ToString()));
                }
                else
                {
                    var value = Convert.ChangeType(dto.PropertyValue, entityType.GetProperty(dto.PropertyName).PropertyType, CultureInfo.InvariantCulture);
                    entityType.GetProperty(dto.PropertyName).SetValue(model, value);
                }
            }
            else
            {
                //todo check property support null value
                var propertyType = entityType.GetProperty(dto.PropertyName).PropertyType;
                var canBeNull = !propertyType.IsValueType || (Nullable.GetUnderlyingType(propertyType) != null);
                if (canBeNull)
                    entityType.GetProperty(dto.PropertyName).SetValue(model, null);
                else
                {
                    //todo exception
                }
            }

            await userRepository.UpdateAsync(model, cancellationToken);

            var resultDto = await userRepository.TableNoTracking.ProjectTo<UserDto>(mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(p => p.Id.Equals(model.Id), cancellationToken);

            return resultDto;

        }

        public async Task<UserSelectDto> UpdateCustomProperties(UserDto userDto, CancellationToken cancellationToken = default)
        {
            if (userDto == null)
                throw new NotFoundException("'dto' can not NULL");

            if (userDto.UpdateProperties == null || !userDto.UpdateProperties.Any())
                throw new NotFoundException("'dto.UpdateProperties' can't be NULL or Empty");

            var model = await userRepository.TableNoTracking.SingleOrDefaultAsync(p => p.Id == userDto.Id, cancellationToken);
            //var model = await userManager.FindByIdAsync(userDto.Id.ToString());
            if (model == null)
                throw new NotFoundException($"not found User entity to PKey(Id) : '{userDto.Id}'");

            var concurrencyStamp = model.GetType().GetProperty("ConcurrencyStamp").GetValue(model).ToString();
            model = mapper.Map<User>(userDto);
            model.GetType().GetProperty("ConcurrencyStamp").SetValue(model, concurrencyStamp);

            await userRepository.UpdateCustomPropertiesAsync(model, cancellationToken, true, userDto.UpdateProperties.ToArray());

            var resultDto = await userRepository.TableNoTracking.ProjectTo<UserSelectDto>(mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(p => p.Id.Equals(model.Id), cancellationToken);

            return resultDto;
        }

        public async Task<UserDto> AvatarUploader(string userId, IFormFile file, CancellationToken cancellationToken = default)
        {
            //if (file.Length > 0)
            //{
            //    if (file.Length / 1024 > 800)
            //        throw new AppException("دقت کنید که سایز عکس زیر 800KB باشد");

            //    var user = await userManager.FindByIdAsync(userId);

            //    if (user == null)
            //        return null;

            //    await using (var stream = new MemoryStream())
            //    {
            //        await file.CopyToAsync(stream, cancellationToken);
            //        user.Person.Avatar = stream.ToArray();
            //    }

            //    var result = await userManager.UpdateAsync(user);

            //    if (result.Succeeded)
            //        return mapper.Map<UserDto>(user);
            //}

            return null;
        }
        public async Task<UserAddModel> PrepareModelForAdd(CancellationToken cancellationToken)
        {
            return new UserAddModel()
            {
                Provinces = (await locationRepository.SelectByAsync(p => p.IsActive.Equals(true) && p.LocationType == LocationTypeEnum.Province,
                    p => new SelectListItem
                    {
                        Text = p.Name,
                        Value = p.Id.ToString()
                    }, cancellationToken))
                    .OrderBy(p => p.Text)
                    .ToList(),
            };
        }

        public async Task<UserAddEditModelByAdmin> PrepareModelForAddByAdmin(CancellationToken cancellationToken)
        {
            return new UserAddEditModelByAdmin()
            {
                Provinces = (await locationRepository.SelectByAsync(p => p.IsActive.Equals(true) && p.LocationType == LocationTypeEnum.Province,
                    p => new SelectListItem
                    {
                        Text = p.Name,
                        Value = p.Id.ToString()
                    }, cancellationToken))
                    .OrderBy(p => p.Text)
                    .ToList(),
                OrganizationTypes = (await organizationTypeRepository.SelectByAsync(p => p.IsActive.Equals(true),
                     p => new SelectListItem
                     {
                         Text = p.Name,
                         Value = p.Id.ToString()
                     }, cancellationToken))
                     .ToList(),
                Roles = await roleManager.Roles.OrderBy(p => p.Order)
                    .Select(p => new SelectListItem
                    {
                        Value = p.Id.ToString(),
                        Text = p.Title
                    }).ToListAsync(cancellationToken)
            };
        }

        public async Task<UserAddEditModelByAdmin?> PrepareModelForEditByAdmin(Guid userId, CancellationToken cancellationToken)
        {
            var provinceList = (await locationRepository.SelectByAsync(p => p.IsActive.Equals(true) && p.LocationType == LocationTypeEnum.Province,
                    p => new
                    {
                        p.Name,
                        p.Id
                    }, cancellationToken))
                    .ToList();
            var user = (await userRepository.SelectByAsync(p => p.Id == userId,
                cancellationToken,
                true,
                pageNumber: null,
                pageSize: null,
                p => p.UserRoles,
                p => p.Person,
                p => p.Person.City,
                p => p.Person.Organization,
                p => p.Person.Organization.OrganizationType))
                .FirstOrDefault();

            if (user == default(User))
                return null;

            return new UserAddEditModelByAdmin()
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                Address = user.Person.Address,
                FName = user.Person.FName,
                LName = user.Person.LName,
                FatherName = user.Person.FatherName,
                BirthDate = user.Person.BirthDate,
                Gender = user.Person.Gender.HasValue ? user.Person.Gender.Value : default,
                SSID = user.Person.SSID,
                Mobile = user.Person.Mobile,
                NationalCode = user.Person.NationalCode,
                PhoneNumber = user.PhoneNumber,
                PostalCode = user.Person.PostalCode,
                IsActive = user.IsActive,

                ProvinceId = user.Person.CityId.HasValue ? user.Person.City.ParentId : null,
                Provinces = provinceList.Select(p => new SelectListItem
                {
                    Text = p.Name,
                    Value = p.Id.ToString(),
                    Selected = user.Person.CityId.HasValue ? user.Person.City.ParentId == p.Id : false,
                })
                .OrderBy(p => p.Text)
                .ToList(),
                Cities = user.Person.CityId.HasValue
                ? (await locationRepository.SelectByAsync(p => p.ParentId.Equals(user.Person.City.ParentId) && p.IsActive.Equals(true) && p.LocationType == LocationTypeEnum.City,
                    p => new SelectListItem
                    {
                        Text = p.Name,
                        Value = p.Id.ToString(),
                        Selected = user.Person.CityId.HasValue ? user.Person.CityId == p.Id : false,
                    }, cancellationToken))
                    .OrderBy(p => p.Text)
                    .ToList()
                : null,
                CityId = user.Person.CityId,
                OrganizationTypeId = user.Person.Organization != null ? user.Person.Organization.OrganizationTypeId : null,
                OrganizationTypes = (await organizationTypeRepository.SelectByAsync(p => p.IsActive.Equals(true),
                    p => new SelectListItem
                    {
                        Text = p.Name,
                        Value = p.Id.ToString(),
                        Selected = user.Person.Organization != null ? user.Person.Organization.OrganizationTypeId == p.Id : false
                    }, cancellationToken))
                    .OrderBy(p => p.Text)
                    .ToList(),

                OrganizationId = user.Person.Organization != null ? user.Person.OrganizationId : null,
                Organizations = user.Person.Organization != null
                    ? (await organizationRepository.SelectByAsync(p => p.OrganizationTypeId.Equals(user.Person.Organization.OrganizationTypeId) && p.IsActive.Equals(true),
                    p => new SelectListItem
                    {
                        Text = p.Name,
                        Value = p.Id.ToString(),
                        Selected = user.Person.OrganizationId == p.Id
                    }, cancellationToken))
                    .OrderBy(p => p.Text)
                    .ToList()
                    : null,
                RoleId = user.UserRoles != null && user.UserRoles.Any() ? user.UserRoles.FirstOrDefault().RoleId : Guid.Empty,
                Roles = await roleManager.Roles.OrderBy(p => p.Order)
                               .Select(p => new SelectListItem
                               {
                                   Value = p.Id.ToString(),
                                   Text = p.Title,
                                   Selected = user.UserRoles != null && user.UserRoles.Any() ? user.UserRoles.FirstOrDefault().RoleId == p.Id : false
                               }).ToListAsync(cancellationToken),
                IsEditMode = true
            };
        }

        public async Task<UserEditModel?> PrepareModelForEdit(Guid userId, CancellationToken cancellationToken)
        {
            var provinceList = (await locationRepository.SelectByAsync(p => p.IsActive.Equals(true) && p.LocationType == LocationTypeEnum.Province,
                p => new SelectListItem
                {
                    Text = p.Name,
                    Value = p.Id.ToString(),
                }, cancellationToken))
                .OrderBy(p => p.Text)
                .ToList();

            var userInfo = (await userRepository.TableNoTracking
                      .Where(p => p.Id == userId)
                      .Select(p => new
                      {
                          PersonJobInfos = p.Person.PersonJobInfos.Any()
                          ? p.Person.PersonJobInfos.Select(x => new
                          {
                              JobAddress = x.Address,
                              JobPhoneNumber = x.PhoneNumber,
                              JobTitle = x.JobTitle.Title,
                              SalaryRangeTitle = x.SalaryRange.Title,
                              x.SalaryRangeId,
                              x.JobTitleId,
                          })
                          : null,
                          UserBankAccounts = p.UserBankAccounts.Any()
                          ? p.UserBankAccounts.Select(x => new
                          {
                              AccountNumber = x.Deposit,
                              x.CardNumber,
                              x.ClientId,
                              x.IBAN,
                              x.BankName,
                              x.DepositOwner,
                              x.DepositStatus,
                          })
                          : null,
                          p.Person.Address,
                          p.Email,
                          p.Person.FName,
                          p.Person.LName,
                          p.Person.FatherName,
                          p.Person.BirthDate,
                          p.Person.SSID,
                          p.Person.Gender,
                          p.Person.Mobile,
                          p.Person.NationalCode,
                          p.PhoneNumber,
                          p.UserName,
                          p.Person.PostalCode,
                          p.Person.CustomerNumber,
                          ProvinceId = p.Person.CityId.HasValue ? p.Person.City.ParentId : null,
                          CityId = p.Person.CityId,
                          IsEditMode = true
                      }).ToListAsync(cancellationToken))
                      .Select(p => new UserEditModel
                      {
                          JobAddress = p.PersonJobInfos != null ? p.PersonJobInfos.First().JobAddress : null,
                          JobPhoneNumber = p.PersonJobInfos != null ? p.PersonJobInfos.First().JobPhoneNumber : null,
                          JobTitle = p.PersonJobInfos != null ? p.PersonJobInfos.First().JobTitle : null,
                          SalaryRangeTitle = p.PersonJobInfos != null ? p.PersonJobInfos.First().SalaryRangeTitle : null,
                          //SalaryRangeId = p.PersonJobInfos != null ? p.PersonJobInfos.First().SalaryRangeId : null,
                          //JobTitleId = p.PersonJobInfos != null ? p.PersonJobInfos.First().JobTitleId : null,

                          AccountNumber = p.UserBankAccounts != null ? p.UserBankAccounts.First().AccountNumber : null,
                          ClientId = p.UserBankAccounts != null ? p.UserBankAccounts.First().ClientId : null,
                          CardNumberWithoutDash = p.UserBankAccounts != null ? p.UserBankAccounts.First().CardNumber : null,
                          IBAN = p.UserBankAccounts != null ? p.UserBankAccounts.First().IBAN : null,
                          BankName = p.UserBankAccounts != null ? p.UserBankAccounts.First().BankName : null,
                          DepositOwners = p.UserBankAccounts != null ? p.UserBankAccounts.First().DepositOwner : null,
                          DepositStatus = p.UserBankAccounts != null ? p.UserBankAccounts.First().DepositStatus : null,

                          Address = p.Address,
                          Email = p.Email,
                          FName = p.FName,
                          LName = p.LName,
                          FatherName = p.FatherName,
                          BirthDate = p.BirthDate,
                          SSID = p.SSID,
                          //BirthCertificateSerial = p.BirthCertificateSerial,
                          Gender = p.Gender,
                          Mobile = p.Mobile,
                          NationalCode = p.NationalCode,
                          PhoneNumber = p.PhoneNumber,
                          UserName = p.UserName,
                          PostalCode = p.PostalCode,
                          CustomerNumber = p.CustomerNumber,
                          ProvinceId = p.ProvinceId,
                          CityId = p.CityId,
                          IsEditMode = true
                      }).FirstOrDefault();

            if (userInfo == default)
                return null;

            userInfo.Provinces = provinceList.Select(p => new SelectListItem
            {
                Text = p.Text,
                Value = p.Value,
                Selected = userInfo.CityId.HasValue ? p.Value == userInfo.ProvinceId.ToString() : false,
            });

            userInfo.Cities = userInfo.CityId.HasValue
            ? locationRepository.TableNoTracking.Where(x => x.ParentId.Equals(userInfo.ProvinceId) && x.IsActive.Equals(true) && x.LocationType == LocationTypeEnum.City)
                .Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString(),
                    Selected = userInfo.CityId.HasValue ? userInfo.CityId == x.Id : false,
                })
                .OrderBy(x => x.Text)
                .ToList()
            : Enumerable.Empty<SelectListItem>();

            return userInfo;
        }

        public async Task<UserEditModel> PrepareModelForEditGuarantor(Guid userId, CancellationToken cancellationToken)
        {
            var user = (await userRepository.SelectByAsync(p => p.Id == userId, cancellationToken, true, pageNumber: null, pageSize: null,
                p => p.Person,
                p => p.Person.City))
                .FirstOrDefault();

            if (user == default(User))
                return null;
            var provinceList = (await locationRepository.SelectByAsync(p => p.IsActive.Equals(true) && p.LocationType == LocationTypeEnum.Province,
                p => new SelectListItem
                {
                    Text = p.Name,
                    Value = p.Id.ToString(),
                }, cancellationToken))
                    .OrderBy(p => p.Text)
                    .ToList();
            long cardNumber;
            return new UserEditModel()
            {
                Address = user.Person.Address,
                Email = user.Email,
                FName = user.Person.FName,
                LName = user.Person.LName,
                FatherName = user.Person.FatherName,
                BirthDate = user.Person.BirthDate,
                SSID = user.Person.SSID,
                Gender = user.Person.Gender,
                Mobile = user.Person.Mobile,
                NationalCode = user.Person.NationalCode,
                PhoneNumber = user.PhoneNumber,
                UserName = user.UserName,
                PostalCode = user.Person.PostalCode,
                Provinces = provinceList.Select(p => new SelectListItem
                {
                    Text = p.Text,
                    Value = p.Value,
                    Selected = user.Person.CityId.HasValue ? p.Value == user.Person.City.ParentId.ToString() : false,
                }),
                Cities = user.Person.CityId.HasValue
                    ? (await locationRepository.SelectByAsync(p => p.ParentId.Equals(user.Person.City.ParentId) && p.IsActive.Equals(true) && p.LocationType == LocationTypeEnum.City,
                        p => new SelectListItem
                        {
                            Text = p.Name,
                            Value = p.Id.ToString(),
                            Selected = user.Person.CityId.HasValue ? user.Person.CityId == p.Id : false,
                        }, cancellationToken))
                        .OrderBy(p => p.Text)
                        .ToList()
                    : Enumerable.Empty<SelectListItem>(),
                ProvinceId = user.Person.CityId.HasValue ? user.Person.City.ParentId : null,
                CityId = user.Person.CityId,
                IsEditMode = true
            };
        }

        public async Task<UserFilterModel> PrepareFilterModel(CancellationToken cancellationToken)
        {
            var model = new UserFilterModel()
            {
                Roles = await roleManager.Roles.OrderBy(p => p.Order)
                               .Select(p => new SelectListItem
                               {
                                   Value = p.Id.ToString(),
                                   Text = p.Title
                               }).ToListAsync(cancellationToken)
            };
            return model;
        }
        public async Task<bool> DeleteUserByNationalCode(string nationalCode, CancellationToken cancellationToken)
        {
            return await userRepository.DeleteUserByNationalCode(nationalCode, cancellationToken);
        }

        public async Task<bool> UpdateBankAccount(UpdateBankAccountModel model, CancellationToken cancellationToken)
        {
            try
            {
                var userBankAccount = userBankAccountRepository.GetByCondition(g => g.UserId == new Guid(model.UserId));
                var person = personRepository.GetByCondition(g => g.User.Id == new Guid(model.UserId));

                if (userBankAccount is not null)
                {
                    userBankAccount.IBAN = model.IBan;
                    userBankAccount.Deposit = model.AccountNumber;
                    userBankAccount.ClientId = model.ClientId;
                    userBankAccount.DepositOwner = model.DepositOwner;
                    userBankAccountRepository.Update(userBankAccount);
                }
                else
                {
                    var newUserBankAccount = new UserBankAccount()
                    {
                        UserId = new Guid(model.UserId),
                        BankName = "آینده",
                        Deposit = model.AccountNumber,
                        ClientId = model.ClientId,
                        IBAN = model.IBan,
                        IsConfirm = true,
                        DepositOwner = model.DepositOwner,
                        IsDeleted = false,
                        CreatedDate = DateTime.UtcNow,
                    };

                    userBankAccountRepository.Add(newUserBankAccount);
                }
                person.CustomerNumber = model.ClientId;
                personRepository.Update(person);

                return true;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
