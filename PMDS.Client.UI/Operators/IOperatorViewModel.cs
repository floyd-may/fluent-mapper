using System;

namespace PMDS.Client.UI.Operators
{
    public interface IOperatorViewModel
    {
        string LastName { get; }
        string FirstName { get; }
        string EmployeeId { get; }
        string Phone { get; }
        bool IsActive { get; }
        byte[] Photo { get; }
        bool IsHazmatCertified { get; }

        bool PassesFilter(string filterText);

        void Edit();

        event EventHandler Edited;
    }

    public interface ICarrierViewModel : IOperatorViewModel
    {
    }

    public interface IDriverViewModel : IOperatorViewModel
    {}
}