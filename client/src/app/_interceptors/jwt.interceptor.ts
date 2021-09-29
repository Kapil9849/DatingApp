import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable } from 'rxjs';
import { AccountService } from '../_services/account.service';
import { User } from '../_models/user';
import { take } from 'rxjs/operators';

@Injectable()
export class JwtInterceptor implements HttpInterceptor {

  constructor(private service: AccountService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    let currentuser:User;

    this.service.currentUser$.pipe(take(1)).subscribe(user=>
      {
        currentuser=user;
        if(currentuser)
        {
          request=request.clone({
            setHeaders:{
              Authorization: 'Nita '+currentuser.token
            }
          })
        }
      })
    return next.handle(request);
  }
}
