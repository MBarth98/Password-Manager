import {HttpHandlerFn, HttpRequest} from '@angular/common/http';

export function authInterceptor(req: HttpRequest<unknown>, next: HttpHandlerFn) {

  const url = req.url;
  // If url is login or register, forward without token
  if (url.endsWith('/login') || url.endsWith('/register')) {
    return next(req);
  }

  // Get the token from the local-storage
  const token = localStorage.getItem('jwt_token');
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
