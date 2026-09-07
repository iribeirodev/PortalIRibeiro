import { HttpInterceptorFn } from '@angular/common/http';
import { environment } from '../../environments/environment';

export const appTokenInterceptor: HttpInterceptorFn = (req, next) => {
  if (req.url.startsWith('/api') && !req.headers.has('X-App-Token')) {
    const cloned = req.clone({
      setHeaders: { 'X-App-Token': environment.appToken },
    });
    return next(cloned);
  }
  return next(req);
};