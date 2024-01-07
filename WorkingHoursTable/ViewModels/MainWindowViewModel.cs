using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using WorkingHoursTable.Models;
using WorkingHoursTable.Util;
using MahApps.Metro.Controls.Dialogs;
using System.Diagnostics.Metrics;
using System.Windows;
using System.Reactive.Linq;

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

        public ObservableCollection<int> YearList { get; } = new ObservableCollection<int>();

        public ReactiveProperty<int> SelectedYear { get; } = new ReactiveProperty<int>();
        public ObservableCollection<int> MonthList { get; } = new ObservableCollection<int>();

        public ReactiveProperty<int> SelectedMonth { get; } = new ReactiveProperty<int>();
        public ReactiveProperty<Visibility> TableVisibility { get; } = new ReactiveProperty<Visibility>();
        public ReactiveProperty<Visibility> NowProgress { get; } = new ReactiveProperty<Visibility>();
        public ReactiveCommand PrevMonthCommand { get; } = new ReactiveCommand();
        public ReactiveCommand NextMonthCommand { get; } = new ReactiveCommand();
        public MainWindowViewModel()
        {
            YearList.AddAll(Enumerable.Range(2000, DateTime.Now.Year - 2000 + 1));
            SelectedYear.Value = DateTime.Now.Year;
            MonthList.AddAll(Enumerable.Range(1, 12));
            SelectedMonth.Value = DateTime.Now.Month;

            NowProgress = TableVisibility.Select(x => x == Visibility.Visible ? Visibility.Hidden : Visibility.Visible).ToReactiveProperty();

            SetTable(SelectedYear.Value, SelectedMonth.Value);

            PrevMonthCommand.Subscribe(x =>
            {
                var selected = new DateTime(SelectedYear.Value, SelectedMonth.Value, 1);
                selected = selected.AddMonths(-1);
                SelectedYear.Value = selected.Year;
                SelectedMonth.Value = selected.Month;
                SetTable(SelectedYear.Value, SelectedMonth.Value);
            });

            NextMonthCommand.Subscribe(x =>
            {
                var selected = new DateTime(SelectedYear.Value, SelectedMonth.Value, 1);
                selected = selected.AddMonths(1);
                SelectedYear.Value = selected.Year;
                SelectedMonth.Value = selected.Month;
                SetTable(SelectedYear.Value, SelectedMonth.Value);
            });
        }

        private async void SetTable(int year, int month)
        {
            DateList.Clear();
            TableVisibility.Value = Visibility.Hidden;

            var collectList = new List<Collect>();
            var dt = new DateTime(year, month, 1);

            while (dt.Month == month)
            {
                var collect = new Collect();

                collect.Base = new DateTime(dt.Year, dt.Month, dt.Day);
                collect.Start = new DateTime(dt.Year, dt.Month, dt.Day, 5, 0, 0);
                dt = dt.AddDays(1);
                collect.End = new DateTime(dt.Year, dt.Month, dt.Day, 4, 59, 59);
                collect.Events = new List<EventLogEntry>();
                collectList.Add(collect);
            }

            await Task.Run(() =>
            {
                // 取得するイベントログ名
                var logName = "System";
                // コンピュータ名（"."はローカルコンピュータ）
                var machineName = ".";

                var idList = new List<long>() { StartEventID, EndEventID };

                // 指定したイベントログが存在しているか調べる
                if (EventLog.Exists(logName, machineName))
                {
                    // EventLogオブジェクトを作成する
                    var log = new EventLog(logName, machineName);

                    // ログエントリをすべて取得する
                    foreach (var entry in log.Entries.OfType<EventLogEntry>()
                        .Where(x => x.TimeWritten.Year == year && x.TimeWritten.Month == month && idList.Contains(x.InstanceId)))
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
            });


            foreach (var collect in collectList)
            {
                var model = new DateModel();

                model.Base = collect.Base;

                model.Start = collect.Events.Where(x => x.InstanceId == StartEventID).MinBy(y => y.TimeWritten)?.TimeWritten;

                model.End = collect.Events.Where(x => x.InstanceId == EndEventID).MaxBy(y => y.TimeWritten)?.TimeWritten;

                DateList.Add(model);
            }

            TableVisibility.Value = Visibility.Visible;
        }
    }
}
