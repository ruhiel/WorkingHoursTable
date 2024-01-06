using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkingHoursTable.Models;

namespace WorkingHoursTable.ViewModels
{
    public class MainWindowViewModel
    {
        private const int StartEventID = 7001;
        private const int EndEventID = 7002;

        public class Collect
        {
            public DateTime Base { get; set; }
            public DateTime Start { get; set; }
            public DateTime End { get; set; }
            public List<EventLogEntry> Events { get; set; } = new List<EventLogEntry>();
        }

        public ObservableCollection<DateModel> DateList { get; } = new ObservableCollection<DateModel>();
        public MainWindowViewModel()
        {
            SetTable(DateTime.Now);
        }

        private void SetTable(DateTime dateTime)
        {
            var collectList = new List<Collect>();
            var dt = new DateTime(dateTime.Year, dateTime.Month, 1);
            while (dt.Month == dateTime.Month)
            {
                var collect = new Collect();

                collect.Base = new DateTime(dt.Year, dt.Month, dt.Day);
                collect.Start = new DateTime(dt.Year, dt.Month, dt.Day, 5, 0, 0);
                dt = dt.AddDays(1);
                collect.End = new DateTime(dt.Year, dt.Month, dt.Day, 4, 59, 59);
                collect.Events = new List<EventLogEntry>();
                collectList.Add(collect);
            }

            // 取得するイベントログ名
            string logName = "System";
            // コンピュータ名（"."はローカルコンピュータ）
            string machineName = ".";

            var idList = new List<long>() { StartEventID, EndEventID };

            // 指定したイベントログが存在しているか調べる
            if (EventLog.Exists(logName, machineName))
            {
                // EventLogオブジェクトを作成する
                var log = new EventLog(logName, machineName);

                // ログエントリをすべて取得する
                foreach (var entry in log.Entries.OfType<EventLogEntry>()
                    .Where(x => x.TimeWritten.Year == dateTime.Year && x.TimeWritten.Month == dateTime.Month && idList.Contains(x.InstanceId)))
                {
                    var first = collectList.Where(x => x.Start <= entry.TimeWritten && entry.TimeWritten <= x.End).FirstOrDefault();

                    if (first != null)
                    {
                        first.Events.Add(entry);
                    }
                }

                //閉じる
                log.Close();
            }

            foreach (var collect in collectList)
            {
                var model = new DateModel();

                model.Base = collect.Base;

                model.Start = collect.Events.Where(x => x.InstanceId == StartEventID).MinBy(y => y.TimeWritten)?.TimeWritten;

                model.End = collect.Events.Where(x => x.InstanceId == EndEventID).MaxBy(y => y.TimeWritten)?.TimeWritten;

                DateList.Add(model);
            }
        }
    }
}
