import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpHandler, HttpEvent, HttpRequest } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthContextService } from 'src/app/services/auth-context.service';

@Injectable()
export class TokenInterceptor implements HttpInterceptor {

  constructor(public authCtx: AuthContextService) {
  }
  

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
      let token = localStorage.getItem("Token");
    if (token != null) {
      request = request.clone({
        setHeaders: {
          Authorization: 'Bearer ' + token,
          'Cache-Control': 'no-cache',
          Pragma: 'no-cache',
          Expires: 'Sat, 01 Jan 2000 00:00:00 GMT'
        }
      });
    }


    return next.handle(request);
  }
}
