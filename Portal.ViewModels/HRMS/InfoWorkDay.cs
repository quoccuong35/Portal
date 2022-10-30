using Portal.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.ViewModels
{
    public class InfoWorkDay
    {
        public IEnumerable<RALog> raLogs { get; set; }
        public IEnumerable<ConfirmWorkingTimeModel> conFirms { get; set; }
        public IEnumerable<OvertimeModel> oTs { get; set; }
        public IEnumerable<FurloughModel> furloughs { get; set; }
        public IEnumerable<WorkdayEditModel> workDayEdit { get; set; }
    }
}
