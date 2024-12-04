import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { tap } from 'rxjs/operators';
import { PasswordEntry } from '../models/model';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private apiUrl = 'https://your-backend-url/api'; // Replace with your backend URL
  private authToken: string = '';
  private encryptedPasswords: PasswordEntry[] = [];

  isLoggedIn$ = new BehaviorSubject<boolean>(false);

  constructor(private http: HttpClient) {}

  login(email: string, password: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/auth/login`, { email, password }).pipe(
      tap((response: any) => {
        this.authToken = response.token;
        this.encryptedPasswords = response.encryptedPasswords;
        this.isLoggedIn$.next(true);
      })
    );
  }

  logout(): void {
    this.authToken = '';
    this.encryptedPasswords = [];
    this.isLoggedIn$.next(false);
  }

  getEncryptedPasswords(): PasswordEntry[] {
    return this.encryptedPasswords;
  }

  getToken(): string {
    return this.authToken;
  }
}
