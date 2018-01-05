using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TZGCMS.Service.Systems
{
    public interface IBackupServices
    {
        List<object> SqlQuery(string query);
      
    }
    public class BackupServices: IBackupServices
    {
        readonly UnitOfWork _unitOfWork = new UnitOfWork();      
        public List<object> SqlQuery(string query)
        {
            return _unitOfWork.ExecuteQuery<object>(query).ToList();
        }     
    }
}
