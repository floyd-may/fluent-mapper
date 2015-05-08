using System;
using PDMS.Client.AppServices.Caching.Drivers;

namespace PMDS.Client.UI.Operators
{
    public sealed class OperatorViewModelFactory<TOperator, TEquipment> : IOperatorViewModelFactory<TOperator, TEquipment>
        where TOperator : ICacheableDriver<TEquipment>
    {
        private readonly IDriverViewModelFactory<TEquipment> _driverFactory;
        private readonly ICarrierViewModelFactory<TEquipment> _carrierFactory;

        public OperatorViewModelFactory(
            IDriverViewModelFactory<TEquipment> driverFactory,
            ICarrierViewModelFactory<TEquipment> carrierFactory
            )
        {
            _driverFactory = driverFactory;
            _carrierFactory = carrierFactory;
        }

        public IOperatorViewModel Create(ICacheableDriver<TEquipment> @operator)
        {
            if (@operator.IsDriverRecord)
                return _driverFactory.Create(@operator);

            return _carrierFactory.Create(@operator);
        }
    }
}