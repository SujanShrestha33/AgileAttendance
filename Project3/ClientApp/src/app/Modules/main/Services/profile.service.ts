import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { AccountService } from '../../auth/Services/account.service';
import { getBaseUrl } from 'src/main';

@Injectable({
  providedIn: 'root'
})
export class ProfileService {
  a = getBaseUrl();
  private baseUrl = `${this.a}`;
  constructor(
    private http : HttpClient,
    private accountService : AccountService
  ) { }
  getToken(): string {
    return localStorage.getItem('token'); // Adjust the key if needed
  }
  updateProfile(updatedUserData: any): Observable<any> {
    const url = `${this.baseUrl}account/editprofile`;
    const token = this.getToken();
    console.log(token);

    // You can add headers and other options as needed
    // const httpOptions = {
    //   headers: new HttpHeaders({
    //     'Content-Type': 'application/json',
    //     'Authorization': `Bearer ${token}`
    //   })
    // };
    return this.http.post<any>(url, updatedUserData)
    .pipe(
      tap((response) => {
        if (response && response.email && response.displayName) {
          localStorage.setItem('email', response.email);
          localStorage.setItem('name', response.displayName);
        }
        if (response.token) {
          localStorage.setItem('token', response.token);
        }
        if(response){
          this.accountService.currentUserSource.next(response);
          // console.log(this.accountService.currentUserSource);
        }
      })
    )
  }

  changePassword(funBody : any){
    const token = this.getToken();
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`
      })
    };
    return this.http.post(`${this.baseUrl}account/change-password`, funBody, httpOptions)
  }

}
