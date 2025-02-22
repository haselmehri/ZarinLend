using Core.Data.Repositories;
using Core.Entities;
using Services.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace ZarinLend.Services.DataInitializer;

public class WorkFlowStepRejectionReasonDataInitializer(IBaseRepository<WorkFlowStepRejectionReason> workFlowStepRejectionReasonRepository) : IDataInitializer
{
    public int Order => 2;

    public void InitializeData()
    {
        // برای WorkFlowStepId 100021
        int workFlowStepId1 = 100021;
        var rejectionReasonIds1 = new List<int>
        {
            16, // فرم‌ها یا تضامین فاقد امضای الکترونیک
            17, // مطابقت نداشتن مدارک بارگذاری شده
            18, // عدم تایید فرم‌ها یا تضامین
            19, // نقص مدارک بارگذاری شده
            3,  //رد کردن سقف مجاز وام"
            20, // ناواضح بودن تصویر مدرک
            2,  //تسهیلات سررسید گذشته
            1,  //مشتری دارای چک برگشتی
            21, // عدم تایید سفته
            22, // عدم تایید قراردادها
            23, // عدم تایید مدارک شناسایی
            24, // عدم تایید استعلام شناسنامه
            25, // عدم تایید استعلام کارت ملی
            26, // عدم تایید مدارک شغلی
            27, // به حد نصاب نرسیدن حقوق دریافتی
            28, // منقضی شدن زمان اعتبار مدرک
            29, // عدم اعتبار مهر یا امضا
            30, // عدم بارگذاری سابقه بیمه
            31, // مغایرت/نقص در صحت‌سنجی مدرک
            6,  //مشکل در گزارش اعتبار سنجی بانکی
            7,  //فاقد اعتبارسنجی معتبر
            8,  //پایین بودن نمره اعتبارسنجی بانکی
            9,
            10,
            12,
            13,
            14,
            15

        };

        var workFlowStepRejectionReasons1 = rejectionReasonIds1.Select(id => new WorkFlowStepRejectionReason
        {
            RejectionReasonId = id,
            WorkFlowStepId = workFlowStepId1,
            IsActive = true
        }).ToList();

        foreach (var workFlowStepRejectionReason in workFlowStepRejectionReasons1)
        {
            if (!workFlowStepRejectionReasonRepository.TableNoTracking.Any(p => p.RejectionReasonId == workFlowStepRejectionReason.RejectionReasonId && p.WorkFlowStepId == workFlowStepId1))
            {
                workFlowStepRejectionReasonRepository.Add(workFlowStepRejectionReason);
            }
        }

        // برای WorkFlowStepId 100024
        int workFlowStepId2 = 100024;
        var rejectionReasonIds2 = new List<int>
        {
            1,  // مشتری دارای چک برگشتی
            2,  // تسهیلات سررسید گذشته
            5,  // عدم تایید ثبت احوال
            9   // منقضی شدن اعتبارسنجی بانکی
        };

        var workFlowStepRejectionReasons2 = rejectionReasonIds2.Select(id => new WorkFlowStepRejectionReason
        {
            RejectionReasonId = id,
            WorkFlowStepId = workFlowStepId2,
            IsActive = true
        }).ToList();

        foreach (var workFlowStepRejectionReason in workFlowStepRejectionReasons2)
        {
            if (!workFlowStepRejectionReasonRepository.TableNoTracking.Any(p => p.RejectionReasonId == workFlowStepRejectionReason.RejectionReasonId && p.WorkFlowStepId == workFlowStepId2))
            {
                workFlowStepRejectionReasonRepository.Add(workFlowStepRejectionReason);
            }
        }

        // برای WorkFlowStepId 100011
        int workFlowStepId3 = 100011;
        var rejectionReasonIds3 = new List<int>
        {
            1,  // مشتری دارای چک برگشتی
            2,  // تسهیلات سررسید گذشته
            3,  // رد کردن سقف مجاز وام
            4,  // غیرمجاز بودن وضعیت نظام وظیفه
            5,  // عدم تایید ثبت احوال
            6,  // مشکل در گزارش اعتبار سنجی بانکی
            7,  // فاقد اعتبارسنجی معتبر
            8,  // پایین بودن نمره اعتبارسنجی بانکی
            9,  // منقضی شدن اعتبارسنجی بانکی
            10, // مشکل در حساب بانک آینده
            11, // فاقد شماره حساب بانک آینده
            12, // فاقد شرایط برداشت از حساب
            13, // شماره حساب راکد
            14, // حساب دارای دستورالعمل ویژه
            15  // حساب مسدود می باشد
        };

        var workFlowStepRejectionReasons3 = rejectionReasonIds3.Select(id => new WorkFlowStepRejectionReason
        {
            RejectionReasonId = id,
            WorkFlowStepId = workFlowStepId3,
            IsActive = true
        }).ToList();

        foreach (var workFlowStepRejectionReason in workFlowStepRejectionReasons3)
        {
            if (!workFlowStepRejectionReasonRepository.TableNoTracking.Any(p => p.RejectionReasonId == workFlowStepRejectionReason.RejectionReasonId && p.WorkFlowStepId == workFlowStepId3))
            {
                workFlowStepRejectionReasonRepository.Add(workFlowStepRejectionReason);
            }
        }

        // برای WorkFlowStepId 100012
        int workFlowStepId4 = 100012;
        var rejectionReasonIds4 = new List<int>
        {
            1,  // مشتری دارای چک برگشتی
            2,  // تسهیلات سررسید گذشته
            3,  // رد کردن سقف مجاز وام
            4,  // غیرمجاز بودن وضعیت نظام وظیفه
            5,  // عدم تایید ثبت احوال
            6,  // مشکل در گزارش اعتبار سنجی بانکی
            7,  // فاقد اعتبارسنجی معتبر
            8,  // پایین بودن نمره اعتبارسنجی بانکی
            9,  // منقضی شدن اعتبارسنجی بانکی
            10, // مشکل در حساب بانک آینده
            11, // فاقد شماره حساب بانک آینده
            12, // فاقد شرایط برداشت از حساب
            13, // شماره حساب راکد
            14, // حساب دارای دستورالعمل ویژه
            15  // حساب مسدود می باشد
        };

        var workFlowStepRejectionReasons4 = rejectionReasonIds4.Select(id => new WorkFlowStepRejectionReason
        {
            RejectionReasonId = id,
            WorkFlowStepId = workFlowStepId4,
            IsActive = true
        }).ToList();

        foreach (var workFlowStepRejectionReason in workFlowStepRejectionReasons4)
        {
            if (!workFlowStepRejectionReasonRepository.TableNoTracking.Any(p => p.RejectionReasonId == workFlowStepRejectionReason.RejectionReasonId && p.WorkFlowStepId == workFlowStepId4))
            {
                workFlowStepRejectionReasonRepository.Add(workFlowStepRejectionReason);
            }
        }

        // برای WorkFlowStepId 100022
        int workFlowStepId5 = 100022;
        var rejectionReasonIds5 = new List<int>
        {
            16, // فرم‌ها یا تضامین فاقد امضای الکترونیک
            17, // مطابقت نداشتن مدارک بارگذاری شده
            18, // عدم تایید فرم‌ها یا تضامین
            19, // نقص مدارک بارگذاری شده
            3,  //رد کردن سقف مجاز وام"
            20, // ناواضح بودن تصویر مدرک
            2,  //تسهیلات سررسید گذشته
            1,  //مشتری دارای چک برگشتی
            21, // عدم تایید سفته
            22, // عدم تایید قراردادها
            23, // عدم تایید مدارک شناسایی
            24, // عدم تایید استعلام شناسنامه
            25, // عدم تایید استعلام کارت ملی
            26, // عدم تایید مدارک شغلی
            27, // به حد نصاب نرسیدن حقوق دریافتی
            28, // منقضی شدن زمان اعتبار مدرک
            29, // عدم اعتبار مهر یا امضا
            30, // عدم بارگذاری سابقه بیمه
            31, // مغایرت/نقص در صحت‌سنجی مدرک
            6,  //مشکل در گزارش اعتبار سنجی بانکی
            7,  //فاقد اعتبارسنجی معتبر
            8,  //پایین بودن نمره اعتبارسنجی بانکی
            9,
            10,
            12,
            13,
            14,
            15
        };

        var workFlowStepRejectionReasons5 = rejectionReasonIds5.Select(id => new WorkFlowStepRejectionReason
        {
            RejectionReasonId = id,
            WorkFlowStepId = workFlowStepId5,
            IsActive = true
        }).ToList();

        foreach (var workFlowStepRejectionReason in workFlowStepRejectionReasons5)
        {
            if (!workFlowStepRejectionReasonRepository.TableNoTracking.Any(p => p.RejectionReasonId == workFlowStepRejectionReason.RejectionReasonId && p.WorkFlowStepId == workFlowStepId5))
            {
                workFlowStepRejectionReasonRepository.Add(workFlowStepRejectionReason);
            }
        }

        // برای WorkFlowStepId 1000241
        int workFlowStepId6 = 1000241;
        var rejectionReasonIds6 = new List<int>
        {
            1,  // مشتری دارای چک برگشتی
            6,  // تسهیلات سررسید گذشته
            5,  // عدم تایید ثبت احوال
            9   // منقضی شدن اعتبارسنجی بانکی
        };

        var workFlowStepRejectionReasons6 = rejectionReasonIds6.Select(id => new WorkFlowStepRejectionReason
        {
            RejectionReasonId = id,
            WorkFlowStepId = workFlowStepId6,
            IsActive = true
        }).ToList();

        foreach (var workFlowStepRejectionReason in workFlowStepRejectionReasons6)
        {
            if (!workFlowStepRejectionReasonRepository.TableNoTracking.Any(p => p.RejectionReasonId == workFlowStepRejectionReason.RejectionReasonId && p.WorkFlowStepId == workFlowStepId6))
            {
                workFlowStepRejectionReasonRepository.Add(workFlowStepRejectionReason);
            }
        }

        // برای WorkFlowStepId 100023
        int workFlowStepId7 = 100023;
        var rejectionReasonIds7 = new List<int>
        {
            1,  // مشتری دارای چک برگشتی
            2,  // تسهیلات سررسید گذشته
            3,  // رد کردن سقف مجاز وام
            4,  // غیرمجاز بودن وضعیت نظام وظیفه
            5,  // عدم تایید ثبت احوال
            6,  // مشکل در گزارش اعتبار سنجی بانکی
            7,  // فاقد اعتبارسنجی معتبر
            8,  // پایین بودن نمره اعتبارسنجی بانکی
            9,  // منقضی شدن اعتبارسنجی بانکی
            10, // مشکل در حساب بانک آینده
            11, // فاقد شماره حساب بانک آینده
            12, // فاقد شرایط برداشت از حساب
            13, // شماره حساب راکد
            14, // حساب دارای دستورالعمل ویژه
            15,  // حساب مسدود می باشد
            16,
            17,
            18,
            19,
            20,
            21,
            22,
            23,
            24,
            25,
            26,
            27,
            28,
            29,
            30,
            31
        };

        var workFlowStepRejectionReasons7 = rejectionReasonIds7.Select(id => new WorkFlowStepRejectionReason
        {
            RejectionReasonId = id,
            WorkFlowStepId = workFlowStepId7,
            IsActive = true
        }).ToList();

        foreach (var workFlowStepRejectionReason in workFlowStepRejectionReasons7)
        {
            if (!workFlowStepRejectionReasonRepository.TableNoTracking.Any(p => p.RejectionReasonId == workFlowStepRejectionReason.RejectionReasonId && p.WorkFlowStepId == workFlowStepId7))
            {
                workFlowStepRejectionReasonRepository.Add(workFlowStepRejectionReason);
            }
        }
        
    }
}
