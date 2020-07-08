using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ArnelAves_Exam.Controllers
{
    public class ConsumerLoanCalculatorController : Controller
    {
        //
        // GET: /ConsumerLoanCalculator/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult clcinput()
        {
            return View();
        }

        public ActionResult clcresult()
        {
            string _startDate = Request["TxtStartDate"];
            string _loanAmount = Request["TxtLoanAmount"];
            string _interestRate = Request["TxtInterestRate"];
            string _loanTenure = Request["TxtLoanTenure"];

            DateTime StartDate = DateTime.Parse(_startDate);
            double loanAmount = double.Parse(_loanAmount);
            double interestRate = double.Parse(_interestRate)/100;
            double loanTenure = double.Parse(_loanTenure);

            CustomLogic.ConsumerLoanCalculatorLogic consumerLoanCalculatorLogic = new CustomLogic.ConsumerLoanCalculatorLogic();
            List<Models.AmortizationSchedule> amortizationSchedules = consumerLoanCalculatorLogic.ComputeAmortizationSchedule(StartDate, loanAmount, interestRate, loanTenure);


            return View(amortizationSchedules);
        }

        public ActionResult clcdboutput()
        {
            string _startDate = Request["TxtStartDate"];
            string _loanAmount = Request["TxtLoanAmount"];
            string _interestRate = Request["TxtInterestRate"];
            string _loanTenure = Request["TxtLoanTenure"];

            DateTime StartDate = DateTime.Parse(_startDate);
            double loanAmount = double.Parse(_loanAmount);
            double interestRate = double.Parse(_interestRate) / 100;
            double loanTenure = double.Parse(_loanTenure);

            CustomLogic.ConsumerLoanCalculatorLogic consumerLoanCalculatorLogic = new CustomLogic.ConsumerLoanCalculatorLogic();
            List<Models.AmortizationSchedule> amortizationSchedules = consumerLoanCalculatorLogic.ComputeAmortizationScheduleAndSaveToDB(StartDate, loanAmount, interestRate, loanTenure);


            return View(amortizationSchedules);
        }
    }
}
