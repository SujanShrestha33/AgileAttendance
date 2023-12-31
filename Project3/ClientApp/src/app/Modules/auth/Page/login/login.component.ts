import { Component, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from 'src/app/Modules/auth/Services/account.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
  hide : boolean = true;
  returnUrl : string;
  submitted : boolean;
  loading : boolean;
  isLoading =  false;

  constructor (private fb : FormBuilder,
    // private authService : AuthService,
    private router : Router,
    private route : ActivatedRoute,
    private accountService : AccountService,
    private toastr : ToastrService
    ) {}

  ngOnInit(): void {
    if(this.accountService.currentUserSource.value) {
      this.router.navigate(['main/devices']);
    }
  }

  loginForm = this.fb.group({
    username : ['', Validators.required],
    password : ['', Validators.required]
  })

  get f() {
    return this.loginForm.controls;
  }

  submit() {
    this.isLoading = true;
    // console.log(this.loginForm.value);
    this.accountService.login(this.loginForm.value).subscribe(() => {
      // console.log('user logged in')
      this.isLoading = false;
      this.router.navigate(["main/devices"])
    },error =>{
      this.isLoading = false;
      // console.log(error);
      this.toastr.error('Invalid username or password, Please try again')
    })
    
  }

}
