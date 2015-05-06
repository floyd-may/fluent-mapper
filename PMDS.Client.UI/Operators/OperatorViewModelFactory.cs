using System;
using PDMS.Client.AppServices.Caching.Drivers;

namespace PMDS.Client.UI.Operators
{
    public sealed class OperatorViewModelFactory<TOperator, TEquipment> : IOperatorViewModelFactory<TOperator, TEquipment>
        where TOperator : ICacheableDriver<TEquipment>
    {
        private readonly Func<ICacheableDriver<TEquipment>, IDriverViewModel> _driverFactory;
        private readonly Func<ICacheableDriver<TEquipment>, ICarrierViewModel> _carrierFactory;

        public OperatorViewModelFactory(
            Func<ICacheableDriver<TEquipment>, IDriverViewModel> driverFactory,
            Func<ICacheableDriver<TEquipment>, ICarrierViewModel> carrierFactory
            )
        {
            _driverFactory = driverFactory;
            _carrierFactory = carrierFactory;
        }

        public IOperatorViewModel Create(ICacheableDriver<TEquipment> @operator)
        {
            if (@operator.IsDriverRecord)
                return _driverFactory(@operator);

            return _carrierFactory(@operator);
        }
    }
}