import {inject, Injectable} from '@angular/core';
import {HttpHandler, HttpHandlerFn, HttpInterceptor, HttpRequest} from '@angular/common/http';
import {AuthService} from '../services/auth.service';


export function authInterceptor(req: HttpRequest<unknown>, next: HttpHandlerFn) {

  const authService = inject(AuthService);

  const url = req.url;
  // If url is login or register, forward without token
  if (url.endsWith('/login') || url.endsWith('/register')) {
    return next(req);
  }


  // Get the token from the AuthService
  const token = authService.getToken();

  // Clone the request and set the new header
  if (token) {
    const cloned = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}` // Add the token in Authorization header
      }
    });
    return next(cloned); // Pass the cloned request instead of the original request to the next handler
  }

  return next(req); // If no token, just pass the original request
}
