import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, RouterStateSnapshot, UrlTree } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { observable, Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { AccountService } from '../_services/account.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor(private accountservice:AccountService,
    private toastr:ToastrService)
  { }

  canActivate(): Observable<boolean>{
    return this.accountservice.currentUser$.pipe(
      map(user=>{
        if(!user)
        {
          this.toastr.error("You shall not pass!");
        }
        return !!user;
      })
    );
  }
  
}