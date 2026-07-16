import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../environments/environment';
import { Loan } from '../models/loan.model';

@Injectable({ providedIn: 'root' })
export class LoanService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/loans`;

  getLoans(): Observable<Loan[]> {
    return this.http.get<Loan[]>(this.baseUrl);
  }

  createLoan(payload: { applicantName: string; amount: number }): Observable<Loan> {
    return this.http.post<Loan>(this.baseUrl, payload);
  }
}
