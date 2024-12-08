import {Injectable} from '@angular/core';
import {HttpEvent, HttpHandler, HttpHandlerFn, HttpInterceptor, HttpRequest} from '@angular/common/http';
import {Observable} from 'rxjs';


const baseUrl = 'http://localhost:5055/api'; // Define the base URL

export function baseUrlInterceptor(req: HttpRequest<unknown>, next: HttpHandlerFn) {

  // Clone the request and set the new base URL
  const cloned = req.clone({
    url: `${baseUrl}${req.url}` // Concatenate base URL with the request URL
  });

  return next(cloned); // Pass the cloned request to the next handler
}

