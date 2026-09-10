import { HttpInterceptorFn } from '@angular/common/http';
import { environment } from '../../environments/environment';

/**
 * Interceptor HTTP que injeta o cabeçalho `X-App-Token` em requisições
 * relativas a `/api` que ainda não o possuam.
 */
export const appTokenInterceptor: HttpInterceptorFn = (req, next) => {
  if (req.url.startsWith('/api') && !req.headers.has('X-App-Token')) {
    const cloned = req.clone({
      setHeaders: { 'X-App-Token': environment.appToken },
    });
    return next(cloned);
  }
  return next(req);
};