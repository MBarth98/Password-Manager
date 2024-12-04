import { Component } from '@angular/core';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
})
export class RegisterComponent {
  email: string = '';
  password: string = '';
  confirmPassword: string = '';

  onRegister() {
    if (this.password === this.confirmPassword) {
      console.log('Register clicked:', { email: this.email, password: this.password });
    } else {
      console.error('Passwords do not match');
    }
  }
}
