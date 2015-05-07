using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PDMS.Client.AppServices.Caching.Drivers;

namespace PMDS.Client.UI.Operators
{
    public interface IDriverEditor
    {
        Task<bool> EditDriver(Guid driverId);

        Task<bool> AddDriver();
    }
}