import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { getBaseUrl } from 'src/main';

@Injectable({
  providedIn: 'root'
})
export class UserServiceService {
  a = getBaseUrl();
  private userUrl = `${this.a}user`;

  constructor(private http : HttpClient) { }

  getUserInfo(){
    return this.http.get<any>(`${this.userUrl}/GetUserInfo`)
   }

  getUserInfoLive(): Observable<any> {
      return this.http.get<any>(`${this.userUrl}/GetUserInfoCZKEM`)
  }

  getMultipleDeviceUserInfo(body: any): Observable<any> {
   
    return this.http.post<any>(
      `${this.userUrl}/GetUserInfoOfMultipleDevicesCZKEM`,
      body, // Send the request body here
       // Send query parameters here
    );
  }
}
