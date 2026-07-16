namespace Fundo.Applications.Data.Models;

public class Loan : BaseEntity
{
    public string ApplicantName { get; set; } = string.Empty;

    /// <summary>Total amount originally requested.</summary>
    public decimal Amount { get; set; }

    /// <summary>Remaining balance still owed.</summary>
    public decimal CurrentBalance { get; set; }

    public LoanStatus Status { get; set; } = LoanStatus.Active;

    /// <summary>
    /// Applies a payment to the loan, reducing the current balance and
    /// flipping the status to <see cref="LoanStatus.Paid"/> once fully settled.
    /// </summary>
    public void ApplyPayment(decimal amount)
    {
        CurrentBalance -= amount;

        if (CurrentBalance <= 0)
        {
            CurrentBalance = 0;
            Status = LoanStatus.Paid;
        }
    }
}
