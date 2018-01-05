using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TZGCMS.Data.Entity.Logs;

namespace TZGCMS.Service.Systems
{
    public interface ILogServices
    {
        IEnumerable<Log> SearchLogs(int pageIndex, int pageSize, DateTime? startDate, DateTime? expireDate,
            string level, out int count);
        bool RemoveAll();
        bool Delete(int id);
    }
    public class LogServices : ILogServices
    {
        readonly UnitOfWork _unitOfWork = new UnitOfWork();

        public IEnumerable<Log> SearchLogs(int pageIndex, int pageSize, DateTime? startDate, DateTime? expireDate,
            string level,out int count)
        {
            var logs = _unitOfWork.LogRepository.Table.AsQueryable();

            if (startDate != null)
            {
                logs = logs.Where(l => l.Date >= startDate);
            }

            if (expireDate != null)
            {
                logs = logs.Where(l => l.Date <= expireDate);
            }

            if (!string.IsNullOrEmpty(level))
                logs = logs.Where(l => l.Level == level);

            count = logs.Count();

            var result = logs
                .OrderByDescending(l => l.Date)
                .Skip(pageIndex * pageSize).Take(pageSize).AsEnumerable();

            return result;
        }

        public bool RemoveAll()
        {
           return _unitOfWork.LogRepository.Delete(_unitOfWork.LogRepository.Table.AsEnumerable());
        }
        public bool Delete(int id)
        {
          return  _unitOfWork.LogRepository.Delete(id);
        }
    }
}
