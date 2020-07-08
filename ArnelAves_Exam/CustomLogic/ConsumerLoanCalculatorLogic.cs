using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArnelAves_Exam.CustomLogic
{
    public class ConsumerLoanCalculatorLogic
    {
        private const int DueDayOfMonth = 25;
        private const int DaysInYear = 365;

        public List<Models.AmortizationSchedule> ComputeAmortizationSchedule(DateTime _startDate, double _loanAmount, double _interestRate, double _loanTenure)
        {
            List<Models.AmortizationSchedule> retval = new List<Models.AmortizationSchedule>();

            DateTime startDate = _startDate;
            double loanAmount = _loanAmount;
            double interestRate = _interestRate;
            double loanTenure = _loanTenure;

            DateTime previousDate = startDate;
            double principalAmount = loanAmount / loanTenure;
            double runningLoanAmount = _loanAmount;

            for (int i = 1; i <= loanTenure; i++ )
            {
                Models.AmortizationSchedule amortizationSchedule = new Models.AmortizationSchedule();
                amortizationSchedule.ScheduleOrderNumber = i;
                amortizationSchedule.PrincipalAmount = principalAmount;
                DateTime runningDate = GetNextDate(previousDate);
                amortizationSchedule.DueDate = runningDate;
                int runningDaysInterval = runningDate.Subtract(previousDate).Days;
                amortizationSchedule.DaysInterval = runningDaysInterval;
                double runningInterestAmount = runningLoanAmount * runningDaysInterval * interestRate / DaysInYear;
                amortizationSchedule.InterestAmount = runningInterestAmount;
                amortizationSchedule.DueAmount = principalAmount + runningInterestAmount;
                runningLoanAmount -= principalAmount;
                amortizationSchedule.RemainingBalance = runningLoanAmount;
                amortizationSchedule.InterestRate = interestRate;
                previousDate = runningDate;

                retval.Add(amortizationSchedule);
            }

            return retval;
        }

        private DateTime GetNextDate(DateTime _baseDate)
        {
            DateTime retval = _baseDate;

            int _month = retval.Month;
            int _year = retval.Year;
            int _day = retval.Day;

            if (_day >= DueDayOfMonth)
            { 
                if (_month >= 12)
                {
                    _year += 1;
                    _month = 1;
                }
                else
                {
                    _month += 1;
                }
            }
            // build the date
            retval = new DateTime(_year, _month, DueDayOfMonth);

            return retval;
        }
        
        public List<Models.AmortizationSchedule> ComputeAmortizationScheduleAndSaveToDB(DateTime _startDate, double _loanAmount, double _interestRate, double _loanTenure)
        {
            List<Models.AmortizationSchedule> retval = new List<Models.AmortizationSchedule>();

            DateTime startDate = _startDate;
            double loanAmount = _loanAmount;
            double interestRate = _interestRate;
            double loanTenure = _loanTenure;

            int sessionID = SaveSessionToDB(startDate, loanAmount, interestRate, loanTenure);

            DateTime previousDate = startDate;
            double principalAmount = loanAmount / loanTenure;
            double runningLoanAmount = _loanAmount;

            for (int i = 1; i <= loanTenure; i++)
            {
                Models.AmortizationSchedule amortizationSchedule = new Models.AmortizationSchedule();
                amortizationSchedule.ScheduleOrderNumber = i;
                amortizationSchedule.PrincipalAmount = principalAmount;
                DateTime runningDate = GetNextDate(previousDate);
                amortizationSchedule.DueDate = runningDate;
                int runningDaysInterval = runningDate.Subtract(previousDate).Days;
                amortizationSchedule.DaysInterval = runningDaysInterval;
                double runningInterestAmount = runningLoanAmount * runningDaysInterval * interestRate / DaysInYear;
                amortizationSchedule.InterestAmount = runningInterestAmount;
                amortizationSchedule.DueAmount = principalAmount + runningInterestAmount;
                runningLoanAmount -= principalAmount;
                amortizationSchedule.RemainingBalance = runningLoanAmount;
                amortizationSchedule.InterestRate = interestRate;
                previousDate = runningDate;

                //retval.Add(amortizationSchedule);
                SaveAmortizationToDB(amortizationSchedule, sessionID);
            }

            // retrieve list from db

            ArnelAvesExamEntities db = new ArnelAvesExamEntities();

            var empQuery = from emp in db.AmortizationSchedules
                           where emp.SessionNumber == sessionID
                           orderby emp.ScheduleOrderNumber
                           select emp;

            List<AmortizationSchedule> dblist = empQuery.ToList();
            foreach (var obj in dblist)
            {
                AmortizationSchedule asched = (AmortizationSchedule)obj;
                Models.AmortizationSchedule amortizationSchedule = new Models.AmortizationSchedule();

                amortizationSchedule.ScheduleOrderNumber = (int)asched.ScheduleOrderNumber;
                amortizationSchedule.PrincipalAmount = (double)asched.PrincipalAmount;
                amortizationSchedule.DueDate = (DateTime)asched.DueDate;
                amortizationSchedule.DaysInterval = (int)asched.DaysInterval;
                amortizationSchedule.InterestAmount = (double)asched.InterestAmount;
                amortizationSchedule.DueAmount = (double)asched.DueAmount;
                amortizationSchedule.RemainingBalance = (double)asched.RemainingBalance;
                amortizationSchedule.InterestRate = (double)asched.InterestRate;

                retval.Add(amortizationSchedule);
            }

            return retval;
        }

        public int SaveAmortizationToDB(Models.AmortizationSchedule _amortizationSchedule, int _consumerLoanSessionID)
        {
            int retval = 0;

            Models.AmortizationSchedule amortizationSchedule = _amortizationSchedule;
            
            ArnelAvesExamEntities db = new ArnelAvesExamEntities();
            AmortizationSchedule asched = new AmortizationSchedule();
            asched.SessionNumber = _consumerLoanSessionID;
            asched.ScheduleOrderNumber = amortizationSchedule.ScheduleOrderNumber;
            asched.DueDate = amortizationSchedule.DueDate;
            asched.PrincipalAmount = amortizationSchedule.PrincipalAmount;
            asched.InterestRate = amortizationSchedule.InterestRate;
            asched.InterestAmount = amortizationSchedule.InterestAmount;
            asched.DueAmount = amortizationSchedule.DueAmount;
            asched.RemainingBalance = amortizationSchedule.RemainingBalance;
            asched.DaysInterval = amortizationSchedule.DaysInterval;

            db.AmortizationSchedules.Add(asched);
            db.SaveChanges();

            retval = asched.AmortizationID;
            return retval;
        }

        public int SaveSessionToDB(DateTime _startDate, double _loanAmount, double _interestRate, double _loanTenure)
        {
            int retval = 0;

            ArnelAvesExamEntities db = new ArnelAvesExamEntities();
            
            ConsumerLoanSsession clSession = new ConsumerLoanSsession();
            clSession.StartDate = _startDate;
            clSession.LoanAmount = _loanAmount;
            clSession.LoanInterest = _interestRate;
            clSession.LoanTenure = _loanTenure;

            db.ConsumerLoanSsessions.Add(clSession);
            db.SaveChanges();

            retval = clSession.SessionNumber;

            return retval;
        }
        
    }
}