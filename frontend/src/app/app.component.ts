import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';

import { Loan } from './models/loan.model';
import { LoanService } from './services/loan.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatTableModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    MatFormFieldModule,
    MatInputModule,
  ],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent implements OnInit {
  private readonly loanService = inject(LoanService);

  readonly displayedColumns = ['applicantName', 'amount', 'currentBalance', 'status'];
  readonly loans = signal<Loan[]>([]);
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);

  readonly showForm = signal(false);
  readonly submitting = signal(false);
  readonly formError = signal<string | null>(null);
  newApplicantName = '';
  newAmount: number | null = null;

  ngOnInit(): void {
    this.loadLoans();
  }

  loadLoans(): void {
    this.loading.set(true);
    this.error.set(null);

    this.loanService.getLoans().subscribe({
      next: (loans) => {
        this.loans.set(loans);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Unable to load loans. Please make sure the API is running.');
        this.loading.set(false);
      },
    });
  }

  toggleForm(): void {
    this.showForm.update((open) => !open);
    this.formError.set(null);
  }

  createLoan(): void {
    const applicantName = this.newApplicantName.trim();
    const amount = this.newAmount;

    if (!applicantName || amount == null || amount <= 0) {
      this.formError.set('Enter an applicant name and an amount greater than 0.');
      return;
    }

    this.submitting.set(true);
    this.formError.set(null);

    this.loanService.createLoan({ applicantName, amount }).subscribe({
      next: () => {
        this.submitting.set(false);
        this.newApplicantName = '';
        this.newAmount = null;
        this.showForm.set(false);
        this.loadLoans();
      },
      error: () => {
        this.submitting.set(false);
        this.formError.set('Could not create the loan. Please try again.');
      },
    });
  }
}
