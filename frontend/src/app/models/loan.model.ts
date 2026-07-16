export type LoanStatus = 'Active' | 'Paid';

export interface Loan {
  id: string;
  applicantName: string;
  amount: number;
  currentBalance: number;
  status: LoanStatus;
  createdAt: string;
  updatedAt: string | null;
}
