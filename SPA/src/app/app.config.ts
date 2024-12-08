import {ApplicationConfig, provideZoneChangeDetection} from '@angular/core';
import {provideRouter} from '@angular/router';

import {routes} from './app.routing.module';
import {
  provideHttpClient,
  withInterceptors,
} from '@angular/common/http';
import {baseUrlInterceptor} from './interceptors/base-url-interceptor';
import { authInterceptor } from './interceptors/auth-interceptor';


export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({eventCoalescing: true}),
    provideRouter(routes), // Your existing router configuration
    provideHttpClient(
      withInterceptors([
        baseUrlInterceptor,
        authInterceptor
      ]),
    ), // Add HttpClientModule
  ]
};
