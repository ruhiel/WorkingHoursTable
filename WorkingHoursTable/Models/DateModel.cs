using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WorkingHoursTable.Models
{
    public class DateModel : BindableBase
    {
        private DateTime _Base;
        public DateTime Base
        {
            get => _Base;
            set => SetProperty(ref _Base, value);
        }

        private DateTime? _Start;
        public DateTime? Start
        {
            get => _Start;
            set => SetProperty(ref _Start, value);
        }

        private DateTime? _End;
        public DateTime? End
        {
            get => _End;
            set => SetProperty(ref _End, value);
        }

        private DateTime? _StartView;
        public DateTime? StartView
        {
            get => _StartView;
            set => SetProperty(ref _StartView, value);
        }

        private DateTime? _EndView;
        public DateTime? EndView
        {
            get => _EndView;
            set => SetProperty(ref _EndView, value);
        }
    }
}
