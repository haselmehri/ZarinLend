using Core.Data.Repositories;
using Core.Entities;
using Services.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace Services.DataInitializer
{
    public class WorkFlowStepDataInitializer : IDataInitializer
    {
        private readonly IBaseRepository<WorkFlowStep> workFlowStepRepository;
        private readonly IBaseRepository<Role> roleRepository;

        public WorkFlowStepDataInitializer(IBaseRepository<WorkFlowStep> workFlowStepRepository, IBaseRepository<Role> roleRepository)
        {
            this.workFlowStepRepository = workFlowStepRepository;
            this.roleRepository = roleRepository;
        }

        public int Order => 2;
        public void InitializeData()
        {
            var zarinLendSuperAdminRoleId =
                roleRepository.GetByConditionAsync(p => p.Name == RoleEnum.SuperAdmin.ToString(), cancellationToken: default).GetAwaiter().GetResult().Id;
            var zarinLendAdminRoleId =
                roleRepository.GetByConditionAsync(p => p.Name == RoleEnum.Admin.ToString(), cancellationToken: default).GetAwaiter().GetResult().Id;
            var leasingRoleId =
               roleRepository.GetByConditionAsync(p => p.Name == RoleEnum.BankLeasing.ToString(), cancellationToken: default).GetAwaiter().GetResult().Id;
            var adminLeasingRoleId =
               roleRepository.GetByConditionAsync(p => p.Name == RoleEnum.AdminBankLeasing.ToString(), cancellationToken: default).GetAwaiter().GetResult().Id;
            var supervisorLeasingRoleId =
                roleRepository.GetByConditionAsync(p => p.Name == RoleEnum.SupervisorLeasing.ToString(), cancellationToken: default).GetAwaiter().GetResult().Id;
            var buyerRoleId =
               roleRepository.GetByConditionAsync(p => p.Name == RoleEnum.Buyer.ToString(), cancellationToken: default).GetAwaiter().GetResult().Id;
            var sellerRoleId =
               roleRepository.GetByConditionAsync(p => p.Name == RoleEnum.Seller.ToString(), cancellationToken: default).GetAwaiter().GetResult().Id;
            var zarinLendExpertRoleId =
                roleRepository.GetByConditionAsync(p => p.Name == RoleEnum.ZarinLendExpert.ToString(), cancellationToken: default).GetAwaiter().GetResult().Id;

            #region 'Request Facility New Version' steps
            if (!workFlowStepRepository.TableNoTracking.Any(p => p.Id == 10001))
            {
                workFlowStepRepository.Add(new WorkFlowStep
                {
                    Id = 10001,
                    WorkFlowId = (int)WorkFlowEnum.RequestFacility,
                    IsLastStep = true,
                    StepIsManual = true,
                    Name = "رد درخواست",
                    Description = "رد درخواست",
                    IsActive = true,
                    WorkFlowStepRoles = new List<WorkFlowStepRole>()
                    {
                        new(){ RoleId = zarinLendAdminRoleId},
                        new(){ RoleId = zarinLendSuperAdminRoleId}
                    }
                });
            }
            if (!workFlowStepRepository.TableNoTracking.Any(p => p.Id == 10002))
            {
                workFlowStepRepository.Add(new WorkFlowStep
                {
                    Id = 10002,
                    WorkFlowId = (int)WorkFlowEnum.RequestFacility,
                    IsFirstStep = true,
                    IsActive = true,
                    Name = "ثبت درخواست",
                    Description = "ثبت درخواست تسهیلات/میزان اعتبار درخواستی/مدت باز پرداخت",
                    WorkFlowFormId = WorkFlowFormEnum.RegisterRequestFacility,
                    WorkFlowStepRoles = new List<WorkFlowStepRole>()
                    {
                        new(){ RoleId = buyerRoleId}
                    }
                });
            }
            if (!workFlowStepRepository.TableNoTracking.Any(p => p.Id == 10003))
            {
                workFlowStepRepository.Add(new WorkFlowStep
                {
                    Id = 10003,
                    WorkFlowId = (int)WorkFlowEnum.RequestFacility,
                    IsActive = true,
                    StepIsManual = true,
                    Name = "تکمیل اطلاعات",
                    Description = "تکمیل اطلاعات هویتی",
                    WorkFlowFormId = WorkFlowFormEnum.RegisterIdentityInfo,
                    WorkFlowStepRoles = new List<WorkFlowStepRole>()
                    {
                        new(){ RoleId = buyerRoleId}
                    }
                });
            }
            if (!workFlowStepRepository.TableNoTracking.Any(p => p.Id == 10004))
            {
                workFlowStepRepository.Add(new WorkFlowStep
                {
                    Id = 10004,
                    WorkFlowId = (int)WorkFlowEnum.RequestFacility,
                    IsActive = true,
                    StepIsManual = true,
                    Name = "بارگذاری مدارک",
                    Description = "بارگذاری اسناد هویتی،کارت ملی،شناسنامه",
                    WorkFlowFormId = WorkFlowFormEnum.UploadIdentityDocuments,
                    WorkFlowStepRoles = new List<WorkFlowStepRole>()
                    {
                        new(){ RoleId = buyerRoleId}
                    }
                });
            }
            if (!workFlowStepRepository.TableNoTracking.Any(p => p.Id == 10008))
            {
                workFlowStepRepository.Add(new WorkFlowStep
                {
                    Id = 10008,
                    WorkFlowId = (int)WorkFlowEnum.RequestFacility,
                    IsActive = true,
                    StepIsManual = true,
                    RejectNextStepId = 10001,
                    ReturnToCorrectionNextStepId = 10003,
                    Name = "بررسی اولیه زرین لند",
                    Description = "بررسی اولیه و ارجاع به نهاد مالی توسط کارشناس زرین لند",
                    WorkFlowFormId = WorkFlowFormEnum.VerifyZarrinLend,
                    WorkFlowStepRoles = new List<WorkFlowStepRole>()
                    {
                        new(){ RoleId = zarinLendAdminRoleId},
                        new(){ RoleId = zarinLendSuperAdminRoleId},
                        new(){ RoleId = zarinLendExpertRoleId}
                    }
                });
            }
            if (!workFlowStepRepository.TableNoTracking.Any(p => p.Id == 10009))
            {
                workFlowStepRepository.Add(new WorkFlowStep
                {
                    Id = 10009,
                    WorkFlowId = (int)WorkFlowEnum.RequestFacility,
                    IsActive = true,
                    StepIsManual = true,
                    Name = "استعلام",
                    Description = "بررسی توسط کارشناس نهاد مالی(اخذ استعلام 8 گانه)",
                    WorkFlowFormId = WorkFlowFormEnum.VerifyLeasing,
                    WorkFlowStepRoles = new List<WorkFlowStepRole>()
                    {
                        new(){ RoleId = adminLeasingRoleId},
                        new(){ RoleId = supervisorLeasingRoleId},
                        new(){ RoleId = leasingRoleId}
                    }
                });
            }
            if (!workFlowStepRepository.TableNoTracking.Any(p => p.Id == 100010))
            {
                workFlowStepRepository.Add(new WorkFlowStep
                {
                    Id = 100010,
                    WorkFlowId = (int)WorkFlowEnum.RequestFacility,
                    IsActive = true,
                    StepIsManual = true,
                    Name = "در انتظار نتیجه استعلام",
                    Description = "در انتظار نتیجه استعلام",
                    ReturnToCorrectionNextStepId = 10008,
                    WorkFlowFormId = WorkFlowFormEnum.PendingForVerifyResult,
                    WorkFlowStepRoles = new List<WorkFlowStepRole>()
                    {
                        new(){ RoleId = adminLeasingRoleId},
                        new(){ RoleId = supervisorLeasingRoleId},
                        new(){ RoleId = leasingRoleId}
                    }
                });
            }
            if (!workFlowStepRepository.TableNoTracking.Any(p => p.Id == 100011))
            {
                workFlowStepRepository.Add(new WorkFlowStep
                {
                    Id = 100011,
                    WorkFlowId = (int)WorkFlowEnum.RequestFacility,
                    IsActive = true,
                    StepIsManual = true,
                    Name = "تنظیم شماره قرارداد/تسهیلات",
                    Description = "تنظیم شماره قرارداد/تسهیلات",
                    WorkFlowFormId = WorkFlowFormEnum.EnterFacilityNumber,
                    WorkFlowStepRoles = new List<WorkFlowStepRole>()
                    {
                        new(){ RoleId = adminLeasingRoleId},
                        new(){ RoleId = supervisorLeasingRoleId},
                        new(){ RoleId = leasingRoleId}
                    }
                });
            }
            if (!workFlowStepRepository.TableNoTracking.Any(p => p.Id == 100012))
            {
                workFlowStepRepository.Add(new WorkFlowStep
                {
                    Id = 100012,
                    WorkFlowId = (int)WorkFlowEnum.RequestFacility,
                    IsActive = true,
                    StepIsManual = true,
                    Name = "در انتظار تنظیم شماره قرارداد/تسهیلات",
                    Description = "در انتظار تنظیم شماره قرارداد/تسهیلات",
                    WorkFlowFormId = WorkFlowFormEnum.PendingEnterFacilityNumber,
                    WorkFlowStepRoles = new List<WorkFlowStepRole>()
                    {
                        new(){ RoleId = adminLeasingRoleId},
                        new(){ RoleId = supervisorLeasingRoleId},
                        new(){ RoleId = leasingRoleId}
                    }
                });
            }
            if (!workFlowStepRepository.TableNoTracking.Any(p => p.Id == 100013))
            {
                //facility requester must do this step if he/she has selected the "Purchase From Contracted Stores" option during submit request facility
                workFlowStepRepository.Add(new WorkFlowStep
                {
                    Id = 100013,
                    WorkFlowId = (int)WorkFlowEnum.RequestFacility,
                    IsActive = true,
                    StepIsManual = true,
                    Name = "پرداخت هزینه عضویت در باشگاه مشتریان",
                    Description = "پرداخت هزینه عضویت در باشگاه مشتریان",
                    WorkFlowFormId = WorkFlowFormEnum.PaySalesCommission,
                    WorkFlowStepRoles = new List<WorkFlowStepRole>()
                    {
                        new(){ RoleId = buyerRoleId}
                    }
                });
            }
            //if (!workFlowStepRepository.TableNoTracking.Any(p => p.Id == 100015))
            //{
            //    workFlowStepRepository.Add(new WorkFlowStep
            //    {
            //        Id = 100015,
            //        WorkFlowId = (int)WorkFlowEnum.RequestFacility,
            //        Name = "فرم بارگذاری و ثبت تضامین",
            //        Description = "فرم بارگذاری و ثبت تضامین",
            //        WorkFlowFormId = WorkFlowFormEnum.RegisterGuarantees,
            //        WorkFlowStepRoles = new List<WorkFlowStepRole>()
            //        {
            //            new(){ RoleId = buyerRoleId}
            //        }
            //    });
            //}
            if (!workFlowStepRepository.TableNoTracking.Any(p => p.Id == 100016))
            {
                workFlowStepRepository.Add(new WorkFlowStep
                {
                    Id = 100016,
                    WorkFlowId = (int)WorkFlowEnum.RequestFacility,
                    IsActive = true,
                    StepIsManual = true,
                    Name = "صدور و امضاء سفته",
                    Description = "صدور و امضاء سفته",
                    WorkFlowFormId = WorkFlowFormEnum.SignPromissoryByUser,
                    WorkFlowStepRoles = new List<WorkFlowStepRole>()
                    {
                        new(){ RoleId = buyerRoleId}
                    }
                });
            }
            if (!workFlowStepRepository.TableNoTracking.Any(p => p.Id == 100017))
            {
                workFlowStepRepository.Add(new WorkFlowStep
                {
                    Id = 100017,
                    WorkFlowId = (int)WorkFlowEnum.RequestFacility,
                    IsActive = true,
                    StepIsManual = false,
                    Name = "در انتظار امضاء سفته",
                    Description = "در انتظار امضاء سفته",
                    WorkFlowFormId = WorkFlowFormEnum.WaitingSignPromissoryByUser,
                    WorkFlowStepRoles = new List<WorkFlowStepRole>()
                    {
                        new(){ RoleId = buyerRoleId}
                    }
                });
            }
            if (!workFlowStepRepository.TableNoTracking.Any(p => p.Id == 100018))
            {
                workFlowStepRepository.Add(new WorkFlowStep
                {
                    Id = 100018,
                    WorkFlowId = (int)WorkFlowEnum.RequestFacility,
                    IsActive = true,
                    StepIsManual = true,
                    Name = "امضا قرارداد",
                    Description = "امضاء قرارداد",
                    WorkFlowFormId = WorkFlowFormEnum.SignContractByUser,
                    WorkFlowStepRoles = new List<WorkFlowStepRole>()
                    {
                        new(){ RoleId = buyerRoleId}
                    }
                });
            }
            if (!workFlowStepRepository.TableNoTracking.Any(p => p.Id == 100019))
            {
                workFlowStepRepository.Add(new WorkFlowStep
                {
                    Id = 100019,
                    WorkFlowId = (int)WorkFlowEnum.RequestFacility,
                    IsActive = true,
                    StepIsManual = false,
                    Name = "در انتظار امضای قرارداد",
                    Description = "در انتظار امضای قرارداد",
                    WorkFlowFormId = WorkFlowFormEnum.WaitingToSignContractByUser,
                    WorkFlowStepRoles = new List<WorkFlowStepRole>()
                    {
                        new(){ RoleId = buyerRoleId}
                    }
                });
            }
            if (!workFlowStepRepository.TableNoTracking.Any(p => p.Id == 100020))
            {
                workFlowStepRepository.Add(new WorkFlowStep
                {
                    Id = 100020,
                    WorkFlowId = (int)WorkFlowEnum.RequestFacility,
                    IsActive = true,
                    StepIsManual = true,
                    Name = "صدور بن کارت",
                    Description = "صدور بن کارت",
                    //ReturnToCorrectionNextStepId = 100015,
                    WorkFlowFormId = WorkFlowFormEnum.CardIssuance,
                    WorkFlowStepRoles = new List<WorkFlowStepRole>()
                    {
                        new(){ RoleId = zarinLendSuperAdminRoleId},
                        new(){ RoleId = zarinLendAdminRoleId},
                        new(){ RoleId = zarinLendExpertRoleId}
                    }
                });
            }
            if (!workFlowStepRepository.TableNoTracking.Any(p => p.Id == 100021))
            {
                workFlowStepRepository.Add(new WorkFlowStep
                {
                    Id = 100021,
                    WorkFlowId = (int)WorkFlowEnum.RequestFacility,
                    IsActive = true,
                    StepIsManual = true,
                    Name = "بررسی نهایی/ثبت شماره انتظامی",
                    Description = "بررسی نهایی/ثبت شماره انتظامی",
                    WorkFlowFormId = WorkFlowFormEnum.BankLeasingInquiry,
                    WorkFlowStepRoles = new List<WorkFlowStepRole>()
                    {
                        new(){ RoleId = adminLeasingRoleId},
                        new(){ RoleId = supervisorLeasingRoleId},
                        new(){ RoleId = leasingRoleId}
                    }
                });
            }
            if (!workFlowStepRepository.TableNoTracking.Any(p => p.Id == 100022))
            {
                workFlowStepRepository.Add(new WorkFlowStep
                {
                    Id = 100022,
                    WorkFlowId = (int)WorkFlowEnum.RequestFacility,
                    IsActive = true,
                    StepIsManual = true,
                    Name = "در انتظار بررسی نهایی/ثبت شماره انتظامی",
                    Description = "در انتظار بررسی نهایی/ثبت شماره انتظامی",
                    WorkFlowFormId = WorkFlowFormEnum.PendingBankLeasingInquiry,
                    WorkFlowStepRoles = new List<WorkFlowStepRole>()
                    {
                        new(){ RoleId = adminLeasingRoleId},
                        new(){ RoleId = supervisorLeasingRoleId},
                        new(){ RoleId = leasingRoleId}
                    }
                });
            }
            if (!workFlowStepRepository.TableNoTracking.Any(p => p.Id == 100023))
            {
                workFlowStepRepository.Add(new WorkFlowStep
                {
                    Id = 100023,
                    WorkFlowId = (int)WorkFlowEnum.RequestFacility,
                    IsActive = true,
                    StepIsManual = true,
                    Name = "تایید مدیر بانک",
                    Description = "در انتظار تایید و امضای قرارداد توسط مدیر بانک",
                    WorkFlowFormId = WorkFlowFormEnum.AdminBankLeasingSignature,
                    CanBeIncomplete = true,
                    WorkFlowStepRoles = new List<WorkFlowStepRole>()
                    {
                        new(){ RoleId = adminLeasingRoleId}
                    }
                });
            }
            if (!workFlowStepRepository.TableNoTracking.Any(p => p.Id == 100024))
            {
                workFlowStepRepository.Add(new WorkFlowStep
                {
                    Id = 100024,
                    WorkFlowId = (int)WorkFlowEnum.RequestFacility,
                    IsActive = true,
                    StepIsManual = true,
                    Name = "واریز تسهیلات",
                    Description = "واریز تسهیلات به حساب زرین لند",
                    WorkFlowFormId = WorkFlowFormEnum.DepositFacilityAmount,
                    WorkFlowStepRoles = new List<WorkFlowStepRole>()
                    {
                        new(){ RoleId = adminLeasingRoleId},
                        new(){ RoleId = supervisorLeasingRoleId},
                        new(){ RoleId = leasingRoleId}
                    }
                });
            }
            if (!workFlowStepRepository.TableNoTracking.Any(p => p.Id == 1000241))
            {
                workFlowStepRepository.Add(new WorkFlowStep
                {
                    Id = 1000241,
                    WorkFlowId = (int)WorkFlowEnum.RequestFacility,
                    IsActive = true,
                    StepIsManual = true,
                    Name = "در انتظار واریز تسهیلات",
                    Description = "در انتظار واریز تسهیلات به حساب زرین لند",
                    WorkFlowFormId = WorkFlowFormEnum.PendingDepositFacilityAmount,
                    WorkFlowStepRoles = new List<WorkFlowStepRole>()
                    {
                        new(){ RoleId = adminLeasingRoleId},
                        new(){ RoleId = supervisorLeasingRoleId},
                        new(){ RoleId = leasingRoleId}
                    }
                });
            }
            if (!workFlowStepRepository.TableNoTracking.Any(p => p.Id == 100025))
            {
                workFlowStepRepository.Add(new WorkFlowStep
                {
                    Id = 100025,
                    WorkFlowId = (int)WorkFlowEnum.RequestFacility,
                    IsActive = true,
                    StepIsManual = true,
                    IsLastStep = false,
                    IsApproveFinalStep = false,
                    Name = "شارژ بن کارت",
                    Description = "واریز تسهیلات به کارت مشتری",
                    WorkFlowFormId = WorkFlowFormEnum.CardRecharge,
                    WorkFlowStepRoles = new List<WorkFlowStepRole>()
                    {
                        new(){ RoleId = leasingRoleId},
                        new(){ RoleId = adminLeasingRoleId},
                        new(){ RoleId = supervisorLeasingRoleId},
                        new(){ RoleId = zarinLendAdminRoleId},
                        new(){ RoleId = zarinLendSuperAdminRoleId},
                        new(){ RoleId = zarinLendExpertRoleId}
                    }
                });
            }
            if (!workFlowStepRepository.TableNoTracking.Any(p => p.Id == 1000251))
            {
                workFlowStepRepository.Add(new WorkFlowStep
                {
                    Id = 1000251,
                    WorkFlowId = (int)WorkFlowEnum.RequestFacility,
                    IsActive = true,
                    StepIsManual = true,
                    IsLastStep = true,
                    IsApproveFinalStep = true,
                    Name = "تکمیل اطلاعات بن کارت",
                    Description = "تکمیل اطلاعات بن کارت",
                    WorkFlowFormId = WorkFlowFormEnum.CompleteBonCardInfo,
                    WorkFlowStepRoles = new List<WorkFlowStepRole>()
                    {
                        new(){ RoleId = leasingRoleId},
                        new(){ RoleId = adminLeasingRoleId},
                        new(){ RoleId = zarinLendExpertRoleId}
                    }
                });
            }
            if (!workFlowStepRepository.TableNoTracking.Any(p => p.Id == 100026))
            {
                workFlowStepRepository.Add(new WorkFlowStep
                {
                    Id = 100026,
                    WorkFlowId = (int)WorkFlowEnum.RequestFacility,
                    IsLastStep = false,
                    StepIsManual = true,
                    Name = "بررسی اولیه زرین لند",
                    Description = "بررسی اولیه زرین لند - برگشت جهت اصلاح - ثبت شماره انتظامی",
                    IsActive = true,
                    WorkFlowFormId = WorkFlowFormEnum.VerifyZarrinLend,
                    WorkFlowStepRoles = new List<WorkFlowStepRole>()
                    {
                        new(){ RoleId = zarinLendAdminRoleId },
                        new(){ RoleId = zarinLendSuperAdminRoleId },
                        new(){ RoleId = zarinLendExpertRoleId }
                    }
                });
            }
            if (!workFlowStepRepository.TableNoTracking.Any(p => p.Id == 100027))
            {
                workFlowStepRepository.Add(new WorkFlowStep
                {
                    Id = 100027,
                    WorkFlowId = (int)WorkFlowEnum.RequestFacility,
                    IsLastStep = false,
                    StepIsManual = true,
                    Name = "بررسی اولیه زرین لند",
                    Description = "بررسی اولیه زرین لند - برگشت جهت اصلاح - واریز تسهیلات",
                    IsActive = true,
                    WorkFlowFormId = WorkFlowFormEnum.VerifyZarrinLend,
                    WorkFlowStepRoles = new List<WorkFlowStepRole>()
                    {
                        new(){ RoleId = zarinLendAdminRoleId },
                        new(){ RoleId = zarinLendSuperAdminRoleId },
                        new(){ RoleId = zarinLendExpertRoleId }
                    }
                });
            }
            if (!workFlowStepRepository.TableNoTracking.Any(p => p.Id == 100028))
            {
                workFlowStepRepository.Add(new WorkFlowStep
                {
                    Id = 100028,
                    WorkFlowId = (int)WorkFlowEnum.RequestFacility,
                    IsLastStep = false,
                    StepIsManual = true,
                    Name = "بررسی اولیه زرین لند",
                    Description = "بررسی اولیه زرین لند - برگشت جهت اصلاح - در انتظار تنظیم شماره قرارداد/تسهیلات",
                    IsActive = true,
                    WorkFlowFormId = WorkFlowFormEnum.VerifyZarrinLend,
                    WorkFlowStepRoles = new List<WorkFlowStepRole>()
                    {
                        new(){ RoleId = zarinLendAdminRoleId },
                        new(){ RoleId = zarinLendSuperAdminRoleId },
                        new(){ RoleId = zarinLendExpertRoleId }
                    }
                });
            }
            if (!workFlowStepRepository.TableNoTracking.Any(p => p.Id == 100029))
            {
                workFlowStepRepository.Add(new WorkFlowStep
                {
                    Id = 100029,
                    WorkFlowId = (int)WorkFlowEnum.RequestFacility,
                    IsLastStep = false,
                    StepIsManual = true,
                    Name = "بررسی اولیه زرین لند",
                    Description = "بررسی اولیه زرین لند - برگشت جهت اصلاح - در انتظار بررسی نهایی/ثبت شماره انتظامی",
                    IsActive = true,
                    WorkFlowFormId = WorkFlowFormEnum.VerifyZarrinLend,
                    WorkFlowStepRoles = new List<WorkFlowStepRole>()
                    {
                        new(){ RoleId = zarinLendAdminRoleId },
                        new(){ RoleId = zarinLendSuperAdminRoleId },
                        new(){ RoleId = zarinLendExpertRoleId }
                    }
                });
            }
            if (!workFlowStepRepository.TableNoTracking.Any(p => p.Id == 100030))
            {
                workFlowStepRepository.Add(new WorkFlowStep
                {
                    Id = 100030,
                    WorkFlowId = (int)WorkFlowEnum.RequestFacility,
                    IsLastStep = false,
                    StepIsManual = true,
                    Name = "بررسی اولیه زرین لند",
                    Description = "بررسی اولیه زرین لند - برگشت جهت اصلاح - در انتظار واریز تسهیلات",
                    IsActive = true,
                    WorkFlowFormId = WorkFlowFormEnum.VerifyZarrinLend,
                    WorkFlowStepRoles = new List<WorkFlowStepRole>()
                    {
                        new(){ RoleId = zarinLendAdminRoleId },
                        new(){ RoleId = zarinLendSuperAdminRoleId },
                        new(){ RoleId = zarinLendExpertRoleId }
                    }
                });
            }
            if (!workFlowStepRepository.TableNoTracking.Any(p => p.Id == 100031))
            {
                workFlowStepRepository.Add(new WorkFlowStep
                {
                    Id = 100031,
                    WorkFlowId = (int)WorkFlowEnum.RequestFacility,
                    IsLastStep = false,
                    StepIsManual = true,
                    Name = "بررسی اولیه زرین لند",
                    Description = "بررسی اولیه زرین لند - برگشت جهت اصلاح - تایید مدیر بانک",
                    IsActive = true,
                    WorkFlowFormId = WorkFlowFormEnum.VerifyZarrinLend,
                    WorkFlowStepRoles = new List<WorkFlowStepRole>()
                    {
                        new(){ RoleId = zarinLendAdminRoleId },
                        new(){ RoleId = zarinLendSuperAdminRoleId },
                        new(){ RoleId = zarinLendExpertRoleId }
                    }
                });
            }

            #region Update Steps(ApproveNextStepId,...)
            if (workFlowStepRepository.TableNoTracking.Any(p => p.Id == 10002))
            {
                var entity = workFlowStepRepository.GetById(10002);
                entity.ApproveNextStepId = 10003;
                workFlowStepRepository.Update(entity);
            }
            if (workFlowStepRepository.TableNoTracking.Any(p => p.Id == 10003))
            {
                var entity = workFlowStepRepository.GetById(10003);
                entity.ApproveNextStepId = 10004;
                workFlowStepRepository.Update(entity);
            }
            if (workFlowStepRepository.TableNoTracking.Any(p => p.Id == 10004))
            {
                var entity = workFlowStepRepository.GetById(10004);
                entity.ApproveNextStepId = 10008;
                workFlowStepRepository.Update(entity);
            }
            if (workFlowStepRepository.TableNoTracking.Any(p => p.Id == 10008))
            {
                var entity = workFlowStepRepository.GetById(10008);
                entity.ApproveNextStepId = 10009;
                workFlowStepRepository.Update(entity);
            }
            if (workFlowStepRepository.TableNoTracking.Any(p => p.Id == 10009))
            {
                var entity = workFlowStepRepository.GetById(10009);
                entity.ApproveNextStepId = 100010;
                workFlowStepRepository.Update(entity);
            }
            if (workFlowStepRepository.TableNoTracking.Any(p => p.Id == 100010))
            {
                var entity = workFlowStepRepository.GetById(100010);
                entity.ApproveNextStepId = 100011;
                workFlowStepRepository.Update(entity);
            }
            if (workFlowStepRepository.TableNoTracking.Any(p => p.Id == 100011))
            {
                var entity = workFlowStepRepository.GetById(100011);
                entity.ApproveNextStepId = 100012;
                entity.ReturnToCorrectionNextStepId = 10008;
                workFlowStepRepository.Update(entity);
            }
            if (workFlowStepRepository.TableNoTracking.Any(p => p.Id == 100012))
            {
                var entity = workFlowStepRepository.GetById(100012);
                entity.ApproveNextStepId = 100013;
                entity.ReturnToCorrectionNextStepId = 100028;
                workFlowStepRepository.Update(entity);
            }
            if (workFlowStepRepository.TableNoTracking.Any(p => p.Id == 100013))
            {
                var entity = workFlowStepRepository.GetById(100013);
                entity.ApproveNextStepId = 100016;
                workFlowStepRepository.Update(entity);
            }
            //if (workFlowStepRepository.TableNoTracking.Any(p => p.Id == 100015))
            //{
            //    var entity = workFlowStepRepository.GetById(100015);
            //    entity.ApproveNextStepId = 100016;
            //    workFlowStepRepository.Update(entity);
            //}
            if (workFlowStepRepository.TableNoTracking.Any(p => p.Id == 100016))
            {
                var entity = workFlowStepRepository.GetById(100016);
                entity.ApproveNextStepId = 100017;
                workFlowStepRepository.Update(entity);
            }
            if (workFlowStepRepository.TableNoTracking.Any(p => p.Id == 100017))
            {
                var entity = workFlowStepRepository.GetById(100017);
                entity.ApproveNextStepId = 100018;
                workFlowStepRepository.Update(entity);
            }
            if (workFlowStepRepository.TableNoTracking.Any(p => p.Id == 100018))
            {
                var entity = workFlowStepRepository.GetById(100018);
                entity.ApproveNextStepId = 100019;
                workFlowStepRepository.Update(entity);
            }
            if (workFlowStepRepository.TableNoTracking.Any(p => p.Id == 100019))
            {
                var entity = workFlowStepRepository.GetById(100019);
                entity.ApproveNextStepId = 100020;
                workFlowStepRepository.Update(entity);
            }
            if (workFlowStepRepository.TableNoTracking.Any(p => p.Id == 100020))
            {
                var entity = workFlowStepRepository.GetById(100020);
                entity.ApproveNextStepId = 100021;
                workFlowStepRepository.Update(entity);
            }
            if (workFlowStepRepository.TableNoTracking.Any(p => p.Id == 100021))
            {
                var entity = workFlowStepRepository.GetById(100021);
                entity.ApproveNextStepId = 100022;
                entity.ReturnToCorrectionNextStepId = 100026;
                workFlowStepRepository.Update(entity);
            }
            if (workFlowStepRepository.TableNoTracking.Any(p => p.Id == 100022))
            {
                var entity = workFlowStepRepository.GetById(100022);
                entity.ApproveNextStepId = 100023;
                entity.ReturnToCorrectionNextStepId = 100029;
                workFlowStepRepository.Update(entity);
            }
            if (workFlowStepRepository.TableNoTracking.Any(p => p.Id == 100023))
            {
                var entity = workFlowStepRepository.GetById(100023);
                entity.ApproveNextStepId = 100024;
                entity.ReturnToCorrectionNextStepId = 100031;
                workFlowStepRepository.Update(entity);
            }
            if (workFlowStepRepository.TableNoTracking.Any(p => p.Id == 100024))
            {
                var entity = workFlowStepRepository.GetById(100024);
                entity.ApproveNextStepId = 1000241;
                entity.ReturnToCorrectionNextStepId = 100027;
                workFlowStepRepository.Update(entity);
            }
            if (workFlowStepRepository.TableNoTracking.Any(p => p.Id == 1000241))
            {
                var entity = workFlowStepRepository.GetById(1000241);
                entity.ApproveNextStepId = 100025;
                entity.ReturnToCorrectionNextStepId = 100030;
                workFlowStepRepository.Update(entity);
            }
            if (workFlowStepRepository.TableNoTracking.Any(p => p.Id == 100025))
            {
                var entity = workFlowStepRepository.GetById(100025);
                entity.ApproveNextStepId = 1000251;
                workFlowStepRepository.Update(entity);
            }
            if (workFlowStepRepository.TableNoTracking.Any(p => p.Id == 100026))
            {
                var entity = workFlowStepRepository.GetById(100026);
                entity.ApproveNextStepId = 100021;
                workFlowStepRepository.Update(entity);
            }
            if (workFlowStepRepository.TableNoTracking.Any(p => p.Id == 100027))
            {
                var entity = workFlowStepRepository.GetById(100027);
                entity.ApproveNextStepId = 100024;
                workFlowStepRepository.Update(entity);
            }
            if (workFlowStepRepository.TableNoTracking.Any(p => p.Id == 100028))
            {
                var entity = workFlowStepRepository.GetById(100028);
                entity.ApproveNextStepId = 100011;
                workFlowStepRepository.Update(entity);
            }
            if (workFlowStepRepository.TableNoTracking.Any(p => p.Id == 100029))
            {
                var entity = workFlowStepRepository.GetById(100029);
                entity.ApproveNextStepId = 100021;
                workFlowStepRepository.Update(entity);
            }
            if (workFlowStepRepository.TableNoTracking.Any(p => p.Id == 100030))
            {
                var entity = workFlowStepRepository.GetById(100030);
                entity.ApproveNextStepId = 100024;
                workFlowStepRepository.Update(entity);
            }
            if (workFlowStepRepository.TableNoTracking.Any(p => p.Id == 100031))
            {
                var entity = workFlowStepRepository.GetById(100031);
                entity.ApproveNextStepId = 100023;
                workFlowStepRepository.Update(entity);
            }
            #endregion

            #endregion 'Request Facility New Version' steps

            #region 'Request Facility Old Version' steps
            if (!workFlowStepRepository.TableNoTracking.Any(p => p.Id == 1))
            {
                workFlowStepRepository.Add(new WorkFlowStep
                {
                    Id = 1,
                    WorkFlowId = (int)WorkFlowEnum.RequestFacility_OldVersion,
                    IsLastStep = true,
                    Name = "رد درخواست",
                    Description = "رد درخواست",
                    WorkFlowStepRoles = new List<WorkFlowStepRole>()
                    {
                        new WorkFlowStepRole(){ RoleId = zarinLendAdminRoleId},
                        new WorkFlowStepRole(){ RoleId = zarinLendSuperAdminRoleId}
                    }
                });
            }
            if (!workFlowStepRepository.TableNoTracking.Any(p => p.Id == 2))
            {
                workFlowStepRepository.Add(new WorkFlowStep
                {
                    Id = 2,
                    WorkFlowId = (int)WorkFlowEnum.RequestFacility_OldVersion,
                    IsFirstStep = true,
                    Name = "ثبت درخواست",
                    Description = "ثبت درخواست تسهیلات/میزان اعتبار درخواستی/مدت باز پرداخت",
                    WorkFlowFormId = WorkFlowFormEnum.RegisterRequestFacility,
                    WorkFlowStepRoles = new List<WorkFlowStepRole>()
                    {
                        new WorkFlowStepRole(){ RoleId = buyerRoleId}
                    }
                });
            }
            if (!workFlowStepRepository.TableNoTracking.Any(p => p.Id == 3))
            {
                workFlowStepRepository.Add(new WorkFlowStep
                {
                    Id = 3,
                    WorkFlowId = (int)WorkFlowEnum.RequestFacility_OldVersion,
                    Name = "تکمیل اطلاعات",
                    Description = "تکمیل اطلاعات هویتی",
                    WorkFlowFormId = WorkFlowFormEnum.RegisterIdentityInfo,
                    WorkFlowStepRoles = new List<WorkFlowStepRole>()
                    {
                        new WorkFlowStepRole(){ RoleId = buyerRoleId}
                    }
                });
            }
            if (!workFlowStepRepository.TableNoTracking.Any(p => p.Id == 4))
            {
                workFlowStepRepository.Add(new WorkFlowStep
                {
                    Id = 4,
                    WorkFlowId = (int)WorkFlowEnum.RequestFacility_OldVersion,
                    Name = "بارگذاری مدارک",
                    Description = "بارگذاری اسناد هویتی،کارت ملی،شناسنامه",
                    WorkFlowFormId = WorkFlowFormEnum.UploadIdentityDocuments,
                    WorkFlowStepRoles = new List<WorkFlowStepRole>()
                    {
                        new WorkFlowStepRole(){ RoleId = buyerRoleId}
                    }
                });
            }
            if (!workFlowStepRepository.TableNoTracking.Any(p => p.Id == 5))
            {
                workFlowStepRepository.Add(new WorkFlowStep
                {
                    Id = 5,
                    WorkFlowId = (int)WorkFlowEnum.RequestFacility_OldVersion,
                    Name = "پرداخت هزینه اعتبارسنجی",
                    Description = "پرداخت هزینه اعتبارسنجی",
                    WorkFlowFormId = WorkFlowFormEnum.PaymentVerifyShahkarAndSamatService,
                    WorkFlowStepRoles = new List<WorkFlowStepRole>()
                    {
                        new WorkFlowStepRole(){ RoleId = buyerRoleId}
                    }
                });
            }
            //این مرحله بصورت اتوماتیک انجام میشود
            if (!workFlowStepRepository.TableNoTracking.Any(p => p.Id == 6))
            {
                workFlowStepRepository.Add(new WorkFlowStep
                {
                    Id = 6,
                    WorkFlowId = (int)WorkFlowEnum.RequestFacility_OldVersion,
                    Name = "انجام اعتبارسنجی",
                    Description = "انجام اعتبارسنجی و نمایش به خریدار",
                    RejectNextStepId = 1,
                    WorkFlowFormId = WorkFlowFormEnum.VerifyShahkarAndSamatService,
                    StepIsManual = false,
                    WorkFlowStepRoles = new List<WorkFlowStepRole>()
                    {
                        new WorkFlowStepRole(){ RoleId = zarinLendAdminRoleId},
                        new WorkFlowStepRole(){ RoleId = zarinLendSuperAdminRoleId},
                        //new WorkFlowStepRole(){ RoleId = leasingRoleId},
                        new WorkFlowStepRole(){ RoleId = buyerRoleId},
                    }
                });
            }
            if (!workFlowStepRepository.TableNoTracking.Any(p => p.Id == 7))
            {
                workFlowStepRepository.Add(new WorkFlowStep
                {
                    Id = 7,
                    WorkFlowId = (int)WorkFlowEnum.RequestFacility_OldVersion,
                    Name = "انجام اعتبارسنجی",
                    Description = "انجام اعتبارسنجی",
                    RejectNextStepId = 1,
                    WorkFlowFormId = WorkFlowFormEnum.VerifyIranCreditScoring,
                    WorkFlowStepRoles = new List<WorkFlowStepRole>()
                    {
                        //new WorkFlowStepRole(){ RoleId = zarinLendAdminRoleId},
                        //new WorkFlowStepRole(){ RoleId = zarinLendSuperAdminRoleId},
                        //new WorkFlowStepRole(){ RoleId = leasingRoleId},
                        new WorkFlowStepRole(){ RoleId = buyerRoleId},
                    }
                });
            }
            if (!workFlowStepRepository.TableNoTracking.Any(p => p.Id == 8))
            {
                workFlowStepRepository.Add(new WorkFlowStep
                {
                    Id = 8,
                    WorkFlowId = (int)WorkFlowEnum.RequestFacility_OldVersion,
                    RejectNextStepId = 1,
                    ReturnToCorrectionNextStepId = 3,
                    Name = "بررسی اولیه زرین لند",
                    Description = "بررسی اولیه و ارجاع به نهاد مالی توسط کارشناس زرین لند",
                    WorkFlowFormId = WorkFlowFormEnum.VerifyZarrinLend,
                    WorkFlowStepRoles = new List<WorkFlowStepRole>()
                    {
                        new WorkFlowStepRole(){ RoleId = zarinLendAdminRoleId},
                        new WorkFlowStepRole(){ RoleId = zarinLendSuperAdminRoleId}
                    }
                });
            }
            if (!workFlowStepRepository.TableNoTracking.Any(p => p.Id == 9))
            {
                workFlowStepRepository.Add(new WorkFlowStep
                {
                    Id = 9,
                    WorkFlowId = (int)WorkFlowEnum.RequestFacility_OldVersion,
                    Name = "استعلام",
                    Description = "بررسی توسط کارشناس نهاد مالی(اخذ استعلام 8 گانه)",
                    WorkFlowFormId = WorkFlowFormEnum.VerifyLeasing,
                    WorkFlowStepRoles = new List<WorkFlowStepRole>()
                    {
                        new WorkFlowStepRole(){ RoleId = adminLeasingRoleId},
                        new WorkFlowStepRole(){ RoleId = supervisorLeasingRoleId},
                        new WorkFlowStepRole(){ RoleId = leasingRoleId}
                    }
                });
            }
            if (!workFlowStepRepository.TableNoTracking.Any(p => p.Id == 10))
            {
                workFlowStepRepository.Add(new WorkFlowStep
                {
                    Id = 10,
                    WorkFlowId = (int)WorkFlowEnum.RequestFacility_OldVersion,
                    Name = "در انتظار نتیجه استعلام",
                    Description = "در انتظار نتیجه استعلام",
                    ReturnToCorrectionNextStepId = 8,
                    WorkFlowFormId = WorkFlowFormEnum.PendingForVerifyResult,
                    WorkFlowStepRoles = new List<WorkFlowStepRole>()
                    {
                        new WorkFlowStepRole(){ RoleId = adminLeasingRoleId},
                        new WorkFlowStepRole(){ RoleId = supervisorLeasingRoleId},
                        new WorkFlowStepRole(){ RoleId = leasingRoleId}
                    }
                });
            }
            if (!workFlowStepRepository.TableNoTracking.Any(p => p.Id == 11))
            {
                workFlowStepRepository.Add(new WorkFlowStep
                {
                    Id = 11,
                    WorkFlowId = (int)WorkFlowEnum.RequestFacility_OldVersion,
                    Name = "تنظیم شماره قرارداد/تسهیلات",
                    Description = "تنظیم شماره قرارداد/تسهیلات",
                    WorkFlowFormId = WorkFlowFormEnum.EnterFacilityNumber,
                    WorkFlowStepRoles = new List<WorkFlowStepRole>()
                    {
                        new WorkFlowStepRole(){ RoleId = adminLeasingRoleId},
                        new WorkFlowStepRole(){ RoleId = supervisorLeasingRoleId},
                        new WorkFlowStepRole(){ RoleId = leasingRoleId}
                    }
                });
            }
            if (!workFlowStepRepository.TableNoTracking.Any(p => p.Id == 12))
            {
                workFlowStepRepository.Add(new WorkFlowStep
                {
                    Id = 12,
                    WorkFlowId = (int)WorkFlowEnum.RequestFacility_OldVersion,
                    Name = "در انتظار تنظیم شماره قرارداد/تسهیلات",
                    Description = "در انتظار تنظیم شماره قرارداد/تسهیلات",
                    WorkFlowFormId = WorkFlowFormEnum.PendingEnterFacilityNumber,
                    WorkFlowStepRoles = new List<WorkFlowStepRole>()
                    {
                        new WorkFlowStepRole(){ RoleId = adminLeasingRoleId},
                        new WorkFlowStepRole(){ RoleId = supervisorLeasingRoleId},
                        new WorkFlowStepRole(){ RoleId = leasingRoleId}
                    }
                });
            }
            if (!workFlowStepRepository.TableNoTracking.Any(p => p.Id == 13))
            {
                workFlowStepRepository.Add(new WorkFlowStep
                {
                    Id = 13,
                    WorkFlowId = (int)WorkFlowEnum.RequestFacility_OldVersion,
                    Name = "دریافت بیمه",
                    Description = "ارسال به بیمه و اطلاع رسانی به مشتری",
                    WorkFlowFormId = WorkFlowFormEnum.InsuranceIssuance,
                    WorkFlowStepRoles = new List<WorkFlowStepRole>()
                    {
                        new WorkFlowStepRole(){ RoleId = zarinLendSuperAdminRoleId},
                        new WorkFlowStepRole(){ RoleId = zarinLendAdminRoleId}
                    }
                });
            }
            if (!workFlowStepRepository.TableNoTracking.Any(p => p.Id == 14))
            {
                workFlowStepRepository.Add(new WorkFlowStep
                {
                    Id = 14,
                    WorkFlowId = (int)WorkFlowEnum.RequestFacility_OldVersion,
                    Name = "پرداخت هزینه عضویت در باشگاه مشتریان",
                    Description = "پرداخت هزینه عضویت در باشگاه مشتریان",
                    WorkFlowFormId = WorkFlowFormEnum.PaySalesCommission,
                    IsLastStep = false,
                    IsApproveFinalStep = false,
                    WorkFlowStepRoles = new List<WorkFlowStepRole>()
                    {
                        new WorkFlowStepRole(){ RoleId = buyerRoleId}
                    }
                });
            }
            if (!workFlowStepRepository.TableNoTracking.Any(p => p.Id == 15))
            {
                workFlowStepRepository.Add(new WorkFlowStep
                {
                    Id = 15,
                    WorkFlowId = (int)WorkFlowEnum.RequestFacility_OldVersion,
                    Name = "فرم بارگذاری و ثبت تضامین",
                    Description = "فرم بارگذاری و ثبت تضامین",
                    WorkFlowFormId = WorkFlowFormEnum.RegisterGuarantees,
                    WorkFlowStepRoles = new List<WorkFlowStepRole>()
                    {
                        new WorkFlowStepRole(){ RoleId = buyerRoleId}
                    }
                });
            }
            if (!workFlowStepRepository.TableNoTracking.Any(p => p.Id == 16))
            {
                workFlowStepRepository.Add(new WorkFlowStep
                {
                    Id = 16,
                    WorkFlowId = (int)WorkFlowEnum.RequestFacility_OldVersion,
                    Name = "امضا قرارداد",
                    Description = "امضاء قرارداد",
                    WorkFlowFormId = WorkFlowFormEnum.SignContractByUser,
                    WorkFlowStepRoles = new List<WorkFlowStepRole>()
                    {
                        new WorkFlowStepRole(){ RoleId = buyerRoleId}
                    }
                });
            }
            if (!workFlowStepRepository.TableNoTracking.Any(p => p.Id == 17))
            {
                workFlowStepRepository.Add(new WorkFlowStep
                {
                    Id = 17,
                    WorkFlowId = (int)WorkFlowEnum.RequestFacility_OldVersion,
                    Name = "در انتظار امضای قرارداد",
                    Description = "در انتظار امضای قرارداد",
                    WorkFlowFormId = WorkFlowFormEnum.WaitingToSignContractByUser,
                    StepIsManual = false,
                    WorkFlowStepRoles = new List<WorkFlowStepRole>()
                    {
                        new WorkFlowStepRole(){ RoleId = buyerRoleId}
                    }
                });
            }
            if (!workFlowStepRepository.TableNoTracking.Any(p => p.Id == 18))
            {
                workFlowStepRepository.Add(new WorkFlowStep
                {
                    Id = 18,
                    WorkFlowId = (int)WorkFlowEnum.RequestFacility_OldVersion,
                    Name = "صدور بن کارت",
                    Description = "صدور بن کارت",
                    ReturnToCorrectionNextStepId = 15,
                    WorkFlowFormId = WorkFlowFormEnum.CardIssuance,
                    WorkFlowStepRoles = new List<WorkFlowStepRole>()
                    {
                        new WorkFlowStepRole(){ RoleId = zarinLendSuperAdminRoleId},
                        new WorkFlowStepRole(){ RoleId = zarinLendAdminRoleId}
                    }
                });
            }
            if (!workFlowStepRepository.TableNoTracking.Any(p => p.Id == 19))
            {
                workFlowStepRepository.Add(new WorkFlowStep
                {
                    Id = 19,
                    WorkFlowId = (int)WorkFlowEnum.RequestFacility_OldVersion,
                    Name = "بررسی نهایی/ثبت شماره انتظامی",
                    Description = "بررسی نهایی/ثبت شماره انتظامی",
                    WorkFlowFormId = WorkFlowFormEnum.BankLeasingInquiry,
                    WorkFlowStepRoles = new List<WorkFlowStepRole>()
                    {
                        new WorkFlowStepRole(){ RoleId = adminLeasingRoleId},
                        new WorkFlowStepRole(){ RoleId = supervisorLeasingRoleId},
                        new WorkFlowStepRole(){ RoleId = leasingRoleId}
                    }
                });
            }
            if (!workFlowStepRepository.TableNoTracking.Any(p => p.Id == 20))
            {
                workFlowStepRepository.Add(new WorkFlowStep
                {
                    Id = 20,
                    WorkFlowId = (int)WorkFlowEnum.RequestFacility_OldVersion,
                    Name = "در انتظار بررسی نهایی/ثبت شماره انتظامی",
                    Description = "در انتظار بررسی نهایی/ثبت شماره انتظامی",
                    WorkFlowFormId = WorkFlowFormEnum.PendingBankLeasingInquiry,
                    WorkFlowStepRoles = new List<WorkFlowStepRole>()
                    {
                        new WorkFlowStepRole(){ RoleId = adminLeasingRoleId},
                        new WorkFlowStepRole(){ RoleId = supervisorLeasingRoleId},
                        new WorkFlowStepRole(){ RoleId = leasingRoleId}
                    }
                });
            }
            if (!workFlowStepRepository.TableNoTracking.Any(p => p.Id == 21))
            {
                workFlowStepRepository.Add(new WorkFlowStep
                {
                    Id = 21,
                    WorkFlowId = (int)WorkFlowEnum.RequestFacility_OldVersion,
                    Name = "امضاء قرارداد",
                    Description = "امضاء قرارداد دیجیتال مدیر بانک",
                    WorkFlowFormId = WorkFlowFormEnum.AdminBankLeasingSignature,
                    CanBeIncomplete = true,
                    WorkFlowStepRoles = new List<WorkFlowStepRole>()
                    {
                        new WorkFlowStepRole(){ RoleId = adminLeasingRoleId}
                    }
                });
            }
            if (!workFlowStepRepository.TableNoTracking.Any(p => p.Id == 22))
            {
                workFlowStepRepository.Add(new WorkFlowStep
                {
                    Id = 22,
                    WorkFlowId = (int)WorkFlowEnum.RequestFacility_OldVersion,
                    Name = "واریز تسهیلات",
                    Description = "واریز تسهیلات به حساب زرین لند",
                    WorkFlowFormId = WorkFlowFormEnum.DepositFacilityAmount,
                    WorkFlowStepRoles = new List<WorkFlowStepRole>()
                    {
                        new WorkFlowStepRole(){ RoleId = adminLeasingRoleId},
                        new WorkFlowStepRole(){ RoleId = supervisorLeasingRoleId},
                        new WorkFlowStepRole(){ RoleId = leasingRoleId}
                    }
                });
            }
            if (!workFlowStepRepository.TableNoTracking.Any(p => p.Id == 23))
            {
                workFlowStepRepository.Add(new WorkFlowStep
                {
                    Id = 23,
                    WorkFlowId = (int)WorkFlowEnum.RequestFacility_OldVersion,
                    IsLastStep = true,
                    IsApproveFinalStep = true,
                    Name = "شارژ بن کارت",
                    Description = "واریز تسهیلات به کارت مشتری",
                    WorkFlowFormId = WorkFlowFormEnum.CardRecharge,
                    WorkFlowStepRoles = new List<WorkFlowStepRole>()
                    {
                        new WorkFlowStepRole(){ RoleId = leasingRoleId},
                        new WorkFlowStepRole(){ RoleId = adminLeasingRoleId},
                        new WorkFlowStepRole(){ RoleId = supervisorLeasingRoleId},
                        new WorkFlowStepRole(){ RoleId = zarinLendAdminRoleId},
                        new WorkFlowStepRole(){ RoleId = zarinLendSuperAdminRoleId}
                    }
                });
            }

            #region comment steps

            //if (!workFlowStepRepository.TableNoTracking.Any(p => p.Id == 11))
            //{
            //    workFlowStepRepository.Add(new WorkFlowStep
            //    {
            //        Id = 11,
            //        WorkFlowId = 1,
            //        RejectNextStepId = 1,
            //        ReturnToCorrectionNextStepId = 10,
            //        Name = "بررسی چک توسط زرین لند",
            //        Description = "بررسی چک توسط زرین لند",
            //        WorkFlowFormId = WorkFlowFormEnum.VerifyCheckByZarinLend,
            //        WorkFlowStepRoles = new List<WorkFlowStepRole>()
            //        {
            //            new WorkFlowStepRole(){ RoleId = zarinLendAdminRoleId},
            //            new WorkFlowStepRole(){ RoleId = zarinLendSuperAdminRoleId}
            //        }
            //    });
            //}
            //if (!workFlowStepRepository.TableNoTracking.Any(p => p.Id == 12))
            //{
            //    workFlowStepRepository.Add(new WorkFlowStep
            //    {
            //        Id = 12,
            //        WorkFlowId = 1,
            //        ReturnToCorrectionNextStepId = 11,
            //        Name = "بررسی چک توسط نهاد مالی",
            //        Description = "بررسی چک توسط نهاد مالی",
            //        WorkFlowFormId = WorkFlowFormEnum.VerifyCheckByLeasing,
            //        WorkFlowStepRoles = new List<WorkFlowStepRole>()
            //        {
            //            new WorkFlowStepRole(){ RoleId = leasingRoleId}
            //        }
            //    });
            //}
            #endregion comment steps

            #region Update Steps(ApproveNextStepId,...)
            //if (workFlowStepRepository.TableNoTracking.Any(p => p.Id == 2))
            //{
            //    var entity = workFlowStepRepository.GetById(2);
            //    entity.ApproveNextStepId = 3;
            //    workFlowStepRepository.Update(entity);
            //}
            //if (workFlowStepRepository.TableNoTracking.Any(p => p.Id == 3))
            //{
            //    var entity = workFlowStepRepository.GetById(3);
            //    entity.ApproveNextStepId = 4;
            //    workFlowStepRepository.Update(entity);
            //}
            //if (workFlowStepRepository.TableNoTracking.Any(p => p.Id == 4))
            //{
            //    var entity = workFlowStepRepository.GetById(4);
            //    entity.ApproveNextStepId = 5;
            //    workFlowStepRepository.Update(entity);
            //}
            //if (workFlowStepRepository.TableNoTracking.Any(p => p.Id == 5))
            //{
            //    var entity = workFlowStepRepository.GetById(5);
            //    entity.ApproveNextStepId = 6;
            //    workFlowStepRepository.Update(entity);
            //}
            //if (workFlowStepRepository.TableNoTracking.Any(p => p.Id == 6))
            //{
            //    var entity = workFlowStepRepository.GetById(6);
            //    entity.ApproveNextStepId = 7;
            //    workFlowStepRepository.Update(entity);
            //}
            //if (workFlowStepRepository.TableNoTracking.Any(p => p.Id == 7))
            //{
            //    var entity = workFlowStepRepository.GetById(7);
            //    entity.ApproveNextStepId = 8;
            //    workFlowStepRepository.Update(entity);
            //}
            //if (workFlowStepRepository.TableNoTracking.Any(p => p.Id == 8))
            //{
            //    var entity = workFlowStepRepository.GetById(8);
            //    entity.ApproveNextStepId = 9;
            //    workFlowStepRepository.Update(entity);
            //}
            //if (workFlowStepRepository.TableNoTracking.Any(p => p.Id == 9))
            //{
            //    var entity = workFlowStepRepository.GetById(9);
            //    entity.ApproveNextStepId = 10;
            //    workFlowStepRepository.Update(entity);
            //}
            //if (workFlowStepRepository.TableNoTracking.Any(p => p.Id == 10))
            //{
            //    var entity = workFlowStepRepository.GetById(10);
            //    entity.ApproveNextStepId = 11;
            //    workFlowStepRepository.Update(entity);
            //}
            //if (workFlowStepRepository.TableNoTracking.Any(p => p.Id == 11))
            //{
            //    var entity = workFlowStepRepository.GetById(11);
            //    entity.ApproveNextStepId = 12;
            //    workFlowStepRepository.Update(entity);
            //}
            //if (workFlowStepRepository.TableNoTracking.Any(p => p.Id == 12))
            //{
            //    var entity = workFlowStepRepository.GetById(12);
            //    entity.ApproveNextStepId = 13;
            //    workFlowStepRepository.Update(entity);
            //}
            //if (workFlowStepRepository.TableNoTracking.Any(p => p.Id == 13))
            //{
            //    var entity = workFlowStepRepository.GetById(13);
            //    entity.ApproveNextStepId = 14;
            //    workFlowStepRepository.Update(entity);
            //}
            //if (workFlowStepRepository.TableNoTracking.Any(p => p.Id == 14))
            //{
            //    var entity = workFlowStepRepository.GetById(14);
            //    entity.ApproveNextStepId = 15;
            //    workFlowStepRepository.Update(entity);
            //}
            //if (workFlowStepRepository.TableNoTracking.Any(p => p.Id == 15))
            //{
            //    var entity = workFlowStepRepository.GetById(15);
            //    entity.ApproveNextStepId = 16;
            //    workFlowStepRepository.Update(entity);
            //}
            //if (workFlowStepRepository.TableNoTracking.Any(p => p.Id == 16))
            //{
            //    var entity = workFlowStepRepository.GetById(16);
            //    entity.ApproveNextStepId = 17;
            //    workFlowStepRepository.Update(entity);
            //}
            //if (workFlowStepRepository.TableNoTracking.Any(p => p.Id == 17))
            //{
            //    var entity = workFlowStepRepository.GetById(17);
            //    entity.ApproveNextStepId = 18;
            //    workFlowStepRepository.Update(entity);
            //}
            //if (workFlowStepRepository.TableNoTracking.Any(p => p.Id == 18))
            //{
            //    var entity = workFlowStepRepository.GetById(18);
            //    entity.ApproveNextStepId = 19;
            //    workFlowStepRepository.Update(entity);
            //}
            //if (workFlowStepRepository.TableNoTracking.Any(p => p.Id == 19))
            //{
            //    var entity = workFlowStepRepository.GetById(19);
            //    entity.ApproveNextStepId = 20;
            //    workFlowStepRepository.Update(entity);
            //}
            //if (workFlowStepRepository.TableNoTracking.Any(p => p.Id == 20))
            //{
            //    var entity = workFlowStepRepository.GetById(20);
            //    entity.ApproveNextStepId = 21;
            //    workFlowStepRepository.Update(entity);
            //}
            //if (workFlowStepRepository.TableNoTracking.Any(p => p.Id == 21))
            //{
            //    var entity = workFlowStepRepository.GetById(21);
            //    entity.ApproveNextStepId = 22;
            //    workFlowStepRepository.Update(entity);
            //}
            //if (workFlowStepRepository.TableNoTracking.Any(p => p.Id == 22))
            //{
            //    var entity = workFlowStepRepository.GetById(22);
            //    entity.ApproveNextStepId = 23;
            //    workFlowStepRepository.Update(entity);
            //}
            #endregion

            #endregion 'Request Facility' steps

            #region 'Register Guarantor' steps

            if (!workFlowStepRepository.TableNoTracking.Any(p => p.Id == 1001))
            {
                workFlowStepRepository.Add(new WorkFlowStep
                {
                    Id = 1001,
                    WorkFlowId = (int)WorkFlowEnum.RegisterGuarantor,
                    IsLastStep = true,
                    Name = "رد درخواست",
                    Description = "رد درخواست",
                    WorkFlowStepRoles = new List<WorkFlowStepRole>()
                    {
                        new WorkFlowStepRole(){ RoleId = zarinLendAdminRoleId},
                        new WorkFlowStepRole() { RoleId = zarinLendExpertRoleId },
                        new WorkFlowStepRole(){ RoleId = zarinLendSuperAdminRoleId}
                    }
                });
            }
            if (!workFlowStepRepository.TableNoTracking.Any(p => p.Id == 1002))
            {
                workFlowStepRepository.Add(new WorkFlowStep
                {
                    Id = 1002,
                    WorkFlowId = (int)WorkFlowEnum.RegisterGuarantor,
                    IsFirstStep = true,
                    Name = "ثبت درخواست جهت ضامن شدن",
                    Description = "ثبت درخواست جهت ضامن شدن",
                    WorkFlowFormId = WorkFlowFormEnum.RegisterRequestGuarantor,
                    WorkFlowStepRoles = new List<WorkFlowStepRole>()
                    {
                        new WorkFlowStepRole(){ RoleId = buyerRoleId}
                    }
                });
            }
            if (!workFlowStepRepository.TableNoTracking.Any(p => p.Id == 1003))
            {
                workFlowStepRepository.Add(new WorkFlowStep
                {
                    Id = 1003,
                    WorkFlowId = (int)WorkFlowEnum.RegisterGuarantor,
                    Name = "تکمیل اطلاعات هویتی",
                    Description = "تکمیل اطلاعات هویتی",
                    WorkFlowFormId = WorkFlowFormEnum.EditGuarantorInfo,
                    WorkFlowStepRoles = new List<WorkFlowStepRole>()
                    {
                        new WorkFlowStepRole(){ RoleId = buyerRoleId}
                    }
                });
            }
            if (!workFlowStepRepository.TableNoTracking.Any(p => p.Id == 1004))
            {
                workFlowStepRepository.Add(new WorkFlowStep
                {
                    Id = 1004,
                    WorkFlowId = (int)WorkFlowEnum.RegisterGuarantor,
                    Name = "بارگذاری مدارک",
                    Description = "بارگذاری اسناد هویتی،کارت ملی،شناسنامه",
                    WorkFlowFormId = WorkFlowFormEnum.UploadIdentityDocumentsGuarantor,
                    WorkFlowStepRoles = new List<WorkFlowStepRole>()
                    {
                        new WorkFlowStepRole(){ RoleId = buyerRoleId}
                    }
                });
            }
            if (!workFlowStepRepository.TableNoTracking.Any(p => p.Id == 1005))
            {
                workFlowStepRepository.Add(new WorkFlowStep
                {
                    Id = 1005,
                    IsLastStep = false,
                    IsApproveFinalStep = false,
                    WorkFlowId = (int)WorkFlowEnum.RegisterGuarantor,
                    Name = "بررسی اطلاعات و مدارک",
                    Description = "بررسی اطلاعات و مدارک",
                    WorkFlowFormId = WorkFlowFormEnum.VerifyGuarantorByZarinLend,
                    ReturnToCorrectionNextStepId = 1003,
                    RejectNextStepId = 1001,
                    WorkFlowStepRoles = new List<WorkFlowStepRole>()
                    {
                        new WorkFlowStepRole() { RoleId = zarinLendAdminRoleId },
                        new WorkFlowStepRole() { RoleId = zarinLendExpertRoleId },
                        new WorkFlowStepRole() { RoleId = zarinLendSuperAdminRoleId }
                    }
                });
            }
            if (!workFlowStepRepository.TableNoTracking.Any(p => p.Id == 1006))
            {
                workFlowStepRepository.Add(new WorkFlowStep
                {
                    Id = 1006,
                    IsLastStep = false,
                    IsApproveFinalStep = false,
                    WorkFlowId = (int)WorkFlowEnum.RegisterGuarantor,
                    Name = "ثبت اطلاعات و بارگذاری تصویر چک",
                    Description = "ثبت اطلاعات و بارگذاری تصویر چک",
                    WorkFlowFormId = WorkFlowFormEnum.RegisterGuaranteesByGuarantor,
                    WorkFlowStepRoles = new List<WorkFlowStepRole>()
                    {
                        new WorkFlowStepRole() { RoleId = buyerRoleId }
                    }
                });
            }
            if (!workFlowStepRepository.TableNoTracking.Any(p => p.Id == 1007))
            {
                workFlowStepRepository.Add(new WorkFlowStep
                {
                    Id = 1007,
                    IsLastStep = true,
                    IsApproveFinalStep = true,
                    WorkFlowId = (int)WorkFlowEnum.RegisterGuarantor,
                    Name = "بررسی چک",
                    Description = "بررسی اطلاعات چک و تصویر چک",
                    WorkFlowFormId = WorkFlowFormEnum.VerifyGuaranteesByZarinLend,
                    ReturnToCorrectionNextStepId = 1006,
                    RejectNextStepId = 1001,
                    WorkFlowStepRoles = new List<WorkFlowStepRole>()
                    {
                        new WorkFlowStepRole() { RoleId = zarinLendAdminRoleId },
                        new WorkFlowStepRole() { RoleId = zarinLendExpertRoleId },
                        new WorkFlowStepRole() { RoleId = zarinLendSuperAdminRoleId }
                    }
                });
            }
            #region Update Steps(ApproveNextStepId,...)

            if (workFlowStepRepository.TableNoTracking.Any(p => p.Id == 1002))
            {
                var entity = workFlowStepRepository.GetById(1002);
                entity.ApproveNextStepId = 1003;
                workFlowStepRepository.Update(entity);
            }
            if (workFlowStepRepository.TableNoTracking.Any(p => p.Id == 1003))
            {
                var entity = workFlowStepRepository.GetById(1003);
                entity.ApproveNextStepId = 1004;
                workFlowStepRepository.Update(entity);
            }
            if (workFlowStepRepository.TableNoTracking.Any(p => p.Id == 1004))
            {
                var entity = workFlowStepRepository.GetById(1004);
                entity.ApproveNextStepId = 1005;
                workFlowStepRepository.Update(entity);
            }
            if (workFlowStepRepository.TableNoTracking.Any(p => p.Id == 1005))
            {
                var entity = workFlowStepRepository.GetById(1005);
                entity.ApproveNextStepId = 1006;
                workFlowStepRepository.Update(entity);
            }
            if (workFlowStepRepository.TableNoTracking.Any(p => p.Id == 1006))
            {
                var entity = workFlowStepRepository.GetById(1006);
                entity.ApproveNextStepId = 1007;
                workFlowStepRepository.Update(entity);
            }

            #endregion

            #endregion 'Register Guarantor' steps
        }
    }
}