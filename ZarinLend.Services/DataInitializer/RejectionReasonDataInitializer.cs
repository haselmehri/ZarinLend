using Core.Data.Repositories;
using Core.Entities.Business.RequestFacility;
using Services.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace ZarinLend.Services.DataInitializer;

public class RejectionReasonDataInitializer(IBaseRepository<RejectionReason> rejectionReasonRepository) : IDataInitializer
{
    public int Order => 1;

    public void InitializeData()
    {
        var rejectionReasons = new List<RejectionReason>
        {
            new() { Id = 1, Name = "مشتری دارای چک برگشتی" },
            new() { Id = 2, Name = "تسهیلات سررسید گذشته" },
            new() { Id = 3, Name = "رد کردن سقف مجاز وام" },
            new() { Id = 4, Name = "غیرمجاز بودن وضعیت نظام وظیفه" },
            new() { Id = 5, Name = "عدم تایید ثبت احوال" },
            new() { Id = 6, Name = "مشکل در گزارش اعتبار سنجی بانکی" },
            new() { Id = 7, Name = "فاقد اعتبارسنجی معتبر" },
            new() { Id = 8, Name = "پایین بودن نمره اعتبارسنجی بانکی" },
            new() { Id = 9, Name = "منقضی شدن اعتبارسنجی بانکی" },
            new() { Id = 10, Name = "مشکل در حساب بانک آینده" },
            new() { Id = 11, Name = "فاقد شماره حساب بانک آینده" },
            new() { Id = 12, Name = "فاقد شرایط برداشت از حساب" },
            new() { Id = 13, Name = "شماره حساب راکد" },
            new() { Id = 14, Name = "حساب دارای دستورالعمل ویژه" },
            new() { Id = 15, Name = "حساب مسدود می باشد" },
            new() { Id = 16, Name = "فرم‌ها یا تضامین فاقد امضای الکترونیک" },
            new() { Id = 17, Name = "مطابقت نداشتن مدارک بارگذاری شده" },
            new() { Id = 18, Name = "عدم تایید فرم‌ها یا تضامین" },
            new() { Id = 19, Name = "نقص مدارک بارگذاری شده" },
            new() { Id = 20, Name = "ناواضح بودن تصویر مدرک" },
            new() { Id = 21, Name = "عدم تایید سفته" },
            new() { Id = 22, Name = "عدم تایید قراردادها" },
            new() { Id = 23, Name = "عدم تایید مدارک شناسایی" },
            new() { Id = 24, Name = "عدم تایید استعلام شناسنامه" },
            new() { Id = 25, Name = "عدم تایید استعلام کارت ملی" },
            new() { Id = 26, Name = "عدم تایید مدارک شغلی" },
            new() { Id = 27, Name = "به حد نصاب نرسیدن حقوق دریافتی" },
            new() { Id = 28, Name = "منقضی شدن زمان اعتبار مدرک" },
            new() { Id = 29, Name = "عدم اعتبار مهر یا امضا" },
            new() { Id = 30, Name = "عدم بارگذاری سابقه بیمه" },
            new() { Id = 31, Name = "مغایرت/نقص در صحت‌سنجی مدرک" }
        };

        foreach (var reason in rejectionReasons)
        {
            if (!rejectionReasonRepository.TableNoTracking.Any(p => p.Id == reason.Id))
            {
                rejectionReasonRepository.Add(reason);
            }
        }
    }
}
