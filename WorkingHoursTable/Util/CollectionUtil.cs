using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkingHoursTable.Util
{
    public static class CollectionUtil
    {
        public static void AddAll<T>(this Collection<T> collection, IEnumerable<T> iEnumerable)
        {
            foreach (var item in iEnumerable)
            {
                collection.Add(item);
            }
        }
    }
}
