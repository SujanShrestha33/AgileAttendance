import { Component, OnInit } from '@angular/core';
import { NgOption } from '@ng-select/ng-select';
import IdleTimer from "../main/idle-timer.js";
import { AccountService } from '../auth/Services/account.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-main',
  templateUrl: './main.component.html',
  styleUrls: ['./main.component.scss']
})
export class MainComponent implements OnInit {
  timer = IdleTimer;

  constructor(private accountService: AccountService, private router: Router) {}
  ngOnInit(){
    if (localStorage.getItem("token")) {
      this.idleTime();
      this.refreshTokenTimer();
    }
  }

  refreshTokenTimer() {
    this.accountService.startRefreshTokenTimer()
  }

  idleTime() {
      this.timer = new IdleTimer({
          timeout: 2400, //expired after 40 min
          onTimeout: () => {
              this.accountService.logout()
              this.timer.cleanUp();
              alert("You have been logged out for being inactive")
              const returnModule = location.hash.substring(2);
              this.router.navigate(['/'], { queryParams: { returnUrl: returnModule } });
          }
      });
  }
}
