using Common;
using Core.Data.Repositories;
using Core.Entities;
using Core.Entities.Business.Payment;
using Core.Entities.Business.RequestFacility;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using Services.Dto;
using Services.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    public class RequestFacilityInstallmentService : IRequestFacilityInstallmentService, IScopedDependency
    {
        private readonly ILogger<RequestFacilityInstallmentService> logger;
        private readonly IBaseRepository<RequestFacilityInstallment> requestFacilityInstallmentRepository;
        private readonly IBaseRepository<SamanInternetPayment> samanInternetPaymentRepository;

        public RequestFacilityInstallmentService(ILogger<RequestFacilityInstallmentService> logger,
                                                 IBaseRepository<RequestFacilityInstallment> requestFacilityInstallmentRepository,
                                                 IBaseRepository<SamanInternetPayment> samanInternetPaymentRepository)
        {
            this.logger = logger;
            this.requestFacilityInstallmentRepository = requestFacilityInstallmentRepository;
            this.samanInternetPaymentRepository = samanInternetPaymentRepository;
        }

        public async Task<List<RequestFacilityInstallmentModel>> SelectInstallment(Guid userId, int requestFacilityId, CancellationToken cancellationToken)
        {
            var query = requestFacilityInstallmentRepository.TableNoTracking.Where(p => p.RequestFacilityId.Equals(requestFacilityId) &&
                p.RequestFacility.BuyerId.Equals(userId) &&
                p.RequestFacility.RequestFacilityWorkFlowSteps
                    .Any(x => x.StatusId == (short)StatusEnum.Approved &&
                              x.WorkFlowStep.IsApproveFinalStep));

            DateTime today = DateTime.Now.Date;
            var installmentList = await query
                .Select(p => new RequestFacilityInstallmentModel
                {
                    Id = p.Id,
                    Amount = p.Amount,
                    PenaltyDays = p.Paid
                        ? p.PenaltyDays!.Value
                        : today > p.DueDate
                            ? (int)Math.Ceiling((today - p.DueDate).TotalDays)
                            : 0,
                    PenaltyAmount = p.Paid
                        ? p.PenaltyAmount!.Value
                        : today > p.DueDate
                            ? (p.Amount * ((int)p.RequestFacility.GlobalSetting.FacilityInterest + 6) * (int)Math.Ceiling((today - p.DueDate).TotalDays)) / 36500
                            : 0,
                    DueDate = p.DueDate,
                    RealPayAmount = p.Paid
                        ? p.RealPayAmount!.Value
                        : today > p.DueDate
                            ? p.Amount + (p.Amount * ((int)p.RequestFacility.GlobalSetting.FacilityInterest + 6) * (int)Math.Ceiling((today - p.DueDate).TotalDays)) / 36500
                            : p.Amount,
                    RealPayDate = p.RealPayDate,
                    Paid = p.Paid,
                }).ToListAsync(cancellationToken);

            return installmentList;
        }

        public async Task<RequestFacilityInstallmentModel> GetById(int id, CancellationToken cancellationToken)
        {
            DateTime today = DateTime.Now.Date;
            return await requestFacilityInstallmentRepository.TableNoTracking.Where(p => p.Id.Equals(id))
                 .Select(p => new RequestFacilityInstallmentModel
                 {
                     Id = p.Id,
                     RequestFacilityId = p.RequestFacilityId,
                     Amount = p.Amount,
                     PenaltyDays = p.Paid
                         ? p.PenaltyDays.Value
                     : today > p.DueDate
                             ? (int)Math.Ceiling((today - p.DueDate).TotalDays)
                             : 0,
                     PenaltyAmount = p.Paid
                         ? p.PenaltyAmount.Value
                         : today > p.DueDate
                     ? (p.Amount * ((int)p.RequestFacility.GlobalSetting.FacilityInterest + 6) * (int)Math.Ceiling((today - p.DueDate).TotalDays)) / 36500
                     : 0,
                     DueDate = p.DueDate,
                     RealPayAmount = p.Paid
                     ? p.RealPayAmount.Value
                     : today > p.DueDate
                     ? p.Amount + (p.Amount * ((int)p.RequestFacility.GlobalSetting.FacilityInterest + 6) * (int)Math.Ceiling((today - p.DueDate).TotalDays)) / 36500
                     : p.Amount,
                     RealPayDate = p.RealPayDate,
                     Paid = p.Paid,
                 }).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<RequestFacilityInstallmentModel> PrepareForPayment(int id, CancellationToken cancellationToken)
        {
            var model = await requestFacilityInstallmentRepository.TableNoTracking.Where(p => p.Id.Equals(id))
                        .Select(p => new RequestFacilityInstallmentModel
                        {
                            Id = p.Id,
                            RequestFacilityId = p.RequestFacilityId,
                            Amount = p.Amount,
                            PenaltyDays = p.Paid
                               ? p.PenaltyDays.Value
                               : p.StartForPayment.Value.Date > p.DueDate
                                   ? (int)Math.Ceiling((p.StartForPayment.Value.Date - p.DueDate).TotalDays)
                                   : 0,
                            PenaltyAmount = p.Paid
                               ? p.PenaltyAmount.Value
                               : p.StartForPayment.Value.Date > p.DueDate
                                   ? (p.Amount * ((int)p.RequestFacility.GlobalSetting.FacilityInterest + 6) * (int)Math.Ceiling((p.StartForPayment.Value.Date - p.DueDate).TotalDays)) / 36500
                                   : 0,
                            DueDate = p.DueDate,
                            RealPayAmount = p.Paid
                               ? p.RealPayAmount.Value
                               : p.StartForPayment.Value.Date > p.DueDate
                                   ? p.Amount + (p.Amount * ((int)p.RequestFacility.GlobalSetting.FacilityInterest + 6) * (int)Math.Ceiling((p.StartForPayment.Value.Date - p.DueDate).TotalDays)) / 36500
                                   : p.Amount,
                            RealPayDate = p.RealPayDate,
                            Paid = p.Paid,
                        }).FirstOrDefaultAsync(cancellationToken);

            if (model != null)
                await requestFacilityInstallmentRepository.UpdateCustomPropertiesAsync(new RequestFacilityInstallment() { Id = id, StartForPayment = DateTime.Now.Date },
                    cancellationToken, true,
                    nameof(RequestFacilityInstallment.StartForPayment));

            return model;
        }

        public async Task<RequestFacilityInstallmentModel> CheckExistUnpaidInstallmentBeforeThis(Guid userId, int id, CancellationToken cancellationToken)
        {
            DateTime today = DateTime.Now.Date;
            var thisInstallment = await requestFacilityInstallmentRepository.TableNoTracking
                .Where(x => x.Id.Equals(id) && x.RequestFacility.BuyerId.Equals(userId))
                .Select(p => new { p.DueDate, p.RequestFacilityId })
                .FirstOrDefaultAsync();

            var beforeInstallmentNotPay = await requestFacilityInstallmentRepository.TableNoTracking
                         .Where(p => !p.Paid && p.DueDate < thisInstallment.DueDate && p.RequestFacilityId.Equals(thisInstallment.RequestFacilityId) && !p.Id.Equals(id))
                         .FirstOrDefaultAsync(cancellationToken);

            if (beforeInstallmentNotPay != default)
                return await requestFacilityInstallmentRepository.TableNoTracking
                        .Where(p => !p.Paid && p.DueDate < thisInstallment.DueDate && p.RequestFacilityId.Equals(thisInstallment.RequestFacilityId) && !p.Id.Equals(id))
                        .Select(p => new RequestFacilityInstallmentModel
                        {
                            Id = beforeInstallmentNotPay.Id,
                            RequestFacilityId = beforeInstallmentNotPay.RequestFacilityId,
                            Amount = beforeInstallmentNotPay.Amount,
                            PenaltyDays = today > beforeInstallmentNotPay.DueDate
                             ? (int)Math.Ceiling((today - beforeInstallmentNotPay.DueDate).TotalDays)
                             : 0,
                            PenaltyAmount = today > beforeInstallmentNotPay.DueDate
                             ? (beforeInstallmentNotPay.Amount * ((int)p.RequestFacility.GlobalSetting.FacilityInterest + 6) * (int)Math.Ceiling((today - beforeInstallmentNotPay.DueDate).TotalDays)) / 36500
                             : 0,
                            DueDate = beforeInstallmentNotPay.DueDate,
                            RealPayAmount = today > beforeInstallmentNotPay.DueDate
                             ? beforeInstallmentNotPay.Amount + (beforeInstallmentNotPay.Amount * ((int)p.RequestFacility.GlobalSetting.FacilityInterest + 6) * (int)Math.Ceiling((today - beforeInstallmentNotPay.DueDate).TotalDays)) / 36500
                             : beforeInstallmentNotPay.Amount,
                            RealPayDate = beforeInstallmentNotPay.RealPayDate,
                            Paid = beforeInstallmentNotPay.Paid,
                        }).FirstOrDefaultAsync(cancellationToken);

            return null;
        }

        public async Task<PagingDto<RequestFacilityInstallmentModel>> Search(PagingFilterDto filter, bool executeForExport, CancellationToken cancellationToken)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            var query = requestFacilityInstallmentRepository.TableNoTracking
                .Include(p => p.RequestFacility)
                    .ThenInclude(p => p.FacilityType)
                .Include(p => p.RequestFacility)
                    .ThenInclude(p => p.GlobalSetting)
                .Include(p => p.RequestFacility)
                    .ThenInclude(p => p.Buyer.Person)
                    .Include(p => p.PaymentReasons.Where(p => p.Payment.IsSuccess == true))
                //.Include(p => p.Payments.Where(p => p.IsSuccess == true && p.IpgType == IpgType.SamanIPG)
                //                        .GroupJoin(samanInternetPaymentRepository.TableNoTracking,
                //                                   payment => payment.Id,
                //                                   samanIpg => samanIpg.Id,
                //                                   (payment,samanIpg) => new { Payment = payment,  SamanIpg = samanIpg})
                //                        .SelectMany(x=>x.SamanIpg.DefaultIfEmpty(),
                //                                    (x,y)=>new JoinModel { Payment = x.Payment, SamanInternetPayment  = y})).Select((x,v)=> v)
                //.GroupJoin(samanInternetPaymentRepository.TableNoTracking,
                //      rfi => rfi.py,
                //      samanPayment => samanPayment.Id,
                //      (rfi, samanPayment) => new { RequestFacilityInstallment = rfi, SamanInternetPayment = samanPayment })
                .AsSplitQuery();

            ApplyFilter(ref query, filter);
            DateTime today = DateTime.Now.Date;

            var paginationQuery = executeForExport
                          ? (await query
                              .OrderBy(p => p.Paid)
                              .ToListAsync(cancellationToken)
                              .ConfigureAwait(false))
                          : (await query
                              .OrderBy(p => p.Paid)
                              .Skip((filter.Page - 1) * filter.PageSize)
                              .Take(filter.PageSize)
                              .ToListAsync(cancellationToken)
                              .ConfigureAwait(false));

            var filterList = paginationQuery
                .Select(p => new RequestFacilityInstallmentModel
                {
                    Id = p.Id,
                    Amount = p.Amount,
                    PenaltyDays = p.Paid
                        ? p.PenaltyDays.Value
                        : today > p.DueDate
                            ? (int)Math.Ceiling((today - p.DueDate).TotalDays)
                            : 0,
                    PenaltyAmount = p.Paid
                        ? p.PenaltyAmount.Value
                        : today > p.DueDate
                            ? (p.Amount * ((int)p.RequestFacility.GlobalSetting.FacilityInterest + 6) * (int)Math.Ceiling((today - p.DueDate).TotalDays)) / 36500
                            : 0,
                    DueDate = p.DueDate,
                    RealPayAmount = p.Paid
                        ? p.RealPayAmount.Value
                        : today > p.DueDate
                            ? p.Amount + (p.Amount * ((int)p.RequestFacility.GlobalSetting.FacilityInterest + 6) * (int)Math.Ceiling((today - p.DueDate).TotalDays)) / 36500
                            : p.Amount,
                    RealPayDate = p.RealPayDate,
                    Paid = p.Paid,
                    FacilityAmount = p.RequestFacility.Amount,
                    MonthCountTitle = p.RequestFacility.FacilityType.MonthCountTitle,
                    Requester = $"{p.RequestFacility.Buyer.Person.FName} {p.RequestFacility.Buyer.Person.LName}",
                    NationalCode = p.RequestFacility.Buyer.Person.NationalCode,
                    PaymentResult = p.PaymentReasons.Any(p => p.Payment.IsSuccess == true)
                            ? p.PaymentReasons.Where(p => p.Payment.IsSuccess == true)
                                                 .Select(x => new SamanIntenetBankPaymentExportModel()
                                                 {
                                                     Amount = x.Payment.Amount,
                                                     CreateDate = x.CreatedDate,
                                                     IsSuccess = x.Payment.IsSuccess,
                                                     FinancialInstitutionFacilityFee = p.RequestFacility.GlobalSetting.FinancialInstitutionFacilityFee,
                                                     LendTechFacilityFee = p.RequestFacility.GlobalSetting.LendTechFacilityFee,
                                                     //MaskedPan = x.Payment.TransactionDetail_MaskedPan,
                                                     //ResNum = x.Payment.ResNum,
                                                     //RefNum = x.Payment.RefNum,
                                                     //RRN = x.Payment.RRN,
                                                     //StraceDate = x.Payment.TransactionDetail_StraceDate,
                                                     UpdateDate = x.UpdateDate,
                                                     Id = x.Id,
                                                 }).FirstOrDefault()
                            : null,
                }).ToList();

            if (!executeForExport)
            {
                var totalRowCounts = await query.CountAsync();

                return new PagingDto<RequestFacilityInstallmentModel>()
                {
                    CurrentPage = filter.Page,
                    Data = filterList,
                    TotalRowCount = totalRowCounts,
                    TotalPages = (int)Math.Ceiling(Convert.ToDouble(totalRowCounts) / filter.PageSize)
                };
            }
            else
            {
                return new PagingDto<RequestFacilityInstallmentModel>()
                {
                    Data = filterList
                };
            }
        }

        private IQueryable<RequestFacilityInstallment> ApplyFilter(ref IQueryable<RequestFacilityInstallment> query, PagingFilterDto filter)
        {
            if (filter != null && filter.FilterList != null)
            {
                bool? paidValue = null;
                foreach (var item in filter.FilterList)
                {
                    switch (item.PropertyName)
                    {
                        case "NationalCode":
                            {
                                string propertyValue = item.PropertyValue;
                                query = query.Where(p => p.RequestFacility.Buyer.Person.NationalCode.Contains(propertyValue));
                                break;
                            }
                        case "StartDueDate":
                            {
                                DateTime propertyValue = Convert.ToDateTime(item.PropertyValue);
                                query = query.Where(p => p.DueDate.Date >= propertyValue.Date);
                                break;
                            }
                        case "EndDueDate":
                            {
                                DateTime propertyValue = Convert.ToDateTime(item.PropertyValue);
                                query = query.Where(p => p.DueDate.Date <= propertyValue.Date);
                                break;
                            }
                        case "StartRealPaymentDate":
                            {
                                paidValue = true;
                                DateTime propertyValue = Convert.ToDateTime(item.PropertyValue);
                                query = query.Where(p => p.RealPayDate.HasValue && p.RealPayDate.Value.Date >= propertyValue.Date);
                                break;
                            }
                        case "EndRealPaymentDate":
                            {
                                paidValue = true;
                                DateTime propertyValue = Convert.ToDateTime(item.PropertyValue);
                                query = query.Where(p => p.RealPayDate.HasValue && p.RealPayDate.Value.Date <= propertyValue.Date);
                                break;
                            }
                        case "PaidStatus":
                            {
                                long propertyValue = item.PropertyValue;
                                switch (propertyValue)
                                {
                                    case (long)PaidStatus.PaidAndNotPenalty:
                                        {
                                            paidValue = true;

                                            query = query.Where(p => p.DueDate >= p.RealPayDate.Value.Date);
                                            break;
                                        }
                                    case (long)PaidStatus.PaidAndPenalty:
                                        {
                                            paidValue = true;
                                            query = query.Where(p => p.RealPayDate.HasValue &&
                                                                     p.DueDate < p.RealPayDate.Value.Date &&
                                                                     p.PenaltyAmount.HasValue &&
                                                                     p.PenaltyAmount > 0 &&
                                                                     p.PenaltyDays.HasValue &&
                                                                     p.PenaltyDays > 0);
                                        }
                                        break;
                                    case (long)PaidStatus.NotPayAndPenalty:
                                        {
                                            paidValue = false;
                                            var today = DateTime.Now.Date;
                                            query = query.Where(p => p.DueDate.Date < today);
                                            break;
                                        }
                                    case (long)PaidStatus.NotPayAndNotPenalty:
                                        {
                                            paidValue = false;
                                            var today = DateTime.Now.Date;
                                            query = query.Where(p => p.DueDate.Date >= today);
                                            break;
                                        }
                                }
                                break;
                            }
                        default:
                            break;
                    }
                }

                if (paidValue.HasValue)
                    query = query.Where(p => p.Paid.Equals(paidValue.Value));
            }

            return query;
        }
    }
}
