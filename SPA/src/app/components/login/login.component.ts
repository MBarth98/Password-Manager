import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { PasswordService } from '../../services/password.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
})
export class LoginComponent {
  email: string = '';
  password: string = '';
  errorMessage: string = '';

  constructor(private authService: AuthService, private passwordService: PasswordService, private router: Router) {}

  login() {
    this.authService.login(this.email, this.password).subscribe({
      next: () => {
        this.passwordService.setMasterCredentials(this.email, this.password);
        this.router.navigate(['/passwords']);
      },
      error: (err) => {
        this.errorMessage = 'Login failed. Please check your credentials.';
      },
    });
  }
}
