using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndoCash.MonitorService.Utils.Models
{
    public class MyService
    {
        private readonly NamedConcurrentDictionary<string, DateTime> _dicActionRec;
        private readonly NamedConcurrentDictionary<string, DateTime> _dicServiceRec;
        private readonly UnitHelper _unitHelper;
        public MyService()
        {
            
        }
        public MyService(
            NamedConcurrentDictionary<string, DateTime> dicActionRec,
            NamedConcurrentDictionary<string, DateTime> dicServiceRec,
            UnitHelper unitHelper)
        {
            _dicActionRec = dicActionRec;
            _dicServiceRec = dicServiceRec;
            _unitHelper = unitHelper;
        }
        public bool RemoveDicActionRecByKey(string key)
        {
            bool rst = false;
            if (_dicActionRec.ContainsKey(key))
            {
                DateTime dttm = DateTime.Now;
                _dicActionRec.Remove(key, out dttm);
                rst = true;
            }
            return rst;
        }
        public void SetDicActionRecKeyValue(string key, DateTime value)
        {
            if (_dicActionRec.ContainsKey(key))
            {
                _dicActionRec[key] = value;
            }
            else
            {
                _dicActionRec.TryAdd(key, value);
            }
        }
        public DateTime? GetDicActionRecValueByKey(string key)
        {
            if (_dicActionRec.ContainsKey(key))
                return _dicActionRec[key];
            return null;
        }
        public bool RemoveDicServiceRecByKey(string key)
        {
            bool rst = false;
            if (_dicServiceRec.ContainsKey(key))
            {
                DateTime dttm = DateTime.Now;
                _dicServiceRec.TryRemove(key, out dttm);
                rst = true;
            }
            return rst;
        }
        public void SetDicServiceRecKeyValue(string key, DateTime value)
        {
            if (_dicServiceRec.ContainsKey(key))
            {
                _dicServiceRec[key] = value;
            }
            else
            {
                _dicServiceRec.TryAdd(key, value);
            }
        }
        public virtual DateTime? GetDicServiceRecValueByKey(string key)
        {
            var list = _dicServiceRec.ToList();
            foreach (var item in list)
            {
                if (item.Key.IndexOf(key) >= 0)
                {
                    return item.Value;
                }
            }
            return null;
        }
    }

}
