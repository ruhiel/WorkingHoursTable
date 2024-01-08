using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace WorkingHoursTable.Util
{
    public class TransactionDispatchObservableCollection<T> : ObservableCollection<T>
    {
        /// <summary>
        /// コレクションディスパッチャー
        /// </summary>
        public Dispatcher Dispatch { get; set; }

        public DispatcherPriority Priority { get; set; } = DispatcherPriority.Background;

        private object TransactLock = new object();

        #region コンストラクタ

        public TransactionDispatchObservableCollection(Dispatcher Dispatch)
        {
            this.Dispatch = Dispatch;
        }
        public TransactionDispatchObservableCollection(Dispatcher Dispatch, IEnumerable<T> collection) : base(collection)
        {
            this.Dispatch = Dispatch;
        }

        #endregion

        /// <summary>
        /// 関連処理を排他制御しながら実行します。
        /// </summary>
        /// <param name="action"></param>
        public void TransactExcution(Action action)
        {
            lock (TransactLock)
            {
                action();
            }
        }
        /// <summary>
        /// 関連処理を排他制御しながら実行します。(非同期)
        /// </summary>
        public async Task TransactExcutionAsync(Action action)
        {
            await Task.Run(() =>
            {
                lock (TransactLock)
                {
                    action();
                }
            });
        }
        protected override void ClearItems()
        {
            TransactExcution(() => base.ClearItems());
        }
        protected override void InsertItem(int index, T item)
        {
            TransactExcution(() => base.InsertItem(index, item));
        }
        protected override void MoveItem(int oldIndex, int newIndex)
        {
            TransactExcution(() => base.MoveItem(oldIndex, newIndex));
        }
        protected override void SetItem(int index, T item)
        {
            TransactExcution(() => base.SetItem(index, item));
        }
        protected override void RemoveItem(int index)
        {
            TransactExcution(() => base.RemoveItem(index));
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (this.Dispatch.Thread == Thread.CurrentThread)
            {
                base.OnCollectionChanged(e);
            }
            else
            {
                Action<NotifyCollectionChangedEventArgs> changed = OnCollectionChanged;
                this.Dispatch.Invoke(changed, Priority, e);
            }
        }
    }
}
