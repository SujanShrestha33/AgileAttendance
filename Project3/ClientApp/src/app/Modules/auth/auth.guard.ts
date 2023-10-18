import { ActivatedRouteSnapshot, CanActivate, CanActivateFn, Router, RouterStateSnapshot } from '@angular/router';
import { AccountService } from './Services/account.service';
import { Observable, map } from 'rxjs';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})

export class authGuard implements CanActivate {

  constructor(private router: Router, private accountService:  AccountService){}
      canActivate(): boolean {
        var user = this.accountService.currentUserSource;
        if (user.value != null) {
          // console.log("tr");
          return true;
        }
        else {
          // console.log("f")
          this.router.navigate(['/auth/login']);
          return false;
        }
      }


  }
