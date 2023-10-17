import { Component, OnInit } from '@angular/core';
import { AccountService } from './Modules/auth/Services/account.service';


@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  title = 'ClientApp';

  constructor(private accountService: AccountService) {}

  ngOnInit(): void {
   
  }

  
}
