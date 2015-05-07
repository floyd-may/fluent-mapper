using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PDMS.Client.AppServices.Caching.Drivers;

namespace PMDS.Client.UI.Operators
{
    public interface ICarrierEditor
    {
        Task<bool> EditCarrier(Guid driverId);

        Task<bool> AddCarrier();
    }
}