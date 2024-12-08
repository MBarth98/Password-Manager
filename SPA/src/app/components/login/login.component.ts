import * as CryptoJS from 'crypto-js';
import { Component } from '@angular/core';
import { ApiService } from '../../services/api.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html'
})
export class LoginComponent {
  email = '';
  masterPassword = '';
  errorMessage = '';

  // Store the derived encryption key in a service or a global variable
  constructor(private apiService: ApiService, private router: Router) {}

  login() {
    this.apiService.login(this.email, this.masterPassword).subscribe({
      next: (response) => {
        localStorage.setItem('jwt_token', response.token);

        // Derive a stable master key from email + masterPassword
        // For example, we use PBKDF2 with the user's email as salt
        const stableSalt = this.email;
        const iterations = 10000;
        const keySize = 256 / 32; // 256-bit key

        // Derive the masterKey used for all password encryption/decryption
        const masterKey = CryptoJS.PBKDF2(this.email + this.masterPassword, stableSalt, {
          keySize,
          iterations
        }).toString();

        // Store masterKey in memory (e.g., in a global service)
        this.apiService.setMasterKey(masterKey);

        // Clear masterPassword from memory immediately
        this.masterPassword = '';

        this.router.navigate(['/home']);
      },
      error: () => {
        this.errorMessage = 'Login failed.';
      }
    });
  }
}
