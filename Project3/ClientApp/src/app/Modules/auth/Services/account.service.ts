import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, map } from 'rxjs';
import { User } from '../../main/Models/user';
import { Router } from '@angular/router';
import { getBaseUrl } from 'src/main';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  a = getBaseUrl();
  private loginUrl = `${this.a}`;
  public currentUserSource = new BehaviorSubject<User>(null);
  currentUser$ = this.currentUserSource.asObservable();

  constructor(private http : HttpClient, private router: Router) {
    // console.log(a);
    // console.log(environment.apiUrl)
    if(localStorage.getItem('token') != null
      || localStorage.getItem('email') != null
      || localStorage.getItem('username') != null
      )
    {
      const token = localStorage.getItem('token').toString();
      const email = localStorage.getItem('email');
      const username = localStorage.getItem('username');
      const user = {
        email: email,
        token: token,
        username: username
      }

      this.currentUserSource = new BehaviorSubject<User>(user);
      // console.log(this.currentUserSource);

      this.currentUser$ = this.currentUserSource.asObservable();
      // console.log(this.currentUser$);
  }
}

  getCurrentUserValue(){
    // console.log(this.currentUser$);
    // console.log(this.currentUserSource);

    return this.currentUserSource.value;
  }

  login(values: any){
    // console.log(this.loginUrl);
    return this.http.post(this.loginUrl + 'account/Login' ,values).pipe(
      map((user:User) =>{
        if(user){
          localStorage.setItem('email', user.email);
          localStorage.setItem('username', user.username);
          localStorage.setItem('token', user.token);
          this.currentUserSource.next(user);
        }
      })
    );
  }

  logout(){
    localStorage.removeItem('token');
    localStorage.removeItem('email');
    localStorage.removeItem('username');
    this.currentUserSource.next(null);
    this.router.navigateByUrl('/');
  }

  //for jwt-interceptor
  getToken(){
  return localStorage.getItem('token');
  }
}
