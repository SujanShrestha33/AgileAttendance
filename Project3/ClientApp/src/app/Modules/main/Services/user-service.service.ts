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

  getUserInfoLive(){
   return this.http.get<any>(`${this.userUrl}/GetUserInfoCZKEM`)
  }

  getMultipleDeviceUserInfo(body: any, pageNumber: number, pageSize: number): Observable<any> {
    const params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());

    return this.http.post<any>(
      `${this.userUrl}/GetUserInfoOfMultipleDevicesCZKEM`,
      body, // Send the request body here
      { params } // Send query parameters here
    );
  }
}
