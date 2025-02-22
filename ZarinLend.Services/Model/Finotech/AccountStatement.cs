using Common.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ZarinLend.Common.LocalizationResource;

namespace Services.Model.AccountStatement
{
    [Serializable]
    public class AccountStatementInput
    {
        /// <summary>
        /// account number
        /// </summary>
        [Display(Name = "AccountNumber", ResourceType = typeof(ResourceFile))]
        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [RegularExpression(RegularExpression.AccountNumber, ErrorMessage = "شمار حساب را با فرمت صحیح وارد کنید")]
        public string deposit { get; set; }

        [Display(Name = "FromDate", ResourceType = typeof(ResourceFile))]
        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        public string fromDate { get; set; }

        [Display(Name = "ToDate", ResourceType = typeof(ResourceFile))]
        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        public string toDate { get; set; }
        public string fromTime { get; set; }
        public string toTime { get; set; }
        public string Json { get; set; }
        public AccountStatementModel AccountStatementModel { get; set; }
    }

    [Serializable]
    public class AccountStatementModel
    {
        public string TrackId { get; set; }
        public string Status { get; set; }
        public string Error { get; set; }
        public AccountStatementResultModel Result { get; set; }
    }

    [Serializable]
    public class AccountStatementResultModel
    {
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string AccountTypeCode { get; set; }
        public string SubTypeCode { get; set; }
        public string Iban { get; set; }
        public string CustomerName { get; set; }
        public string CustomerFamilyName { get; set; }
        public string BackupAccountNumber { get; set; }
        public string AccountCurrentBalance { get; set; }

        /// <summary>
        /// C: credit
        /// D: debit
        /// </summary>
        public string AccountCurrentBalanceSign { get; set; }
        public string AccountAvailableBalance { get; set; }

        public long AccountAvailableBalanceLong
        {
            get
            {
                long value;
                if (long.TryParse(AccountAvailableBalance, out value))
                    return value;

                return 0;
            }
        }

        /// <summary>
        /// C: credit
        /// D: debit
        /// </summary>
        public string AccountAvailableBalanceSign { get; set; }
        public string EffectiveAccountBalance { get; set; }
        public long EffectiveAccountBalanceLong
        {
            get
            {
                long value;
                if (long.TryParse(EffectiveAccountBalance, out value))
                    return value;

                return 0;
            }
        }
        public string EffectiveAccountBalanceSign { get; set; }

        /// <summary>
        /// M: وجود تراکنش بیشتر از 100
        /// L: عدم وجود تراکنش بیشتر
        /// </summary>
        public string TransactionChain { get; set; }

        /// <summary>
        /// M: وجود تراکنش بیشتر از 100
        /// L: عدم وجود تراکنش بیشتر
        /// </summary>
        public string OpenDate { get; set; }
        public List<AccountTransactionModel> Transactions { get; set; }
        public string AvailableSign { get; set; }
        public string AverageBalance { get; set; }
        public string AverageBalanceSign { get; set; }
        public string CurrentBalance { get; set; }
        public string CurrentBalanceSign { get; set; }
    }

    [Serializable]
    public class AccountTransactionModel
    {
        public string RecordNumber { get; set; }
        public string Balance { get; set; }
        public long BalanceLong
        {
            get
            {
                long value;
                if (long.TryParse(Balance, out value))
                    return value;

                return 0;
            }
        }
        public string TransactionAmountCredit { get; set; }
        public long TransactionAmountCreditLong
        {
            get
            {
                long value;
                if (long.TryParse(TransactionAmountCredit, out value))
                    return value;

                return 0;
            }
        }
        public string TransactionAmountDebit { get; set; }
        public long TransactionAmountDebitLong
        {
            get
            {
                long value;
                if (long.TryParse(TransactionAmountDebit, out value))
                    return value;

                return 0;
            }
        }
        public string BranchNo { get; set; }

        /// <summary>
        ///  تاریخ که باید به صورت YYMMDD
        /// </summary>
        public string Date { get; set; }

        /// <summary>
        /// باید به صورت HHMMSS
        /// </summary>
        public string Time { get; set; }
        public OperationCodeModel OperationCode { get; set; }
        public string BalanceSign { get; set; }
        public string OptionalInformation30 { get; set; }
        public string OptionalInformation15 { get; set; }
        public string OptionalInformation1 { get; set; }
        public string OptionalInformation { get; set; }
        public string FullDate { get; set; }
        public string RequestDate { get; set; }
        public string RequestTime { get; set; }
        public string OriginKey { get; set; }
        public string RawOriginKey { get; set; }
        public string AdditionalInformation { get; set; }
        public string RefCode { get; set; }
        public string TransactionAmount { get; set; }
        public string TransactionClass { get; set; }
        public IbanInformationMapModel IbanInformationMap { get; set; }
        public CardInformationMapModel CardInformationMap { get; set; }
        public LedgerInformationMapModel LedgerInformationMap { get; set; }

    }

    [Serializable]
    public class OperationCodeModel
    {
        public string Code { get; set; }
        public string Message { get; set; }
    }

    [Serializable]
    public class IbanInformationMapModel
    {
        public string BranchCode { get; set; }
        public string BankCode { get; set; }
        public string TransactionClass { get; set; }
        public string Name { get; set; }
        public string DestinationIban { get; set; }
        public string PayId { get; set; }
        public string ChakavakInfo { get; set; }
        /// <summary>
        /// satna: 62
        /// atm: 63
        /// pin pad: 03
        /// pos: 14
        /// telebank: 07
        /// internet: 59
        /// mobile: 05
        /// info kiosk: 13
        /// info kiosk: 43
        /// ledger: 02
        /// </summary>
        public string TransactionTypeCode { get; set; }
        public string ReferenceId { get; set; }
    }

    [Serializable]
    public class CardInformationMapModel
    {
        public string BranchCode { get; set; }
        public string BankCode { get; set; }
        public string TransactionClass { get; set; }
        public string ReferenceNo { get; set; }
        public string DestinationPan { get; set; }
        public string SourcePan { get; set; }
        public string AcquiringNo { get; set; }
        /// <summary>
        /// satna: 62
        /// atm: 63
        /// pin pad: 03
        /// pos: 14
        /// telebank: 07
        /// internet: 59
        /// mobile: 05
        /// info kiosk: 13
        /// info kiosk: 43
        /// ledger: 02
        /// </summary>
        public string TransactionTypeCode { get; set; }
        public string DeviceSerial { get; set; }
        public string Trace { get; set; }
        public string BillId { get; set; }
        public string Name { get; set; }
        public string DestinationIban { get; set; }
        public string PayId { get; set; }
    }

    [Serializable]
    public class LedgerInformationMapModel
    {
        public string TransactionClass { get; set; }

        /// <summary>
        /// satna: 62
        /// atm: 63
        /// pin pad: 03
        /// pos: 14
        /// telebank: 07
        /// internet: 59
        /// mobile: 05
        /// info kiosk: 13
        /// info kiosk: 43
        /// ledger: 02
        /// </summary>
        public string TransactionTypeCode { get; set; }
        public string PayId { get; set; }
        public string PermissionId { get; set; }
        public string ReverseIndex { get; set; }
        public string ReverseOriginKey { get; set; }
        public string ReasonDescription { get; set; }
    }
}
