import * as CryptoJS from 'crypto-js';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';


@Injectable({
  providedIn: 'root',
})
export class ApiService {
  private baseUrl = ''; // Replace with your backend URL

  constructor(private http: HttpClient) {}

  setMasterKey(key: string) {
    sessionStorage.setItem("key", key);
  }

  getMasterKey(): string {
    return sessionStorage.getItem("key") ?? "";
  }

  // Authentication
  register(email: string, password: string): Observable<any> {
    return this.http.post(`${this.baseUrl}/user/register`, { email, password });
  }

  login(email: string, password: string): Observable<{ token: string }> {
    return this.http.post<{ token: string }>(`${this.baseUrl}/user/login`, { email, password });
  }

  // Password Management
  addPassword(website: string, username: string, encryptedPassword: string, salt: string): Observable<any> {
    return this.http.post(`${this.baseUrl}/passwords`, { website, username, encryptedPassword, salt });
  }

  getPasswords(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/passwords`);
  }


  encryptPassword(plainPassword: string) {
    const masterKeyHex = this.getMasterKey();
    const passwordSalt = CryptoJS.lib.WordArray.random(16).toString();
    const iterations = 10000;
    const keySize = 256 / 32;
    const passwordKey = CryptoJS.PBKDF2(masterKeyHex, passwordSalt, { keySize, iterations });
    const encryptedPassword = CryptoJS.AES.encrypt(plainPassword, passwordKey.toString()).toString();

    return { encryptedPassword, salt: passwordSalt };
  }

  decryptPassword(encryptedPassword: string, salt: string) {
    const masterKeyHex = this.getMasterKey();
    const iterations = 10000;
    const keySize = 256 / 32;
    const passwordKey = CryptoJS.PBKDF2(masterKeyHex, salt, { keySize, iterations });
    const decryptedBytes = CryptoJS.AES.decrypt(encryptedPassword, passwordKey.toString());
    return decryptedBytes.toString(CryptoJS.enc.Utf8);
  }

  updatePassword(id: number, updateDto: any) {
    return this.http.put(`${this.baseUrl}/passwords/${id}`, updateDto);
  }

  deletePassword(id: number) {
    return this.http.delete(`${this.baseUrl}/passwords/${id}`);
  }

  generateRandomPassword(length: number = 16): string {
    const chars = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()';
    let result = '';
    const array = new Uint8Array(length);
    window.crypto.getRandomValues(array);
    for (let i = 0; i < length; i++) {
      result += chars[array[i] % chars.length];
    }
    return result;
  }
}
