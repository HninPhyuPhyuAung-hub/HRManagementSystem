using HRManagementSystem.Models;
using HRManagementSystem.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace HRManagementSystem.Manager
{
    public class EmployeeManager
    {
        LeaveRepository emprepo = new LeaveRepository();
        public Leave getEmployee(int id)
        {

            Leave result = emprepo.Get(id);
            return result;
        }
        public Leave updateleave(Leave leave)
        {
            Leave updatevalue = null;
            if (leave.LeaveId == 0)
            {
                updatevalue = emprepo.Add(leave);
            }
            else
            {
                updatevalue = emprepo.Update(leave);
            }
            return updatevalue;
        }
        public List<chartmodel> getchart(int EmpId, DateTime? fromdate = null, DateTime? todate = null, string key = null)
        {
            var result = new List<chartmodel>();
            fromdate = (fromdate == null) ? new DateTime(1800, 1, 1) : fromdate;
            todate = (todate == null) ? DateTime.UtcNow : todate;
            key = (key == null) ? key = "M" : key;
            var param1 = new SqlParameter();
            param1.ParameterName = "@F";
            param1.SqlDbType = SqlDbType.DateTime;
            param1.SqlValue = fromdate;

            var param2 = new SqlParameter();
            param2.ParameterName = "@T";
            param2.SqlDbType = SqlDbType.DateTime;
            param2.SqlValue = todate;

            var param3 = new SqlParameter();
            param3.ParameterName = "@K";
            param3.SqlDbType = SqlDbType.VarChar;
            param3.SqlValue = key;

            HumanResourceContext context = new HumanResourceContext();
            var data = context.Database.SqlQuery<chartmodel>("LineChart @F,@T,@K", param1, param2, param3).Where(e => e.EmpID == EmpId).ToList();
            if (key == "Y")
            {
                var diff = todate.Value.Year - fromdate.Value.Year;
                if (diff > 0)
                {
                    int n = 0;
                    var comparedate = todate;
                    while (n <= diff)
                    {

                        chartmodel temp = new chartmodel();

                        int? c = data.Where(e => e.date.Value.Year == comparedate.Value.Year).Select(e => e.count).Sum();
                        temp.count = c ?? 0;
                        temp.date = comparedate;
                        result.Add(temp);
                        comparedate = comparedate.Value.AddYears(-1);
                        n++;
                    };
                }
                else
                {
                    chartmodel temp = new chartmodel();

                    int? c = data.Select(e => e.count).Sum();

                    temp.count = c ?? 0;
                    temp.EmpID = EmpId;
                    temp.date = todate;
                    result.Add(temp);

                }
            }
            else if (key == "M")
            {

                int diff = 12 * (fromdate.Value.Year - todate.Value.Year) + fromdate.Value.Month - todate.Value.Month;
                var diff1 = Math.Abs(diff);

                if (diff1 > 0)
                {
                    int n = 0;
                    var comparedate = todate;

                    while (n <= diff1)
                    {
                        chartmodel temp = new chartmodel();

                        int? c = data.Where(e => e.date.Value.Month == comparedate.Value.Month && e.date.Value.Year == comparedate.Value.Year).Select(e => e.count).Sum();
                        temp.count = c ?? 0;
                        temp.date = comparedate;
                        temp.EmpID = EmpId;
                        result.Add(temp);
                        comparedate = comparedate.Value.AddMonths(-1);
                        n++;
                    };
                }

                else
                {
                    chartmodel temp = new chartmodel();

                    int? c = data.Select(e => e.count).Sum();

                    temp.count = c ?? 0;
                    temp.date = todate;
                    result.Add(temp);

                }
            }
            return result.OrderBy(e => e.date).ToList();
        }
    }
}