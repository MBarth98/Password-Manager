import { Injectable } from '@angular/core';
import * as CryptoJS from 'crypto-js';
import { HttpClient } from '@angular/common/http';
import { AuthService } from './auth.service';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class PasswordService {
  private apiUrl = 'https://localhost:5000/api'; // Replace with your backend URL
  private masterKey: string = '';

  constructor(private http: HttpClient, private authService: AuthService) {}

  setMasterCredentials(email: string, masterPassword: string) {
    const hashedEmail = CryptoJS.SHA256(email).toString(CryptoJS.enc.Hex);
    const hashedPassword = CryptoJS.SHA256(masterPassword).toString(CryptoJS.enc.Hex);
    this.masterKey = hashedEmail + hashedPassword; // Combine hashes
  }

  getDecryptedPasswords() {
    if (!this.masterKey) {
      throw new Error('Master credentials are not set.');
    }

    const encryptedPasswords = this.authService.getEncryptedPasswords();
    return encryptedPasswords.map((entry) => {
      const encryptionKey = CryptoJS.PBKDF2(this.masterKey, entry.salt, {
        keySize: 256 / 32,
        iterations: 10000,
      }).toString();

      const decryptedBytes = CryptoJS.AES.decrypt(entry.encryptedPassword, encryptionKey);
      const decryptedPassword = decryptedBytes.toString(CryptoJS.enc.Utf8);

      return {
        website: entry.website,
        username: entry.username,
        password: decryptedPassword,
      };
    });
  }

  addPassword(website: string, username: string, password: string): Observable<any> {
    if (!this.masterKey) {
      throw new Error('Master credentials are not set.');
    }

    // Generate a unique salt
    const salt = CryptoJS.lib.WordArray.random(16).toString(CryptoJS.enc.Hex);

    // Derive the encryption key using PBKDF2
    const encryptionKey = CryptoJS.PBKDF2(this.masterKey, salt, {
      keySize: 256 / 32,
      iterations: 10000,
    }).toString();

    // Encrypt the password
    const encryptedPassword = CryptoJS.AES.encrypt(password, encryptionKey).toString();

    // Send the password to the backend
    const token = this.authService.getToken();
    const headers = { Authorization: `Bearer ${token}` };

    return this.http.post(`${this.apiUrl}/passwords`, { website, username, salt, encryptedPassword }, { headers });
  }
}
