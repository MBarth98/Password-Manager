import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../services/api.service';

@Component({
  selector: 'app-password-list',
  templateUrl: './password-list.component.html'
})
export class PasswordListComponent implements OnInit {
  passwords: {
    id: number;
    website: string;
    username: string;
    encryptedPassword: string;
    salt: string;
    showPassword: boolean;
    decryptedPassword?: string;
  }[] = [];

  constructor(private apiService: ApiService) {}

  ngOnInit() {
    this.loadPasswords();
  }

  loadPasswords() {
    this.apiService.getPasswords().subscribe({
      next: (entries) => {
        this.passwords = entries.map(entry => ({
          id: entry.id,
          website: entry.website,
          username: entry.username,
          encryptedPassword: entry.encryptedPassword,
          salt: entry.salt,
          showPassword: false
        }));
      },
      error: (err) => {
        console.error('Failed to load passwords:', err);
      }
    });
  }

  toggleShowPassword(index: number) {
    const pwd = this.passwords[index];

    // Check if master key is still available
    const masterKeyHex = this.apiService.getMasterKey();
    if (!masterKeyHex) {
      // Session expired, cannot decrypt
      alert('Your session has expired. Please log in again to view this password.');
      pwd.showPassword = false;
      return;
    }

    if (!pwd.showPassword) {
      if (!pwd.decryptedPassword) {
        const decrypted = this.apiService.decryptPassword(pwd.encryptedPassword, pwd.salt);
        pwd.decryptedPassword = decrypted;
      }
      pwd.showPassword = true;
    } else {
      pwd.showPassword = false;
      pwd.decryptedPassword = undefined;
    }
  }

  deletePassword(index: number) {
    const pwd = this.passwords[index];
    this.apiService.deletePassword(pwd.id).subscribe({
      next: () => {
        this.passwords.splice(index, 1); // Remove from local array
      },
      error: (err) => {
        console.error('Failed to delete password:', err);
      }
    });
  }

  editMode = false;
  editingPassword: any = { id: 0, website: '', username: '', password: '' };

  editPassword(pwd: any) {
    this.editingPassword = { ...pwd }; // copy current values
    this.editMode = true;
  }

  // Called when user saves the changes
  savePasswordChanges() {
    // Re-encrypt the updated password
    const { encryptedPassword, salt } = this.apiService.encryptPassword(this.editingPassword.password);

    const updateDto = {
      website: this.editingPassword.website,
      username: this.editingPassword.username,
      encryptedPassword,
      salt
    };

    this.apiService.updatePassword(this.editingPassword.id, updateDto).subscribe({
      next: () => {
        this.editMode = false;
        this.loadPasswords();
      },
      error: (err) => {
        console.error('Failed to update password:', err);
      }
    });
  }

  // Called if user cancels
  cancelEdit() {
    this.editMode = false;
  }

  generateRandomPasswordForEdit() {
    this.editingPassword.password = this.apiService.generateRandomPassword();
  }
}
