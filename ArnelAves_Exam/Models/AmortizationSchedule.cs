using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArnelAves_Exam.Models
{
    public class AmortizationSchedule
    {
        public int ScheduleOrderNumber { get; set; }
        public DateTime DueDate { get; set; }
        public double PrincipalAmount { get; set; }
        public double InterestRate { get; set; }
        public double InterestAmount { get; set; }
        public double DueAmount { get; set; }
        public double RemainingBalance { get; set; }
        public int DaysInterval { get; set; }
    }
}