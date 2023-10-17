import { Injectable } from '@angular/core';
import {
  HttpInterceptor,
  HttpRequest,
  HttpHandler,
  HttpEvent,
} from '@angular/common/http';
import { Observable } from 'rxjs';
import { AccountService } from '../../auth/Services/account.service';
import { Token } from '@angular/compiler';

@Injectable()
export class JwtInterceptor implements HttpInterceptor {

  constructor(private accountService: AccountService){}

 intercept(
    request: HttpRequest<any>,
    next: HttpHandler
    ): Observable<HttpEvent<any>> {

      const token = this.accountService.getToken();

      if(token){
        request = request.clone({
          setHeaders :{
            Authorization:`Bearer ${token}`
          }
        })
      }
      // console.log(request);
     return next.handle(request);
 }
}
