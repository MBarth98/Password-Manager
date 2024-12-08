import { Component } from '@angular/core';
import { ApiService } from '../../services/api.service';
import * as CryptoJS from 'crypto-js';

@Component({
  selector: 'app-add-password',
  templateUrl: './add-password.component.html'
})
export class AddPasswordComponent {
  website = '';
  username = '';
  password = '';
  errorMessage = '';

  constructor(private apiService: ApiService) {}

  addPassword() {

    if (this.website === "" || this.username === "") {
      this.errorMessage = 'missing values';
      return;
    }

    const masterKeyHex = this.apiService.getMasterKey();
    if (!masterKeyHex) {
      this.errorMessage = 'No master key available. Please re-login.';
      return;
    }

    // Generate a unique random salt for this password entry
    const passwordSalt = CryptoJS.lib.WordArray.random(16).toString();

    // Derive a per-password key using PBKDF2 with masterKey as the password and passwordSalt as the salt
    const iterations = 10000;
    const keySize = 256 / 32;
    const passwordKey = CryptoJS.PBKDF2(masterKeyHex, passwordSalt, {
      keySize,
      iterations
    });

    // Encrypt the plaintext password with AES
    const encrypted = CryptoJS.AES.encrypt(this.password, passwordKey.toString()).toString();

    // Send encrypted password and salt to backend
    this.apiService.addPassword(this.website, this.username, encrypted, passwordSalt).subscribe({
      next: () => {
        alert('Password added successfully!');
        this.website = '';
        this.username = '';
        this.password = '';
      },
      error: () => {
        this.errorMessage = 'Failed to add password.';
      }
    });
  }

  generateRandomPassword() {
    this.password = this.apiService.generateRandomPassword();
  }
}
