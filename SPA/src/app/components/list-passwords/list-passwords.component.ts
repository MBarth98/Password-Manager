import { Component, OnInit } from '@angular/core';
import { PasswordService } from '../../services/password.service';

@Component({
  selector: 'app-list-passwords',
  templateUrl: './list-passwords.component.html',
})
export class ListPasswordsComponent implements OnInit {
  passwords: { website: string; username: string; password: string }[] = [];

  constructor(private passwordService: PasswordService) {}

  ngOnInit() {
    try {
      this.passwords = this.passwordService.getDecryptedPasswords();
    } catch (error) {
      console.error('Failed to decrypt passwords:', error);
    }
  }
}
