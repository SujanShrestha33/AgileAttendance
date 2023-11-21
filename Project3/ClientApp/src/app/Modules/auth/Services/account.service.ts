import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, map } from 'rxjs';
import { User } from '../../main/Models/user';
import { Router } from '@angular/router';
import { getBaseUrl } from 'src/main';

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
      const expiresAt = localStorage.getItem('expiresAt');
      const user = {
        email: email,
        token: token,
        username: username,
        expiresAt:expiresAt,
      }

      this.currentUserSource = new BehaviorSubject<User>(user);
      this.currentUser$ = this.currentUserSource.asObservable();
  }
}

  getCurrentUserValue(){   
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
    localStorage.clear();
    // localStorage.removeItem('token');
    // localStorage.removeItem('email');
    // localStorage.removeItem('username');
    this.currentUserSource.next(null);
    this.stopRefreshTokenTimer();
    this.router.navigateByUrl('/');
  
  }

  //for jwt-interceptor
  getToken(){
    return localStorage.getItem('token');
  }

  refreshToken() {
    return this.http.post(this.loginUrl + 'account/refresh-token', this.currentUserSource.value)
      .pipe(map((user: User) => {
        localStorage.setItem('token', user.token);
        this.currentUserSource.next(user);
        this.startRefreshTokenTimer();
        return user;
      }));
  }
  
  // helper methods
  private refreshTokenTimeout;
  
  startRefreshTokenTimer() {
    clearTimeout(this.refreshTokenTimeout);
    // set a timeout to refresh the token a minute before it expires
    const expires = new Date(this.currentUserSource.value.expiresAt);
    const timeout = expires.getTime() - Date.now() - (120 * 1000);
  
    this.refreshTokenTimeout = setTimeout(() => this.refreshToken().subscribe(), timeout);
  }
  
  private stopRefreshTokenTimer() {
    clearTimeout(this.refreshTokenTimeout);
  }
  
  sessionExpire() {
    if (localStorage.getItem("token") == null) {
      this.router.navigateByUrl("/");
    }
  }
}
