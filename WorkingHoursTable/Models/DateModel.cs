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
    }
}
