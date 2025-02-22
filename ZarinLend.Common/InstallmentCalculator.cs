using System;

namespace Common
{
    public class InstallmentCalculator
    {
        //public const int INTEREST_RATE = 23;
        public const int INSTALLMENT_COUNT_IN_YEAR = 12;
        //public const int OPERATING_COST_PERCENTAGE = 15;
        //public const int CHEQUE_AMOUNT_WARRANTY_PERCENTAGE = 120;
        public static double CalculateAmountInstallment(long amountRequest, long monthCount, double interestRate)
        {
            if (amountRequest == 0 || monthCount == 0)
                return 0;

            var result =
                amountRequest * interestRate / (INSTALLMENT_COUNT_IN_YEAR * 100.0) * Math.Pow(1 + (interestRate / (INSTALLMENT_COUNT_IN_YEAR * 100.0)), monthCount) /
                (Math.Pow(1 + (interestRate / (INSTALLMENT_COUNT_IN_YEAR * 100.0)), monthCount) - 1);

            return result;
        }

        public static double CalculatePrePaymentOrFee(long amountRequest, long paymentPeriodBaseMonth, double operatingCostPercentage)
        {
            if (amountRequest == 0 ||
                paymentPeriodBaseMonth == 0)
                return 0;

            //var feePercentage = 0.0;
            //switch (paymentPeriodBaseMonth)
            //{
            //    case 6:
            //        feePercentage = 5.5;
            //        break;
            //    case 12:
            //        feePercentage = 8;
            //        break;
            //    case 18:
            //        feePercentage = 10.5;
            //        break;
            //    case 24:
            //        feePercentage = 12.6;
            //        break;
            //}
            //if (feePercentage == 0)
            //    return 0;

            //double result = (amountRequest * feePercentage / 100.0) + (amountRequest * feePercentage / 100.0 * 9 / 100.0) +
            //    ((CalculateTotalInstallment(amountRequest,paymentPeriodBaseMonth) - amountRequest) * 9 / 100.0);

            double result = amountRequest * operatingCostPercentage / 100.0;
            return result;
        }

        public static double CalculateTotalInstallment(long amountRequest, long monthCount, double interestRate)
        {
            return CalculateAmountInstallment(amountRequest, monthCount, interestRate) * monthCount;
        }

        public static double CalculateTotalPayment(long amountRequest, int paymentPeriodBaseMonth, double interestRate, double operatingCostPercentage)
        {
            var totalInstallment = CalculateTotalInstallment(amountRequest, paymentPeriodBaseMonth, interestRate);
            return totalInstallment + CalculatePrePaymentOrFee(amountRequest, paymentPeriodBaseMonth, operatingCostPercentage);
        }

        public static double CalculateChequeAmountWarranty(long amountRequest, int paymentPeriodBaseMonth, double interestRate, double WarantyPercentage)
        {
            return RoundUp(CalculateTotalInstallment(amountRequest, paymentPeriodBaseMonth, interestRate) * WarantyPercentage / 100.0, 6);
        }

        public static double RoundUp(double number, int digits)
        {
            var baseDigit = Math.Pow(10, digits);

            return Math.Ceiling(number / baseDigit) * baseDigit;
        }
    }
}
