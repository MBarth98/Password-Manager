import { Component } from '@angular/core';
import { PasswordService } from '../../services/password.service';

@Component({
  selector: 'app-add-password',
  templateUrl: './add-password.component.html',
})
export class AddPasswordComponent {
  website = '';
  username = '';
  password = '';

  constructor(private passwordService: PasswordService) {}

  addPassword() {
    this.passwordService.addPassword(this.website, this.username, this.password);
    this.website = '';
    this.username = '';
    this.password = '';
  }
}
