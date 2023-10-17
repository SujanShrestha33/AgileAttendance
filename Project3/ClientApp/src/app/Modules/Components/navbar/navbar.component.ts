import { Component } from '@angular/core';
import { User } from '../../main/Models/user';
import { Observable } from 'rxjs';
import { AccountService } from '../../auth/Services/account.service';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss']
})
export class NavbarComponent {
  currentUser$: Observable<User>;

  constructor(private accountService : AccountService) {}

  ngOnInit(){
    this.currentUser$ = this.accountService.currentUser$;
  }

  logout(){
    this.accountService.logout();
  }
  
}
